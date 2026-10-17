namespace MusicTheory.UnitTests;

public class PianoVoicingTests
{
    [Fact]
    public void Voice_RootPosition()
    {
        ChordSymbol.Parse("Cmaj7").Voice(4).Select(n => n.MidiNumber).ShouldBe([60, 64, 67, 71]);
    }

    [Fact]
    public void Voice_SlashBassGoesBelow()
    {
        ChordSymbol.Parse("D/F#").Voice(4).Select(n => n.MidiNumber).ShouldBe([54, 62, 66, 69]);
        ChordSymbol.Parse("C/E").Voice(4).Select(n => n.MidiNumber).ShouldBe([52, 60, 64, 67]);
    }

    [Fact]
    public void Voice_UsesIntervalSpelling()
    {
        var notes = ChordSymbol.Parse("E7#9").Voice(3);
        notes.Select(n => n.MidiNumber).ShouldBeInOrder(SortDirection.Ascending);
        notes.Select(MusicTheoryUtilities.GetPitchClass).ShouldBe([4, 8, 11, 2, 7]);
        notes[^1].GetName().ShouldBe("F##");
    }

    [Fact]
    public void PianoLayout_TwoOctaves()
    {
        var keys = PianoLayout.Keys(48, 72);
        keys.Count.ShouldBe(25);
        keys.Count(k => !k.IsBlack).ShouldBe(15);
        keys.Count(k => k.IsBlack).ShouldBe(10);
        // C#3 sits on the C3|D3 boundary, F#3 on F3|G3.
        keys.Single(k => k.Midi == 49).X.ShouldBe(0.7, 1e-9);
        keys.Single(k => k.Midi == 54).X.ShouldBe(3.7, 1e-9);
        keys.Single(k => k.Midi == 72).X.ShouldBe(14);
    }
}
