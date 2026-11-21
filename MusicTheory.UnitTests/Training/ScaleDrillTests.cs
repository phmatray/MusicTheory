using MusicTheory.Training;
using Shouldly;

namespace MusicTheory.UnitTests.Training;

public class ScaleDrillTests
{
    private static string Describe(Question<ScaleType> q) =>
        q.Prompt + "|" + string.Join(" ", q.Sounds.Select(s => string.Join("+", s))) + "|" + q.Answer;

    private static List<Question<ScaleType>> Draw(Difficulty d, ulong seed, int n)
    {
        var drill = new ScaleDrill(ScaleDrillOptions.For(d), seed);
        return Enumerable.Range(0, n).Select(_ => drill.Next()).ToList();
    }

    [Fact]
    public void SameSeed_ReplaysSameQuestions()
    {
        Draw(Difficulty.Advanced, 7, 20).Select(Describe).ShouldBe(Draw(Difficulty.Advanced, 7, 20).Select(Describe));
        Draw(Difficulty.Advanced, 1, 20).Select(Describe).ShouldNotBe(Draw(Difficulty.Advanced, 2, 20).Select(Describe));
    }

    [Fact]
    public void Pools_AreStrictlyNested()
    {
        var b = ScaleDrillOptions.For(Difficulty.Beginner).Pool;
        var i = ScaleDrillOptions.For(Difficulty.Intermediate).Pool;
        var a = ScaleDrillOptions.For(Difficulty.Advanced).Pool;
        b.ShouldBeSubsetOf(i);
        i.ShouldBeSubsetOf(a);
        i.Count.ShouldBeGreaterThan(b.Count);
        a.Count.ShouldBeGreaterThan(i.Count);
    }

    [Fact]
    public void Advanced_QuestionsMatchIndependentlyBuiltScales()
    {
        var pool = ScaleDrillOptions.For(Difficulty.Advanced).Pool;
        foreach (var q in Draw(Difficulty.Advanced, 5, 300))
        {
            pool.ShouldContain(q.Answer);
            q.Choices.ShouldBe(pool);
            q.Key.ShouldBeNull();
            q.Sounds.ShouldAllBe(s => s.Count == 1);
            var notes = q.Sounds.Select(s => s[0]).ToList();
            notes.Select(n => n.ToString()).ShouldBe(new Scale(notes[0], q.Answer).GetNotes().Select(n => n.ToString()));
        }
    }

    [Fact]
    public void Check_AcceptsEnharmonicModeNames()
    {
        var drill = new ScaleDrill(ScaleDrillOptions.For(Difficulty.Advanced), 1);
        Question<ScaleType> Q(ScaleType t) => new("", [], [], t);
        drill.Check(Q(ScaleType.Major), ScaleType.Ionian).IsCorrect.ShouldBeTrue();
        drill.Check(Q(ScaleType.NaturalMinor), ScaleType.Aeolian).IsCorrect.ShouldBeTrue();
        drill.Check(Q(ScaleType.Major), ScaleType.Dorian).ShouldBe(new AnswerResult<ScaleType>(false, 0, ScaleType.Major));
        drill.Check(Q(ScaleType.Dorian), ScaleType.Dorian).Score.ShouldBe(1);
        Should.Throw<ArgumentNullException>(() => drill.Check(null!, ScaleType.Major));
    }

    [Fact]
    public void Guards()
    {
        Should.Throw<ArgumentNullException>(() => new ScaleDrill(null!, 1));
        Should.Throw<ArgumentException>(() => new ScaleDrill(new ScaleDrillOptions([]), 1));
        Should.Throw<ArgumentOutOfRangeException>(() => ScaleDrillOptions.For((Difficulty)99));
    }
}
