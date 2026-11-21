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
