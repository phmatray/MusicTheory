using System.Globalization;

namespace MusicTheory.Guitar;

/// <summary>Spoken names and accessible labels for chords and voicings.</summary>
public static class VoicingDescriber
{
    /// <summary>Screen-reader label, e.g. <c>A minor, x-0-2-2-1-0</c> (<c>la mineur, x-0-2-2-1-0</c> in French).</summary>
    /// <param name="v">The voicing.</param>
    /// <param name="culture">A French culture speaks French; null or any other culture speaks English.</param>
    /// <exception cref="ArgumentNullException"><paramref name="v"/> is null.</exception>
    public static string AccessibleLabel(GuitarVoicing v, CultureInfo? culture = null) => v is null ? throw new ArgumentNullException(nameof(v)) :
        $"{ChordName(v.Chord, culture)}, {string.Join("-", v.Frets.Select(f => f?.ToString() ?? "x"))}";

    /// <summary>Spoken chord name, e.g. <c>A minor</c>, <c>D major over F sharp</c>, <c>B flat major seven</c>; <c>la mineur</c> in French.</summary>
    /// <param name="c">The chord.</param>
    /// <param name="culture">A French culture speaks French (solfège note names); null or any other culture speaks English.</param>
    /// <exception cref="ArgumentNullException"><paramref name="c"/> is null.</exception>
    public static string ChordName(ChordSymbol c, CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(c);
        var french = culture?.TwoLetterISOLanguageName == "fr";
        var name = $"{NoteName(c.Root, french)} {TypeName(c.Type, french)}";
        return c.Bass is null ? name : $"{name} {(french ? "sur" : "over")} {NoteName(c.Bass, french)}";
    }

    private static string NoteName(Note n, bool french) => french
        ? Solfege.FixedDo(n).ToLowerInvariant()
        : n.Name + new[] { " double flat", " flat", "", " sharp", " double sharp" }[(int)n.Alteration + 2];

    private static string TypeName(ChordType t, bool french) => french ? TypeNames()[t].Fr : TypeNames()[t].En;

    // Every ChordType, spoken in English and French; GuitarValueTests checks each one. Built per call: a describer is not hot.
    private static Dictionary<ChordType, (string En, string Fr)> TypeNames() => new()
    {
        [ChordType.Major] = ("major", "majeur"),
        [ChordType.Minor] = ("minor", "mineur"),
        [ChordType.Diminished] = ("diminished", "diminué"),
        [ChordType.Augmented] = ("augmented", "augmenté"),
        [ChordType.Major7] = ("major seven", "sept majeur"),
        [ChordType.Minor7] = ("minor seven", "mineur sept"),
        [ChordType.Dominant7] = ("seven", "sept"),
        [ChordType.MinorMajor7] = ("minor major seven", "mineur sept majeur"),
        [ChordType.HalfDiminished7] = ("half-diminished seven", "demi-diminué sept"),
        [ChordType.Diminished7] = ("diminished seven", "diminué sept"),
        [ChordType.Augmented7] = ("augmented seven", "augmenté sept"),
        [ChordType.AugmentedMajor7] = ("augmented major seven", "augmenté sept majeur"),
        [ChordType.Sus2] = ("suspended two", "suspendu deux"),
        [ChordType.Sus4] = ("suspended four", "suspendu quatre"),
        [ChordType.Sus2Sus4] = ("suspended two four", "suspendu deux quatre"),
        [ChordType.Dominant7Sus4] = ("seven suspended four", "sept suspendu quatre"),
        [ChordType.Major6] = ("six", "six"),
        [ChordType.Minor6] = ("minor six", "mineur six"),
        [ChordType.Major6Add9] = ("six nine", "six neuf"),
        [ChordType.Major9] = ("major nine", "neuf majeur"),
        [ChordType.Minor9] = ("minor nine", "mineur neuf"),
        [ChordType.Dominant9] = ("nine", "neuf"),
        [ChordType.Major11] = ("major eleven", "onze majeur"),
        [ChordType.Minor11] = ("minor eleven", "mineur onze"),
        [ChordType.Dominant11] = ("eleven", "onze"),
        [ChordType.Major13] = ("major thirteen", "treize majeur"),
        [ChordType.Minor13] = ("minor thirteen", "mineur treize"),
        [ChordType.Dominant13] = ("thirteen", "treize"),
        [ChordType.MajorAdd9] = ("add nine", "ajout neuf"),
        [ChordType.MinorAdd9] = ("minor add nine", "mineur ajout neuf"),
        [ChordType.MajorAdd11] = ("add eleven", "ajout onze"),
        [ChordType.MinorAdd11] = ("minor add eleven", "mineur ajout onze"),
        [ChordType.Dominant7Flat5] = ("seven flat five", "sept bémol cinq"),
        [ChordType.Dominant7Sharp5] = ("seven sharp five", "sept dièse cinq"),
        [ChordType.Dominant7Flat9] = ("seven flat nine", "sept bémol neuf"),
        [ChordType.Dominant7Sharp9] = ("seven sharp nine", "sept dièse neuf"),
        [ChordType.Dominant7Flat5Flat9] = ("seven flat five flat nine", "sept bémol cinq bémol neuf"),
        [ChordType.Dominant7Flat5Sharp9] = ("seven flat five sharp nine", "sept bémol cinq dièse neuf"),
        [ChordType.Dominant7Sharp5Flat9] = ("seven sharp five flat nine", "sept dièse cinq bémol neuf"),
        [ChordType.Dominant7Sharp5Sharp9] = ("seven sharp five sharp nine", "sept dièse cinq dièse neuf"),
        [ChordType.Dominant7Alt] = ("seven altered", "sept altéré"),
        [ChordType.Power5] = ("five", "cinq"),
        [ChordType.Dominant7Sus2] = ("seven suspended two", "sept suspendu deux"),
        [ChordType.MinorMajor9] = ("minor major nine", "mineur neuf majeur"),
    };
}
