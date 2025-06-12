namespace MusicTheory.UnitTests;

public class ChordInversionTests
{
    [Fact]
    public void Chord_ShouldHaveInversionProperty_DefaultToRoot()
    {
        // Arrange & Act
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        
        // Assert
        chord.Inversion.Should().Be(ChordInversion.Root);
    }

    [Fact]
    public void Chord_ShouldGetBassNote_ForRootPosition()
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        
        // Act
        var bassNote = cMajor.GetBassNote();
        
        // Assert
        bassNote.Name.Should().Be(NoteName.C);
        bassNote.Octave.Should().Be(4);
    }

    [Fact]
    public void Chord_ShouldGetBassNote_ForFirstInversion()
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        var firstInversion = cMajor.WithInversion(ChordInversion.First);
        
        // Act
        var bassNote = firstInversion.GetBassNote();
        
        // Assert
        bassNote.Name.Should().Be(NoteName.E); // Third of C major
        bassNote.Octave.Should().Be(4);
    }

    [Fact]
    public void Chord_ShouldGetBassNote_ForSecondInversion()
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        var secondInversion = cMajor.WithInversion(ChordInversion.Second);
        
        // Act
        var bassNote = secondInversion.GetBassNote();
        
        // Assert
        bassNote.Name.Should().Be(NoteName.G); // Fifth of C major
        bassNote.Octave.Should().Be(4);
    }

    [Fact]
    public void Chord_ShouldGetBassNote_ForThirdInversion_WithSeventh()
    {
        // Arrange
        var cMajor7 = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .AddExtension(7, IntervalQuality.Major);
        var thirdInversion = cMajor7.WithInversion(ChordInversion.Third);
        
        // Act
        var bassNote = thirdInversion.GetBassNote();
        
        // Assert
        bassNote.Name.Should().Be(NoteName.B); // Seventh of C major 7
        bassNote.Octave.Should().Be(4);
    }

    [Fact]
    public void Chord_ShouldArrangeNotes_ForFirstInversion()
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        var firstInversion = cMajor.WithInversion(ChordInversion.First);
        
        // Act
        var notes = firstInversion.GetNotesInInversion().ToList();
        
        // Assert
        notes.Should().HaveCount(3);
        notes[0].Name.Should().Be(NoteName.E); // Third (bass)
        notes[0].Octave.Should().Be(4);
        notes[1].Name.Should().Be(NoteName.G); // Fifth
        notes[1].Octave.Should().Be(4);
        notes[2].Name.Should().Be(NoteName.C); // Root (moved up an octave)
        notes[2].Octave.Should().Be(5);
    }

    [Theory]
    [InlineData(ChordInversion.Root, "C/C")]
    [InlineData(ChordInversion.First, "C/E")]
    [InlineData(ChordInversion.Second, "C/G")]
    public void Chord_ShouldGetSlashChordNotation(ChordInversion inversion, string expected)
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var inverted = cMajor.WithInversion(inversion);
        
        // Act
        var notation = inverted.GetSlashChordNotation();
        
        // Assert
        notation.Should().Be(expected);
    }

    [Fact]
    public void Chord_ShouldMaintainInversion_WhenTransposed()
    {
        // Arrange
        var cMajorFirstInversion = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .WithInversion(ChordInversion.First);
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        
        // Act
        var dMajorFirstInversion = cMajorFirstInversion.Transpose(majorSecond);
        
        // Assert
        dMajorFirstInversion.Inversion.Should().Be(ChordInversion.First);
        dMajorFirstInversion.GetBassNote().Name.Should().Be(NoteName.F); // Third of D major
        dMajorFirstInversion.GetBassNote().Alteration.Should().Be(Alteration.Sharp);
    }
}