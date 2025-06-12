namespace MusicTheory.UnitTests;

public class DurationTests
{
    [Theory]
    [InlineData(DurationType.Whole, 1.0)]
    [InlineData(DurationType.Half, 0.5)]
    [InlineData(DurationType.Quarter, 0.25)]
    [InlineData(DurationType.Eighth, 0.125)]
    [InlineData(DurationType.Sixteenth, 0.0625)]
    [InlineData(DurationType.ThirtySecond, 0.03125)]
    [InlineData(DurationType.SixtyFourth, 0.015625)]
    public void Duration_GetValueInWholeNotes_ShouldReturnCorrectValue(DurationType type, double expectedValue)
    {
        // Arrange
        var duration = new Duration(type);
        
        // Act
        var value = duration.GetValueInWholeNotes();
        
        // Assert
        value.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData(DurationType.Whole, 0, 1.0)]
    [InlineData(DurationType.Half, 0, 0.5)]
    [InlineData(DurationType.Quarter, 0, 0.25)]
    [InlineData(DurationType.Half, 1, 0.75)]      // Dotted half
    [InlineData(DurationType.Quarter, 1, 0.375)]   // Dotted quarter
    [InlineData(DurationType.Eighth, 1, 0.1875)]   // Dotted eighth
    [InlineData(DurationType.Quarter, 2, 0.4375)]  // Double dotted quarter
    [InlineData(DurationType.Half, 2, 0.875)]      // Double dotted half
    public void Duration_WithDots_ShouldCalculateCorrectValue(DurationType type, int dots, double expectedValue)
    {
        // Arrange
        var duration = new Duration(type, dots);
        
        // Act
        var value = duration.GetValueInWholeNotes();
        
        // Assert
        value.ShouldBe(expectedValue, 0.0001);
    }

    [Theory]
    [InlineData(DurationType.Quarter, 120, 0.5)]    // Quarter note at 120 BPM = 0.5 seconds
    [InlineData(DurationType.Quarter, 60, 1.0)]     // Quarter note at 60 BPM = 1 second
    [InlineData(DurationType.Half, 120, 1.0)]       // Half note at 120 BPM = 1 second
    [InlineData(DurationType.Whole, 60, 4.0)]       // Whole note at 60 BPM = 4 seconds
    [InlineData(DurationType.Eighth, 120, 0.25)]    // Eighth note at 120 BPM = 0.25 seconds
    public void Duration_GetTimeInSeconds_WithQuarterNoteBeat_ShouldReturnCorrectTime(
        DurationType type, int bpm, double expectedSeconds)
    {
        // Arrange
        var duration = new Duration(type);
        
        // Act
        var seconds = duration.GetTimeInSeconds(bpm);
        
        // Assert
        seconds.ShouldBe(expectedSeconds, 0.0001);
    }

    [Theory]
    [InlineData(DurationType.Whole, "𝅝")]
    [InlineData(DurationType.Half, "𝅗𝅥")]
    [InlineData(DurationType.Quarter, "𝅘𝅥")]
    [InlineData(DurationType.Eighth, "𝅘𝅥𝅮")]
    [InlineData(DurationType.Sixteenth, "𝅘𝅥𝅯")]
    [InlineData(DurationType.ThirtySecond, "𝅘𝅥𝅰")]
    [InlineData(DurationType.SixtyFourth, "𝅘𝅥𝅱")]
    public void Duration_GetSymbol_ShouldReturnCorrectUnicodeSymbol(DurationType type, string expectedSymbol)
    {
        // Arrange
        var duration = new Duration(type);
        
        // Act
        var symbol = duration.GetSymbol();
        
        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    [Theory]
    [InlineData(DurationType.Half, 1, "𝅗𝅥.")]        // Dotted half
    [InlineData(DurationType.Quarter, 1, "𝅘𝅥.")]     // Dotted quarter
    [InlineData(DurationType.Quarter, 2, "𝅘𝅥..")]    // Double dotted quarter
    [InlineData(DurationType.Eighth, 1, "𝅘𝅥𝅮.")]     // Dotted eighth
    public void Duration_GetSymbol_WithDots_ShouldIncludeDots(DurationType type, int dots, string expectedSymbol)
    {
        // Arrange
        var duration = new Duration(type, dots);
        
        // Act
        var symbol = duration.GetSymbol();
        
        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    [Theory]
    [InlineData(DurationType.Whole, "whole")]
    [InlineData(DurationType.Half, "half")]
    [InlineData(DurationType.Quarter, "quarter")]
    [InlineData(DurationType.Eighth, "eighth")]
    [InlineData(DurationType.Sixteenth, "16th")]
    [InlineData(DurationType.ThirtySecond, "32nd")]
    [InlineData(DurationType.SixtyFourth, "64th")]
    public void Duration_ToString_ShouldReturnReadableName(DurationType type, string expectedName)
    {
        // Arrange
        var duration = new Duration(type);
        
        // Act
        var name = duration.ToString();
        
        // Assert
        name.ShouldBe(expectedName);
    }

    [Theory]
    [InlineData(DurationType.Half, 1, "dotted half")]
    [InlineData(DurationType.Quarter, 1, "dotted quarter")]
    [InlineData(DurationType.Quarter, 2, "double dotted quarter")]
    [InlineData(DurationType.Eighth, 1, "dotted eighth")]
    [InlineData(DurationType.Eighth, 3, "triple dotted eighth")]
    public void Duration_ToString_WithDots_ShouldIncludeDotDescription(DurationType type, int dots, string expectedName)
    {
        // Arrange
        var duration = new Duration(type, dots);
        
        // Act
        var name = duration.ToString();
        
        // Assert
        name.ShouldBe(expectedName);
    }

    [Theory]
    [InlineData(DurationType.Whole, DurationType.Half, 2)]         // 1 whole = 2 halves
    [InlineData(DurationType.Whole, DurationType.Quarter, 4)]      // 1 whole = 4 quarters
    [InlineData(DurationType.Half, DurationType.Eighth, 4)]        // 1 half = 4 eighths
    [InlineData(DurationType.Quarter, DurationType.Sixteenth, 4)]  // 1 quarter = 4 sixteenths
    public void Duration_Divide_ShouldReturnCorrectCount(DurationType dividend, DurationType divisor, int expectedCount)
    {
        // Arrange
        var duration1 = new Duration(dividend);
        var duration2 = new Duration(divisor);
        
        // Act
        var count = duration1.Divide(duration2);
        
        // Assert
        count.ShouldBe(expectedCount);
    }

    [Fact]
    public void Duration_Add_ShouldCombineDurations()
    {
        // Arrange
        var quarter = new Duration(DurationType.Quarter);
        var eighth = new Duration(DurationType.Eighth);
        
        // Act
        var sum = quarter.Add(eighth);
        
        // Assert
        sum.GetValueInWholeNotes().ShouldBe(0.375); // Quarter + Eighth = 3/8
    }

    [Fact]
    public void Duration_Equals_ShouldCompareCorrectly()
    {
        // Arrange
        var duration1 = new Duration(DurationType.Quarter, 1);
        var duration2 = new Duration(DurationType.Quarter, 1);
        var duration3 = new Duration(DurationType.Quarter, 0);
        var duration4 = new Duration(DurationType.Half, 0);
        
        // Act & Assert
        duration1.Equals(duration2).ShouldBeTrue();
        duration1.Equals(duration3).ShouldBeFalse();
        duration1.Equals(duration4).ShouldBeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(10)]
    public void Duration_Constructor_ShouldThrowForInvalidDots(int dots)
    {
        // Act & Assert
        Action act = () => new Duration(DurationType.Quarter, dots);
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldContain("Dots must be between 0 and 3");
    }

    [Theory]
    [InlineData(DurationType.Quarter, 3, 2, 0.166667)]   // Quarter in 3/2 time = 0.25 / 1.5
    [InlineData(DurationType.Half, 6, 8, 0.666667)]      // Half note in 6/8 time = 0.5 / 0.75
    [InlineData(DurationType.Whole, 4, 4, 1.0)]          // Whole note in 4/4 time = 1.0 / 1.0
    public void Duration_GetValueInMeasures_ShouldCalculateCorrectly(
        DurationType type, int numerator, int denominator, double expectedMeasures)
    {
        // Arrange
        var duration = new Duration(type);
        var timeSignature = new TimeSignature(numerator, denominator);
        
        // Act
        var measures = duration.GetValueInMeasures(timeSignature);
        
        // Assert
        measures.ShouldBe(expectedMeasures, 0.0001);
    }

    [Fact]
    public void Duration_IsTuplet_ShouldBeFalseForRegularDurations()
    {
        // Arrange
        var duration = new Duration(DurationType.Quarter);
        
        // Act & Assert
        duration.IsTuplet.ShouldBeFalse();
    }

    [Theory]
    [InlineData(DurationType.Quarter, 3, 2, 0.166667)]  // Triplet quarter = 1/6
    [InlineData(DurationType.Eighth, 3, 2, 0.083333)]   // Triplet eighth = 1/12
    [InlineData(DurationType.Eighth, 5, 4, 0.1)]        // Quintuplet eighth = 1/10
    [InlineData(DurationType.Sixteenth, 7, 4, 0.035714)] // Septuplet sixteenth
    public void Duration_CreateTuplet_ShouldCalculateCorrectValue(
        DurationType baseType, int tupletCount, int normalCount, double expectedValue)
    {
        // Arrange & Act
        var tuplet = Duration.CreateTuplet(baseType, tupletCount, normalCount);
        
        // Assert
        tuplet.IsTuplet.ShouldBeTrue();
        tuplet.GetValueInWholeNotes().ShouldBe(expectedValue, 0.0001);
    }

    [Fact]
    public void Duration_GetSymbol_WithInvalidDurationType_ShouldReturnQuestionMark()
    {
        // Use reflection to create an invalid enum value
        var duration = new Duration((DurationType)99);
        
        // Act
        var symbol = duration.GetSymbol();
        
        // Assert
        symbol.ShouldBe("?");
    }

    [Fact]
    public void Duration_GetValueInWholeNotes_WithInvalidDurationType_ShouldThrow()
    {
        // Use reflection to create an invalid enum value
        var duration = new Duration((DurationType)99);
        
        // Act
        Action act = () => duration.GetValueInWholeNotes();
        
        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Duration_ToString_WithUnknownDurationType_ShouldReturnUnknown()
    {
        // Use reflection to create an invalid enum value
        var duration = new Duration((DurationType)99);
        
        // Act
        var result = duration.ToString();
        
        // Assert
        result.ShouldBe("unknown");
    }
}