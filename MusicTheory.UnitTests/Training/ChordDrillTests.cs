using MusicTheory.Training;
using Shouldly;

namespace MusicTheory.UnitTests.Training;

public class ChordDrillTests
{
    private static string Describe(Question<ChordType> q) =>
        q.Prompt + "|" + string.Join(" ", q.Sounds.Select(s => string.Join("+", s))) + "|" + q.Answer;

    private static List<Question<ChordType>> Draw(Difficulty d, ulong seed, int n)
    {
        var drill = new ChordDrill(ChordDrillOptions.For(d), seed);
        return Enumerable.Range(0, n).Select(_ => drill.Next()).ToList();
    }

    private static HashSet<int> PitchClasses(IEnumerable<Note> notes) => notes.Select(n => n.MidiNumber % 12).ToHashSet();

    // Some chord tones cannot be spelled as a root (three accidentals); those candidates are skipped.
    private static HashSet<int>? PitchClassesOfChordOn(Note root, ChordType type)
    {
        try { return PitchClasses(new Chord(root, type).GetNotes()); }
        catch (InvalidOperationException) { return null; }
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
        var b = ChordDrillOptions.For(Difficulty.Beginner).Pool;
        var i = ChordDrillOptions.For(Difficulty.Intermediate).Pool;
        var a = ChordDrillOptions.For(Difficulty.Advanced).Pool;
        b.ShouldBeSubsetOf(i);
        i.ShouldBeSubsetOf(a);
        i.Count.ShouldBeGreaterThan(b.Count);
        a.Count.ShouldBeGreaterThan(i.Count);
    }

    [Fact]
    public void Advanced_QuestionsMatchIndependentlyBuiltChords()
    {
        var pool = ChordDrillOptions.For(Difficulty.Advanced).Pool;
        foreach (var q in Draw(Difficulty.Advanced, 5, 300))
        {
            pool.ShouldContain(q.Answer);
            q.Choices.ShouldBe(pool);
            q.Key.ShouldBeNull();
            q.Sounds.Count.ShouldBe(1);
            var sounds = q.Sounds[0];
            var expected = sounds.Select(n => PitchClassesOfChordOn(n, q.Answer)).ToList();
            expected.Any(e => e != null && e.SetEquals(PitchClasses(sounds))).ShouldBeTrue();
        }
    }

    [Fact]
    public void Advanced_SometimesInverts()
    {
        Draw(Difficulty.Advanced, 5, 300)
            .Any(q => PitchClassesOfChordOn(q.Sounds[0].MinBy(n => n.MidiNumber)!, q.Answer)?
                .SetEquals(PitchClasses(q.Sounds[0])) != true).ShouldBeTrue();
    }

    [Fact]
    public void Beginner_NeverInverts()
    {
        foreach (var q in Draw(Difficulty.Beginner, 3, 200))
        {
            var lowest = q.Sounds[0].MinBy(n => n.MidiNumber)!;
            PitchClasses(new Chord(lowest, q.Answer).GetNotes()).ShouldBe(PitchClasses(q.Sounds[0]));
        }
    }

    [Fact]
    public void Check_ComparesTypes()
    {
        var drill = new ChordDrill(ChordDrillOptions.For(Difficulty.Beginner), 1);
        var q = drill.Next();
        var other = q.Answer == ChordType.Major ? ChordType.Minor : ChordType.Major;
        drill.Check(q, q.Answer).ShouldBe(new AnswerResult<ChordType>(true, 1, q.Answer));
        drill.Check(q, other).ShouldBe(new AnswerResult<ChordType>(false, 0, q.Answer));
        Should.Throw<ArgumentNullException>(() => drill.Check(null!, other));
    }

    [Fact]
    public void Guards()
    {
        Should.Throw<ArgumentNullException>(() => new ChordDrill(null!, 1));
        Should.Throw<ArgumentException>(() => new ChordDrill(new ChordDrillOptions([], false), 1));
        Should.Throw<ArgumentOutOfRangeException>(() => ChordDrillOptions.For((Difficulty)99));
    }
}
