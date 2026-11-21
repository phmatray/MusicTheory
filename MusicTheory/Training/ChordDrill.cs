namespace MusicTheory.Training;

/// <summary>Which chord qualities a drill asks, and whether it inverts them.</summary>
/// <param name="Pool">The chord types to draw from; also the answer choices.</param>
/// <param name="Inversions">Whether questions may sound in inversion.</param>
public sealed record ChordDrillOptions(IReadOnlyList<ChordType> Pool, bool Inversions)
{
    /// <summary>The preset for a difficulty level.</summary>
    public static ChordDrillOptions For(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Beginner => new([ChordType.Major, ChordType.Minor], false),
        Difficulty.Intermediate => new(
            [ChordType.Major, ChordType.Minor, ChordType.Diminished, ChordType.Augmented, ChordType.Dominant7], false),
        Difficulty.Advanced => new(
            [ChordType.Major, ChordType.Minor, ChordType.Diminished, ChordType.Augmented, ChordType.Dominant7,
             ChordType.Major7, ChordType.Minor7, ChordType.HalfDiminished7, ChordType.Diminished7], true),
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty)),
    };
}

/// <summary>A reproducible chord-quality drill: the same options and seed replay the same questions.</summary>
public sealed class ChordDrill
{
    private readonly ChordDrillOptions _options;
    private readonly SeededRandom _random;

    /// <summary>Creates a drill. Throws <see cref="ArgumentException"/> when the pool is empty.</summary>
    public ChordDrill(ChordDrillOptions options, ulong seed)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Pool.Count == 0)
        {
            throw new ArgumentException("The chord pool must not be empty.", nameof(options));
        }

        _options = options;
        _random = new SeededRandom(seed);
    }

    /// <summary>
    /// The next question. Draw order (a public contract): root, octave, type, inversion (only when inversions are on;
    /// 0 to 2 for triads, 0 to 3 for sevenths).
    /// </summary>
    public Question<ChordType> Next()
    {
        var r = _random.Pick(DrillRoots.Twelve);
        var root = new Note(r.Name, r.Alteration, 3 + _random.Next(2));
        var type = _random.Pick(_options.Pool);
        var chord = new Chord(root, type);
        var inversion = _options.Inversions ? _random.Next(chord.GetNotes().Count()) : 0;
        IReadOnlyList<IReadOnlyList<Note>> sounds =
            [chord.WithInversion((ChordInversion)inversion).GetNotesInInversion().ToList()];
        return new Question<ChordType>("Name the chord quality", sounds, _options.Pool, type);
    }

    /// <summary>Checks an answer: the chord type must match exactly.</summary>
    public AnswerResult<ChordType> Check(Question<ChordType> question, ChordType given)
    {
        ArgumentNullException.ThrowIfNull(question);
        var correct = given == question.Answer;
        return new AnswerResult<ChordType>(correct, correct ? 1 : 0, question.Answer);
    }
}
