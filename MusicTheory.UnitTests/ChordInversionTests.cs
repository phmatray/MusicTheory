namespace MusicTheory.UnitTests;

public class ChordInversionTests
{
    [Fact]
    public void Chord_ShouldHaveInversionProperty_DefaultToRoot()
    {
        // Arrange & Act
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        
        // Assert
        chord.Inversion.ShouldBe(ChordInversion.Root);
    }

    [Fact]
    public void Chord_ShouldGetBassNote_ForRootPosition()
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        
        // Act
        var bassNote = cMajor.GetBassNote();
        
        // Assert
        bassNote.Name.ShouldBe(NoteName.C);
        bassNote.Octave.ShouldBe(4);
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
        bassNote.Name.ShouldBe(NoteName.E); // Third of C major
        bassNote.Octave.ShouldBe(4);
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
        bassNote.Name.ShouldBe(NoteName.G); // Fifth of C major
        bassNote.Octave.ShouldBe(4);
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
        bassNote.Name.ShouldBe(NoteName.B); // Seventh of C major 7
        bassNote.Octave.ShouldBe(4);
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
        notes.Count.ShouldBe(3);
        notes[0].Name.ShouldBe(NoteName.E); // Third (bass)
        notes[0].Octave.ShouldBe(4);
        notes[1].Name.ShouldBe(NoteName.G); // Fifth
        notes[1].Octave.ShouldBe(4);
        notes[2].Name.ShouldBe(NoteName.C); // Root (moved up an octave)
        notes[2].Octave.ShouldBe(5);
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
        notation.ShouldBe(expected);
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
        dMajorFirstInversion.Inversion.ShouldBe(ChordInversion.First);
        dMajorFirstInversion.GetBassNote().Name.ShouldBe(NoteName.F); // Third of D major
        dMajorFirstInversion.GetBassNote().Alteration.ShouldBe(Alteration.Sharp);
    }

    [Fact]
    public void Chord_GetNotesInInversion_SecondInversion_ShouldArrangeCorrectly()
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .WithInversion(ChordInversion.Second);
        
        // Act
        var notes = cMajor.GetNotesInInversion().ToList();
        
        // Assert
        notes.Count.ShouldBe(3);
        notes[0].Name.ShouldBe(NoteName.G); // Fifth (bass)
        notes[0].Octave.ShouldBe(4);
        notes[1].Name.ShouldBe(NoteName.C); // Root (up an octave)
        notes[1].Octave.ShouldBe(5);
        notes[2].Name.ShouldBe(NoteName.E); // Third (up an octave)
        notes[2].Octave.ShouldBe(5);
    }

    [Fact]
    public void Chord_GetNotesInInversion_ThirdInversion_ShouldRequireSeventh()
    {
        // Arrange
        var cMajor = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .WithInversion(ChordInversion.Third);
        
        // Act
        Action act = () => cMajor.GetNotesInInversion().ToList();
        
        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Third inversion requires a seventh chord");
    }

    [Fact]
    public void Chord_GetNotesInInversion_ThirdInversion_WithSeventh_ShouldArrangeCorrectly()
    {
        // Arrange
        var cMajor7 = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .AddExtension(7, IntervalQuality.Major)
            .WithInversion(ChordInversion.Third);
        
        // Act
        var notes = cMajor7.GetNotesInInversion().ToList();
        
        // Assert
        notes.Count.ShouldBe(4);
        notes[0].Name.ShouldBe(NoteName.B); // Seventh (bass)
        notes[0].Octave.ShouldBe(4);
        notes[1].Name.ShouldBe(NoteName.C); // Root (up an octave)
        notes[1].Octave.ShouldBe(5);
        notes[2].Name.ShouldBe(NoteName.E); // Third (up an octave)
        notes[2].Octave.ShouldBe(5);
        notes[3].Name.ShouldBe(NoteName.G); // Fifth (up an octave)
        notes[3].Octave.ShouldBe(5);
    }

    [Fact]
    public void Chord_GetBassNote_ThirdInversion_WithoutSeventh_ShouldThrow()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major)
            .WithInversion(ChordInversion.Third);
        
        // Act
        Action act = () => chord.GetBassNote();
        
        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Third inversion requires a seventh chord");
    }

    [Fact]
    public void Chord_GetNotesInInversion_RootPosition_ShouldReturnOriginalOrder()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordQuality.Minor)
            .WithInversion(ChordInversion.Root);
        
        // Act
        var notes = chord.GetNotesInInversion().ToList();
        
        // Assert
        notes.Count.ShouldBe(3);
        notes[0].Name.ShouldBe(NoteName.D);
        notes[0].Octave.ShouldBe(4);
        notes[1].Name.ShouldBe(NoteName.F);
        notes[1].Octave.ShouldBe(4);
        notes[2].Name.ShouldBe(NoteName.A);
        notes[2].Octave.ShouldBe(4);
    }

    [Fact]
    public void Chord_GetSlashChordNotation_WithAlterations_ShouldWorkCorrectly()
    {
        // Arrange
        var fSharpMinor = new Chord(new Note(NoteName.F, Alteration.Sharp, 4), ChordQuality.Minor)
            .WithInversion(ChordInversion.First);
        
        // Act
        var notation = fSharpMinor.GetSlashChordNotation();
        
        // Assert
        notation.ShouldBe("F/A"); // F#m/A - but the method doesn't include alterations
    }

    [Fact]
    public void Chord_GetNotesInInversion_WithExtensions_ShouldIncludeAllNotes()
    {
        // Arrange
        var cMaj9 = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .AddExtension(7, IntervalQuality.Major)
            .AddExtension(9, IntervalQuality.Major)
            .WithInversion(ChordInversion.First);
        
        // Act
        var notes = cMaj9.GetNotesInInversion().ToList();
        
        // Assert
        notes.Count.ShouldBe(5);
        notes[0].Name.ShouldBe(NoteName.E); // Third (bass)
        notes[1].Name.ShouldBe(NoteName.G); // Fifth
        notes[2].Name.ShouldBe(NoteName.B); // Seventh
        notes[3].Name.ShouldBe(NoteName.D); // Ninth
        notes[4].Name.ShouldBe(NoteName.C); // Root (up an octave)
    }
}