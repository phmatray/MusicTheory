namespace MusicTheory.Guitar;

/// <summary>Spoken names and accessible labels for chords and voicings.</summary>
public static class VoicingDescriber
{
    /// <summary>Screen-reader label, e.g. <c>A minor, x-0-2-2-1-0</c>.</summary>
    public static string AccessibleLabel(GuitarVoicing v) =>
        $"{ChordName(v.Chord)}, {string.Join("-", v.Frets.Select(f => f?.ToString() ?? "x"))}";

    /// <summary>Spoken chord name, e.g. <c>A minor</c>, <c>D major over F sharp</c>, <c>B flat major seven</c>.</summary>
    public static string ChordName(ChordSymbol c)
    {
        var name = $"{NoteName(c.Root)} {TypeName(c.Type)}";
        return c.Bass is null ? name : $"{name} over {NoteName(c.Bass)}";
    }

    private static string NoteName(Note n) => n.Name + n.Alteration switch
    {
        Alteration.Flat => " flat",
        Alteration.Sharp => " sharp",
        Alteration.DoubleFlat => " double flat",
        Alteration.DoubleSharp => " double sharp",
        _ => "",
    };

    private static string TypeName(ChordType t) => t switch
    {
        ChordType.Major => "major",
        ChordType.Minor => "minor",
        ChordType.Diminished => "diminished",
        ChordType.Augmented => "augmented",
        ChordType.Major7 => "major seven",
        ChordType.Minor7 => "minor seven",
        ChordType.Dominant7 => "seven",
        ChordType.MinorMajor7 => "minor major seven",
        ChordType.HalfDiminished7 => "half-diminished seven",
        ChordType.Diminished7 => "diminished seven",
        ChordType.Augmented7 => "augmented seven",
        ChordType.AugmentedMajor7 => "augmented major seven",
        ChordType.Sus2 => "suspended two",
        ChordType.Sus4 => "suspended four",
        ChordType.Sus2Sus4 => "suspended two four",
        ChordType.Dominant7Sus4 => "seven suspended four",
        ChordType.Major6 => "six",
        ChordType.Minor6 => "minor six",
        ChordType.Major6Add9 => "six nine",
        ChordType.Major9 => "major nine",
        ChordType.Minor9 => "minor nine",
        ChordType.Dominant9 => "nine",
        ChordType.Major11 => "major eleven",
        ChordType.Minor11 => "minor eleven",
        ChordType.Dominant11 => "eleven",
        ChordType.Major13 => "major thirteen",
        ChordType.Minor13 => "minor thirteen",
        ChordType.Dominant13 => "thirteen",
        ChordType.MajorAdd9 => "add nine",
        ChordType.MinorAdd9 => "minor add nine",
        ChordType.MajorAdd11 => "add eleven",
        ChordType.MinorAdd11 => "minor add eleven",
        ChordType.Dominant7Flat5 => "seven flat five",
        ChordType.Dominant7Sharp5 => "seven sharp five",
        ChordType.Dominant7Flat9 => "seven flat nine",
        ChordType.Dominant7Sharp9 => "seven sharp nine",
        ChordType.Dominant7Flat5Flat9 => "seven flat five flat nine",
        ChordType.Dominant7Flat5Sharp9 => "seven flat five sharp nine",
        ChordType.Dominant7Sharp5Flat9 => "seven sharp five flat nine",
        ChordType.Dominant7Sharp5Sharp9 => "seven sharp five sharp nine",
        ChordType.Dominant7Alt => "seven altered",
        ChordType.Power5 => "five",
        _ => t.ToString(),
    };
}
