using MusicTheory.Training;
using Shouldly;

namespace MusicTheory.UnitTests.Training;

public class MelodyDrillTests
{
    private static string Describe(Question<IReadOnlyList<Note>> q) => q.Prompt + "|" + string.Join(" ", q.Answer);

    private static List<Question<IReadOnlyList<Note>>> Draw(Difficulty d, ulong seed, int n)
    {
        var drill = new MelodyDrill(MelodyDrillOptions.For(d), seed);
        return Enumerable.Range(0, n).Select(_ => drill.Next()).ToList();
    }

    [Fact]
    public void SameSeed_ReplaysSameQuestions()
    {
        Draw(Difficulty.Advanced, 7, 20).Select(Describe).ShouldBe(Draw(Difficulty.Advanced, 7, 20).Select(Describe));
        Draw(Difficulty.Advanced, 1, 20).Select(Describe).ShouldNotBe(Draw(Difficulty.Advanced, 2, 20).Select(Describe));
    }

    [Theory]
    [InlineData(Difficulty.Beginner, 4)]
    [InlineData(Difficulty.Intermediate, 6)]
    [InlineData(Difficulty.Advanced, 8)]
    public void Melodies_StayInOneOctaveOfTheKeysScale(Difficulty d, int length)
    {
        var sawMinor = false;
        foreach (var q in Draw(d, 11, 200))
        {
            var key = q.Key!;
            sawMinor |= key.Mode == KeyMode.Minor;
            var scale = new Scale(key.Tonic, key.Mode == KeyMode.Minor ? ScaleType.NaturalMinor : ScaleType.Major);
            q.Answer.Count.ShouldBe(length);
            q.Answer[0].ShouldBe(key.Tonic);
            q.Answer[0].Octave.ShouldBe(4);
            q.Sounds.Count.ShouldBe(length);
            q.Choices.ShouldBeEmpty();
            foreach (var n in q.Answer)
            {
                scale.Contains(n).ShouldBeTrue();
                n.MidiNumber.ShouldBeInRange(key.Tonic.MidiNumber, key.Tonic.MidiNumber + 12);
            }
        }

        sawMinor.ShouldBe(d != Difficulty.Beginner);
    }

    [Fact]
    public void Beginner_MovesByStep_AndEndsOnTonic()
    {
        foreach (var q in Draw(Difficulty.Beginner, 5, 200))
        {
            var a = q.Answer;
            for (var i = 1; i < a.Count - 1; i++)
            {
                Interval.Between(a[i - 1], a[i]).Number.ShouldBe(2);
            }

            a[^1].Name.ShouldBe(q.Key!.Tonic.Name);
        }

        foreach (var q in Draw(Difficulty.Intermediate, 5, 200))
        {
            q.Answer[^1].Name.ShouldBe(q.Key!.Tonic.Name);
        }
    }

    [Fact]
    public void Check_ScoresPerNote()
    {
        var drill = new MelodyDrill(MelodyDrillOptions.For(Difficulty.Beginner), 3);
        var q = drill.Next();
        var a = q.Answer.ToList();

        var same = drill.Check(q, a);
        same.IsCorrect.ShouldBeTrue();
        same.Score.ShouldBe(1);
        same.Expected.ShouldBe(a);

        var enh = a.ToList();
        enh[1] = enh[1].GetEnharmonicEquivalent()!;
        drill.Check(q, enh).IsCorrect.ShouldBeTrue();

        var wrong = a.ToList();
        wrong[3] = wrong[3].Transpose(Interval.Parse("M2"));
        var w = drill.Check(q, wrong);
        w.IsCorrect.ShouldBeFalse();
        w.Score.ShouldBe(0.75);

        var half = drill.Check(q, a.Take(2).ToList());
        half.IsCorrect.ShouldBeFalse();
        half.Score.ShouldBe(0.5);
    }

    [Fact]
    public void InvalidArguments_Throw()
    {
        Should.Throw<ArgumentNullException>(() => new MelodyDrill(null!, 1));
        Should.Throw<ArgumentException>(() => new MelodyDrill(new(1, [1], false, false), 1));
        Should.Throw<ArgumentException>(() => new MelodyDrill(new(4, [], false, false), 1));
        Should.Throw<ArgumentException>(() => new MelodyDrill(new(4, [0], false, false), 1));
        Should.Throw<ArgumentException>(() => new MelodyDrill(new(4, [8], false, false), 1));
        Should.Throw<ArgumentOutOfRangeException>(() => MelodyDrillOptions.For((Difficulty)9));
        var drill = new MelodyDrill(MelodyDrillOptions.For(Difficulty.Beginner), 1);
        var q = drill.Next();
        Should.Throw<ArgumentNullException>(() => drill.Check(null!, q.Answer));
        Should.Throw<ArgumentNullException>(() => drill.Check(q, null!));
    }
}
