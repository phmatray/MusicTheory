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
public class VoiceLeadingViolation
{
    /// <summary>
    /// Gets the type of violation.
    /// </summary>
    public VoiceLeadingViolationType Type { get; }

    /// <summary>
    /// Gets a description of the violation.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the voice indices involved in the violation.
    /// </summary>
    public (int Voice1, int Voice2)? VoiceIndices { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VoiceLeadingViolation"/> class.
    /// </summary>
    public VoiceLeadingViolation(VoiceLeadingViolationType type, string description, (int, int)? voiceIndices = null)
    {
        Type = type;
        Description = description;
        VoiceIndices = voiceIndices;
    }
}

/// <summary>
/// Provides voice leading rules and validation for part-writing.
/// </summary>
public static class VoiceLeadingRules
{
    /// <summary>
    /// Checks if there are parallel perfect fifths between two voices.
    /// </summary>
    public static bool HasParallelFifths(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2)
    {
        var interval1 = Interval.Between(voice1Note1, voice2Note1);
        var interval2 = Interval.Between(voice1Note2, voice2Note2);

        // Check if both intervals are perfect fifths
        if (interval1.Quality != IntervalQuality.Perfect || interval2.Quality != IntervalQuality.Perfect)
            return false;

        var simpleNumber1 = ((interval1.Number - 1) % 7) + 1;
        var simpleNumber2 = ((interval2.Number - 1) % 7) + 1;

        if (simpleNumber1 != 5 || simpleNumber2 != 5)
            return false;

        // Check if both voices move in the same direction
        var motion = GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2);
        return motion == MotionType.Parallel;
    }

    /// <summary>
    /// Checks if there are parallel perfect octaves between two voices.
    /// </summary>
    public static bool HasParallelOctaves(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2)
    {
        var interval1 = Interval.Between(voice1Note1, voice2Note1);
        var interval2 = Interval.Between(voice1Note2, voice2Note2);

        // Check if both intervals are perfect octaves (or compound octaves)
        if (interval1.Quality != IntervalQuality.Perfect || interval2.Quality != IntervalQuality.Perfect)
            return false;

        var simpleNumber1 = ((interval1.Number - 1) % 7) + 1;
        var simpleNumber2 = ((interval2.Number - 1) % 7) + 1;

        if (simpleNumber1 != 1 || simpleNumber2 != 1)
            return false;

        // Both must be octaves (not unisons in same octave)
        if (interval1.Number == 1 && interval2.Number == 1)
            return false;

        // Check if both voices move in the same direction
        var motion = GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2);
        return motion == MotionType.Parallel;
    }

    /// <summary>
    /// Checks if there are parallel unisons between two voices.
    /// </summary>
    public static bool HasParallelUnisons(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2)
    {
        var interval1 = Interval.Between(voice1Note1, voice2Note1);
        var interval2 = Interval.Between(voice1Note2, voice2Note2);

        // Check if both intervals are perfect unisons
        if (interval1.Quality != IntervalQuality.Perfect || interval2.Quality != IntervalQuality.Perfect)
            return false;

        if (interval1.Number != 1 || interval2.Number != 1)
            return false;

        // Check if both voices move in the same direction
        var motion = GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2);
        return motion == MotionType.Parallel;
    }

    /// <summary>
    /// Checks if there are hidden (direct) fifths between outer voices.
    /// </summary>
    public static bool HasHiddenFifths(Note bassNote1, Note bassNote2, Note sopranoNote1, Note sopranoNote2)
    {
        // Determine the interval at the destination
        var lowerNote = bassNote2;
        var higherNote = sopranoNote2;

        // Ensure we're calculating from lower to higher
        if (MusicTheoryUtilities.GetTotalSemitones(bassNote2) > MusicTheoryUtilities.GetTotalSemitones(sopranoNote2))
        {
            lowerNote = sopranoNote2;
            higherNote = bassNote2;
        }

        var interval2 = Interval.Between(lowerNote, higherNote);

        // Check if the resulting interval is a perfect fifth (or compound fifth)
        if (interval2.Quality != IntervalQuality.Perfect)
            return false;

        var simpleNumber = ((interval2.Number - 1) % 7) + 1;
        if (simpleNumber != 5)
            return false;

        // Check if both voices move in similar or parallel motion (same direction)
        var motion = GetMotionType(bassNote1, bassNote2, sopranoNote1, sopranoNote2);
        if (motion != MotionType.Similar && motion != MotionType.Parallel)
            return false;

        // Hidden fifths are particularly problematic when the soprano leaps
        int sopranoSemitones1 = MusicTheoryUtilities.GetTotalSemitones(sopranoNote1);
        int sopranoSemitones2 = MusicTheoryUtilities.GetTotalSemitones(sopranoNote2);
        int sopranoLeap = Math.Abs(sopranoSemitones2 - sopranoSemitones1);

        return sopranoLeap > 2; // More than a whole step
    }

    /// <summary>
    /// Checks if there is voice crossing between two voices.
    /// </summary>
    public static bool HasVoiceCrossing(Note lowerVoiceNote, Note higherVoiceNote)
    {
        var lowerSemitones = MusicTheoryUtilities.GetTotalSemitones(lowerVoiceNote);
        var higherSemitones = MusicTheoryUtilities.GetTotalSemitones(higherVoiceNote);

        return lowerSemitones > higherSemitones;
    }

    /// <summary>
    /// Checks if there is voice overlap between two voices.
    /// Voice overlap occurs when a voice moves past the previous note of an adjacent voice.
    /// </summary>
    public static bool HasVoiceOverlap(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2)
    {
        var voice1Semitones1 = MusicTheoryUtilities.GetTotalSemitones(voice1Note1);
        var voice1Semitones2 = MusicTheoryUtilities.GetTotalSemitones(voice1Note2);
        var voice2Semitones1 = MusicTheoryUtilities.GetTotalSemitones(voice2Note1);
        var voice2Semitones2 = MusicTheoryUtilities.GetTotalSemitones(voice2Note2);

        // Check if voice1 (lower) moves above where voice2 (higher) was
        if (voice1Semitones1 < voice2Semitones1 && voice1Semitones2 > voice2Semitones1)
            return true;

        // Check if voice2 (higher) moves below where voice1 (lower) was
        if (voice2Semitones1 > voice1Semitones1 && voice2Semitones2 < voice1Semitones1)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if there is an augmented interval in a melodic line.
    /// </summary>
    public static bool HasAugmentedInterval(Note note1, Note note2)
    {
        // Ensure we calculate from lower to higher note
        var lowerNote = note1;
        var higherNote = note2;

        if (MusicTheoryUtilities.GetTotalSemitones(note1) > MusicTheoryUtilities.GetTotalSemitones(note2))
        {
            lowerNote = note2;
            higherNote = note1;
        }

        var interval = Interval.Between(lowerNote, higherNote);
        return interval.Quality == IntervalQuality.Augmented;
    }

    /// <summary>
    /// Checks if there is a large melodic leap (greater than an octave).
    /// </summary>
    public static bool HasLargeMelodicLeap(Note note1, Note note2, int maxSemitones = 12)
    {
        // Calculate semitone distance regardless of direction
        int semitones1 = MusicTheoryUtilities.GetTotalSemitones(note1);
        int semitones2 = MusicTheoryUtilities.GetTotalSemitones(note2);
        int distance = Math.Abs(semitones2 - semitones1);

        return distance > maxSemitones;
    }

    /// <summary>
    /// Gets the type of motion between two voices.
    /// </summary>
    public static MotionType GetMotionType(Note voice1Note1, Note voice1Note2, Note voice2Note1, Note voice2Note2)
    {
        var voice1Semitones1 = MusicTheoryUtilities.GetTotalSemitones(voice1Note1);
        var voice1Semitones2 = MusicTheoryUtilities.GetTotalSemitones(voice1Note2);
        var voice2Semitones1 = MusicTheoryUtilities.GetTotalSemitones(voice2Note1);
        var voice2Semitones2 = MusicTheoryUtilities.GetTotalSemitones(voice2Note2);

        var voice1Movement = voice1Semitones2 - voice1Semitones1;
        var voice2Movement = voice2Semitones2 - voice2Semitones1;

        // Oblique: one voice stays, the other moves
        if (voice1Movement == 0 && voice2Movement != 0)
            return MotionType.Oblique;
        if (voice2Movement == 0 && voice1Movement != 0)
            return MotionType.Oblique;

        // No motion
        if (voice1Movement == 0 && voice2Movement == 0)
            return MotionType.Oblique;

        // Contrary: voices move in opposite directions
        if ((voice1Movement > 0 && voice2Movement < 0) || (voice1Movement < 0 && voice2Movement > 0))
            return MotionType.Contrary;

        // Parallel: same direction and same interval
        if (Math.Abs(voice1Movement) == Math.Abs(voice2Movement))
            return MotionType.Parallel;

        // Similar: same direction but different intervals
        return MotionType.Similar;
    }

    /// <summary>
    /// Validates voice leading between two chords and returns all violations.
    /// </summary>
    /// <param name="chord1">The first chord (array of notes from bass to soprano).</param>
    /// <param name="chord2">The second chord (array of notes from bass to soprano).</param>
    /// <returns>A list of voice leading violations.</returns>
    public static List<VoiceLeadingViolation> ValidateVoiceLeading(Note[] chord1, Note[] chord2)
    {
        if (chord1.Length != chord2.Length)
            throw new ArgumentException("Both chords must have the same number of voices.");

        var violations = new List<VoiceLeadingViolation>();
        int voiceCount = chord1.Length;

        // Check all voice pairs
        for (int i = 0; i < voiceCount; i++)
        {
            for (int j = i + 1; j < voiceCount; j++)
            {
                // Check parallel fifths
                if (HasParallelFifths(chord1[i], chord2[i], chord1[j], chord2[j]))
                {
                    violations.Add(new VoiceLeadingViolation(
                        VoiceLeadingViolationType.ParallelFifths,
                        $"Parallel fifths between voice {i} and voice {j}",
                        (i, j)));
                }

                // Check parallel octaves
                if (HasParallelOctaves(chord1[i], chord2[i], chord1[j], chord2[j]))
                {
                    violations.Add(new VoiceLeadingViolation(
                        VoiceLeadingViolationType.ParallelOctaves,
                        $"Parallel octaves between voice {i} and voice {j}",
                        (i, j)));
                }

                // Check parallel unisons
                if (HasParallelUnisons(chord1[i], chord2[i], chord1[j], chord2[j]))
                {
                    violations.Add(new VoiceLeadingViolation(
                        VoiceLeadingViolationType.ParallelUnisons,
                        $"Parallel unisons between voice {i} and voice {j}",
                        (i, j)));
                }

                // Check voice crossing
                if (HasVoiceCrossing(chord2[i], chord2[j]))
                {
                    violations.Add(new VoiceLeadingViolation(
                        VoiceLeadingViolationType.VoiceCrossing,
                        $"Voice crossing between voice {i} and voice {j}",
                        (i, j)));
                }

                // Check voice overlap
                if (HasVoiceOverlap(chord1[i], chord2[i], chord1[j], chord2[j]))
                {
                    violations.Add(new VoiceLeadingViolation(
                        VoiceLeadingViolationType.VoiceOverlap,
                        $"Voice overlap between voice {i} and voice {j}",
                        (i, j)));
                }
            }

            // Check melodic intervals for each voice
            if (HasAugmentedInterval(chord1[i], chord2[i]))
            {
                violations.Add(new VoiceLeadingViolation(
                    VoiceLeadingViolationType.AugmentedInterval,
                    $"Augmented interval in voice {i}",
                    null));
            }

            if (HasLargeMelodicLeap(chord1[i], chord2[i]))
            {
                violations.Add(new VoiceLeadingViolation(
                    VoiceLeadingViolationType.LargeMelodicLeap,
                    $"Large melodic leap in voice {i}",
                    null));
            }
        }

        // Check hidden fifths and octaves in outer voices (bass and soprano)
        if (voiceCount >= 2)
        {
            int bassIndex = 0;
            int sopranoIndex = voiceCount - 1;

            if (HasHiddenFifths(chord1[bassIndex], chord2[bassIndex], chord1[sopranoIndex], chord2[sopranoIndex]))
            {
                violations.Add(new VoiceLeadingViolation(
                    VoiceLeadingViolationType.HiddenFifths,
                    "Hidden fifths in outer voices",
                    (bassIndex, sopranoIndex)));
            }
        }

        return violations;
    }
}
