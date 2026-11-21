# MusicTheory.ChordPro

ChordPro chord sheets for [MusicTheory](https://www.nuget.org/packages/MusicTheory): parse a sheet into
directives, lyric lines with chord segments and comments, and write it back byte for byte.

```csharp
using MusicTheory.ChordPro;

var song = ChordProDocument.Parse("{t: Amazing Grace}\n{key: G}\n[G]Amazing [C]grace");
song.Title;      // "Amazing Grace"
song.Key;        // G major
song.Chords;     // ["G", "C"]
song.ToString(); // the input text, unchanged
```

Lines the parser cannot classify become lyric lines, so `Parse` never throws on content. Each line keeps its
`Raw` text (a trailing `\r` included), which is what makes the round trip exact.
