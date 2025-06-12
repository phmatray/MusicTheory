namespace MusicTheory.UnitTests;

public class ChordProgressionTests
{
    [Fact]
    public void ChordProgression_ShouldCreateWithKey()
    {
        // Arrange
        var key = new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), KeyMode.Major);
        
        // Act
        var progression = new ChordProgression(key);
        
        // Assert
        progression.Key.ShouldBe(key);
    }

    [Fact]
    public void ChordProgression_ShouldGetDiatonicChords_ForMajorKey()
    {
        // Arrange
        var cMajorKey = new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), KeyMode.Major);
        var progression = new ChordProgression(cMajorKey);
        
        // Act
        var diatonicChords = progression.GetDiatonicChords().ToList();
        
        // Assert
        diatonicChords.Count.ShouldBe(7);
        
        // I - C major
        diatonicChords[0].Root.Name.ShouldBe(NoteName.C);
        diatonicChords[0].Quality.ShouldBe(ChordQuality.Major);
        
        // ii - D minor
        diatonicChords[1].Root.Name.ShouldBe(NoteName.D);
        diatonicChords[1].Quality.ShouldBe(ChordQuality.Minor);
        
        // iii - E minor
        diatonicChords[2].Root.Name.ShouldBe(NoteName.E);
        diatonicChords[2].Quality.ShouldBe(ChordQuality.Minor);
        
        // IV - F major
        diatonicChords[3].Root.Name.ShouldBe(NoteName.F);
        diatonicChords[3].Quality.ShouldBe(ChordQuality.Major);
        
        // V - G major
        diatonicChords[4].Root.Name.ShouldBe(NoteName.G);
        diatonicChords[4].Quality.ShouldBe(ChordQuality.Major);
        
        // vi - A minor
        diatonicChords[5].Root.Name.ShouldBe(NoteName.A);
        diatonicChords[5].Quality.ShouldBe(ChordQuality.Minor);
        
        // vii° - B diminished
        diatonicChords[6].Root.Name.ShouldBe(NoteName.B);
        diatonicChords[6].Quality.ShouldBe(ChordQuality.Diminished);
    }

    [Fact]
    public void ChordProgression_ShouldGetRomanNumeral_ForDegree()
    {
        // Arrange
        var cMajorKey = new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), KeyMode.Major);
        var progression = new ChordProgression(cMajorKey);
        
        // Act & Assert
        progression.GetRomanNumeral(1).ShouldBe("I");
        progression.GetRomanNumeral(2).ShouldBe("ii");
        progression.GetRomanNumeral(3).ShouldBe("iii");
        progression.GetRomanNumeral(4).ShouldBe("IV");
        progression.GetRomanNumeral(5).ShouldBe("V");
        progression.GetRomanNumeral(6).ShouldBe("vi");
        progression.GetRomanNumeral(7).ShouldBe("vii°");
    }

    [Fact]
    public void ChordProgression_ShouldGetChordByDegree()
    {
        // Arrange
        var gMajorKey = new KeySignature(new Note(NoteName.G, Alteration.Natural, 4), KeyMode.Major);
        var progression = new ChordProgression(gMajorKey);
        
        // Act
        var tonic = progression.GetChordByDegree(1);
        var dominant = progression.GetChordByDegree(5);
        
        // Assert
        tonic.Root.Name.ShouldBe(NoteName.G);
        tonic.Quality.ShouldBe(ChordQuality.Major);
        
        dominant.Root.Name.ShouldBe(NoteName.D);
        dominant.Quality.ShouldBe(ChordQuality.Major);
    }

    [Fact]
    public void ChordProgression_ShouldParseRomanNumeralSequence()
    {
        // Arrange
        var cMajorKey = new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), KeyMode.Major);
        var progression = new ChordProgression(cMajorKey);
        
        // Act
        var chords = progression.ParseProgression("I - IV - V - I").ToList();
        
        // Assert
        chords.Count.ShouldBe(4);
        chords[0].Root.Name.ShouldBe(NoteName.C); // I
        chords[1].Root.Name.ShouldBe(NoteName.F); // IV
        chords[2].Root.Name.ShouldBe(NoteName.G); // V
        chords[3].Root.Name.ShouldBe(NoteName.C); // I
    }

    [Fact]
    public void ChordProgression_ShouldGetDiatonicChords_ForMinorKey()
    {
        // Arrange
        var aMinorKey = new KeySignature(new Note(NoteName.A, Alteration.Natural, 4), KeyMode.Minor);
        var progression = new ChordProgression(aMinorKey);
        
        // Act
        var diatonicChords = progression.GetDiatonicChords().ToList();
        
        // Assert
        diatonicChords.Count.ShouldBe(7);
        
        // i - A minor
        diatonicChords[0].Root.Name.ShouldBe(NoteName.A);
        diatonicChords[0].Quality.ShouldBe(ChordQuality.Minor);
        
        // ii° - B diminished
        diatonicChords[1].Root.Name.ShouldBe(NoteName.B);
        diatonicChords[1].Quality.ShouldBe(ChordQuality.Diminished);
        
        // III - C major
        diatonicChords[2].Root.Name.ShouldBe(NoteName.C);
        diatonicChords[2].Quality.ShouldBe(ChordQuality.Major);
        
        // iv - D minor
        diatonicChords[3].Root.Name.ShouldBe(NoteName.D);
        diatonicChords[3].Quality.ShouldBe(ChordQuality.Minor);
        
        // v - E minor (natural minor)
        diatonicChords[4].Root.Name.ShouldBe(NoteName.E);
        diatonicChords[4].Quality.ShouldBe(ChordQuality.Minor);
        
        // VI - F major
        diatonicChords[5].Root.Name.ShouldBe(NoteName.F);
        diatonicChords[5].Quality.ShouldBe(ChordQuality.Major);
        
        // VII - G major
        diatonicChords[6].Root.Name.ShouldBe(NoteName.G);
        diatonicChords[6].Quality.ShouldBe(ChordQuality.Major);
    }

    [Fact]
    public void ChordProgression_ShouldSupportSeventhChords()
    {
        // Arrange
        var cMajorKey = new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), KeyMode.Major);
        var progression = new ChordProgression(cMajorKey);
        
        // Act
        var chords = progression.ParseProgression("IMaj7 - ii7 - V7 - IMaj7").ToList();
        
        // Assert
        chords.Count.ShouldBe(4);
        
        // IMaj7
        chords[0].Root.Name.ShouldBe(NoteName.C);
        chords[0].GetNotes().Count().ShouldBe(4);
        
        // ii7
        chords[1].Root.Name.ShouldBe(NoteName.D);
        chords[1].Quality.ShouldBe(ChordQuality.Minor);
        chords[1].GetNotes().Count().ShouldBe(4);
        
        // V7
        chords[2].Root.Name.ShouldBe(NoteName.G);
        chords[2].GetNotes().Count().ShouldBe(4);
    }

    [Fact]
    public void ChordProgression_ShouldGetSecondaryDominant()
    {
        // Arrange
        var cMajorKey = new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), KeyMode.Major);
        var progression = new ChordProgression(cMajorKey);
        
        // Act
        var vOfV = progression.GetSecondaryDominant(5); // V/V (D major in C major)
        
        // Assert
        vOfV.Root.Name.ShouldBe(NoteName.D);
        vOfV.Quality.ShouldBe(ChordQuality.Major);
    }

    [Theory]
    [InlineData("I - V - vi - IV", 4)] // Pop progression
    [InlineData("I - vi - IV - V", 4)]  // 50s progression
    [InlineData("ii - V - I", 3)]       // Jazz ii-V-I
    public void ChordProgression_ShouldParseCommonProgressions(string progression, int expectedCount)
    {
        // Arrange
        var cMajorKey = new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), KeyMode.Major);
        var chordProgression = new ChordProgression(cMajorKey);
        
        // Act
        var chords = chordProgression.ParseProgression(progression).ToList();
        
        // Assert
        chords.Count.ShouldBe(expectedCount);
    }

    [Fact]
    public void ChordProgression_ParseProgression_WithInvalidRomanNumeral_ShouldThrow()
    {
        // Arrange
        var progression = new ChordProgression(new KeySignature(new Note(NoteName.C), KeyMode.Major));
        
        // Act
        Action act = () => progression.ParseProgression("I - IX - V").ToList(); // IX is not valid
        
        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("Invalid Roman numeral: IX");
    }
}