using MusicTheory.Toolbox.Drills;
using MusicTheory.Toolbox.Rendering;
using MusicTheory.Toolbox.State;
using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.Tools.Dictation;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

public class MelodyInputModelTests
{
    [Fact]
    public void Add_and_undo_stay_within_bounds()
    {
        var m = new MelodyInputModel(2);
        m.Undo(); // nothing to undo is fine
        m.Value.ShouldBeEmpty();
        m.Add(Note.Parse("C4"));
        m.IsComplete.ShouldBeFalse();
        m.Add(Note.Parse("D4"));
        m.IsComplete.ShouldBeTrue();
        Should.Throw<InvalidOperationException>(() => m.Add(Note.Parse("E4")));
        m.Undo();
        m.IsComplete.ShouldBeFalse();
        m.Value.Select(n => n.ToString()).ShouldBe(["C4"]);
    }

    [Fact]
    public void The_right_notes_score_one_and_one_wrong_note_scores_a_fraction_through_the_session()
    {
        var drill = new MelodyDrill(MelodyDrillOptions.For(Difficulty.Beginner), 42);
        var questions = new[] { drill.Next(), drill.Next() };
        var session = new DrillSessionModel<IReadOnlyList<Note>>(questions, drill.Check);

        session.Answer(questions[0].Answer).Score.ShouldBe(1.0);

        var wrong = questions[1].Answer.ToList();
        wrong[1] = Note.FromMidiNumber(wrong[1].MidiNumber + 1);
        var r = session.Answer(wrong);
        r.IsCorrect.ShouldBeFalse();
        r.Score.ShouldBe(3.0 / 4);
        session.Score.ShouldBe(1.75);
    }

    [Fact]
    public void The_keyboard_covers_the_melody_and_spells_for_the_key()
    {
        var q = new MelodyDrill(MelodyDrillOptions.For(Difficulty.Advanced), 42).Next();
        var (low, high) = MelodyInputModel.KeyboardRange(q.Answer);
        high.ShouldBe(low + 18);
        q.Answer.ShouldAllBe(n => n.MidiNumber >= low && n.MidiNumber <= high);
        MelodyInputModel.Spell(70, new KeySignature(Note.Parse("F4"), KeyMode.Major)).GetName().ShouldBe("Bb");
        MelodyInputModel.Spell(70, new KeySignature(Note.Parse("G4"), KeyMode.Major)).GetName().ShouldBe("A#");
    }

    [Fact]
    public void The_dictation_link_reads_back_and_the_review_abc_round_trips()
    {
        var state = new ToolState("dictation", Lang.En, new Dictionary<string, string> { ["d"] = "advanced", ["seed"] = "42", ["n"] = "2", ["key"] = "1" });
        var link = DrillLink.FromState(state);
        (link.Tool, link.Difficulty, link.Seed, link.Count, link.TeacherKey).ShouldBe(("dictation", Difficulty.Advanced, 42UL, 2, true));

        var q = new MelodyDrill(MelodyDrillOptions.For(link.Difficulty), 42).Next();
        var abc = StaffAbc.FromNotes(q.Key!, q.Answer);
        MusicTheory.Abc.AbcTune.Parse(abc).ToString().ShouldBe(abc);
        StaffAbc.FromNotes(q.Key!, q.Answer.Take(2).ToArray()).ShouldNotBe(abc);
    }
}
