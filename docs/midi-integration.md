---
title: MIDI and frequencies
description: MIDI note numbers, frequencies at any concert pitch, and piano-keyboard geometry.
section: Topics
order: 4
---

## MIDI note numbers

MIDI numbers run from 0 (C-1) to 127 (G9); middle C is 60.

```csharp
Console.WriteLine(Note.Parse("C4").MidiNumber);    // 60
Console.WriteLine(Note.Parse("A4").MidiNumber);    // 69
Console.WriteLine(Note.Parse("B#3").MidiNumber);   // 60
Console.WriteLine(Note.Parse("C-1").MidiNumber);   // 0
```

A note outside 0..127 has no MIDI number:

```csharp
try
{
    _ = Note.Parse("Cb-1").MidiNumber;
}
catch (InvalidOperationException e)
{
    Console.WriteLine(e.Message);   // Cb-1 is outside the MIDI range 0..127.
}
```

From a number, black keys are spelled C♯ E♭ F♯ A♭ B♭ by default, or D♭ E♭ G♭ A♭ B♭ with `preferFlats`:

```csharp
Console.WriteLine(string.Join(" ", Enumerable.Range(60, 12).Select(n => Note.FromMidiNumber(n))));
// C4 C#4 D4 Eb4 E4 F4 F#4 G4 Ab4 A4 Bb4 B4
Console.WriteLine(Note.FromMidiNumber(66, preferFlats: true));   // Gb4
```

To spell incoming MIDI in a key, respell with the key:

```csharp
var key = KeySignature.Parse("E");
var played = new[] { 64, 68, 71, 75 }.Select(n => Note.FromMidiNumber(n).TransposeBySemitones(0, key));
Console.WriteLine(string.Join(" ", played));   // E4 G#4 B4 D#5
```

## Chords to MIDI

```csharp
var notes = ChordSymbol.Parse("Cmaj7").Voice(4);
Console.WriteLine(string.Join(" ", notes.Select(n => n.MidiNumber)));   // 60 64 67 71

var identified = ChordFinder.IdentifyFromPitchClasses(new[] { 60, 64, 67, 70 }.Select(n => n % 12));
Console.WriteLine(identified.OrderBy(m => m.ExtraNotes).First().Symbol);   // C7
```

## Frequencies

Frequencies use twelve-tone equal temperament from a reference A4: 440 Hz by default, any value on request.

```csharp
Console.WriteLine(Math.Round(Note.Parse("A4").Frequency, 2));             // 440
Console.WriteLine(Math.Round(Note.Parse("A3").Frequency, 2));             // 220
Console.WriteLine(Math.Round(Note.Parse("C4").Frequency, 2));             // 261.63
Console.WriteLine(Math.Round(Note.Parse("A4").GetFrequency(442), 2));     // 442
Console.WriteLine(Math.Round(Note.Parse("C4").GetFrequency(415), 2));     // 246.76
```

## Piano keyboard

`PianoLayout.Keys` returns the geometry of a keyboard range in white-key units, ready to draw:

```csharp
var keys = PianoLayout.Keys(60, 64);
Console.WriteLine(string.Join(" ", keys.Select(k => $"{k.Midi}:{(k.IsBlack ? "black" : "white")}@{k.X}")));
// 60:white@0 61:black@0.7 62:white@1 63:black@1.7 64:white@2
```
