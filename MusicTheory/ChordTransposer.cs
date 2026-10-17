namespace MusicTheory;

/// <summary>
/// String-in, string-out chord transposition for lyric sheets and tools. Input that is not a chord
/// symbol (<c>N.C.</c>, <c>%</c>, lyrics) is returned unchanged.
/// </summary>
public static class ChordTransposer
{
    /// <summary>
    /// Transposes a chord symbol by semitones. With a <paramref name="targetKey"/> the result is spelled in
    /// that key. Without one, a sharp root keeps sharps, a flat root keeps flats, and a natural root uses
    /// Db Eb F# Ab Bb, so a plain Bb is never written A#.
    /// </summary>
    /// <param name="chordSymbol">The chord, e.g. <c>D/F#</c>.</param>
    /// <param name="semitones">Semitones up (positive) or down (negative).</param>
    /// <param name="targetKey">The key of the result, if known.</param>
    /// <param name="style">Output style; defaults to the input's own: Unicode accidentals if it uses ♭/♯, jazz glyphs if it uses ° ø Δ +.</param>
    public static string Transpose(string chordSymbol, int semitones, KeySignature? targetKey = null, ChordSymbolStyle? style = null)
    {
        if (!ChordSymbol.TryParse(chordSymbol, out var chord))
            return chordSymbol;
        targetKey ??= DefaultKey(chord);
        return chord.Transpose(semitones, targetKey).ToString(style ?? StyleOf(chordSymbol));
    }

    /// <summary>Transposes a chord symbol from one key to another, spelled in <paramref name="to"/>.</summary>
    public static string Transpose(string chordSymbol, KeySignature from, KeySignature to, ChordSymbolStyle? style = null) =>
        ChordSymbol.TryParse(chordSymbol, out var chord)
            ? chord.Transpose(from, to).ToString(style ?? StyleOf(chordSymbol))
            : chordSymbol;

    private static KeySignature DefaultKey(ChordSymbol chord) => Math.Sign((int)chord.Root.Alteration) switch
    {
        > 0 => new KeySignature(new Note(NoteName.G), KeyMode.Major),  // sharp side
        < 0 => new KeySignature(new Note(NoteName.F), KeyMode.Major),  // flat side
        _ => new KeySignature(new Note(NoteName.C), KeyMode.Major),
    };

    private static ChordSymbolStyle StyleOf(string text) => new(
        text.AsSpan().IndexOfAny("♭♯") >= 0 ? AccidentalStyle.Unicode : AccidentalStyle.Ascii,
        text.AsSpan().IndexOfAny("°øΔ+") >= 0);
}
