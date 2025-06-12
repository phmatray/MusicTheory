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
    public bool IsTuplet { get; }

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
        if (dots < 0 || dots > 3)
            throw new ArgumentException("Dots must be between 0 and 3.", nameof(dots));

        Type = type;
        Dots = dots;
        IsTuplet = false;
        TupletRatio = null;
    }

    /// <summary>
    /// Private constructor for creating tuplet durations.
    /// </summary>
    private Duration(DurationType type, int tupletCount, int normalCount)
    {
        Type = type;
        Dots = 0;
        IsTuplet = true;
        TupletRatio = (tupletCount, normalCount);
    }

    /// <summary>
    /// Creates a tuplet duration.
    /// </summary>
    /// <param name="baseType">The base duration type.</param>
    /// <param name="tupletCount">Number of notes in the tuplet.</param>
    /// <param name="normalCount">Number of normal notes it replaces.</param>
    /// <returns>A new tuplet duration.</returns>
    public static Duration CreateTuplet(DurationType baseType, int tupletCount, int normalCount)
    {
        return new Duration(baseType, tupletCount, normalCount);
    }

    /// <summary>
    /// Gets the value of this duration in whole notes.
    /// </summary>
    /// <returns>The duration value as a fraction of a whole note.</returns>
    public virtual double GetValueInWholeNotes()
    {
        // Base values for each duration type
        double baseValue = Type switch
        {
            DurationType.Whole => 1.0,
            DurationType.Half => 0.5,
            DurationType.Quarter => 0.25,
            DurationType.Eighth => 0.125,
            DurationType.Sixteenth => 0.0625,
            DurationType.ThirtySecond => 0.03125,
            DurationType.SixtyFourth => 0.015625,
            _ => throw new ArgumentOutOfRangeException()
        };

        // Handle tuplets
        if (IsTuplet && TupletRatio.HasValue)
        {
            var (count, normal) = TupletRatio.Value;
            return baseValue * normal / count;
        }

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
    public double GetTimeInSeconds(int bpm)
    {
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
        string baseSymbol = Type switch
        {
            DurationType.Whole => "𝅝",
            DurationType.Half => "𝅗𝅥",
            DurationType.Quarter => "𝅘𝅥",
            DurationType.Eighth => "𝅘𝅥𝅮",
            DurationType.Sixteenth => "𝅘𝅥𝅯",
            DurationType.ThirtySecond => "𝅘𝅥𝅰",
            DurationType.SixtyFourth => "𝅘𝅥𝅱",
            _ => "?"
        };

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
        string baseName = Type switch
        {
            DurationType.Whole => "whole",
            DurationType.Half => "half",
            DurationType.Quarter => "quarter",
            DurationType.Eighth => "eighth",
            DurationType.Sixteenth => "16th",
            DurationType.ThirtySecond => "32nd",
            DurationType.SixtyFourth => "64th",
            _ => "unknown"
        };

        if (IsTuplet && TupletRatio.HasValue)
        {
            var (count, normal) = TupletRatio.Value;
            return $"{count}:{normal} {baseName}";
        }

        if (Dots == 0)
            return baseName;

        string dotPrefix = Dots switch
        {
            1 => "dotted ",
            2 => "double dotted ",
            3 => "triple dotted ",
            _ => ""
        };

        return dotPrefix + baseName;
    }

    /// <summary>
    /// Determines whether this duration equals another.
    /// </summary>
    /// <param name="other">The other duration.</param>
    /// <returns>True if equal; otherwise, false.</returns>
    public bool Equals(Duration? other)
    {
        if (other is null)
            return false;

        return Type == other.Type && 
               Dots == other.Dots && 
               IsTuplet == other.IsTuplet &&
               TupletRatio == other.TupletRatio;
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
        return HashCode.Combine(Type, Dots, IsTuplet, TupletRatio);
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
    }
}