namespace MusicTheory.Jazz;

/// <summary>
/// Provides chord-scale theory functionality for jazz improvisation and harmony.
/// Chord-scale theory matches scales to chords based on their harmonic function
/// and the key context.
/// </summary>
public static class ChordScaleTheory
{
    /// <summary>
    /// Gets all scales that work with a given chord.
    /// </summary>
    /// <param name="chord">The chord to find scales for.</param>
    /// <returns>A list of scales that work with the chord.</returns>
    public static IReadOnlyList<Scale> GetScalesForChord(Chord chord)
    {
        var scales = new List<Scale>();
        var root = chord.Root;

        switch (chord.Type)
        {
            case ChordType.Major:
            case ChordType.Major7:
                scales.Add(new Scale(root, ScaleType.Major));    // Ionian
                scales.Add(new Scale(root, ScaleType.Lydian));   // Lydian
                break;

            case ChordType.Minor:
            case ChordType.Minor7:
                scales.Add(new Scale(root, ScaleType.Dorian));
                scales.Add(new Scale(root, ScaleType.NaturalMinor)); // Aeolian
                scales.Add(new Scale(root, ScaleType.Phrygian));
                break;

            case ChordType.Dominant7:
                scales.Add(new Scale(root, ScaleType.Mixolydian));
                scales.Add(new Scale(root, ScaleType.LydianDominant));
                scales.Add(new Scale(root, ScaleType.Altered));
                scales.Add(new Scale(root, ScaleType.DiminishedHalfWhole));
                scales.Add(new Scale(root, ScaleType.WholeTone));
                break;

            case ChordType.Dominant7Alt:
                scales.Add(new Scale(root, ScaleType.Altered));
                scales.Add(new Scale(root, ScaleType.DiminishedHalfWhole));
                break;

            case ChordType.Dominant7Flat9:
            case ChordType.Dominant7Sharp9:
                scales.Add(new Scale(root, ScaleType.DiminishedHalfWhole));
                scales.Add(new Scale(root, ScaleType.Altered));
                break;

            case ChordType.HalfDiminished7:
                scales.Add(new Scale(root, ScaleType.Locrian));
                scales.Add(new Scale(root, ScaleType.LocrianSharp2));
                break;

            case ChordType.Diminished7:
                scales.Add(new Scale(root, ScaleType.DiminishedWholeHalf));
                break;

            case ChordType.Diminished:
                scales.Add(new Scale(root, ScaleType.DiminishedWholeHalf));
                scales.Add(new Scale(root, ScaleType.Locrian));
                break;

            case ChordType.MinorMajor7:
                scales.Add(new Scale(root, ScaleType.MelodicMinor));
                scales.Add(new Scale(root, ScaleType.HarmonicMinor));
                break;

            case ChordType.Augmented:
            case ChordType.Augmented7:
            case ChordType.AugmentedMajor7:
                scales.Add(new Scale(root, ScaleType.WholeTone));
                break;

            default:
                // Default to major or minor based on quality
                if (chord.Quality == ChordQuality.Major)
                    scales.Add(new Scale(root, ScaleType.Major));
                else if (chord.Quality == ChordQuality.Minor)
                    scales.Add(new Scale(root, ScaleType.Dorian));
                else
                    scales.Add(new Scale(root, ScaleType.Major));
                break;
        }

        return scales;
    }

    /// <summary>
    /// Gets the primary (most common) scale for a chord in a given key context.
    /// </summary>
    /// <param name="chord">The chord.</param>
    /// <param name="key">The key context.</param>
    /// <returns>The primary scale for the chord.</returns>
    public static Scale GetPrimaryScale(Chord chord, KeySignature key)
    {
        var degree = GetScaleDegree(chord.Root, key);

        // Diatonic chord-scale relationships in major
        if (key.Mode == KeyMode.Major)
        {
            return degree switch
            {
                1 => new Scale(chord.Root, ScaleType.Major),      // I = Ionian
                2 => new Scale(chord.Root, ScaleType.Dorian),     // ii = Dorian
                3 => new Scale(chord.Root, ScaleType.Phrygian),   // iii = Phrygian
                4 => new Scale(chord.Root, ScaleType.Lydian),     // IV = Lydian
                5 => new Scale(chord.Root, ScaleType.Mixolydian), // V = Mixolydian
                6 => new Scale(chord.Root, ScaleType.NaturalMinor), // vi = Aeolian
                7 => new Scale(chord.Root, ScaleType.Locrian),    // vii° = Locrian
                _ => GetDefaultScaleForChordType(chord)
            };
        }
        else // Minor key
        {
            return degree switch
            {
                1 => new Scale(chord.Root, ScaleType.NaturalMinor), // i = Aeolian
                2 => new Scale(chord.Root, ScaleType.Locrian),      // ii° = Locrian
                3 => new Scale(chord.Root, ScaleType.Major),        // III = Ionian
                4 => new Scale(chord.Root, ScaleType.Dorian),       // iv = Dorian
                5 => new Scale(chord.Root, ScaleType.Phrygian),     // v = Phrygian (or Mixolydian if V7)
                6 => new Scale(chord.Root, ScaleType.Lydian),       // VI = Lydian
                7 => new Scale(chord.Root, ScaleType.Mixolydian),   // VII = Mixolydian
                _ => GetDefaultScaleForChordType(chord)
            };
        }
    }

    /// <summary>
    /// Gets the avoid notes for a chord-scale combination.
    /// Avoid notes are scale degrees that create dissonance with chord tones.
    /// </summary>
    /// <param name="chord">The chord.</param>
    /// <param name="scale">The scale being used.</param>
    /// <returns>A list of avoid notes.</returns>
    public static IReadOnlyList<Note> GetAvoidNotes(Chord chord, Scale scale)
    {
        var avoidNotes = new List<Note>();
        var scaleNotes = scale.GetNotes().ToList();
        var chordNotes = chord.GetNotes().ToList();

        // Avoid notes are typically:
        // - Notes that are a half step above a chord tone (creates minor 9th dissonance)
        // - Specifically, the 4th is an avoid note in Ionian and Mixolydian

        switch (scale.Type)
        {
            case ScaleType.Major: // Ionian
            case ScaleType.Ionian:
                // 4th degree is avoid (half step above 3rd)
                if (scaleNotes.Count > 3)
                    avoidNotes.Add(scaleNotes[3]); // 4th degree (0-indexed = 3)
                break;

            case ScaleType.Dorian:
                // 6th degree can be an avoid (but less strict in jazz)
                if (scaleNotes.Count > 5)
                    avoidNotes.Add(scaleNotes[5]); // 6th degree
                break;

            case ScaleType.Phrygian:
                // 2nd degree is avoid (b2)
                if (scaleNotes.Count > 1)
                    avoidNotes.Add(scaleNotes[1]); // b2
                break;

            case ScaleType.Mixolydian:
                // 4th degree is avoid
                if (scaleNotes.Count > 3)
                    avoidNotes.Add(scaleNotes[3]); // 4th degree
                break;

            case ScaleType.NaturalMinor:
            case ScaleType.Aeolian:
                // 6th degree can be avoid (b6 - half step above 5th)
                if (scaleNotes.Count > 5)
                    avoidNotes.Add(scaleNotes[5]); // b6
                break;

            case ScaleType.Locrian:
                // 2nd degree is avoid
                if (scaleNotes.Count > 1)
                    avoidNotes.Add(scaleNotes[1]); // b2
                break;
        }

        return avoidNotes;
    }

    /// <summary>
    /// Gets the color tones (tensions) for a chord-scale combination.
    /// Color tones are non-chord tones that add harmonic interest without creating dissonance.
    /// </summary>
    /// <param name="chord">The chord.</param>
    /// <param name="scale">The scale being used.</param>
    /// <returns>A list of color tones.</returns>
    public static IReadOnlyList<Note> GetColorTones(Chord chord, Scale scale)
    {
        var colorTones = new List<Note>();
        var scaleNotes = scale.GetNotes().ToList();
        var avoidNotes = GetAvoidNotes(chord, scale);

        // Color tones are scale degrees that aren't chord tones or avoid notes
        // Typically 9, 11 (if not avoid), 13

        // 9th (2nd degree)
        if (scaleNotes.Count > 1)
        {
            var ninth = scaleNotes[1];
            if (!avoidNotes.Any(a => a.Name == ninth.Name && a.Alteration == ninth.Alteration))
                colorTones.Add(ninth);
        }

        // 11th (4th degree) - only if not an avoid note
        if (scaleNotes.Count > 3)
        {
            var eleventh = scaleNotes[3];
            if (!avoidNotes.Any(a => a.Name == eleventh.Name && a.Alteration == eleventh.Alteration))
                colorTones.Add(eleventh);
        }

        // 13th (6th degree) - only if not an avoid note
        if (scaleNotes.Count > 5)
        {
            var thirteenth = scaleNotes[5];
            if (!avoidNotes.Any(a => a.Name == thirteenth.Name && a.Alteration == thirteenth.Alteration))
                colorTones.Add(thirteenth);
        }

        return colorTones;
    }

    private static int GetScaleDegree(Note root, KeySignature key)
    {
        var tonicPitchClass = GetPitchClass(key.Tonic);
        var rootPitchClass = GetPitchClass(root);

        // Major scale intervals
        var majorIntervals = new[] { 0, 2, 4, 5, 7, 9, 11 };
        var minorIntervals = new[] { 0, 2, 3, 5, 7, 8, 10 };

        var intervals = key.Mode == KeyMode.Major ? majorIntervals : minorIntervals;
        var intervalFromTonic = ((rootPitchClass - tonicPitchClass) + 12) % 12;

        for (int i = 0; i < intervals.Length; i++)
        {
            if (intervals[i] == intervalFromTonic)
                return i + 1;
        }

        return 0; // Non-diatonic
    }

    private static Scale GetDefaultScaleForChordType(Chord chord)
    {
        var scales = GetScalesForChord(chord);
        return scales.FirstOrDefault() ?? new Scale(chord.Root, ScaleType.Major);
    }

    private static int GetPitchClass(Note note)
    {
        var semitones = MusicTheoryConstants.SemitonesFromC[(int)note.Name] + (int)note.Alteration;
        return ((semitones % 12) + 12) % 12;
    }
}
