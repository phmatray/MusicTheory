namespace MusicTheory.UnitTests.Analysis;

using MusicTheory.Analysis;

public class HarmonicAnalyzerTests
{
    #region Harmonic Function Determination

    [Fact]
    public void HarmonicAnalyzer_DetermineFunction_I_ShouldBeTonic()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var function = HarmonicAnalyzer.DetermineFunction(chord, key);

        // Assert
        function.ShouldBe(HarmonicFunction.Tonic);
    }

    [Fact]
    public void HarmonicAnalyzer_DetermineFunction_V_ShouldBeDominant()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.G), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var function = HarmonicAnalyzer.DetermineFunction(chord, key);

        // Assert
        function.ShouldBe(HarmonicFunction.Dominant);
    }

    [Fact]
    public void HarmonicAnalyzer_DetermineFunction_IV_ShouldBeSubdominant()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.F), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var function = HarmonicAnalyzer.DetermineFunction(chord, key);

        // Assert
        function.ShouldBe(HarmonicFunction.Subdominant);
    }

    [Fact]
    public void HarmonicAnalyzer_DetermineFunction_ii_ShouldBeSubdominant()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.D), ChordQuality.Minor);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var function = HarmonicAnalyzer.DetermineFunction(chord, key);

        // Assert
        function.ShouldBe(HarmonicFunction.Subdominant);
    }

    [Fact]
    public void HarmonicAnalyzer_DetermineFunction_vi_ShouldBeTonic()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.A), ChordQuality.Minor);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var function = HarmonicAnalyzer.DetermineFunction(chord, key);

        // Assert
        function.ShouldBe(HarmonicFunction.Tonic);
    }

    [Fact]
    public void HarmonicAnalyzer_DetermineFunction_viidim_ShouldBeDominant()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.B), ChordQuality.Diminished);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var function = HarmonicAnalyzer.DetermineFunction(chord, key);

        // Assert
        function.ShouldBe(HarmonicFunction.Dominant);
    }

    #endregion

    #region Roman Numeral Assignment

    [Theory]
    [InlineData(NoteName.C, ChordQuality.Major, "I")]
    [InlineData(NoteName.D, ChordQuality.Minor, "ii")]
    [InlineData(NoteName.E, ChordQuality.Minor, "iii")]
    [InlineData(NoteName.F, ChordQuality.Major, "IV")]
    [InlineData(NoteName.G, ChordQuality.Major, "V")]
    [InlineData(NoteName.A, ChordQuality.Minor, "vi")]
    public void HarmonicAnalyzer_GetRomanNumeral_DiatonicChords_InCMajor(
        NoteName root, ChordQuality quality, string expectedNumeral)
    {
        // Arrange
        var chord = new Chord(new Note(root), quality);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var numeral = HarmonicAnalyzer.GetRomanNumeral(chord, key);

        // Assert
        numeral.ShouldBe(expectedNumeral);
    }

    [Fact]
    public void HarmonicAnalyzer_GetRomanNumeral_DominantSeventh_ShouldInclude7()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var numeral = HarmonicAnalyzer.GetRomanNumeral(chord, key);

        // Assert
        numeral.ShouldBe("V7");
    }

    #endregion

    #region Chord Progression Analysis

    [Fact]
    public void HarmonicAnalyzer_Analyze_ShouldReturn_AnalyzedChords()
    {
        // Arrange
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);
        var chords = new[]
        {
            new Chord(new Note(NoteName.C), ChordQuality.Major),
            new Chord(new Note(NoteName.F), ChordQuality.Major),
            new Chord(new Note(NoteName.G), ChordQuality.Major),
            new Chord(new Note(NoteName.C), ChordQuality.Major)
        };

        // Act
        var analyzed = HarmonicAnalyzer.Analyze(chords, key).ToList();

        // Assert
        analyzed.Count.ShouldBe(4);
        analyzed[0].RomanNumeral.ShouldBe("I");
        analyzed[1].RomanNumeral.ShouldBe("IV");
        analyzed[2].RomanNumeral.ShouldBe("V");
        analyzed[3].RomanNumeral.ShouldBe("I");
    }

    #endregion

    #region Cadence Detection

    [Fact]
    public void HarmonicAnalyzer_DetectCadence_VI_ShouldBe_AuthenticPerfect()
    {
        // Arrange
        var penultimate = new Chord(new Note(NoteName.G), ChordQuality.Major);
        var final = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var cadence = HarmonicAnalyzer.DetectCadence(penultimate, final, key);

        // Assert
        cadence.ShouldNotBeNull();
        cadence.ShouldBe(CadenceType.AuthenticPerfect);
    }

    [Fact]
    public void HarmonicAnalyzer_DetectCadence_IVI_ShouldBe_Plagal()
    {
        // Arrange
        var penultimate = new Chord(new Note(NoteName.F), ChordQuality.Major);
        var final = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var cadence = HarmonicAnalyzer.DetectCadence(penultimate, final, key);

        // Assert
        cadence.ShouldNotBeNull();
        cadence.ShouldBe(CadenceType.Plagal);
    }

    [Fact]
    public void HarmonicAnalyzer_DetectCadence_Vvi_ShouldBe_Deceptive()
    {
        // Arrange
        var penultimate = new Chord(new Note(NoteName.G), ChordQuality.Major);
        var final = new Chord(new Note(NoteName.A), ChordQuality.Minor);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var cadence = HarmonicAnalyzer.DetectCadence(penultimate, final, key);

        // Assert
        cadence.ShouldNotBeNull();
        cadence.ShouldBe(CadenceType.Deceptive);
    }

    [Fact]
    public void HarmonicAnalyzer_DetectCadence_IV_ShouldBe_Half()
    {
        // Arrange - any chord to V
        var penultimate = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var final = new Chord(new Note(NoteName.G), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var cadence = HarmonicAnalyzer.DetectCadence(penultimate, final, key);

        // Assert
        cadence.ShouldNotBeNull();
        cadence.ShouldBe(CadenceType.Half);
    }

    [Fact]
    public void HarmonicAnalyzer_DetectCadence_NoRecognizedPattern_ShouldReturnNull()
    {
        // Arrange - ii to iii is not a standard cadence
        var penultimate = new Chord(new Note(NoteName.D), ChordQuality.Minor);
        var final = new Chord(new Note(NoteName.E), ChordQuality.Minor);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var cadence = HarmonicAnalyzer.DetectCadence(penultimate, final, key);

        // Assert
        cadence.ShouldBeNull();
    }

    #endregion
}
