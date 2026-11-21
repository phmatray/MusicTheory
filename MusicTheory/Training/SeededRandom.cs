namespace MusicTheory.Training;

/// <summary>
/// A deterministic random source (SplitMix64). The same seed always yields the same sequence, on every
/// runtime, which <see cref="System.Random"/> does not guarantee.
/// </summary>
public sealed class SeededRandom(ulong seed)
{
    private ulong _state = seed;

    /// <summary>The next 64 random bits.</summary>
    public ulong NextUInt64()
    {
        unchecked
        {
            _state += 0x9E3779B97F4A7C15UL;
            var z = _state;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
    }

    /// <summary>A number in <c>0..maxExclusive-1</c>. <paramref name="maxExclusive"/> must be at least 1.</summary>
    public int Next(int maxExclusive)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxExclusive, 1);
        // ponytail: multiply-shift has a tiny bias; switch to Lemire's rejection if ranges grow large
        return (int)(((NextUInt64() >> 32) * (ulong)maxExclusive) >> 32);
    }

    /// <summary>A uniformly drawn element. Throws <see cref="ArgumentException"/> on an empty list.</summary>
    public T Pick<T>(IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (items.Count == 0)
        {
            throw new ArgumentException("Cannot pick from an empty list.", nameof(items));
        }

        return items[Next(items.Count)];
    }
}
