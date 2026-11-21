using System.Globalization;

namespace MusicTheory;

/// <summary>
/// An exact fraction of <see cref="long"/> parts, always in lowest terms with the sign on the numerator.
/// Arithmetic is <c>checked</c>: it throws <see cref="OverflowException"/> rather than wrapping.
/// </summary>
/// <remarks>
/// <c>default(Rational)</c> is zero. The denominator is stored minus one so that the default value is valid
/// and equal to <see cref="Zero"/>.
/// </remarks>
// ponytail: long parts, checked arithmetic. Swap to BigInteger if a caller ever overflows.
public readonly record struct Rational : IComparable<Rational>
{
    private readonly long _denominatorLessOne;

    private Rational(long numerator, long denominator)
    {
        Numerator = numerator;
        _denominatorLessOne = denominator - 1;
    }

    /// <summary>The numerator, in lowest terms; it carries the sign.</summary>
    public long Numerator { get; }

    /// <summary>The denominator, in lowest terms; always positive.</summary>
    public long Denominator => _denominatorLessOne + 1;

    /// <summary>The value 0.</summary>
    public static readonly Rational Zero = default;

    /// <summary>The value 1.</summary>
    public static readonly Rational One = Of(1);

    /// <summary>Builds the fraction <paramref name="numerator"/>/<paramref name="denominator"/> in lowest terms.</summary>
    /// <exception cref="DivideByZeroException">The denominator is zero.</exception>
    /// <exception cref="OverflowException">A part is <see cref="long.MinValue"/> and cannot be negated or reduced.</exception>
    public static Rational Of(long numerator, long denominator = 1)
    {
        if (denominator == 0)
        {
            throw new DivideByZeroException("A Rational denominator cannot be zero.");
        }

        if (denominator < 0)
        {
            numerator = checked(-numerator);
            denominator = checked(-denominator);
        }

        var (a, b) = (Math.Abs(numerator), denominator);
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        // a is the gcd of |numerator| and denominator, and at least 1 because the denominator is positive.
        return new Rational(numerator / a, denominator / a);
    }

    /// <summary>Exact sum.</summary>
    public static Rational operator +(Rational a, Rational b) =>
        Of(checked(a.Numerator * b.Denominator + b.Numerator * a.Denominator), checked(a.Denominator * b.Denominator));

    /// <summary>Exact difference.</summary>
    public static Rational operator -(Rational a, Rational b) =>
        Of(checked(a.Numerator * b.Denominator - b.Numerator * a.Denominator), checked(a.Denominator * b.Denominator));

    /// <summary>Exact product.</summary>
    public static Rational operator *(Rational a, Rational b) =>
        Of(checked(a.Numerator * b.Numerator), checked(a.Denominator * b.Denominator));

    /// <summary>Exact quotient.</summary>
    /// <exception cref="DivideByZeroException"><paramref name="b"/> is zero.</exception>
    public static Rational operator /(Rational a, Rational b) =>
        Of(checked(a.Numerator * b.Denominator), checked(a.Denominator * b.Numerator));

    /// <summary>True when <paramref name="a"/> is smaller than <paramref name="b"/>.</summary>
    public static bool operator <(Rational a, Rational b) => a.CompareTo(b) < 0;

    /// <summary>True when <paramref name="a"/> is larger than <paramref name="b"/>.</summary>
    public static bool operator >(Rational a, Rational b) => a.CompareTo(b) > 0;

    /// <summary>True when <paramref name="a"/> is smaller than or equal to <paramref name="b"/>.</summary>
    public static bool operator <=(Rational a, Rational b) => a.CompareTo(b) <= 0;

    /// <summary>True when <paramref name="a"/> is larger than or equal to <paramref name="b"/>.</summary>
    public static bool operator >=(Rational a, Rational b) => a.CompareTo(b) >= 0;

    /// <inheritdoc />
    public int CompareTo(Rational other) =>
        checked(Numerator * other.Denominator).CompareTo(checked(other.Numerator * Denominator));

    /// <summary>The nearest <see cref="double"/>.</summary>
    public double ToDouble() => (double)Numerator / Denominator;

    /// <summary>Formats as <c>3/8</c>, <c>-1/4</c>, or just <c>3</c> when the denominator is 1.</summary>
    public override string ToString() => Denominator == 1
        ? Numerator.ToString(CultureInfo.InvariantCulture)
        : string.Create(CultureInfo.InvariantCulture, $"{Numerator}/{Denominator}");
}
