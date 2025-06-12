namespace MusicTheory.UnitTests;

public class KeySignatureTests
{
    [Fact]
    public void KeySignature_ShouldHaveTonicAndMode_WhenCreated()
    {
        // Arrange & Act
        var keySignature = new KeySignature(new Note(NoteName.C), KeyMode.Major);
        
        // Assert
        keySignature.Tonic.Name.ShouldBe(NoteName.C);
        keySignature.Mode.ShouldBe(KeyMode.Major);
    }

    [Theory]
    [InlineData(NoteName.C, KeyMode.Major, 0, new NoteName[] { })]
    [InlineData(NoteName.G, KeyMode.Major, 1, new[] { NoteName.F })] // F#
    [InlineData(NoteName.D, KeyMode.Major, 2, new[] { NoteName.F, NoteName.C })] // F#, C#
    [InlineData(NoteName.A, KeyMode.Major, 3, new[] { NoteName.F, NoteName.C, NoteName.G })] // F#, C#, G#
    [InlineData(NoteName.E, KeyMode.Major, 4, new[] { NoteName.F, NoteName.C, NoteName.G, NoteName.D })] // F#, C#, G#, D#
    [InlineData(NoteName.B, KeyMode.Major, 5, new[] { NoteName.F, NoteName.C, NoteName.G, NoteName.D, NoteName.A })] // F#, C#, G#, D#, A#
    [InlineData(NoteName.F, KeyMode.Major, -1, new[] { NoteName.B })] // Bb
    public void KeySignature_ShouldCalculateCorrectSharpsAndFlats_ForMajorKeys(
        NoteName tonic, KeyMode mode, int expectedAccidentals, NoteName[] expectedAlteredNotes)
    {
        // Arrange
        var keySignature = new KeySignature(new Note(tonic), mode);
        
        // Act & Assert
        keySignature.AccidentalCount.ShouldBe(expectedAccidentals);
        keySignature.AlteredNotes.ShouldBe(expectedAlteredNotes);
    }

    [Theory]
    [InlineData(NoteName.A, KeyMode.Minor, 0, new NoteName[] { })]
    [InlineData(NoteName.E, KeyMode.Minor, 1, new[] { NoteName.F })] // F#
    [InlineData(NoteName.B, KeyMode.Minor, 2, new[] { NoteName.F, NoteName.C })] // F#, C#
    [InlineData(NoteName.D, KeyMode.Minor, -1, new[] { NoteName.B })] // Bb
    [InlineData(NoteName.G, KeyMode.Minor, -2, new[] { NoteName.B, NoteName.E })] // Bb, Eb
    public void KeySignature_ShouldCalculateCorrectSharpsAndFlats_ForMinorKeys(
        NoteName tonic, KeyMode mode, int expectedAccidentals, NoteName[] expectedAlteredNotes)
    {
        // Arrange
        var keySignature = new KeySignature(new Note(tonic), mode);
        
        // Act & Assert
        keySignature.AccidentalCount.ShouldBe(expectedAccidentals);
        keySignature.AlteredNotes.ShouldBe(expectedAlteredNotes);
    }

    [Fact]
    public void KeySignature_ShouldProvideAlterationForNote()
    {
        // Arrange
        var gMajor = new KeySignature(new Note(NoteName.G), KeyMode.Major);
        
        // Act & Assert
        gMajor.GetAlteration(NoteName.F).ShouldBe(Alteration.Sharp);
        gMajor.GetAlteration(NoteName.C).ShouldBe(Alteration.Natural);
        gMajor.GetAlteration(NoteName.G).ShouldBe(Alteration.Natural);
    }
}