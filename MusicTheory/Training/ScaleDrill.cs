namespace MusicTheory.Training;

/// <summary>Which scales a drill asks.</summary>
/// <param name="Pool">The scale types to draw from; also the answer choices.</param>
public sealed record ScaleDrillOptions(IReadOnlyList<ScaleType> Pool)
{
    /// <summary>The preset for a difficulty level.</summary>
    public static ScaleDrillOptions For(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Beginner => new([ScaleType.Major, ScaleType.NaturalMinor]),
        Difficulty.Intermediate => new(
            [ScaleType.Major, ScaleType.NaturalMinor, ScaleType.HarmonicMinor, ScaleType.MelodicMinor,
             ScaleType.PentatonicMajor, ScaleType.PentatonicMinor]),
        Difficulty.Advanced => new(
            [ScaleType.Major, ScaleType.NaturalMinor, ScaleType.HarmonicMinor, ScaleType.MelodicMinor,
             ScaleType.PentatonicMajor, ScaleType.PentatonicMinor, ScaleType.Dorian, ScaleType.Phrygian,
             ScaleType.Lydian, ScaleType.Mixolydian, ScaleType.Locrian]),
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty)),
    };
}

/// <summary>A reproducible scale-identification drill: the same options and seed replay the same questions.</summary>
public sealed class ScaleDrill
{
    private readonly ScaleDrillOptions _options;
    private readonly SeededRandom _random;

    /// <summary>Creates a drill. Throws <see cref="ArgumentException"/> when the pool is empty.</summary>
    public ScaleDrill(ScaleDrillOptions options, ulong seed)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Pool.Count == 0)
        {
            throw new ArgumentException("The scale pool must not be empty.", nameof(options));
        }

        _options = options;
        _random = new SeededRandom(seed);
    }

    /// <summary>The next question. Draw order (a public contract): root, octave, type.</summary>
    public Question<ScaleType> Next()
    {
        var r = _random.Pick(DrillRoots.Twelve);
        var root = new Note(r.Name, r.Alteration, 3 + _random.Next(2));
        var type = _random.Pick(_options.Pool);
        IReadOnlyList<IReadOnlyList<Note>> sounds =
            new Scale(root, type).GetNotes().Select(n => (IReadOnlyList<Note>)[n]).ToList();
        return new Question<ScaleType>("Name the scale", sounds, _options.Pool, type);
    }

    /// <summary>Checks an answer by ear: <c>Ionian</c> is accepted for <c>Major</c> and <c>Aeolian</c> for <c>NaturalMinor</c>.</summary>
    public AnswerResult<ScaleType> Check(Question<ScaleType> question, ScaleType given)
    {
        ArgumentNullException.ThrowIfNull(question);
        var correct = Canonical(given) == Canonical(question.Answer);
        return new AnswerResult<ScaleType>(correct, correct ? 1 : 0, question.Answer);
    }

    private static ScaleType Canonical(ScaleType t) => t switch
    {
        ScaleType.Ionian => ScaleType.Major,
        ScaleType.Aeolian => ScaleType.NaturalMinor,
        _ => t,
    };
}
