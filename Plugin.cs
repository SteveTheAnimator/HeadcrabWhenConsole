using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using GPhys.Types.Objects;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace HeadcrabWhenConsole
{
    [BepInPlugin("Rhosyn.HeadcrabWhenConsole", "HeadcrabWhenConsole", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public ConfigEntry<bool> DoCanister { get; set; }
        public ConfigEntry<HeadcrabType> HeadcrabType { get; set; }

        public void Awake()
        {
            var harmony = new Harmony("Rhosyn.HeadcrabWhenConsole");
            harmony.PatchAll();

            DoCanister = Config.Bind("General", "DoCanister", false, "Whether to spawn a headcrab canister when blocking requests.");
            HeadcrabType = Config.Bind("General", "HeadcrabType", GPhys.Types.Objects.HeadcrabType.Normal, "The type of headcrab to spawn.");

            Instance = this;
            Debug.Log("[HeadcrabWhenConsole] Initialized");
        }

        public void SpawnCrab()
        {
            if (!Plugin.Instance.DoCanister.Value)
                GPhys.Plugin.Instance.SpawnHeadcrab(GorillaTagger.Instance.transform.position + Vector3.up * 2, Quaternion.identity, Plugin.Instance.HeadcrabType.Value);
            else
                GPhys.Plugin.Instance.SpawnHeadcrabCanister(GorillaTagger.Instance.transform.position, Plugin.Instance.HeadcrabType.Value);
        }

        public static void TestSendMessage()
        {
            HttpClient client = new HttpClient();
            string url = Constants.BlockedUrls[Random.Range(0, Constants.BlockedUrls.Count)];
            // send the request :3
            var response = client.GetByteArrayAsync(url).Result;
            Debug.Log($"[HeadcrabWhenConsole] TestSendMessage response length: {response.Length}");
        }
    }

    public class Constants
    {
        public static List<string> BlockedUrls = new List<string>()
        {
            "https://iidk.online/",
            "https://raw.githubusercontent.com/iiDk-the-actual/Console",
            "https://data.hamburbur.org",
            "https://files.hamburbur.org"
        };
    }

    [HarmonyPatch(typeof(UnityWebRequest), nameof(UnityWebRequest.SendWebRequest))]
    public class UnityWebRequestPatch
    {
        [HarmonyPrefix]
        static bool Prefix(UnityWebRequest __instance)
        {
            if (Constants.BlockedUrls.Any(blocked => __instance.url.StartsWith(blocked)))
            {
                Debug.Log($"[HeadcrabWhenConsole] Blocked {__instance.url}");
                Plugin.Instance.SpawnCrab();
                __instance.url = null;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HttpClient), nameof(HttpClient.GetByteArrayAsync), new[] { typeof(string) })]
    public class HttpClientPatch
    {
        [HarmonyPrefix]
        static bool Prefix(string requestUri, ref Task<byte[]> __result)
        {
            if (Constants.BlockedUrls.Any(blocked => requestUri.StartsWith(blocked)))
            {
                Debug.Log($"[HeadcrabWhenConsole] Blocked {requestUri}");
                Plugin.Instance.SpawnCrab();
                __result = Task.FromResult(new byte[0]);
                return false;
            }
            return true;
        }
    }
}

