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
    public void Chord_WithInversion_Undefined_ShouldThrow()
    {
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        Should.Throw<ArgumentOutOfRangeException>(() => chord.WithInversion((ChordInversion)99)).ParamName.ShouldBe("inversion");
    }

    [Fact]
    public void Chord_WithUndefinedQualityOrType_ShouldThrow()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Chord(new Note(NoteName.C), (ChordQuality)99)).ParamName.ShouldBe("quality");
        Should.Throw<ArgumentOutOfRangeException>(() => new Chord(new Note(NoteName.C), (ChordType)99)).ParamName.ShouldBe("type");
        Should.Throw<ArgumentNullException>(() => new Chord(null!, ChordType.Major));
    }

    [Fact]
    public void Chord_GetNotes_ThatNeedTripleAccidentals_ShouldThrow()
    {
        // B# augmented is B#, D##, F###: version 1 returned an "F" with an alteration of -9; 2.0 refuses.
        var chord = new Chord(new Note(NoteName.B, Alteration.Sharp, 4), ChordQuality.Augmented);

        Should.Throw<InvalidOperationException>(() => chord.GetNotes().ToList())
            .Message.ShouldBe("B#4 transposed by A5 needs 3 sharps on F; at most two are supported.");
    }
}