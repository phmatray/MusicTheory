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

## Transposing

```csharp
song.Transpose(2).ToString();   // {key: A} ... [A]Amazing [A7]grace ...
song.Transpose(new KeySignature(Note.Parse("Bb"), KeyMode.Major)); // spelled in Bb
```

Every chord goes through `ChordTransposer` and the `{key:}` directive is rewritten; lyrics, annotations
(`[*Riff]`), unparseable chords (`[N.C.]`, `[Gfoo/B]`) and `{start_of_tab}` blocks are left as written by default;
`Transpose(2, SuffixHandling.KeepUnparsed)` also moves the root and bass of chords the library cannot read
(`[Gfoo/B]` becomes `[Afoo/C#]`). Only changed lines
get new text. `Transpose(KeySignature)` needs a parseable `{key:}`, else it throws `InvalidOperationException`.
Suffixes are rewritten to their canonical form by default (`CM7` becomes `Dmaj7`);
`Transpose(2, SuffixHandling.Verbatim)` keeps the writer's spelling and moves only root and bass (`CM7` becomes `DM7`, `C-7` becomes `D-7`).
