namespace MusicTheory;

/// <summary>
/// Represents the type of motion between two voices.
/// </summary>
public enum MotionType
{
    /// <summary>Both voices move in the same direction by the same interval</summary>
    Parallel,
    /// <summary>Both voices move in the same direction by different intervals</summary>
    Similar,
    /// <summary>Voices move in opposite directions</summary>
    Contrary,
    /// <summary>One voice stays while the other moves</summary>
    Oblique
}

/// <summary>
/// Represents the type of voice leading violation.
/// </summary>
public enum VoiceLeadingViolationType
{
    /// <summary>Parallel perfect fifths between two voices</summary>
    ParallelFifths,
    /// <summary>Parallel perfect octaves between two voices</summary>
    ParallelOctaves,
    /// <summary>Parallel unisons between two voices</summary>
    ParallelUnisons,
    /// <summary>Hidden (direct) fifths in outer voices</summary>
    HiddenFifths,
    /// <summary>Hidden (direct) octaves in outer voices</summary>
    HiddenOctaves,
    /// <summary>Voice crossing (lower voice above higher voice)</summary>
    VoiceCrossing,
    /// <summary>Voice overlap (voice moves into range of previous note in another voice)</summary>
    VoiceOverlap,
    /// <summary>Augmented interval in melodic line</summary>
    AugmentedInterval,
    /// <summary>Large melodic leap (greater than an octave)</summary>
    LargeMelodicLeap,
    /// <summary>Unresolved leading tone</summary>
    UnresolvedLeadingTone
}

/// <summary>
/// Represents a voice leading violation.
/// </summary>
/// <param name="Type">The kind of violation.</param>
/// <param name="Description">A readable description.</param>
/// <param name="VoiceIndices">The two voices involved, or null for a single-voice violation.</param>
public sealed record VoiceLeadingViolation(VoiceLeadingViolationType Type, string Description, (int Voice1, int Voice2)? VoiceIndices = null);

/// <summary>
/// Common-practice part-writing checks. Voices are given note by note: the first chord's note, then the
/// second chord's, for each voice. Every check is spelling-aware (a diminished 5th is not a perfect 5th).
/// </summary>
public static class VoiceLeadingRules
{
    /// <summary>Parallel perfect fifths (or compound fifths) between two voices.</summary>
    public static bool HasParallelFifths(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2) =>
        IsPerfect(voice1Note1, voice2Note1, 5) && IsPerfect(voice1Note2, voice2Note2, 5)
        && GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2) == MotionType.Parallel;

    /// <summary>Parallel perfect octaves (or compound octaves) between two voices; two unisons are not octaves.</summary>
    public static bool HasParallelOctaves(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2) =>
        IsPerfect(voice1Note1, voice2Note1, 1) && IsPerfect(voice1Note2, voice2Note2, 1)
        && !(Interval.Between(voice1Note1, voice2Note1).Number == 1 && Interval.Between(voice1Note2, voice2Note2).Number == 1)
        && GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2) == MotionType.Parallel;

    /// <summary>Parallel perfect unisons between two voices.</summary>
    public static bool HasParallelUnisons(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2) =>
        Interval.Between(voice1Note1, voice2Note1) == Unison && Interval.Between(voice1Note2, voice2Note2) == Unison
        && GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2) == MotionType.Parallel;

    /// <summary>
    /// Hidden (direct) fifths in the outer voices: similar or parallel motion into a perfect fifth with a
    /// leap of more than a whole step in the soprano.
    /// </summary>
    public static bool HasHiddenFifths(Note bassNote1, Note bassNote2, Note sopranoNote1, Note sopranoNote2) =>
        IsPerfect(bassNote2, sopranoNote2, 5)
        && GetMotionType(bassNote1, bassNote2, sopranoNote1, sopranoNote2) is MotionType.Similar or MotionType.Parallel
        && Math.Abs(Pitch(sopranoNote2, nameof(sopranoNote2)) - Pitch(sopranoNote1, nameof(sopranoNote1))) > 2;

    /// <summary>The lower voice sounds above the higher one.</summary>
    public static bool HasVoiceCrossing(Note lowerVoiceNote, Note higherVoiceNote) =>
        Pitch(lowerVoiceNote, nameof(lowerVoiceNote)) > Pitch(higherVoiceNote, nameof(higherVoiceNote));

    /// <summary>A voice moves past the previous note of the other voice (voice 1 is the lower one).</summary>
    public static bool HasVoiceOverlap(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2)
    {
        int low1 = Pitch(voice1Note1, nameof(voice1Note1)), low2 = Pitch(voice1Note2, nameof(voice1Note2));
        int high1 = Pitch(voice2Note1, nameof(voice2Note1)), high2 = Pitch(voice2Note2, nameof(voice2Note2));
        return low1 < high1 && (low2 > high1 || high2 < low1);
    }

    /// <summary>An augmented melodic interval, of any degree, in either direction.</summary>
    public static bool HasAugmentedInterval(Note note1, Note note2) => Interval.Between(note1, note2).IsAugmented;

    /// <summary>A melodic leap wider than <paramref name="maxSemitones"/> (an octave by default).</summary>
    public static bool HasLargeMelodicLeap(Note note1, Note note2, int maxSemitones = 12) =>
        Math.Abs(Pitch(note2, nameof(note2)) - Pitch(note1, nameof(note1))) > maxSemitones;

    /// <summary>
    /// The motion between two voices: oblique when one (or neither) moves, contrary in opposite directions,
    /// parallel by the same distance, similar otherwise.
    /// </summary>
    public static MotionType GetMotionType(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2)
    {
        int move1 = Pitch(voice1Note2, nameof(voice1Note2)) - Pitch(voice1Note1, nameof(voice1Note1));
        int move2 = Pitch(voice2Note2, nameof(voice2Note2)) - Pitch(voice2Note1, nameof(voice2Note1));
        if (move1 == 0 || move2 == 0)
            return MotionType.Oblique;
        if (Math.Sign(move1) != Math.Sign(move2))
            return MotionType.Contrary;
        return move1 == move2 ? MotionType.Parallel : MotionType.Similar;
    }

    /// <summary>
    /// Every violation between two chords given bass to soprano: parallel fifths, octaves and unisons,
    /// crossing and overlap for each pair of voices; augmented intervals and leaps over an octave in each
    /// voice; hidden fifths in the outer voices.
    /// </summary>
    /// <exception cref="ArgumentNullException">A chord is null.</exception>
    /// <exception cref="ArgumentException">The chords have different voice counts.</exception>
    public static IReadOnlyList<VoiceLeadingViolation> ValidateVoiceLeading(Note[] chord1, Note[] chord2)
    {
        ArgumentNullException.ThrowIfNull(chord1);
        ArgumentNullException.ThrowIfNull(chord2);
        if (chord1.Length != chord2.Length)
            throw new ArgumentException("Both chords must have the same number of voices.", nameof(chord2));

        var violations = new List<VoiceLeadingViolation>();
        void Pair(bool found, VoiceLeadingViolationType type, string what, int i, int j)
        {
            if (found)
                violations.Add(new VoiceLeadingViolation(type, $"{what} between voice {i} and voice {j}", (i, j)));
        }
        void Single(bool found, VoiceLeadingViolationType type, string what, int i)
        {
            if (found)
                violations.Add(new VoiceLeadingViolation(type, $"{what} in voice {i}"));
        }

        for (int i = 0; i < chord1.Length; i++)
        {
            for (int j = i + 1; j < chord1.Length; j++)
            {
                Pair(HasParallelFifths(chord1[i], chord2[i], chord1[j], chord2[j]), VoiceLeadingViolationType.ParallelFifths, "Parallel fifths", i, j);
                Pair(HasParallelOctaves(chord1[i], chord2[i], chord1[j], chord2[j]), VoiceLeadingViolationType.ParallelOctaves, "Parallel octaves", i, j);
                Pair(HasParallelUnisons(chord1[i], chord2[i], chord1[j], chord2[j]), VoiceLeadingViolationType.ParallelUnisons, "Parallel unisons", i, j);
                Pair(HasVoiceCrossing(chord2[i], chord2[j]), VoiceLeadingViolationType.VoiceCrossing, "Voice crossing", i, j);
                Pair(HasVoiceOverlap(chord1[i], chord2[i], chord1[j], chord2[j]), VoiceLeadingViolationType.VoiceOverlap, "Voice overlap", i, j);
            }
            Single(HasAugmentedInterval(chord1[i], chord2[i]), VoiceLeadingViolationType.AugmentedInterval, "Augmented interval", i);
            Single(HasLargeMelodicLeap(chord1[i], chord2[i]), VoiceLeadingViolationType.LargeMelodicLeap, "Large melodic leap", i);
        }

        int soprano = chord1.Length - 1;
        if (soprano > 0 && HasHiddenFifths(chord1[0], chord2[0], chord1[soprano], chord2[soprano]))
            violations.Add(new VoiceLeadingViolation(VoiceLeadingViolationType.HiddenFifths, "Hidden fifths in outer voices", (0, soprano)));
        return violations;
    }

    private static readonly Interval Unison = new(IntervalQuality.Perfect, 1);

    // A perfect interval whose number reduces to `simple` (1 for unisons and octaves, 5 for fifths).
    private static bool IsPerfect(Note a, Note b, int simple)
    {
        var interval = Interval.Between(a, b);
        return interval.Quality == IntervalQuality.Perfect && (interval.Number - 1) % 7 + 1 == simple;
    }

    private static int Pitch(Note note, string name) =>
        note?.Semitones ?? throw new ArgumentNullException(name);
}
