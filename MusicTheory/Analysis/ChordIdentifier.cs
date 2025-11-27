namespace MusicTheory.Analysis;

/// <summary>
/// Provides chord identification from a collection of notes.
/// </summary>
public static class ChordIdentifier
{
    // Pitch class intervals for each chord quality (semitones from root)
    private static readonly Dictionary<ChordQuality, int[]> TriadIntervals = new()
    {
        { ChordQuality.Major, new[] { 0, 4, 7 } },       // Root, Major 3rd, Perfect 5th
        { ChordQuality.Minor, new[] { 0, 3, 7 } },       // Root, Minor 3rd, Perfect 5th
        { ChordQuality.Diminished, new[] { 0, 3, 6 } },  // Root, Minor 3rd, Diminished 5th
        { ChordQuality.Augmented, new[] { 0, 4, 8 } }    // Root, Major 3rd, Augmented 5th
    };

    // Pitch class intervals for seventh chords (semitones from root)
    private static readonly Dictionary<ChordType, int[]> SeventhChordIntervals = new()
    {
        { ChordType.Major7, new[] { 0, 4, 7, 11 } },         // Root, M3, P5, M7
        { ChordType.Minor7, new[] { 0, 3, 7, 10 } },         // Root, m3, P5, m7
        { ChordType.Dominant7, new[] { 0, 4, 7, 10 } },      // Root, M3, P5, m7
        { ChordType.MinorMajor7, new[] { 0, 3, 7, 11 } },    // Root, m3, P5, M7
        { ChordType.HalfDiminished7, new[] { 0, 3, 6, 10 } },// Root, m3, d5, m7
        { ChordType.Diminished7, new[] { 0, 3, 6, 9 } },     // Root, m3, d5, d7
        { ChordType.Augmented7, new[] { 0, 4, 8, 10 } },     // Root, M3, A5, m7
        { ChordType.AugmentedMajor7, new[] { 0, 4, 8, 11 } } // Root, M3, A5, M7
    };

    /// <summary>
    /// Identifies a chord from a collection of notes.
    /// </summary>
    /// <param name="notes">The notes to identify as a chord.</param>
    /// <returns>The chord identification result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when notes is null.</exception>
    /// <exception cref="ArgumentException">Thrown when fewer than 3 notes are provided.</exception>
    public static ChordIdentification Identify(IEnumerable<Note> notes)
    {
        if (notes == null)
            throw new ArgumentNullException(nameof(notes));

        var noteList = notes.ToList();

        if (noteList.Count < 3)
            throw new ArgumentException("At least 3 notes are required to identify a chord.", nameof(notes));

        // Get unique pitch classes (0-11) from the notes
        var pitchClasses = GetUniquePitchClasses(noteList);

        // Find the bass note (lowest pitch)
        var bassNote = noteList.OrderBy(n => MusicTheoryUtilities.GetTotalSemitones(n)).First();
        var bassPitchClass = GetPitchClass(bassNote);

        // Try to identify the chord
        var (bestChord, bestInversion, bestConfidence) = FindBestMatch(pitchClasses, bassPitchClass, noteList);

        if (bestChord == null)
        {
            // If no match found, return a major chord on the bass note with low confidence
            bestChord = new Chord(bassNote, ChordQuality.Major);
            bestInversion = ChordInversion.Root;
            bestConfidence = 0.0;
        }

        return new ChordIdentification(bestChord, bestInversion, bestConfidence);
    }

    /// <summary>
    /// Identifies a chord with context from a key signature.
    /// </summary>
    /// <param name="notes">The notes to identify as a chord.</param>
    /// <param name="context">The key signature context.</param>
    /// <returns>The chord identification result.</returns>
    public static ChordIdentification Identify(IEnumerable<Note> notes, KeySignature context)
    {
        // For now, delegate to the basic identify method
        // In future, this could use the key context to prefer diatonic interpretations
        return Identify(notes);
    }

    /// <summary>
    /// Gets all possible chord interpretations for a collection of notes.
    /// </summary>
    /// <param name="notes">The notes to analyze.</param>
    /// <returns>All possible chord interpretations, ordered by confidence.</returns>
    public static IEnumerable<ChordIdentification> GetAllInterpretations(IEnumerable<Note> notes)
    {
        if (notes == null)
            throw new ArgumentNullException(nameof(notes));

        var noteList = notes.ToList();

        if (noteList.Count < 3)
            throw new ArgumentException("At least 3 notes are required to identify a chord.", nameof(notes));

        var pitchClasses = GetUniquePitchClasses(noteList);
        var bassNote = noteList.OrderBy(n => MusicTheoryUtilities.GetTotalSemitones(n)).First();
        var bassPitchClass = GetPitchClass(bassNote);

        var interpretations = new List<ChordIdentification>();

        // Try each pitch class as a potential root
        foreach (var rootPitchClass in pitchClasses)
        {
            // Try each chord quality
            foreach (var quality in TriadIntervals.Keys)
            {
                var match = TryMatchChord(pitchClasses, rootPitchClass, quality, bassPitchClass, noteList);
                if (match != null && match.Confidence > 0)
                {
                    interpretations.Add(match);
                }
            }
        }

        return interpretations.OrderByDescending(i => i.Confidence);
    }

    private static (Chord? Chord, ChordInversion Inversion, double Confidence) FindBestMatch(
        HashSet<int> pitchClasses,
        int bassPitchClass,
        List<Note> originalNotes)
    {
        Chord? bestChord = null;
        ChordInversion bestInversion = ChordInversion.Root;
        double bestConfidence = 0.0;

        // Try each pitch class as a potential root
        foreach (var rootPitchClass in pitchClasses)
        {
            // Try seventh chords first (more specific match)
            foreach (var (chordType, intervals) in SeventhChordIntervals)
            {
                var match = TryMatchSeventhChord(pitchClasses, rootPitchClass, chordType, intervals, bassPitchClass, originalNotes);
                if (match != null && match.Confidence > bestConfidence)
                {
                    bestChord = match.IdentifiedChord;
                    bestInversion = match.Inversion;
                    bestConfidence = match.Confidence;
                }
            }

            // Then try triads
            foreach (var (quality, intervals) in TriadIntervals)
            {
                var match = TryMatchChord(pitchClasses, rootPitchClass, quality, bassPitchClass, originalNotes);
                if (match != null && match.Confidence > bestConfidence)
                {
                    bestChord = match.IdentifiedChord;
                    bestInversion = match.Inversion;
                    bestConfidence = match.Confidence;
                }
            }
        }

        return (bestChord, bestInversion, bestConfidence);
    }

    private static ChordIdentification? TryMatchChord(
        HashSet<int> pitchClasses,
        int rootPitchClass,
        ChordQuality quality,
        int bassPitchClass,
        List<Note> originalNotes)
    {
        var intervals = TriadIntervals[quality];
        var expectedPitchClasses = intervals.Select(i => (rootPitchClass + i) % 12).ToHashSet();

        // Check how well the pitch classes match
        var matchedCount = pitchClasses.Intersect(expectedPitchClasses).Count();
        var expectedCount = expectedPitchClasses.Count;
        var extraNotes = pitchClasses.Except(expectedPitchClasses).Count();

        // All expected pitch classes must be present for a valid match
        if (matchedCount < expectedCount)
            return null;

        // Calculate confidence based on match quality
        double confidence = (double)matchedCount / expectedCount;
        if (extraNotes > 0)
        {
            confidence *= 1.0 - (extraNotes * 0.1); // Reduce confidence for extra notes
        }

        // Find the root note from original notes
        var rootNote = FindNoteWithPitchClass(originalNotes, rootPitchClass);
        if (rootNote == null)
            return null;

        // Create the chord
        var chord = new Chord(rootNote, quality);

        // Determine inversion based on bass note
        var thirdPitchClass = (rootPitchClass + intervals[1]) % 12;
        var fifthPitchClass = (rootPitchClass + intervals[2]) % 12;

        ChordInversion inversion;
        if (bassPitchClass == rootPitchClass)
        {
            inversion = ChordInversion.Root;
        }
        else if (bassPitchClass == thirdPitchClass)
        {
            inversion = ChordInversion.First;
        }
        else if (bassPitchClass == fifthPitchClass)
        {
            inversion = ChordInversion.Second;
        }
        else
        {
            inversion = ChordInversion.Root; // Default to root if bass doesn't match
            confidence *= 0.8; // Reduce confidence
        }

        return new ChordIdentification(chord, inversion, confidence);
    }

    private static ChordIdentification? TryMatchSeventhChord(
        HashSet<int> pitchClasses,
        int rootPitchClass,
        ChordType chordType,
        int[] intervals,
        int bassPitchClass,
        List<Note> originalNotes)
    {
        var expectedPitchClasses = intervals.Select(i => (rootPitchClass + i) % 12).ToHashSet();

        // Check how well the pitch classes match
        var matchedCount = pitchClasses.Intersect(expectedPitchClasses).Count();
        var expectedCount = expectedPitchClasses.Count;
        var extraNotes = pitchClasses.Except(expectedPitchClasses).Count();

        // All expected pitch classes must be present for a valid match
        if (matchedCount < expectedCount)
            return null;

        // Calculate confidence based on match quality
        // Seventh chords get a slight boost for being a more complete match
        double confidence = (double)matchedCount / expectedCount;
        if (extraNotes > 0)
        {
            confidence *= 1.0 - (extraNotes * 0.1);
        }
        // Boost confidence for seventh chords when all 4 notes are present
        if (matchedCount == 4 && extraNotes == 0)
        {
            confidence *= 1.1; // Slight boost to prefer seventh chord over triad with extra note
            if (confidence > 1.0) confidence = 1.0;
        }

        // Find the root note from original notes
        var rootNote = FindNoteWithPitchClass(originalNotes, rootPitchClass);
        if (rootNote == null)
            return null;

        // Create the chord
        var chord = new Chord(rootNote, chordType);

        // Determine inversion based on bass note
        var thirdPitchClass = (rootPitchClass + intervals[1]) % 12;
        var fifthPitchClass = (rootPitchClass + intervals[2]) % 12;
        var seventhPitchClass = (rootPitchClass + intervals[3]) % 12;

        ChordInversion inversion;
        if (bassPitchClass == rootPitchClass)
        {
            inversion = ChordInversion.Root;
        }
        else if (bassPitchClass == thirdPitchClass)
        {
            inversion = ChordInversion.First;
        }
        else if (bassPitchClass == fifthPitchClass)
        {
            inversion = ChordInversion.Second;
        }
        else if (bassPitchClass == seventhPitchClass)
        {
            inversion = ChordInversion.Third;
        }
        else
        {
            inversion = ChordInversion.Root;
            confidence *= 0.8;
        }

        return new ChordIdentification(chord, inversion, confidence);
    }

    private static Note? FindNoteWithPitchClass(List<Note> notes, int targetPitchClass)
    {
        // Prefer natural notes, then sharps, then flats
        var candidates = notes.Where(n => GetPitchClass(n) == targetPitchClass).ToList();

        if (!candidates.Any())
            return null;

        // Return the first note with the matching pitch class
        // Prefer the one that appeared lowest in pitch (likely the root)
        return candidates.OrderBy(n => MusicTheoryUtilities.GetTotalSemitones(n)).First();
    }

    private static HashSet<int> GetUniquePitchClasses(IEnumerable<Note> notes)
    {
        return notes.Select(GetPitchClass).ToHashSet();
    }

    private static int GetPitchClass(Note note)
    {
        // Get semitones from C and normalize to 0-11
        var semitones = MusicTheoryConstants.SemitonesFromC[(int)note.Name] + (int)note.Alteration;
        return ((semitones % 12) + 12) % 12; // Handle negative values
    }
}
