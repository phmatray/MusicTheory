namespace MusicTheory.Training;

/// <summary>How long a dictated melody is and how it moves.</summary>
/// <param name="Length">The number of notes.</param>
/// <param name="Steps">The scale-degree steps the melody may take between notes.</param>
/// <param name="MinorKeys">Whether questions may be in a minor key.</param>
/// <param name="EndOnTonic">Whether the last note is the tonic or its octave.</param>
public sealed record MelodyDrillOptions(int Length, IReadOnlyList<int> Steps, bool MinorKeys, bool EndOnTonic)
{
    /// <summary>The preset for a difficulty level.</summary>
    public static MelodyDrillOptions For(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Beginner => new(4, [-1, 1], false, true),
        Difficulty.Intermediate => new(6, [-2, -1, 1, 2], true, true),
        Difficulty.Advanced => new(8, [-4, -3, -2, -1, 1, 2, 3, 4], true, false),
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty)),
    };
}

/// <summary>A reproducible melodic-dictation drill: the same options and seed replay the same questions.</summary>
public sealed class MelodyDrill
{
    private readonly MelodyDrillOptions _options;
    private readonly SeededRandom _random;

    /// <summary>
    /// Creates a drill. Throws <see cref="ArgumentException"/> for a length below 2, no steps, or a step of 0
    /// or beyond an octave.
    /// </summary>
    public MelodyDrill(MelodyDrillOptions options, ulong seed)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Length < 2)
        {
            throw new ArgumentException("A melody needs at least 2 notes.", nameof(options));
        }

        if (options.Steps.Count == 0 || options.Steps.Any(s => s == 0 || Math.Abs(s) > 7))
        {
            throw new ArgumentException("Steps must be non-empty, non-zero and at most 7 degrees.", nameof(options));
        }

        _options = options;
        _random = new SeededRandom(seed);
    }

    /// <summary>
    /// The next question. Draw order (a public contract): mode, tonic, then one step per drawn note after the
    /// first; the last note is not drawn when the options end on the tonic.
    /// </summary>
    public Question<IReadOnlyList<Note>> Next()
    {
        var minor = _options.MinorKeys && _random.Next(2) == 1;
        var tonic = _random.Pick(minor ? DrillRoots.MinorTonics : DrillRoots.Twelve);
        var notes = new Scale(tonic, minor ? ScaleType.NaturalMinor : ScaleType.Major).GetNotes().ToArray();

        var drawn = _options.EndOnTonic ? _options.Length - 1 : _options.Length;
        var indices = new List<int> { 0 };
        while (indices.Count < drawn)
        {
            var i = indices[^1] + _random.Pick(_options.Steps);
            indices.Add(i < 0 ? -i : i > 7 ? 14 - i : i);
        }

        if (_options.EndOnTonic)
        {
            indices.Add(indices[^1] >= 4 ? 7 : 0);
        }

        IReadOnlyList<Note> answer = indices.Select(i => notes[i]).ToArray();
        var key = new KeySignature(tonic, minor ? KeyMode.Minor : KeyMode.Major);
        return new Question<IReadOnlyList<Note>>(
            $"Write the melody in {key}",
            answer.Select(n => (IReadOnlyList<Note>)[n]).ToArray(),
            [],
            answer,
            key);
    }

    /// <summary>
    /// Scores each note by ear (same <see cref="Note.MidiNumber"/>, so enharmonic spellings count; the octave
    /// matters). The score is the right notes over the answer length.
    /// </summary>
    public AnswerResult<IReadOnlyList<Note>> Check(Question<IReadOnlyList<Note>> question, IReadOnlyList<Note> given)
    {
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(given);
        var answer = question.Answer;
        var right = Enumerable.Range(0, Math.Min(given.Count, answer.Count))
            .Count(i => given[i].MidiNumber == answer[i].MidiNumber);
        var score = (double)right / answer.Count;
        return new(given.Count == answer.Count && right == answer.Count, score, answer);
    }
}
