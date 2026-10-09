# Dumpling Kitchen notes

Notes on the 2022 VR dumpling project and a plan for rebuilding it in current Unity. Written October 2026.

## The short answer

- **Which repo had asymmetric VR/PC working?** **NewVRDumpling** (July 2022), using a paid Oculus asymmetric template, Photon PUN 2 and the Oculus Integration. **Big-Project** (this repo) had the better structured lobby code (Mirror) but never had a VR player, and its lobby is currently broken by a scene rename.
- **Upgrade to the latest Unity, or start again?** **Start again** in Unity 6.3 LTS and bring the art across. The VR SDKs (SteamVR plugin, Oculus Integration) and networking libraries (Mirror 32, PUN 2) both repos are built on have been replaced by OpenXR and Netcode for GameObjects. Upgrading means deleting those parts and rebuilding them anyway.
- **The tutorial**: [`tutorial/dumpling-kitchen-tutorial.html`](tutorial/dumpling-kitchen-tutorial.html). Open it in a browser. It builds the chef-vs-dumplings game from an empty project: Part 1 as a local couch game (Quest 2 + split-screen on one PC), Part 2 networked with Netcode for GameObjects.

## Files

| File | What it is |
|---|---|
| [`01-repo-assessment.md`](01-repo-assessment.md) | What's in Big-Project and NewVRDumpling, what worked, what's broken, what to salvage |
| [`02-upgrade-or-rebuild.md`](02-upgrade-or-rebuild.md) | Why rebuilding beats upgrading, current versions, how to bring the old art across |
| [`DECISIONS-TO-REVIEW.md`](DECISIONS-TO-REVIEW.md) | Every call I made without asking you (D1–D29), with alternatives |
| [`tutorial/dumpling-kitchen-tutorial.html`](tutorial/dumpling-kitchen-tutorial.html) | The tutorial (18 chapters + appendices) |
| [`tutorial/code/part1-local/`](tutorial/code/part1-local/) | Every Part 1 script, laid out as `Assets/_Project/...` so you can copy the folder into a new project |
| [`tutorial/code/part2-networked/`](tutorial/code/part2-networked/) | Part 2 scripts: new files plus the replacements for files that change |
| [`tutorial/src/`](tutorial/src/) + [`tutorial/build.py`](tutorial/build.py) | Tutorial source. The build inlines the real `.cs` files so the page and the code never drift apart |

## Editing the tutorial

Edit the chapter files in `tutorial/src/` or the scripts in `tutorial/code/`, then rebuild:

```sh
python3 docs/tutorial/build.py
```

Code blocks are written as `<!--@code part1 Scripts/Core/RoundTimer.cs-->` markers; the build replaces them with the file contents.
