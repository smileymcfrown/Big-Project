# Dumpling Madness VR: archive findings

Written 16 October 2026 from the local archive at `G:\D\Gavin\Adv. Diploma\Dumplings\`.

File paths in this report are relative to that archive folder, not to this repo: `Big-Project_Original_Repo/`, `DumplingVR/`, the Discord CSVs and so on all sit beside `Big-Project/`. It builds on [`03-original-design-and-code-review.md`](03-original-design-and-code-review.md) and refers to the decision numbers in [`DECISIONS-TO-REVIEW.md`](DECISIONS-TO-REVIEW.md).

Nothing in the old repos or documents was modified, moved, committed or pushed. The four repos have "dubious ownership" on this PC, so git was run with a one-off `-c safe.directory=*` flag. Your global git config was not changed.

## How to read this

**Status**
- **confirmed**: implemented in code or a scene, or clearly agreed.
- **proposed**: an idea that was discussed.
- **dropped**: explicitly rejected or replaced.

**Source abbreviations**

| Code | Source |
|---|---|
| **G** | `The Big Project - Text Channels - general [...].csv` |
| **P** | `... - programming [...].csv` |
| **A** | `... - art [...].csv` |
| **Doc** | `... - documentation [...].csv` |
| **Q** | `... - questions-for-later [...].csv` |
| **H** | `... - github [...].csv` |
| **V** | `The Big Project - Voice Channels - vc [...].csv` |
| **Rant** | `gavin's fucking rant` |
| **BPO** | `Big-Project_Original_Repo/` (a local clone of `Big-Project`, checked out on **`DevelopXR`**) |
| **DVR** | `DumplingVR/` |
| **NVD** | `NewVRDumpling/` |

- `L123` is the file line in the CSV where the message starts.
- Commit hashes are from the repo named with them.
- Dates in chat citations are dd-Mmm-yy, as the export shows them.

**People.** Discord handles and first names only:
- Smiley McFrown = **Gavin**
- Lala⚔ / Lala-banners = **Lara**, programmer
- Kanser Reverie = **Kieran**, programmer
- 🌺Saoirse🌺 / SaoirseChar = **Saoirse**, art lead and organiser
- Blerp = **Georgina**, artist
- ErmActually = **Emily**, tutor

Kerstin, David and JayRay were tutors or helpers.

**What I didn't repeat.** Anything already in `03-original-design-and-code-review.md` is only referenced here: Kieran's modular character controller, the NewVRDumpling "Gather Fuses / Hide 4 Fuses" quest and the cucumber cutting test.

**A correction to 03.** That doc describes `Big-Project` as "Mirror lobby, no VR player". That is only true of the **`Master`** branch. The **`DevelopXR`** branch of the same repo holds the VR + 4-player local build and most of the gameplay found below. That branch is `origin/DevelopXR` in your `Big-Project` clone too, at tip `432db0df`.

---

# One-page summary

## The concept

**"Dumpling Madness"**, also called "Ruckus in the Restaurant" and "Restaurant Ruckus", is a same-room **asymmetric party game for 1 VR chef against up to 4 dumplings** on one PC. The dumplings play split-screen with gamepads or keyboard and mouse.

It is set in the cramped back kitchen of an old, run-down, cosy Vietnamese/Cambodian-style street restaurant. The VR play space is only 1.5 × 2 m.

The chef is a girl trying to cook a simple dumpling or noodle soup: chop vegetables, fill the pot, and put the ingredients in the pot. The dumplings are tiny (about 5 cm), round, xiao-long-bao-style "mischievous" clones. They jump, climb, use launchers and pipe teleports, and make mischief: knock food off counters, flick the light switch, mess with the gas and plugs, jump on the chef's hat, and above all steal the kitchen's fuses and hide them.

The chef fights back by grabbing dumplings (and throwing them: "dumpling shot put"). Each dumpling the chef catches costs the team one of a shared pool of lives.

The tone is cute, cosy and chaotic: "mischief making", not just hide and seek. The art direction is a Ghibli-like, hand-painted, mostly unlit look. Sources: G L105, L568, L811, L1017; A L246, L1312; Q L30.

## Win/lose, as last implemented (Aug 2022)

- The round is a **3-minute timer**: `MpCountdownTimer.timeRemaining = 180` and `Timer.prefab`. Lara set it in commit `a5d163ce`, "Updated game timer to 3 minutes", on 01-Aug-22.
- **The dumplings win** by completing their mission before the time runs out. The only finished mission was the fuse heist: collect all **6 fuses** from the fuse panel and hide them in the 6 hiding spots. This shows the **"DUMPLINGS WIN"** canvas (`HideQuestGoal.DropItem`, `DumplingsWinCanvas.prefab`).
- **The chef wins** if the dumplings don't finish their mission in time, or if the shared lives run out. This shows **"VR WINS!"**, **"DUMPLINGS LOSE"** and "Chef Wins Music" (`QuestManager`, `DumplingsLoseCanvas.prefab`, Kitchen.unity).
- The chef also has a **cooking score of 0/6**: each of tomato, egg, cucumber, onion, shallot and carrot dropped in the pot scores 1 (`FinalIngredientsLocation.cs`, `ScoreManager.cs`, max 6).
- The win/lose wiring was **unfinished**. Lara's commit `89cbdc90` is titled "Need to tweak win lose condition", and the lose check runs when a player joins rather than when the timer ends. The intent is clear; the exact rules were never settled.

## Top 10 gameplay facts

1. **1 VR chef vs 1–4 local dumplings on one PC** (split screen), working on 28-Jul-22. That day's test was 1 Quest, 2 gamepads, 1 keyboard and 1 mouse (G L2586). The original intent was explicitly **no networking** (G L618; P L58).
2. **Round = 3 minutes** (BPO `a5d163ce`). Dumplings must **complete a task** in time (Saoirse's paper-prototype iteration 2, G L559). The fuse heist was the implemented task.
3. **Fuse heist** (confirmed):
   - 6 fuses sit in a fuse panel. Gavin placed the panel above the service counter, behind the chef, so dumplings must climb past the chef to reach it (P L430–442).
   - Fuses are carried to 6 hiding spots, which light up with particles once a fuse is carried. A hidden fuse becomes chef-grabbable (`ChefInteractable` layer).
   - The fuses' item description is "Used to power the generator".
4. **Shared lives:** a team pool of 10 (`Lives and Spawning.prefab`). Each dumpling's HUD shows the count with a happy or scared face. Death → respawn at the spawn point; the last life → game over (`DumplingControllerDeathAndRespawning.cs`, `LivesManager.cs`). Gavin's original idea: lives look per-dumpling but are actually shared (G L1001).
5. **Chef catches dumplings by grabbing them** (an XRI `XRGrabInteractable` on `Dumpling.prefab`) and can throw them (H L503, 04-Aug-22). Earlier proposed alternatives:
   - Trap dumplings under thrown cups or pots, then lift the trap and grab (G L305).
   - A dumpling-mould "land mine" (G L215).
6. **Chef's tasks:** the in-scene "VR Tasks" board reads "Fill pan with water / Chop 3 Vegetables / Place chopped vegetables in pan" (Kitchen.unity at BPO `92aed0f3`). Agreed recipes were **wonton/dumpling noodle soup** and **dumpling soup with vegetables** (G L816).
7. **The hat sabotage is Gavin's idea** (Q L79, 21-Jun-22). A dumpling landing on the chef's head knocks the hat or visor over **one eye**, on the side it landed. It stays down for a set time or until the chef does a **wiggle** to fix it. Lara liked the wiggle (Q L88).
8. **Other sabotages:**
   - Confirmed or built: light switch (`LightSwitch.cs`, triggered by a dumpling touching it), knocking props off counters (A L2673).
   - Assets built for sabotage: an easily-knocked-out Asian power socket (A L2230) and a gas canister (A L1970).
   - Proposed: toaster (burn → fire → extinguisher), salt in the sugar, turning off the stove, the tap, soy sauce, chemicals in food, stealing ingredients off the chopping board, can opener, fan, sauces, messing with the recipe book, mortar and pestle (G L738–747; `Programming notes - 29th July.png`; drawio, Jan 2023).
9. **Dumpling movement:**
   - Built: Rigidbody movement, jump (double jump reaches counter tops), a third-person or first-person camera swap, **Activate points** for pipe/drain **teleports** and **launchers**, and holes in the counters as secret passages (BPO `ActivationPoints/*`; P L694; A L1052).
   - Proposed: a pedal-bin launcher, a bouncy sponge, a spoon catapult, and a "dash" that squirts filling (A L721, L2192; Q L82).
10. **Lobby:** a PC lobby titled **"Ruckus in the Restaurant"**:
    - Player 1–4 Ready / Not Ready slots, plus Start Game and Quit Game buttons.
    - Players join by pressing a Join button: Space, left mouse button or gamepad South/A.
    - End screens offer **Replay / Return Lobby / Quit** (`PCLobby.prefab`, `Dumplings*Canvas.prefab`).

## Where the working local co-op build lives

**`Big-Project`, branch `DevelopXR`.** It is checked out locally in `Big-Project_Original_Repo/` and is also `origin/DevelopXR` in your `Big-Project` clone. The repo is `smileymcfrown/Big-Project` on GitHub.

- **Best snapshot:** commit **`92aed0f3`** (Kieran, 05-Aug-22, "Kitchen scene back to normal"). It sits right after Kieran's `b2c88955` "Not Spawning VR" fix.
- **Scene:** `Assets/Big-Project/Scenes/Kitchen.unity`.
- **Engine and packages:** Unity **2021.3.1f1**, **XR Interaction Toolkit 2.0.2**, **Oculus XR Plugin 3.0.2**, Input System 1.3.0 and URP 12.1.6. No paid template or Photon is involved.
- **Proof of concept:** Gavin's never-committed first version is in `DumplingVR/` (`Assets/Scenes/XRManualTest.unity`, `Assets/Scripts/Gav/`, 26–28-Jul-22).

---

# 1. Core concept

| Finding | Status | Who | Source |
|---|---|---|---|
| Settled concept: "VR chef vs dumpling hide and seek" | confirmed | Lara asked, Saoirse agreed | G L105–107, 08-Apr-22 |
| 1 VR chef vs 4 controller dumplings, working as a team | confirmed | Saoirse, Gavin | G L568–570, L811 |
| The focus is **mischief making**, not just evading capture. This sets it apart from a similar YouTube game | confirmed | Saoirse, Lara | G L1016–1018, 09-May-22 |
| Same-room social party game; Steam Remote Play and controller support are selling points | confirmed | Lara | G L618, 29-Apr-22 |
| The chef also cooks, which gives a reason to open cupboards and the fridge | confirmed | Gavin | G L180 |
| Backstory option: 4 comic gangsters reincarnated as dumplings; the chef isn't evil, just busy | proposed | Gavin | G L988–1006 |
| Backstory option: dumplings come alive at the full moon or lantern festival | proposed | Saoirse | G L986, L999 |
| Trailer/intro idea: a dumpling in a box wakes up and jumps out, and the chef turns to grab it | proposed | Gavin, Blerp | G L998–1008 |
| Earlier ideas: a dumpling rolling to the steamer vs a "god" player; a rotating-box world | dropped | Lara, Gavin | G L43–76 |
| Late alternative: a tiny VR player with a grapple, hunted by normal-size dumplings | dropped | Lara and Kieran | G L2198–2206, 19-Jul-22 |

**Titles**

| Title | Status | Source |
|---|---|---|
| **"Dumpling Madness"** (Kieran's logo video) | confirmed working title | G L1342, 31-May-22 |
| **"Ruckus in the Restaurant"**: the title text on the in-game PC lobby | confirmed (in build) | BPO `Prefabs/PCLobby.prefab` |
| **"Restaurant Ruckus"**: build and product name | confirmed (build name) | `Builds/RestaurantRuckus/RestaurantRuckus_Data/app.info`; G L2446; P L413 |
| "Dumpling Madness VR" | menu title in NVD | per doc 03 |
| Hide & Gyoza Seek, Hide 'n Steam, Escape the Kitchen!, Dumpling Trouble/Malarkey/…, Lumpy Dumpily | dropped/proposed | G L118, L1286–1308 |
| A game-name vote | never held | Q L65 |

**Studio, genre, platform and session**

- **Studio:** "Poppycock" (Poppycock Studios in the build). Source: G L115, L2446; Doc L34.
- **Genre:** asymmetric VR/PC local party game.
- **Platform conflict:**
  - Early assumption: Quest 1 or 2 (A L26, L1446).
  - By June: "**PCVR only**", but still built to Quest-level budgets (A L2275, 11-Jun-22; L2348). This is the latest position.
  - The test headset was a Quest 2 over Link/Air Link (P L610).
  - Steam (and itch) was the assumed store (Doc L185–200).
- **Players:** 1 VR + 1–4 PC dumplings (G L811; `PlayerInputManager.m_MaxPlayerCount: 4`).
- **Session length:** 3-minute rounds (see section 2). No match or multi-round structure was built. Gavin suggested "each round" could swap appliances for variety (A L2693).
- **Tone:**
  - Cute, cosy, cheeky.
  - The kitchen is "commercial beat-up cosy" (A L246–251).
  - "Always for cute/appealing" (A L1062–1075).

---

# 2. Win/lose conditions and rules

| Rule | Status | Who | Source |
|---|---|---|---|
| **Round timer 3 min** (180 s), shown as `mm:ss` | confirmed (code) | Lara | BPO `MpCountdownTimer.cs`, `Timer.prefab`, commit `a5d163ce` 01-Aug-22 |
| Earlier timer placeholders: 20 s; a networked timer via Photon | superseded | Lara | BPO `NetworkedTimer.cs`; P L465, 25-Jul-22 |
| **Dumplings win by completing their task(s) within the time limit** (paper-prototype iteration 2), while the chef tries to stop them | confirmed direction | Saoirse | G L559, 22-Apr-22 |
| Implemented dumpling win: all 6 fuses hidden → "DUMPLINGS WIN" canvas, game paused | confirmed (code) | Lara | BPO `HideQuestGoal.cs`, `DumplingsWinCanvas.prefab`, `c6001e4c`, `e2c54866` |
| Implemented chef win: dumpling goal not reached → "VR WINS!" on the VR canvas, "DUMPLINGS LOSE" on the PC, plus Chef Wins music | confirmed (code), wiring buggy | Lara | BPO `QuestManager.InitiateDumplingTasks`, `42665832` "added audio for when VR player wins", `89cbdc90` "Need to tweak win lose condition" |
| **Shared lives**: 10 in the pool (script default 5; NVD/DVR placeholder "3 Lives"). Die → respawn at the `SpawnPoint`. Last life → game over | confirmed (code) | Kieran | BPO `LivesManager.cs`, `Lives and Spawning.prefab`, `604cb4ba`, `a2e95f13` |
| Gavin's original lives idea: 4 different dumplings appear to have 3 lives each, but the lives are shared | proposed → built as a shared pool | Gavin | G L1001, 08-May-22 |
| Chef cooking score: +1 per ingredient in the pot, out of 6 (tomato, egg, cucumber, onion, shallot, carrot). Shown on the dumpling HUD as "Score" and "0/6" | confirmed (code) | Lara | BPO `FinalIngredientsLocation.cs`, `ScoreManager.cs`, `Dumpling.prefab` |
| **"Chef Madness" bar** on the dumpling HUD (`SliderController`). Hiding fuses raises it and gathering lowers it; the ingredient hook is commented out. Its meaning is unclear: possibly a tug-of-war meter | confirmed UI, rules unclear | Lara | BPO `HideQuestGoal.cs`, `GatherQuestGoal.cs`, `FinalIngredientsLocation.cs` |
| Paper iteration 1, **hide and seek**: dumplings hide and survive until time out; they lose when put in the pot | proposed, superseded by iteration 2 | Saoirse | G L555–557 |
| Paper iteration 3, **"dirty dumplings"**: a dirty dumpling reaching the pot forces the chef to throw the pot out, freeing everyone | proposed | Saoirse | G L561–562 |
| Task risk/reward. Toaster: easy to disrupt, easy to fix. Salt in sugar: high risk, getting caught undoes progress. Knocking ingredients off shelves: low stakes, repeatable | proposed | Saoirse, Gavin | G L743–746 |
| A neglected fire counts against the chef the longer it burns | proposed | Gavin | G L742 |
| Missions randomised from a pool of similar difficulty; must not conflict (e.g. "knock egg on floor" vs "put egg in wok") | proposed (tutor requirement) | Emily via Gavin | P L490–493, 26-Jul-22 |
| A "framework for missions" plus a playable demo by 02-Aug, or a new game | tutor deadline | Emily | G L2511 |
| Armed dumplings (holding a knife or fork) can't be grabbed | proposed | Gavin | A L1419 |
| Unreachable spots: one view is that safe zones are fine (Saoirse), another that they could make dumplings uncatchable (Gavin) | open conflict | | A L1087, L1480, L2703 |
| Chef UI shows goals completed by dumplings vs by the chef; no health bars | proposed | Gavin | G L2047 |
| Extra modes: free-for-all, endless; AI dumplings for empty slots | proposed; AI dropped for the course | Gavin, Lara | G L1580–1589 |

**Conflicts**
- Who wins on time-out: iteration 1 says the dumplings; iteration 2 and the August code say the chef. **The latest is the code: time-out = chef wins.**
- Lives: 3 (idea and DVR prefab), 5 (script default) or 10 (final prefab). **The latest is 10.**

---

# 3. Chef gameplay

## Recipes, ingredients and steps

| Finding | Status | Who | Source |
|---|---|---|---|
| Recipes: **wonton/dumpling noodle soup** and **dumpling soup with vegetables** ("simple steps we can code in") | confirmed (agreed) | Lara and Kieran | G L816, 05-May-22 |
| Wontons need only the pot; steamed dumplings need the steamer | discussion | Gavin | G L927 |
| In-scene **VR Tasks**: "Fill pan with water. Chop 3 Vegetables. Place chopped vegetables in pan." | confirmed (UI text) | Lara | BPO Kitchen.unity @ `92aed0f3` (`VR Task Canvas`) |
| Chop the ingredients and put them in the steamer; water handling undecided | in progress | Lara | G L2334, 24-Jul-22 |
| Ingredients modelled: carrot, shallot, tomato, onion, cucumber, egg. Chopped versions requested; egg "not super important" | confirmed | Blerp (models), Lara (request) | A L1784–1991, L2654–2670 |
| **Chopping conflict:** "there's no chopping (right? Maybe?)" (25-Jun) vs chopped ingredients needed (24-Jul). Cutting scripts exist: NVD `Cut.cs` (Jul-22), and BPO Feb-2023 EzySlice `Slicer.cs` / `SliceListener.cs` ("Slicing works!!", `6f2cb3c4`). **Latest: chopping is in.** | conflict → confirmed | Gavin vs Lara | Q L84; A L2654; BPO `623b8f93`, `6f2cb3c4` |
| Put all ingredients on the chopping board before throwing them in the wok, so dumplings can steal them | proposed | Gavin (friend's idea) | Q L84 |
| Mincemeat as a flat square blob (no packaging) | confirmed direction | Gavin | Q L19–29; A L2239 |
| Jan-2023 chef task list: chop potato, carrot and onion; put vegetables in the pot; **put dumplings in the pot** | proposed (2023 design) | Gavin and Kieran | BPO `Documentation/ProgrammingFrameworkDumpling.drawio` (`e373e565`) |
| Jan-2023 chef "quest activities": **Fuses** (restore them) and **Dirty Food** (deal with it) | proposed | Gavin and Kieran | same drawio |

## Stations, tools and props

| Item | Status | Source |
|---|---|---|
| Wok (with ladle or spatula), stock pot (36 cm), bamboo steamer with a separate lid (`Steamer_Hat`), rice cooker, gas stove with gas bottle, sink and basin, chopping boards (2), butcher's knife / cleaver plus a small knife, mortar and pestle, rolling pin, bowls, plates, chopsticks, spoons, forks, cans, bottles, jug, crate, containers | confirmed (assets) | A L826–834, L988, L1746; BPO `Kitchen1.5x2m.prefab` |
| Microwave replaced by the rice cooker; Saoirse noted the rice cooker could double as a catapult | microwave dropped | A L1091–1104 |
| No oven, so no baking tins | confirmed | A L1912 |
| Bamboo sieve scoop | dropped | A L884 |
| Toaster: modelled by Saoirse; Gavin added `Toaster.fbx` in Feb 2023 | confirmed (asset) | A L1962, L2203; BPO `5e8d8450` |
| Interactable doors on hinge joints: fridge, freezer, dairy doors L/R, sink doors L/R, main door, flip-up counter; bin lid with pedal script | confirmed (setup), doors buggy | G L1696; A L2393; BPO `Kitchen1.5x2m.prefab` |
| Separate fridge handle | deferred | G L2062–2081 |

## Movement, reach and the chef's body

- **Movement:** physical only, no teleport (Saoirse, G L383). Splitting the kitchen into two halves with a swap button was proposed (Gavin, G L378).
- **Force-grab:** found by accident on 12-Jul. Gavin worried it would make the chef too strong; it was to be playtested (**conflict**, G L2087–2091). Saoirse earlier wanted a distance-grab "force pull" for reach (G L190). The Jan-2023 drawio lists a "Reach Mechanic".
- **Chef body: conflict.** The agreed "cop-out" was head and hands only. Gavin wanted a **"ghost body"**, an apron to the floor that trails the head, so the chef physically blocks the dumplings' view (Q L44–64). In Feb 2023 Kieran's "Body Connected To VR!!" (`432db0df`) attached a full body using FinalIK VRIK. That is a paid asset; only the idea matters here.
- **Chef face:** blinking, and maybe eyes that follow dumplings (Saoirse, A L1345).
- **Chef identity:**
  - A **girl**, "Chef" (Q L30).
  - Name ideas: "Moon-pie" (Gavin), "Chef Tafe" (Blerp) (A L2646).
  - Hat, pin and apron (A L1997).

## Fighting back against dumplings

| Method | Status | Who | Source |
|---|---|---|---|
| **Grab dumplings** (an XRI `XRGrabInteractable` on `Dumpling.prefab`) | confirmed (code) | Gavin (`c8f4ce2e` "Dumplings can be grabbed and dropped!"), Kieran (`0d822b97`) | G L2649; H L503 |
| Throw caught dumplings ("dumpling shot put is fun") | confirmed (emergent) | Kieran | H L503–504, 04-Aug-22 |
| Trap under thrown cups, bowls or pots; lift the trap with one hand, grab with the other. Light traps can be jumped off; heavy traps hold | proposed | Gavin | G L305–309 |
| Dumpling-mould "land mine" floor trap | proposed | Blerp | G L215–226 |
| A grabbed dumpling is killed (lose a life) | confirmed (lives system); method proposed | Gavin | Q L79 |
| Put caught dumplings in the pot or steamer | proposed | Saoirse, Lara | G L557, L2275 |
| Swat at the swinging hanging lamp; grab a dumpling off your face; wiggle to fix the hat | proposed | Gavin | A L1578; Q L79 |
| Scrabble in the sink to catch a dumpling | proposed | Gavin | A L2288 |
| Retrieve hidden fuses (they become grabbable on the `ChefInteractable` layer) and flip the fuse box back | confirmed (layer), proposed (restore) | Lara, Gavin | BPO `PickUp.cs`; G L747 |
| Toaster: press the button while it smokes, or use the extinguisher once it's on fire | proposed | Gavin | G L738 |

## Chef UI

| Finding | Status | Source |
|---|---|---|
| In-scene world-space canvases: **VR Task Canvas** (task list), **VR Timer Canvas** plus a **wall clock** with hour, minute and second hands, **VR Win Canvas** | confirmed (scene) | BPO Kitchen.unity @ `92aed0f3` |
| Wrist UI ("probably", since Lara had one): `VrWristUI` / `WristUI Canvas` | proposed, prototype exists | G L2057; doc 03 |
| Order slips on a rail, or a paper recipe on the wall | proposed | G L2053 |
| Recipe book or journal (like Phasmophobia) that dumplings can "mess with" | proposed | G L254–260; drawio |
| No head-locked HUD; diegetic only (TV, watch, wrist) | principle | G L2050–2052 |
| A wristwatch timer was proposed; Gavin isn't keen on watches | proposed | G L303–304 |
| Pausing hides or disables the VR hands | confirmed (code, partly commented out) | BPO `UiManager.cs`, `0bcbcf44` |

---

# 4. Dumpling gameplay

## Sabotages, missions and tasks

| Sabotage / mission | Status | Who | Source |
|---|---|---|---|
| **Fuse heist** (DevelopXR version, latest). How it plays: (1) `Gather Quest`: collect **6 fuses** (`Fuse1`–`Fuse6`) from the `Fuse Panel`; walking into one parents it to the dumpling. (2) The 6 hiding spots (`1`–`6`) switch on with particle effects. (3) `Hide Quest`: drop the fuses there; each becomes chef-retrievable. (4) All 6 hidden → dumplings win. Item description: "Used to power the generator" | **confirmed** | Lara (code), Gavin (fuse models and panel placement) | BPO `PickUp.cs`, `Gather Quest.prefab`, `Hide Quest.prefab`, `e2c54866`, `f6566a67`, `c6001e4c`, `01d1d751`; P L419–442; G L2337 |
| Fuse heist, NVD version: 4 fuses and 4 hiding spots | superseded by 6 | Kieran and Lara | doc 03 |
| **Fuse panel placement:** Lara put it under the sink; Gavin moved it above the service counter, behind the chef, so dumplings must climb into view | confirmed (Gavin's placement) | Gavin | P L430, L442, 24-Jul-22 |
| Taking a fuse should make the lights go out; dining-room light leaks in | proposed | Gavin | G L747, L1509–1519 |
| **Light switch:** dumplings jump and flick it. `LightSwitch.cs` toggles lights when a dumpling touches it. The switch model was built to be easy to hit and see | confirmed | Gavin (spec), Blerp (model), Kieran/Lara (code) | P L93; A L2022–2164; BPO `LightSwitch.cs` |
| Wall or hanging lights flicker or dim when landed on; the lamp swings or snaps. Epilepsy-safe fade rather than strobe | proposed | Gavin, Blerp, Saoirse | A L1578–1615 |
| **Power plug / socket:** an Asian-style socket chosen because it's easier to knock a plug out | confirmed (asset), mechanic proposed | Blerp, Gavin | A L2230–2237 |
| **Gas:** gas bottle under the stove "perfect for dumplings to fuck with"; turn off the stove | asset confirmed, mechanic proposed | Saoirse, Gavin | A L402, L1970; G L747 |
| **Knock things off counters** (food on the floor = unusable) | confirmed (prototype) | team | A L2673; `Programming notes - 29th July.png` |
| **Chef's hat / visor over one eye**, fixed by a wiggle or after a set time. Alternative: cling to the chef's face (suicidal) | proposed | Gavin; Blerp says it was her trailer idea | Q L79, L88–89 |
| **Toaster:** jump on the lever → toast burns → fire if ignored | proposed | Gavin, Saoirse | G L738–743 |
| Salt in the sugar | proposed | Saoirse | G L741 |
| Turn on the tap; turn on the toaster, blender, can opener or stove; pour soy sauce; chemicals (cleaning products) in the food. "Tasks using…" rice cooker, cans, bottles, knives, light switch | proposed (29-Jul notes) | Gavin, Kieran, Georgina | `Art/Notes/Programming notes - 29th July.png`; P L629 |
| Steal ingredients off the chopping board | proposed | Gavin | Q L84 |
| Blerp's task list: knock X items off before being caught; break plates; skate in the sauce bowl for a high score; stack items as a ladder; stack on another dumpling | proposed | Blerp | V L479–492, 28-Jul-22 |
| Jan-2023 dumpling task list: Toaster, Fuses, Can Opener, Fan, Light, Sauces, Recipe Book, Mortar & Pestle (herb/spice coating) | proposed (latest list) | Gavin and Kieran | BPO drawio `e373e565` |
| Dumplings just running around may be distraction enough | proposed | Gavin | G L1569 |

## Movement and abilities

| Ability | Status | Who | Source |
|---|---|---|---|
| Rigidbody dumpling (rotation frozen), third-person **and** first-person cameras with a **Camera Swap** button | confirmed | Kieran | BPO `Dumpling.prefab`, `MultiMotor`/`MultiCamera`, `88f908c4` |
| Jump; double jump reaches counter tops | confirmed | Kieran, Gavin | P L694; BPO `b31dc496` |
| Camera further back plus a gravity tweak "feel a lot more like dumplings" | confirmed | Kieran | H L458, 02-Aug-22 |
| **Activate points** with an "Activate" prompt. **Teleport points** (pipe in at the bottom, out at the top; the sink drain) and **Throw points** (launchers with force (2,2,2) that disable movement for a few seconds). Saoirse placed them in the kitchen | confirmed | Kieran (code), Saoirse (placement) | BPO `ActivationPoints/*`, `fe82f487`, `105c1651`, `16ccaef2`; P L448; A L2727 |
| Secret dumpling passages: holes in the counter side panels behind crates or the gas bottle; collider gaps to jump through | confirmed (model and colliders) | Gavin | A L1052, L1476; G L2670 |
| Stepped shelves, wall lights as ledges, the fridge door top, the sink pipe as a jump point | proposed/modelled | Gavin, Blerp | A L1185–1239, L1602, L2384, L2513 |
| **Pedal-bin launcher:** one dumpling stands on the lid, another hits the pedal | proposed (model built) | Gavin | A L721, L1288, L2382 |
| Bouncy/slidey sponge; spoon catapult; rice-cooker catapult; mouse-trap launcher; sauce-bowl skateboard; chopstick ramps | proposed | various | A L823, L1103, L2192; G L99–101, L2645 |
| **Dash** by squirting filling, refilled at the chopping board or a mixing bowl. A Dash button existed in Gavin's input map | proposed (input only) | Gavin | Q L82; DVR `PC Controls.inputactions` |
| Wet-noodle grappling hook | proposed, deferred | Gavin, Blerp | P L289–307 |
| Wall climbing ("Added spiderman", Climbing Game prototype). It may be a dumpling or VR climbing test; it's built on VRIF and its purpose is unclear | prototype, unclear | Kieran | BPO `8e931d33`, `a9339849`, `da01066e` (04–14-Aug-22) |
| Hiding: clutter on open shelves, the dish rack, cover behind shelf brackets; a skinny-cupboard safe zone | proposed | Gavin | A L1052, L1233, L1480 |
| Size about 5 cm diameter; fast movement tunnels through thin colliders | confirmed (issue) | | A L1079; P L648–694 |

## Characters, character select and respawn

- **Look:**
  - Round, not oval. Xiao long bao preferred. Pinched pastry.
  - Gavin wants them to roll or wobble. A "jiggly butt wobble" with no legs was popular; stumpy legs, pasta legs and floating limbs were also discussed.
  - Source: G L212, L708–716; A L2089–2131, L2557.
- **Clones:** all dumplings are clones, with no names and no separate personalities (Gavin, A L2633). Conflicting idea: 4 visually different dumpling types (Gavin G L1001; Blerp G L93).
- **4 dumpling materials** (`Dumpling1`–`4`) and Lara's `DumplingSelection` exist in NVD (doc 03). Blerp proposed oval and round options plus coin unlocks (A L2117). Multiple faces to choose from (Lara, G L2254). **Status:** proposed. No character select was built on DevelopXR.
- **Respawn:** at the `SpawnPoint` after "death", spending a shared life. A `DeathCollider.prefab` (a small trigger) exists but isn't placed in the 2022 Kitchen scene. The intended killer, e.g. the chef's hand or the pot, is unconfirmed. Source: BPO `DeathAndRespawning/*`.

## Dumpling controls (final input map)

Source: BPO `Assets/Big-Project/Inputs/DumplingInputs.inputactions`, map `Player`, schemes `Keyboard&Mouse` and `Gamepad`.

| Action | Gamepad | Keyboard & mouse |
|---|---|---|
| Move | Left stick / D-pad | WASD / arrow keys |
| Look | Right stick | Mouse delta |
| Jump | South (A) | Space |
| Activate (teleport or launch point) | West (X) | Left mouse button / Shift |
| Camera Swap (third/first person) | North (Y) | Q |
| JoinButton | South (A) | Space / left mouse button |

The Jan-2023 drawio adds Dash, Pause and Quest Menu. The code also has Esc for pause and **I** to open the task window.

---

# 5. The local co-op setup that worked

## Which repo, when, and what versions

| | |
|---|---|
| **Working build** | `Big-Project` repo, branch **`DevelopXR`**. Local copy: `Big-Project_Original_Repo/`. Best snapshot `92aed0f3` (05-Aug-22). The co-op fix is at `b2c88955` (04-Aug-22) |
| **Proof of concept** | `DumplingVR/`. **Uncommitted**, local-only files dated 26–28-Jul-22: `Assets/Scenes/XRManualTest.unity`, `Assets/Scripts/Gav/GamepadExample.cs`, `PCController.cs`, `Assets/Prefabs/Gav/XRDumpling.prefab`, `CapsuleDude.prefab`, and `PC Controls.inputactions` (in `Assets/Samples/XR Interaction Toolkit/2.0.2/Starter Assets/`). Copy these somewhere safe; they exist nowhere else |
| **Unity** | 2021.3.1f1 (BPO `f3a7c3fb` "Updated to Unity 2021.3.1f") |
| **VR SDK** | **XR Interaction Toolkit 2.0.2** (XR Origin, `ActionBasedController`, `XRGrabInteractable`) with **Oculus XR Plugin 3.0.2** through XR Management 4.2.1. Tested on a Quest 2 over Link/Air Link. OpenXR 1.3.1 was added in the Feb-2023 commits |
| **Input** | Unity **Input System 1.3.0** with `PlayerInputManager` and `PlayerInput` |
| **Render** | URP 12.1.6 |
| **Paid / Photon** | **None in the co-op path.** The `BNG Framework` (VRIF) folder is on the branch from Lara's and Kieran's imports and reverts (`7d3ab7e1`, `81c6cdbc`, `d59ce5c5`, `42ac4f96`). Its player prefab in Kitchen.unity, "BNG Playerstuff Spiderman", is **inactive** (`m_IsActive: 0`). The chef is the free XRI XR Origin |

## How it evolved

1. **Earlier networking attempts failed or were dropped:**
   - Mirror, May–8 Jun (P L27–88).
   - Photon PUN2 with the paid asymmetric template and VRIF, 16–25 Jul (P L144–459).
   - Kieran's one-machine "depth" camera attempt, which broke with more than one dumpling (H L378–383).
2. **26-Jul-22, Gavin:**
   - Built "a scene that output to both PC screen and VR headset in about 30 minutes" (P L489).
   - Posted the VR output plus "**4 different cameras on the PC at the same time**" (G L2453–2454).
   - Photon was ditched (P L491).
   - His reference is the archived `vr-asynchronous-input.mp4`, a Unity Answers video. My stills show two Game views: the VR camera on Display 1, and a second camera set to **Target Display = Display 2** (P L504).
3. **26/27-Jul:**
   - Separate Input System maps for gamepad and Oculus (P L580–585).
   - XRI 2.0.2 installed. `PlayerInput` didn't work in the XRI example scenes, so `GamepadExample.cs` read `Gamepad.current` directly (P L595–603).
   - Result: **1 VR + 1 gamepad** working (P L610–620).
4. **26–28-Jul, DVR `XRManualTest.unity`:**
   - XR Origin plus a `PlayerInputManager` (join when a button is pressed, SendMessages, no split screen) spawning `XRDumpling.prefab`. That prefab is a Rigidbody with a `PlayerInput` using the `PC Controls` "Dumpling" map, its own Camera, and lives, timer and activate canvases.
   - `PCController.cs` responds to `OnMove`, `OnJump` and `OnDash`.
5. **28-Jul, video test:** 1 Quest + Player 1 gamepad, Player 2 gamepad (drifting), Player 3 keyboard (jump only) and Player 4 mouse (G L2586).
6. **29/30-Jul:**
   - Gavin's "**The BIG Commit – Ready to go and updated to URP!**" (`c8f4ce2e`): "Scene fully setup / Extra Audio Listeners disabled / Dumplings can be grabbed and dropped!"
   - Input control schemes added (`16c9101d`).
   - Announced in G L2649.
7. **31-Jul–02-Aug:** Kieran's dumpling prefab, movement and camera (`88f908c4`), Lara's tasks, timer, UI and win/lose, Kieran's lives, respawn, throw and teleport, plus Gavin's colliders.
8. **04-Aug, Kieran: the VR controllers no longer spawned an extra dumpling** (`b2c88955` "Not Spawning VR"; H L503). He added a dedicated **`JoinButton`** action bound only to keyboard, mouse and gamepad, and set it as the `PlayerInputManager` join action.

## How the views were split (final `Kitchen.unity` @ `92aed0f3`)

- **Headset:** the XRI **XR Origin** camera renders to the HMD.
- **`DisableView.cs`** on the XR Origin sets `XRSettings.gameViewRenderMode = GameViewRenderMode.None` at the end of the first frame. The headset image is **not mirrored** to the monitor, which leaves the whole window for the dumplings.
- **Monitor:** a `PlayerInputManager` with:
  - **Split Screen = on**, **Max Player Count = 4**, Allow Joining = on;
  - Join Behavior = *join when the join action is triggered*, with the join action `DumplingInputs/Player/JoinButton`;
  - Notification Behavior = *Invoke Unity Events*;
  - Player Prefab = `Assets/Big-Project/Prefabs/Dumpling.prefab`.

  The OnPlayerJoined event calls `QuestManager.InitiateDumplingTasks` and activates a UI object.
- **Each `Dumpling.prefab`** has:
  - a `PlayerInput` (actions `DumplingInputs`, default map `Player`, its camera linked for split screen);
  - first- and third-person cameras with **Target Eye = None**, so they never render into the headset;
  - a URP `OverlayCamera` for its HUD.
- **HUD per dumpling:** "Dumpling Tasks" panel, "Chef Madness" bar, Score and 0/6 count, lives with a happy or scared dumpling, an "Activate" prompt, and a pause menu.
- **Input separation:**
  - Gamepads and keyboard/mouse each get their own `PlayerInput` with the device paired at join.
  - XR controllers are excluded because they can't trigger `JoinButton`.
  - This was the "all controllers control all players" / extra-player problem Gavin warned about (P L590).
- **Audio:** extra AudioListeners disabled, so only the chef's listener plays (`c8f4ce2e`). Per-dumpling positional audio was an open problem; the free **MLPAS** multi-listener asset was the proposed fix (P L623–631).

## Key files

**DevelopXR**
- `Assets/Big-Project/Scenes/Kitchen.unity`
- `Assets/Big-Project/Scripts/DisableView.cs`
- `Assets/Big-Project/Inputs/DumplingInputs.inputactions`
- `Assets/Big-Project/Prefabs/Dumpling.prefab`
- `Assets/Big-Project/Prefabs/PCLobby.prefab`
- `Assets/Big-Project/Prefabs/Kitchen1.5x2m.prefab`
- `Assets/Big-Project/Scripts/Dumplings/**`
- `Assets/Big-Project/Scripts/VR Cooking Tasks/**`

**DVR**
- `Assets/Scenes/XRManualTest.unity`
- `Assets/Scripts/Gav/*`
- `Assets/Prefabs/Gav/XRDumpling.prefab`

## Known co-op issues at the end

- Gamepad drift on one pad; the keyboard player could only jump (G L2586).
- Dumplings tunnelling through thin colliders at speed; the fix was a speed limit (P L648–694).
- A gap between the counter and the wall pushed a dumpling camera outside the kitchen (P L701).
- Split-screen audio (above).
- TAFE PCs couldn't run the project (G L2726).
- The number of simultaneous gamepads after the 04-Aug fix is **not stated**.

---

# 6. Level and kitchen layout

| Finding | Status | Who | Source |
|---|---|---|---|
| **Play space 1.5 × 2 m**: an open middle with counters around it that the chef reaches over. Footprint about 2.6 × 2 m. Gavin preferred 2 × 2 m | confirmed | Saoirse, Gavin | A L1084–1090, L1689; G L1646–1657; `Art/Notes/Layout F360.png` |
| Counter depth 45 → 40 → about 25 cm (slim-line); props shrunk to fit. Saoirse and Blerp thought the props too small | confirmed; scale conflict unresolved | Gavin vs Saoirse, Blerp | A L1102, L2435; G L1825–1863 |
| **Saoirse's layout adopted.** Bathroom and shelves outside the kitchen. Curtains to the front of the restaurant. Back opens through an open doorway. Inside: stoves along the top, sink and oven/table-top on the right, fridge at the bottom, shelf and table on the left, a fold-out counter | confirmed | Saoirse; Gavin annotated | A L1057–1105; `Art/Notes/Saoirse Layout.png` |
| Fusion 360 plan (cm): 170 shelf/counter, flip-up counter, stove counter 140, sink 70, 80 counter, 115 shelf, rice cooker; microwave crossed out; "Gap?"; fridge to 80 cm (later 70 cm so the bin fits beside it) | confirmed | Gavin | `Art/Notes/Layout F360.png`, `Kitchen Layout Sketch.f3d/.dxf`; A L1459 |
| Final Unity prefab `Kitchen1.5x2m`: Long_Counter, Service_Counter, Flip-Up_Counter, Basin with Sink_Door_L/R, Fridge with Freezer_Door and Dairy_Door_L/R, fridge shelves 1–4, even and stepped shelves (`G.Shelf_*`), Light_Switch, Main Door, Kitchen_Wall, hero props, ingredients | confirmed | Gavin, Saoirse, Blerp | BPO `Kitchen1.5x2m.prefab` @ `92aed0f3` |
| Kitchen.unity also has: Fuse Panel, Fuse1–6, Fuse Hiding Spots 1–6, Teleport System, Throw System, Lives and Spawning, Task Manager, VR Task / Timer / Win canvases, a wall clock, Chef Wins Music, Temp Roof, PC Canvas, PCLobby | confirmed | | BPO Kitchen.unity @ `92aed0f3` |
| Restaurant kitchen (not domestic), Asian theme. The serving counter opens to the dining room. Background lanterns, tables and stools | confirmed | Saoirse, Gavin, Blerp | G L1516, L193–205; A L841, L1709 |
| Modular pipes on the walls as dumpling travel routes (in at the bottom, out at the top) | confirmed (models), teleports built | Gavin | Q L67–74; P L448 |
| Sink: a solid basin, a drain onto the floor, the drain as a teleport | proposed/partly built | Saoirse, Gavin | A L2273–2302, L2727–2735 |
| Ceiling: hanging lamp vs fluorescent tube; warm light; ceiling fan; Temp Roof in the scene | proposed | | A L1578–1606, L2274 |
| A second level: an open-market restaurant with ice boxes and diners who ignore the chaos | proposed (deferred) | Gavin | A L1701–1705 |
| "Shinobi bonus level" note | unknown | Gavin | Q L97 |
| Intended flow: dumplings start at the spawn point, climb or launch to the fuse panel behind the chef, carry fuses to the hiding spots and avoid capture. The chef cooks at the board, stove and sink while defending | inferred from the placements | Gavin | P L442 |

---

# 7. Menus and flow

| Screen | Status | Source |
|---|---|---|
| **PC lobby** "Ruckus in the Restaurant": Player 1–4 Ready / Not Ready slots, Start Game, Quit Game. `StartGame()` simply hides the lobby, and none of the prefab's buttons have click handlers | confirmed (prefab), logic minimal and unwired | BPO `PCLobby.prefab`, `PCLobbyManager.cs`, `f8070929` "Started PC lobby (some issues)" |
| Join: press A, Space or the left mouse button (`PlayerInputManager`) | confirmed | BPO Kitchen.unity, `b2c88955` |
| Timer starts automatically when the scene loads | confirmed | BPO `MpCountdownTimer.cs` |
| Pause: Esc (`timeScale = 0`), pause UI with Quit | confirmed | BPO `UiManager.cs` |
| Task window: **I** toggles "Dumpling Tasks" (task name, description, status) | confirmed | BPO `QuestManager.cs`, `Dumpling.prefab` |
| End screens: "DUMPLINGS WIN" / "DUMPLINGS LOSE" with Replay, Return Lobby and Quit; "VR WINS!" plus VR Menu on the chef side | confirmed (UI); buttons not wired (empty `m_Calls` in both prefabs) | BPO prefabs, Kitchen.unity |
| Earlier NVD menu: PC main menu, settings, lobby with Ready Status and 4 name slots | confirmed (NVD) | doc 03 |
| Players type a name before joining | proposed | A L2639 |
| Character select (faces, shapes, unlocks) | proposed | G L2254; A L2117 |
| Jan-2023 drawio: Lobby (lobby controls), Rounds System (end of round, timer/clock), Quest Manager/Tracker with separate Dumpling and VR quest displays | proposed | BPO drawio |

---

# 8. Audio and art direction

**Art direction**

| Finding | Status | Source |
|---|---|---|
| **Ghibli-like hand-painted**, mostly unlit look, with baked lighting and AO, made with Substance plus hand painting. Driven by Quest limits | confirmed | A L6–30; G L1123–1145 |
| Outlines only on characters, via inverted hull | proposed | A L20–27 |
| Prop budget under 500 tris; characters about 15–20k; real-world scale; assets face −Y; `Collider_[Prop]` naming | confirmed | A L627–644, L1283, L1455, L2736 |
| Walls: Vietnamese colonial, off-white with green mould. Floor orangey, no red. Warm light over realistic fluorescent | confirmed | A L339–348, L1581 |
| References: Mawaru Penguindrum backgrounds, the "Cozy Afternoon" two-tone palette, an "Asian Ratatouille" concept, Phnom Penh street kitchens | confirmed refs | A L716, L2679, L1116, L420 |
| Fallbacks once texturing stalled ("a bit of a bust", 27-Jun): Flat Kit or Quibli toon shaders (paid) | proposed | A L2420, L2708, L2755 |
| Chef: a girl in a hat, pin and apron; head sculpt in progress (`Art/Chef/Saoirse/*.png`) | in progress | Q L30; A L1955 |
| Dumpling: no final in-game model; restarted 26-Jul; a placeholder beige dumpling | not done | A L2671; G L2244 |
| Steamer lid ("hat") dimension notes | asset note | `Art/Notes/DumplingSteamerHatDimensions.png` |
| Interactable props: simpler textures so they read as interactive (Saoirse) vs subtle (Gavin) | conflict | A L715–720 |

The full who-modelled-what list is in the art-channel extraction. In short:
- **Saoirse:** floor, crate, cutlery, cleaver, gas canister, toaster, sinks, chef head, dumpling sculpts.
- **Gavin:** walls, pot, mortar, wok, steamer, counters, bin, pipes, fridge, rice cooker, fuses and fuse panel, dumpling.
- **Blerp:** mortar, pestle, bowl, shelves, rolling pin, cans, plate, all vegetables and egg, light switch, power socket, jug.

**Audio**

| Finding | Status | Source |
|---|---|---|
| Music and SFX to come from another class group; a full sound list and brief were to be written | proposed | Q L90–96; G L1271 |
| Background music from a friend of Blerp's (team doesn't make its own music) | dropped | P L262–267 |
| Implemented: Chef Wins music; "Updated audio" (`d982ba6d`) | confirmed | BPO |
| Split-screen audio-listener problem | open | P L623 |
| Electric SFX pack in the project (`Assets/Electric Sfx`), presumably for sparks from the fuses and plugs | asset present | BPO |

---

# 9. Production history

## Timeline

Team: Gavin, Lara, Kieran, Saoirse, Georgina. Tutors: Emily, Kerstin, David.

| Date | Who | What | Source |
|---|---|---|---|
| Oct-2021 | Gavin | `vr-asynchronous-input.mp4` saved; `DumplingVR/Assets/Scenes/SampleScene.unity` dated 15-Oct-21 | file dates |
| 04-Apr-22 | Lara | Big-Project repo created; Miro, GDD and HacknPlan set up | BPO `14d12557`; Doc L2–20 |
| 08-Apr-22 | team | Concept decided: VR chef vs dumplings | G L105 |
| 22-Apr-22 | Saoirse | 3 paper prototypes | G L555–562 |
| 04–06-May-22 | team | Sabotage brainstorm; recipes chosen; pitch to Emily | G L706–881 |
| 07/08-May → 08-Jun | Kieran, Lara | Mirror networking chosen, then dropped | P L27–88 |
| 15–22-May | artists | Art style; 1.5 × 2 m layout; greybox | A L1052–1494 |
| 31-May | Kieran | "Dumpling Madness" logo | G L1342 |
| 14-Jun | Lara | Fuse task and lamp script working (NVD era) | G L1503 |
| 11–15-Jun | — | PCVR only | A L2275 |
| 21–25-Jun | Gavin | Hat, dash and ingredient-steal ideas | Q L79–84 |
| 27-Jun–11-Jul | Gavin | Missed playable-demo dates; "3 weeks since we were meant to have a playable scene" | G L1644, L2011 |
| 16–25-Jul | Lara, Kieran, Gavin | NVD (VRIF), then DVR (Photon template); "Restaurant Ruckus" Quest/PC builds | P L144–459; `Builds/` |
| 19–25-Jul | Saoirse, Kerstin | Talk of changing the game; meeting to cut scope | V L372; G L2375 |
| 26–28-Jul | Gavin | Local VR + PC views; VR + gamepad; 1 VR + 4 local test | P L489–620; G L2453, L2586 |
| 29/30-Jul | Gavin | `DevelopXR` "BIG Commit", Unity 2021.3.1, URP, input schemes | BPO `f3a7c3fb`, `c8f4ce2e`, `16c9101d` |
| 31-Jul–02-Aug | all | Colliders and jump (Gavin); dumpling controller (Kieran); quests, timer, UI and win/lose (Lara); sinks and teleports (Saoirse); death/respawn, throw/teleport and lives (Kieran); PC lobby started | BPO `8cb496f7`…`f8070929` |
| 02–04-Aug | team, Emily | Team decides to switch games ("Slice of Knife"); Gavin says he'll keep making this game regardless | V L532–547; H L489–493; G L2777–2826 |
| 04-Aug | Kieran | Chef picks up dumplings; VR no longer spawns a player | BPO `0d822b97`, `b2c88955` |
| 04–14-Aug | Kieran | Climbing Game ("spiderman") prototype | BPO `8e931d33`…`7fa3938d` |
| 26-Jan–24-Feb-23 | Gavin, Kieran, Lara | **Second attempt:** programming-framework drawio; EzySlice cutting ("Slicing works!!"); "Unable to connect to VR game"; toaster model; full VR body (VRIK); `Kitchen VR Test - New Models.unity` | BPO `5b7c13b0`…`432db0df` |

## What worked when it stopped (Aug 2022)

On one PC with Quest Link:
- VR chef: grab props, grab and throw dumplings.
- 1–4 split-screen dumplings: move, jump, camera swap, teleport and throw points.
- Fuse gather-and-hide quest with a win screen.
- 3-minute timer.
- Shared lives with respawn.
- Chef ingredient score 0/6.
- Pause and task window.
- PC lobby UI.
- Hinged doors, which were buggy.

The props were mostly untextured. There were no final chef or dumpling models, and the win/lose logic still needed tweaking.

## Known bugs

- Door hinges "go crazy"; handles don't work (P L357, L461).
- Props flew off or exploded on load (P L358; G L1867).
- Some props were ungrabbable and disabled the hand: the wok, bottles, a can, the mortar (P L469–488).
- Dumplings tunnelled through walls and floor (P L648–694).
- Counter/wall gap (P L701).
- Win/lose evaluation runs on join (BPO `QuestManager.cs`).
- Hard-coded `GameObject.Find` names, e.g. hiding spots "1"–"6" (BPO `PickUp.cs`).
- `LightSwitch` emission typo, from doc 03.
- The death collider isn't placed in the scene.

## Backlog at the end

- Gavin's 30-Jul to-do image (G L2662). It's a Discord attachment, not in the folder.
- Mission framework with randomised, balanced, non-conflicting tasks (Emily, P L490–493).
- Lobby and timer (Kieran, H L453).
- Chop-and-steam with water (G L2334).
- Chef UI concept.
- Split-screen audio.
- Traversal tuning: teleports, launchers, grapple.
- Colliders and hinges.
- Textures and final models.
- Fridge handle.
- Character select.
- Results/replay wiring.

## Why it switched

- No playable build for months.
- Technology churn: Mirror → none → VRIF + Photon → local XRI.
- Paid-asset and SteamVR conflicts.
- Communication problems and illness.
- TAFE PCs unable to run the build.
- Tutors pushing for a smaller scope.

Sources: V L372; P L257; G L2468–2533, L2726; Rant.

---

# 10. Other things that matter for the rebuild

- **The `DevelopXR` tip is 2023, not 2022.** `432db0df` (24-Feb-23) includes the VRIK body and new-model test scene, and the Feb-2023 Kitchen.unity changes. For the 2022 game as it stood, use **`92aed0f3`**.
- **Reference games the team used:**
  - Cook-Out (main VR cooking reference) and Sep's Diner.
  - Takelings House Party: UI on the walls, a static VR player, play-space reviews.
  - Panoptic: "strong influence".
  - Acron: Attack of the Squirrels: party-game lag and respawn.
  - Late to Work: PCVR split-screen.
  - Rick and Morty: Virtual Rick-ality: play-area lesson.
  - Astro Bot, Tentacular, Moss.
  - Sources: G L245–415, L913–914, L1650.
- **Tutor rules worth keeping:**
  - Missions from a balanced, randomised, non-conflicting pool (Emily, P L490–493).
  - Chef tasks should have pick up → put down → complete steps (P L490).
- **Play-space rule:** no counter top bigger than its base, because the chef must not lean over unreachable areas (G L1650). Dumplings need places the chef must bend to see (G L1847).
- **The pitch deck isn't this game.** `Art/Pitch Deck.pdf` is for a different VR project (a woodwork health-and-safety trainer by another team) and has nothing on the dumpling game.
- **`Art/SliceOfKnife/` is the replacement knife game.** It was skipped except to confirm it isn't dumpling material.

---

# Differences from the rebuild

Rebuild assumptions are compared against the latest archive evidence. "Change" means the archive clearly disagrees. "Keep" means it agrees or is silent. "Your call" means the sources conflict or the original was unfinished.

| Rebuild assumption | Decision | What the archive says | Recommendation |
|---|---|---|---|
| **5-minute rounds** | **D7** | 3 minutes (180 s) in the last build (Lara, `a5d163ce`, 01-Aug-22). No other length was ever stated | **Change** the default to 180 s; keep it in `GameConfig` |
| Countdown 3 s, results 10 s | D7 | No countdown. Results screens wait for **Replay / Return Lobby / Quit** buttons | **Change:** results waits for input (Replay / Lobby / Quit); keep or drop the 3 s countdown as you like |
| **The chef wins instantly by serving 3 dishes; otherwise the dumplings win at time-out** | **D8** | The other way round. **Dumplings win by completing their mission (hiding all fuses) before time runs out**; at time-out (or when shared lives run out) the **chef wins**. The chef has a cooking score (6 ingredients into the pot = 0/6) but no instant-win was built. The win/lose wiring was unfinished | **Change.** Suggested rule: dumplings win when their mission(s) are done; the chef wins at time-out, when the shared lives are gone, or (your call) on finishing the recipe. The sabotage count shouldn't decide the winner |
| **The chef rings a service bell to start** | **D9** | No bell. Dumplings join with A, Space or a click into a PC lobby ("Ruckus in the Restaurant", Player 1–4 Ready, Start Game); the timer starts on load | **Your call.** The bell is a nice diegetic start for the chef, but add the 4-slot ready-up lobby on the monitor |
| Sabotage fixed by the chef grabbing or pressing it; **5 s cooldown** | **D10** | Grab or press fits: toaster button or extinguisher, fuse box flipped back, replace knocked food, retrieve hidden fuses. **The hat fix is a wiggle** or a timer, not a press. No cooldown appears anywhere | **Keep** grab/press. **Change** the hat to a wiggle. The 5 s cooldown is new: fine to keep, but mark it as a rebuild addition |
| Chef's hat = **full blindfold** quad | **D11** | The visor or hat drops over **one eye**, on the side the dumpling landed, for a set time or until wiggled (Gavin, Q L79) | **Change** to one-eye occlusion plus a wiggle or timeout |
| **Serving:** cooked ingredients put on a counter at once; orders loop | **D12** | No serving or plates. The chef **puts chopped ingredients into the pot**. Tasks: "Fill pan with water / Chop 3 Vegetables / Place chopped vegetables in pan". Recipes: wonton noodle soup, dumpling soup with vegetables. Jan-2023: chop potato, carrot, onion → put veg in pot → **put dumplings in pot**. A `Service_Counter` exists in the kitchen | **Change** to a **pot-based recipe with steps** (water → chop → into the pot). Optionally finish by ladling to the service counter. Consider "put a caught dumpling in the pot" as the capture method (G L557) |
| **Cooking = heat-seconds** in a pot on a stove | **D13** | No heat model was built. Cooking was chop + add to the pot. Gas and stove only appear as sabotage targets | **Your call.** Heat-seconds can stay, but add **chopping** (cutting was prototyped twice) and the water step; that is the original core loop |
| **Starter sabotages:** gas valve, light switch, power plug, knock-over, chef's hat | **D14** | Matches the asset list (gas canister, light switch, Asian socket, knock-overs, hat idea). **Missing the only fully built mission, the fuse heist** (6 fuses → 6 hiding spots, chef retrieves them). Also proposed: toaster fire, salt in sugar, tap, steal ingredients, recipe book, can opener, fan, sauces, mortar | **Change:** add the **fuse heist** as the main win objective (taking fuses kills the lights). Treat the 5 instant sabotages as harassment between objectives. Toaster is the best next addition |
| No lives or respawn (not in the D list) | — | **A shared pool of 10 lives**; caught/dead → respawn at a spawn point; last life = chef wins. HUD with happy/scared face | **Add** shared lives plus respawn |
| No movement aids (not in the D list) | — | **Activate points:** teleport pipes/drain and launch points; double jump; secret passages; camera swap. Proposed: pedal-bin launcher, sponge, dash | **Add** teleport and launch points; consider the Camera Swap button |
| Rigidbody dumplings | D16 | The same (Rigidbody, rotation frozen). The known bug was tunnelling at speed | **Keep;** use continuous collision detection |
| Hand-written orbit camera | D17 | Third-person orbit **plus a first-person mode**, swapped with Y or Q | **Keep;** optionally add the swap |
| `IDumplingInput` | D18 | — | Keep |
| Split screen with `PlayerInputManager`, up to 4, join with **Start** or Enter, joining closes mid-round | **D19** | Split screen with `PlayerInputManager`, max 4, join action **A** (gamepad South), Space or left click, through a dedicated `JoinButton` action that **excludes XR controllers** (the 04-Aug fix) | **Keep;** the button choice is cosmetic. **Make sure XR devices can't join** (bind the join action only to gamepad and keyboard/mouse) |
| Headset not mirrored to the monitor | D20 | Identical: `DisableView.cs` sets `XRSettings.gameViewRenderMode = None` | **Keep** (confirmed original) |
| Then networked, with the chef as host | **D21–D29** | The original intent was explicitly **local only** ("no networking", G L618; "original split screen idea", P L58). Mirror and Photon attempts were abandoned. Gavin's Quest-as-host worry (P L57–60) agrees with chef-as-host if networking happens | **Keep as a stretch goal** (Part 2 is new scope, not a recreation). D28 (no mid-round joins) is fine; the original allowed joining any time but used a lobby |
| XR only for the chef | D25 | n/a in local mode | Keep |
| OpenXR + XRI only | D4 | Original: XRI 2.0.2 + **Oculus XR Plugin**; OpenXR added in 2023 | Keep OpenXR |
| Working title "Dumpling Kitchen" | D5 | "Dumpling Madness" (logo), "Ruckus in the Restaurant" (lobby), "Restaurant Ruckus" (build) | **Your call:** "Dumpling Madness" was the team's working title |

**Also worth carrying over**
- The 1.5 × 2 m play space with about 25 cm counters.
- The fuse panel placed **behind the chef** above the service counter.
- The "Chef Madness" meter (its rules were never pinned down).
- Diegetic chef UI: a task board and a wall clock; no head-locked HUD.
- The chef's body blocking the dumplings' view: Gavin's "ghost body" (Q L44–64).

---

# Files I couldn't read (or only partly)

| File | Why | Suggestion |
|---|---|---|
| `Discord Notes.clip`, `Programming Layout.clip`, `Art/Notes/Layout sketch.clip` | Clip Studio Paint binary; the embedded preview is corrupt or unreadable | Export each canvas as PNG from Clip Studio |
| `Pitch Sketches.clip` | Only a tiny thumbnail is embedded (about 4 KB) | Export as PNG |
| `Programming notes - 29th July.clip` | Not needed; read via `Art/Notes/Programming notes - 29th July.png` | — |
| `Art/Textures/*.clip`, `Art/SliceOfKnife/**/*.clip` | Binary and/or the replacement game | Skip, or export if wanted |
| `vr-asynchronous-input.mp4` (57 s) | Video; I sampled stills only (no audio or narration) | Watch it, or give me a transcript or key timestamps |
| `AsymKitchen/AsymmetricKitchen/AsymmetricKitchen.zip` (478 MB) | Huge archive; the project beside it is the paid template + Photon | Skip, unless it holds something unique |
| `Builds/*.apk`, `Builds/RestaurantRuckus/` | Compiled builds; only metadata, scene list and type names inspected (Photon2Lobby/Room template build, 25-Jul-22) | Run `RestaurantRuckus.exe` if you want to see the Photon version |
| `.blend`, `.blend1`, `.fbx`, `.obj`, `.f3d`, `.dxf`, `.spp`, `.ai`, `.ma`, `.svg` models and artwork in `Art/` | 3D/design binaries | Not needed for design; open in Blender, Fusion or Illustrator if required |
| `Art/Chef/Saoirse/unknown2–8.png` | Viewed only `unknown1` (chef head sculpt in progress) | Low priority |
| `Discord_Graph_A/B.png`, `Example Modular Code Diagram.png`, `Art/ProgrammingFrameworkDumpling.drawio.png` | Not viewed: participation graphs, Kieran's MCC (covered by 03), and a render of the drawio I decoded from git | — |
| `The Big Project - Text Channels - programming [...].html` | Duplicate of the CSV | — |
| Discord attachments, which are links only and aren't in the folder: Gavin's 30-Jul to-do list (G L2662), the 28-Jul 1 VR + 4 player video (G L2586), the 26-Jul VR / 4-camera screenshots (G L2453–2454), 29-Jul planning notes ×3 (P L629), the trailers (G L2834–2858) | Not in the archive; I didn't fetch them | Download them from Discord if they still exist. **The to-do image and 28-Jul video are the most valuable** |
| External documents: the GDD and TDD Google Docs, asset list and sprint sheets, Saoirse's scope-down doc, the HacknPlan board, the Miro board, the `VR.zip` on the Poppycock Google Drive (P L601) | Not in the folder | Export the GDD, HacknPlan and Miro if you still have access. The GDD is likely the richest missing source |

## Secrets and personal data (locations only, values not copied)

- **Credential:** `The Big Project - Text Channels - art [...].csv` lines 213–215 and 221 hold a private file-server address with a shared username and password.
- **Personal email addresses:**
  - `... - documentation [...].csv` L88;
  - `... - general [...].csv` L12 and L22;
  - `... - vc [...].csv` L2;
  - the git author metadata of all four repos.
- **Home address and phone number:** `... - general [...].csv` L2213–2220.
- **Full names:**
  - `Art/Pitch Deck.pdf` (the other team);
  - git author names in BPO.

  Neither is reproduced here.
- **Sharing and invite links:** Google, Figma, Teams and SharePoint links in G, P and V. Scrub them before publishing any export.
