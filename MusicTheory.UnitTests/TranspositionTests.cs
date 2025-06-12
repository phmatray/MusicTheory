namespace MusicTheory.UnitTests;

public class TranspositionTests
{
    [Fact]
    public void Note_ShouldTransposeUp_ByInterval()
    {
        // Arrange
        var c4 = new Note(NoteName.C, Alteration.Natural, 4);
        var majorThird = new Interval(IntervalQuality.Major, 3);
        
        // Act
        var result = c4.Transpose(majorThird);
        
        // Assert
        result.Name.ShouldBe(NoteName.E);
        result.Alteration.ShouldBe(Alteration.Natural);
        result.Octave.ShouldBe(4);
    }

    [Fact]
    public void Note_ShouldTransposeDown_ByNegativeInterval()
    {
        // Arrange
        var e4 = new Note(NoteName.E, Alteration.Natural, 4);
        var majorThird = new Interval(IntervalQuality.Major, 3);
        
        // Act
        var result = e4.Transpose(majorThird, Direction.Down);
        
        // Assert
        result.Name.ShouldBe(NoteName.C);
        result.Alteration.ShouldBe(Alteration.Natural);
        result.Octave.ShouldBe(4);
    }

    [Fact]
    public void Note_ShouldTransposeAcrossOctave()
    {
        // Arrange
        var b4 = new Note(NoteName.B, Alteration.Natural, 4);
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        
        // Act
        var result = b4.Transpose(majorSecond);
        
        // Assert
        result.Name.ShouldBe(NoteName.C);
        result.Alteration.ShouldBe(Alteration.Sharp);
        result.Octave.ShouldBe(5);
    }

    [Theory]
    [InlineData(NoteName.C, Alteration.Natural, 4, 12, NoteName.C, Alteration.Natural, 5)] // Up an octave
    [InlineData(NoteName.D, Alteration.Natural, 4, 2, NoteName.E, Alteration.Natural, 4)]   // Up major 2nd
    [InlineData(NoteName.G, Alteration.Natural, 4, 7, NoteName.D, Alteration.Natural, 5)]   // Up perfect 5th
    [InlineData(NoteName.F, Alteration.Sharp, 4, 1, NoteName.G, Alteration.Natural, 4)]     // F# up minor 2nd
    public void Note_ShouldTransposeBySemitones(
        NoteName startName, Alteration startAlteration, int startOctave, int semitones,
        NoteName expectedName, Alteration expectedAlteration, int expectedOctave)
    {
        // Arrange
        var note = new Note(startName, startAlteration, startOctave);
        
        // Act
        var result = note.TransposeBySemitones(semitones);
        
        // Assert
        result.Name.ShouldBe(expectedName);
        result.Alteration.ShouldBe(expectedAlteration);
        result.Octave.ShouldBe(expectedOctave);
    }

    [Fact]
    public void Chord_ShouldTranspose_AllNotes()
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        
        // Act
        var dMajor = cMajor.Transpose(majorSecond);
        var notes = dMajor.GetNotes().ToList();
        
        // Assert
        dMajor.Root.Name.ShouldBe(NoteName.D);
        notes[0].Name.ShouldBe(NoteName.D); // Root
        notes[1].Name.ShouldBe(NoteName.F); // Third
        notes[1].Alteration.ShouldBe(Alteration.Sharp);
        notes[2].Name.ShouldBe(NoteName.A); // Fifth
    }

    [Fact]
    public void Scale_ShouldTranspose_ToNewKey()
    {
        // Arrange
        var cMajorScale = new Scale(new Note(NoteName.C, Alteration.Natural, 4), ScaleType.Major);
        var perfectFourth = new Interval(IntervalQuality.Perfect, 4);
        
        // Act
        var fMajorScale = cMajorScale.Transpose(perfectFourth);
        var notes = fMajorScale.GetNotes().ToList();
        
        // Assert
        fMajorScale.Root.Name.ShouldBe(NoteName.F);
        notes[0].Name.ShouldBe(NoteName.F);
        notes[3].Name.ShouldBe(NoteName.B);
        notes[3].Alteration.ShouldBe(Alteration.Flat); // Bb in F major
    }
}