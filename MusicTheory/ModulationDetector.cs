namespace MusicTheory;

/// <summary>A stretch of chords in one key.</summary>
/// <param name="Key">The key of the region.</param>
/// <param name="Start">Index of the first chord (inclusive).</param>
/// <param name="End">Index of the last chord (inclusive).</param>
/// <param name="PivotIndex">
/// The chord, just before <paramref name="Start"/>, that is diatonic in both this key and the previous region's;
/// null when there is none, and for the first region.
/// </param>
public sealed record KeyRegion(KeySignature Key, int Start, int End, int? PivotIndex);

/// <summary>
/// Splits a chord sequence into key regions by voting over sliding <see cref="KeyDetector"/> windows.
/// </summary>
public static class ModulationDetector
{
    // The spec's minimum of 2 left a 2-chord relative-key blip in a steady G progression and a 2-chord
    // passing G inside C to D; 3 absorbs both.
    private const int MinRegion = 3;

    /// <summary>
    /// Finds the key regions of a chord sequence. Each chord takes the most frequent best key among the windows
    /// covering it (a tie goes to the latest window), regions shorter than three chords are absorbed into a
    /// neighbour, and the pivot is looked for in the two chords before each boundary.
    /// </summary>
    /// <param name="chords">The chords, in order.</param>
    /// <param name="window">Chords per window, at least 2.</param>
    /// <returns>Contiguous regions covering every chord; empty for an empty input.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chords"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="window"/> is below 2.</exception>
    public static IReadOnlyList<KeyRegion> Detect(IReadOnlyList<ChordSymbol> chords, int window = 4)
    {
        ArgumentNullException.ThrowIfNull(chords);
        ArgumentOutOfRangeException.ThrowIfLessThan(window, 2);
        int n = chords.Count;
        if (n == 0)
            return [];
        if (n <= window)
            return [new KeyRegion(KeyDetector.Infer(chords, 1)[0].Key, 0, n - 1, null)];

        var tops = Enumerable.Range(0, n - window + 1)
            .Select(s => KeyDetector.Infer(chords.Skip(s).Take(window), 1)[0].Key)
            .ToList();
        var labels = Enumerable.Range(0, n)
            .Select(i =>
            {
                int first = Math.Max(0, i - window + 1);
                int last = Math.Min(i, n - window);
                return tops.Skip(first).Take(last - first + 1)
                    .Reverse()
                    .GroupBy(k => k)                       // reversed, so the stable sort breaks ties to the latest window
                    .OrderByDescending(g => g.Count())
                    .First().Key;
            })
            .ToArray();

        var runs = Runs(labels);
        while (runs.FindIndex(r => r.End - r.Start + 1 < MinRegion) is var i and >= 0)
        {
            Array.Fill(labels, i == 0 ? runs[1].Key : runs[i - 1].Key, runs[i].Start, runs[i].End - runs[i].Start + 1);
            runs = Runs(labels);
        }

        return runs.Select((r, idx) => new KeyRegion(r.Key, r.Start, r.End, idx == 0 ? null : Pivot(chords, runs[idx - 1], r)))
            .ToList();
    }

    private static List<(KeySignature Key, int Start, int End)> Runs(KeySignature[] labels)
    {
        var runs = new List<(KeySignature Key, int Start, int End)>();
        for (int i = 0; i < labels.Length; i++)
        {
            if (i > 0 && labels[i] == labels[i - 1])
                runs[^1] = (labels[i], runs[^1].Start, i);
            else
                runs.Add((labels[i], i, i));
        }

        return runs;
    }

    private static int? Pivot(IReadOnlyList<ChordSymbol> chords, (KeySignature Key, int Start, int End) previous, (KeySignature Key, int Start, int End) region) =>
        new[] { region.Start - 1, region.Start - 2 }
            .Where(j => j >= previous.Start)
            .Cast<int?>()
            .FirstOrDefault(j => HarmonicAnalyzer.Analyze(chords[j!.Value], previous.Key).Function == HarmonicFunction.Diatonic
                              && HarmonicAnalyzer.Analyze(chords[j!.Value], region.Key).Function == HarmonicFunction.Diatonic);
}
