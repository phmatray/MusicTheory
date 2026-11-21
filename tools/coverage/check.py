"""Fails unless the packaged libraries are covered at 100% of lines and branches.

    dotnet test --project MusicTheory.UnitTests --coverage --coverage-output-format cobertura --coverage-output $PWD/TestResults/core.xml
    dotnet test --project MusicTheory.Guitar.UnitTests --coverage --coverage-output-format cobertura --coverage-output $PWD/TestResults/guitar.xml
    dotnet test --project MusicTheory.ChordPro.UnitTests --coverage --coverage-output-format cobertura --coverage-output $PWD/TestResults/chordpro.xml
    python3 tools/coverage/check.py TestResults/*.xml

A line counts as covered when any report covers it; a branch takes the best count seen. Every uncovered
line and partial branch is printed, so the failure says what to test. Test projects and the demo app
are not measured.
"""
import collections
import re
import sys
import xml.etree.ElementTree as ET

LIBRARIES = {"MusicTheory", "MusicTheory.Guitar", "MusicTheory.Guitar.Blazor", "MusicTheory.ChordPro"}


def main(paths):
    lines = collections.defaultdict(dict)  # (package, file) -> line -> [hits, covered, total]
    for path in paths:
        for package in ET.parse(path).getroot().iter("package"):
            name = package.get("name")
            if name not in LIBRARIES:
                continue
            for cls in package.iter("class"):
                file = cls.get("filename")
                for line in cls.find("lines").iter("line"):
                    number, hits = int(line.get("number")), int(line.get("hits"))
                    m = re.search(r"\((\d+)/(\d+)\)", line.get("condition-coverage") or "")
                    covered, total = (int(m.group(1)), int(m.group(2))) if m else (0, 0)
                    entry = lines[(name, file)].setdefault(number, [0, 0, total])
                    entry[0] = max(entry[0], hits)
                    entry[1] = max(entry[1], covered)
                    entry[2] = max(entry[2], total)

    totals = collections.Counter()
    gaps = []
    for (package, file), entries in sorted(lines.items()):
        for number, (hits, covered, total) in sorted(entries.items()):
            totals[package, "lines"] += 1
            totals[package, "covered lines"] += hits > 0
            totals[package, "branches"] += total
            totals[package, "covered branches"] += covered
            if hits == 0:
                gaps.append(f"{file}:{number} never runs")
            elif covered < total:
                gaps.append(f"{file}:{number} takes {covered} of {total} branches")

    missing = LIBRARIES - {package for package, _ in totals}
    for package in sorted({p for p, _ in totals}):
        print(f"{package}: lines {totals[package, 'covered lines']}/{totals[package, 'lines']}, "
              f"branches {totals[package, 'covered branches']}/{totals[package, 'branches']}")
    for gap in gaps:
        print(f"  {gap}")
    if missing:
        print(f"no coverage data for {', '.join(sorted(missing))}")
    return 1 if gaps or missing else 0


if __name__ == "__main__":
    if len(sys.argv) < 2:
        sys.exit(__doc__)
    sys.exit(main(sys.argv[1:]))
