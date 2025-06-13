namespace MusicTheory.UnitTests;

public class ChordTypesTests
{
    #region Triads
    
    [Fact]
    public void Major_Triad_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(3);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
    }
    
    [Fact]
    public void Minor_Triad_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.Minor);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(3);
        notes[0].ToString().ShouldBe("D4");
        notes[1].ToString().ShouldBe("F4");
        notes[2].ToString().ShouldBe("A4");
    }
    
    [Fact]
    public void Diminished_Triad_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.B, Alteration.Natural, 4), ChordType.Diminished);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(3);
        notes[0].ToString().ShouldBe("B4");
        notes[1].ToString().ShouldBe("D5");
        notes[2].ToString().ShouldBe("F5");
    }
    
    [Fact]
    public void Augmented_Triad_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Augmented);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(3);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G#4");
    }
    
    #endregion
    
    #region Seventh Chords
    
    [Fact]
    public void Major7_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major7);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("B4");
    }
    
    [Fact]
    public void Minor7_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.Minor7);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("D4");
        notes[1].ToString().ShouldBe("F4");
        notes[2].ToString().ShouldBe("A4");
        notes[3].ToString().ShouldBe("C5");
    }
    
    [Fact]
    public void Dominant7_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.G, Alteration.Natural, 4), ChordType.Dominant7);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("G4");
        notes[1].ToString().ShouldBe("B4");
        notes[2].ToString().ShouldBe("D5");
        notes[3].ToString().ShouldBe("F5");
    }
    
    [Fact]
    public void MinorMajor7_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.MinorMajor7);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("Eb4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("B4");
    }
    
    [Fact]
    public void HalfDiminished7_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.B, Alteration.Natural, 4), ChordType.HalfDiminished7);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("B4");
        notes[1].ToString().ShouldBe("D5");
        notes[2].ToString().ShouldBe("F5");
        notes[3].ToString().ShouldBe("A5");
    }
    
    [Fact]
    public void Diminished7_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Diminished7);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("Eb4");
        notes[2].ToString().ShouldBe("Gb4");
        notes[3].ToString().ShouldBe("Bbb4"); // Diminished 7th from C is Bbb (double flat B)
    }
    
    [Fact]
    public void Augmented7_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Augmented7);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G#4");
        notes[3].ToString().ShouldBe("Bb4");
    }
    
    [Fact]
    public void AugmentedMajor7_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.AugmentedMajor7);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G#4");
        notes[3].ToString().ShouldBe("B4");
    }
    
    #endregion
    
    #region Suspended Chords
    
    [Fact]
    public void Sus2_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Sus2);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(3);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("D4");
        notes[2].ToString().ShouldBe("G4");
    }
    
    [Fact]
    public void Sus4_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Sus4);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(3);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("F4");
        notes[2].ToString().ShouldBe("G4");
    }
    
    [Fact]
    public void Sus2Sus4_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Sus2Sus4);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("D4");
        notes[2].ToString().ShouldBe("F4");
        notes[3].ToString().ShouldBe("G4");
    }
    
    [Fact]
    public void Dominant7Sus4_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.G, Alteration.Natural, 4), ChordType.Dominant7Sus4);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("G4");
        notes[1].ToString().ShouldBe("C5");
        notes[2].ToString().ShouldBe("D5");
        notes[3].ToString().ShouldBe("F5");
    }
    
    #endregion
    
    #region Sixth Chords
    
    [Fact]
    public void Major6_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major6);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("A4");
    }
    
    [Fact]
    public void Minor6_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Minor6);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("Eb4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("A4");
    }
    
    [Fact]
    public void Major6Add9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major6Add9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("A4");
        notes[4].ToString().ShouldBe("D5");
    }
    
    #endregion
    
    #region Extended Chords
    
    [Fact]
    public void Major9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("B4");
        notes[4].ToString().ShouldBe("D5");
    }
    
    [Fact]
    public void Minor9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.Minor9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("D4");
        notes[1].ToString().ShouldBe("F4");
        notes[2].ToString().ShouldBe("A4");
        notes[3].ToString().ShouldBe("C5");
        notes[4].ToString().ShouldBe("E5");
    }
    
    [Fact]
    public void Dominant9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.G, Alteration.Natural, 4), ChordType.Dominant9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("G4");
        notes[1].ToString().ShouldBe("B4");
        notes[2].ToString().ShouldBe("D5");
        notes[3].ToString().ShouldBe("F5");
        notes[4].ToString().ShouldBe("A5");
    }
    
    [Fact]
    public void Major11_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major11);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(6);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("B4");
        notes[4].ToString().ShouldBe("D5");
        notes[5].ToString().ShouldBe("F5");
    }
    
    [Fact]
    public void Minor11_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.Minor11);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(6);
        notes[0].ToString().ShouldBe("D4");
        notes[1].ToString().ShouldBe("F4");
        notes[2].ToString().ShouldBe("A4");
        notes[3].ToString().ShouldBe("C5");
        notes[4].ToString().ShouldBe("E5");
        notes[5].ToString().ShouldBe("G5");
    }
    
    [Fact]
    public void Dominant11_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.G, Alteration.Natural, 4), ChordType.Dominant11);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(6);
        notes[0].ToString().ShouldBe("G4");
        notes[1].ToString().ShouldBe("B4");
        notes[2].ToString().ShouldBe("D5");
        notes[3].ToString().ShouldBe("F5");
        notes[4].ToString().ShouldBe("A5");
        notes[5].ToString().ShouldBe("C6");
    }
    
    [Fact]
    public void Major13_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major13);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(7);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("B4");
        notes[4].ToString().ShouldBe("D5");
        notes[5].ToString().ShouldBe("F5");
        notes[6].ToString().ShouldBe("A5");
    }
    
    [Fact]
    public void Minor13_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.Minor13);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(7);
        notes[0].ToString().ShouldBe("D4");
        notes[1].ToString().ShouldBe("F4");
        notes[2].ToString().ShouldBe("A4");
        notes[3].ToString().ShouldBe("C5");
        notes[4].ToString().ShouldBe("E5");
        notes[5].ToString().ShouldBe("G5");
        notes[6].ToString().ShouldBe("B5");
    }
    
    [Fact]
    public void Dominant13_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.G, Alteration.Natural, 4), ChordType.Dominant13);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(7);
        notes[0].ToString().ShouldBe("G4");
        notes[1].ToString().ShouldBe("B4");
        notes[2].ToString().ShouldBe("D5");
        notes[3].ToString().ShouldBe("F5");
        notes[4].ToString().ShouldBe("A5");
        notes[5].ToString().ShouldBe("C6");
        notes[6].ToString().ShouldBe("E6");
    }
    
    #endregion
    
    #region Add Chords
    
    [Fact]
    public void MajorAdd9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.MajorAdd9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("D5");
    }
    
    [Fact]
    public void MinorAdd9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.MinorAdd9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("D4");
        notes[1].ToString().ShouldBe("F4");
        notes[2].ToString().ShouldBe("A4");
        notes[3].ToString().ShouldBe("E5");
    }
    
    [Fact]
    public void MajorAdd11_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.MajorAdd11);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G4");
        notes[3].ToString().ShouldBe("F5");
    }
    
    [Fact]
    public void MinorAdd11_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.MinorAdd11);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("D4");
        notes[1].ToString().ShouldBe("F4");
        notes[2].ToString().ShouldBe("A4");
        notes[3].ToString().ShouldBe("G5");
    }
    
    #endregion
    
    #region Altered Chords
    
    [Fact]
    public void Dominant7Flat5_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Dominant7Flat5);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("Gb4");
        notes[3].ToString().ShouldBe("Bb4");
    }
    
    [Fact]
    public void Dominant7Sharp5_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Dominant7Sharp5);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(4);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G#4");
        notes[3].ToString().ShouldBe("Bb4");
    }
    
    [Fact]
    public void Dominant7Flat9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.G, Alteration.Natural, 4), ChordType.Dominant7Flat9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("G4");
        notes[1].ToString().ShouldBe("B4");
        notes[2].ToString().ShouldBe("D5");
        notes[3].ToString().ShouldBe("F5");
        notes[4].ToString().ShouldBe("Ab5");
    }
    
    [Fact]
    public void Dominant7Sharp9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.G, Alteration.Natural, 4), ChordType.Dominant7Sharp9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("G4");
        notes[1].ToString().ShouldBe("B4");
        notes[2].ToString().ShouldBe("D5");
        notes[3].ToString().ShouldBe("F5");
        notes[4].ToString().ShouldBe("A#5");
    }
    
    [Fact]
    public void Dominant7Flat5Flat9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Dominant7Flat5Flat9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("Gb4");
        notes[3].ToString().ShouldBe("Bb4");
        notes[4].ToString().ShouldBe("Db5");
    }
    
    [Fact]
    public void Dominant7Flat5Sharp9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Dominant7Flat5Sharp9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("Gb4");
        notes[3].ToString().ShouldBe("Bb4");
        notes[4].ToString().ShouldBe("D#5");
    }
    
    [Fact]
    public void Dominant7Sharp5Flat9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Dominant7Sharp5Flat9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G#4");
        notes[3].ToString().ShouldBe("Bb4");
        notes[4].ToString().ShouldBe("Db5");
    }
    
    [Fact]
    public void Dominant7Sharp5Sharp9_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Dominant7Sharp5Sharp9);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(5);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("G#4");
        notes[3].ToString().ShouldBe("Bb4");
        notes[4].ToString().ShouldBe("D#5");
    }
    
    [Fact]
    public void Dominant7Alt_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Dominant7Alt);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(6);
        notes[0].ToString().ShouldBe("C4");
        notes[1].ToString().ShouldBe("E4");
        notes[2].ToString().ShouldBe("Gb4");
        notes[3].ToString().ShouldBe("G#4");
        notes[4].ToString().ShouldBe("Bb4");
        notes[5].ToString().ShouldBe("Db5");
    }
    
    #endregion
    
    #region Power Chords
    
    [Fact]
    public void Power5_Chord_ShouldHaveCorrectIntervals()
    {
        var chord = new Chord(new Note(NoteName.A, Alteration.Natural, 4), ChordType.Power5);
        var notes = chord.GetNotes().ToList();
        
        notes.Count.ShouldBe(2);
        notes[0].ToString().ShouldBe("A4");
        notes[1].ToString().ShouldBe("E5");
    }
    
    #endregion
    
    #region Chord Symbol Generation
    
    [Theory]
    [InlineData(ChordType.Major, "C")]
    [InlineData(ChordType.Minor, "Cm")]
    [InlineData(ChordType.Diminished, "C°")]
    [InlineData(ChordType.Augmented, "C+")]
    [InlineData(ChordType.Major7, "Cmaj7")]
    [InlineData(ChordType.Minor7, "Cm7")]
    [InlineData(ChordType.Dominant7, "C7")]
    [InlineData(ChordType.MinorMajor7, "Cm(maj7)")]
    [InlineData(ChordType.HalfDiminished7, "Cø7")]
    [InlineData(ChordType.Diminished7, "C°7")]
    [InlineData(ChordType.Sus2, "Csus2")]
    [InlineData(ChordType.Sus4, "Csus4")]
    [InlineData(ChordType.Major6, "C6")]
    [InlineData(ChordType.Minor6, "Cm6")]
    [InlineData(ChordType.Major9, "Cmaj9")]
    [InlineData(ChordType.Minor9, "Cm9")]
    [InlineData(ChordType.Dominant9, "C9")]
    [InlineData(ChordType.MajorAdd9, "Cadd9")]
    [InlineData(ChordType.MinorAdd9, "Cm(add9)")]
    [InlineData(ChordType.Dominant7Flat5, "C7♭5")]
    [InlineData(ChordType.Dominant7Sharp5, "C7♯5")]
    [InlineData(ChordType.Dominant7Flat9, "C7♭9")]
    [InlineData(ChordType.Dominant7Sharp9, "C7♯9")]
    [InlineData(ChordType.Power5, "C5")]
    public void GetSymbol_ShouldReturnCorrectSymbol(ChordType chordType, string expectedSymbol)
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Natural, 4), chordType);
        chord.GetSymbol().ShouldBe(expectedSymbol);
    }
    
    #endregion
}