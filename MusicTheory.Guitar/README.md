# MusicTheory.Guitar

Guitar companion to [MusicTheory](https://www.nuget.org/packages/MusicTheory): tunings, chord voicings
and chord-diagram geometry as plain data. No Blazor, ASP.NET Core or JavaScript dependency, so any UI
(Blazor, MAUI, a web front end, a CLI) can render it.

```csharp
using MusicTheory.Guitar;

var voicings = new GuitarVoicingProvider().GetVoicings(ChordSymbol.Parse("F#m7"));
voicings[0].ToTab();    // "242222"
voicings[0].Barre;      // Barre(2, 0, 5): index finger across fret 2

// Capo: frets are relative to the capo; the shape is named, the sound is transposed.
var g = new GuitarVoicingProvider().GetVoicings(ChordSymbol.Parse("G"), new VoicingOptions(Capo: 2))[0];
g.Sound(Tuning.Standard, capo: 2); // A major
```

Every `ChordType` on every root has at least one voicing in standard tuning: curated open shapes, movable
E/A/D-form barres and a fretboard search, ranked easiest first.

Source and issues: https://github.com/phmatray/MusicTheory
