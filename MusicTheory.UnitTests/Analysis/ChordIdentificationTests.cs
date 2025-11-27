namespace MusicTheory.UnitTests.Analysis;

using MusicTheory.Analysis;

public class ChordIdentificationTests
{
    [Fact]
    public void ChordIdentification_ShouldHave_IdentifiedChordProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);

        // Act
        var identification = new ChordIdentification(chord, ChordInversion.Root, 1.0);

        // Assert
        identification.IdentifiedChord.ShouldBe(chord);
    }

    [Fact]
    public void ChordIdentification_ShouldHave_InversionProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);

        // Act
        var identification = new ChordIdentification(chord, ChordInversion.First, 0.9);

        // Assert
        identification.Inversion.ShouldBe(ChordInversion.First);
    }

    [Fact]
    public void ChordIdentification_ShouldHave_ConfidenceProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);

        // Act
        var identification = new ChordIdentification(chord, ChordInversion.Root, 0.85);

        // Assert
        identification.Confidence.ShouldBe(0.85);
    }

    [Fact]
    public void ChordIdentification_ShouldHave_UnmatchedNotesProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var unmatchedNotes = new List<Note> { new Note(NoteName.F) };

        // Act
        var identification = new ChordIdentification(chord, ChordInversion.Root, 0.8, unmatchedNotes);

        // Assert
        identification.UnmatchedNotes.ShouldNotBeEmpty();
        identification.UnmatchedNotes.Count.ShouldBe(1);
        identification.UnmatchedNotes[0].Name.ShouldBe(NoteName.F);
    }

    [Fact]
    public void ChordIdentification_ShouldHave_EmptyUnmatchedNotes_WhenNotProvided()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);

        // Act
        var identification = new ChordIdentification(chord, ChordInversion.Root, 1.0);

        // Assert
        identification.UnmatchedNotes.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void ChordIdentification_ShouldAccept_ValidConfidenceValues(double confidence)
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);

        // Act
        var identification = new ChordIdentification(chord, ChordInversion.Root, confidence);

        // Assert
        identification.Confidence.ShouldBe(confidence);
    }
}
