# Consoleless
A BepInEx mod that prevents [Console](https://github.com/iiDk-the-actual/Console) from sending telemetry data and accessing the internet (mostly).

This currently blocks a majority of endpoints on iidk.online, but not the friend system.

and yes this code is messy please fix it thank you

# Info
II's menu sends telemetry data every time you join a room, this includes your:
- Room name
- Nickname
- Player ID
- Room visibility
- Room's playerlist
- Room's player count
- Gamemode

There is no way to disable this as far as I'm aware in his menu. This is the backbone of his tracker afaik.

# WARNING
Due to the nature of this mod, it ***WILL*** break some **(if not most)** features of mods that include console or that need to contact iidk's server data or the hamburbur servers. I am not responsible for any incidents that occur because of this.
