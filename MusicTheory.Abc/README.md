# MusicTheory.Abc

ABC notation for [MusicTheory](https://www.nuget.org/packages/MusicTheory): read a single-voice ABC tune into
its header fields and measures of spelled notes with exact durations.

```csharp
using MusicTheory.Abc;

var tune = AbcTune.Parse("""
    X:1
    T:Speed the Plough
    M:4/4
    L:1/8
    Q:1/4=120
    K:G
    GABc dedB|dedB dedB|
    """);
tune.Title;       // "Speed the Plough"
tune.Key;         // G major
tune.Measures[0]; // G4 A4 B4 C5 D5 E5 D5 B4, all eighths
```

## Supported subset

Headers `X:` `T:` `M:` (including `C` and `C|`) `L:` `Q:` `K:`; notes with octave marks, `z` rests, lengths
(`2`, `/2`, `3/2`), ties, chords, triplets `(3`, and bar lines. A first bar shorter than the meter is a pickup.
`AbcTune.ToString()` writes the canonical form back, and parsing it returns equal measures.

## Modal keys

A modal key is stored as the major key with the same signature, so the tune keeps its notes and the
signature is right: `K:Ador` and `K:Gmaj` both give G major (one sharp), `K:Edor` gives D major.

## Reference

The parser is checked against music21 on six hand-written folk-tune fixtures
(`MusicTheory.Abc.UnitTests/Reference`, regenerate with `tools/reference/generate.py`). music21 does not carry
an accidental to the end of the bar, so the fixtures only use accidentals where both readings agree.

Accidentals follow the key and last to the end of the bar, as in ABC. Modes (`K:Edor`) are stored as their
signature-equivalent major key (D major). Repeats are plain barlines; `V:` voices and `w:` lyrics throw
`NotSupportedException`. Only the first tune of a text is read.
