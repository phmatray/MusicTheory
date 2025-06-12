namespace MusicTheory.UnitTests;

public class EnharmonicTests
{
    [Theory]
    [InlineData(NoteName.C, Alteration.Sharp, NoteName.D, Alteration.Flat)] // C# = Db
    [InlineData(NoteName.D, Alteration.Sharp, NoteName.E, Alteration.Flat)] // D# = Eb
    [InlineData(NoteName.F, Alteration.Sharp, NoteName.G, Alteration.Flat)] // F# = Gb
    [InlineData(NoteName.G, Alteration.Sharp, NoteName.A, Alteration.Flat)] // G# = Ab
    [InlineData(NoteName.A, Alteration.Sharp, NoteName.B, Alteration.Flat)] // A# = Bb
    [InlineData(NoteName.E, Alteration.Sharp, NoteName.F, Alteration.Natural)] // E# = F
    [InlineData(NoteName.B, Alteration.Sharp, NoteName.C, Alteration.Natural)] // B# = C
    [InlineData(NoteName.C, Alteration.Flat, NoteName.B, Alteration.Natural)] // Cb = B
    [InlineData(NoteName.F, Alteration.Flat, NoteName.E, Alteration.Natural)] // Fb = E
    public void Note_IsEnharmonicWith_ShouldReturnTrueForEnharmonicNotes(
        NoteName name1, Alteration alteration1, NoteName name2, Alteration alteration2)
    {
        // Arrange
        var note1 = new Note(name1, alteration1, 4);
        var note2 = new Note(name2, alteration2, 4);
        
        // Act & Assert
        note1.IsEnharmonicWith(note2).Should().BeTrue();
        note2.IsEnharmonicWith(note1).Should().BeTrue(); // Should be symmetric
    }

    [Theory]
    [InlineData(NoteName.C, Alteration.Natural, NoteName.D, Alteration.Natural)] // C != D
    [InlineData(NoteName.C, Alteration.Sharp, NoteName.D, Alteration.Sharp)] // C# != D#
    [InlineData(NoteName.E, Alteration.Natural, NoteName.F, Alteration.Sharp)] // E != F#
    public void Note_IsEnharmonicWith_ShouldReturnFalseForNonEnharmonicNotes(
        NoteName name1, Alteration alteration1, NoteName name2, Alteration alteration2)
    {
        // Arrange
        var note1 = new Note(name1, alteration1, 4);
        var note2 = new Note(name2, alteration2, 4);
        
        // Act & Assert
        note1.IsEnharmonicWith(note2).Should().BeFalse();
    }

    [Fact]
    public void Note_IsEnharmonicWith_ShouldIgnoreOctave()
    {
        // Arrange
        var cSharp3 = new Note(NoteName.C, Alteration.Sharp, 3);
        var dFlat5 = new Note(NoteName.D, Alteration.Flat, 5);
        
        // Act & Assert
        cSharp3.IsEnharmonicWith(dFlat5).Should().BeTrue();
    }

    [Theory]
    [InlineData(NoteName.C, Alteration.Sharp)] // C# -> Db
    [InlineData(NoteName.D, Alteration.Flat)] // Db -> C#
    [InlineData(NoteName.E, Alteration.Sharp)] // E# -> F
    [InlineData(NoteName.F, Alteration.Natural)] // F -> E#
    [InlineData(NoteName.B, Alteration.Natural)] // B -> Cb
    [InlineData(NoteName.C, Alteration.Natural)] // C -> B#
    public void Note_GetEnharmonicEquivalent_ShouldReturnCorrectEquivalent(
        NoteName name, Alteration alteration)
    {
        // Arrange
        var note = new Note(name, alteration, 4);
        
        // Act
        var equivalent = note.GetEnharmonicEquivalent();
        
        // Assert
        equivalent.Should().NotBeNull();
        note.IsEnharmonicWith(equivalent).Should().BeTrue();
        equivalent.Name.Should().NotBe(note.Name); // Should have different name
    }

    [Theory]
    [InlineData(NoteName.C, Alteration.Natural)] // C has B# as equivalent
    [InlineData(NoteName.D, Alteration.Natural)] // D has no simple equivalent
    [InlineData(NoteName.E, Alteration.Natural)] // E has Fb as equivalent
    [InlineData(NoteName.F, Alteration.Natural)] // F has E# as equivalent
    [InlineData(NoteName.G, Alteration.Natural)] // G has no simple equivalent
    [InlineData(NoteName.A, Alteration.Natural)] // A has no simple equivalent
    [InlineData(NoteName.B, Alteration.Natural)] // B has Cb as equivalent
    public void Note_GetEnharmonicEquivalent_ForNaturalNotes(NoteName name, Alteration alteration)
    {
        // Arrange
        var note = new Note(name, alteration, 4);
        
        // Act
        var equivalent = note.GetEnharmonicEquivalent();
        
        // Assert for notes that have equivalents
        if (name == NoteName.C || name == NoteName.E || name == NoteName.F || name == NoteName.B)
        {
            equivalent.Should().NotBeNull();
            note.IsEnharmonicWith(equivalent).Should().BeTrue();
        }
        else
        {
            // D, G, A don't have simple enharmonic equivalents (would need double sharps/flats)
            equivalent.Should().BeNull();
        }
    }

    [Fact]
    public void Note_GetAllEnharmonicEquivalents_ShouldReturnAllPossibleEquivalents()
    {
        // Arrange
        var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
        
        // Act
        var equivalents = cSharp.GetAllEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.Should().Contain(n => n.Name == NoteName.D && n.Alteration == Alteration.Flat);
        equivalents.Should().Contain(n => n.Name == NoteName.B && n.Alteration == Alteration.DoubleSharp);
        equivalents.All(e => cSharp.IsEnharmonicWith(e)).Should().BeTrue();
    }

    [Theory]
    [InlineData(NoteName.C, Alteration.DoubleSharp, NoteName.D, Alteration.Natural)] // C## = D
    [InlineData(NoteName.D, Alteration.DoubleFlat, NoteName.C, Alteration.Natural)] // Dbb = C
    [InlineData(NoteName.F, Alteration.DoubleSharp, NoteName.G, Alteration.Natural)] // F## = G
    [InlineData(NoteName.G, Alteration.DoubleFlat, NoteName.F, Alteration.Natural)] // Gbb = F
    public void Note_IsEnharmonicWith_ShouldHandleDoubleAlterations(
        NoteName name1, Alteration alteration1, NoteName name2, Alteration alteration2)
    {
        // Arrange
        var note1 = new Note(name1, alteration1, 4);
        var note2 = new Note(name2, alteration2, 4);
        
        // Act & Assert
        note1.IsEnharmonicWith(note2).Should().BeTrue();
        note2.IsEnharmonicWith(note1).Should().BeTrue();
    }

    [Fact]
    public void Interval_IsEnharmonicWith_ShouldCompareIntervals()
    {
        // Arrange
        var augmentedFourth = new Interval(IntervalQuality.Augmented, 4);
        var diminishedFifth = new Interval(IntervalQuality.Diminished, 5);
        
        // Act & Assert
        augmentedFourth.IsEnharmonicWith(diminishedFifth).Should().BeTrue();
        diminishedFifth.IsEnharmonicWith(augmentedFourth).Should().BeTrue();
    }

    [Fact]
    public void Chord_GetEnharmonicEquivalent_ShouldReturnEquivalentChord()
    {
        // Arrange
        var cSharpMajor = new Chord(new Note(NoteName.C, Alteration.Sharp), ChordQuality.Major);
        
        // Act
        var equivalent = cSharpMajor.GetEnharmonicEquivalent();
        
        // Assert
        equivalent.Should().NotBeNull();
        equivalent.Root.Name.Should().Be(NoteName.D);
        equivalent.Root.Alteration.Should().Be(Alteration.Flat);
        equivalent.Quality.Should().Be(ChordQuality.Major);
    }

    [Theory]
    [InlineData("C#4", "Db4")]
    [InlineData("Db4", "C#4")]
    [InlineData("E#4", "F4")]
    [InlineData("Fb4", "E4")]
    [InlineData("B#4", "C5")]
    [InlineData("Cb4", "B3")]
    public void Note_SimplifyEnharmonic_ShouldPreferSimpleNotation(string input, string expected)
    {
        // Arrange
        var note = Note.Parse(input);
        
        // Act
        var simplified = note.SimplifyEnharmonic();
        
        // Assert
        simplified.ToString().Should().Be(expected);
    }
}