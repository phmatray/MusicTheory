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
    /// <remarks>The root keeps the spelling it has in <paramref name="notes"/> (Bb stays Bb).</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="notes"/> is null or contains null.</exception>
    public static IEnumerable<ChordMatch> Identify(IEnumerable<Note> notes)
    {
        ArgumentNullException.ThrowIfNull(notes);
        var spelled = notes.Select(n => n ?? throw new ArgumentNullException(nameof(notes), "A note is null.")).ToList();
        // One spelling per pitch class: the first one given.
        var roots = spelled.GroupBy(n => n.PitchClass).Select(g => g.First()).OrderBy(n => n.PitchClass).ToList();
        if (roots.Count < 2)
            return [];

        var matches = new List<ChordMatch>();
        foreach (var root in roots)
        {
            var intervals = roots.Select(n => MusicTheoryUtilities.Mod12(n.PitchClass - root.PitchClass)).ToHashSet();
            foreach (var (type, pattern) in Patterns)
            {
                // Every tone of the pattern must sound; extra notes are allowed (added tensions).
                if (pattern.IsSubsetOf(intervals))
                    matches.Add(new ChordMatch(new Note(root.Name, root.Alteration), type, intervals.Count - pattern.Count));
            }
        }
        return matches;
    }

    /// <summary>
    /// Identifies chords from pitch classes (0 = C … 11 = B, any integer is reduced modulo 12). Black keys
    /// are spelled C#, Eb, F#, Ab, Bb.
    /// </summary>
    public static IEnumerable<ChordMatch> IdentifyFromPitchClasses(IEnumerable<int> pitchClasses)
    {
        ArgumentNullException.ThrowIfNull(pitchClasses);
        return Identify(pitchClasses.Select(pc => Note.FromMidiNumber(60 + MusicTheoryUtilities.Mod12(pc))));
    }

    /// <summary>
    /// The best single match: fewest extra notes, then the richest chord type; null when nothing matches.
    /// </summary>
    public static ChordMatch? BestMatch(IEnumerable<Note> notes)
        => Identify(notes)
            .OrderBy(m => m.ExtraNotes)
            .ThenByDescending(m => ChordComplexity(m.Type))
            .FirstOrDefault();

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

    // Pitch-class sets above the root, derived from the chord-type table. Types that share a set
    // (Augmented7 and Dominant7Sharp5) both match; BestMatch keeps the first.
    private static readonly (ChordType Type, HashSet<int> Pattern)[] Patterns = ChordTypes.All
        .Select(t => (t, ChordTypes.Intervals(t).Select(i => MusicTheoryUtilities.Mod12(i.Semitones)).Append(0).ToHashSet()))
        .ToArray();
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

    /// <summary>The lead-sheet symbol, e.g. <c>Cmaj7</c>, <c>F#m7</c>.</summary>
    public string Symbol => ToChord().GetSymbol();

    /// <summary>The symbol and the extra-note count, e.g. <c>Cmaj7 (extra: 0)</c>.</summary>
    public override string ToString() => $"{Symbol} (extra: {ExtraNotes})";
}
