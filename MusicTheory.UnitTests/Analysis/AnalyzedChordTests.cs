namespace MusicTheory.UnitTests.Analysis;

using MusicTheory.Analysis;

public class AnalyzedChordTests
{
    [Fact]
    public void AnalyzedChord_ShouldHave_ChordProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var analyzed = new AnalyzedChord(chord, 1, "I", HarmonicFunction.Tonic, key);

        // Assert
        analyzed.Chord.ShouldBe(chord);
    }

    [Fact]
    public void AnalyzedChord_ShouldHave_ScaleDegreeProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.G), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var analyzed = new AnalyzedChord(chord, 5, "V", HarmonicFunction.Dominant, key);

        // Assert
        analyzed.ScaleDegree.ShouldBe(5);
    }

    [Fact]
    public void AnalyzedChord_ShouldHave_RomanNumeralProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.A), ChordQuality.Minor);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var analyzed = new AnalyzedChord(chord, 6, "vi", HarmonicFunction.Tonic, key);

        // Assert
        analyzed.RomanNumeral.ShouldBe("vi");
    }

    [Fact]
    public void AnalyzedChord_ShouldHave_FunctionProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.F), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var analyzed = new AnalyzedChord(chord, 4, "IV", HarmonicFunction.Subdominant, key);

        // Assert
        analyzed.Function.ShouldBe(HarmonicFunction.Subdominant);
    }

    [Fact]
    public void AnalyzedChord_ShouldHave_ContextKeyProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var analyzed = new AnalyzedChord(chord, 1, "I", HarmonicFunction.Tonic, key);

        // Assert
        analyzed.ContextKey.ShouldBe(key);
    }

    [Fact]
    public void AnalyzedChord_ShouldHave_IsNonDiatonicProperty()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.B, Alteration.Flat), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act - bVII is non-diatonic (borrowed from minor)
        var analyzed = new AnalyzedChord(chord, 7, "bVII", HarmonicFunction.ModalInterchange, key, isNonDiatonic: true);

        // Assert
        analyzed.IsNonDiatonic.ShouldBeTrue();
    }

    [Fact]
    public void AnalyzedChord_IsNonDiatonic_ShouldBeFalse_ForDiatonicChords()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.G), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var analyzed = new AnalyzedChord(chord, 5, "V", HarmonicFunction.Dominant, key);

        // Assert
        analyzed.IsNonDiatonic.ShouldBeFalse();
    }

    [Theory]
    [InlineData(1, "I", HarmonicFunction.Tonic)]
    [InlineData(4, "IV", HarmonicFunction.Subdominant)]
    [InlineData(5, "V", HarmonicFunction.Dominant)]
    public void AnalyzedChord_ShouldCorrectlyStore_AllProperties(
        int degree, string romanNumeral, HarmonicFunction function)
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var analyzed = new AnalyzedChord(chord, degree, romanNumeral, function, key);

        // Assert
        analyzed.ScaleDegree.ShouldBe(degree);
        analyzed.RomanNumeral.ShouldBe(romanNumeral);
        analyzed.Function.ShouldBe(function);
    }
}
