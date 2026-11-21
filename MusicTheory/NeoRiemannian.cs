namespace MusicTheory;

/// <summary>
/// Neo-Riemannian P, L and R moves on spelled major and minor triads. Each keeps two common tones and moves
/// one voice by a step. Roots are spelled through <see cref="Note.Transpose(Interval)"/>, so
/// <c>R(Eb major)</c> is C minor, never D# or B-flat respelled by semitones.
/// </summary>
public static class NeoRiemannian
{
    /// <summary>Parallel: flips major and minor on the same root. The result is in root position at octave 4.</summary>
    public static Chord P(Chord triad) => Move(triad, "P1", "P1");

    /// <summary>
    /// Leading-tone exchange: major to the minor chord a major third up (C to Em), minor to the major chord a
    /// minor sixth up (Em to C). The result is in root position at octave 4.
    /// </summary>
    public static Chord L(Chord triad) => Move(triad, "M3", "m6");

    /// <summary>
    /// Relative: major to the minor chord a major sixth up (C to Am), minor to the major chord a minor third up
    /// (Am to C). The result is in root position at octave 4.
    /// </summary>
    public static Chord R(Chord triad) => Move(triad, "M6", "m3");

    /// <summary>
    /// Applies the moves left to right: <c>"PLR"</c> is <c>R(L(P(triad)))</c>. Whitespace is ignored and case
    /// does not matter. An empty string returns the triad unchanged.
    /// </summary>
    /// <exception cref="ArgumentException">A character other than P, L, R or whitespace, or a triad that is not a plain major or minor chord.</exception>
    public static Chord Apply(Chord triad, string moves)
    {
        ArgumentNullException.ThrowIfNull(moves);
        foreach (var c in moves.Where(c => !char.IsWhiteSpace(c)))
        {
            triad = char.ToUpperInvariant(c) switch
            {
                'P' => P(triad),
                'L' => L(triad),
                'R' => R(triad),
                _ => throw new ArgumentException($"Unknown neo-Riemannian move '{c}'; use P, L or R.", nameof(moves)),
            };
        }
        return triad;
    }

    private static Chord Move(Chord triad, string fromMajor, string fromMinor)
    {
        ArgumentNullException.ThrowIfNull(triad);
        if (triad.Extensions.Count > 0)
            throw new ArgumentException($"Neo-Riemannian moves apply to plain triads, not {triad}.", nameof(triad));
        var major = triad.Type switch
        {
            ChordType.Major => true,
            ChordType.Minor => false,
            _ => throw new ArgumentException($"Neo-Riemannian moves apply to major and minor triads, not {triad.Type}.", nameof(triad)),
        };
        var root = triad.Root.Transpose(Interval.Parse(major ? fromMajor : fromMinor));
        return new Chord(new Note(root.Name, root.Alteration, 4), major ? ChordType.Minor : ChordType.Major);
    }
}
