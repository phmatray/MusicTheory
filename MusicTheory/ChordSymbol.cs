using System.Diagnostics.CodeAnalysis;

namespace MusicTheory;

/// <summary>
/// How accidentals are written: <c>Bb</c> / <c>F#</c> or <c>B♭</c> / <c>F♯</c>.
/// </summary>
public enum AccidentalStyle
{
    /// <summary>ASCII: <c>b</c>, <c>#</c>, <c>bb</c>, <c>##</c>.</summary>
    Ascii,
    /// <summary>Unicode: ♭, ♯, 𝄫, 𝄪.</summary>
    Unicode
}

/// <summary>
/// How a <see cref="ChordSymbol"/> is rendered.
/// </summary>
/// <param name="Accidentals">Accidental characters for the root, the bass and alterations.</param>
/// <param name="UseJazzGlyphs">Write ° ø + instead of <c>dim</c>, <c>m7b5</c>, <c>aug</c>.</param>
public sealed record ChordSymbolStyle(AccidentalStyle Accidentals, bool UseJazzGlyphs)
{
    /// <summary>Lead-sheet ASCII: <c>Bbmaj7</c>, <c>Cdim7</c>, <c>Bm7b5</c>, <c>Caug</c>.</summary>
    public static readonly ChordSymbolStyle Ascii = new(AccidentalStyle.Ascii, false);

    /// <summary>Unicode with jazz glyphs: <c>B♭maj7</c>, <c>C°7</c>, <c>Bø7</c>, <c>C+</c>.</summary>
    public static readonly ChordSymbolStyle Unicode = new(AccidentalStyle.Unicode, true);
}

/// <summary>
/// A lead-sheet chord symbol such as <c>Am7</c>, <c>D/F#</c> or <c>B♭Δ7</c>: a root, a chord type and an
/// optional bass that need not be a chord tone (<c>C/Bb</c>).
/// </summary>
/// <param name="Root">The chord root (octave is not significant).</param>
/// <param name="Type">The chord type.</param>
/// <param name="Bass">The slash bass, or null when the root is in the bass.</param>
public sealed record ChordSymbol(Note Root, ChordType Type, Note? Bass = null)
{
    // Canonical ASCII suffix per type. Parsing also accepts every alias normalised by Normalize.
    private static readonly Dictionary<ChordType, string> Suffixes = new()
    {
        [ChordType.Major] = "", [ChordType.Minor] = "m", [ChordType.Diminished] = "dim", [ChordType.Augmented] = "aug",
        [ChordType.Major7] = "maj7", [ChordType.Minor7] = "m7", [ChordType.Dominant7] = "7",
        [ChordType.MinorMajor7] = "mMaj7", [ChordType.HalfDiminished7] = "m7b5", [ChordType.Diminished7] = "dim7",
        [ChordType.Augmented7] = "aug7", [ChordType.AugmentedMajor7] = "maj7#5",
        [ChordType.Sus2] = "sus2", [ChordType.Sus4] = "sus4", [ChordType.Sus2Sus4] = "sus2sus4", [ChordType.Dominant7Sus4] = "7sus4",
        [ChordType.Major6] = "6", [ChordType.Minor6] = "m6", [ChordType.Major6Add9] = "6/9",
        [ChordType.Major9] = "maj9", [ChordType.Minor9] = "m9", [ChordType.Dominant9] = "9",
        [ChordType.Major11] = "maj11", [ChordType.Minor11] = "m11", [ChordType.Dominant11] = "11",
        [ChordType.Major13] = "maj13", [ChordType.Minor13] = "m13", [ChordType.Dominant13] = "13",
        [ChordType.MajorAdd9] = "add9", [ChordType.MinorAdd9] = "madd9", [ChordType.MajorAdd11] = "add11", [ChordType.MinorAdd11] = "madd11",
        [ChordType.Dominant7Flat5] = "7b5", [ChordType.Dominant7Sharp5] = "7#5", [ChordType.Dominant7Flat9] = "7b9",
        [ChordType.Dominant7Sharp9] = "7#9", [ChordType.Dominant7Flat5Flat9] = "7b5b9", [ChordType.Dominant7Flat5Sharp9] = "7b5#9",
        [ChordType.Dominant7Sharp5Flat9] = "7#5b9", [ChordType.Dominant7Sharp5Sharp9] = "7#5#9", [ChordType.Dominant7Alt] = "7alt",
        [ChordType.Power5] = "5",
    };

    private static readonly Dictionary<ChordType, string> JazzGlyphs = new()
    {
        [ChordType.Diminished] = "°", [ChordType.Diminished7] = "°7", [ChordType.HalfDiminished7] = "ø7",
        [ChordType.Augmented] = "+", [ChordType.Augmented7] = "+7", [ChordType.AugmentedMajor7] = "+maj7",
    };

    // Normalised (lower-case, ASCII, no parentheses) suffix → type.
    private static readonly Dictionary<string, ChordType> Lookup = BuildLookup();

    private static Dictionary<string, ChordType> BuildLookup()
    {
        var lookup = Suffixes.ToDictionary(kv => kv.Value.ToLowerInvariant(), kv => kv.Key);
        lookup["maj"] = ChordType.Major;
        lookup["augmaj7"] = ChordType.AugmentedMajor7;
        lookup["sus"] = ChordType.Sus4;
        lookup["7sus"] = ChordType.Dominant7Sus4;
        lookup["69"] = ChordType.Major6Add9;
        lookup["add2"] = ChordType.MajorAdd9;
        lookup["madd2"] = ChordType.MinorAdd9;
        return lookup;
    }

    /// <summary>
    /// Parses a chord symbol: an upper-case root (<c>#</c>, <c>b</c>, ♯, ♭ accidentals), a suffix and an
    /// optional <c>/bass</c>. Accepts common aliases: <c>m</c>/<c>min</c>/<c>-</c>, <c>maj7</c>/<c>M7</c>/<c>Δ</c>/<c>Δ7</c>/<c>ma7</c>,
    /// <c>dim</c>/<c>°</c>/<c>o</c>, <c>m7b5</c>/<c>ø</c>/<c>ø7</c>/<c>m7(b5)</c>, <c>aug</c>/<c>+</c>, <c>sus</c>, <c>add2</c>,
    /// <c>6/9</c>/<c>69</c>, <c>7alt</c>, <c>mMaj7</c>/<c>m(maj7)</c>, <c>5</c>, and parenthesised alterations (<c>7(b9)</c>).
    /// </summary>
    /// <exception cref="FormatException">The text is not a chord symbol.</exception>
    public static ChordSymbol Parse(string text) =>
        TryParse(text, out var symbol) ? symbol : throw new FormatException($"Not a chord symbol: '{text}'.");

    /// <summary>
    /// Tries to parse a chord symbol; see <see cref="Parse"/>. Returns false (never throws) for
    /// <c>N.C.</c>, <c>%</c>, empty strings and other non-chords.
    /// </summary>
    public static bool TryParse(string? text, [NotNullWhen(true)] out ChordSymbol? symbol)
    {
        symbol = null;
        var s = text.AsSpan().Trim();
        if (s.IsEmpty || !char.IsUpper(s[0]) || !Note.TryParsePitchClass(s, out var root, out var consumed))
            return false;

        var suffix = s[consumed..];
        Note? bass = null;
        int slash = suffix.LastIndexOf('/');
        if (slash >= 0)
        {
            var tail = suffix[(slash + 1)..];
            if (!tail.IsEmpty && char.IsUpper(tail[0]) && Note.TryParsePitchClass(tail, out var b, out var used) && used == tail.Length)
            {
                bass = b;
                suffix = suffix[..slash];
            }
        }

        if (!Lookup.TryGetValue(Normalize(suffix.ToString()), out var type))
            return false;

        symbol = new ChordSymbol(root, type, bass);
        return true;
    }

    private static string Normalize(string suffix)
    {
        var s = suffix.Replace("(", "").Replace(")", "").Replace(" ", "").Replace('♭', 'b').Replace('♯', '#');
        if (s.StartsWith('-')) s = "m" + s[1..];
        s = s.Replace("min", "m")
             .Replace("Δ7", "maj7").Replace("Δ", "maj7")
             .Replace("ø7", "m7b5").Replace("ø", "m7b5")
             .Replace("°", "dim").Replace("+", "aug")
             .Replace("Maj", "maj").Replace("ma7", "maj7").Replace("ma9", "maj9").Replace("ma13", "maj13");
        if (s is "o" or "o7") s = s.Replace("o", "dim");
        // A capital M followed by a digit means major (M7, mM7); lone "M" is a major triad.
        s = System.Text.RegularExpressions.Regex.Replace(s, "M(?=\\d)", "maj");
        if (s == "M") s = "";
        s = s.Replace("majmaj", "maj").ToLowerInvariant();
        return s;
    }

    /// <summary>
    /// The chord this symbol names. A bass that is a chord tone becomes the matching inversion; a bass
    /// that is not a chord tone (<c>C/Bb</c>) stays on <see cref="Bass"/> only.
    /// </summary>
    public Chord ToChord()
    {
        var chord = new Chord(Root, Type);
        if (Bass is null)
            return chord;
        var index = chord.GetNotes().ToList().FindIndex(n => n.Name == Bass.Name && n.Alteration == Bass.Alteration);
        return index switch
        {
            1 => chord.WithInversion(ChordInversion.First),
            2 => chord.WithInversion(ChordInversion.Second),
            3 => chord.WithInversion(ChordInversion.Third),
            _ => chord,
        };
    }

    /// <summary>Transposes root and bass by an interval, keeping interval spelling (D/F# up a m2 is Eb/G).</summary>
    public ChordSymbol Transpose(Interval interval, Direction direction = Direction.Up) =>
        this with { Root = Root.Transpose(interval, direction), Bass = Bass?.Transpose(interval, direction) };

    /// <summary>Transposes root and bass by semitones and spells them in <paramref name="targetKey"/>.</summary>
    public ChordSymbol Transpose(int semitones, KeySignature targetKey) =>
        this with { Root = Root.TransposeBySemitones(semitones, targetKey), Bass = Bass?.TransposeBySemitones(semitones, targetKey) };

    /// <summary>Moves the chord from one key to another (by tonic distance), spelled in <paramref name="to"/>.</summary>
    public ChordSymbol Transpose(KeySignature from, KeySignature to) =>
        Transpose(MusicTheoryUtilities.GetPitchClass(to.Tonic) - MusicTheoryUtilities.GetPitchClass(from.Tonic), to);

    /// <summary>Renders the symbol in the given style, e.g. <c>Bbmaj7</c>, <c>D/F#</c>, <c>Bø7</c>.</summary>
    public string ToString(ChordSymbolStyle style)
    {
        var suffix = style.UseJazzGlyphs && JazzGlyphs.TryGetValue(Type, out var glyph) ? glyph : Suffixes[Type];
        if (style.Accidentals == AccidentalStyle.Unicode)
            suffix = suffix.Replace('b', '♭').Replace('#', '♯');
        var text = Spell(Root, style.Accidentals) + suffix;
        return Bass is null ? text : $"{text}/{Spell(Bass, style.Accidentals)}";
    }

    /// <summary>Renders the symbol in <see cref="ChordSymbolStyle.Ascii"/>.</summary>
    public override string ToString() => ToString(ChordSymbolStyle.Ascii);

    /// <summary>Equal when root and bass have the same spelling (octave ignored) and the type matches.</summary>
    public bool Equals(ChordSymbol? other) =>
        other is not null && Type == other.Type && SameSpelling(Root, other.Root) &&
        (Bass is null ? other.Bass is null : other.Bass is not null && SameSpelling(Bass, other.Bass));

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(Type, Root.Name, Root.Alteration, Bass?.Name, Bass?.Alteration);

    private static bool SameSpelling(Note a, Note b) => a.Name == b.Name && a.Alteration == b.Alteration;

    /// <summary>Note name without octave, e.g. <c>Bb</c> or <c>B♭</c>.</summary>
    internal static string Spell(Note note, AccidentalStyle style) => note.Name + (style, note.Alteration) switch
    {
        (_, Alteration.Natural) => "",
        (AccidentalStyle.Ascii, Alteration.Flat) => "b",
        (AccidentalStyle.Ascii, Alteration.DoubleFlat) => "bb",
        (AccidentalStyle.Ascii, Alteration.Sharp) => "#",
        (AccidentalStyle.Ascii, Alteration.DoubleSharp) => "##",
        (_, Alteration.Flat) => "♭",
        (_, Alteration.DoubleFlat) => "𝄫",
        (_, Alteration.Sharp) => "♯",
        _ => "𝄪",
    };
}
