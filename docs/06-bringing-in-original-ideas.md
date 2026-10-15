# 6. Bringing the original ideas into the rebuild

Checked 16 October 2026 against the tutorial code (`docs/tutorial/code/`) and the archive findings in [`05-archive-findings.md`](05-archive-findings.md).

## Verdict

**Build the tutorial as written. Nothing in it blocks any of the original ideas.** Every original mechanic plugs into an existing extension point: a new `SabotageTarget` subclass, a new `IDumplingInteractable`, a new data asset, or a small change inside one class. Nothing needs a rewrite.

One real snag was found and fixed: `DumplingCamera` had a minimum camera distance of 0.5 m, which would have stopped you using the original 5 cm dumplings. It's now 0.05 m, and chapter 9 has a "tiny dumplings need tiny numbers" box with values for that scale.

Your decisions so far: **keep 5-minute rounds** (the original was 3) and **keep the service bell**.

## Where each original idea plugs in

Effort is a rough guide for after you've finished the tutorial: **S** ≈ an evening, **M** ≈ a day, **L** ≈ a few days.

| Original idea | Where it goes | What changes | Effort |
|---|---|---|---|
| **Fuse heist** (6 fuses → 6 hiding spots, chef retrieves them) | New `Fuse` (carryable), `HidingSpot : IDumplingInteractable`, `FuseBox` | A small **carry system** for dumplings (`DumplingCarrier`: pick up, hold at a socket, drop), since the tutorial has none. Hidden fuses get an XR Grab Interactable; an XR Socket on the fuse box takes them back | M |
| **Dumplings win by completing the mission; chef wins at time-out or when lives run out** | `RoundManager` | Add `ReportMissionComplete()` → `EndRound(Team.Dumplings)`; flip the time-out winner in `HandleTimerExpired`. Every win/lose rule lives in that one class, so this is a few lines | S |
| **Shared pool of lives, capture → respawn** | `GameConfig` (lives), `RoundManager` (count + `ReportDumplingCaught()`), a `DumplingCatcher` trigger (the pot, or the chef's grip) | Respawn reuses the spawn points from `LocalPlayerJoiner` | S–M |
| **Chef grabs and throws dumplings** | XR Grab Interactable on the Dumpling prefab | `DumplingMotor` sets the dumpling's velocity every physics step, so it would fight the chef's hand. Add a "held" flag that skips movement while grabbed (listen to the interactable's select entered/exited) | S |
| **Hat over one eye, fixed by a wiggle** | `ChefHat` (unchanged), `ChefVisionBlocker`, new `HeadWiggleDetector` | Anything can call `SabotageTarget.Restore()`, so a wiggle detector (head angular speed over a threshold) replaces the grab. **One eye only** needs a shader that draws for one eye: `unity_StereoEyeIndex` in a Shader Graph Custom Function node, because single-pass instanced renders both eyes at once | M |
| **Pot-based recipe: water → chop → into the pot** | `Cooking` module | A pot that tracks contents already exists (`CookingVessel`). Add a chopped state to `Ingredient`, a "has water" flag on the vessel (filled at the tap), and recipe steps on `RecipeDefinition`. `ServingCounter` becomes optional or is replaced by a "pot complete" check. Heat-seconds can stay or go | M |
| **Chopping** | New `ChoppingBoard` / knife | **Swap the whole ingredient for a pre-cut prefab** rather than slicing meshes at runtime (EzySlice). Runtime-sliced meshes can't be networked in Part 2, and swapping is simpler anyway | S–M |
| **Light switch + fuses both darken the kitchen** | `LightSwitch`, `FuseBox` | Today `LightSwitch` turns lights on and off directly, so a fuse heist would fight it: flicking the switch back on would light a kitchen with no fuses. Put one small `KitchenLighting` component in charge (lit = switch on AND power on) and have both sabotages talk to it | S |
| **Teleport pipes / drain and launch points** (the old "Activate points") | New `ActivatePoint : IDumplingInteractable` subclasses | Teleport = set the Rigidbody position. **Launchers** need the motor to stop steering briefly (it would otherwise brake the launch to walking speed within a fraction of a second): add `DumplingMotor.SuspendControl(seconds)` | S |
| **Double jump, dash, camera swap** | `DumplingMotor`, `DumplingCamera`, input asset, `IDumplingInput` | New actions → new `IDumplingInput` events (only `PlayerDumplingInput` implements it, so one place to update). For the camera swap, **keep one Camera** and move it between orbit and head positions: split-screen only resizes the camera that `PlayerInput` knows about | S each |
| **More sabotages** (toaster fire, can opener, fan, tap, salt in sugar, recipe book, sauces) | New `SabotageTarget` or `ElectricAppliance` subclasses | Exactly what the base classes are for. "Dirty food" / "salt in sugar" = a contaminated flag on `Ingredient` that the recipe check rejects | S each |
| **Mission pool: random, balanced, non-conflicting** (tutor's rule) | New `MissionDefinition` assets + `MissionDirector` | Listens to the existing static events (`SabotageTarget.AnySabotaged`, `ServingCounter.DishServed`...) and to new ones from the fuse heist. Each definition lists which missions it conflicts with | M–L |
| **"Chef Madness" meter** | `RoundManager` (a value) + `RoundDisplay` | Only if you decide what it means; the original never did | S |
| **Ready-up lobby** ("Ruckus in the Restaurant", Player 1–4 Ready) | `LocalPlayerJoiner` + a lobby canvas | `RoundManager.CanStartRound` already gates the bell; add "and every joined dumpling is ready" | S |
| **Results screen waits for Replay / Lobby / Quit** | `RoundManager.EnterPhase` | Today a zero-length phase moves straight on, so add a "wait for input" option for `RoundOver` and a `ReturnToWaiting()` the buttons call | S |
| **Diegetic chef UI** (task board, wall clock) | `RoundDisplay` on a world-space canvas | Already how the tutorial does it | — |
| **1.5 × 2 m kitchen, 25 cm counters, fuse panel behind the chef** | Scene layout | Level design only | — |
| **Ghost body / full chef body** | Chef rig | Independent of the game code. (The 2023 attempt used a paid IK asset; Unity's free Animation Rigging package can do a simple version) | M |

## The few things to watch

1. **Scale.** Decide early whether dumplings are the original 5 cm or something bigger, and tune once. Every distance in the motor, camera and interactor is an Inspector field. Also scale the server's reach tolerance: `DumplingInteractor.IsInReach` has a fixed 0.75 m allowance, so pass a smaller value at 5 cm.
2. **Grabbing dumplings in Part 2 (networked).** Dumplings are moved by their own player (owner authority), while the chef (the host) would be holding them. The fix is standard but needs care: when the chef grabs a dumpling, the host takes ownership with `NetworkObject.ChangeOwnership`, and gives it back on release. This only matters if you do Part 2. The original game was local-only, and Part 1 has no such issue.
3. **Chopping and networking.** As above, use prefab swaps, not runtime mesh slicing, if you might go networked.
4. **Split-screen audio.** It's the same as the original: only the chef's AudioListener plays. That's fine for a party game on one PC. Per-dumpling positional audio needs a multi-listener asset or custom mixing.

## Suggested order after the tutorial

1. Scale pass (5 cm dumplings, physics contact offset).
2. Shared lives + capture + new win rules. This turns it into the original game's loop.
3. Carry system + fuse heist + `KitchenLighting`.
4. Pot recipe with chopping and water.
5. Activate points (teleports, launchers) and double jump.
6. Hat-over-one-eye with wiggle.
7. Ready-up lobby and results buttons.
8. Mission pool.
