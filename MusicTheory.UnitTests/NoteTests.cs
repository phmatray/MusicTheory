namespace MusicTheory.UnitTests;

public class NoteTests
{
    [Fact]
    public void Note_ShouldHaveNoteName_WhenCreated()
    {
        var note = new Note(NoteName.C);
        
        note.Name.Should().Be(NoteName.C);
    }

    [Fact]
    public void Note_ShouldHaveAlteration_WhenCreatedWithAlteration()
    {
        var note = new Note(NoteName.C, Alteration.Sharp);
        
        note.Name.Should().Be(NoteName.C);
        note.Alteration.Should().Be(Alteration.Sharp);
    }

    [Fact]
    public void Note_ShouldHaveNaturalAlteration_WhenCreatedWithoutAlteration()
    {
        var note = new Note(NoteName.C);
        
        note.Alteration.Should().Be(Alteration.Natural);
    }

    [Fact]
    public void Note_ShouldHaveOctave_WhenCreatedWithOctave()
    {
        var note = new Note(NoteName.C, Alteration.Natural, 4);
        
        note.Octave.Should().Be(4);
    }

    [Fact]
    public void Note_ShouldDefaultToOctave4_WhenCreatedWithoutOctave()
    {
        var note = new Note(NoteName.C);
        
        note.Octave.Should().Be(4);
    }

    [Fact]
    public void Note_ShouldCalculateFrequency_ForA4()
    {
        var a4 = new Note(NoteName.A, Alteration.Natural, 4);
        
        a4.Frequency.Should().BeApproximately(440.0, 0.01);
    }

    [Fact]
    public void Note_ShouldCalculateFrequency_ForC4()
    {
        var c4 = new Note(NoteName.C, Alteration.Natural, 4);
        
        c4.Frequency.Should().BeApproximately(261.63, 0.01);
    }
}