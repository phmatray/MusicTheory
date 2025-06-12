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
        progression.Key.Should().Be(key);
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
        diatonicChords.Should().HaveCount(7);
        
        // I - C major
        diatonicChords[0].Root.Name.Should().Be(NoteName.C);
        diatonicChords[0].Quality.Should().Be(ChordQuality.Major);
        
        // ii - D minor
        diatonicChords[1].Root.Name.Should().Be(NoteName.D);
        diatonicChords[1].Quality.Should().Be(ChordQuality.Minor);
        
        // iii - E minor
        diatonicChords[2].Root.Name.Should().Be(NoteName.E);
        diatonicChords[2].Quality.Should().Be(ChordQuality.Minor);
        
        // IV - F major
        diatonicChords[3].Root.Name.Should().Be(NoteName.F);
        diatonicChords[3].Quality.Should().Be(ChordQuality.Major);
        
        // V - G major
        diatonicChords[4].Root.Name.Should().Be(NoteName.G);
        diatonicChords[4].Quality.Should().Be(ChordQuality.Major);
        
        // vi - A minor
        diatonicChords[5].Root.Name.Should().Be(NoteName.A);
        diatonicChords[5].Quality.Should().Be(ChordQuality.Minor);
        
        // vii° - B diminished
        diatonicChords[6].Root.Name.Should().Be(NoteName.B);
        diatonicChords[6].Quality.Should().Be(ChordQuality.Diminished);
    }

    [Fact]
    public void ChordProgression_ShouldGetRomanNumeral_ForDegree()
    {
        // Arrange
        var cMajorKey = new KeySignature(new Note(NoteName.C, Alteration.Natural, 4), KeyMode.Major);
        var progression = new ChordProgression(cMajorKey);
        
        // Act & Assert
        progression.GetRomanNumeral(1).Should().Be("I");
        progression.GetRomanNumeral(2).Should().Be("ii");
        progression.GetRomanNumeral(3).Should().Be("iii");
        progression.GetRomanNumeral(4).Should().Be("IV");
        progression.GetRomanNumeral(5).Should().Be("V");
        progression.GetRomanNumeral(6).Should().Be("vi");
        progression.GetRomanNumeral(7).Should().Be("vii°");
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
        tonic.Root.Name.Should().Be(NoteName.G);
        tonic.Quality.Should().Be(ChordQuality.Major);
        
        dominant.Root.Name.Should().Be(NoteName.D);
        dominant.Quality.Should().Be(ChordQuality.Major);
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
        chords.Should().HaveCount(4);
        chords[0].Root.Name.Should().Be(NoteName.C); // I
        chords[1].Root.Name.Should().Be(NoteName.F); // IV
        chords[2].Root.Name.Should().Be(NoteName.G); // V
        chords[3].Root.Name.Should().Be(NoteName.C); // I
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
        diatonicChords.Should().HaveCount(7);
        
        // i - A minor
        diatonicChords[0].Root.Name.Should().Be(NoteName.A);
        diatonicChords[0].Quality.Should().Be(ChordQuality.Minor);
        
        // ii° - B diminished
        diatonicChords[1].Root.Name.Should().Be(NoteName.B);
        diatonicChords[1].Quality.Should().Be(ChordQuality.Diminished);
        
        // III - C major
        diatonicChords[2].Root.Name.Should().Be(NoteName.C);
        diatonicChords[2].Quality.Should().Be(ChordQuality.Major);
        
        // iv - D minor
        diatonicChords[3].Root.Name.Should().Be(NoteName.D);
        diatonicChords[3].Quality.Should().Be(ChordQuality.Minor);
        
        // v - E minor (natural minor)
        diatonicChords[4].Root.Name.Should().Be(NoteName.E);
        diatonicChords[4].Quality.Should().Be(ChordQuality.Minor);
        
        // VI - F major
        diatonicChords[5].Root.Name.Should().Be(NoteName.F);
        diatonicChords[5].Quality.Should().Be(ChordQuality.Major);
        
        // VII - G major
        diatonicChords[6].Root.Name.Should().Be(NoteName.G);
        diatonicChords[6].Quality.Should().Be(ChordQuality.Major);
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
        chords.Should().HaveCount(4);
        
        // IMaj7
        chords[0].Root.Name.Should().Be(NoteName.C);
        chords[0].GetNotes().Should().HaveCount(4);
        
        // ii7
        chords[1].Root.Name.Should().Be(NoteName.D);
        chords[1].Quality.Should().Be(ChordQuality.Minor);
        chords[1].GetNotes().Should().HaveCount(4);
        
        // V7
        chords[2].Root.Name.Should().Be(NoteName.G);
        chords[2].GetNotes().Should().HaveCount(4);
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
        vOfV.Root.Name.Should().Be(NoteName.D);
        vOfV.Quality.Should().Be(ChordQuality.Major);
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
        chords.Should().HaveCount(expectedCount);
    }
}