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
/// A lead-sheet chord symbol such as <c>Am7</c>, <c>D/F#</c>, <c>B♭Δ7</c> or <c>C7b9#11</c>: a root, a chord
/// type, optional tensions and an optional bass that need not be a chord tone (<c>C/Bb</c>).
/// </summary>
/// <param name="Root">The chord root (octave is not significant).</param>
/// <param name="Type">The chord type.</param>
/// <param name="Bass">The slash bass, or null when the root is in the bass.</param>
/// <param name="Tensions">Tensions on top of the type, in order: altered (<c>b9</c>, <c>#11</c>, <c>b13</c>,
/// <c>b5</c>, <c>#5</c>) replace the natural degree, natural ones (<c>add9</c>, <c>13</c>) are added.</param>
public sealed record ChordSymbol(Note Root, ChordType Type, Note? Bass = null, IReadOnlyList<Interval>? Tensions = null)
{
    /// <summary>The root.</summary>
    public Note Root { get; init => field = value ?? throw new ArgumentNullException(nameof(Root)); } = Root ?? throw new ArgumentNullException(nameof(Root));

    /// <summary>The chord type.</summary>
    public ChordType Type { get; init => field = ChordTypes.Checked(value, nameof(Type)); } = ChordTypes.Checked(Type, nameof(Type));

    /// <summary>The tensions, never null (empty when there are none).</summary>
    public IReadOnlyList<Interval> Tensions { get; init => field = CheckTensions(value); } = CheckTensions(Tensions);

    // Tension text (ASCII, lower case) → interval. Altered tensions are written inline, natural ones with "add".
    private static readonly Dictionary<string, Interval> TensionNames = new()
    {
        ["b5"] = Interval.Parse("d5"), ["#5"] = Interval.Parse("A5"),
        ["b9"] = Interval.Parse("m9"), ["9"] = Interval.Parse("M9"), ["#9"] = Interval.Parse("A9"),
        ["11"] = Interval.Parse("P11"), ["#11"] = Interval.Parse("A11"),
        ["b13"] = Interval.Parse("m13"), ["13"] = Interval.Parse("M13"),
    };

    /// <summary>True for the intervals a symbol can carry as tensions: b5 #5 b9 9 #9 11 #11 b13 13.</summary>
    internal static bool IsTension(Interval interval) => TensionNames.ContainsValue(interval);

    private static IReadOnlyList<Interval> CheckTensions(IReadOnlyList<Interval>? tensions)
    {
        var list = (tensions ?? []).ToArray();
        foreach (var t in list)
            if (t is null || !IsTension(t))
                throw new ArgumentException($"Not a chord tension: {t?.ToString() ?? "null"}. Use b5, #5, b9, 9, #9, 11, #11, b13 or 13.", nameof(Tensions));
        return list;
    }

    private static readonly Dictionary<ChordType, string> JazzGlyphs = new()
    {
        [ChordType.Diminished] = "°", [ChordType.Diminished7] = "°7", [ChordType.HalfDiminished7] = "ø7",
        [ChordType.Augmented] = "+", [ChordType.Augmented7] = "+7", [ChordType.AugmentedMajor7] = "+maj7",
    };

    // Normalised (lower-case, ASCII, no parentheses) suffix → type.
    private static readonly Dictionary<string, ChordType> Lookup = BuildLookup();

    private static Dictionary<string, ChordType> BuildLookup()
    {
        var lookup = ChordTypes.All.ToDictionary(t => ChordTypes.Suffix(t).ToLowerInvariant());
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

        var normalized = Normalize(suffix.ToString());
        // The longest known suffix that leaves only tensions behind: "7b9#11" is 7b9 plus #11.
        for (int length = normalized.Length; length >= 0; length--)
        {
            if (Lookup.TryGetValue(normalized[..length], out var type) && TryParseTensions(normalized[length..], out var tensions))
            {
                symbol = new ChordSymbol(root, type, bass, tensions);
                return true;
            }
        }
        return false;
    }

    private static bool TryParseTensions(string text, out List<Interval> tensions)
    {
        tensions = [];
        int i = 0;
        while (i < text.Length)
        {
            bool add = text.AsSpan(i).StartsWith("add");
            if (add)
                i += 3;
            var match = TensionNames.Keys
                .Where(k => text.AsSpan(i).StartsWith(k))
                .MaxBy(k => k.Length);
            // A bare 5 is not a tension.
            if (match is null)
                return false;
            tensions.Add(TensionNames[match]);
            i += match.Length;
        }
        return true;
    }

    private static string Normalize(string suffix)
    {
        var s = suffix.Replace("(", "").Replace(")", "").Replace(" ", "").Replace(",", "").Replace('♭', 'b').Replace('♯', '#');
        if (s.StartsWith('-')) s = "m" + s[1..];
        s = s.Replace("min", "m")
             .Replace("Δ7", "maj7").Replace("Δ", "maj7")
             .Replace("ø7", "m7b5").Replace("ø", "m7b5")
             .Replace("°", "dim").Replace("+", "aug")
             .Replace("Maj", "maj").Replace("ma7", "maj7").Replace("ma9", "maj9").Replace("ma13", "maj13");
        if (s.StartsWith("mi")) s = "m" + s[2..];
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
        var chord = Tensions.Aggregate(new Chord(Root, Type), (c, t) => c.AddExtension(t.Number, t.Quality));
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
    public ChordSymbol Transpose(Interval interval) =>
        this with { Root = Root.Transpose(interval), Bass = Bass?.Transpose(interval) };

    /// <summary>
    /// Transposes root and bass by semitones and spells them in <paramref name="targetKey"/>. A black-key note
    /// outside the key keeps the direction of its own accidental (a flat Eb moved to G is Bb, not A#).
    /// </summary>
    public ChordSymbol Transpose(int semitones, KeySignature targetKey) =>
        this with { Root = BySemitones(Root, semitones, targetKey), Bass = Bass is null ? null : BySemitones(Bass, semitones, targetKey) };

    /// <summary>
    /// Moves the chord from one key to another by the interval between the tonics, so a borrowed Eb in C is Bb in G.
    /// It falls back to the spelling of <paramref name="to"/> only to avoid a double accidental.
    /// </summary>
    public ChordSymbol Transpose(KeySignature from, KeySignature to)
    {
        int semitones = to.Tonic.PitchClass - from.Tonic.PitchClass;
        int letters = ((int)to.Tonic.Name - (int)from.Tonic.Name + 7) % 7;
        return this with { Root = ByInterval(Root, semitones, letters, to), Bass = Bass is null ? null : ByInterval(Bass, semitones, letters, to) };
    }

    private static Note ByInterval(Note n, int semitones, int letters, KeySignature to)
    {
        var spelled = n.TransposeBySemitones(semitones).Respell((NoteName)(((int)n.Name + letters) % 7));
        return spelled is { Alteration: Alteration.Flat or Alteration.Natural or Alteration.Sharp }
            ? spelled
            : n.TransposeBySemitones(semitones, to);
    }

    private static Note BySemitones(Note n, int semitones, KeySignature key)
    {
        var spelled = n.TransposeBySemitones(semitones, key);
        if (spelled.Alteration == Alteration.Natural || n.Alteration == Alteration.Natural
            || key.GetAlteration(spelled.Name) == spelled.Alteration
            || Math.Sign((int)n.Alteration) == Math.Sign((int)spelled.Alteration))
            return spelled;
        // A flat source takes the letter above, a sharp source the letter below.
        int step = n.Alteration == Alteration.Flat ? 1 : 6;
        return spelled.Respell((NoteName)(((int)spelled.Name + step) % 7))!;
    }

    /// <summary>
    /// Piano voicing: the chord in root position from <paramref name="octave"/> (interval spelling, so the #9 of
    /// E7#9 is F##), ascending, with a slash bass placed below the root.
    /// </summary>
    public IReadOnlyList<Note> Voice(int octave = 4)
    {
        var notes = (this with { Root = new Note(Root.Name, Root.Alteration, octave), Bass = null }).ToChord().GetNotes()
            .OrderBy(MusicTheoryUtilities.GetTotalSemitones).ToList();
        if (Bass is null)
            return notes;
        int root = MusicTheoryUtilities.GetTotalSemitones(notes[0]);
        int bassOctave = octave + 1;
        while (MusicTheoryUtilities.GetTotalSemitones(Bass.Name, Bass.Alteration, bassOctave) >= root)
            bassOctave--;
        return [new Note(Bass.Name, Bass.Alteration, bassOctave), .. notes];
    }

    /// <summary>Renders the symbol in the given style, e.g. <c>Bbmaj7</c>, <c>D/F#</c>, <c>Bø7</c>.</summary>
    public string ToString(ChordSymbolStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);
        var suffix = (style.UseJazzGlyphs && JazzGlyphs.TryGetValue(Type, out var glyph) ? glyph : ChordTypes.Suffix(Type))
            + string.Concat(Tensions.Select((t, i) => TensionText(t, i == 0 && ChordTypes.Suffix(Type).Length == 0)));
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
        (Bass is null ? other.Bass is null : other.Bass is not null && SameSpelling(Bass, other.Bass)) &&
        Tensions.SequenceEqual(other.Tensions);

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(Type, Root.Name, Root.Alteration, Bass?.Name, Bass?.Alteration, Tensions.Count);

    private static string TensionText(Interval tension, bool firstOnPlainMajor)
    {
        var name = TensionNames.First(kv => kv.Value == tension).Key;
        // "C#11" would re-parse as a different chord, so an altered first tension on a bare triad keeps "add".
        return char.IsAsciiDigit(name[0]) || firstOnPlainMajor ? "add" + name : name;
    }

    private static bool SameSpelling(Note a, Note b) => a.Name == b.Name && a.Alteration == b.Alteration;

    /// <summary>Note name without octave, e.g. <c>Bb</c> or <c>B♭</c>.</summary>
    internal static string Spell(Note note, AccidentalStyle style) =>
        note.Name + (style == AccidentalStyle.Unicode ? UnicodeAccidentals : AsciiAccidentals)[(int)note.Alteration + 2];

    private static readonly string[] AsciiAccidentals = ["bb", "b", "", "#", "##"];
    private static readonly string[] UnicodeAccidentals = ["𝄫", "♭", "", "♯", "𝄪"];
}
