# MusicTheory.MusicXml

MusicXML for [MusicTheory](https://www.nuget.org/packages/MusicTheory): write measures, key, meter and title as
a single-part partwise MusicXML 4.0 document, built with `System.Xml.Linq` only.

```csharp
using MusicTheory.MusicXml;

XDocument doc = MusicXmlWriter.Write(measures, KeySignature.Parse("F"), title: "Minuet");
doc.Save("minuet.musicxml");
```

Spelled notes map one to one onto `<step>`, `<alter>` and `<octave>`. Chords, ties, rests, dots, tuplets and
pickup measures are written; beams, slurs, dynamics and lyrics are not.

Read the first part of a partwise file back into measures:

```csharp
MusicXmlScore score = MusicXmlReader.Read(XDocument.Load("minuet.musicxml"));
Console.WriteLine($"{score.Title} in {score.Key}, {score.Measures.Count} measures");
```

The reader is pinned to files written by [music21](https://www.music21.org/) (accidentals, triplets, pickups,
ties, chords, minor keys). Several voices (`<backup>`, `<forward>`), score-timewise files and microtones throw
`NotSupportedException`; grace and cue notes are skipped.
