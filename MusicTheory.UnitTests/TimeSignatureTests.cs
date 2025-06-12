namespace MusicTheory.UnitTests;

public class TimeSignatureTests
{
    [Theory]
    [InlineData(4, 4, "4/4")]
    [InlineData(3, 4, "3/4")]
    [InlineData(6, 8, "6/8")]
    [InlineData(5, 4, "5/4")]
    [InlineData(7, 8, "7/8")]
    [InlineData(2, 2, "2/2")]
    [InlineData(12, 8, "12/8")]
    public void TimeSignature_ToString_ShouldReturnCorrectFormat(int numerator, int denominator, string expected)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act & Assert
        timeSignature.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData(4, 4, 1.0)]      // 4/4 = 1 whole note
    [InlineData(3, 4, 0.75)]     // 3/4 = 3/4 whole note
    [InlineData(6, 8, 0.75)]     // 6/8 = 6/8 whole note = 3/4
    [InlineData(2, 2, 1.0)]      // 2/2 = 1 whole note
    [InlineData(5, 4, 1.25)]     // 5/4 = 5/4 whole note
    public void TimeSignature_GetMeasureDuration_ShouldReturnCorrectValue(int numerator, int denominator, double expectedDuration)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act
        var duration = timeSignature.GetMeasureDuration();
        
        // Assert
        duration.Should().Be(expectedDuration);
    }

    [Theory]
    [InlineData(4, 4, true)]   // 4/4 is simple
    [InlineData(3, 4, true)]   // 3/4 is simple
    [InlineData(2, 4, true)]   // 2/4 is simple
    [InlineData(2, 2, true)]   // 2/2 is simple
    [InlineData(6, 8, false)]  // 6/8 is compound
    [InlineData(9, 8, false)]  // 9/8 is compound
    [InlineData(12, 8, false)] // 12/8 is compound
    [InlineData(5, 4, false)]  // 5/4 is complex (odd)
    [InlineData(7, 8, false)]  // 7/8 is complex (odd)
    public void TimeSignature_IsSimple_ShouldIdentifySimpleMeters(int numerator, int denominator, bool expectedIsSimple)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act & Assert
        timeSignature.IsSimple.Should().Be(expectedIsSimple);
    }

    [Theory]
    [InlineData(4, 4, false)]  // 4/4 is not compound
    [InlineData(6, 8, true)]   // 6/8 is compound
    [InlineData(9, 8, true)]   // 9/8 is compound
    [InlineData(12, 8, true)]  // 12/8 is compound
    [InlineData(6, 4, true)]   // 6/4 is compound
    [InlineData(5, 4, false)]  // 5/4 is complex, not compound
    public void TimeSignature_IsCompound_ShouldIdentifyCompoundMeters(int numerator, int denominator, bool expectedIsCompound)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act & Assert
        timeSignature.IsCompound.Should().Be(expectedIsCompound);
    }

    [Theory]
    [InlineData(4, 4, false)]  // 4/4 is not complex
    [InlineData(5, 4, true)]   // 5/4 is complex
    [InlineData(7, 8, true)]   // 7/8 is complex
    [InlineData(5, 8, true)]   // 5/8 is complex
    [InlineData(11, 8, true)]  // 11/8 is complex
    public void TimeSignature_IsComplex_ShouldIdentifyComplexMeters(int numerator, int denominator, bool expectedIsComplex)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act & Assert
        timeSignature.IsComplex.Should().Be(expectedIsComplex);
    }

    [Theory]
    [InlineData(4, 4, 4)]   // 4/4 has 4 beats
    [InlineData(3, 4, 3)]   // 3/4 has 3 beats
    [InlineData(6, 8, 2)]   // 6/8 has 2 dotted quarter beats
    [InlineData(9, 8, 3)]   // 9/8 has 3 dotted quarter beats
    [InlineData(12, 8, 4)]  // 12/8 has 4 dotted quarter beats
    [InlineData(5, 4, 5)]   // 5/4 has 5 beats
    public void TimeSignature_GetBeatsPerMeasure_ShouldReturnCorrectBeats(int numerator, int denominator, int expectedBeats)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act
        var beats = timeSignature.GetBeatsPerMeasure();
        
        // Assert
        beats.Should().Be(expectedBeats);
    }

    [Theory]
    [InlineData(4, 4, "Common Time")]
    [InlineData(2, 2, "Cut Time")]
    [InlineData(3, 4, "Waltz Time")]
    [InlineData(6, 8, "Compound Duple")]
    [InlineData(9, 8, "Compound Triple")]
    [InlineData(12, 8, "Compound Quadruple")]
    public void TimeSignature_GetCommonName_ShouldReturnCorrectName(int numerator, int denominator, string expectedName)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act
        var name = timeSignature.GetCommonName();
        
        // Assert
        name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData(0, 4)]   // Zero numerator
    [InlineData(-1, 4)]  // Negative numerator
    [InlineData(4, 0)]   // Zero denominator
    [InlineData(4, -4)]  // Negative denominator
    [InlineData(4, 3)]   // Non-power-of-2 denominator
    [InlineData(4, 5)]   // Non-power-of-2 denominator
    public void TimeSignature_Constructor_ShouldThrowForInvalidValues(int numerator, int denominator)
    {
        // Act & Assert
        var act = () => new TimeSignature(numerator, denominator);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TimeSignature_Equals_ShouldCompareCorrectly()
    {
        // Arrange
        var ts1 = new TimeSignature(4, 4);
        var ts2 = new TimeSignature(4, 4);
        var ts3 = new TimeSignature(3, 4);
        
        // Act & Assert
        ts1.Equals(ts2).Should().BeTrue();
        ts1.Equals(ts3).Should().BeFalse();
        (ts1 == ts2).Should().BeTrue();
        (ts1 != ts3).Should().BeTrue();
    }

    [Theory]
    [InlineData(4, 4, "C")]     // Common time symbol
    [InlineData(2, 2, "¢")]     // Cut time symbol
    [InlineData(3, 4, "3/4")]   // Regular notation
    [InlineData(6, 8, "6/8")]   // Regular notation
    public void TimeSignature_GetSymbol_ShouldReturnCorrectSymbol(int numerator, int denominator, string expectedSymbol)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act
        var symbol = timeSignature.GetSymbol();
        
        // Assert
        symbol.Should().Be(expectedSymbol);
    }

    [Theory]
    [InlineData(4, 4, 1, 4)]    // 4/4: quarter note gets the beat
    [InlineData(3, 4, 1, 4)]    // 3/4: quarter note gets the beat
    [InlineData(6, 8, 3, 8)]    // 6/8: dotted quarter gets the beat
    [InlineData(9, 8, 3, 8)]    // 9/8: dotted quarter gets the beat
    [InlineData(2, 2, 1, 2)]    // 2/2: half note gets the beat
    public void TimeSignature_GetBeatUnit_ShouldReturnCorrectUnit(int numerator, int denominator, int expectedNumerator, int expectedDenominator)
    {
        // Arrange
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act
        var (beatNumerator, beatDenominator) = timeSignature.GetBeatUnit();
        
        // Assert
        beatNumerator.Should().Be(expectedNumerator);
        beatDenominator.Should().Be(expectedDenominator);
    }
}