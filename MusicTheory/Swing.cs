namespace MusicTheory;

/// <summary>
/// Swing timing: maps straight score positions to performed positions.
/// </summary>
public static class Swing
{
    /// <summary>
    /// Maps a straight position to its swung position. Pairs of <paramref name="subdivision"/> are swung so the
    /// first takes <paramref name="ratio"/> of the pair's span (0.5 is straight, 2/3 is triplet swing).
    /// On-beat positions do not move.
    /// </summary>
    /// <param name="position">The straight position in whole notes, at least 0.</param>
    /// <param name="subdivision">The length of each half of a pair, in whole notes.</param>
    /// <param name="ratio">The share of the pair taken by its first half, strictly between 0 and 1.</param>
    /// <returns>The performed position in whole notes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A negative position, a non-positive subdivision, or a ratio outside (0, 1).</exception>
    public static double Apply(Rational position, Rational subdivision, double ratio)
    {
        if (position < Rational.Zero)
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position must not be negative.");
        if (subdivision <= Rational.Zero)
            throw new ArgumentOutOfRangeException(nameof(subdivision), subdivision, "Subdivision must be positive.");
        if (!(ratio > 0 && ratio < 1))
            throw new ArgumentOutOfRangeException(nameof(ratio), ratio, "Ratio must be between 0 and 1, exclusive.");

        var pair = subdivision * Rational.Of(2);
        var q = position / pair;
        var origin = pair * Rational.Of(q.Numerator / q.Denominator); // non-negative, so integer division is floor
        var r = position - origin;
        double pairD = pair.ToDouble(), sub = subdivision.ToDouble();
        return r < subdivision
            ? origin.ToDouble() + r.ToDouble() * (ratio * pairD / sub)
            : origin.ToDouble() + ratio * pairD + (r - subdivision).ToDouble() * ((1 - ratio) * pairD / sub);
    }
}
