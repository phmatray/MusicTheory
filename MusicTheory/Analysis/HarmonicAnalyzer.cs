namespace MusicTheory.Analysis;

/// <summary>
/// Provides harmonic analysis capabilities for chords and progressions.
/// </summary>
public static class HarmonicAnalyzer
{
    // Major scale intervals in semitones from root
    private static readonly int[] MajorScaleIntervals = { 0, 2, 4, 5, 7, 9, 11 };

    // Natural minor scale intervals in semitones from root
    private static readonly int[] MinorScaleIntervals = { 0, 2, 3, 5, 7, 8, 10 };

    // Roman numerals for major keys (uppercase = major, lowercase = minor)
    private static readonly string[] MajorKeyRomanNumerals = { "I", "ii", "iii", "IV", "V", "vi", "vii°" };

    // Roman numerals for minor keys
    private static readonly string[] MinorKeyRomanNumerals = { "i", "ii°", "III", "iv", "v", "VI", "VII" };

    /// <summary>
    /// Determines the harmonic function of a chord within a key.
    /// </summary>
    /// <param name="chord">The chord to analyze.</param>
    /// <param name="key">The key context.</param>
    /// <returns>The harmonic function of the chord.</returns>
    public static HarmonicFunction DetermineFunction(Chord chord, KeySignature key)
    {
        var degree = GetScaleDegree(chord.Root, key);

        if (degree == null)
        {
            // Non-diatonic chord - could be modal interchange or chromatic
            return HarmonicFunction.ModalInterchange;
        }

        return degree.Value switch
        {
            1 => HarmonicFunction.Tonic,      // I
            2 => HarmonicFunction.Subdominant, // ii
            3 => HarmonicFunction.Tonic,      // iii (tonic substitute)
            4 => HarmonicFunction.Subdominant, // IV
            5 => HarmonicFunction.Dominant,   // V
            6 => HarmonicFunction.Tonic,      // vi (tonic substitute)
            7 => HarmonicFunction.Dominant,   // vii° (dominant substitute)
            _ => HarmonicFunction.Passing
        };
    }

    /// <summary>
    /// Gets the Roman numeral representation of a chord in a key.
    /// </summary>
    /// <param name="chord">The chord.</param>
    /// <param name="key">The key context.</param>
    /// <returns>The Roman numeral string.</returns>
    public static string GetRomanNumeral(Chord chord, KeySignature key)
    {
        var degree = GetScaleDegree(chord.Root, key);

        if (degree == null)
        {
            // Non-diatonic - use flat/sharp prefix
            return GetNonDiatonicRomanNumeral(chord, key);
        }

        var numerals = key.Mode == KeyMode.Major ? MajorKeyRomanNumerals : MinorKeyRomanNumerals;
        var baseNumeral = numerals[degree.Value - 1];

        // Adjust for chord quality if different from expected
        baseNumeral = AdjustNumeralForQuality(baseNumeral, chord, degree.Value, key);

        // Add seventh indicator if applicable
        if (IsSeventhChord(chord.Type))
        {
            baseNumeral += "7";
        }

        return baseNumeral;
    }

    /// <summary>
    /// Analyzes a chord progression and returns analyzed chords.
    /// </summary>
    /// <param name="chords">The chord progression.</param>
    /// <param name="key">The key context.</param>
    /// <returns>Analyzed chords with harmonic information.</returns>
    public static IEnumerable<AnalyzedChord> Analyze(IEnumerable<Chord> chords, KeySignature key)
    {
        foreach (var chord in chords)
        {
            var degree = GetScaleDegree(chord.Root, key) ?? 0;
            var romanNumeral = GetRomanNumeral(chord, key);
            var function = DetermineFunction(chord, key);
            var isNonDiatonic = degree == 0;

            yield return new AnalyzedChord(chord, degree, romanNumeral, function, key, isNonDiatonic);
        }
    }

    /// <summary>
    /// Detects the cadence type between two chords.
    /// </summary>
    /// <param name="penultimate">The second-to-last chord.</param>
    /// <param name="final">The final chord.</param>
    /// <param name="key">The key context.</param>
    /// <returns>The cadence type, or null if no recognized cadence.</returns>
    public static CadenceType? DetectCadence(Chord penultimate, Chord final, KeySignature key)
    {
        var penultimateDegree = GetScaleDegree(penultimate.Root, key);
        var finalDegree = GetScaleDegree(final.Root, key);

        if (penultimateDegree == null || finalDegree == null)
            return null;

        // Authentic cadence: V → I
        if (penultimateDegree == 5 && finalDegree == 1)
        {
            // For now, treat all V-I as perfect authentic
            // (Full implementation would check inversions and soprano note)
            return CadenceType.AuthenticPerfect;
        }

        // Plagal cadence: IV → I
        if (penultimateDegree == 4 && finalDegree == 1)
        {
            return CadenceType.Plagal;
        }

        // Deceptive cadence: V → vi
        if (penultimateDegree == 5 && finalDegree == 6)
        {
            return CadenceType.Deceptive;
        }

        // Half cadence: anything → V
        if (finalDegree == 5)
        {
            return CadenceType.Half;
        }

        return null;
    }

    /// <summary>
    /// Determines if a chord is a secondary dominant of another chord.
    /// </summary>
    /// <param name="chord">The potential secondary dominant.</param>
    /// <param name="target">The target chord.</param>
    /// <returns>True if chord is V of target.</returns>
    public static bool IsSecondaryDominant(Chord chord, Chord target)
    {
        // A secondary dominant is a major or dominant 7th chord
        // whose root is a perfect fifth above the target root
        if (chord.Quality != ChordQuality.Major && chord.Type != ChordType.Dominant7)
            return false;

        var chordPitchClass = GetPitchClass(chord.Root);
        var targetPitchClass = GetPitchClass(target.Root);

        // Check if chord root is 7 semitones (P5) above target root
        var expectedPitchClass = (targetPitchClass + 7) % 12;
        return chordPitchClass == expectedPitchClass;
    }

    private static int? GetScaleDegree(Note root, KeySignature key)
    {
        var rootPitchClass = GetPitchClass(root);
        var tonicPitchClass = GetPitchClass(key.Tonic);
        var intervals = key.Mode == KeyMode.Major ? MajorScaleIntervals : MinorScaleIntervals;

        for (int i = 0; i < intervals.Length; i++)
        {
            var expectedPitchClass = (tonicPitchClass + intervals[i]) % 12;
            if (rootPitchClass == expectedPitchClass)
            {
                return i + 1; // Scale degrees are 1-indexed
            }
        }

        return null; // Non-diatonic
    }

    private static string GetNonDiatonicRomanNumeral(Chord chord, KeySignature key)
    {
        var rootPitchClass = GetPitchClass(chord.Root);
        var tonicPitchClass = GetPitchClass(key.Tonic);
        var semitones = ((rootPitchClass - tonicPitchClass) + 12) % 12;

        // Find closest scale degree and use flat/sharp prefix
        var prefix = "";
        var degree = 1;

        var intervals = key.Mode == KeyMode.Major ? MajorScaleIntervals : MinorScaleIntervals;
        for (int i = 0; i < intervals.Length; i++)
        {
            if (semitones == intervals[i])
            {
                degree = i + 1;
                break;
            }
            if (semitones == intervals[i] - 1)
            {
                degree = i + 1;
                prefix = "b";
                break;
            }
            if (semitones == intervals[i] + 1)
            {
                degree = i + 1;
                prefix = "#";
                break;
            }
        }

        var numeral = chord.Quality == ChordQuality.Major ? ToUpperRoman(degree) : ToLowerRoman(degree);
        return prefix + numeral;
    }

    private static string AdjustNumeralForQuality(string baseNumeral, Chord chord, int degree, KeySignature key)
    {
        // If the chord quality doesn't match the expected diatonic quality,
        // adjust the case of the numeral
        var expectedQuality = GetExpectedQuality(degree, key);

        if (chord.Quality == ChordQuality.Major && expectedQuality != ChordQuality.Major)
        {
            return baseNumeral.ToUpperInvariant();
        }
        if (chord.Quality == ChordQuality.Minor && expectedQuality != ChordQuality.Minor)
        {
            return baseNumeral.ToLowerInvariant();
        }
        if (chord.Quality == ChordQuality.Diminished)
        {
            return baseNumeral.Replace("°", "") + "°";
        }
        if (chord.Quality == ChordQuality.Augmented)
        {
            return baseNumeral + "+";
        }

        return baseNumeral;
    }

    private static ChordQuality GetExpectedQuality(int degree, KeySignature key)
    {
        if (key.Mode == KeyMode.Major)
        {
            return degree switch
            {
                1 => ChordQuality.Major,
                2 => ChordQuality.Minor,
                3 => ChordQuality.Minor,
                4 => ChordQuality.Major,
                5 => ChordQuality.Major,
                6 => ChordQuality.Minor,
                7 => ChordQuality.Diminished,
                _ => ChordQuality.Major
            };
        }
        else
        {
            return degree switch
            {
                1 => ChordQuality.Minor,
                2 => ChordQuality.Diminished,
                3 => ChordQuality.Major,
                4 => ChordQuality.Minor,
                5 => ChordQuality.Minor, // Natural minor
                6 => ChordQuality.Major,
                7 => ChordQuality.Major,
                _ => ChordQuality.Minor
            };
        }
    }

    private static bool IsSeventhChord(ChordType type)
    {
        return type is ChordType.Major7 or ChordType.Minor7 or ChordType.Dominant7
            or ChordType.MinorMajor7 or ChordType.HalfDiminished7
            or ChordType.Diminished7 or ChordType.Augmented7 or ChordType.AugmentedMajor7;
    }

    private static string ToUpperRoman(int degree) => degree switch
    {
        1 => "I", 2 => "II", 3 => "III", 4 => "IV", 5 => "V", 6 => "VI", 7 => "VII", _ => "?"
    };

    private static string ToLowerRoman(int degree) => degree switch
    {
        1 => "i", 2 => "ii", 3 => "iii", 4 => "iv", 5 => "v", 6 => "vi", 7 => "vii", _ => "?"
    };

    private static int GetPitchClass(Note note)
    {
        var semitones = MusicTheoryConstants.SemitonesFromC[(int)note.Name] + (int)note.Alteration;
        return ((semitones % 12) + 12) % 12;
    }
}
