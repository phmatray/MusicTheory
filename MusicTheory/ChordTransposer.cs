namespace MusicTheory;

/// <summary>
/// String-in, string-out chord transposition for lyric sheets and tools. Input that is not a chord
/// symbol (<c>N.C.</c>, <c>%</c>, lyrics) is returned unchanged.
/// </summary>
public static class ChordTransposer
{
    /// <summary>
    /// Transposes a chord symbol by semitones. With a <paramref name="targetKey"/> the result is spelled in
    /// that key. Without one, the new root keeps the side of the old one (a sharp root keeps sharps, a flat
    /// root flats, a natural root uses Db Eb F# Ab Bb, so a plain Bb is never written A#), and the slash bass
    /// is spelled in the major key of the new root: D/F# up two semitones is E/G#.
    /// </summary>
    /// <param name="chordSymbol">The chord, e.g. <c>D/F#</c>.</param>
    /// <param name="semitones">Semitones up (positive) or down (negative).</param>
    /// <param name="targetKey">The key of the result, if known.</param>
    /// <param name="style">Output style; defaults to the input's own: Unicode accidentals if it uses ♭/♯, jazz glyphs if it uses ° ø Δ +.</param>
    /// <param name="suffix">With <see cref="SuffixHandling.KeepUnparsed"/>, text that is not a chord but starts with a root (<c>Cfoo/E</c>) still moves its root and bass and keeps the rest as written. With <see cref="SuffixHandling.Verbatim"/>, every chord keeps its suffix as written (<c>C-7</c> becomes <c>D-7</c>).</param>
    public static string Transpose(string chordSymbol, int semitones, KeySignature? targetKey = null, ChordSymbolStyle? style = null, SuffixHandling suffix = SuffixHandling.Canonical) =>
        Move(chordSymbol, c => c.Transpose(semitones, Practical(targetKey ?? KeyOfNewRoot(c, semitones))), style, suffix);

    /// <summary>Transposes a chord symbol from one key to another, spelled in <paramref name="to"/>.</summary>
    public static string Transpose(string chordSymbol, KeySignature from, KeySignature to, ChordSymbolStyle? style = null, SuffixHandling suffix = SuffixHandling.Canonical) =>
        Move(chordSymbol, c => c.Transpose(Practical(from), Practical(to)), style, suffix);

    private static string Move(string text, Func<ChordSymbol, ChordSymbol> move, ChordSymbolStyle? style, SuffixHandling suffix)
    {
        if (suffix != SuffixHandling.Verbatim && ChordSymbol.TryParse(text, out var chord))
            return move(chord).ToString(style ?? StyleOf(text));
        if (suffix == SuffixHandling.Canonical || !TrySplit(text.Trim(), out var root, out var tail, out var bass))
            return text;

        // Root and bass go through a scratch chord so the spelling rules live in one place.
        var moved = move(new ChordSymbol(root, ChordType.Major, bass));
        var accidentals = (style ?? StyleOf(text)).Accidentals;
        return ChordSymbol.Spell(moved.Root, accidentals) + tail + (moved.Bass is null ? "" : "/" + ChordSymbol.Spell(moved.Bass, accidentals));
    }

    // Same root and bass rules as ChordSymbol.TryParse; the tail is everything between them, verbatim.
    private static bool TrySplit(string s, out Note root, out string tail, out Note? bass)
    {
        root = default!;
        tail = s;
        bass = null;
        if (s.Length == 0 || !char.IsUpper(s[0]) || !Note.TryParsePitchClass(s, out var r, out var consumed))
            return false;

        root = r;
        tail = s[consumed..];
        int slash = tail.LastIndexOf('/');
        if (slash >= 0)
        {
            var b = tail.AsSpan(slash + 1);
            if (!b.IsEmpty && char.IsUpper(b[0]) && Note.TryParsePitchClass(b, out var n, out var used) && used == b.Length)
            {
                bass = n;
                tail = tail[..slash];
            }
        }

        return true;
    }

    // A theoretical key (A# major, ten sharps) is read as its practical enharmonic (Bb).
    private static KeySignature Practical(KeySignature key) =>
        Math.Abs(key.AccidentalCount) <= 7 ? key : key.GetEnharmonicEquivalents().First();

    // The new root, spelled on the old root's side, then its major key when that key has a real signature.
    private static KeySignature KeyOfNewRoot(ChordSymbol chord, int semitones)
    {
        var side = Side(chord);
        var key = new KeySignature(chord.Root.TransposeBySemitones(semitones, side), KeyMode.Major);
        return Math.Abs(key.AccidentalCount) <= 7 ? key : side;
    }

    private static KeySignature Side(ChordSymbol chord) => Math.Sign((int)chord.Root.Alteration) switch
    {
        > 0 => new KeySignature(new Note(NoteName.G), KeyMode.Major),  // sharp side
        < 0 => new KeySignature(new Note(NoteName.F), KeyMode.Major),  // flat side
        _ => new KeySignature(new Note(NoteName.C), KeyMode.Major),
    };

    private static ChordSymbolStyle StyleOf(string text) => new(
        text.AsSpan().IndexOfAny("♭♯") >= 0 ? AccidentalStyle.Unicode : AccidentalStyle.Ascii,
        text.AsSpan().IndexOfAny("°øΔ+") >= 0);
}
