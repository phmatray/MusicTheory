namespace MusicTheory;

/// <summary>Common jazz reharmonisation moves, spelled by interval.</summary>
public static class Reharmonizer
{
    /// <summary>The dominant a tritone away, same type and tensions: G7 gives Db7, B7 gives F7.</summary>
    /// <param name="dominant">A chord containing a major third and a minor seventh.</param>
    /// <returns>The substitute, rooted on whichever of d5 or A4 above needs fewer accidentals (d5 on a tie).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dominant"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="dominant"/> is not a dominant chord.</exception>
    public static ChordSymbol TritoneSubstitute(ChordSymbol dominant)
    {
        ArgumentNullException.ThrowIfNull(dominant);
        var intervals = ChordTypes.Intervals(dominant.Type);
        if (!intervals.Contains(Interval.Parse("M3")) || !intervals.Contains(Interval.Parse("m7")))
        {
            throw new ArgumentException($"{dominant} is not a dominant chord.", nameof(dominant));
        }

        var d5 = dominant.Transpose(Interval.Parse("d5"));
        var a4 = dominant.Transpose(Interval.Parse("A4"));
        return Math.Abs((int)a4.Root.Alteration) < Math.Abs((int)d5.Root.Alteration) ? a4 : d5;
    }
}
