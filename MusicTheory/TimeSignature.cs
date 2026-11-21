namespace MusicTheory;

/// <summary>
/// Represents a musical time signature (meter).
/// </summary>
public sealed record TimeSignature
{
    /// <summary>
    /// Gets the numerator (top number) of the time signature.
    /// </summary>
    public int Numerator { get; }

    /// <summary>
    /// Gets the denominator (bottom number) of the time signature.
    /// </summary>
    public int Denominator { get; }

    /// <summary>
    /// Gets whether this is a simple time signature.
    /// </summary>
    public bool IsSimple => !IsCompound && !IsComplex;

    /// <summary>
    /// Gets whether this is a compound time signature.
    /// </summary>
    public bool IsCompound => Numerator % 3 == 0 && Numerator > 3;

    /// <summary>
    /// Gets whether this is a complex (odd/irregular) time signature.
    /// </summary>
    public bool IsComplex => Numerator > 3 && Numerator % 2 != 0 && !IsCompound;

    /// <summary>
    /// Common time (4/4).
    /// </summary>
    public static TimeSignature CommonTime => new(4, 4);

    /// <summary>
    /// Cut time (2/2).
    /// </summary>
    public static TimeSignature CutTime => new(2, 2);

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSignature"/> class.
    /// </summary>
    /// <param name="numerator">The numerator (beats per measure).</param>
    /// <param name="denominator">The denominator (note value that gets the beat).</param>
    /// <exception cref="ArgumentException">Thrown when values are invalid.</exception>
    public TimeSignature(int numerator, int denominator)
    {
        if (numerator <= 0)
            throw new ArgumentException("Numerator must be positive.", nameof(numerator));
        
        if (denominator <= 0)
            throw new ArgumentException("Denominator must be positive.", nameof(denominator));
        
        // Check if denominator is a power of 2
        if ((denominator & (denominator - 1)) != 0)
            throw new ArgumentException("Denominator must be a power of 2.", nameof(denominator));

        Numerator = numerator;
        Denominator = denominator;
        BeatGroups = ToRationals(DefaultGroups(numerator, denominator), denominator);
    }

    /// <summary>
    /// Initializes a time signature with an explicit additive grouping, such as 7/8 as 3+2+2.
    /// </summary>
    /// <param name="numerator">The numerator (beats per measure).</param>
    /// <param name="denominator">The denominator (note value that gets the beat).</param>
    /// <param name="beatGroups">The length of each beat group in denominator units; they must sum to <paramref name="numerator"/>.</param>
    /// <exception cref="ArgumentException">Thrown when values are invalid, or the groups are empty, not positive, or do not sum to the numerator.</exception>
    public TimeSignature(int numerator, int denominator, IReadOnlyList<int> beatGroups)
        : this(numerator, denominator)
    {
        ArgumentNullException.ThrowIfNull(beatGroups);
        if (beatGroups.Count == 0 || beatGroups.Any(g => g <= 0) || beatGroups.Sum() != numerator)
            throw new ArgumentException($"Beat groups must be positive and sum to {numerator}.", nameof(beatGroups));

        BeatGroups = ToRationals(beatGroups, denominator);
    }

    /// <summary>
    /// Gets the length of each beat group of the bar in whole notes. Without an explicit grouping the
    /// default follows the meter: one group per beat in simple time, threes in compound time, a single
    /// group for 3/8 and 3/16, and twos then a final three for odd numerators over 8 or 16
    /// (5/8 is 2+3, 7/8 is 2+2+3), as music21 beams them.
    /// </summary>
    public IReadOnlyList<Rational> BeatGroups { get; }

    /// <inheritdoc />
    public bool Equals(TimeSignature? other) =>
        other is not null && Numerator == other.Numerator && Denominator == other.Denominator
        && BeatGroups.SequenceEqual(other.BeatGroups);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Numerator);
        hash.Add(Denominator);
        foreach (var group in BeatGroups)
            hash.Add(group);
        return hash.ToHashCode();
    }

    private static Rational[] ToRationals(IEnumerable<int> groups, int denominator) =>
        groups.Select(g => Rational.Of(g, denominator)).ToArray();

    private static IEnumerable<int> DefaultGroups(int numerator, int denominator)
    {
        if (numerator == 3 && denominator >= 8)
            return [3];
        if (numerator > 3 && numerator % 3 == 0)
            return Enumerable.Repeat(3, numerator / 3);
        if (numerator > 3 && numerator % 2 != 0 && denominator >= 8)
            return Enumerable.Repeat(2, (numerator - 3) / 2).Append(3);
        return Enumerable.Repeat(1, numerator);
    }

    /// <summary>
    /// Gets the duration of one measure in whole notes, exactly.
    /// </summary>
    public Rational MeasureLength => Rational.Of(Numerator, Denominator);

    /// <summary>
    /// Gets the number of beats per measure.
    /// </summary>
    /// <returns>The number of beats.</returns>
    public int GetBeatsPerMeasure() => BeatGroups.Count;

    /// <summary>
    /// Gets the common name for this time signature.
    /// </summary>
    /// <returns>The common name or null if none exists.</returns>
    public string? GetCommonName()
    {
        return (Numerator, Denominator) switch
        {
            (4, 4) => "Common Time",
            (2, 2) => "Cut Time",
            (3, 4) => "Waltz Time",
            (6, 8) => "Compound Duple",
            (9, 8) => "Compound Triple",
            (12, 8) => "Compound Quadruple",
            _ => null
        };
    }

    /// <summary>
    /// Gets the symbol representation of the time signature.
    /// </summary>
    /// <returns>The symbol (C for common time, ¢ for cut time, or fraction).</returns>
    public string GetSymbol()
    {
        return (Numerator, Denominator) switch
        {
            (4, 4) => "C",
            (2, 2) => "¢",
            _ => ToString()
        };
    }

    /// <summary>
    /// Gets the beat unit (what note value gets the beat).
    /// </summary>
    /// <returns>A tuple of (numerator, denominator) representing the beat unit.</returns>
    public (int Numerator, int Denominator) GetBeatUnit()
    {
        if (IsCompound)
        {
            // In compound time, the beat is a dotted note
            // For example, 6/8 has dotted quarter note beats (3/8)
            return (3, Denominator);
        }
        
        // In simple time, the denominator indicates the beat
        return (1, Denominator);
    }

    /// <summary>
    /// Returns a string representation of the time signature.
    /// </summary>
    /// <returns>A string in the format "numerator/denominator".</returns>
    public override string ToString()
    {
        return $"{Numerator}/{Denominator}";
    }
}
