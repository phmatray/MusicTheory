namespace MusicTheory.UnitTests;

public class NoteEnharmonicTests
{
    [Theory]
    [InlineData("C#4", "C#4")] // Already simplified
    [InlineData("Db4", "Db4")] // Already simplified
    [InlineData("E#4", "F4")]  // E# simplifies to F
    [InlineData("Fb4", "E4")]  // Fb simplifies to E
    [InlineData("B#4", "C5")]  // B# simplifies to C (octave up)
    [InlineData("Cb4", "B3")]  // Cb simplifies to B (octave down)
    public void Note_SimplifyEnharmonic_ShouldReturnSimplifiedNote(string input, string expected)
    {
        // Arrange
        var note = Note.Parse(input);
        
        // Act
        var simplified = note.SimplifyEnharmonic();
        
        // Assert
        simplified.ToString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("C##4", "D4")]   // C double sharp = D
    [InlineData("Dbb4", "C4")]   // D double flat = C
    [InlineData("E##4", "F#4")]  // E double sharp = F#
    [InlineData("Fbb4", "Eb4")]  // F double flat = Eb
    [InlineData("G##4", "A4")]   // G double sharp = A
    [InlineData("Abb4", "G4")]   // A double flat = G
    public void Note_SimplifyEnharmonic_WithDoubleAlterations_ShouldSimplify(string input, string expected)
    {
        // Arrange
        var note = Note.Parse(input);
        
        // Act
        var simplified = note.SimplifyEnharmonic();
        
        // Assert
        simplified.ToString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("C4")]
    [InlineData("D4")]
    [InlineData("E4")]
    [InlineData("F4")]
    [InlineData("G4")]
    [InlineData("A4")]
    [InlineData("B4")]
    public void Note_SimplifyEnharmonic_WithNaturalNotes_ShouldReturnSame(string noteString)
    {
        // Arrange
        var note = Note.Parse(noteString);
        
        // Act
        var simplified = note.SimplifyEnharmonic();
        
        // Assert
        simplified.ShouldBeSameAs(note);
    }

    [Theory]
    [InlineData(1, true, "Db-1")]   // MIDI 1 with preferFlats
    [InlineData(3, true, "Eb-1")]   // MIDI 3 with preferFlats
    [InlineData(6, true, "Gb-1")]   // MIDI 6 with preferFlats
    [InlineData(8, true, "Ab-1")]   // MIDI 8 with preferFlats
    [InlineData(10, true, "Bb-1")]  // MIDI 10 with preferFlats
    [InlineData(13, true, "Db0")]   // MIDI 13 with preferFlats
    [InlineData(61, true, "Db4")]   // MIDI 61 with preferFlats (Middle C# as Db)
    public void Note_FromMidiNumber_WithPreferFlats_ShouldReturnFlatNotes(int midiNumber, bool preferFlats, string expected)
    {
        // Act
        var note = Note.FromMidiNumber(midiNumber, preferFlats);
        
        // Assert
        note.ToString().ShouldBe(expected);
    }

    [Fact]
    public void Note_GetEnharmonicEquivalent_ForNaturalNotes_WithNoSimpleEquivalent()
    {
        // Arrange
        var d = new Note(NoteName.D);
        var g = new Note(NoteName.G);
        var a = new Note(NoteName.A);
        
        // Act & Assert
        d.GetEnharmonicEquivalent().ShouldBeNull();
        g.GetEnharmonicEquivalent().ShouldBeNull();
        a.GetEnharmonicEquivalent().ShouldBeNull();
    }

    [Theory]
    [InlineData(NoteName.C, Alteration.DoubleSharp, NoteName.D, Alteration.Natural)]
    [InlineData(NoteName.D, Alteration.DoubleFlat, NoteName.C, Alteration.Natural)]
    [InlineData(NoteName.E, Alteration.DoubleSharp, NoteName.F, Alteration.Sharp)]
    [InlineData(NoteName.F, Alteration.DoubleFlat, NoteName.E, Alteration.Flat)]
    [InlineData(NoteName.B, Alteration.DoubleSharp, NoteName.C, Alteration.Sharp)]
    public void Note_GetEnharmonicEquivalent_WithDoubleAlterations_ShouldReturnCorrect(
        NoteName name, Alteration alteration, NoteName expectedName, Alteration expectedAlteration)
    {
        // Arrange
        var note = new Note(name, alteration, 4);
        
        // Act
        var equivalent = note.GetEnharmonicEquivalent();
        
        // Assert
        equivalent.ShouldNotBeNull();
        equivalent!.Name.ShouldBe(expectedName);
        equivalent.Alteration.ShouldBe(expectedAlteration);
    }

    [Fact]
    public void Note_GetAllEnharmonicEquivalents_ShouldNotIncludeSelf()
    {
        // Arrange
        var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
        
        // Act
        var equivalents = cSharp.GetAllEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.ShouldNotContain(n => n.Name == NoteName.C && n.Alteration == Alteration.Sharp);
    }

    [Fact]
    public void Note_GetAllEnharmonicEquivalents_ForC_ShouldReturnBSharpAndDDoubleFlat()
    {
        // Arrange
        var c = new Note(NoteName.C, Alteration.Natural, 4);
        
        // Act
        var equivalents = c.GetAllEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.ShouldContain(n => n.Name == NoteName.B && n.Alteration == Alteration.Sharp);
        equivalents.ShouldContain(n => n.Name == NoteName.D && n.Alteration == Alteration.DoubleFlat);
    }

    [Theory]
    [InlineData("", "Note string cannot be empty.")]
    [InlineData("   ", "Note string cannot be empty.")]
    [InlineData("H", "Invalid note name: H")]
    [InlineData("C$", "Invalid octave: $")]
    [InlineData("C#b", "Invalid octave: b")]
    [InlineData("Csharp", "Invalid octave: sharp")]
    public void Note_Parse_WithInvalidInput_ShouldThrowArgumentException(string input, string expectedMessage)
    {
        // Act
        Action act = () => Note.Parse(input);
        
        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldStartWith(expectedMessage);
    }

    [Theory]
    [InlineData("C", NoteName.C, Alteration.Natural, 4)]
    [InlineData("D5", NoteName.D, Alteration.Natural, 5)]
    [InlineData("E#3", NoteName.E, Alteration.Sharp, 3)]
    [InlineData("Fb2", NoteName.F, Alteration.Flat, 2)]
    [InlineData("G##6", NoteName.G, Alteration.DoubleSharp, 6)]
    [InlineData("Abb1", NoteName.A, Alteration.DoubleFlat, 1)]
    [InlineData("B-1", NoteName.B, Alteration.Natural, -1)]
    public void Note_Parse_WithValidInput_ShouldParseCorrectly(
        string input, NoteName expectedName, Alteration expectedAlteration, int expectedOctave)
    {
        // Act
        var note = Note.Parse(input);
        
        // Assert
        note.Name.ShouldBe(expectedName);
        note.Alteration.ShouldBe(expectedAlteration);
        note.Octave.ShouldBe(expectedOctave);
    }

    [Fact]
    public void Note_Parse_ShouldHandleMultipleAlterations()
    {
        // Act & Assert
        Note.Parse("C#").Alteration.ShouldBe(Alteration.Sharp);
        Note.Parse("C##").Alteration.ShouldBe(Alteration.DoubleSharp);
        Note.Parse("Cb").Alteration.ShouldBe(Alteration.Flat);
        Note.Parse("Cbb").Alteration.ShouldBe(Alteration.DoubleFlat);
    }
}