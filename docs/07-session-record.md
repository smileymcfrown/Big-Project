# 7. Session record: everything said and decided

A full record of the Claude Code session that produced this `docs/` folder. It ran from 9 to 17 October 2026 on branch `claude/vibrant-cerf-xk6ce6`. It covers what you asked, what I answered, every decision (yours and mine) with the reason for it, the estimates, the mistakes I made and corrected, what was and wasn't tested, and what's still open. Use it to pick the work up without the chat, or hand it to a future session.

Dates are UTC, taken from the session log.

## Where things stand

- **The plan (your decision, 15 Oct):** build the game exactly as the tutorial describes, then go through the original project's notes ([`05`](05-archive-findings.md)) and choose what to bring in, using [`06`](06-bringing-in-original-ideas.md) as the map.
- **Deliverables:**

  | Thing | Where |
  |---|---|
  | Build guide (18 chapters + appendices) | [`tutorial/dumpling-kitchen-tutorial.html`](tutorial/dumpling-kitchen-tutorial.html); published privately at https://claude.ai/artifact/K3B77TH2PHemDgxFRkTd8i |
  | Gamified quest board (9 sessions, XP, ranks, guide built in) | [`tutorial/dumpling-kitchen-quest.html`](tutorial/dumpling-kitchen-quest.html); published at https://claude.ai/artifact/MqqigCt4LV7EsPx9MkUeHj (you shared it as "anyone with the link"; each person's progress stays private) |
  | All tutorial scripts as real files | [`tutorial/code/part1-local/`](tutorial/code/part1-local/), [`tutorial/code/part2-networked/`](tutorial/code/part2-networked/) |
  | Notes 01–06 and the decisions list | this folder; index in [`README.md`](README.md) |

- **Branch state before this record:** last commit `a1bcfc1a`, everything pushed. No pull request has been opened (none was asked for).

## Who's who

These are the names that appear in the old repos and the archive. Details are in [`05`](05-archive-findings.md) under "How to read this".

- **Gavin (smileymcfrown):** you. You built the working VR + 4-player local co-op.
- **Lara (Lala-banners):** programmer. Mirror lobby, networked experiments, game timer.
- **Kieran:** programmer. Modular character controller, quest system, light switch.
- **Saoirse:** art lead and organiser.
- **Georgina:** artist.
- **Emily:** tutor.

---

## Timeline of the conversation

### 1. The original request (9 Oct)

**You asked:**
- Review this repo, an unfinished Unity VR game from about 2022, and make a `docs` folder for notes.
- Say whether it can easily be upgraded to the latest Unity, or whether it's better to start again given how much VR has changed.
- Then write either an upgrade tutorial or an HTML tutorial for remaking the game. Also check `smileymcfrown/NewVRDumpling`, because you weren't sure which repo had the working asymmetric VR/PC setup.

**The game:** an HTML tutorial for an asymmetric PC/VR game. The VR player is a chef cooking a meal, and the PC players are dumplings who sabotage the chef: unplugging things, knocking things over, switching off lights, turning off the gas, knocking the chef's hat over their eyes.

**The goal:** get back into game development after 4 years. Learn best practices for setting up a Unity project, where scripts go, and core game programming with good OOP. Start with 5-minute rounds. Don't worry about game details; you'd adjust those later.

**How to work:** ask initial questions, then work without your input, and note every decision for you to review.

**My three questions and your answers:**

| Question | Options I gave | Your answer |
|---|---|---|
| How should the chef and dumplings connect? | Networked (recommended) / Local couch on one PC / Both, local first | **Both, local first** |
| Which headset? | Quest via PC Link / SteamVR headset / Quest standalone / Not sure | **Quest 2, set up for both Air Link and SteamVR** ("that can also run through SteamVR") |
| Which networking library? | Netcode for GameObjects (recommended) / Mirror / Photon Fusion | **Netcode for GameObjects** |

**What I did:**
- Surveyed both repos (branches, commits, authors, scenes, scripts, packages).
- Checked current Unity, XR and networking versions.
- Verified the tutorial's API calls against the real package sources (NGO 2.13.3, XRI 3.6.1, Input System 1.20.1, Multiplayer Services 2.4.0, XR Management 4.6.1).
- Wrote the C# for both parts.
- Compile-checked everything with a real C# 9 compiler against hand-written stubs of the Unity APIs.
- Ran the 15 unit tests (all pass).
- Wrote [`01`](01-repo-assessment.md), [`02`](02-upgrade-or-rebuild.md), [`DECISIONS-TO-REVIEW.md`](DECISIONS-TO-REVIEW.md), the 18-chapter guide and a build script.
- Published the guide privately.
- Commit `785d49ea`.

**What I told you:**
- **Start again, don't upgrade.** Use Unity 6.3 LTS and bring the art across. The VR layer (SteamVR plugin, Oculus Integration) has been replaced by OpenXR + XR Interaction Toolkit. The networking (Mirror 32) would be rewritten anyway, since you chose Netcode for GameObjects. Upgrading would mean deleting exactly the parts that worked. NewVRDumpling's 44 FBX models are the art worth keeping (Big-Project's 32 are a subset).
- **The tutorial follows your answers:**
  - Part 1 is a couch game on one PC: the chef in the headset and up to 4 split-screen dumplings. It covers setup, Git, folders, assembly definitions, code style, the round, cooking, dumpling movement, and five sabotages.
  - Part 2 converts it to networked play with the chef as host, joining over LAN or by join code. Only five gameplay files change. The UI, the dumpling code and the sabotage classes stay as they were, which shows why the Part 1 structure pays off.
- **Tested:** compiles, and the tests pass. **Not tested:** it hasn't been run in the Unity editor or on a headset.
- **Two claims I later corrected** (see "Corrections" below): that NewVRDumpling's "It works!" commit was your asymmetric setup, and that Big-Project never had a VR player.

### 2. Attribution correction; paid assets and Photon dropped (9 Oct)

**You said:**
- The "It works! VR and non VR player in same scene" commit was probably Lara's, not yours. You got asymmetric 4-player local co-op working **without a paid template**.
- It doesn't matter much since everything is being rebuilt.
- **"Forget all paid assets and Photon; I will not be using them."**

**What I did:**
- Corrected the attribution in the docs.
- Removed every paid-asset and Photon reference from the docs, the tutorial and code comments, along with the to-dos about them (licences, the committed Photon App ID).
- Reworded D4: OpenXR + XRI only, no paid assets, no Photon.
- Changed the "Then → now" notes to use Big-Project's own code as the "then" example.
- None of the tutorial code ever used paid assets or Photon, so no code changed.
- Commit `f1eed6d2`; guide republished.

### 3. How long the tutorial takes (9 Oct)

**You asked** how many hours it would take an average person with basic programming knowledge.

**My answer:** 50–75 hours for that person, about 2–3× the chapter time labels (which add up to about 22 h of typing time for someone with no problems).

| Section | Label total | Realistic for a beginner | Where the extra time goes |
|---|---|---|---|
| Setup (ch. 1–5) | ~3.5 h | 6–10 h | Installs; headset trouble (Link and SteamVR runtimes, drivers, Air Link quality) |
| Core game (ch. 6–11) | ~10 h | 20–30 h | Building the scene and prefabs, wiring Inspector references, tuning physics, learning C# events and interfaces |
| Networking (ch. 12–17) | ~8.5 h | 20–30 h | Network bugs are slow to find (ownership, spawning, host vs client behaviour); Relay setup |
| Next steps (ch. 18) | 15 min | — | Optional reading |

**For you specifically:** 30–45 hours. Part 1 alone is about 2–3 weekends.

**What decides where someone lands in the range:** previous Unity experience (10–20 h more without it), copying versus typing out the code, and debugging time. The code hasn't been run in Unity, so expect a few hours of setup slips.

### 4. The gamified quest board (9 Oct)

**You said:**
- You remember the basic editor layout and already program well; you just need reminding of the syntax.
- You always type out code and are reasonably fast.
- Assume the debug time I'd estimated, and 5 hours a day.
- Make a gamified version that pushes you forward and levels you up, with a deadline or goal, because that matters for your ADHD.

**What I built:** a separate page ([quest board](https://claude.ai/artifact/MqqigCt4LV7EsPx9MkUeHj), also `tutorial/dumpling-kitchen-quest.html`). Commit `ebf81a73`.

**The plan:** 9 sessions × 5 h = 45 h.
- 45 h is the top of my 30–45 h estimate for you, because typing everything out adds time; debugging is included.
- Working Monday to Friday from 9 Oct, the final session landed on **Wed 21 Oct**.
- The start date and workdays are settings. The schedule is calculated from the day you first open the board, and "Replan from today" moves it.

| Session | Name | Boss fight |
|---|---|---|
| 1 | Mise en place (setup + VR) | Grab a cube via Link **and** via SteamVR |
| 2 | Prep station (art, round timer) | Ring the bell; the 5:00 countdown runs |
| 3 | Fire up the wok (cooking) | Cook and serve a first dish |
| 4 | Dumplings, assemble | Drive a dumpling with keyboard and gamepad |
| 5 | Kitchen chaos (sabotage) | Gas goes off; the chef turns it back on |
| 6 | Couch service | A full round with a real second player; tag `v0.1-local` |
| 7 | Wire the restaurant | Host as chef; a second player connects |
| 8 | Networked kitchen | Sabotage seen across two windows |
| 9 | Grand opening | A full online round with friends |

**Design choices and why:**

| Choice | Why |
|---|---|
| **One "next ticket" at a time**, in big type, with a Done button (the full checklist is optional) | You never have to decide what to do next, which is where ADHD focus tends to leak |
| **XP and 10 chef ranks** (Kitchen Porter → Michelin Star). XP: step 10, boss 250, quest 50, day 100, on-time 50, sprint 20, bug fixed 15 | Constant small rewards. Session 1 alone earns about two promotions, so the rewards come early, while motivation is being built |
| **Promotion chime and confetti** | A clear moment of reward |
| **Focus sprints** (25/5 or 45/10) showing the current step, with hours counted against the 5 h goal | Time-boxing plus visible progress toward the daily target |
| **Deadline countdown** ("On track" / "Ahead by N" / "Behind by N") | The deadline you asked for |
| **"Replan from today" with no penalty**, and an achievement for using it | Slipping shouldn't turn into quitting |
| **Escape hatches:** "Park it for later", a brain dump, a "Stuck?" box, XP for logged bugs | Being stuck is where people drop off; debugging counts as progress |
| **Real-life rewards** you choose, for sessions 3, 6 and 9 | Rewards outside the screen |
| **Progress saved privately to your claude.ai account** (`db` + `user` capabilities). The repo copy saves in the browser only | Syncs between PC and phone; nobody else sees your progress |

**Tested:** in a headless browser, at desktop and phone widths, in light and dark. That run found and fixed two bugs (see "Bugs" below). Live syncing between devices only works on claude.ai and couldn't be tested from here.

### 5. "Open chapter" did nothing (10 Oct)

**You reported** that clicking "Open chapter" on the quest board did nothing, and asked whether you should use the HTML files on GitHub instead.

**Cause:** the claude.ai viewer doesn't follow links from one artifact to another.

**Fix:**
- The whole guide is now built into the quest board. "Open chapter" opens it in a side panel at the right chapter (full screen on a phone).
- Close or Esc closes it, including while you're reading inside the panel.
- In-guide links jump instantly.
- Commit `74426056`.

**About GitHub:** you don't need the GitHub copies. GitHub shows HTML as source code, not as a page. Downloaded and opened in a browser, the files work the same way.

**Also noted:** you had changed the quest board to "anyone with the link". Others can open it, but their progress is private to their own accounts.

### 6. Old code quality, original missions, and access to your local files (13 Oct)

**You asked** three things:
- Was the old game built with good OOP and Unity practices?
- What missions or goals existed for the dumplings and the chef?
- How could you give me access to your local repos and documents? You want the remake to be as close as possible to the original concept.

**What I did:** searched both repos' code, scenes, prefabs and assets, and wrote [`03`](03-original-design-and-code-review.md). Commit `642b084f`.

**Code quality:** a typical capable student team, with one standout.
- **Kieran's modular character controller** was the best code: small components on a shared base class, settings in data assets, well commented. Its flaw is that Photon and a paid inspector tool are wired into its core class.
- **Lara's Mirror lobby** had decent habits. Its problems: one class doing too much, scenes referred to by name (which broke it after the rename), and no server checks on movement.
- **The gameplay code was prototype stage, with real bugs:**
  - The quest UI read goal values from the prefab, so it never showed live progress.
  - A shader property typo (`_EmissionColour`) meant the bulb glow never changed.
  - The cutting script looked for `"Butcher Knife"`, but the prop is named `Prop_Butcher'sKnife`.
  - Objects were found by name everywhere.

**Missions found:**
- The game's title was "Dumpling Madness VR".
- The dumplings had a **fuse heist** (Gather Fuses, then Hide 4 Fuses, after which the chef can retrieve them), with waypoint markers and a task UI.
- Other ideas: a light switch, launch pads, dumpling selection, first- and third-person cameras, cutting a cucumber, a wrist menu, and a fully interactive kitchen.
- No recipes, win conditions or round length were in either repo.

**Access:** two routes.
1. Push the material to GitHub and connect it to this session.
2. Run Claude Code on your own computer in the folder that holds it.

I suggested sharing only the local co-op repo, any design document, and a Discord export.

### 7. Running Claude Code locally (13 Oct)

**You asked** how to switch to Claude Code on your computer, and whether you need a local clone.

**My answer:**
- You can stay in the cloud if the material goes on GitHub.
- For files that are private or too large, install the Claude desktop app (or the CLI) and open it on the folder. It reads files where they are.
- Clone Big-Project into the same parent folder only if you want the local session to write straight into these docs.
- `claude remote-control` lets you drive a local session from the app.
- This cloud session can't see your computer, so local work always happens in a separate session.

### 8. The prompt for the local session (15 Oct)

**You asked** for a prompt to give a local session with access to your archive.

**What I wrote:** [`04`](04-local-archive-prompt.md) (commit `e8d5e379`).

**Rules in the prompt:**
- Read only.
- Skip third-party folders.
- Ignore how paid assets and Photon worked.
- Never copy secrets or personal details; give file locations only.
- Cite a source for every finding and mark it confirmed, proposed or dropped.

**What it asked for:** a report with a one-page summary, ten topic sections, a "Differences from the rebuild" table keyed to the decision numbers, and a list of files it couldn't read.

**Two tips I gave:** clone Big-Project alongside the archive so the report lands in `docs/`, and export the Discord channel first.

### 9. The archive findings and the compatibility check (15–16 Oct)

**The local session's report:** it wrote [`05`](05-archive-findings.md), which you pushed as `062e3ffe`. It was one document plus a README update, not two documents. You said the original ideas may not have been that solid. You prefer 5-minute rounds to the original 3, and you think several tutorial choices, such as the service bell, are better.

**You asked** me to check that nothing in how the tutorial builds and sets up the game would cause major problems when bringing in original ideas. If not, you'd build the tutorial as written and then pick from the original notes.

**What I did:**
- Read the report in full.
- Read the Feb 2023 architecture diagram on `DevelopXR`.
- Checked each original idea against the tutorial code.
- Wrote [`06`](06-bringing-in-original-ideas.md).
- Fixed one snag (below).
- Corrected `01`, chapter 1 of the guide, `README` and `DECISIONS`.
- Rebuilt and republished both pages; tests still pass.
- Commit `a1bcfc1a`.

**My answer:**
- **No major issues. Build the tutorial as written.** Every original idea goes into an existing extension point: a new sabotage subclass, a new dumpling interactable, a data asset, or a change inside one class.
- **One fix:** `DumplingCamera`'s minimum distance was 0.5 m, which rules out the original 5 cm dumplings. It's now 0.05 m.
- **Scale note:** chapter 9 now has a "tiny dumplings need tiny numbers" box. The original team had dumplings tunnelling through walls at speed, so it gives starting values: speed, jump, camera, reach, continuous collision and the physics contact offset.
- **Things that need care**, all in `06`: the fuse heist needs a carry system and one owner for the kitchen lights; a grabbed dumpling's motor must pause; the one-eye hat needs a custom shader; chopping should use prefab swaps; and in Part 2 the chef's grab needs an ownership change.
- **Corrections:**
  - Your working local co-op build is on Big-Project's `DevelopXR` branch (best snapshot `92aed0f3`), which I missed by only looking at `Master`.
  - Your choices to keep D7 (5 minutes) and D9 (bell) are recorded in `DECISIONS`.
- **Warning:** the archive report found a shared file-server password and personal details in your Discord exports. It records only where they are. Scrub them before sharing those exports.

### 10. This record (17 Oct)

**You asked:** "Document everything including what has been said, decisions made with reasons why, etc."

**What I did:** wrote this file.

---

## Decisions

### Yours

| Decision | When | Your reason |
|---|---|---|
| Local couch game first, then networked | 9 Oct | Chosen from my options |
| Quest 2, set up for both Air Link and SteamVR | 9 Oct | You'll use a Quest 2, which can also run through SteamVR |
| Netcode for GameObjects | 9 Oct | Chosen (my recommendation: Unity's first-party, free, well documented, works with Relay) |
| **No paid assets, no Photon** | 9 Oct | You won't be using them |
| Pace: 5 h/day, typing out all code, with the debug time I'd estimated | 9 Oct | Basis for the quest board's schedule |
| **Keep 5-minute rounds** (D7) | 15 Oct | You think 5 is better than the original 3 |
| **Keep the service bell** (D9) | 15 Oct | Better than the original's timer starting on load |
| **Build the tutorial as written, then choose original ideas to bring in** | 15 Oct | The original ideas may not have been that solid; several tutorial choices are better |

### Mine: D1–D29 with their status

Full reasons and alternatives are in [`DECISIONS-TO-REVIEW.md`](DECISIONS-TO-REVIEW.md). The "Archive says" column summarises the comparison in [`05`](05-archive-findings.md). "Open" means you haven't decided yet.

| # | Decision | Why I chose it | Archive says | Status |
|---|---|---|---|---|
| D1 | Rebuild in a new project | The VR SDKs and networking both repos used have been replaced; little of our code is worth keeping | — | Agreed (you're building the tutorial) |
| D2 | Unity 6.3 LTS + NGO 2.13.x | 6.3 is the current LTS. NGO 3.x needs Unity 6.7, which is in beta | — | Fixed by your plan |
| D3 | Start from the URP 3D template, not the VR template | You learn each XR piece by adding it yourself | — | Open |
| D4 | OpenXR + XRI 3.6 only | One build runs on Link and SteamVR; no Meta-only features are needed for PC VR | Original used the Oculus XR plugin | Fixed by you (no paid assets or Photon) |
| D5 | Working title "Dumpling Kitchen", namespace `DumplingKitchen` | Needed a name for folders and assemblies | The team used "Dumpling Madness", "Ruckus in the Restaurant" and "Restaurant Ruckus" | Open |
| D6 | One runtime assembly + an EditMode test assembly | Faster compiles and testable code, without assembly-reference juggling | — | Open |
| D7 | 5-minute rounds, 3 s countdown, 10 s results | Your starting brief | 3 minutes; results wait for Replay / Lobby / Quit | **5 min kept by you.** Results waiting for input is in `06` |
| D8 | Chef wins instantly on 3 dishes served; dumplings win at time-out | Gives both sides a clear goal using the starter systems | The other way round: dumplings win by completing the mission; the chef wins at time-out or when lives run out | **Open.** `06`: a few lines in `RoundManager` |
| D9 | Chef rings a service bell to start | A diegetic start for the VR player | No bell; PC ready-up lobby | **Kept by you.** Ready-up lobby can be added (`06`) |
| D10 | Sabotage fixed by grab/press; 5 s re-arm | Simple, physical, fits VR | Grab/press matches; the hat used a wiggle; no cooldown existed | Open |
| D11 | Hat = full blindfold until pushed back | Simplest occlusion | One eye only, on the side the dumpling landed; fixed by a wiggle or a timer | Open. `06`: shader + wiggle detector |
| D12 | Serve cooked ingredients on a counter; orders loop | A complete loop with the minimum systems | Pot-based steps: water → chop → into the pot | Open. `06`: medium effort |
| D13 | Heat-seconds cooking | Teaches pure logic and tests | No heat model; chopping was the core | Open |
| D14 | Five starter sabotages | Your examples from the brief | Add the fuse heist as the main objective | Open. `06`: carry system + `KitchenLighting` |
| D15 | Small `RoundManager` singleton + static events | Easiest to follow when returning to Unity; statics reset on Play | — | Open |
| D16 | Rigidbody dumplings | Being shoved and thrown is the fun | Same; tunnelling was a known bug | Keep (continuous collision added to the scale note) |
| D17 | Hand-written orbit camera, not Cinemachine | Cinemachine 3 split screen is a lot on day one | Also had a first-person swap | Keep. `06`: one camera for the swap |
| D18 | Input through `IDumplingInput` | Motor and camera don't care where input comes from | — | Keep |
| D19 | `PlayerInputManager` split screen, up to 4 | The standard Unity way | Same; join excluded XR controllers | Keep; bind join to gamepad and keyboard only |
| D20 | Headset not mirrored to the monitor | Saves GPU; the monitor is for dumplings | Identical (`DisableView.cs`) | Keep (confirmed original) |
| D21 | Chef always hosts | VR grabbing with zero lag; no prop ownership transfers | Original was local only | Part 2 is new scope |
| D22 | Owner-authoritative dumpling movement | Responsive PC controls | — | Part 2 |
| D23 | Server re-validates every sabotage | Never trust the client | — | Part 2 |
| D24 | Part 2 converts the project; local lives on as tag `v0.1-local` | Supporting both at runtime doubles testing | — | Part 2 |
| D25 | XR starts only for the chef | Dumpling PCs shouldn't need SteamVR | — | Part 2 |
| D26 | LAN (port 7777) and Relay join codes | LAN is easy to debug; Relay avoids port forwarding | — | Part 2 |
| D27 | Bootstrap → Menu → Kitchen scenes | Avoids duplicate NetworkManagers | — | Part 2 |
| D28 | No joining mid-round; max 4 dumplings | Fairness and simple spawning | Original used a lobby | Part 2 |
| D29 | No host migration, reconnects or voice | Scope | — | Part 2 |
| H1 | Ask the original artists before using their models publicly | Their work | — | **For you to do** |

### Other decisions I made, and why

**Project and tools**
- **Graphics API D3D11, not Vulkan** (chapter 4). Open Unity issues exist where SteamVR + Vulkan exits Play mode. For the SteamVR + Link crash in some Unity 6 versions, the guide says to turn on SteamVR's Meta compatibility option or use the Meta Link runtime for editor work.
- **Bring the art across from NewVRDumpling**, copying FBX files without their `.meta` files and converting materials to URP. Its 44 models are a superset of Big-Project's. `Kitchen_Populated.prefab` is used as a layout reference only, because its links point at the old project.
- **Not touching NewVRDumpling.** You only asked me to check it. No pull request either, since you didn't ask for one.

**How the tutorial is built**
- **HTML generated from chapter sources plus the real `.cs` files** (`build.py` replaces `<!--@code ...-->` markers with the file contents). The page and the code can't drift apart, and the scripts also exist as files you can copy into a project.
- **"Review D#" boxes** in the guide at each place a decision matters, and **"Then → now" boxes** that compare old Big-Project code with the new pattern. They connect the guide back to your review list and to your old code.
- **The code teaches OOP on purpose:**
  - `SabotageTarget` is a template method that every sabotage subclasses.
  - Interfaces (`IHeatSource`, `IDumplingInput`, `IDumplingInteractable`).
  - ScriptableObjects for data.
  - A plain C# `RoundTimer` and pure `CookingLogic` that unit tests can cover.
  - Static events reset on Play.

  These were your "best practices" brief.
- **Verification without the Unity editor:** compile against stubs, run the pure-logic tests, and check API names against downloaded package sources. That's what was possible in a cloud container. It catches wrong API names and C# errors, not scene setup mistakes.

**From the compatibility check (`06`), each with its reason**
- **Chopping by prefab swap, not runtime mesh slicing.** It's simpler, and runtime-sliced meshes can't be networked in Part 2.
- **One `KitchenLighting` owner** (lit = switch on AND power on). Without it, flicking the switch back on would light a kitchen whose fuses are gone.
- **A "held" flag on `DumplingMotor`.** The motor sets velocity every physics step and would fight the chef's hand.
- **`DumplingMotor.SuspendControl(seconds)` for launchers.** Otherwise the motor brakes a launch to walking speed almost at once.
- **One camera for the first/third-person swap.** Split screen only resizes the camera `PlayerInput` knows about.
- **One-eye hat with a `unity_StereoEyeIndex` shader.** Single-pass instanced rendering draws both eyes at once, so a normal quad can't cover only one.
- **Part 2 only: `NetworkObject.ChangeOwnership`** when the chef grabs a dumpling. Dumplings are moved by their owner, but the host holds them while grabbed.
- **Scale the server reach tolerance** (`IsInReach` has a fixed 0.75 m allowance) if you use 5 cm dumplings.

**Privacy and security**
- **Secrets and personal data from the archive were never copied.** `05` lists only their locations. The local session's prompt required this.

---

## Corrections: things I got wrong

| What I said | What was true | Found by | Fixed in |
|---|---|---|---|
| The "It works!" asymmetric commit in NewVRDumpling was your setup, on a paid template | It was Lara's networked experiment; you built yours without a template | You (9 Oct) | `f1eed6d2` |
| Neither repo held your local co-op build; Big-Project never had a VR player | It's on Big-Project's `DevelopXR` branch. I only reviewed `Master` | The local archive session (16 Oct) | `a1bcfc1a` (correction boxes in `01`, `README`, chapter 1) |
| The old quest asset referenced *scene* objects | It referenced goal components on the *prefab*, which Unity allows; the bug is that the UI reads the prefab's values | Me, re-checking before sending (13 Oct) | `642b084f` |
| "Open chapter" links would work between artifacts | The viewer doesn't follow artifact-to-artifact links | You (10 Oct) | `74426056` |
| The other session had committed two documents | It was one document (`05`) plus a `README` update, and it hadn't been pushed yet when you first told me | The other session's message (15 Oct) | — |

## Bugs found and fixed

| Bug | Fix |
|---|---|
| A Mermaid state diagram didn't render: the colon in the "5:00 runs out" label broke the syntax | Reworded the label; escaped `<<interface>>`; removed `<br/>` |
| Repo copy of the quest board: the hidden level-up overlay blocked every click, because `.levelup{display:grid}` overrode `[hidden]` | Added `[hidden]{display:none!important}` |
| Quest board scrolled sideways on phones (HUD too wide) | Hid the rank subtitle on small screens; let the label wrap |
| "Open chapter" did nothing on claude.ai | Built the guide into the quest board as a side-panel reader |
| Smooth scrolling made in-guide links slow | Switched to instant scrolling |
| Esc didn't close the reader while focus was inside it | The guide now tells the board to close |
| `DumplingCamera` couldn't sit closer than 0.5 m | Minimum lowered to 0.05 m |

## What's been checked and what hasn't

- **Checked:**
  - All C# compiles with a C# 9 compiler against stubs.
  - API names and signatures match the real package sources.
  - 15 EditMode unit tests pass (`RoundTimer`, `CookingLogic`).
  - Both pages render at desktop and phone widths, in light and dark.
  - The quest board's step ticking, parking, bug logging, XP totals and achievements work.
  - The embedded reader opens at the right chapter.
  - Your private progress store is reachable.
- **Not checked:**
  - Nothing has run in the Unity editor or on a headset. Expect setup slips such as a missing Inspector reference or an unticked layer.
  - Live progress sync between devices.
  - The reader inside the real claude.ai viewer (I couldn't load it from here).
- **Not in the repo:** the compile-check stubs and the downloaded package sources lived in this session's temporary scratchpad and will be gone when the container is reclaimed. Rebuilding the pages needs only `python3 docs/tutorial/build.py`.

## Open items for you

1. **Build the tutorial**, using the quest board to pace it.
2. **Decide the remaining game rules after the tutorial:** D8 (who wins how), D10–D14 (fixes, hat, serving, cooking, sabotages), D5 (title). `06` has the suggested order:
   1. Scale.
   2. Lives and win rules.
   3. Fuse heist.
   4. Pot recipe.
   5. Activate points.
   6. One-eye hat.
   7. Lobby.
   8. Mission pool.
3. **H1:** ask the original artists before using their models publicly.
4. **Scrub the Discord exports** before sharing them anywhere. They contain a file-server login and personal details; `05` lists the line numbers.
5. **Optional, from `05`:** export the GDD, the HacknPlan and Miro boards, and the `.clip` notes as PNG. If you can still get them, download the 30-Jul to-do image and the 28-Jul test video from Discord. These were the richest sources the archive session couldn't read.
6. **Tell me if the reader panel opens blank** on claude.ai, and I'll take another approach.

## Commits from this session

| Commit | Date | What |
|---|---|---|
| `785d49ea` | 9 Oct | Docs 01, 02, DECISIONS, the tutorial and all code |
| `f1eed6d2` | 9 Oct | Attribution fix; paid assets and Photon removed |
| `ebf81a73` | 9 Oct | Quest board |
| `74426056` | 10 Oct | Guide built into the quest board as a reader |
| `642b084f` | 13 Oct | Doc 03: original gameplay and code review |
| `e8d5e379` | 15 Oct | Doc 04: local archive prompt |
| `062e3ffe` | 16 Oct | Doc 05: archive findings (by the local session, pushed by you) |
| `a1bcfc1a` | 16 Oct | Doc 06, camera fix, scale note, corrections |
| this commit | 17 Oct | Doc 07: this record |
