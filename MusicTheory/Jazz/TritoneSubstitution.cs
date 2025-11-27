namespace MusicTheory.Jazz;

/// <summary>
/// Provides tritone substitution functionality for jazz harmony.
/// A tritone substitution replaces a dominant chord with another dominant chord
/// whose root is a tritone (6 semitones) away.
/// </summary>
public static class TritoneSubstitution
{
    private const int TritoneSemitones = 6;

    /// <summary>
    /// Gets the tritone substitute for a dominant chord.
    /// </summary>
    /// <param name="dominantChord">The original dominant chord.</param>
    /// <returns>The tritone substitute chord.</returns>
    /// <exception cref="ArgumentException">If the chord cannot be tritone substituted.</exception>
    public static Chord GetSubstitute(Chord dominantChord)
    {
        if (!CanSubstitute(dominantChord))
        {
            throw new ArgumentException(
                "Only dominant 7th chords or major triads can be tritone substituted.",
                nameof(dominantChord));
        }

        // Calculate the new root (tritone away)
        var newRoot = GetTritoneRoot(dominantChord.Root);

        // Return a dominant 7th chord on the new root
        return new Chord(newRoot, ChordType.Dominant7);
    }

    /// <summary>
    /// Determines if a chord can be tritone substituted.
    /// </summary>
    /// <param name="chord">The chord to check.</param>
    /// <returns>True if the chord can be tritone substituted.</returns>
    public static bool CanSubstitute(Chord chord)
    {
        // Dominant 7th chords are the primary candidates
        if (chord.Type == ChordType.Dominant7)
            return true;

        // Major triads can function as dominants (but not Major7 chords)
        if (chord.Type == ChordType.Major)
            return true;

        // Extended dominant chords
        if (chord.Type is ChordType.Dominant9 or ChordType.Dominant11 or ChordType.Dominant13)
            return true;

        // Altered dominant chords
        if (chord.Type is ChordType.Dominant7Flat5 or ChordType.Dominant7Sharp5
            or ChordType.Dominant7Flat9 or ChordType.Dominant7Sharp9
            or ChordType.Dominant7Alt)
            return true;

        return false;
    }

    /// <summary>
    /// Determines if two chords are tritone related (one is the tritone substitute of the other).
    /// </summary>
    /// <param name="chord1">The first chord.</param>
    /// <param name="chord2">The second chord.</param>
    /// <returns>True if the chords are tritone related.</returns>
    public static bool AreTritoneRelated(Chord chord1, Chord chord2)
    {
        var pitchClass1 = GetPitchClass(chord1.Root);
        var pitchClass2 = GetPitchClass(chord2.Root);

        // Roots should be exactly 6 semitones apart
        var difference = Math.Abs(pitchClass1 - pitchClass2);
        return difference == TritoneSemitones || difference == 12 - TritoneSemitones;
    }

    /// <summary>
    /// Gets the shared guide tones between a dominant chord and its tritone substitute.
    /// The guide tones (3rd and 7th) are enharmonically equivalent between tritone-related chords.
    /// </summary>
    /// <param name="original">The original dominant chord.</param>
    /// <param name="substitute">The tritone substitute chord.</param>
    /// <returns>The shared guide tones.</returns>
    public static IReadOnlyList<Note> GetSharedGuideTones(Chord original, Chord substitute)
    {
        var originalNotes = original.GetNotes().ToList();
        var substituteNotes = substitute.GetNotes().ToList();

        var sharedTones = new List<Note>();

        // Get the 3rd and 7th of original chord
        if (originalNotes.Count >= 4)
        {
            var originalThird = originalNotes[1]; // 3rd
            var originalSeventh = originalNotes[3]; // 7th

            // Find enharmonic equivalents in substitute
            foreach (var subNote in substituteNotes)
            {
                if (AreEnharmonic(originalThird, subNote) || AreEnharmonic(originalSeventh, subNote))
                {
                    sharedTones.Add(subNote);
                }
            }
        }

        // If we couldn't find them that way, use pitch class comparison
        if (sharedTones.Count < 2)
        {
            var originalPitchClasses = originalNotes.Select(GetPitchClass).ToHashSet();
            var substitutePitchClasses = substituteNotes.Select(GetPitchClass).ToHashSet();
            var shared = originalPitchClasses.Intersect(substitutePitchClasses);

            sharedTones.Clear();
            foreach (var pc in shared)
            {
                var note = substituteNotes.FirstOrDefault(n => GetPitchClass(n) == pc);
                if (note != null)
                    sharedTones.Add(note);
            }
        }

        return sharedTones;
    }

    private static Note GetTritoneRoot(Note root)
    {
        var rootPitchClass = GetPitchClass(root);
        var newPitchClass = (rootPitchClass + TritoneSemitones) % 12;

        // Determine the best spelling for the new root
        // Prefer flats for most tritone substitutes (jazz convention)
        return PitchClassToNote(newPitchClass, preferFlats: true);
    }

    private static Note PitchClassToNote(int pitchClass, bool preferFlats)
    {
        // Map pitch classes to notes, preferring flats for jazz
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

    private static bool AreEnharmonic(Note note1, Note note2)
    {
        return GetPitchClass(note1) == GetPitchClass(note2);
    }

    private static int GetPitchClass(Note note)
    {
        var semitones = MusicTheoryConstants.SemitonesFromC[(int)note.Name] + (int)note.Alteration;
        return ((semitones % 12) + 12) % 12;
    }
}
