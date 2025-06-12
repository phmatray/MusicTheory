namespace MusicTheory.UnitTests;

public class ScaleTests
{
    [Fact]
    public void Scale_ShouldHaveRootAndType_WhenCreated()
    {
        // Arrange & Act
        var scale = new Scale(new Note(NoteName.C), ScaleType.Major);
        
        // Assert
        scale.Root.Name.Should().Be(NoteName.C);
        scale.Type.Should().Be(ScaleType.Major);
    }

    [Fact]
    public void Scale_ShouldGenerateCorrectNotes_ForCMajor()
    {
        // Arrange
        var scale = new Scale(new Note(NoteName.C, Alteration.Natural, 4), ScaleType.Major);
        
        // Act
        var notes = scale.GetNotes().ToList();
        
        // Assert
        notes.Should().HaveCount(8); // Including octave
        notes[0].Name.Should().Be(NoteName.C);
        notes[1].Name.Should().Be(NoteName.D);
        notes[2].Name.Should().Be(NoteName.E);
        notes[3].Name.Should().Be(NoteName.F);
        notes[4].Name.Should().Be(NoteName.G);
        notes[5].Name.Should().Be(NoteName.A);
        notes[6].Name.Should().Be(NoteName.B);
        notes[7].Name.Should().Be(NoteName.C);
        notes[7].Octave.Should().Be(5);
    }

    [Fact]
    public void Scale_ShouldGenerateCorrectNotes_ForANaturalMinor()
    {
        // Arrange
        var scale = new Scale(new Note(NoteName.A, Alteration.Natural, 4), ScaleType.NaturalMinor);
        
        // Act
        var notes = scale.GetNotes().ToList();
        
        // Assert
        notes.Should().HaveCount(8);
        notes[0].Name.Should().Be(NoteName.A);
        notes[1].Name.Should().Be(NoteName.B);
        notes[2].Name.Should().Be(NoteName.C);
        notes[3].Name.Should().Be(NoteName.D);
        notes[4].Name.Should().Be(NoteName.E);
        notes[5].Name.Should().Be(NoteName.F);
        notes[6].Name.Should().Be(NoteName.G);
        notes[7].Name.Should().Be(NoteName.A);
    }

    [Fact]
    public void Scale_ShouldGenerateCorrectNotes_ForGMajor()
    {
        // Arrange
        var scale = new Scale(new Note(NoteName.G, Alteration.Natural, 4), ScaleType.Major);
        
        // Act
        var notes = scale.GetNotes().ToList();
        
        // Assert
        notes.Should().HaveCount(8);
        notes[0].Name.Should().Be(NoteName.G);
        notes[0].Alteration.Should().Be(Alteration.Natural);
        notes[1].Name.Should().Be(NoteName.A);
        notes[1].Alteration.Should().Be(Alteration.Natural);
        notes[2].Name.Should().Be(NoteName.B);
        notes[2].Alteration.Should().Be(Alteration.Natural);
        notes[3].Name.Should().Be(NoteName.C);
        notes[3].Alteration.Should().Be(Alteration.Natural);
        notes[4].Name.Should().Be(NoteName.D);
        notes[4].Alteration.Should().Be(Alteration.Natural);
        notes[5].Name.Should().Be(NoteName.E);
        notes[5].Alteration.Should().Be(Alteration.Natural);
        notes[6].Name.Should().Be(NoteName.F);
        notes[6].Alteration.Should().Be(Alteration.Sharp); // F# in G major
        notes[7].Name.Should().Be(NoteName.G);
    }
}