using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Drills;

/// <summary>FR and EN labels of the chord types, scale types and degrees the drills ask. A test enumerates the library pools.</summary>
public static class TypeNames
{
    private static readonly Dictionary<ChordType, (string En, string Fr)> Chords = new()
    {
        [ChordType.Major] = ("major", "majeur"),
        [ChordType.Minor] = ("minor", "mineur"),
        [ChordType.Diminished] = ("diminished", "diminué"),
        [ChordType.Augmented] = ("augmented", "augmenté"),
        [ChordType.Dominant7] = ("dominant seventh", "septième de dominante"),
        [ChordType.Major7] = ("major seventh", "septième majeure"),
        [ChordType.Minor7] = ("minor seventh", "septième mineure"),
        [ChordType.HalfDiminished7] = ("half-diminished seventh", "septième semi-diminuée"),
        [ChordType.Diminished7] = ("diminished seventh", "septième diminuée"),
    };

    private static readonly Dictionary<ScaleType, (string En, string Fr)> Scales = new()
    {
        [ScaleType.Major] = ("major", "majeure"),
        [ScaleType.NaturalMinor] = ("natural minor", "mineure naturelle"),
        [ScaleType.HarmonicMinor] = ("harmonic minor", "mineure harmonique"),
        [ScaleType.MelodicMinor] = ("melodic minor", "mineure mélodique"),
        [ScaleType.PentatonicMajor] = ("major pentatonic", "pentatonique majeure"),
        [ScaleType.PentatonicMinor] = ("minor pentatonic", "pentatonique mineure"),
        [ScaleType.Dorian] = ("Dorian", "dorien"),
        [ScaleType.Phrygian] = ("Phrygian", "phrygien"),
        [ScaleType.Lydian] = ("Lydian", "lydien"),
        [ScaleType.Mixolydian] = ("Mixolydian", "mixolydien"),
        [ScaleType.Locrian] = ("Locrian", "locrien"),
    };

    /// <summary>The label, or the enum name when the type has none (a test keeps that from happening for the pools).</summary>
    public static string Label(ChordType type, Lang lang) =>
        Chords.TryGetValue(type, out var n) ? (lang == Lang.Fr ? n.Fr : n.En) : type.ToString();

    public static string Label(ScaleType type, Lang lang) =>
        Scales.TryGetValue(type, out var n) ? (lang == Lang.Fr ? n.Fr : n.En) : type.ToString();

    /// <summary>The Roman numeral of a degree in a major or minor key (<c>V</c>, <c>ii°</c>); the same in both languages.</summary>
    public static string Degree(int degree, bool minor, Lang lang) =>
        new ChordProgression(new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), minor ? KeyMode.Minor : KeyMode.Major))
            .GetRomanNumeral(degree);

    /// <summary>The key as spoken: <c>C major</c>, <c>do mineur</c>.</summary>
    public static string Key(KeySignature key, Lang lang) =>
        NoteNames.Format(key.Tonic, lang) + (lang == Lang.Fr
            ? key.Mode == KeyMode.Minor ? " mineur" : " majeur"
            : key.Mode == KeyMode.Minor ? " minor" : " major");
}
