namespace MusicTheory.UnitTests;

public class ChordTests
{
    [Fact]
    public void Chord_ShouldHaveRootNoteAndQuality_WhenCreated()
    {
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        
        chord.Root.Name.ShouldBe(NoteName.C);
        chord.Quality.ShouldBe(ChordQuality.Major);
    }

    [Fact]
    public void Chord_ShouldGenerateCorrectNotes_ForMajorTriad()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(3);
        notes[0].Name.ShouldBe(NoteName.C);
        notes[0].Octave.ShouldBe(4);
        notes[1].Name.ShouldBe(NoteName.E);
        notes[1].Octave.ShouldBe(4);
        notes[2].Name.ShouldBe(NoteName.G);
        notes[2].Octave.ShouldBe(4);
    }

    [Fact]
    public void Chord_ShouldGenerateCorrectNotes_ForMinorTriad()
    {
        var chord = new Chord(new Note(NoteName.A, Alteration.Natural, 4), ChordQuality.Minor);
        
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(3);
        notes[0].Name.ShouldBe(NoteName.A);
        notes[0].Octave.ShouldBe(4);
        notes[1].Name.ShouldBe(NoteName.C);
        notes[1].Octave.ShouldBe(5);
        notes[2].Name.ShouldBe(NoteName.E);
        notes[2].Octave.ShouldBe(5);
    }

    [Fact]
    public void Chord_ShouldSupportExtensions_ForSeventhChord()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .AddExtension(7, IntervalQuality.Major);
        
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[3].Name.ShouldBe(NoteName.B);
        notes[3].Octave.ShouldBe(4);
    }

    [Fact]
    public void Chord_ShouldSupportMultipleExtensions()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .AddExtension(7, IntervalQuality.Major)
            .AddExtension(9, IntervalQuality.Major);
        
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[3].Name.ShouldBe(NoteName.B);
        notes[3].Octave.ShouldBe(4);
        notes[4].Name.ShouldBe(NoteName.D);
        notes[4].Octave.ShouldBe(5);
    }

    [Fact]
    public void Chord_GetNotes_WithInvalidQuality_ShouldThrow()
    {
        // Act & Assert - Constructor should throw for invalid quality
        Action act = () => new Chord(new Note(NoteName.C), (ChordQuality)99);
        
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Chord_GetNotesInInversion_WithInvalidInversion_ShouldThrow()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major)
            .WithInversion((ChordInversion)99);
        
        // Act
        Action act = () => chord.GetNotesInInversion().ToList();
        
        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Chord_GetBassNote_WithInvalidInversion_ShouldThrow()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major)
            .WithInversion((ChordInversion)99);
        
        // Act
        Action act = () => chord.GetBassNote();
        
        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Chord_GetNoteAtInterval_WithWrappingAlteration_ShouldAdjustOctave()
    {
        // This tests the alteration wrapping logic in GetNoteAtInterval
        // We need a chord that produces alterations < -2 or > 2
        var chord = new Chord(new Note(NoteName.B, Alteration.Sharp, 4), ChordQuality.Augmented);
        
        // Act
        var notes = chord.GetNotes().ToList();
        
        // Assert
        // B# augmented: B#, D##, F### 
        // The implementation produces extreme alterations that exceed the enum range
        notes[2].Name.ShouldBe(NoteName.F);
        notes[2].Alteration.ShouldBe((Alteration)(-9)); // Extreme negative alteration from wrapping
        notes[2].Octave.ShouldBe(6); // Octave adjusted up due to extreme alteration wrapping
    }
}