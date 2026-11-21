using System.Globalization;
using System.Text.RegularExpressions;
using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Finder;

/// <summary>One identified chord: its symbol (root spelled for the language), the type name and how many notes it leaves over.</summary>
public sealed record FinderRow(string Symbol, string TypeLabel, int ExtraNotes);

/// <summary>A scale that fits a chord; <paramref name="Id"/> is the <see cref="ScaleType"/> name used in the URL.</summary>
public sealed record FinderScale(string Id, string Label, string Notes);

/// <summary>The scales for a typed chord, or the library's message when it did not parse.</summary>
public sealed record FinderScales(IReadOnlyList<FinderScale> Scales, string? Error, Note? Root);

/// <summary>One mode of a scale (index 0 is the scale itself).</summary>
public sealed record FinderMode(int Degree, string Label, string Notes);

public static class FinderModel
{
    /// <summary>The pitch classes in a <c>pcs</c> value: 0..11 only, sorted, de-duplicated; anything else is ignored.</summary>
    public static IReadOnlyList<int> ParsePitchClasses(string? text) =>
        (text ?? "").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out var n) ? n : -1)
            .Where(n => n is >= 0 and <= 11).Distinct().Order().ToList();

    /// <summary>The <c>pcs</c> value for a set, or null when it is empty (so the key leaves the URL).</summary>
    public static string? FormatPitchClasses(IEnumerable<int> pcs)
    {
        var list = pcs.Distinct().Order().ToList();
        return list.Count == 0 ? null : string.Join(',', list);
    }

    /// <summary>The set with <paramref name="pc"/> switched on or off.</summary>
    public static IReadOnlyList<int> Toggle(IReadOnlyList<int> pcs, int pc) =>
        (pcs.Contains(pc) ? pcs.Where(p => p != pc) : pcs.Append(pc)).Order().ToList();

    /// <summary>Chords matching the set, best first (the library's order). Empty for an empty set.</summary>
    public static IReadOnlyList<FinderRow> Identify(IReadOnlyList<int> pitchClasses, Lang lang) =>
        pitchClasses.Count == 0
            ? []
            : ChordFinder.IdentifyFromPitchClasses(pitchClasses)
                .Select(m => new FinderRow(Spell(m.Symbol, m.Root, lang), TypeNames.Chord(m.Type, lang), m.ExtraNotes)).ToList();

    /// <summary>The scales on the chord's root that hold every chord tone; <c>Error</c> carries the library's message for a bad symbol.</summary>
    public static FinderScales ScalesFor(string? chordText, Lang lang)
    {
        try
        {
            var chord = ChordSymbol.Parse(chordText ?? "");
            var scales = ChordScale.ScalesFor(chord)
                .Select(s => new FinderScale(s.Type.ToString(), ScaleLabel(s, lang), Notes(s.GetNotes().SkipLast(1), lang))).ToList();
            return new FinderScales(scales, null, chord.Root);
        }
        catch (FormatException e)
        {
            return new FinderScales([], e.Message, null);
        }
    }

    /// <summary>The scale's modes; empty when its type has no mode family.</summary>
    public static IReadOnlyList<FinderMode> Modes(Scale scale, Lang lang)
    {
        try
        {
            return scale.GetModes().Select((m, i) => new FinderMode(i + 1, ScaleLabel(m, lang), Notes(m.GetNotes().SkipLast(1), lang))).ToList();
        }
        catch (InvalidOperationException)
        {
            return [];
        }
    }

    /// <summary>The scale of <paramref name="type"/> (a <see cref="ScaleType"/> name) on <paramref name="root"/>, or null.</summary>
    public static Scale? ScaleOf(Note root, string? type) =>
        Enum.TryParse<ScaleType>(type, out var t) && Enum.IsDefined(t) ? new Scale(root, t) : null;

    /// <summary>The scale's name for display, for example <c>Do mixolydien</c>.</summary>
    public static string ScaleLabel(Scale s, Lang lang) => NoteNames.Format(s.Root, lang) + " " + TypeNames.Scale(s.Type, lang);

    private static string Notes(IEnumerable<Note> notes, Lang lang) => string.Join(" ", notes.Select(n => NoteNames.Format(n, lang)));

    // "Cmaj7" -> "Do maj7": the library's ASCII suffix after a root spelled for the language.
    private static string Spell(string symbol, Note root, Lang lang) =>
        NoteNames.Format(root, lang) + symbol[root.GetName().Length..];
}

/// <summary>Display names of chord and scale types; French is a table, English splits the enum name.</summary>
public static class TypeNames
{
    public static string Chord(ChordType t, Lang lang) =>
        lang == Lang.Fr && ChordFr.TryGetValue(t, out var fr) ? fr : Humanize(t.ToString());

    public static string Scale(ScaleType t, Lang lang) =>
        lang == Lang.Fr && ScaleFr.TryGetValue(t, out var fr) ? fr : Humanize(t.ToString());

    private static string Humanize(string name) => Regex.Replace(name, "(?<=[a-z0-9])(?=[A-Z])", " ").ToLowerInvariant();

    // ponytail: only the common types are translated, the rest fall back to the English name; extend when a teacher asks.
    private static readonly Dictionary<ChordType, string> ChordFr = new()
    {
        [ChordType.Major] = "majeur", [ChordType.Minor] = "mineur", [ChordType.Diminished] = "diminué", [ChordType.Augmented] = "augmenté",
        [ChordType.Major7] = "septième majeure", [ChordType.Minor7] = "mineur septième", [ChordType.Dominant7] = "septième de dominante",
        [ChordType.MinorMajor7] = "mineur septième majeure", [ChordType.HalfDiminished7] = "demi-diminué", [ChordType.Diminished7] = "septième diminuée",
        [ChordType.Sus2] = "suspendu 2", [ChordType.Sus4] = "suspendu 4", [ChordType.Major6] = "sixte majeure", [ChordType.Minor6] = "mineur sixte",
        [ChordType.Major9] = "neuvième majeure", [ChordType.Minor9] = "mineur neuvième", [ChordType.Dominant9] = "neuvième de dominante",
        [ChordType.Power5] = "quinte", [ChordType.Augmented7] = "septième augmentée",
    };

    private static readonly Dictionary<ScaleType, string> ScaleFr = new()
    {
        [ScaleType.Major] = "majeur", [ScaleType.Ionian] = "ionien", [ScaleType.NaturalMinor] = "mineur naturel", [ScaleType.Aeolian] = "éolien",
        [ScaleType.HarmonicMinor] = "mineur harmonique", [ScaleType.MelodicMinor] = "mineur mélodique", [ScaleType.Dorian] = "dorien",
        [ScaleType.Phrygian] = "phrygien", [ScaleType.Lydian] = "lydien", [ScaleType.Mixolydian] = "mixolydien", [ScaleType.Locrian] = "locrien",
        [ScaleType.Chromatic] = "chromatique", [ScaleType.WholeTone] = "par tons", [ScaleType.Blues] = "blues",
        [ScaleType.PentatonicMajor] = "pentatonique majeure", [ScaleType.PentatonicMinor] = "pentatonique mineure",
        [ScaleType.LydianDominant] = "lydien dominant", [ScaleType.Altered] = "altéré", [ScaleType.PhrygianDominant] = "phrygien dominant",
    };
}
