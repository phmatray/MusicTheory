namespace MusicTheory;

/// <summary>
/// Provides methods for identifying chords from a set of notes.
/// Given any collection of notes (regardless of octave or order), 
/// returns all matching <see cref="ChordType"/>s and root candidates.
/// </summary>
/// <remarks>
/// This is useful for:
/// - Chord recognition in MIDI input or notation parsing
/// - Building chord-suggestion features in music apps
/// - Identifying slash chords (first/second inversion) from raw note sets
/// </remarks>
public static class ChordFinder
{
    /// <summary>
    /// Identifies all chord types that match a given set of pitch-class integers (0–11).
    /// Each entry in the result represents a possible root + chord type interpretation.
    /// </summary>
    /// <param name="notes">
    /// The notes to identify. Octave information is ignored — only pitch class is used.
    /// Duplicates and unison octaves are collapsed.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ChordMatch"/> ordered by confidence (more specific chords
    /// — i.e. those requiring fewer "missing" tones — rank higher).
    /// Returns an empty enumerable if no chord is identified.
    /// </returns>
    public static IEnumerable<ChordMatch> Identify(IEnumerable<Note> notes)
    {
        // Normalise to distinct pitch classes (semitones mod 12)
        var pitchClasses = notes
            .Select(n => MusicTheoryUtilities.GetTotalSemitones(n) % 12)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        if (pitchClasses.Count < 2)
            yield break;

        // Try each note as a potential root
        foreach (var rootPc in pitchClasses)
        {
            // Normalise intervals relative to this root
            var intervals = pitchClasses
                .Select(pc => (pc - rootPc + 12) % 12)
                .OrderBy(x => x)
                .ToHashSet();

            foreach (var (type, pattern) in ChordIntervalPatterns)
            {
                // A chord matches if the input contains all required intervals of the pattern
                // (we allow extra notes — useful for voicings with added tensions)
                if (pattern.All(i => intervals.Contains(i)))
                {
                    var rootNote = PitchClassToNote(rootPc);
                    var extraNotes = intervals.Count - pattern.Count;
                    yield return new ChordMatch(rootNote, type, extraNotes);
                }
            }
        }
    }

    /// <summary>
    /// Identifies chords from raw pitch-class integers (0 = C, 1 = C#, …, 11 = B).
    /// Convenience overload for callers that already have semitone values.
    /// </summary>
    public static IEnumerable<ChordMatch> IdentifyFromPitchClasses(IEnumerable<int> pitchClasses)
    {
        var notes = pitchClasses
            .Select(pc => PitchClassToNote(((pc % 12) + 12) % 12));
        return Identify(notes);
    }

    /// <summary>
    /// Returns the best single chord match (highest confidence) for a note set,
    /// or <c>null</c> if no chord is identified.
    /// </summary>
    public static ChordMatch? BestMatch(IEnumerable<Note> notes)
        => Identify(notes)
            .OrderBy(m => m.ExtraNotes)              // fewest extra notes = most specific
            .ThenByDescending(m => ChordComplexity(m.Type)) // prefer richer chords when tied
            .FirstOrDefault();

    // ── Private helpers ──────────────────────────────────────────────────────

    private static Note PitchClassToNote(int pc)
    {
        // Map pitch class (0–11) to a natural or single-sharp note
        return pc switch
        {
            0  => new Note(NoteName.C),
            1  => new Note(NoteName.C, Alteration.Sharp),
            2  => new Note(NoteName.D),
            3  => new Note(NoteName.D, Alteration.Sharp),
            4  => new Note(NoteName.E),
            5  => new Note(NoteName.F),
            6  => new Note(NoteName.F, Alteration.Sharp),
            7  => new Note(NoteName.G),
            8  => new Note(NoteName.G, Alteration.Sharp),
            9  => new Note(NoteName.A),
            10 => new Note(NoteName.A, Alteration.Sharp),
            11 => new Note(NoteName.B),
            _  => throw new ArgumentOutOfRangeException(nameof(pc), pc, "Pitch class must be 0–11")
        };
    }

    private static int ChordComplexity(ChordType type) => type switch
    {
        // Prefer richer chords as "better" matches when note counts are equal
        ChordType.Dominant13  or ChordType.Major13 or ChordType.Minor13 => 6,
        ChordType.Dominant11  or ChordType.Major11 or ChordType.Minor11 => 5,
        ChordType.Dominant9   or ChordType.Major9  or ChordType.Minor9  => 4,
        ChordType.Major7 or ChordType.Minor7 or ChordType.Dominant7
            or ChordType.Diminished7 or ChordType.HalfDiminished7       => 3,
        ChordType.Major or ChordType.Minor or ChordType.Diminished
            or ChordType.Augmented                                       => 2,
        ChordType.Sus2 or ChordType.Sus4                                 => 1,
        _                                                                => 0
    };

    /// <summary>
    /// Interval patterns (semitone offsets from root, always including 0)
    /// for each supported chord type. Patterns are sorted ascending.
    /// </summary>
    private static readonly IReadOnlyDictionary<ChordType, IReadOnlySet<int>> ChordIntervalPatterns =
        new Dictionary<ChordType, IReadOnlySet<int>>
        {
            // Triads
            [ChordType.Major]       = Set(0, 4, 7),
            [ChordType.Minor]       = Set(0, 3, 7),
            [ChordType.Diminished]  = Set(0, 3, 6),
            [ChordType.Augmented]   = Set(0, 4, 8),
            [ChordType.Power5]      = Set(0, 7),

            // Suspended
            [ChordType.Sus2]        = Set(0, 2, 7),
            [ChordType.Sus4]        = Set(0, 5, 7),

            // Seventh chords
            [ChordType.Major7]           = Set(0, 4, 7, 11),
            [ChordType.Minor7]           = Set(0, 3, 7, 10),
            [ChordType.Dominant7]        = Set(0, 4, 7, 10),
            [ChordType.MinorMajor7]      = Set(0, 3, 7, 11),
            [ChordType.HalfDiminished7]  = Set(0, 3, 6, 10),
            [ChordType.Diminished7]      = Set(0, 3, 6, 9),
            [ChordType.AugmentedMajor7]  = Set(0, 4, 8, 11),
            [ChordType.Augmented7]       = Set(0, 4, 8, 10),
            [ChordType.Dominant7Sus4]    = Set(0, 5, 7, 10),

            // Sixth chords
            [ChordType.Major6]      = Set(0, 4, 7, 9),
            [ChordType.Minor6]      = Set(0, 3, 7, 9),

            // Extended chords (dominant)
            [ChordType.Dominant9]   = Set(0, 4, 7, 10, 2),
            [ChordType.Dominant11]  = Set(0, 4, 7, 10, 2, 5),
            [ChordType.Dominant13]  = Set(0, 4, 7, 10, 2, 5, 9),

            // Extended chords (major)
            [ChordType.Major9]      = Set(0, 4, 7, 11, 2),
            [ChordType.Major11]     = Set(0, 4, 7, 11, 2, 5),
            [ChordType.Major13]     = Set(0, 4, 7, 11, 2, 5, 9),

            // Extended chords (minor)
            [ChordType.Minor9]      = Set(0, 3, 7, 10, 2),
            [ChordType.Minor11]     = Set(0, 3, 7, 10, 2, 5),
            [ChordType.Minor13]     = Set(0, 3, 7, 10, 2, 5, 9),

            // Add chords
            [ChordType.MajorAdd9]   = Set(0, 4, 7, 2),
            [ChordType.MinorAdd9]   = Set(0, 3, 7, 2),
        };

    private static IReadOnlySet<int> Set(params int[] values)
        => new HashSet<int>(values);
}

/// <summary>
/// Represents a single chord identification result.
/// </summary>
/// <param name="Root">The identified root note (using the simplest enharmonic spelling).</param>
/// <param name="Type">The chord type that matches the input note set.</param>
/// <param name="ExtraNotes">
/// The number of input notes not required by this chord type's base pattern.
/// 0 = exact match; higher values indicate more tension notes / added colours.
/// </param>
public sealed record ChordMatch(Note Root, ChordType Type, int ExtraNotes)
{
    /// <summary>Creates a <see cref="Chord"/> from this match result.</summary>
    public Chord ToChord() => new(Root, Type);

    /// <summary>Human-readable symbol for this match (e.g. "Cmaj7", "F#m7").</summary>
    public string Symbol => ToChord().GetSymbol();

    public override string ToString() => $"{Symbol} (extra: {ExtraNotes})";
}
