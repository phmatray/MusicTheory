namespace MusicTheory.Training;

/// <summary>Which intervals a drill asks, and how.</summary>
/// <param name="Pool">The ascending simple intervals to draw from; also the answer choices.</param>
/// <param name="Descending">Whether questions may sound downwards.</param>
/// <param name="Harmonic">Whether questions may sound both notes together.</param>
public sealed record IntervalDrillOptions(IReadOnlyList<Interval> Pool, bool Descending, bool Harmonic)
{
    /// <summary>The preset for a difficulty level.</summary>
    public static IntervalDrillOptions For(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Beginner => new(Parse("m3 M3 P4 P5 P8"), false, false),
        Difficulty.Intermediate => new(Parse("m2 M2 m3 M3 P4 P5 m6 M6 P8"), true, false),
        Difficulty.Advanced => new(Parse("m2 M2 m3 M3 P4 A4 P5 m6 M6 m7 M7 P8"), true, true),
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty)),
    };

    private static Interval[] Parse(string names) =>
        names.Split(' ').Select(Interval.Parse).ToArray();
}

/// <summary>A reproducible interval ear-training drill: the same options and seed replay the same questions.</summary>
public sealed class IntervalDrill
{
    private readonly IntervalDrillOptions _options;
    private readonly SeededRandom _random;

    /// <summary>Creates a drill. Throws <see cref="ArgumentException"/> when the pool is empty.</summary>
    public IntervalDrill(IntervalDrillOptions options, ulong seed)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Pool.Count == 0)
        {
            throw new ArgumentException("The interval pool must not be empty.", nameof(options));
        }

        _options = options;
        _random = new SeededRandom(seed);
    }

    /// <summary>
    /// The next question. Draw order (a public contract): root, octave, interval, direction, harmonic.
    /// </summary>
    public Question<Interval> Next()
    {
        var r = _random.Pick(DrillRoots.Twelve);
        var root = new Note(r.Name, r.Alteration, 3 + _random.Next(2));
        var interval = _random.Pick(_options.Pool);
        var down = _options.Descending && _random.Next(2) == 1;
        var harmonic = _options.Harmonic && _random.Next(2) == 1;

        var second = root.Transpose(down ? interval.Reverse() : interval);
        IReadOnlyList<IReadOnlyList<Note>> sounds = harmonic
            ? [new[] { root, second }.OrderBy(n => n.MidiNumber).ToArray()]
            : [[root], [second]];

        return new Question<Interval>("Name the interval", sounds, _options.Pool, interval);
    }

    /// <summary>
    /// Checks an answer by ear: same size in semitones once simplified, so <c>d5</c> is right for <c>A4</c>
    /// and direction is ignored.
    /// </summary>
    public AnswerResult<Interval> Check(Question<Interval> question, Interval given)
    {
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(given);
        var correct = given.ToSimple().Semitones == question.Answer.Semitones;
        return new AnswerResult<Interval>(correct, correct ? 1 : 0, question.Answer);
    }
}
