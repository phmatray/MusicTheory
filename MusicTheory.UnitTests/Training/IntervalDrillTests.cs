using MusicTheory.Training;
using Shouldly;

namespace MusicTheory.UnitTests.Training;

public class IntervalDrillTests
{
    private static string Describe(Question<Interval> q) =>
        q.Prompt + "|" + string.Join(" ", q.Sounds.Select(s => string.Join("+", s))) + "|" + q.Answer.Name;

    private static List<Question<Interval>> Draw(Difficulty d, ulong seed, int n)
    {
        var drill = new IntervalDrill(IntervalDrillOptions.For(d), seed);
        return Enumerable.Range(0, n).Select(_ => drill.Next()).ToList();
    }

    [Fact]
    public void SameSeed_ReplaysSameQuestions()
    {
        Draw(Difficulty.Advanced, 7, 20).Select(Describe)
            .ShouldBe(Draw(Difficulty.Advanced, 7, 20).Select(Describe));
        Draw(Difficulty.Advanced, 1, 20).Select(Describe)
            .ShouldNotBe(Draw(Difficulty.Advanced, 2, 20).Select(Describe));
    }

    [Fact]
    public void Beginner_QuestionsAreAscendingMelodicPoolIntervals()
    {
        var pool = IntervalDrillOptions.For(Difficulty.Beginner).Pool;
        foreach (var q in Draw(Difficulty.Beginner, 5, 200))
        {
            pool.ShouldContain(q.Answer);
            q.Choices.ShouldBe(pool);
            q.Key.ShouldBeNull();
            q.Sounds.Count.ShouldBe(2);
            q.Sounds.ShouldAllBe(s => s.Count == 1);
            var between = Interval.Between(q.Sounds[0][0], q.Sounds[1][0]);
            between.Semitones.ShouldBe(q.Answer.Semitones);
            between.Direction.ShouldBe(Direction.Up);
        }
    }

    [Fact]
    public void Pools_AreStrictlyNested()
    {
        var b = IntervalDrillOptions.For(Difficulty.Beginner).Pool;
        var i = IntervalDrillOptions.For(Difficulty.Intermediate).Pool;
        var a = IntervalDrillOptions.For(Difficulty.Advanced).Pool;
        b.ShouldBeSubsetOf(i);
        i.ShouldBeSubsetOf(a);
        b.Count.ShouldBeLessThan(i.Count);
        i.Count.ShouldBeLessThan(a.Count);
    }

    [Fact]
    public void Advanced_IncludesDescendingAndHarmonic()
    {
        var qs = Draw(Difficulty.Advanced, 3, 200);
        qs.ShouldContain(q => q.Sounds.Count == 2
            && Interval.Between(q.Sounds[0][0], q.Sounds[1][0]).Direction == Direction.Down);
        qs.ShouldContain(q => q.Sounds.Count == 1 && q.Sounds[0].Count == 2
            && q.Sounds[0][0].MidiNumber <= q.Sounds[0][1].MidiNumber);
    }

    [Fact]
    public void Check_IsAural()
    {
        var drill = new IntervalDrill(IntervalDrillOptions.For(Difficulty.Advanced), 1);
        var q = new Question<Interval>("p", [], [], Interval.Parse("A4"));
        drill.Check(q, Interval.Parse("d5")).ShouldBe(new AnswerResult<Interval>(true, 1, q.Answer));
        var wrong = drill.Check(q, Interval.Parse("P5"));
        wrong.IsCorrect.ShouldBeFalse();
        wrong.Score.ShouldBe(0);
    }

    [Fact]
    public void Guards_RejectBadArguments()
    {
        var o = IntervalDrillOptions.For(Difficulty.Beginner);
        var drill = new IntervalDrill(o, 1);
        var q = drill.Next();
        Should.Throw<ArgumentNullException>(() => new IntervalDrill(null!, 1));
        Should.Throw<ArgumentException>(() => new IntervalDrill(o with { Pool = [] }, 1));
        Should.Throw<ArgumentNullException>(() => drill.Check(null!, q.Answer));
        Should.Throw<ArgumentNullException>(() => drill.Check(q, null!));
        Should.Throw<ArgumentOutOfRangeException>(() => IntervalDrillOptions.For((Difficulty)99));
    }
}
