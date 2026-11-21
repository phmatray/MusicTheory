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

    /// <summary>The ii and V of a target: Cmaj7 gives [Dm7, G7]; Am gives [Bm7b5, E7].</summary>
    /// <param name="target">A major or minor chord; the slash bass is ignored.</param>
    /// <returns>The ii then the V, both in root position.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="target"/> is diminished or augmented.</exception>
    public static IReadOnlyList<ChordSymbol> TwoFiveTo(ChordSymbol target)
    {
        ArgumentNullException.ThrowIfNull(target);
        var quality = target.ToChord().Quality;
        if (quality is ChordQuality.Diminished or ChordQuality.Augmented)
        {
            throw new ArgumentException($"{target} is diminished or augmented and has no ii-V.", nameof(target));
        }

        var ii = quality == ChordQuality.Minor ? ChordType.HalfDiminished7 : ChordType.Minor7;

        return
        [
            new ChordSymbol(target.Root.Transpose(Interval.Parse("M2")), ii),
            new ChordSymbol(target.Root.Transpose(Interval.Parse("P5")), ChordType.Dominant7),
        ];
    }

    /// <summary>A copy of <paramref name="chords"/> with the ii-V of the chord at <paramref name="targetIndex"/> inserted before it.</summary>
    /// <param name="chords">The progression; not modified.</param>
    /// <param name="targetIndex">Index of the chord to approach.</param>
    /// <returns>The longer progression.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chords"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetIndex"/> is outside the list.</exception>
    public static IReadOnlyList<ChordSymbol> InsertTwoFive(IReadOnlyList<ChordSymbol> chords, int targetIndex)
    {
        ArgumentNullException.ThrowIfNull(chords);
        ArgumentOutOfRangeException.ThrowIfNegative(targetIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(targetIndex, chords.Count);
        return [.. chords.Take(targetIndex), .. TwoFiveTo(chords[targetIndex]), .. chords.Skip(targetIndex)];
    }
}
