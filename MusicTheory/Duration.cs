namespace MusicTheory;

/// <summary>
/// Represents the type of note duration.
/// </summary>
public enum DurationType
{
    /// <summary>Whole note (semibreve)</summary>
    Whole,
    /// <summary>Half note (minim)</summary>
    Half,
    /// <summary>Quarter note (crotchet)</summary>
    Quarter,
    /// <summary>Eighth note (quaver)</summary>
    Eighth,
    /// <summary>Sixteenth note (semiquaver)</summary>
    Sixteenth,
    /// <summary>Thirty-second note (demisemiquaver)</summary>
    ThirtySecond,
    /// <summary>Sixty-fourth note (hemidemisemiquaver)</summary>
    SixtyFourth
}

/// <summary>
/// Represents a musical duration (note or rest length).
/// </summary>
public class Duration : IEquatable<Duration>
{
    /// <summary>
    /// Gets the base duration type.
    /// </summary>
    public DurationType Type { get; }

    /// <summary>
    /// Gets the number of dots (0-3).
    /// </summary>
    public int Dots { get; }

    /// <summary>
    /// Gets whether this is a tuplet duration.
    /// </summary>
    public bool IsTuplet => TupletRatio is not null;

    /// <summary>
    /// Gets the tuplet ratio (e.g., 3:2 for triplet).
    /// </summary>
    private (int Count, int Normal)? TupletRatio { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Duration"/> class.
    /// </summary>
    /// <param name="type">The duration type.</param>
    /// <param name="dots">The number of dots (0-3).</param>
    public Duration(DurationType type, int dots = 0)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), type, "Undefined duration type.");
        if (dots < 0 || dots > 3)
            throw new ArgumentException("Dots must be between 0 and 3.", nameof(dots));

        Type = type;
        Dots = dots;
    }

    /// <summary>
    /// Private constructor for creating tuplet durations.
    /// </summary>
    private Duration(DurationType type, int tupletCount, int normalCount)
    {
        Type = type;
        TupletRatio = (tupletCount, normalCount);
    }

    /// <summary>
    /// Creates a tuplet duration.
    /// </summary>
    /// <param name="baseType">The base duration type.</param>
    /// <param name="tupletCount">Number of notes in the tuplet.</param>
    /// <param name="normalCount">Number of normal notes it replaces.</param>
    /// <returns>A new tuplet duration.</returns>
    /// <exception cref="ArgumentOutOfRangeException">An undefined type, or a count below 1.</exception>
    public static Duration CreateTuplet(DurationType baseType, int tupletCount, int normalCount)
    {
        if (!Enum.IsDefined(baseType))
            throw new ArgumentOutOfRangeException(nameof(baseType), baseType, "Undefined duration type.");
        ArgumentOutOfRangeException.ThrowIfLessThan(tupletCount, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(normalCount, 1);
        return new Duration(baseType, tupletCount, normalCount);
    }

    /// <summary>
    /// Gets the value of this duration in whole notes.
    /// </summary>
    /// <returns>The duration value as a fraction of a whole note.</returns>
    public virtual double GetValueInWholeNotes()
    {
        // Whole = 1, half = 1/2, ... sixty-fourth = 1/64.
        double baseValue = 1.0 / (1 << (int)Type);

        // Handle tuplets
        if (TupletRatio is var (count, normal))
            return baseValue * normal / count;

        // Apply dots
        double totalValue = baseValue;
        double dotValue = baseValue;
        
        for (int i = 0; i < Dots; i++)
        {
            dotValue /= 2;
            totalValue += dotValue;
        }

        return totalValue;
    }

    /// <summary>
    /// Gets the time of this duration in seconds at a given tempo.
    /// </summary>
    /// <param name="bpm">The tempo in beats per minute (quarter note = 1 beat).</param>
    /// <returns>The duration in seconds.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bpm"/> is not positive.</exception>
    public double GetTimeInSeconds(int bpm)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(bpm, 1);
        // Calculate seconds per quarter note
        double secondsPerQuarter = 60.0 / bpm;
        
        // Get this duration's value relative to a quarter note
        double valueInQuarters = GetValueInWholeNotes() * 4;
        
        return valueInQuarters * secondsPerQuarter;
    }

    /// <summary>
    /// Gets the Unicode symbol for this duration.
    /// </summary>
    /// <returns>The Unicode music symbol.</returns>
    public string GetSymbol()
    {
        string baseSymbol = Symbols[(int)Type];

        // Add dots
        return baseSymbol + new string('.', Dots);
    }

    /// <summary>
    /// Gets the value of this duration in measures for a given time signature.
    /// </summary>
    /// <param name="timeSignature">The time signature.</param>
    /// <returns>The number of measures this duration spans.</returns>
    public double GetValueInMeasures(TimeSignature timeSignature)
    {
        ArgumentNullException.ThrowIfNull(timeSignature);
        double measureDuration = timeSignature.GetMeasureDuration();
        return GetValueInWholeNotes() / measureDuration;
    }

    /// <summary>
    /// Divides this duration by another duration.
    /// </summary>
    /// <param name="other">The duration to divide by.</param>
    /// <returns>The number of times the other duration fits into this one.</returns>
    public int Divide(Duration other)
    {
        ArgumentNullException.ThrowIfNull(other);
        double thisValue = GetValueInWholeNotes();
        double otherValue = other.GetValueInWholeNotes();
        return (int)Math.Round(thisValue / otherValue);
    }

    /// <summary>
    /// Adds two durations together.
    /// </summary>
    /// <param name="other">The duration to add.</param>
    /// <returns>A new duration representing the sum.</returns>
    public Duration Add(Duration other)
    {
        ArgumentNullException.ThrowIfNull(other);
        double totalValue = GetValueInWholeNotes() + other.GetValueInWholeNotes();
        
        // This is a simplified implementation that returns a duration
        // with the combined value. In practice, this might need to handle
        // more complex scenarios like tied notes.
        return new CompoundDuration(totalValue);
    }

    /// <summary>
    /// Returns a string representation of the duration.
    /// </summary>
    /// <returns>A readable name for the duration.</returns>
    public override string ToString()
    {
        string baseName = Names[(int)Type];
        if (TupletRatio is var (count, normal))
            return $"{count}:{normal} {baseName}";
        return DotPrefixes[Dots] + baseName;
    }

    private static readonly string[] Symbols = ["𝅝", "𝅗𝅥", "𝅘𝅥", "𝅘𝅥𝅮", "𝅘𝅥𝅯", "𝅘𝅥𝅰", "𝅘𝅥𝅱"];
    private static readonly string[] Names = ["whole", "half", "quarter", "eighth", "16th", "32nd", "64th"];
    private static readonly string[] DotPrefixes = ["", "dotted ", "double dotted ", "triple dotted "];

    /// <summary>
    /// Determines whether this duration equals another.
    /// </summary>
    /// <param name="other">The other duration.</param>
    /// <returns>True if equal; otherwise, false.</returns>
    public bool Equals(Duration? other)
    {
        if (other is null)
            return false;

        // A sum (from Add) is equal only to another sum of the same length.
        return GetType() == other.GetType() &&
               Type == other.Type &&
               Dots == other.Dots &&
               TupletRatio == other.TupletRatio &&
               GetValueInWholeNotes() == other.GetValueInWholeNotes();
    }

    /// <summary>
    /// Determines whether this duration equals another object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Duration);
    }

    /// <summary>
    /// Gets the hash code for this duration.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Dots, TupletRatio, GetValueInWholeNotes());
    }

    /// <summary>
    /// Represents a compound duration (sum of multiple durations).
    /// </summary>
    private class CompoundDuration : Duration
    {
        private readonly double _valueInWholeNotes;

        public CompoundDuration(double valueInWholeNotes) : base(DurationType.Whole, 0)
        {
            _valueInWholeNotes = valueInWholeNotes;
        }

        public override double GetValueInWholeNotes() => _valueInWholeNotes;

        public override string ToString() => FormattableString.Invariant($"{_valueInWholeNotes} whole notes");
    }
}