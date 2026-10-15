# Dumpling Kitchen notes

Notes on the 2022 VR dumpling project and a plan for rebuilding it in current Unity. Written October 2026.

## The short answer

- **Which repo had asymmetric VR/PC working?** *Update: it's this repo's `DevelopXR` branch (best snapshot `92aed0f3`), which the notes below missed because they only looked at `Master`. See [`05-archive-findings.md`](05-archive-findings.md).* Original answer: neither of these two has your version. Your 4-player local co-op VR/PC build (made without a template) must be in another repo. **Big-Project** (this repo) has structured lobby code (Mirror) but no VR player, and its lobby is currently broken by a scene rename. **NewVRDumpling** has Lara's later networked experiments, which aren't carried forward.
- **Upgrade to the latest Unity, or start again?** **Start again** in Unity 6.3 LTS and bring the art across. The VR SDKs (SteamVR plugin, Oculus Integration) and networking library (Mirror 32) the old code is built on have been replaced by OpenXR and Netcode for GameObjects. Upgrading means deleting those parts and rebuilding them anyway.
- **The tutorial**: [`tutorial/dumpling-kitchen-tutorial.html`](tutorial/dumpling-kitchen-tutorial.html). Open it in a browser. It builds the chef-vs-dumplings game from an empty project: Part 1 as a local couch game (Quest 2 + split-screen on one PC), Part 2 networked with Netcode for GameObjects.

## Files

| File | What it is |
|---|---|
| [`01-repo-assessment.md`](01-repo-assessment.md) | What's in Big-Project and NewVRDumpling, what worked, what's broken, what to salvage |
| [`02-upgrade-or-rebuild.md`](02-upgrade-or-rebuild.md) | Why rebuilding beats upgrading, current versions, how to bring the old art across |
| [`03-original-design-and-code-review.md`](03-original-design-and-code-review.md) | The original gameplay found in the old projects (fuse heist, kitchen interactables, chef ideas) and a review of how well the old code was built |
| [`04-local-archive-prompt.md`](04-local-archive-prompt.md) | A ready-to-paste prompt for a Claude Code session on your own computer, to mine your local repos, documents and chat exports for the original design |
| [`05-archive-findings.md`](05-archive-findings.md) | Everything found about the original design in the local archive (repos, Discord exports, notes): rules, chef and dumpling gameplay, the working local co-op setup on `DevelopXR`, production history, and which rebuild decisions to change |
| [`06-bringing-in-original-ideas.md`](06-bringing-in-original-ideas.md) | Check that the tutorial's design can take the original ideas (it can), where each one plugs in, and a suggested order |
| [`DECISIONS-TO-REVIEW.md`](DECISIONS-TO-REVIEW.md) | Every call I made without asking you (D1–D29), with alternatives |
| [`tutorial/dumpling-kitchen-tutorial.html`](tutorial/dumpling-kitchen-tutorial.html) | The tutorial (18 chapters + appendices) |
| [`tutorial/dumpling-kitchen-quest.html`](tutorial/dumpling-kitchen-quest.html) | The same guide as a game: 9 services of 5 hours, XP and chef ranks, boss fights, focus sprints, achievements, deadline countdown |
| [`tutorial/code/part1-local/`](tutorial/code/part1-local/) | Every Part 1 script, laid out as `Assets/_Project/...` so you can copy the folder into a new project |
| [`tutorial/code/part2-networked/`](tutorial/code/part2-networked/) | Part 2 scripts: new files plus the replacements for files that change |
| [`tutorial/src/`](tutorial/src/), [`tutorial/src-quest/`](tutorial/src-quest/) + [`tutorial/build.py`](tutorial/build.py) | Source for both pages. The build inlines the real `.cs` files into the guide so the page and the code never drift apart |

## Editing the tutorial

Edit the chapter files in `tutorial/src/` or the scripts in `tutorial/code/`, then rebuild:

```sh
python3 docs/tutorial/build.py
```

Code blocks are written as `<!--@code part1 Scripts/Core/RoundTimer.cs-->` markers; the build replaces them with the file contents.
