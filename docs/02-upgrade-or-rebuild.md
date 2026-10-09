# 2. Upgrade the old project, or start again?

**Recommendation: start a new Unity 6.3 LTS project and bring the art across.** Don't try to upgrade either repo in place.

The new tutorial (`docs/tutorial/dumpling-kitchen-tutorial.html`) rebuilds the game from scratch with current tools. This page explains why, and covers the one thing worth carrying over: the art.

## Versions this was checked against (October 2026)

| Thing | 2022 project | Now | Source |
|---|---|---|---|
| Unity editor | 2021.3.1f1 | **Unity 6.3 LTS** (6000.3.x; patches still shipping). Unity 6.7 (next LTS) is in beta | unity.com release pages |
| VR runtime layer | SteamVR plugin + OpenVR XR plugin, Oculus Integration | **OpenXR plugin 1.18** (`com.unity.xr.openxr`) | Unity package registry |
| VR interaction | BNG VRIF (paid), SteamVR Interaction System | **XR Interaction Toolkit 3.6** | Unity package registry |
| Networking | Mirror 32, Photon PUN 2 | **Netcode for GameObjects 2.13** (3.x needs Unity 6.7) | Unity package registry |
| Internet play | Photon Cloud | **Unity Relay via the Multiplayer Services package 2.4 (Sessions API)** | Unity package registry |
| Input | Input System 1.3 | Input System 1.20 (now the default) | Unity package registry |
| Rendering | Built-in pipeline | **URP** (the default for new projects) | |

## Why upgrading in place is a bad deal

### 1. The VR layer you used is gone, not just old

Both repos talk to headsets through SDKs that have been replaced:

- **SteamVR Unity Plugin / OpenVR**: Valve's OpenVR XR plugin was a stop-gap that has barely changed in years, and SteamVR's own recommended path for new projects is OpenXR. Every `SteamVR_*` component, the SteamVR Input action JSON in `StreamingAssets/SteamVR`, and the `SteamVR_SteamVR_dumpling` bindings folder would need removing.
- **Oculus Integration** (NewVRDumpling): Meta deprecated this monolithic package in 2023 and split it into the Meta XR SDKs. Code that calls `OVRPlugin`, `OVRCameraRig` and `OVRGrabbable` (all of the asymmetric template) no longer exists in that form.
- **OpenXR** is now the single standard. One build runs on a Quest 2 through **Meta Horizon Link / Air Link** and on SteamVR, which is exactly the setup you want. The tutorial shows how to switch between the two.

So "upgrading" the VR side really means **deleting it and rebuilding it on OpenXR + XR Interaction Toolkit**. That's the same work as starting fresh, plus clean-up.

### 2. The networking code needs rewriting either way

- Big-Project's Mirror 32 code uses APIs that modern Mirror removed or changed (`ClientScene.RegisterPrefab`, `OnClientConnect(NetworkConnection)`, `hasAuthority` → `isOwned`, `NetworkConnection` → `NetworkConnectionToClient`). The lobby is also already broken by a scene rename (see `01-repo-assessment.md`).
- NewVRDumpling's Photon PUN 2 is in maintenance mode; Photon's current products are Fusion and Quantum, which are different APIs.
- You chose **Netcode for GameObjects** for the new version. Mirror → NGO or PUN → NGO is a rewrite, not an upgrade.

### 3. There isn't much of our own code to save

Big-Project has about 13 scripts of our own, most of them from following a lobby tutorial. NewVRDumpling's working parts were a bought template. Neither has gameplay (cooking, sabotage, rounds). The tutorial's code is written to be better structured than either, so you'd be replacing it anyway.

### 4. Unity 2021 → 6 has its own friction

Even with no VR or networking, a 2021.3 project opening in Unity 6 hits:

- Built-in render pipeline materials. URP is the default now, so materials need converting (Window > Rendering > Render Pipeline Converter).
- API renames: `Rigidbody.velocity` → `linearVelocity`, `drag` → `linearDamping`, `PhysicMaterial` → `PhysicsMaterial`, `FindObjectOfType` → `FindFirstObjectByType`/`FindAnyObjectByType`.
- TextMesh Pro is now inside the uGUI package, so the old `Assets/TextMesh Pro` folder and the `com.unity.textmeshpro` package reference need sorting out.
- Huge vendored folders (SteamVR 97 MB, Mirror 13 MB, Oculus 700 MB, BNG 231 MB) to delete or re-import.

### 5. Legal and security clean-up

NewVRDumpling has paid Asset Store code and a Photon App ID in a public repo. A fresh project starts clean.

## Effort comparison (rough)

| Path | What you'd do | Rough effort for a returning dev |
|---|---|---|
| Upgrade Big-Project in place | Open in Unity 6, fix compile errors, delete SteamVR, fix scene names, upgrade Mirror, convert materials, then **add VR from nothing**, then build gameplay | 2–4 weekends before you're back to "where it was", with no VR player yet |
| Upgrade NewVRDumpling in place | Same, plus rip out Oculus Integration, BNG and the template (the parts that made asymmetric play work) and replace Photon | Worse than above: the working parts are the parts that have to go |
| **Fresh Unity 6.3 project (recommended)** | Follow the tutorial; import the old FBX art on day one | First playable local round in about a weekend; networked a weekend or two later |

## If you still want to try opening the old project in Unity 6

It's a reasonable 30-minute experiment to see the errors yourself. Do it on a throwaway copy:

1. `git clone` the repo into a new folder. Never upgrade your only copy.
2. Delete `Library/` if present. Open with Unity 6.3 LTS through Unity Hub and accept the upgrade.
3. Expect compile errors from `Assets/SteamVR`, `Assets/Mirror` and `Assets/Hierarchy 2`. Delete `Assets/SteamVR`, `Assets/SteamVR_Input`, `Assets/SteamVR_Resources`, `Assets/StreamingAssets/SteamVR` and the `com.valvesoftware.unity.openvr` line in `Packages/manifest.json`.
4. Re-import Mirror from the Asset Store (latest), then work through the API errors listed above in `Assets/Prototypes/LaraTutorials/Scripts`.
5. Fix the Lobby: set the NetworkManager's **Menu Scene** to `Lobby`, and change `"Lara_Game"` to `"Game"` in `CustomNetworkManager.cs` (two places).

You'll then have a flat-screen networked lobby in Unity 6 and no VR. That's the honest ceiling of the upgrade path.

## Salvaging the art (do this in the new project)

The tutorial's Chapter 2 covers this step by step. In short:

1. Copy the FBX files from **NewVRDumpling** `Assets/Kitchen Models/` and `Assets/Model/Dumpling.fbx` into the new project's `Assets/_Project/Art/Models/`. Copy the `.fbx` files **without** their `.meta` files so Unity re-imports them with fresh settings.
2. Check each model's import scale against something you know (the kitchen module is 1.5 × 2 m). In the **Materials** tab, use **Extract Materials...**, then switch the extracted materials to *Universal Render Pipeline/Lit* (or run Window > Rendering > Render Pipeline Converter).
3. Use `Kitchen_Populated.prefab` from NewVRDumpling only as a layout reference (open both projects side by side). Its prefab links point at the old project's GUIDs, so rebuild the layout from the fresh imports.
4. Ask the original artists (SaoirseChar / Gina, judging by file names) before using the art in anything public.
