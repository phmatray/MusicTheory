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
import sys

import music21
from fractions import Fraction

from music21 import chord, converter, duration, interval, key, meter, note, scale, stream

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
    write_to(OUT / name, header, rows)


def write_to(path, header, rows):
    name = path.name
    with open(path, "w", newline="") as f:
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


DURATION_TYPES = [("Whole", "whole"), ("Half", "half"), ("Quarter", "quarter"), ("Eighth", "eighth"),
                  ("Sixteenth", "16th"), ("ThirtySecond", "32nd"), ("SixtyFourth", "64th")]


def durations():
    """Undotted-to-triple-dotted values and 3:2, 5:4, 7:4 tuplets, as fractions of a whole note."""
    rows = []
    for ours_name, m21_name in DURATION_TYPES:
        for dots in range(4):
            d = duration.Duration(type=m21_name, dots=dots)
            rows.append([ours_name, dots, "", "", Fraction(d.quarterLength) / 4])
        for actual, normal in ((3, 2), (5, 4), (7, 4)):
            d = duration.Duration(type=m21_name)
            d.appendTuplet(duration.Tuplet(actual, normal))
            rows.append([ours_name, 0, actual, normal, Fraction(d.quarterLength) / 4])
    write("durations.csv", ["type", "dots", "actual", "normal", "whole_notes"], rows)


def forte_name(c):
    """music21's forteClassTnI drops the Z; hasZRelation restores it ('4-15' -> '4-Z15')."""
    card, number = c.forteClassTnI.split("-")
    return f"{card}-{'Z' if c.hasZRelation else ''}{number}"


def pcsets():
    rows, catalog = [], {"0-1": 0}  # the empty set is not a music21 chord; Forte names it 0-1
    for mask in range(1, 4096):
        c = chord.Chord([pc for pc in range(12) if mask >> pc & 1])
        join = lambda xs: " ".join(str(x) for x in xs)
        name = forte_name(c)
        catalog[name] = sum(1 << pc for pc in c.primeForm)
        rows.append([mask, join(c.normalOrder), join(c.primeForm), join(c.intervalVector), name])
    write("pcsets.csv", ["pcs", "normal", "prime", "icv", "forte"], rows)
    return catalog


def forte_catalog(catalog):
    """Writes MusicTheory/ForteCatalog.cs: the TnI set classes as (name, prime-form mask)."""
    key = lambda n: tuple(int(x) for x in n.replace("Z", "").split("-"))
    entries = "\n".join(f'        ("{n}", 0x{catalog[n]:03X}),' for n in sorted(catalog, key=key))
    path = OUT.parents[1] / "MusicTheory" / "ForteCatalog.cs"
    path.write_text(f"""// <auto-generated>
// Generated by: .venv/bin/python tools/reference/generate.py --csharp-forte (music21 {music21.__version__}).
// Do not edit by hand: rerun the generator.
// </auto-generated>
namespace MusicTheory;

/// <summary>Forte's table of the {len(catalog)} TnI set classes, keyed by the mask of their prime form.</summary>
internal static class ForteCatalog
{{
    private static readonly Dictionary<ushort, string> Names = Rows().ToDictionary(r => r.PrimeMask, r => r.Name);
    private static readonly Dictionary<string, ushort> Masks = Rows().ToDictionary(r => r.Name, r => r.PrimeMask);

    /// <summary>The Forte name of the set class whose prime form has this mask, or null.</summary>
    internal static string? NameOf(ushort primeMask) => Names.GetValueOrDefault(primeMask);

    /// <summary>The prime-form mask of the named set class, or null.</summary>
    internal static ushort? PrimeMaskOf(string name) => Masks.TryGetValue(name, out var mask) ? mask : null;

    /// <summary>Every set class, ordered by cardinality then Forte ordinal.</summary>
    internal static IEnumerable<(string Name, ushort PrimeMask)> All() => Rows();

    private static (string Name, ushort PrimeMask)[] Rows() =>
    [
{entries}
    ];
}}
""")
    print(f"ForteCatalog.cs: {len(catalog)} entries")


# (meter, whole-note values; an "r" prefix is a rest)
BARRING = [
    ("4/4", "3/4 1/2"), ("4/4", "1/4 1/4 1/4 1/4"), ("4/4", "1/2 1/2 1/2"), ("4/4", "1/1 1/1"),
    ("4/4", "1/4 1/2 1/2"), ("4/4", "3/8 3/4 1/8"), ("4/4", "1/2 3/4 3/4"), ("4/4", "3/4 3/4 3/4"),
    ("2/4", "1/1"), ("2/4", "3/4 1/4"), ("2/4", "1/4 1/2 1/4"), ("2/4", "r1/1"), ("2/4", "1/4 r1/2 1/4"),
    ("3/4", "1/2 1/2 1/2"), ("3/4", "1/1"), ("3/4", "3/4 3/4"), ("3/4", "1/4 3/4 1/2"), ("3/4", "r1/2 r1/2 1/4"),
    ("6/8", "1/4 1/4 1/4"), ("6/8", "3/4 3/8"), ("6/8", "1/2 1/2"), ("6/8", "1/8 3/4 1/8"),
    ("5/4", "1/1 1/2"), ("5/4", "1/2 1/2 1/2 1/2"), ("5/4", "3/4 3/4 3/4"),
]


def measures():
    def fmt(n):
        return str(Fraction(n) / 4)

    rows = []
    for i, (m, spec) in enumerate(BARRING, 1):
        s = stream.Stream()
        s.insert(0, meter.TimeSignature(m))
        for tok in spec.split():
            rest = tok.startswith("r")
            ql = Fraction(tok.lstrip("r")) * 4
            n = note.Rest() if rest else note.Note("C4")
            n.quarterLength = ql
            s.append(n)
        bars = s.makeMeasures()
        bars.makeTies(inPlace=True)
        out = []
        for bar in bars.getElementsByClass(stream.Measure):
            toks = []
            for n in bar.notesAndRests:
                tied = not n.isRest and n.tie is not None and n.tie.type in ("start", "continue")
                # music21 keeps a complex duration (5/8) as one note; split it the way Duration.Decompose does.
                parts = n.duration.components
                for j, c in enumerate(parts):
                    toks.append(fmt(c.quarterLength) + ("~" if not n.isRest and (j < len(parts) - 1 or tied) else ""))
            out.append(" ".join(toks))
        rows.append([f"case{i}", m, spec, "|".join(out)])
    write("measures.csv", ["case", "meter", "input", "expected"], rows)


def tempo():
    from fractions import Fraction
    from music21 import stream, tempo as m21tempo

    layouts = [
        [(Fraction(0), 120)],
        [(Fraction(0), 120), (Fraction(2), 90)],
        [(Fraction(0), 60), (Fraction(1, 2), 100), (Fraction(3), 45)],
        [(Fraction(0), 90), (Fraction(5, 4), 150)],  # mid-bar change
        [(Fraction(0), 100), (Fraction(1), 80), (Fraction(5, 2), 140)],
    ]
    rows = []
    for layout in layouts:
        s = stream.Stream()
        for pos, bpm in layout:
            s.insert(float(pos * 4), m21tempo.MetronomeMark(number=bpm, referent=note.Note(type="quarter")))
        for i in range(0, 64):  # one quarter note every 1/8 whole note, off the grid of changes too
            n = note.Note("C4", quarterLength=0.5)
            s.insert(i * 0.5, n)
        changes = " ".join(f"{p.numerator}/{p.denominator}@{b}" for p, b in layout)
        for m in s.secondsMap:
            n = m["element"]
            if not isinstance(n, note.Note):
                continue
            pos = Fraction(n.offset).limit_denominator(64) / 4
            rows.append([changes, f"{pos.numerator}/{pos.denominator}", repr(float(m["offsetSeconds"]))])
    write("tempo.csv", ["changes", "position", "seconds"], rows)


# (meter, whole-note values; an "r" prefix is a rest). Each case fills one bar.
BEAMING = [
    ("2/4", "1/8 1/8 1/8 1/8"), ("2/4", "1/16 1/16 1/16 1/16 1/8 1/8"), ("2/4", "1/4 1/8 1/8"),
    ("2/4", "1/8 1/8 r1/4"), ("2/4", "3/16 1/16 1/8 1/8"),
    ("3/4", "1/8 1/8 1/8 1/8 1/8 1/8"), ("3/4", "1/8 1/8 1/4 1/8 1/8"), ("3/4", "1/8 r1/8 1/8 1/8 1/4"),
    ("3/4", "1/16 1/16 1/16 1/16 1/4 1/4"),
    ("4/4", "1/8 1/8 1/8 1/8 1/8 1/8 1/8 1/8"), ("4/4", "1/8 1/8 1/8 1/8 1/2"), ("4/4", "1/4 1/4 1/4 1/4"),
    ("4/4", "1/8 1/8 r1/8 1/8 1/4 1/4"),
    ("3/8", "1/8 1/8 1/8"), ("3/8", "1/4 1/8"), ("3/8", "1/16 1/16 1/16 1/16 1/8"),
    ("6/8", "1/8 1/8 1/8 1/8 1/8 1/8"), ("6/8", "1/8 r1/8 1/8 1/8 1/8 1/8"), ("6/8", "1/4 1/8 1/8 1/8 1/8"),
    ("9/8", "1/8 1/8 1/8 1/8 1/8 1/8 1/8 1/8 1/8"), ("9/8", "1/8 1/8 1/8 1/4 1/8 1/8 1/8 1/8"),
    ("12/8", "1/8 1/8 1/8 1/8 1/8 1/8 1/8 1/8 1/8 1/8 1/8 1/8"),
    ("5/8", "1/8 1/8 1/8 1/8 1/8"), ("5/8", "1/4 1/8 1/8 1/8"),
    ("7/8", "1/8 1/8 1/8 1/8 1/8 1/8 1/8"), ("7/8", "1/8 1/8 1/4 1/8 1/8 1/8"),
]
# music21's beatSequence has one beat per unit in these meters, so their grouping is read from the
# beams of a bar of unit notes instead. The others are read from beatSequence itself.
ASYMMETRIC = {"5/8", "7/8"}  # not 11/8: music21 beams all eleven together, MusicTheory does 2+2+2+2+3


def beams():
    def build(m, spec):
        s = stream.Stream()
        s.insert(0, meter.TimeSignature(m))
        for tok in spec.split():
            n = note.Rest() if tok.startswith("r") else note.Note("C4")
            n.quarterLength = Fraction(tok.lstrip("r")) * 4
            s.append(n)
        bars = s.makeMeasures()
        bars.makeBeams(inPlace=True)
        return list(bars.recurse().notesAndRests)

    def groups_of(notes):
        out, cur = [], []
        for i, n in enumerate(notes):
            t = n.beams.beamsList[0].type if (not n.isRest and n.beams.beamsList) else None
            if t == "start":
                cur = [i]
            elif t in ("continue", "stop") and cur:
                cur.append(i)
                if t == "stop":
                    out.append(cur)
                    cur = []
        return out

    def show(groups):
        return " ".join("-".join(map(str, g)) for g in groups)

    rows = [[f"case{i}", m, spec, show(groups_of(build(m, spec)))] for i, (m, spec) in enumerate(BEAMING, 1)]
    write("beams.csv", ["case", "meter", "input", "groups"], rows)

    rows = []
    for m in ["2/4", "3/4", "4/4", "2/2", "3/8", "6/8", "9/8", "12/8", "6/4", "5/8", "7/8", "5/4"]:
        n, d = (int(x) for x in m.split("/"))
        if m in ASYMMETRIC:
            lengths = [len(g) for g in groups_of(build(m, " ".join([f"1/{d}"] * n)))]
        else:
            lengths = [round(Fraction(b.duration.quarterLength) / Fraction(4, d)) for b in meter.TimeSignature(m).beatSequence]
        rows.append([m, " ".join(map(str, lengths))])
    write("beatgroups.csv", ["meter", "groups"], rows)


MIDI_CASES = [
    # name, meter, key, tempo (bpm), items: (pitches or "r", quarterLength)
    ("scale-4-4", "4/4", None, 120, [("C4", 1), ("D4", 1), ("r", 1), ("F4", 1), ("G4", 4)]),
    ("waltz-d", "3/4", "D", 100, [("D4", 1), ("F#4", 2), ("A4", 2), ("r", 1), ("D5", 3)]),
    ("jig-6-8", "6/8", "G", 120, [("G4", 0.5), ("A4", 0.5), ("B4", 0.5), ("D5", 1.5), ("r", 0.5), ("B4", 1), ("D5", 1.5)]),
    ("chords-f", "4/4", "F", 90, [("F3 A3 C4", 2), ("Bb3 D4 F4", 2), ("C4 E4 G4", 4)]),
    ("tie-bar", "4/4", None, 120, [("C4", 3), ("E4", 2), ("G4", 3)]),
    ("tempo-change", "2/4", "Bb", 80, [("Bb3", 1), ("D4", 1), ("F4", 1), ("Bb4", 1)]),
]


def midi():
    from music21 import tempo as m21tempo
    out = OUT.parents[1] / "MusicTheory.Midi.UnitTests" / "Reference"
    out.mkdir(parents=True, exist_ok=True)
    rows = []
    for name, m, k, bpm, items in MIDI_CASES:
        s = stream.Stream()
        s.insert(0, meter.TimeSignature(m))
        if k:
            s.insert(0, key.Key(k))
        s.insert(0, m21tempo.MetronomeMark(number=bpm))
        items = list(items)
        for p, q in items:
            if p == "r":
                n = note.Rest()
            elif " " in p:
                n = chord.Chord(p.split())
            else:
                n = note.Note(p)
            n.quarterLength = q
            s.append(n)
        if name == "tempo-change":
            s.insert(2, m21tempo.MetronomeMark(number=60))
        fp = out / f"{name}.mid"
        s.write("midi", fp=str(fp))
        parsed = converter.parse(str(fp))
        entries = []
        for n in parsed.recurse().notesAndRests:
            if n.isRest:
                name_ = "r"
            else:
                name_ = "+".join(ours(p) for p in n.pitches)
            tie = "~" if n.tie is not None and n.tie.type in ("start", "continue") else ""
            entries.append(f"{name_}:{Fraction(n.quarterLength) / 4}{tie}")
        rows.append([f"{name}.mid", m, k or "", " ".join(entries)])
    with open(out / "midi.csv", "w", newline="") as f:
        w = csv.writer(f, lineterminator="\n")
        w.writerow(["file", "meter", "key", "notes"])
        w.writerows(rows)
    print(f"midi.csv: {len(rows)} rows")


def abc():
    """Parses the hand-written folk tunes with music21 into abc.csv (file, meter, key_sharps, notes).

    music21 does not carry an accidental to the end of the bar, so the fixtures never rely on it.
    """
    abc_dir = pathlib.Path(__file__).resolve().parents[2] / "MusicTheory.Abc.UnitTests" / "Reference"
    rows = []
    for path in sorted(abc_dir.glob("*.abc")):
        s = music21.converter.parse(str(path), format="abc")
        flat = s.flatten()
        toks = []
        for n in flat.notesAndRests:
            d = Fraction(n.quarterLength).limit_denominator(1000) / 4
            name = "r" if n.isRest else "+".join(ours(p) for p in n.pitches)
            tied = not n.isRest and n.tie is not None and n.tie.type in ("start", "continue")
            toks.append(f"{name}/{d.numerator}/{d.denominator}{'~' if tied else ''}")
        ks = flat.getElementsByClass(key.KeySignature)[0]
        rows.append([path.name, flat.getElementsByClass(meter.TimeSignature)[0].ratioString, ks.sharps, " ".join(toks)])
    write_into(abc_dir, "abc.csv", ["file", "meter", "key_sharps", "notes"], rows)


def write_into(directory, name, header, rows):
    with open(directory / name, "w", newline="") as f:
        w = csv.writer(f, lineterminator="\n")
        w.writerow(header)
        w.writerows(rows)
    print(f"{name}: {len(rows)} rows")


def musicxml():
    """Single-part MusicXML files written by music21, plus the notes the MusicTheory reader must recover."""
    out = OUT.parent.parent / "MusicTheory.MusicXml.UnitTests" / "Reference"
    out.mkdir(parents=True, exist_ok=True)

    def fmt(ql):
        return str(Fraction(ql) / 4)

    cases = {
        "accidentals": ("4/4", key.KeySignature(0), [("n", 1, "F##4"), ("n", 1, "B-3"), ("n", 1, "C#5"), ("n", 1, "E-4")]),
        "triplets": ("4/4", key.KeySignature(1), [("n", Fraction(2, 3), "G4"), ("n", Fraction(2, 3), "A4"), ("n", Fraction(2, 3), "B4"), ("n", 1, "C5"), ("r", 1, None)]),
        "pickup": ("3/4", key.KeySignature(-2), [("n", 1, "F4"), ("n", 3, "B-4"), ("n", 2, "A4"), ("n", 1, "G4")]),
        "ties": ("4/4", key.KeySignature(0), [("n", 3, "C4"), ("n", 3, "C4"), ("n", 2, "C4")]),
        "chords": ("4/4", key.KeySignature(2), [("c", 2, ["D4", "F#4", "A4"]), ("c", 2, ["G3", "B3", "D4"]), ("c", 4, ["D4", "F#4", "A4"])]),
        "minor": ("6/8", key.Key("a", "minor"), [("n", 1.5, "A4"), ("n", 0.5, "G#4"), ("n", 0.5, "A4"), ("n", 0.5, "B4"), ("r", 3, None)]),
    }
    rows = []
    for name, (m, k, items) in cases.items():
        s = stream.Part()
        s.insert(0, k)
        s.insert(0, meter.TimeSignature(m))
        built = []
        for kind, ql, pitch in items:
            n = note.Rest() if kind == "r" else chord.Chord(pitch) if kind == "c" else note.Note(pitch)
            n.quarterLength = ql
            built.append(n)
            s.append(n)
        if name == "pickup":
            # one beat, then two full 3/4 bars: the first bar is the pickup (number 0)
            bars = [stream.Measure(number=i) for i in range(3)]
            bars[0].insert(0, k)
            bars[0].insert(0, meter.TimeSignature(m))
            for bar, chunk in zip(bars, (built[:1], built[1:2], built[2:])):
                for n in chunk:
                    bar.append(n)
            s = stream.Part(bars)
        else:
            s = s.makeMeasures()
        s.makeTies(inPlace=True)
        fp = out / f"{name}.musicxml"
        s.write("musicxml", fp=str(fp))
        if name == "pickup":
            # the exporter never writes implicit="yes"; add it the way MuseScore marks a pickup
            fp.write_text(fp.read_text().replace('<measure implicit="no" number="0">', '<measure implicit="yes" number="0">', 1))
        toks = []
        for n in s.recurse().notesAndRests:
            if n.isRest:
                toks.append(f"r:{fmt(n.quarterLength)}")
                continue
            names = "+".join(ours(p) for p in (n.pitches if n.isChord else [n.pitch]))
            tied = n.tie is not None and n.tie.type in ("start", "continue")
            toks.append(f"{names}:{fmt(n.quarterLength)}" + ("~" if tied else ""))
        mode = getattr(k, "mode", None) or "major"
        rows.append([f"{name}.musicxml", m, k.sharps, mode, " ".join(toks)])
    write_to(out / "musicxml.csv", ["file", "meter", "fifths", "mode", "notes"], rows)


if __name__ == "__main__":
    print("music21", music21.__version__)
    OUT.mkdir(parents=True, exist_ok=True)
    intervals()
    transpositions()
    scales()
    modes()
    keys()
    neoriemannian()
    durations()
    catalog = pcsets()
    if "--csharp-forte" in sys.argv:
        forte_catalog(catalog)
    measures()
    tempo()
    beams()
    midi()
    abc()
    musicxml()
