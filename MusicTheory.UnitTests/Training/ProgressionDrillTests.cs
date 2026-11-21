using MusicTheory.Training;
using Shouldly;

namespace MusicTheory.UnitTests.Training;

public class ProgressionDrillTests
{
    private static List<Question<IReadOnlyList<int>>> Draw(Difficulty d, ulong seed, int n)
    {
        var drill = new ProgressionDrill(ProgressionDrillOptions.For(d), seed);
        return Enumerable.Range(0, n).Select(_ => drill.Next()).ToList();
    }

    private static string Describe(Question<IReadOnlyList<int>> q) => q.Prompt + "|" + string.Join(",", q.Answer);

    [Fact]
    public void SameSeed_ReplaysSameQuestions()
    {
        Draw(Difficulty.Advanced, 7, 20).Select(Describe).ShouldBe(Draw(Difficulty.Advanced, 7, 20).Select(Describe));
        Draw(Difficulty.Advanced, 1, 20).Select(Describe).ShouldNotBe(Draw(Difficulty.Advanced, 2, 20).Select(Describe));
    }

    [Theory]
    [InlineData(Difficulty.Beginner)]
    [InlineData(Difficulty.Intermediate)]
    [InlineData(Difficulty.Advanced)]
    public void Answers_FollowCadenceRules(Difficulty d)
    {
        var pool = ProgressionDrillOptions.For(d).Degrees;
        foreach (var q in Draw(d, 3, 200))
        {
            q.Answer.Count.ShouldBe(4);
            q.Answer[0].ShouldBe(1);
            new[] { 1, 5 }.ShouldContain(q.Answer[^1]);
            for (var i = 1; i < q.Answer.Count; i++)
            {
                q.Answer[i].ShouldNotBe(q.Answer[i - 1]);
            }

            q.Answer.Take(3).ShouldAllBe(x => pool.Contains(x));
            q.Choices.ShouldBeEmpty();
        }
    }

    [Fact]
    public void Sounds_MatchChordProgression()
    {
        foreach (var q in Draw(Difficulty.Advanced, 9, 100))
        {
            var progression = new ChordProgression(q.Key!);
            q.Sounds.Count.ShouldBe(q.Answer.Count);
            for (var i = 0; i < q.Answer.Count; i++)
            {
                q.Sounds[i].ShouldBe(progression.GetChordByDegree(q.Answer[i]).GetNotes());
            }
        }
    }

    [Fact]
    public void KeyModes_FollowDifficulty()
    {
        Draw(Difficulty.Beginner, 4, 200).ShouldAllBe(q => q.Key!.Mode == KeyMode.Major);
        Draw(Difficulty.Intermediate, 4, 200).ShouldAllBe(q => q.Key!.Mode == KeyMode.Major);
        Draw(Difficulty.Advanced, 4, 200).ShouldContain(q => q.Key!.Mode == KeyMode.Minor);
    }

    [Fact]
    public void Check_GivesPartialCredit()
    {
        var drill = new ProgressionDrill(ProgressionDrillOptions.For(Difficulty.Beginner), 1);
        var q = new Question<IReadOnlyList<int>>("p", [], [], new[] { 1, 4, 5, 1 });
        var ok = drill.Check(q, [1, 4, 5, 1]);
        (ok.IsCorrect, ok.Score).ShouldBe((true, 1.0));
        var wrong = drill.Check(q, [1, 4, 4, 1]);
        (wrong.IsCorrect, wrong.Score).ShouldBe((false, 0.75));
        var shortAnswer = drill.Check(q, [1, 4]);
        (shortAnswer.IsCorrect, shortAnswer.Score).ShouldBe((false, 0.5));
        drill.Check(q, [1, 4, 5, 1, 1]).IsCorrect.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_And_Check_Validate()
    {
        Should.Throw<ArgumentNullException>(() => new ProgressionDrill(null!, 1));
        Should.Throw<ArgumentException>(() => new ProgressionDrill(new([1], false, 1), 1));
        Should.Throw<ArgumentException>(() => new ProgressionDrill(new([], false), 1));
        Should.Throw<ArgumentException>(() => new ProgressionDrill(new([0], false), 1));
        Should.Throw<ArgumentException>(() => new ProgressionDrill(new([8], false), 1));
        Should.Throw<ArgumentOutOfRangeException>(() => ProgressionDrillOptions.For((Difficulty)9));
        var drill = new ProgressionDrill(ProgressionDrillOptions.For(Difficulty.Beginner), 1);
        Should.Throw<ArgumentNullException>(() => drill.Check(null!, [1]));
        Should.Throw<ArgumentNullException>(() => drill.Check(drill.Next(), null!));
    }
}
