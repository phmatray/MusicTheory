namespace MusicTheory.UnitTests;

public class ScaleTests
{
    [Fact]
    public void Scale_ShouldHaveRootAndType_WhenCreated()
    {
        // Arrange & Act
        var scale = new Scale(new Note(NoteName.C), ScaleType.Major);
        
        // Assert
        scale.Root.Name.ShouldBe(NoteName.C);
        scale.Type.ShouldBe(ScaleType.Major);
    }

    [Fact]
    public void Scale_ShouldGenerateCorrectNotes_ForCMajor()
    {
        // Arrange
        var scale = new Scale(new Note(NoteName.C, Alteration.Natural, 4), ScaleType.Major);
        
        // Act
        var notes = scale.GetNotes().ToList();
        
        // Assert
        notes.Count.ShouldBe(8); // Including octave
        notes[0].Name.ShouldBe(NoteName.C);
        notes[1].Name.ShouldBe(NoteName.D);
        notes[2].Name.ShouldBe(NoteName.E);
        notes[3].Name.ShouldBe(NoteName.F);
        notes[4].Name.ShouldBe(NoteName.G);
        notes[5].Name.ShouldBe(NoteName.A);
        notes[6].Name.ShouldBe(NoteName.B);
        notes[7].Name.ShouldBe(NoteName.C);
        notes[7].Octave.ShouldBe(5);
    }

    [Fact]
    public void Scale_ShouldGenerateCorrectNotes_ForANaturalMinor()
    {
        // Arrange
        var scale = new Scale(new Note(NoteName.A, Alteration.Natural, 4), ScaleType.NaturalMinor);
        
        // Act
        var notes = scale.GetNotes().ToList();
        
        // Assert
        notes.Count.ShouldBe(8);
        notes[0].Name.ShouldBe(NoteName.A);
        notes[1].Name.ShouldBe(NoteName.B);
        notes[2].Name.ShouldBe(NoteName.C);
        notes[3].Name.ShouldBe(NoteName.D);
        notes[4].Name.ShouldBe(NoteName.E);
        notes[5].Name.ShouldBe(NoteName.F);
        notes[6].Name.ShouldBe(NoteName.G);
        notes[7].Name.ShouldBe(NoteName.A);
    }

    [Fact]
    public void Scale_ShouldGenerateCorrectNotes_ForGMajor()
    {
        // Arrange
        var scale = new Scale(new Note(NoteName.G, Alteration.Natural, 4), ScaleType.Major);
        
        // Act
        var notes = scale.GetNotes().ToList();
        
        // Assert
        notes.Count.ShouldBe(8);
        notes[0].Name.ShouldBe(NoteName.G);
        notes[0].Alteration.ShouldBe(Alteration.Natural);
        notes[1].Name.ShouldBe(NoteName.A);
        notes[1].Alteration.ShouldBe(Alteration.Natural);
        notes[2].Name.ShouldBe(NoteName.B);
        notes[2].Alteration.ShouldBe(Alteration.Natural);
        notes[3].Name.ShouldBe(NoteName.C);
        notes[3].Alteration.ShouldBe(Alteration.Natural);
        notes[4].Name.ShouldBe(NoteName.D);
        notes[4].Alteration.ShouldBe(Alteration.Natural);
        notes[5].Name.ShouldBe(NoteName.E);
        notes[5].Alteration.ShouldBe(Alteration.Natural);
        notes[6].Name.ShouldBe(NoteName.F);
        notes[6].Alteration.ShouldBe(Alteration.Sharp); // F# in G major
        notes[7].Name.ShouldBe(NoteName.G);
    }
}