namespace MusicTheory;

/// <summary>
/// Represents the quality of a musical interval.
/// </summary>
public enum IntervalQuality
{
    /// <summary>Diminished interval</summary>
    Diminished,
    /// <summary>Minor interval</summary>
    Minor,
    /// <summary>Major interval</summary>
    Major,
    /// <summary>Perfect interval</summary>
    Perfect,
    /// <summary>Augmented interval</summary>
    Augmented
}

/// <summary>
/// Represents a musical interval between two notes.
/// </summary>
public class Interval
{
    /// <summary>
    /// Gets the quality of the interval.
    /// </summary>
    public IntervalQuality Quality { get; }

    /// <summary>
    /// Gets the numeric size of the interval (1 = unison, 2 = second, etc.).
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets the number of semitones in the interval.
    /// </summary>
    public int Semitones => CalculateSemitones();

    /// <summary>
    /// Initializes a new instance of the <see cref="Interval"/> class.
    /// </summary>
    /// <param name="quality">The quality of the interval.</param>
    /// <param name="number">The numeric size of the interval.</param>
    public Interval(IntervalQuality quality, int number)
    {
        Quality = quality;
        Number = number;
    }

    /// <summary>
    /// Creates an interval between two notes.
    /// </summary>
    /// <param name="lowerNote">The lower note.</param>
    /// <param name="higherNote">The higher note.</param>
    /// <returns>The interval between the two notes.</returns>
    public static Interval Between(Note lowerNote, Note higherNote)
    {
        // Calculate the semitone difference
        int lowerSemitones = GetTotalSemitones(lowerNote);
        int higherSemitones = GetTotalSemitones(higherNote);
        int semitoneDifference = higherSemitones - lowerSemitones;

        // Calculate the interval number (considering note names)
        int noteDistance = higherNote.Name - lowerNote.Name;
        if (noteDistance < 0) noteDistance += 7;
        
        int octaveDistance = higherNote.Octave - lowerNote.Octave;
        int intervalNumber = noteDistance + 1 + (octaveDistance * 7);

        // Determine the quality based on the interval number and semitone difference
        return DetermineIntervalFromSemitonesAndNumber(semitoneDifference, intervalNumber);
    }

    /// <summary>
    /// Gets the total semitones from C0 for a given note.
    /// </summary>
    private static int GetTotalSemitones(Note note)
    {
        int[] semitonesFromC = { 0, 2, 4, 5, 7, 9, 11 };
        return note.Octave * 12 + semitonesFromC[(int)note.Name] + (int)note.Alteration;
    }

    /// <summary>
    /// Determines the interval quality and creates an interval from semitones and number.
    /// </summary>
    private static Interval DetermineIntervalFromSemitonesAndNumber(int semitones, int number)
    {
        // Get the base interval within an octave
        int baseNumber = ((number - 1) % 7) + 1;
        bool isPerfectInterval = baseNumber == 1 || baseNumber == 4 || baseNumber == 5 || baseNumber == 8;

        // Expected semitones for perfect/major intervals
        int[] expectedSemitones = { 0, 2, 4, 5, 7, 9, 11 };
        int octaves = (number - 1) / 7;
        int expectedSemitonesForMajorOrPerfect = expectedSemitones[baseNumber - 1] + (octaves * 12);

        // Determine quality based on actual vs expected semitones
        int difference = semitones - expectedSemitonesForMajorOrPerfect;

        IntervalQuality quality = difference switch
        {
            -2 => IntervalQuality.Diminished,
            -1 => isPerfectInterval ? IntervalQuality.Diminished : IntervalQuality.Minor,
            0 => isPerfectInterval ? IntervalQuality.Perfect : IntervalQuality.Major,
            1 => IntervalQuality.Augmented,
            _ => throw new InvalidOperationException($"Cannot determine quality for interval with {semitones} semitones and number {number}")
        };

        return new Interval(quality, number);
    }

    /// <summary>
    /// Calculates the number of semitones in the interval.
    /// </summary>
    /// <returns>The number of semitones.</returns>
    private int CalculateSemitones()
    {
        // Base semitones for perfect/major intervals (1-8)
        int[] baseSemitones = { 0, 2, 4, 5, 7, 9, 11, 12 };
        
        // Get the base interval within an octave (1-8)
        int baseNumber = ((Number - 1) % 7) + 1;
        
        // Calculate octaves above the base
        int octaves = (Number - 1) / 7;
        
        // Get base semitones for this interval number
        int semitones = baseSemitones[baseNumber - 1] + (octaves * 12);
        
        // Adjust for quality
        bool isPerfectInterval = baseNumber == 1 || baseNumber == 4 || baseNumber == 5 || baseNumber == 8;
        
        return Quality switch
        {
            IntervalQuality.Diminished => isPerfectInterval ? semitones - 1 : semitones - 2,
            IntervalQuality.Minor => isPerfectInterval ? 
                throw new InvalidOperationException($"Interval {Number} cannot be minor") : 
                semitones - 1,
            IntervalQuality.Major => isPerfectInterval ? 
                throw new InvalidOperationException($"Interval {Number} cannot be major") : 
                semitones,
            IntervalQuality.Perfect => isPerfectInterval ? 
                semitones : 
                throw new InvalidOperationException($"Interval {Number} cannot be perfect"),
            IntervalQuality.Augmented => semitones + 1,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}