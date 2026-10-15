# Prompt for a local Claude Code session

Paste everything between the lines into a Claude Code session (desktop app or CLI) opened on the parent folder that holds your old dumpling repos, documents and chat exports. If you also cloned Big-Project into that folder (branch `claude/vibrant-cerf-xk6ce6`), the findings land straight in its `docs/` folder.

---

I'm rebuilding an asymmetric VR/PC party game that my student team made in Unity 2021.3 in 2022. I want the new version to match the original concept and gameplay as closely as possible. This folder holds my old material: Unity project repos, design documents, notes, and possibly Discord chat exports, task boards, images and slides. Please go through it and pull out everything about the game's design, then write it up for me.

**Context you need.** The game (working titles "Dumpling Madness VR" / "Dumpling Kitchen"): one player is a chef in VR trying to cook a meal; 1–4 PC players are dumplings running around the kitchen sabotaging the chef (unplugging things, knocking things over, switching off lights, turning off the gas, knocking the chef's hat over their eyes). Two old repos have already been analysed and are NOT the priority: `Big-Project` (Mirror lobby, no VR player) and `NewVRDumpling` (Lara's Photon/template experiments, Kieran's modular character controller, a "Gather Fuses / Hide 4 Fuses" dumpling quest, a cucumber-cutting test). The summary of that analysis is in `Big-Project/docs/03-original-design-and-code-review.md` if that repo is in this folder; read it first so you don't repeat it. I personally got **asymmetric 4-player local co-op (one headset + PC players on one machine) working without a paid template**, in a repo that hasn't been analysed yet. Finding that repo and how it worked is a priority.

**Rules.**
- Read only. Don't modify, move, delete or commit anything in the old repos or documents, and don't push anything anywhere.
- Ignore third-party and generated folders: `Library/`, `Temp/`, `Logs/`, `obj/`, `Packages/` caches, and vendored SDKs such as `Assets/SteamVR`, `Assets/Oculus`, `Assets/Mirror`, `Assets/Photon`, `Assets/BNG Framework`, `Assets/Plugins`, `TextMesh Pro`, sample/demo scenes. Our own code is usually under folders named after the project, team members, `Scripts`, `Prototypes` or `_Project`.
- Unity scenes, prefabs and `.asset` files are YAML: look for `m_Name:` (object names), `m_text:` (UI text) and ScriptableObject fields to find gameplay content. Look at git history too (`git log`, commit messages, branches, dates, authors). Repos may use Git LFS; text files are enough.
- I don't use paid assets or Photon any more, so don't spend time on how those worked. Note only the gameplay ideas built with them.
- Don't copy personal details (emails, phone numbers, addresses), passwords, API keys or tokens into your write-up. If you find a secret, say which file it's in, not the value.
- For every finding, cite where it came from (file path, and line or commit hash, or message date and author for chats). Mark each as **confirmed** (implemented or clearly agreed), **proposed** (an idea discussed), or **dropped** (explicitly rejected or replaced), and say who suggested it when you can tell.
- When sources disagree, prefer the latest dated one and list the conflict.

**What I want found.**
1. **Core concept**: pitch, title(s), genre, target platforms and headsets, player counts, session length, tone and art style.
2. **Win/lose conditions and rules**: round length, scoring, what ends a round, how the chef and the dumplings each win.
3. **Chef gameplay**: recipes/dishes and their steps, ingredients, cooking stations, tools, interactions (cutting, frying, steaming...), UI (wrist menu, order tickets), and how the chef fights back against dumplings (grabbing, throwing, swatting?).
4. **Dumpling gameplay**: every sabotage, mission, objective or task (e.g. the fuse heist), movement abilities (jump, climb, vents, hiding), abilities or power-ups, character selection, how dumplings are caught or punished, respawning.
5. **The local co-op setup that worked**: which repo, Unity version, VR SDK, how the headset view and the PC players' views were split (multi-display? split screen?), input handling for multiple PC players, and anything notable about the scene setup. Point to the key scripts and scenes.
6. **Level and kitchen layout**: rooms, interactive objects (doors, fridge, lights, gas, plugs, fuse box), hiding places, and the intended flow.
7. **Menus and flow**: main menu, lobby/ready-up, character select, results screen.
8. **Audio and art direction** notes, if any.
9. **Production history**: timeline of what was built when and by whom, what was working when the project stopped, known bugs, and the to-do list or backlog at the end.
10. **Anything else** that looks important for recreating the game as it was.

**Output.** Write one Markdown report:
- If `Big-Project/docs/` exists in this folder, save it as `Big-Project/docs/05-archive-findings.md`. Otherwise save it as `dumpling-archive-findings.md` in this folder.
- Start with a one-page summary: the concept in a paragraph, the win conditions, the top 10 most important gameplay facts, and where the working local co-op build lives.
- Then one section per topic above, using tables where it helps, each finding with its status (confirmed / proposed / dropped), who, and the source.
- End with **"Differences from the rebuild"**: compare the findings with the rebuild's current assumptions and say which of these to change. The current assumptions are: 5-minute rounds; the chef wins instantly on serving 3 dishes, otherwise the dumplings win when time runs out; the chef starts the round by ringing a service bell; sabotages are gas valve, light switch, power plug, knock-over and chef's hat, each undone by the chef grabbing or pressing it, with a 5-second cooldown; cooking is heat-seconds in a pot on a stove; serving is putting cooked ingredients on a counter; up to 4 dumplings in split-screen on one PC, then networked with the chef as host. If `Big-Project/docs/DECISIONS-TO-REVIEW.md` is available, refer to its decision numbers (D7–D29).
- Finally, list the files you couldn't read (binary formats, corrupted files, huge exports), so I can convert them.

Work through the folder methodically. List what's there first and tell me your plan before the deep read. Stop and ask me if the folder is much larger than expected or you find material that looks private or unrelated to the game.

---
