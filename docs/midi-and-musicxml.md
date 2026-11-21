---
title: MIDI and MusicXML files
description: Write measures to a Standard MIDI file or a MusicXML score and read them back, with ties, tempo, meter and key preserved.
section: Topics
order: 15
---

[Measures and tempo](measures-and-tempo.md) builds the structure; this page puts it in files. Two packages do
the work: `MusicTheory.Midi` reads and writes Standard MIDI files, and `MusicTheory.MusicXml` reads and writes
single-part MusicXML. [MIDI and frequencies](midi-integration.md) is a different subject: note numbers and
pitches, not files. Every example here uses a `MemoryStream` or an `XDocument`, so nothing touches the disk.

## MIDI

`MidiWriter.Write` takes a stream, the measures, a `TempoMap` and an optional key. Two 4/4 bars follow, and the
A in bar one is a half note tied to the A that opens bar two:

```csharp
using MusicTheory.Midi;
using System.IO;

var quarter = new Duration(DurationType.Quarter);
var half = new Duration(DurationType.Half);
var bar1 = new Measure(TimeSignature.CommonTime, [
    new RhythmEvent([Note.Parse("D4")], quarter),
    new RhythmEvent([Note.Parse("F#4")], quarter),
    new RhythmEvent([Note.Parse("A4")], half, TiedToNext: true)]);
var bar2 = new Measure(TimeSignature.CommonTime, [
    new RhythmEvent([Note.Parse("A4")], half),
    new RhythmEvent([Note.Parse("D5")], half)]);
Measure[] tune = [bar1, bar2];

var file = new MemoryStream();
MidiWriter.Write(file, tune, new TempoMap(96), KeySignature.Parse("D"));
file.Position = 0;

var import = MidiReader.Read(file);
Console.WriteLine(import.TimeSignature);                    // 4/4
Console.WriteLine(import.Key);                              // D
Console.WriteLine(import.Tempo.BpmAt(Rational.Of(0)));      // 96
Console.WriteLine(import.Measures.Count);                   // 2
Console.WriteLine(import.Measures[0].Events[2].TiedToNext); // True
```

The tie crossed the barline inside the file as one long note, and the reader cut it at the barline again.
`MidiImport` carries `TimeSignature` (4/4 when the file has none), `Key` (null when it has none), `Tempo` (a
`TempoMap`, 120 bpm when the file has none) and `Measures`. A quarter at 96 bpm lasts 60/96 = 0.625 s:

```csharp
Console.WriteLine(import.Tempo.SecondsAt(Rational.Of(1, 4)));   // 0.625
```

The measures come back with the same capacities, pitches and ties:

```csharp
string Describe(Measure m) => string.Join(" ", m.Events.Select(e => $"{string.Join("+", e.Pitches)}:{e.Duration.Value}{(e.TiedToNext ? "~" : "")}"));

Console.WriteLine(Describe(import.Measures[0]));                      // D4:1/4 F#4:1/4 A4:1/2~
Console.WriteLine(Describe(import.Measures[1]) == Describe(bar2));    // True
```

### Options and the tick grid

`MidiWriteOptions` sets `TicksPerQuarter` (480 by default), the `Channel` and the `Velocity`. A file stores
positions in whole ticks, so a position that falls between two ticks cannot be written, and the
`ArgumentException` names a `TicksPerQuarter` that works. Seven eighth-note tuplets in the space of four
(each 1/14 of a whole note) do not land on a 480 grid:

```csharp
var tuplet = Duration.CreateTuplet(DurationType.Eighth, 7, 4);
var septuplet = new Measure(TimeSignature.CommonTime, Enumerable.Range(0, 7).Select(_ => new RhythmEvent([Note.Parse("C4")], tuplet)).ToList());

try
{
    MidiWriter.Write(new MemoryStream(), [septuplet], new TempoMap(120));
}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message.Split("such as ")[1].TrimEnd('.'));   // 3360
}

MidiWriter.Write(new MemoryStream(), [septuplet], new TempoMap(120), options: new MidiWriteOptions(TicksPerQuarter: 3360));
```

On the way back, `MidiReader.Read` snaps onsets and durations to a grid of 1/64 of a whole note by default.
The second argument changes it, and a grid fine enough for the tuplet reads it back as seven events:

```csharp
var tupletFile = new MemoryStream();
MidiWriter.Write(tupletFile, [septuplet], new TempoMap(120), options: new MidiWriteOptions(TicksPerQuarter: 3360));
tupletFile.Position = 0;

var fine = MidiReader.Read(tupletFile, quantizeGrid: Rational.Of(1, 14));
Console.WriteLine(fine.Measures.Sum(m => m.Events.Count));   // 7
```

## MusicXML

`MusicXmlWriter.Write` returns an `XDocument` and `MusicXmlReader.Read` takes one. Both stay in memory until
you call `Save` or `Load` yourself:

```csharp
using MusicTheory.MusicXml;

var xml = MusicXmlWriter.Write(tune, KeySignature.Parse("D"), "Little tune");
var score = MusicXmlReader.Read(xml);

Console.WriteLine(score.Title);                              // Little tune
Console.WriteLine(score.Key);                                // D
Console.WriteLine(score.TimeSignature);                      // 4/4
Console.WriteLine(score.Measures.Count);                     // 2
Console.WriteLine(score.Measures[0].Events[2].TiedToNext);   // True
```

`MusicXmlScore` holds the `Title` and `Key` (null when the file has none), the `TimeSignature` of the first
bar and the `Measures` of the first part, each carrying the meter in force. A bar shorter than its meter is
written as a pickup and read back as one.

## Choosing a format

| | MIDI | MusicXML |
|---|---|---|
| Spelling | Pitch numbers only, so F# and Gb are the same note | Exact spelling, as the library holds it |
| Tempo | Kept as a tempo map | Not part of `MusicXmlScore` |
| Best for | Playback, sequencers | Notation software |

The same measures go through either one and come back with their capacities, pitches and ties intact.
