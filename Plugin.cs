using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace Consoleless
{
    [BepInPlugin("BrokenStone.Consoleless", "Consoleless", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony("BrokenStone.Consoleless");
            harmony.PatchAll();
            Debug.Log("[Consoleless] Initialized");
        }
    }

    public class Constants
    {
        public static List<string> BlockedUrls = new List<string>()
        {
            "https://iidk.online/serverdata",
            "https://iidk.online/telemetry",
            "https://iidk.online/syncdata",
            "https://iidk.online/reportban",
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
            if (Constants.BlockedUrls.Any(blocked => __instance.url.Contains(blocked)))
            {
                Debug.Log($"[Consoleless] Blocked {__instance.url}");
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
            if (Constants.BlockedUrls.Any(blocked => requestUri.Contains(blocked)))
            {
                Debug.Log($"[Consoleless] Blocked {requestUri}");
                __result = Task.FromResult(new byte[0]);
                return false;
            }
            return true;
        }
    }
}
