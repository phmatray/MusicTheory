# MusicTheory.Midi

Standard MIDI files for [MusicTheory](https://www.nuget.org/packages/MusicTheory): write measures, a tempo map
and a key as a format 1 file, and read format 0 or 1 files back. No dependencies beyond MusicTheory.

```csharp
using MusicTheory.Midi;

using var file = File.Create("tune.mid");
MidiWriter.Write(file, measures, new TempoMap(96), key: KeySignature.Parse("D"),
                 options: new MidiWriteOptions(TicksPerQuarter: 480, Channel: 0, Velocity: 90));
```

Track 0 holds the time signature, key signature and tempo meta events; track 1 holds the notes. Tied events
become one note, and a position that does not fall on a whole tick throws an `ArgumentException` that names
a `TicksPerQuarter` that works (a 7-tuplet at 480 fails, 3360 works).

```csharp
MidiImport import = MidiReader.Read(File.OpenRead("tune.mid"));
// import.TimeSignature, import.Key, import.Tempo, import.Measures (ties restored across barlines)
```

The reader merges all tracks into one voice, snaps onsets and durations to a grid (1/64 of a whole note by
default) and throws `NotSupportedException` for SMPTE division or notes that overlap with different durations.
