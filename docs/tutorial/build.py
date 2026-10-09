#!/usr/bin/env python3
"""Builds the tutorial HTML from src/*.html, inlining the real script files.

Markers in the src files look like:
    <!--@code part1 Scripts/Core/RoundTimer.cs-->
    <!--@code part2 Scripts/DumplingKitchen.Runtime.asmdef json-->

part1 -> code/part1-local/Assets/_Project/
part2 -> code/part2-networked/Assets/_Project/

Outputs:
    dumpling-kitchen-tutorial.html      a complete page you can open from disk
    <fragment path>                      (optional, 1st argument) the same page body without
                                         <html>/<head>/<body>, for publishing as an artifact
"""
import html
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent
SRC = ROOT / "src"
CODE_ROOTS = {
    "part1": ROOT / "code" / "part1-local" / "Assets" / "_Project",
    "part2": ROOT / "code" / "part2-networked" / "Assets" / "_Project",
}
MARKER = re.compile(r"<!--@code (part1|part2) (\S+?)(?: (\w+))?-->")
MERMAID = (
    '<script src="https://cdn.jsdelivr.net/npm/mermaid@10.9.1/dist/mermaid.min.js"></script>\n'
    "<script>mermaid.initialize({ startOnLoad: true, theme: "
    "window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'neutral' });</script>\n"
)


def render_code(match: re.Match) -> str:
    part, rel, lang = match.group(1), match.group(2), match.group(3) or "csharp"
    path = CODE_ROOTS[part] / rel
    if not path.is_file():
        raise SystemExit(f"Missing code file for marker: {path}")
    text = path.read_text(encoding="utf-8").rstrip("\n")
    folder, _, name = rel.rpartition("/")
    badge = " · Part 2 version" if part == "part2" else ""
    short = " short" if text.count("\n") < 30 else ""
    return (
        f'<figure class="code{short}">'
        f'<figcaption><span class="file"><span class="dir">{html.escape(folder)}/</span>{html.escape(name)}'
        f'<span class="dir">{badge}</span></span>'
        f'<button type="button" class="copy">Copy</button></figcaption>'
        f'<pre><code class="language-{lang}">{html.escape(text)}</code></pre></figure>'
    )


def main() -> None:
    parts = sorted(SRC.glob("*.html"))
    head = parts[0].read_text(encoding="utf-8")
    body = "\n".join(p.read_text(encoding="utf-8") for p in parts[1:])
    body, count = MARKER.subn(render_code, body)

    full = (
        "<!doctype html>\n<html lang=\"en\">\n<head>\n<meta charset=\"utf-8\">\n"
        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, viewport-fit=cover\">\n"
        f"{head}</head>\n<body>\n{body}\n{MERMAID}</body>\n</html>\n"
    )
    out = ROOT / "dumpling-kitchen-tutorial.html"
    out.write_text(full, encoding="utf-8")
    print(f"Wrote {out} ({len(full):,} bytes, {count} code blocks)")

    if len(sys.argv) > 1:
        fragment = Path(sys.argv[1])
        fragment.write_text(f"{head}\n{body}\n", encoding="utf-8")
        print(f"Wrote {fragment}")


if __name__ == "__main__":
    main()
