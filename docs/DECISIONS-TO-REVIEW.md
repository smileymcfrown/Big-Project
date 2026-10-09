# Decisions to review

You asked me to work without stopping for input and to note the calls I made. Each one is a sensible default, not a rule. The tutorial marks the place each decision bites with a **"Review D#"** box.

Your answers to my three opening questions are treated as fixed: **local couch first, then networked**; **Quest 2 via Air Link and via SteamVR**; **Netcode for GameObjects**.

## Project and tools

| # | Decision | Why | If you want to change it |
|---|---|---|---|
| D1 | **Rebuild in a new project** instead of upgrading either old repo | The VR SDKs and networking libraries both repos depend on have been replaced; little of our own code is worth keeping. See `02-upgrade-or-rebuild.md` | The "If you still want to try" section of that doc shows the upgrade path |
| D2 | **Unity 6.3 LTS** with **Netcode for GameObjects 2.13.x** | 6.3 is the current LTS. NGO 3.x exists but requires Unity 6.7 (beta in Oct 2026) and now pulls in Netcode for Entities | When Unity 6.7 LTS ships, upgrade both. The only NGO 3 change that touches our code is `NetworkTime` moving namespace, and we never name that type |
| D3 | **Universal 3D (URP)** template, not the VR template | You learn what each XR piece does by adding it yourself | Start from the VR template if you'd rather have everything pre-wired |
| D4 | **OpenXR + XR Interaction Toolkit 3.6** only, no Meta XR SDK | One build works through Meta Horizon Link and SteamVR. You don't need Meta-only features for PC VR | Add *Unity OpenXR: Meta* or the Meta XR SDK later for passthrough, hand tracking extras or a Quest standalone build |
| D5 | Working title **"Dumpling Kitchen"**, root namespace `DumplingKitchen` | Needed a name for folders, namespaces and assemblies | Rename the namespace with your IDE's refactor tool and the `.asmdef` names together |
| D6 | **One runtime assembly** (`DumplingKitchen.Runtime`) plus an EditMode test assembly | Enough to get faster compiles and testable code without assembly-reference juggling | Split into `Core`, `Gameplay`, `Net` assemblies once the project grows |

## Game rules (all in the `GameConfig` asset unless noted)

| # | Decision | Default | Where |
|---|---|---|---|
| D7 | Round length | **300 s** (5 minutes), 3 s countdown, 10 s results screen | `GameConfig` |
| D8 | **Win condition**: the chef wins the moment they serve **3 dishes**; if time runs out first the dumplings win. Sabotage count is shown but doesn't decide the winner | `GameConfig.dishesToWin`, `RoundManager.ReportDishServed` / `HandleTimerExpired` |
| D9 | The **chef starts the round by ringing a service bell** (XR Simple Interactable → `RoundManager.StartRound`), needing at least 1 dumpling | `GameConfig.minDumplingsToStart` |
| D10 | Sabotages are **undone by the chef grabbing/pressing the thing** (gas knob, light switch, plug, knocked-over pot, hat). After a fix, a target is safe for **5 s** | `SabotageTarget.rearmSeconds` (per object) |
| D11 | **Chef's hat**: knocking it shows a dark "blindfold" quad in front of the chef's eyes until they push the hat back up. Dumplings must physically get within reach (jump from a counter) | `ChefHat`, `ChefVisionBlocker`, `DumplingInteractor.reach` |
| D12 | **Serving**: put all the cooked ingredients for the current order on the serving counter at once. No plates. Orders come in a fixed order and loop | `ServingCounter.menu` |
| D13 | **Cooking**: heat-seconds model. Ingredient cooks after N seconds of heat, burns after M more. Pots must sit in a stove's heat zone | `IngredientDefinition` assets |
| D14 | Starter sabotages: **gas valve, light switch, power plug, knock-over, chef's hat** | One class each in `Scripts/Sabotage` |

## Code architecture

| # | Decision | Why | Alternative |
|---|---|---|---|
| D15 | `RoundManager` is a **small singleton** (`RoundManager.Instance`) plus a few **static C# events** (`RoundStarting`, `SabotageTarget.AnySabotaged`) | Easiest to understand when coming back to Unity; statics are reset on Play so it works with fast Enter Play Mode | ScriptableObject event channels, or a DI container such as VContainer |
| D16 | Dumplings use a **Rigidbody** (physics) rather than a CharacterController | Being shoved, knocked and thrown by the chef is the fun part | `CharacterController` is simpler for precise platforming |
| D17 | A **hand-written orbit camera** for dumplings instead of Cinemachine | Cinemachine 3 split-screen needs output channels per player, which is a lot to take in on day one | Swap to Cinemachine later; `DumplingCamera` is the only thing that changes |
| D18 | Input is read through an **`IDumplingInput` interface** | The motor, camera and interactor don't care whether input comes from a gamepad, the network, or an AI dumpling | |

## Local couch mode (Part 1)

| # | Decision | Default |
|---|---|---|
| D19 | **Split-screen** dumplings on the monitor via `PlayerInputManager`; up to **4** players; join with **Start** (gamepad) or **Enter** (keyboard). Joining closes while a round is running | `PlayerInputManager` settings, `LocalPlayerJoiner` |
| D20 | The headset image is **not mirrored** to the monitor (saves GPU; the monitor is for dumplings) | `DesktopViewSetup.hideHeadsetMirror` |

## Networked mode (Part 2)

| # | Decision | Why | Alternative |
|---|---|---|---|
| D21 | **The chef is always the host** (server + player) | The VR player's grabbing and physics run with zero lag, and props never need ownership transfers | Dedicated server, or Distributed Authority (needs ownership handling for grabbed props) |
| D22 | Dumpling movement is **owner-authoritative** (NetworkTransform Authority Mode = Owner) | Responsive controls for the PC players | Server-authoritative movement with client prediction (much more work) |
| D23 | Every sabotage request is **re-validated on the server** (round running, not already broken, cooldown, within reach) | Never trust the client | |
| D24 | Part 2 **converts** the project to networked; the local version lives on as a git tag (`v0.1-local`) | Supporting both at runtime doubles the testing | Stretch goal in Chapter 16: run local couch dumplings as server-owned players inside a hosted game |
| D25 | **XR only starts for the chef** ("Initialize XR on Startup" off, `XRStartup` starts it on Host) | Dumpling PCs shouldn't launch SteamVR or need a headset | |
| D26 | **Both LAN (direct IP, port 7777) and online (Unity Relay join codes)** | LAN is simplest to debug; Relay avoids router port-forwarding | Lobby browsing / matchmaking via the same Multiplayer Services package |
| D27 | Scenes: **Bootstrap → Menu → Kitchen**, with an editor helper that always plays from Bootstrap | Avoids duplicate NetworkManagers | |
| D28 | New dumplings **can't join mid-round**; max 4 dumplings | Keeps the round fair and the spawn logic simple | Allow late joiners as spectators |
| D29 | Not covered: host migration (if the chef quits, the game ends), reconnecting, voice chat, cheating beyond the server checks | Scope | |

## Things outside this repo that need a human

| # | Item |
|---|---|
| H1 | NewVRDumpling (public) contains **paid Asset Store packages** (BNG VRIF, Odin, Chili Games template). Consider making it private or removing them. I did not change that repo |
| H2 | NewVRDumpling contains a **Photon App ID**. Delete or regenerate it in the Photon dashboard if the app still exists |
| H3 | Ask the original artists before using their models in anything public |

## How the tutorial code was checked

- Every script was **compiled with a real C# 9 compiler** (Unity 6 uses C# 9) against hand-written stubs of the Unity, Input System, XRI, Netcode and Multiplayer Services APIs used. The API names and signatures were checked against the actual package sources (NGO 2.13.3, XRI 3.6.1, Input System 1.20.1, Multiplayer Services 2.4.0, XR Management 4.6.1).
- The pure-logic unit tests (`RoundTimer`, `CookingLogic`, 15 tests) **pass**.
- The code has **not been run inside the Unity editor or on a headset** from here. Expect the usual small set-up slips (a missing Inspector reference, a layer not ticked). The tutorial's checklists are there to catch them.
