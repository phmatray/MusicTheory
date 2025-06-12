namespace MusicTheory.UnitTests;

public class NoteTests
{
    [Fact]
    public void Note_ShouldHaveNoteName_WhenCreated()
    {
        var note = new Note(NoteName.C);
        
        note.Name.ShouldBe(NoteName.C);
    }

    [Fact]
    public void Note_ShouldHaveAlteration_WhenCreatedWithAlteration()
    {
        var note = new Note(NoteName.C, Alteration.Sharp);
        
        note.Name.ShouldBe(NoteName.C);
        note.Alteration.ShouldBe(Alteration.Sharp);
    }

    [Fact]
    public void Note_ShouldHaveNaturalAlteration_WhenCreatedWithoutAlteration()
    {
        var note = new Note(NoteName.C);
        
        note.Alteration.ShouldBe(Alteration.Natural);
    }

    [Fact]
    public void Note_ShouldHaveOctave_WhenCreatedWithOctave()
    {
        var note = new Note(NoteName.C, Alteration.Natural, 4);
        
        note.Octave.ShouldBe(4);
    }

    [Fact]
    public void Note_ShouldDefaultToOctave4_WhenCreatedWithoutOctave()
    {
        var note = new Note(NoteName.C);
        
        note.Octave.ShouldBe(4);
    }

    [Fact]
    public void Note_ShouldCalculateFrequency_ForA4()
    {
        var a4 = new Note(NoteName.A, Alteration.Natural, 4);
        
        a4.Frequency.ShouldBe(440.0, 0.01);
    }

    [Fact]
    public void Note_ShouldCalculateFrequency_ForC4()
    {
        var c4 = new Note(NoteName.C, Alteration.Natural, 4);
        
        c4.Frequency.ShouldBe(261.63, 0.01);
    }

    [Fact]
    public void Note_TransposeBySemitones_WithNegativeSemitones_ShouldTransposeDown()
    {
        // Arrange
        var c4 = new Note(NoteName.C, Alteration.Natural, 4);
        
        // Act
        var result = c4.TransposeBySemitones(-13); // Down more than an octave
        
        // Assert
        result.Name.ShouldBe(NoteName.B);
        result.Octave.ShouldBe(2);
    }

    [Fact] 
    public void Note_Transpose_WithComplexInterval_ShouldHandleOctaveWrapping()
    {
        // Arrange
        var b4 = new Note(NoteName.B, Alteration.Natural, 4);
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        
        // Act
        var result = b4.Transpose(majorSecond, Direction.Up);
        
        // Assert
        result.Name.ShouldBe(NoteName.C);
        result.Alteration.ShouldBe(Alteration.Sharp);
        result.Octave.ShouldBe(5);
    }

    [Fact]
    public void Note_Transpose_Down_WithOctaveWrapping_ShouldHandleCorrectly()
    {
        // Arrange
        var c4 = new Note(NoteName.C, Alteration.Natural, 4);
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        
        // Act
        var result = c4.Transpose(majorSecond, Direction.Down);
        
        // Assert
        result.Name.ShouldBe(NoteName.B);
        result.Alteration.ShouldBe(Alteration.Flat);
        result.Octave.ShouldBe(3);
    }
}