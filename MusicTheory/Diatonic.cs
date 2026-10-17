namespace MusicTheory;

/// <summary>Triad family of a chord, as used by key detection and Roman-numeral analysis.</summary>
internal enum Triad { Major, Minor, Diminished, Augmented, Any }

/// <summary>Diatonic degree tables shared by <see cref="KeyDetector"/> and <see cref="HarmonicAnalyzer"/>.</summary>
internal static class Diatonic
{
    private static readonly (int Step, Triad Triad)[] Major =
        [(0, Triad.Major), (2, Triad.Minor), (4, Triad.Minor), (5, Triad.Major), (7, Triad.Major), (9, Triad.Minor), (11, Triad.Diminished)];

    // Natural minor, then the harmonic-minor V and vii° (degrees 5 and 7 again).
    private static readonly (int Step, Triad Triad)[] Minor =
        [(0, Triad.Minor), (2, Triad.Diminished), (3, Triad.Major), (5, Triad.Minor), (7, Triad.Minor), (8, Triad.Major), (10, Triad.Major),
         (7, Triad.Major), (11, Triad.Diminished)];

    /// <summary>Semitone step and triad per degree (index = degree − 1; minor adds V and vii° at 7 and 8).</summary>
    public static (int Step, Triad Triad)[] Degrees(KeyMode mode) => mode == KeyMode.Major ? Major : Minor;

    /// <summary>The scale degree (1..7) of an index into <see cref="Degrees"/>.</summary>
    public static int DegreeOf(int index) => index switch { < 7 => index + 1, 7 => 5, _ => 7 };

    public static Triad TriadOf(ChordType type) => type switch
    {
        ChordType.Minor or ChordType.Minor7 or ChordType.MinorMajor7 or ChordType.Minor6 or ChordType.Minor9
            or ChordType.Minor11 or ChordType.Minor13 or ChordType.MinorAdd9 or ChordType.MinorAdd11 => Triad.Minor,
        ChordType.Diminished or ChordType.HalfDiminished7 or ChordType.Diminished7 => Triad.Diminished,
        ChordType.Augmented or ChordType.Augmented7 or ChordType.AugmentedMajor7 => Triad.Augmented,
        ChordType.Sus2 or ChordType.Sus4 or ChordType.Sus2Sus4 or ChordType.Dominant7Sus4 or ChordType.Power5 => Triad.Any,
        _ => Triad.Major,
    };
}
