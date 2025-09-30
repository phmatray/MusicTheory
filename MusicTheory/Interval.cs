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
        return MusicTheoryUtilities.GetTotalSemitones(note);
    }

    /// <summary>
    /// Determines the interval quality and creates an interval from semitones and number.
    /// </summary>
    private static Interval DetermineIntervalFromSemitonesAndNumber(int semitones, int number)
    {
        // Get the base interval within an octave (1-7)
        int baseNumber = ((number - 1) % 7) + 1;
        bool isPerfectInterval = baseNumber == 1 || baseNumber == 4 || baseNumber == 5;

        // Expected semitones for perfect/major intervals
        int octaves = (number - 1) / 7;

        // For base intervals 1-7, get semitones from array
        // Note: baseNumber 1 = unison (0 semitones), 2 = second (2), ..., 7 = seventh (11)
        int baseSemitones = baseNumber == 1 ? 0 : MusicTheoryConstants.SemitonesFromC[baseNumber - 1];
        int expectedSemitonesForMajorOrPerfect = baseSemitones + (octaves * MusicTheoryConstants.SemitonesPerOctave);

        // Determine quality based on actual vs expected semitones
        int difference = semitones - expectedSemitonesForMajorOrPerfect;

        // Handle larger differences for compound intervals
        IntervalQuality quality;
        if (isPerfectInterval)
        {
            quality = difference switch
            {
                < -1 => IntervalQuality.Diminished,
                -1 => IntervalQuality.Diminished,
                0 => IntervalQuality.Perfect,
                1 => IntervalQuality.Augmented,
                _ => IntervalQuality.Augmented  // > 1
            };
        }
        else
        {
            quality = difference switch
            {
                < -1 => IntervalQuality.Diminished,
                -1 => IntervalQuality.Minor,
                0 => IntervalQuality.Major,
                1 => IntervalQuality.Augmented,
                _ => IntervalQuality.Augmented  // > 1
            };
        }

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
        int semitones = baseSemitones[baseNumber - 1] + (octaves * MusicTheoryConstants.SemitonesPerOctave);
        
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

    /// <summary>
    /// Determines if this interval is enharmonically equivalent to another interval.
    /// Two intervals are enharmonic if they have the same number of semitones.
    /// </summary>
    /// <param name="other">The other interval to compare.</param>
    /// <returns>True if the intervals are enharmonic; otherwise, false.</returns>
    public bool IsEnharmonicWith(Interval? other)
    {
        if (other == null) return false;
        return Semitones == other.Semitones;
    }
    
    /// <summary>
    /// Inverts the interval according to music theory rules.
    /// </summary>
    /// <returns>The inverted interval.</returns>
    /// <remarks>
    /// Interval inversion follows these rules:
    /// - Number: The sum of an interval and its inversion equals 9 (for simple intervals)
    /// - Quality: Perfect remains Perfect, Major becomes Minor, Minor becomes Major, 
    ///   Augmented becomes Diminished, Diminished becomes Augmented
    /// - For compound intervals (> 8), they are reduced to their simple form before inversion
    /// </remarks>
    public Interval Invert()
    {
        // Reduce compound intervals to simple intervals
        int simpleNumber = Number > 8 ? ((Number - 1) % 7) + 1 : Number;
        
        // Calculate inverted number: original + inverted = 9
        int invertedNumber = 9 - simpleNumber;
        
        // Determine inverted quality
        IntervalQuality invertedQuality = Quality switch
        {
            IntervalQuality.Perfect => IntervalQuality.Perfect,
            IntervalQuality.Major => IntervalQuality.Minor,
            IntervalQuality.Minor => IntervalQuality.Major,
            IntervalQuality.Augmented => IntervalQuality.Diminished,
            IntervalQuality.Diminished => IntervalQuality.Augmented,
            _ => throw new InvalidOperationException($"Unknown interval quality: {Quality}")
        };
        
        return new Interval(invertedQuality, invertedNumber);
    }
}