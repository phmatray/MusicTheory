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

Accidentals follow the key and last to the end of the bar, as in ABC. Modes (`K:Edor`) are stored as their
signature-equivalent major key (D major). Repeats are plain barlines; `V:` voices and `w:` lyrics throw
`NotSupportedException`. Only the first tune of a text is read.
