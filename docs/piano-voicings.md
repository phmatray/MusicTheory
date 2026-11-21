---
title: Piano voicings
description: Voice a chord as a pianist plays it (close, drop 2, drop 3, shell, rootless) and voice-lead a progression with the least movement.
section: Topics
order: 10
---

`PianoVoicer` turns a chord symbol into the notes a pianist plays, and `VoiceLeader` picks those shapes
for a whole progression so that each one moves the least from the one before. Guitar shapes are a
different subject and live in the instruments guide.

## Voicing styles

`PianoVoicer.Voice(chord, style, octave, inversion)` returns the notes ascending, spelled by interval.
Every style except `Close` needs a 7th or a 6th in the chord. Here is `Cmaj7` in each
`PianoVoicingStyle`:

```csharp
var cmaj7 = ChordSymbol.Parse("Cmaj7");
string Show(PianoVoicingStyle style) => string.Join(" ", PianoVoicer.Voice(cmaj7, style));
Console.WriteLine(Show(PianoVoicingStyle.Close));       // C4 E4 G4 B4
Console.WriteLine(Show(PianoVoicingStyle.Drop2));       // G3 C4 E4 B4
Console.WriteLine(Show(PianoVoicingStyle.Drop3));       // E3 C4 G4 B4
Console.WriteLine(Show(PianoVoicingStyle.Shell));       // C3 E3 B3
Console.WriteLine(Show(PianoVoicingStyle.RootlessA));   // E4 G4 B4 D5
Console.WriteLine(Show(PianoVoicingStyle.RootlessB));   // B4 D5 E5 G5
```

- **Close** stacks the chord tones in thirds.
- **Drop 2** takes the second note from the top of the close voicing down an octave (G here).
- **Drop 3** does the same with the third note from the top (E here).
- **Shell** is the root, then the third and the seventh. The root sits one octave below the `octave` argument.
- **Rootless A** is third, fifth, seventh, ninth; **rootless B** is seventh, ninth, third, fifth. The bass player supplies the root.

The `inversion` argument rotates the close stack before any dropping, and a shell has two orders:

```csharp
Console.WriteLine(string.Join(" ", PianoVoicer.Voice(cmaj7, PianoVoicingStyle.Close, 4, 1)));   // E4 G4 B4 C5
Console.WriteLine(string.Join(" ", PianoVoicer.Voice(cmaj7, PianoVoicingStyle.Shell, 4, 1)));   // C3 B3 E4
```

## Voice leading a progression

`VoiceLeader.Lead(chords, style, octave)` voices the first chord, then gives each next chord the
candidate with the least total movement from the previous voicing. A candidate that creates a parallel
fifth, octave or unison is penalised. For the ii-V-I in C with rootless A voicings:

```csharp
var voicings = VoiceLeader.Lead(
    new[] { "Dm7", "G7", "Cmaj7" }.Select(ChordSymbol.Parse), PianoVoicingStyle.RootlessA);
Console.WriteLine(string.Join(" ", voicings[0]));   // F4 A4 C5 E5
Console.WriteLine(string.Join(" ", voicings[1]));   // F4 A4 B4 D5
Console.WriteLine(string.Join(" ", voicings[2]));   // E4 G4 B4 D5
```

Each step moves little. From Dm7 to G7, F and A stay put, C falls a semitone to B and E falls two to D:
3 semitones. From G7 to Cmaj7, B and D stay put, F falls a semitone to E and A falls two to G: 3 semitones.
The whole progression moves 6 semitones, with the guide tones (the third and the seventh) resolving by step,
which is the point of rootless voicings (Levine, *The Jazz Piano Book*).

`Lead` is greedy and deterministic. It does not look ahead, and it only guards against the parallels
described above. To check a realisation against the full set of voice-leading rules, see
the voice-leading checks in [Analysing progressions](analyzing-progressions.md).
