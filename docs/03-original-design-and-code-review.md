# 3. What the original game was meant to be, and how well it was built

Dug out of Big-Project and NewVRDumpling on 13 October 2026. There's no design document in either repo; everything below comes from code, prefabs, scene contents and commit messages. Your local files may fill the gaps (see the end).

## Gameplay found in the projects

### The game's name

The menu scene in NewVRDumpling has a title: **"Dumpling Madness VR"**, with a PC main menu, a PC settings menu, a lobby with *Ready Status*, and four name slots (*Dumpling 1 Name* ... *Dumpling 4 Name*).

### Dumpling missions (Kieran's task system, NewVRDumpling)

Two quest assets exist in `Dumplings/ModularCharacterController/Samples/Task Prefabs/`:

| Quest asset | Title | Description | Reward fields |
|---|---|---|---|
| `GatherQuest.asset` | **Gather Fuses** | "Find 4 fuses" | 5 XP, 5 gold |
| `HideQuest.asset` | **Hide 4 Fuses** | "Travel to the hiding spots" | 10 XP, 10 gold |

How it plays, from `PickUp.cs`, `GatherQuestGoal.cs` and `HideQuestGoal.cs`:

1. **Fuses** (`Temp Fuse` prefab, an `InWorldItem`) are scattered around. A dumpling walks into one and carries it (it's parented to the dumpling).
2. The dumpling takes each fuse to one of **4 hiding spots** (`Hiding Spots` prefab). Dropping it there completes part of the Hide goal ("You have hidden all the fuses").
3. A hidden fuse is moved to the **`ChefInteractable`** layer, so **the chef can find and take it back**.
4. A **waypoint marker** (`WaypointMarker`, `ObjectiveMarkerUI`) shows the distance to the objective on the dumpling's screen.
5. A **Dumpling Task UI** shows quest name, description, completion, EXP and gold.

So the original dumpling objective was a **fuse heist**, almost certainly to kill the kitchen's power. The 2026 tutorial's `PowerPlug`/`LightSwitch` sabotages are the instant version of this; the fuse heist is a longer multi-step objective worth bringing back (see "Bringing it back" below).

### Other dumpling ideas

- **Light switch task**: `LightSwitch.cs` turns kitchen point lights and an emissive material on and off. A `Test Lamp` prefab and a `LightCord` material suggest a pull-cord lamp.
- **Throwing / launching dumplings**: Kieran's `ThrowObject` adds a force to anything tagged `Player` that collides with it (a launch pad or trap), plus a "ThrowPlayer" sample scene. A 2022-07-21 commit says "Added Throwing".
- **Combat**: a `CombatBehaviour` stub (`OnShoot` → "Bang"). Only a placeholder.
- **Dumpling selection**: Lara's `DumplingSelection` / `LoadDumpling` (pick one of several dumpling models before the game). There are 4 dumpling materials (`Dumpling1`–`Dumpling4`).
- **Movement**: third-person and first-person motors with camera switching (`MultiMotor`, `CameraSwitcher`), and a "DumplingPlayground" test scene.

### Chef ideas

- **Cutting**: Lara's `Cut.cs` (2022-07-19, "can cut cucumber"). A cucumber made of frozen slices falls apart when hit by the butcher's knife.
- **Wrist menu**: `VrWristUI` (Big-Project) and a `WristUI Canvas` prefab (NewVRDumpling), with exit and return-to-menu buttons.
- **Interactable kitchen** (`Kitchen_Populated` prefab): `Light_Switch`, `Main Door`, `Fridge` with `Freezer_Door`, `Dairy_Door_L/R` and shelves, `Sink_Door_L/R`, `Flip-Up_Counter`, `Service_Counter`, `Basin`, gas canister, wok, stockpot, bamboo steamer with a separate `Steamer_Hat` (lid), chopping board, knives, mortar and pestle, rolling pin.
- **Ingredients placed in the kitchen**: carrots, tomatoes, shallots, onions, cucumbers, eggs, plus cans, bottles and crockery. The steamer and rolling pin hint that the chef's dish was **dumplings** themselves.

### Not found anywhere

No recipe list, win condition, round length, scoring rules or sabotage list beyond the above. The game design document and the Discord chat are the likely places for those.

## Was the old code built well?

Short version: typical of a capable student team, with one genuinely well-architected system (Kieran's character controller) and gameplay code that was still at the prototype stage.

| Area | Who | Good | Problems |
|---|---|---|---|
| **Mirror lobby** (Big-Project) | Lara | Namespaces, `[SerializeField] private`, static events with matching subscribe/unsubscribe in `OnEnable`/`OnDisable`, XML comments, `[Server]`/`[Client]` attributes used correctly | Scene names as strings (a rename broke the lobby); one NetworkManager class doing connection, lobby, readiness, scene changes and spawning; `Resources.LoadAll` for prefabs; client-authoritative movement; input lambdas never unsubscribed; two separate `Controls` instances for one player |
| **Modular Character Controller** (NewVRDumpling) | Kieran (& James) | The best code in either repo: an abstract `ModularBehaviour` base with update phases, an `IMCCPlayer` interface, behaviours registered on a player, ScriptableObject settings assets, thorough comments, a changelog | Photon and Odin are baked into the core class; `TryGetBehaviour` matches the exact type only (subclasses aren't found); a public `toBeUsedInMMCParent` field |
| **Quest / task system** | Kieran & Lara | Right idea: a goal base class with Gather and Hide subclasses, quest data in ScriptableObjects | The quest asset's goal list points at the goal components on the *prefab*, not the copies in the scene, so the quest UI reads the prefab's numbers and never sees the live progress (and in the editor, changing those prefab values at runtime saves them into the asset); `ItemCollected` is re-declared in the subclass instead of overridden, so which version runs depends on the variable's type; `FindObjectOfType` and `GameObject.Find("Dumpling")` by name; four hiding spots hard-coded as fields and name strings; RPG leftovers (XP/gold); `TaskUIManager` empty |
| **LightSwitch** | Kieran | Small and readable | Shader property typo `"_EmissionColour"` (Unity's is `_EmissionColor`), so the bulb glow never changed; `GetComponent` on every toggle; lights found by name string |
| **Cut.cs** | Lara | Neat trick (slices frozen until hit) | Compares the knife's GameObject name to `"Butcher Knife"`, but the prop is named `Prop_Butcher'sKnife`, so it only works if someone renamed it in the scene |
| **VrWristUI** | Lara | | Adds button listeners in `Update`, i.e. every frame |
| **Project hygiene** | everyone | Art pipeline, an `ARTISTS` folder, a ToDo-list editor tool, consistent prefab naming | Code grouped by person, not feature; no assembly definitions or tests; big third-party/demo packages committed |

The 2026 tutorial's code fixes each of these patterns. The `Then → now` boxes in the guide point at several of them.

## Bringing it back: the fuse heist

The fuse heist fits the new architecture well and is the most "original" mission found. A sketch to build after Part 1 (not yet in the guide):

- `FuseBox : ElectricAppliance`. While it has fewer than 4 fuses, the kitchen loses power (lights, hob, rice cooker).
- `Fuse`: a carryable item. Dumplings pick it up from the fuse box and drop it at a `HidingSpot`.
- `HidingSpot : IDumplingInteractable`. It accepts a fuse and marks it findable by the chef (the old `ChefInteractable` layer idea).
- The chef finds the hidden fuse (XR Grab Interactable) and slots it back into the fuse box (XR Socket Interactor).
- Optional: a waypoint marker on the dumpling's screen pointing at the nearest hiding spot, like Kieran's `WaypointMarker`.

## Giving me access to more material

See the reply in the session for options. The most valuable things to share, in order:

1. The repo with your **4-player local co-op** VR/PC build.
2. Any **game design document**, pitch or brief.
3. A **Discord export** of the project channel (DiscordChatExporter can save a channel as HTML, JSON or text).
4. Task boards (Trello, Notion, HacknPlan), playtest notes, feedback from tutors.
