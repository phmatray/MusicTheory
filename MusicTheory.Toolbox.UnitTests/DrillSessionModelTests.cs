using MusicTheory.Toolbox.Drills;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

public class DrillSessionModelTests
{
    private static DrillSessionModel<Interval> Session(ulong seed, int n)
    {
        var drill = new IntervalDrill(IntervalDrillOptions.For(Difficulty.Beginner), seed);
        return new(Enumerable.Range(0, n).Select(_ => drill.Next()).ToArray(), drill.Check);
    }

    [Fact]
    public void Scores_a_right_then_a_wrong_answer_and_finishes()
    {
        var s = Session(42, 2);
        var first = s.Current!.Answer;
        s.Answer(first).IsCorrect.ShouldBeTrue();
        s.IsFinished.ShouldBeFalse();
        var wrong = s.Current!.Choices.First(c => c.Semitones != s.Current.Answer.Semitones);
        s.Answer(wrong).IsCorrect.ShouldBeFalse();
        s.IsFinished.ShouldBeTrue();
        s.Current.ShouldBeNull();
        s.Score.ShouldBe(1);
        s.Summary.ShouldBe(new DrillSummary(1, 2, 2));
    }

    [Fact]
    public void Answering_a_finished_drill_throws()
    {
        var s = Session(1, 1);
        s.Answer(s.Current!.Answer);
        Should.Throw<InvalidOperationException>(() => s.Answer(Interval.Parse("P5")));
    }

    [Fact]
    public void The_same_seed_replays_the_same_questions() =>
        Session(7, 10).Questions.Select(q => q.Answer.Name)
            .ShouldBe(Session(7, 10).Questions.Select(q => q.Answer.Name));
}
