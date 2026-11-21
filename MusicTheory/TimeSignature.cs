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
    }

    /// <summary>
    /// Gets the duration of one measure in whole notes, exactly.
    /// </summary>
    public Rational MeasureLength => Rational.Of(Numerator, Denominator);

    /// <summary>
    /// Gets the number of beats per measure.
    /// </summary>
    /// <returns>The number of beats.</returns>
    public int GetBeatsPerMeasure()
    {
        if (IsCompound)
        {
            // Compound meters group beats in threes
            return Numerator / 3;
        }
        
        return Numerator;
    }

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
