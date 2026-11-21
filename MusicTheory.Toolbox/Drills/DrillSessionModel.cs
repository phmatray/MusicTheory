using MusicTheory.Training;

namespace MusicTheory.Toolbox.Drills;

/// <summary>Right answers out of questions answered so far.</summary>
public sealed record DrillSummary(int Correct, int Answered, int Total);

/// <summary>Progress through a fixed list of questions. Component state only: nothing is stored or sent.</summary>
public sealed class DrillSessionModel<T>(IReadOnlyList<Question<T>> questions, Func<Question<T>, T, AnswerResult<T>> check)
{
    private readonly List<AnswerResult<T>> _results = [];

    public IReadOnlyList<Question<T>> Questions { get; } = questions;
    public IReadOnlyList<AnswerResult<T>> Results => _results;
    public int Index => _results.Count;
    public bool IsFinished => Index >= Questions.Count;
    public Question<T>? Current => IsFinished ? null : Questions[Index];
    public double Score => _results.Sum(r => r.Score);
    public DrillSummary Summary => new(_results.Count(r => r.IsCorrect), _results.Count, Questions.Count);

    /// <summary>Checks <paramref name="given"/> against the current question and moves on. Throws when finished.</summary>
    public AnswerResult<T> Answer(T given)
    {
        if (Current is not { } q) throw new InvalidOperationException("The drill is finished.");
        var result = check(q, given);
        _results.Add(result);
        return result;
    }
}
