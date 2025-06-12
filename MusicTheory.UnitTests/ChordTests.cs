namespace MusicTheory.UnitTests;

public class ChordTests
{
    [Fact]
    public void Chord_ShouldHaveRootNoteAndQuality_WhenCreated()
    {
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        
        chord.Root.Name.Should().Be(NoteName.C);
        chord.Quality.Should().Be(ChordQuality.Major);
    }

    [Fact]
    public void Chord_ShouldGenerateCorrectNotes_ForMajorTriad()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major);
        
        var notes = chord.GetNotes().ToList();
        
        notes.Should().HaveCount(3);
        notes[0].Name.Should().Be(NoteName.C);
        notes[0].Octave.Should().Be(4);
        notes[1].Name.Should().Be(NoteName.E);
        notes[1].Octave.Should().Be(4);
        notes[2].Name.Should().Be(NoteName.G);
        notes[2].Octave.Should().Be(4);
    }

    [Fact]
    public void Chord_ShouldGenerateCorrectNotes_ForMinorTriad()
    {
        var chord = new Chord(new Note(NoteName.A, Alteration.Natural, 4), ChordQuality.Minor);
        
        var notes = chord.GetNotes().ToList();
        
        notes.Should().HaveCount(3);
        notes[0].Name.Should().Be(NoteName.A);
        notes[0].Octave.Should().Be(4);
        notes[1].Name.Should().Be(NoteName.C);
        notes[1].Octave.Should().Be(5);
        notes[2].Name.Should().Be(NoteName.E);
        notes[2].Octave.Should().Be(5);
    }

    [Fact]
    public void Chord_ShouldSupportExtensions_ForSeventhChord()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .AddExtension(7, IntervalQuality.Major);
        
        var notes = chord.GetNotes().ToList();
        
        notes.Should().HaveCount(4);
        notes[3].Name.Should().Be(NoteName.B);
        notes[3].Octave.Should().Be(4);
    }

    [Fact]
    public void Chord_ShouldSupportMultipleExtensions()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordQuality.Major)
            .AddExtension(7, IntervalQuality.Major)
            .AddExtension(9, IntervalQuality.Major);
        
        var notes = chord.GetNotes().ToList();
        
        notes.Should().HaveCount(5);
        notes[3].Name.Should().Be(NoteName.B);
        notes[3].Octave.Should().Be(4);
        notes[4].Name.Should().Be(NoteName.D);
        notes[4].Octave.Should().Be(5);
    }
}