namespace MusicTheory.Training;

/// <summary>Which degrees a progression drill draws from, and whether it uses minor keys.</summary>
/// <param name="Degrees">The diatonic degrees (1 to 7) available to the middle of a progression.</param>
/// <param name="MinorKeys">Whether questions may be in a minor key.</param>
/// <param name="Length">The number of chords in a progression (at least 2).</param>
public sealed record ProgressionDrillOptions(IReadOnlyList<int> Degrees, bool MinorKeys, int Length = 4)
{
    /// <summary>The preset for a difficulty level.</summary>
    public static ProgressionDrillOptions For(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Beginner => new([1, 4, 5], false),
        Difficulty.Intermediate => new([1, 2, 4, 5, 6], false),
        Difficulty.Advanced => new([1, 2, 3, 4, 5, 6, 7], true),
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty)),
    };
}

/// <summary>A reproducible Roman-numeral progression drill: the same options and seed replay the same questions.</summary>
public sealed class ProgressionDrill
{
    private readonly ProgressionDrillOptions _options;
    private readonly SeededRandom _random;

    /// <summary>Creates a drill. Throws <see cref="ArgumentException"/> on a length below 2, an empty pool or a degree outside 1..7.</summary>
    public ProgressionDrill(ProgressionDrillOptions options, ulong seed)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Length < 2)
        {
            throw new ArgumentException("A progression needs at least 2 chords.", nameof(options));
        }

        if (options.Degrees.Count == 0)
        {
            throw new ArgumentException("The degree pool must not be empty.", nameof(options));
        }

        if (options.Degrees.Any(d => d is < 1 or > 7))
        {
            throw new ArgumentException("Degrees must be between 1 and 7.", nameof(options));
        }

        _options = options;
        _random = new SeededRandom(seed);
    }

    /// <summary>
    /// The next question. Draw order (a public contract): mode, tonic, then the middle degrees (never repeating
    /// the previous one), then the final degree (1 or 5, never repeating the previous one). Degree 1 is always first.
    /// </summary>
    public Question<IReadOnlyList<int>> Next()
    {
        var minor = _options.MinorKeys && _random.Next(2) == 1;
        var tonic = _random.Pick(minor ? DrillRoots.MinorTonics : DrillRoots.Twelve);

        var degrees = new List<int> { 1 };
        for (var i = 0; i < _options.Length - 2; i++)
        {
            var previous = degrees[^1];
            degrees.Add(_random.Pick(_options.Degrees.Where(d => d != previous).ToList()));
        }

        var last = degrees[^1];
        degrees.Add(_random.Pick(new[] { 1, 5 }.Where(d => d != last).ToList()));

        var key = new KeySignature(tonic, minor ? KeyMode.Minor : KeyMode.Major);
        var progression = new ChordProgression(key);
        IReadOnlyList<IReadOnlyList<Note>> sounds =
            degrees.Select(d => (IReadOnlyList<Note>)progression.GetChordByDegree(d).GetNotes().ToArray()).ToArray();

        return new Question<IReadOnlyList<int>>($"Write the Roman numerals in {key}", sounds, [], degrees, key);
    }

    /// <summary>Checks an answer with partial credit: the share of positions that match the expected degrees.</summary>
    public AnswerResult<IReadOnlyList<int>> Check(Question<IReadOnlyList<int>> question, IReadOnlyList<int> given)
    {
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(given);
        var expected = question.Answer;
        var matches = Enumerable.Range(0, Math.Min(given.Count, expected.Count)).Count(i => given[i] == expected[i]);
        var score = (double)matches / expected.Count;
        return new AnswerResult<IReadOnlyList<int>>(given.Count == expected.Count && score == 1, score, expected);
    }
}
