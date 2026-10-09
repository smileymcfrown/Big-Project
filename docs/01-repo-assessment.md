# 1. What's in the two old repos

Surveyed on 2026-10-09. Both projects were made in **Unity 2021.3.1f1** between April and July 2022.

| | **Big-Project** (this repo) | **NewVRDumpling** |
|---|---|---|
| Active | 4 Apr – 16 May 2022 (code), art until 2 Jul 2022 | 15 – 22 Jul 2022 |
| People (commit authors) | Lara Bannerman, Kieran, SaoirseChar (art) | Lala-banners (Lara), Kieran, smileymcfrown |
| Networking | **Mirror 32.1.4** (copied into `Assets/Mirror`) | Third-party networking (not carried forward) |
| VR | SteamVR Unity Plugin + OpenVR XR plugin 1.1.4, imported but **not used in any of our scenes** | Oculus Integration (~700 MB), OpenXR 1.3.1, Oculus XR 3.0.2, SteamVR plugin, plus third-party VR packages |
| Asymmetric VR + PC working? | **No.** No VR rig in any project scene | Lara's networked experiments ("It works! VR and non VR player in same scene", 15 Jul). Not your local co-op version |
| Our own scripts | ~13 (lobby, spawning, movement, camera, wrist UI) | ~8 of ours, plus Kieran's Modular Character Controller (~25 scripts) |
| Art | 32 FBX props in `Assets/Models` | 44 FBX in `Assets/Kitchen Models` (**a superset**: adds the kitchen shell, pipes, wok, stockpot, steamer, Gina's veg), `Model/Dumpling.fbx`, `Kitchen_Populated.prefab` |

## Which repo got asymmetric play working?

**Neither of these two holds your version.** You got asymmetric VR/PC **4-player local co-op** working yourself, without a paid template, and that work isn't in Big-Project or NewVRDumpling, so it must be in another repo.

- **Big-Project** went furthest on *structured* development (a host/join lobby with ready-up, spawn points, a custom NetworkManager, art pipeline, naming conventions), but it never had a VR player.
- The asymmetric commits in **NewVRDumpling** ("It works! VR and non VR player in same scene", 15 Jul) are Lala-banners' (Lara's) networked experiments, built on third-party packages that the rebuild doesn't use.

It doesn't change the recommendation: everything gets rebuilt, and the new tutorial's Part 1 is exactly a local co-op asymmetric build.

## Big-Project: what works and what's broken at HEAD

Working (according to the commit history):

- Host / join by IP, name entry saved to PlayerPrefs, lobby with ready-up and a leader-only Start button (`CustomNetworkManager`, `PlayerLobby`, `JoinLobbyMenu`, `MainMenu`, `PlayerNameInput`).
- Moving from lobby to game scene and spawning players at ordered spawn points (`PlayerSpawnSystem`, `SpawnPoint`).
- Client-authoritative networked movement with the new Input System and a Cinemachine camera (`DumplingMovementCtrl`, `PlayerCamCtrl`).

Broken right now (found while reading the code):

1. **The lobby can't be joined.** Commit `9004468 "Renamed stuff"` renamed `Lara_Lobby.unity` → `Lobby.unity` and `Lara_Game.unity` → `Game.unity`, but the NetworkManager in the Lobby scene still has `menuScene: Lara_Lobby`, so `OnServerConnect` disconnects every client and `OnServerAddPlayer` never adds a lobby player.
2. **Start Game can't change scene.** `CustomNetworkManager.StartGame()` calls `ServerChangeScene("Lara_Game")` and checks `StartsWith("Lara_Game")`; that scene no longer exists.
3. `LaraTutorials/Prefabs.meta` has no folder (prefabs were moved to `Resources/SpawnablePrefabs`). Harmless, but Unity will complain.
4. `VrWristUI.Update()` calls `onClick.AddListener` **every frame**, so each button press would run thousands of listeners after a minute. The tutorial uses this as a "then vs now" example.
5. The SteamVR plugin was imported (`ef90359`) but no scene we made uses it; `WristUITest.unity` is a flat canvas.

## NewVRDumpling: other things worth knowing

- Lara's experiments worth remembering as ideas: multi-display output for VR + PC on one machine (`ActivateMultiDisplays.cs`), dumpling selection screen (`DumplingSelection.cs`, `LoadDumpling.cs`), cutting a cucumber into physics slices (`Cooking/Cut.cs`).
- Kieran's Modular Character Controller had task/quest scaffolding and a `LightSwitch` task, the first seed of the sabotage idea, plus `ThrowObject` for throwing dumplings.

## What's worth salvaging

| Keep | Why |
|---|---|
| **Art from NewVRDumpling** (`Kitchen Models/`, `Model/Dumpling.fbx`, `Kitchen/Kitchen_Populated.prefab` as a layout reference) | Original team art; it's the superset of Big-Project's models |
| Game design ideas: lobby → kitchen flow, sabotage tasks, dumpling selection, cutting | Still good ideas |
| Big-Project's lobby code, *as reading material* | The pattern (events for connect/disconnect, leader starts game, spawn points) carries over to Netcode for GameObjects |

| Don't carry forward | Why |
|---|---|
| SteamVR Unity Plugin, OpenVR XR plugin, Oculus Integration | All superseded by **OpenXR** (see the upgrade doc) |
| Mirror 32 and any other networking package | Replaced by Netcode for GameObjects in the new tutorial |
| Any paid or third-party VR/editor packages | Not used. XR Interaction Toolkit 3 now covers grabbing, sockets, poke and UI |
| Built-in render pipeline materials | Convert or remake as URP materials |
