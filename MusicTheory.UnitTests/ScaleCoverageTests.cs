namespace MusicTheory.UnitTests;

public class ScaleCoverageTests
{
    [Fact]
    public void Scale_GetNextNoteInScale_ShouldReturnCorrectNote()
    {
        // Arrange
        var cMajor = new Scale(new Note(NoteName.C), ScaleType.Major);
        var d = new Note(NoteName.D);
        var e = new Note(NoteName.E);
        var b = new Note(NoteName.B);
        
        // Act
        var afterD = cMajor.GetNextNoteInScale(d);
        var afterE = cMajor.GetNextNoteInScale(e);
        var afterB = cMajor.GetNextNoteInScale(b);
        
        // Assert
        afterD.Name.ShouldBe(NoteName.E);
        afterE.Name.ShouldBe(NoteName.F);
        afterB.Name.ShouldBe(NoteName.C);
        afterB.Octave.ShouldBe(b.Octave + 1); // Octave wraps
    }

    [Fact]
    public void Scale_GetNextNoteInScale_WithNoteNotInScale_ShouldThrow()
    {
        // Arrange
        var cMajor = new Scale(new Note(NoteName.C), ScaleType.Major);
        var cSharp = new Note(NoteName.C, Alteration.Sharp);
        
        // Act
        Action act = () => cMajor.GetNextNoteInScale(cSharp);
        
        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("Note C#4 is not in the C Major scale");
    }

    [Fact]
    public void Scale_GetNotes_ForChromaticScale_ShouldReturn12Notes()
    {
        // Arrange
        var chromaticC = new Scale(new Note(NoteName.C), ScaleType.Chromatic);
        
        // Act
        var notes = chromaticC.GetNotes().ToList();
        
        // Assert
        notes.Count.ShouldBe(12);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("C#4");
        notes[2].ToString().ShouldBe("D4");
        notes[3].ToString().ShouldBe("D#4");
        notes[4].ToString().ShouldBe("E4");
        notes[5].ToString().ShouldBe("F4");
        notes[6].ToString().ShouldBe("F#4");
        notes[7].ToString().ShouldBe("G4");
        notes[8].ToString().ShouldBe("G#4");
        notes[9].ToString().ShouldBe("A4");
        notes[10].ToString().ShouldBe("A#4");
        notes[11].ToString().ShouldBe("B4");
    }

    [Fact]
    public void Scale_GetNotes_ForWholeToneScale_ShouldReturn6Notes()
    {
        // Arrange
        var wholeToneC = new Scale(new Note(NoteName.C), ScaleType.WholeTone);
        
        // Act
        var notes = wholeToneC.GetNotes().ToList();
        
        // Assert
        notes.Count.ShouldBe(6);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("D4");
        notes[2].ToString().ShouldBe("E4");
        notes[3].ToString().ShouldBe("F#4");
        notes[4].ToString().ShouldBe("G#4");
        notes[5].ToString().ShouldBe("A#4");
    }

    [Theory]
    [InlineData(ScaleType.Blues, new[] { "C4", "Eb4", "F4", "F#4", "G4", "Bb4" })]
    [InlineData(ScaleType.PentatonicMajor, new[] { "C4", "D4", "E4", "G4", "A4" })]
    [InlineData(ScaleType.PentatonicMinor, new[] { "C4", "Eb4", "F4", "G4", "Bb4" })]
    public void Scale_GetNotes_ForOtherScales_ShouldReturnCorrectNotes(ScaleType scaleType, string[] expectedNotes)
    {
        // Arrange
        var scale = new Scale(new Note(NoteName.C), scaleType);
        
        // Act
        var notes = scale.GetNotes().Select(n => n.ToString()).ToArray();
        
        // Assert
        notes.ShouldBe(expectedNotes);
    }

    [Fact]
    public void Scale_Transpose_ShouldTransposeAllNotes()
    {
        // Arrange
        var cMajor = new Scale(new Note(NoteName.C), ScaleType.Major);
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        
        // Act
        var dMajor = cMajor.Transpose(majorSecond);
        
        // Assert
        dMajor.Root.Name.ShouldBe(NoteName.D);
        var notes = dMajor.GetNotes().ToList();
        notes[0].ToString().ShouldBe("D4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("F#4");
        notes[3].ToString().ShouldBe("G4");
        notes[4].ToString().ShouldBe("A4");
        notes[5].ToString().ShouldBe("B4");
        notes[6].ToString().ShouldBe("C#5");
    }

    [Fact]
    public void Scale_Transpose_Down_ShouldTransposeCorrectly()
    {
        // Arrange
        var gMajor = new Scale(new Note(NoteName.G), ScaleType.Major);
        var perfectFourth = new Interval(IntervalQuality.Perfect, 4);
        
        // Act
        var dMajor = gMajor.Transpose(perfectFourth, Direction.Down);
        
        // Assert
        dMajor.Root.Name.ShouldBe(NoteName.D);
    }

    [Fact]
    public void Scale_Contains_WithEnharmonicNote_ShouldReturnTrue()
    {
        // Arrange
        var dFlatMajor = new Scale(new Note(NoteName.D, Alteration.Flat), ScaleType.Major);
        var cSharp = new Note(NoteName.C, Alteration.Sharp); // Enharmonic to Db
        var db = new Note(NoteName.D, Alteration.Flat);
        
        // Act & Assert
        dFlatMajor.Contains(db).ShouldBeTrue();
        dFlatMajor.Contains(cSharp).ShouldBeFalse(); // Strict comparison, not enharmonic
    }

    [Fact]
    public void Scale_GetDegree_WithNoteNotInScale_ShouldReturnNull()
    {
        // Arrange
        var cMajor = new Scale(new Note(NoteName.C), ScaleType.Major);
        var fSharp = new Note(NoteName.F, Alteration.Sharp);
        
        // Act
        var degree = cMajor.GetDegree(fSharp);
        
        // Assert
        degree.ShouldBeNull();
    }

    [Theory]
    [InlineData(NoteName.C, 1)]
    [InlineData(NoteName.D, 2)]
    [InlineData(NoteName.E, 3)]
    [InlineData(NoteName.F, 4)]
    [InlineData(NoteName.G, 5)]
    [InlineData(NoteName.A, 6)]
    [InlineData(NoteName.B, 7)]
    public void Scale_GetDegree_ForMajorScale_ShouldReturnCorrectDegree(NoteName noteName, int expectedDegree)
    {
        // Arrange
        var cMajor = new Scale(new Note(NoteName.C), ScaleType.Major);
        var note = new Note(noteName);
        
        // Act
        var degree = cMajor.GetDegree(note);
        
        // Assert
        degree.ShouldBe(expectedDegree);
    }

    [Fact]
    public void Scale_GetNotes_WithDifferentOctave_ShouldRespectRootOctave()
    {
        // Arrange
        var cMajorOctave5 = new Scale(new Note(NoteName.C, Alteration.Natural, 5), ScaleType.Major);
        
        // Act
        var notes = cMajorOctave5.GetNotes().ToList();
        
        // Assert
        notes[0].Octave.ShouldBe(5); // C5
        notes[6].Octave.ShouldBe(5); // B5 (7th note)
        notes.Last().Octave.ShouldBe(6); // C6 (octave)
    }
}