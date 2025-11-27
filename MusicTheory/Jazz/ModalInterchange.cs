namespace MusicTheory.Jazz;

/// <summary>
/// Provides modal interchange (borrowed chord) functionality for jazz and popular music harmony.
/// Modal interchange involves borrowing chords from parallel modes (e.g., using chords from
/// C minor while in the key of C major).
/// </summary>
public static class ModalInterchange
{
    // Major scale intervals (semitones from root)
    private static readonly int[] MajorScaleIntervals = { 0, 2, 4, 5, 7, 9, 11 };

    // Natural minor (Aeolian) scale intervals
    private static readonly int[] AeolianIntervals = { 0, 2, 3, 5, 7, 8, 10 };

    // Dorian scale intervals
    private static readonly int[] DorianIntervals = { 0, 2, 3, 5, 7, 9, 10 };

    // Phrygian scale intervals
    private static readonly int[] PhrygianIntervals = { 0, 1, 3, 5, 7, 8, 10 };

    // Mixolydian scale intervals
    private static readonly int[] MixolydianIntervals = { 0, 2, 4, 5, 7, 9, 10 };

    /// <summary>
    /// Determines if a chord is borrowed from a parallel mode.
    /// </summary>
    /// <param name="chord">The chord to check.</param>
    /// <param name="key">The key context.</param>
    /// <returns>True if the chord is borrowed from a parallel mode.</returns>
    public static bool IsBorrowedChord(Chord chord, KeySignature key)
    {
        // A borrowed chord is one that:
        // 1. Is not diatonic to the current key
        // 2. Would be diatonic in a parallel mode

        if (IsDiatonicChord(chord, key))
            return false;

        // Check if it exists in any parallel mode
        return GetSourceMode(chord, key) != BorrowedChordSource.None;
    }

    /// <summary>
    /// Determines the source mode from which a chord is borrowed.
    /// </summary>
    /// <param name="chord">The chord to analyze.</param>
    /// <param name="key">The key context.</param>
    /// <returns>The source mode, or None if the chord is diatonic or not borrowed.</returns>
    public static BorrowedChordSource GetSourceMode(Chord chord, KeySignature key)
    {
        if (IsDiatonicChord(chord, key))
            return BorrowedChordSource.None;

        var tonicPitchClass = GetPitchClass(key.Tonic);
        var chordRootPitchClass = GetPitchClass(chord.Root);
        var intervalFromTonic = ((chordRootPitchClass - tonicPitchClass) + 12) % 12;

        // Check Aeolian (natural minor) - most common source
        if (IsChordInMode(chord, intervalFromTonic, AeolianIntervals, isMinorMode: true))
            return BorrowedChordSource.Aeolian;

        // Check Mixolydian
        if (IsChordInMode(chord, intervalFromTonic, MixolydianIntervals, isMinorMode: false))
            return BorrowedChordSource.Mixolydian;

        // Check Dorian
        if (IsChordInMode(chord, intervalFromTonic, DorianIntervals, isMinorMode: true))
            return BorrowedChordSource.Dorian;

        // Check Phrygian (for bII - Neapolitan)
        if (IsChordInMode(chord, intervalFromTonic, PhrygianIntervals, isMinorMode: true))
            return BorrowedChordSource.Phrygian;

        return BorrowedChordSource.None;
    }

    /// <summary>
    /// Gets common borrowed chords for a given key.
    /// </summary>
    /// <param name="key">The key to get borrowed chords for.</param>
    /// <returns>A list of commonly borrowed chords.</returns>
    public static IReadOnlyList<Chord> GetCommonBorrowedChords(KeySignature key)
    {
        var borrowedChords = new List<Chord>();
        var tonicPitchClass = GetPitchClass(key.Tonic);

        if (key.Mode == KeyMode.Major)
        {
            // Common borrowings from parallel minor in major keys:
            // bIII - major chord on flat 3rd (e.g., Eb in C major)
            var bIII = CreateChordAtInterval(tonicPitchClass, 3, ChordQuality.Major);
            borrowedChords.Add(bIII);

            // iv - minor chord on 4th (e.g., Fm in C major)
            var iv = CreateChordAtInterval(tonicPitchClass, 5, ChordQuality.Minor);
            borrowedChords.Add(iv);

            // bVI - major chord on flat 6th (e.g., Ab in C major)
            var bVI = CreateChordAtInterval(tonicPitchClass, 8, ChordQuality.Major);
            borrowedChords.Add(bVI);

            // bVII - major chord on flat 7th (e.g., Bb in C major)
            var bVII = CreateChordAtInterval(tonicPitchClass, 10, ChordQuality.Major);
            borrowedChords.Add(bVII);

            // i - minor tonic (e.g., Cm in C major)
            var i = CreateChordAtInterval(tonicPitchClass, 0, ChordQuality.Minor);
            borrowedChords.Add(i);

            // bII (Neapolitan) - major chord on flat 2nd (e.g., Db in C major)
            var bII = CreateChordAtInterval(tonicPitchClass, 1, ChordQuality.Major);
            borrowedChords.Add(bII);
        }
        else
        {
            // Common borrowings in minor keys (from parallel major or harmonic minor):
            // IV - major chord on 4th (e.g., F in C minor, from Dorian)
            var IV = CreateChordAtInterval(tonicPitchClass, 5, ChordQuality.Major);
            borrowedChords.Add(IV);

            // I - major tonic (Picardy third)
            var I = CreateChordAtInterval(tonicPitchClass, 0, ChordQuality.Major);
            borrowedChords.Add(I);
        }

        return borrowedChords;
    }

    /// <summary>
    /// Gets the Roman numeral representation for a borrowed chord.
    /// </summary>
    /// <param name="chord">The borrowed chord.</param>
    /// <param name="key">The key context.</param>
    /// <returns>The Roman numeral string (e.g., "bVI", "iv").</returns>
    public static string GetRomanNumeral(Chord chord, KeySignature key)
    {
        var tonicPitchClass = GetPitchClass(key.Tonic);
        var chordRootPitchClass = GetPitchClass(chord.Root);
        var intervalFromTonic = ((chordRootPitchClass - tonicPitchClass) + 12) % 12;

        // Determine the degree and any alterations
        var (degree, prefix) = GetDegreeFromInterval(intervalFromTonic, key);

        // Format the Roman numeral based on chord quality
        var numeral = chord.Quality == ChordQuality.Major
            ? ToUpperRoman(degree)
            : ToLowerRoman(degree);

        if (chord.Quality == ChordQuality.Diminished)
            numeral += "°";
        else if (chord.Quality == ChordQuality.Augmented)
            numeral += "+";

        return prefix + numeral;
    }

    private static bool IsDiatonicChord(Chord chord, KeySignature key)
    {
        var tonicPitchClass = GetPitchClass(key.Tonic);
        var chordRootPitchClass = GetPitchClass(chord.Root);
        var intervalFromTonic = ((chordRootPitchClass - tonicPitchClass) + 12) % 12;

        var scaleIntervals = key.Mode == KeyMode.Major ? MajorScaleIntervals : AeolianIntervals;

        // Check if the chord root is in the scale
        if (!scaleIntervals.Contains(intervalFromTonic))
            return false;

        // Check if the chord quality matches the expected diatonic quality
        var degreeIndex = Array.IndexOf(scaleIntervals, intervalFromTonic);
        var expectedQuality = GetExpectedDiatonicQuality(degreeIndex, key.Mode);

        return chord.Quality == expectedQuality;
    }

    private static bool IsChordInMode(Chord chord, int intervalFromTonic, int[] modeIntervals, bool isMinorMode)
    {
        // Check if the interval exists in this mode
        if (!modeIntervals.Contains(intervalFromTonic))
            return false;

        // Check if the chord quality matches what would be expected in this mode
        var degreeIndex = Array.IndexOf(modeIntervals, intervalFromTonic);
        var expectedQuality = GetExpectedModeQuality(degreeIndex, modeIntervals);

        return chord.Quality == expectedQuality;
    }

    private static ChordQuality GetExpectedDiatonicQuality(int degreeIndex, KeyMode mode)
    {
        if (mode == KeyMode.Major)
        {
            // Major scale diatonic qualities: I, ii, iii, IV, V, vi, vii°
            return degreeIndex switch
            {
                0 => ChordQuality.Major,      // I
                1 => ChordQuality.Minor,      // ii
                2 => ChordQuality.Minor,      // iii
                3 => ChordQuality.Major,      // IV
                4 => ChordQuality.Major,      // V
                5 => ChordQuality.Minor,      // vi
                6 => ChordQuality.Diminished, // vii°
                _ => ChordQuality.Major
            };
        }
        else
        {
            // Natural minor diatonic qualities: i, ii°, III, iv, v, VI, VII
            return degreeIndex switch
            {
                0 => ChordQuality.Minor,      // i
                1 => ChordQuality.Diminished, // ii°
                2 => ChordQuality.Major,      // III
                3 => ChordQuality.Minor,      // iv
                4 => ChordQuality.Minor,      // v
                5 => ChordQuality.Major,      // VI
                6 => ChordQuality.Major,      // VII
                _ => ChordQuality.Minor
            };
        }
    }

    private static ChordQuality GetExpectedModeQuality(int degreeIndex, int[] modeIntervals)
    {
        // Determine quality based on the intervals in the mode
        // This is a simplification - we check if the mode produces major or minor 3rd
        // For the chord built on this degree

        if (modeIntervals == AeolianIntervals)
        {
            return degreeIndex switch
            {
                0 => ChordQuality.Minor,      // i
                1 => ChordQuality.Diminished, // ii°
                2 => ChordQuality.Major,      // III
                3 => ChordQuality.Minor,      // iv
                4 => ChordQuality.Minor,      // v
                5 => ChordQuality.Major,      // VI
                6 => ChordQuality.Major,      // VII
                _ => ChordQuality.Major
            };
        }
        else if (modeIntervals == MixolydianIntervals)
        {
            return degreeIndex switch
            {
                0 => ChordQuality.Major,      // I
                1 => ChordQuality.Minor,      // ii
                2 => ChordQuality.Diminished, // iii°
                3 => ChordQuality.Major,      // IV
                4 => ChordQuality.Minor,      // v
                5 => ChordQuality.Minor,      // vi
                6 => ChordQuality.Major,      // VII
                _ => ChordQuality.Major
            };
        }
        else if (modeIntervals == DorianIntervals)
        {
            return degreeIndex switch
            {
                0 => ChordQuality.Minor,      // i
                1 => ChordQuality.Minor,      // ii
                2 => ChordQuality.Major,      // III
                3 => ChordQuality.Major,      // IV
                4 => ChordQuality.Minor,      // v
                5 => ChordQuality.Diminished, // vi°
                6 => ChordQuality.Major,      // VII
                _ => ChordQuality.Minor
            };
        }
        else if (modeIntervals == PhrygianIntervals)
        {
            return degreeIndex switch
            {
                0 => ChordQuality.Minor,      // i
                1 => ChordQuality.Major,      // II (Neapolitan)
                2 => ChordQuality.Major,      // III
                3 => ChordQuality.Minor,      // iv
                4 => ChordQuality.Diminished, // v°
                5 => ChordQuality.Major,      // VI
                6 => ChordQuality.Minor,      // vii
                _ => ChordQuality.Minor
            };
        }

        return ChordQuality.Major;
    }

    private static Chord CreateChordAtInterval(int tonicPitchClass, int interval, ChordQuality quality)
    {
        var pitchClass = (tonicPitchClass + interval) % 12;
        var root = PitchClassToNote(pitchClass, preferFlats: true);
        return new Chord(root, quality);
    }

    private static (int degree, string prefix) GetDegreeFromInterval(int interval, KeySignature key)
    {
        var scaleIntervals = key.Mode == KeyMode.Major ? MajorScaleIntervals : AeolianIntervals;

        // Check for exact match in diatonic scale
        for (int i = 0; i < scaleIntervals.Length; i++)
        {
            if (scaleIntervals[i] == interval)
                return (i + 1, "");
        }

        // Check for flat alterations
        for (int i = 0; i < scaleIntervals.Length; i++)
        {
            if ((scaleIntervals[i] - 1 + 12) % 12 == interval)
                return (i + 1, "b");
        }

        // Check for sharp alterations
        for (int i = 0; i < scaleIntervals.Length; i++)
        {
            if ((scaleIntervals[i] + 1) % 12 == interval)
                return (i + 1, "#");
        }

        return (1, "");
    }

    private static Note PitchClassToNote(int pitchClass, bool preferFlats)
    {
        return pitchClass switch
        {
            0 => new Note(NoteName.C, Alteration.Natural),
            1 => preferFlats ? new Note(NoteName.D, Alteration.Flat) : new Note(NoteName.C, Alteration.Sharp),
            2 => new Note(NoteName.D, Alteration.Natural),
            3 => preferFlats ? new Note(NoteName.E, Alteration.Flat) : new Note(NoteName.D, Alteration.Sharp),
            4 => new Note(NoteName.E, Alteration.Natural),
            5 => new Note(NoteName.F, Alteration.Natural),
            6 => preferFlats ? new Note(NoteName.G, Alteration.Flat) : new Note(NoteName.F, Alteration.Sharp),
            7 => new Note(NoteName.G, Alteration.Natural),
            8 => preferFlats ? new Note(NoteName.A, Alteration.Flat) : new Note(NoteName.G, Alteration.Sharp),
            9 => new Note(NoteName.A, Alteration.Natural),
            10 => preferFlats ? new Note(NoteName.B, Alteration.Flat) : new Note(NoteName.A, Alteration.Sharp),
            11 => new Note(NoteName.B, Alteration.Natural),
            _ => throw new ArgumentOutOfRangeException(nameof(pitchClass))
        };
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
