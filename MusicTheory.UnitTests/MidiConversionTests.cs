namespace MusicTheory.UnitTests;

public class MidiConversionTests
{
    [Theory]
    [InlineData(NoteName.C, Alteration.Natural, 4, 60)]  // Middle C
    [InlineData(NoteName.A, Alteration.Natural, 4, 69)]  // A440
    [InlineData(NoteName.C, Alteration.Natural, 0, 12)]  // C0
    [InlineData(NoteName.C, Alteration.Natural, -1, 0)]  // C-1 (lowest MIDI note)
    [InlineData(NoteName.G, Alteration.Natural, 9, 127)] // G9 (highest MIDI note)
    [InlineData(NoteName.C, Alteration.Sharp, 4, 61)]    // C#4
    [InlineData(NoteName.D, Alteration.Flat, 4, 61)]     // Db4 (same as C#4)
    [InlineData(NoteName.E, Alteration.Flat, 4, 63)]     // Eb4
    [InlineData(NoteName.B, Alteration.Natural, 3, 59)]  // B3
    public void Note_ShouldGetMidiNoteNumber(NoteName name, Alteration alteration, int octave, int expectedMidi)
    {
        // Arrange
        var note = new Note(name, alteration, octave);
        
        // Act
        var midiNumber = note.MidiNumber;
        
        // Assert
        midiNumber.ShouldBe(expectedMidi);
    }

    [Fact]
    public void Note_ShouldCreateFromMidiNumber_MiddleC()
    {
        // Arrange & Act
        var note = Note.FromMidiNumber(60);
        
        // Assert
        note.Name.ShouldBe(NoteName.C);
        note.Alteration.ShouldBe(Alteration.Natural);
        note.Octave.ShouldBe(4);
    }

    [Theory]
    [InlineData(69, NoteName.A, Alteration.Natural, 4)]  // A440
    [InlineData(0, NoteName.C, Alteration.Natural, -1)]  // C-1
    [InlineData(127, NoteName.G, Alteration.Natural, 9)] // G9
    [InlineData(61, NoteName.C, Alteration.Sharp, 4)]    // C#4
    [InlineData(63, NoteName.E, Alteration.Flat, 4)]     // Eb4
    public void Note_ShouldCreateFromMidiNumber(int midiNumber, NoteName expectedName, Alteration expectedAlteration, int expectedOctave)
    {
        // Arrange & Act
        var note = Note.FromMidiNumber(midiNumber);
        
        // Assert
        note.Name.ShouldBe(expectedName);
        note.Alteration.ShouldBe(expectedAlteration);
        note.Octave.ShouldBe(expectedOctave);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(128)]
    [InlineData(200)]
    public void Note_ShouldThrowForInvalidMidiNumber(int invalidMidiNumber)
    {
        // Act & Assert
        Action act = () => Note.FromMidiNumber(invalidMidiNumber);
        act.ShouldThrow<ArgumentOutOfRangeException>()
            .Message.ShouldContain("MIDI note number must be between 0 and 127");
    }

    [Fact]
    public void Note_ShouldPreferSharpsWhenCreatingFromMidiNumber()
    {
        // Arrange & Act
        var cSharp = Note.FromMidiNumber(61);
        var fSharp = Note.FromMidiNumber(66);
        
        // Assert
        cSharp.Name.ShouldBe(NoteName.C);
        cSharp.Alteration.ShouldBe(Alteration.Sharp);
        
        fSharp.Name.ShouldBe(NoteName.F);
        fSharp.Alteration.ShouldBe(Alteration.Sharp);
    }

    [Fact]
    public void Note_ShouldCreateFromMidiNumberWithPreferFlats()
    {
        // Arrange & Act
        var dFlat = Note.FromMidiNumber(61, preferFlats: true);
        var gFlat = Note.FromMidiNumber(66, preferFlats: true);
        
        // Assert
        dFlat.Name.ShouldBe(NoteName.D);
        dFlat.Alteration.ShouldBe(Alteration.Flat);
        
        gFlat.Name.ShouldBe(NoteName.G);
        gFlat.Alteration.ShouldBe(Alteration.Flat);
    }

    [Theory]
    [InlineData(NoteName.C, Alteration.DoubleSharp, 4, 62)] // C## = D
    [InlineData(NoteName.D, Alteration.DoubleFlat, 4, 60)]  // Dbb = C
    [InlineData(NoteName.E, Alteration.DoubleSharp, 4, 66)] // E## = F#
    [InlineData(NoteName.F, Alteration.DoubleFlat, 4, 63)]  // Fbb = Eb
    public void Note_ShouldHandleDoubleAlterationsInMidiNumber(NoteName name, Alteration alteration, int octave, int expectedMidi)
    {
        // Arrange
        var note = new Note(name, alteration, octave);
        
        // Act
        var midiNumber = note.MidiNumber;
        
        // Assert
        midiNumber.ShouldBe(expectedMidi);
    }

    [Fact]
    public void Note_ShouldRoundTripMidiConversion()
    {
        // Test all valid MIDI numbers
        for (int midi = 0; midi <= 127; midi++)
        {
            // Arrange & Act
            var note = Note.FromMidiNumber(midi);
            var midiNumber = note.MidiNumber;
            
            // Assert
            midiNumber.ShouldBe(midi, $"Failed for MIDI number {midi}");
        }
    }

    [Fact]
    public void Note_ShouldCalculateMidiNumberForExtremeCases()
    {
        // Arrange
        var lowestNote = new Note(NoteName.C, Alteration.Natural, -1);
        var highestNote = new Note(NoteName.G, Alteration.Natural, 9);
        
        // Act & Assert
        lowestNote.MidiNumber.ShouldBe(0);
        highestNote.MidiNumber.ShouldBe(127);
    }

    [Theory]
    [InlineData(NoteName.G, Alteration.Sharp, 9)]  // G#9 would be 128
    [InlineData(NoteName.A, Alteration.Natural, 9)] // A9 would be 129
    [InlineData(NoteName.C, Alteration.DoubleFlat, -1)]  // Cbb-1 would be -2
    public void Note_ShouldThrowForNotesOutsideMidiRange(NoteName name, Alteration alteration, int octave)
    {
        // Arrange
        var note = new Note(name, alteration, octave);
        
        // Act & Assert
        Action act = () => { var _ = note.MidiNumber; };
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("outside the valid MIDI range");
    }
}