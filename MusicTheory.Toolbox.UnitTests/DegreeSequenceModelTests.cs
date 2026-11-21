using MusicTheory.Toolbox.Drills;
using MusicTheory.Toolbox.State;
using MusicTheory.Toolbox.Tools;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

public class DegreeSequenceModelTests
{
    [Fact]
    public void Add_and_undo_keep_the_order()
    {
        var m = new DegreeSequenceModel(4);
        m.Add(1); m.Add(5); m.Add(4);
        m.Undo();
        m.Value.ShouldBe([1, 5]);
    }

    [Fact]
    public void It_cannot_exceed_the_progression_length_and_undo_on_empty_is_harmless()
    {
        var m = new DegreeSequenceModel(2);
        m.Undo();
        m.Add(1); m.Add(4); m.Add(5);
        m.Value.ShouldBe([1, 4]);
        m.IsFull.ShouldBeTrue();
    }

    [Fact]
    public void A_half_right_sequence_scores_one_half()
    {
        var drill = new ProgressionDrill(ProgressionDrillOptions.For(Difficulty.Beginner), 42);
        var q = drill.Next();
        var given = q.Answer.Select((d, i) => i % 2 == 0 ? d : (d == 4 ? 5 : 4)).ToArray();
        drill.Check(q, given).Score.ShouldBe(0.5);
    }
}

public class DrillSessionsTests
{
    private static DrillLink Link(string tool) =>
        DrillLink.FromState(ToolboxUrl.Parse(new Uri($"https://x.test/?t={tool}&seed=42&n=3&d=advanced"), "en"));

    [Fact]
    public void Each_tool_id_reads_as_its_own_drill_and_is_registered()
    {
        foreach (var id in new[] { "chords", "scales", "progressions" })
        {
            Link(id).Tool.ShouldBe(id);
            ToolRegistry.Find(id).ShouldNotBeNull();
        }
    }

    [Fact]
    public void The_same_link_gives_the_same_questions_and_the_count_is_honoured()
    {
        var a = DrillSessions.Chords(Link("chords"), 42).Questions.Select(q => q.Answer).ToList();
        var b = DrillSessions.Chords(Link("chords"), 42).Questions.Select(q => q.Answer).ToList();
        a.Count.ShouldBe(3);
        a.ShouldBe(b);
        DrillSessions.Scales(Link("scales"), 42).Questions.Count.ShouldBe(3);
        DrillSessions.Progressions(Link("progressions"), 42).Questions.Count.ShouldBe(3);
    }

    [Fact]
    public void A_partly_right_progression_adds_a_fractional_score_to_the_session()
    {
        var s = DrillSessions.Progressions(Link("progressions"), 42);
        var answer = s.Current!.Answer;
        var wrongLast = answer.Take(3).Append(answer[3] == 1 ? 5 : 1).ToArray();
        s.Answer(wrongLast).IsCorrect.ShouldBeFalse();
        s.Score.ShouldBe(0.75);
    }
}
