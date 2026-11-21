using System.Diagnostics.CodeAnalysis;

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
/// Represents a musical duration (note or rest length), exact to the fraction.
/// </summary>
public sealed record Duration
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
    /// Gets the tuplet ratio (actual notes in the space of normal ones, e.g. 3:2 for a triplet), or null.
    /// </summary>
    public (int Actual, int Normal)? Tuplet { get; }

    /// <summary>
    /// Gets whether this is a tuplet duration.
    /// </summary>
    public bool IsTuplet => Tuplet is not null;

    /// <summary>
    /// Gets the exact value of this duration in whole notes.
    /// </summary>
    public Rational Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Duration"/> class.
    /// </summary>
    /// <param name="type">The duration type.</param>
    /// <param name="dots">The number of dots (0-3).</param>
    /// <exception cref="ArgumentOutOfRangeException">An undefined type, or dots outside 0-3.</exception>
    public Duration(DurationType type, int dots = 0)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), type, "Undefined duration type.");
        if (dots < 0 || dots > 3)
            throw new ArgumentOutOfRangeException(nameof(dots), dots, "Dots must be between 0 and 3.");

        Type = type;
        Dots = dots;

        // Whole = 1, half = 1/2, ... sixty-fourth = 1/64; each dot adds half the previous addend.
        var dot = Rational.Of(1, 1 << (int)type);
        var total = dot;
        for (int i = 0; i < dots; i++)
        {
            dot *= Rational.Of(1, 2);
            total += dot;
        }

        Value = total;
    }

    private Duration(DurationType type, int actual, int normal)
    {
        Type = type;
        Tuplet = (actual, normal);
        Value = Rational.Of(1, 1 << (int)type) * Rational.Of(normal, actual);
    }

    /// <summary>
    /// Creates a tuplet duration.
    /// </summary>
    /// <param name="type">The base duration type.</param>
    /// <param name="actual">Number of notes in the tuplet.</param>
    /// <param name="normal">Number of normal notes it replaces.</param>
    /// <returns>A new tuplet duration.</returns>
    /// <exception cref="ArgumentOutOfRangeException">An undefined type, or a count below 1.</exception>
    public static Duration CreateTuplet(DurationType type, int actual, int normal)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), type, "Undefined duration type.");
        ArgumentOutOfRangeException.ThrowIfLessThan(actual, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(normal, 1);
        return new Duration(type, actual, normal);
    }

    /// <summary>
    /// Gets the time of this duration in seconds at a given tempo.
    /// </summary>
    /// <param name="bpm">The tempo in beats per minute.</param>
    /// <param name="beat">The duration that counts as one beat; a quarter note when null.</param>
    /// <returns>The duration in seconds.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bpm"/> is not a positive finite number.</exception>
    public double GetTimeInSeconds(double bpm, Duration? beat = null)
    {
        return new TempoMap(bpm, beat).SecondsAt(Value);
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
    /// Finds the plain or dotted (up to 3 dots) duration of exactly the given value. Tuplets are never returned.
    /// </summary>
    /// <param name="value">The value in whole notes.</param>
    /// <param name="duration">The matching duration, or null.</param>
    /// <returns>True when a notatable duration has this value.</returns>
    public static bool TryFromValue(Rational value, [NotNullWhen(true)] out Duration? duration)
    {
        for (var type = DurationType.Whole; type <= DurationType.SixtyFourth; type++)
        {
            for (int dots = 0; dots <= 3; dots++)
            {
                var candidate = new Duration(type, dots);
                if (candidate.Value == value)
                {
                    duration = candidate;
                    return true;
                }
            }
        }

        duration = null;
        return false;
    }

    /// <summary>
    /// Splits a value into notatable durations, greedily and largest first. Tuplets are never produced.
    /// </summary>
    /// <param name="value">The value in whole notes.</param>
    /// <returns>The durations, whose values sum to <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">The value is not positive or is not a finite sum of binary durations down to a 64th.</exception>
    public static IReadOnlyList<Duration> Decompose(Rational value)
    {
        if (value <= Rational.Zero)
            throw new ArgumentException("Value must be positive.", nameof(value));

        var parts = new List<Duration>();
        var remaining = value;
        while (remaining > Rational.Zero)
        {
            var next = LargestAtMost(remaining)
                ?? throw new ArgumentException($"{value} is not a sum of notatable durations.", nameof(value));
            parts.Add(next);
            remaining -= next.Value;
        }

        return parts;
    }

    private static Duration? LargestAtMost(Rational limit)
    {
        // Longest first: within a type the triple-dotted form is longest, and each type is shorter than the previous one's plain form.
        for (var type = DurationType.Whole; type <= DurationType.SixtyFourth; type++)
        {
            for (int dots = 3; dots >= 0; dots--)
            {
                var candidate = new Duration(type, dots);
                if (candidate.Value <= limit)
                    return candidate;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a string representation of the duration.
    /// </summary>
    /// <returns>A readable name for the duration.</returns>
    public override string ToString()
    {
        string baseName = Names[(int)Type];
        if (Tuplet is var (actual, normal))
            return $"{actual}:{normal} {baseName}";
        return DotPrefixes[Dots] + baseName;
    }

    private static readonly string[] Symbols = ["𝅝", "𝅗𝅥", "𝅘𝅥", "𝅘𝅥𝅮", "𝅘𝅥𝅯", "𝅘𝅥𝅰", "𝅘𝅥𝅱"];
    private static readonly string[] Names = ["whole", "half", "quarter", "eighth", "16th", "32nd", "64th"];
    private static readonly string[] DotPrefixes = ["", "dotted ", "double dotted ", "triple dotted "];
}
