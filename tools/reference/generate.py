"""Regenerates the music21 reference tables that MusicTheory.UnitTests/ReferenceTests.cs checks against.

    python3 -m venv .venv && .venv/bin/pip install -r tools/reference/requirements.txt
    .venv/bin/python tools/reference/generate.py

music21 (MIT Lab, BSD) is the reference implementation of spelled music theory. The tables are
committed so the tests need neither Python nor network. Spellings use MusicTheory's syntax:
b/bb for flats, #/## for sharps. Rows music21 cannot express (beyond quadruple
augmented/diminished, or that music21 wraps inconsistently around the octave) are skipped here and
covered by MusicTheory's own tests.
"""
import csv
import itertools
import pathlib

import music21
from music21 import interval, key, note, scale

OUT = pathlib.Path(__file__).resolve().parents[2] / "MusicTheory.UnitTests" / "Reference"
LETTERS = "CDEFGAB"
ALTERS = {-2: "--", -1: "-", 0: "", 1: "#", 2: "##"}
OURS = {-2: "bb", -1: "b", 0: "", 1: "#", 2: "##"}
SPELLINGS = [(l, a) for l in LETTERS for a in range(-2, 3)]


def m21(letter, alter, octave=None):
    return f"{letter}{ALTERS[alter]}{'' if octave is None else octave}"


def ours(p, with_octave=True):
    alter = int(p.alter)
    if abs(alter) > 2:
        return None
    return f"{p.step}{OURS[alter]}{p.octave if with_octave else ''}"


def write(name, header, rows):
    with open(OUT / name, "w", newline="") as f:
        w = csv.writer(f, lineterminator="\n")
        w.writerow(header)
        w.writerows(rows)
    print(f"{name}: {len(rows)} rows")


def intervals():
    rows = []
    for (l1, a1), (l2, a2) in itertools.product(SPELLINGS, SPELLINGS):
        for o2 in (3, 4, 5):
            start, end = note.Note(m21(l1, a1, 4)), note.Note(m21(l2, a2, o2))
            try:
                i = interval.Interval(start, end)
                name = i.directedName
                # Past quadruple, music21 wraps the quality around the octave (B##4 -> Fbb5 comes out
                # as AAAA5 of 2 semitones). Keep only rows music21 itself reads back consistently.
                if interval.Interval(name).semitones != i.semitones:
                    continue
            except interval.IntervalException:
                continue
            rows.append([f"{l1}{OURS[a1]}4", f"{l2}{OURS[a2]}{o2}", name, i.semitones])
    write("intervals.csv", ["from", "to", "interval", "semitones"], rows)


INTERVALS = ["P1", "A1", "d2", "m2", "M2", "A2", "d3", "m3", "M3", "A3", "d4", "P4", "A4",
             "d5", "P5", "A5", "d6", "m6", "M6", "A6", "d7", "m7", "M7", "A7", "d8", "P8",
             "m9", "M9", "A11", "P12", "m13", "M13", "P15"]


def transpositions():
    rows = []
    for (l, a), name in itertools.product(SPELLINGS, INTERVALS):
        for directed in (name, name[:-len(name.lstrip("PMmdA"))] + "-" + name.lstrip("PMmdA")):
            if directed.endswith("-1"):
                continue
            result = note.Note(m21(l, a, 4)).transpose(directed)
            rows.append([f"{l}{OURS[a]}4", directed, ours(result.pitch) or "!"])
    write("transpositions.csv", ["note", "interval", "result"], rows)


SCALES = {
    "Major": scale.MajorScale, "NaturalMinor": scale.MinorScale, "HarmonicMinor": scale.HarmonicMinorScale,
    "MelodicMinor": scale.MelodicMinorScale, "Dorian": scale.DorianScale, "Phrygian": scale.PhrygianScale,
    "Lydian": scale.LydianScale, "Mixolydian": scale.MixolydianScale, "Locrian": scale.LocrianScale,
}


def scales():
    rows = []
    for (l, a), (name, cls) in itertools.product(SPELLINGS, SCALES.items()):
        pitches = cls(m21(l, a, 4)).getPitches(m21(l, a, 4), m21(l, a, 5))
        spelled = [ours(p) for p in pitches]
        rows.append([f"{l}{OURS[a]}4", name, " ".join(s or "!" for s in spelled)])
    write("scales.csv", ["tonic", "type", "notes"], rows)


MODES = [
    (scale.MelodicMinorScale, 2, "DorianFlat2"), (scale.MelodicMinorScale, 3, "LydianAugmented"),
    (scale.MelodicMinorScale, 4, "LydianDominant"), (scale.MelodicMinorScale, 5, "MixolydianFlat6"),
    (scale.MelodicMinorScale, 6, "LocrianNatural2"), (scale.MelodicMinorScale, 7, "Altered"),
    (scale.HarmonicMinorScale, 2, "LocrianNatural6"), (scale.HarmonicMinorScale, 3, "IonianAugmented"),
    (scale.HarmonicMinorScale, 4, "DorianSharp4"), (scale.HarmonicMinorScale, 5, "PhrygianDominant"),
    (scale.HarmonicMinorScale, 6, "LydianSharp2"), (scale.HarmonicMinorScale, 7, "Ultralocrian"),
]


def modes():
    rows = []
    for (l, a), (cls, k, name) in itertools.product(SPELLINGS, MODES):
        # The parent's degree k lands on our tonic: build the parent on the tonic moved down by the
        # parent's own degree-k interval, then read it ascending from the tonic to the octave above.
        ref = cls(note.Note("C4")).getPitches("C4", "C5")
        up = interval.Interval(ref[0], ref[k - 1])
        parent = cls(note.Note(m21(l, a, 4)).transpose(up.reverse()))
        spelled = [ours(p) for p in parent.getPitches(m21(l, a, 4), m21(l, a, 5))]
        rows.append([f"{l}{OURS[a]}4", name, " ".join(s or "!" for s in spelled)])
    write("modes.csv", ["tonic", "type", "notes"], rows)


def keys():
    rows = []
    for (l, a), mode in itertools.product(SPELLINGS, ("major", "minor")):
        k = key.Key(m21(l, a) if mode == "major" else m21(l, a).lower(), mode)
        spelled = [ours(p, False) for p in k.alteredPitches]
        altered = "!" if None in spelled else " ".join(sorted(spelled))
        rows.append([f"{l}{OURS[a]}", mode, k.sharps, altered])
    write("keys.csv", ["tonic", "mode", "sharps", "altered"], rows)


def neoriemannian():
    from music21 import analysis, chord

    rows = []
    for (l, a), quality in itertools.product(SPELLINGS, ("major", "minor")):
        third = interval.Interval("M3" if quality == "major" else "m3")
        root = note.Note(m21(l, a, 4))
        c = chord.Chord([root, root.transpose(third), root.transpose("P5")])
        for move in "PLR":
            r = getattr(analysis.neoRiemannian, move)(c)
            res = ours(r.root(), False)
            if res is None or any(ours(p, False) is None for p in r.pitches):
                continue
            rows.append([f"{l}{OURS[a]}", quality, move, res, r.quality])
    write("neoriemannian.csv", ["root", "type", "move", "result_root", "result_type"], rows)


if __name__ == "__main__":
    print("music21", music21.__version__)
    OUT.mkdir(parents=True, exist_ok=True)
    intervals()
    transpositions()
    scales()
    modes()
    keys()
    neoriemannian()
