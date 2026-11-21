using MusicTheory.Toolbox.Drills;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

public class DrillAudioTests
{
    [Fact]
    public void A_melodic_question_is_two_sequential_entries()
    {
        var q = new IntervalDrill(IntervalDrillOptions.For(Difficulty.Beginner), 42).Next();
        var s = DrillAudio.ToSchedule(q.Sounds);
        s.Count.ShouldBe(2);
        s.ShouldAllBe(step => step.Count == 1);
        s[0][0].ShouldBe(q.Sounds[0][0].MidiNumber);
    }

    [Fact]
    public void A_harmonic_question_is_one_entry_with_both_notes()
    {
        var sounds = new[] { new[] { Note.Parse("C4"), Note.Parse("G4") } };
        DrillAudio.ToSchedule(sounds).Single().ShouldBe([60, 67]);
    }
}
