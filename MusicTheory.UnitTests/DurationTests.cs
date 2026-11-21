namespace MusicTheory.UnitTests;

public class DurationTests
{
    [Theory]
    [InlineData(DurationType.Whole, 1, 1)]
    [InlineData(DurationType.Half, 1, 2)]
    [InlineData(DurationType.Quarter, 1, 4)]
    [InlineData(DurationType.Eighth, 1, 8)]
    [InlineData(DurationType.Sixteenth, 1, 16)]
    [InlineData(DurationType.ThirtySecond, 1, 32)]
    [InlineData(DurationType.SixtyFourth, 1, 64)]
    public void Duration_Value_ShouldBeExact(DurationType type, long num, long den)
    {
        new Duration(type).Value.ShouldBe(Rational.Of(num, den));
    }

    [Theory]
    [InlineData(DurationType.Half, 1, 3, 4)]
    [InlineData(DurationType.Quarter, 1, 3, 8)]
    [InlineData(DurationType.Eighth, 1, 3, 16)]
    [InlineData(DurationType.Quarter, 2, 7, 16)]
    [InlineData(DurationType.Half, 2, 7, 8)]
    [InlineData(DurationType.SixtyFourth, 3, 15, 512)]
    public void Duration_WithDots_ShouldBeExact(DurationType type, int dots, long num, long den)
    {
        new Duration(type, dots).Value.ShouldBe(Rational.Of(num, den));
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

    [Fact]
    public void Duration_GetTimeInSeconds_WithBeatUnit_ShouldScaleByBeat()
    {
        var dottedQuarter = new Duration(DurationType.Quarter, 1);
        new Duration(DurationType.Half, 1).GetTimeInSeconds(60, dottedQuarter).ShouldBe(2.0);
        new Duration(DurationType.Quarter).GetTimeInSeconds(90.5).ShouldBe(60.0 / 90.5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Duration_GetTimeInSeconds_WithInvalidTempo_ShouldThrow(double bpm)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Duration(DurationType.Quarter).GetTimeInSeconds(bpm)).ParamName.ShouldBe("bpm");
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
        act.ShouldThrow<ArgumentOutOfRangeException>()
            .Message.ShouldContain("Dots must be between 0 and 3");
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
    [InlineData(DurationType.Quarter, 3, 2, 1, 6)]
    [InlineData(DurationType.Eighth, 3, 2, 1, 12)]
    [InlineData(DurationType.Eighth, 5, 4, 1, 10)]
    [InlineData(DurationType.Sixteenth, 7, 4, 1, 28)]
    public void Duration_CreateTuplet_ShouldCalculateExactValue(
        DurationType type, int actual, int normal, long num, long den)
    {
        var tuplet = Duration.CreateTuplet(type, actual, normal);

        tuplet.IsTuplet.ShouldBeTrue();
        tuplet.Tuplet.ShouldBe((actual, normal));
        tuplet.Value.ShouldBe(Rational.Of(num, den));
    }

    [Fact]
    public void Duration_ThreeTripletEighths_SumToExactlyAQuarter()
    {
        var triplet = Duration.CreateTuplet(DurationType.Eighth, 3, 2);

        (triplet.Value * Rational.Of(3)).ShouldBe(new Duration(DurationType.Quarter).Value);
    }

    [Fact]
    public void Duration_TupletAndPlainWithEqualValue_ShouldNotBeEqual()
    {
        var tripletQuarter = Duration.CreateTuplet(DurationType.Quarter, 3, 2);
        var plainEighthSixth = Duration.CreateTuplet(DurationType.Eighth, 3, 4);

        tripletQuarter.Value.ShouldBe(plainEighthSixth.Value);
        tripletQuarter.ShouldNotBe(plainEighthSixth);
        new Duration(DurationType.Quarter, 1).ShouldBe(new Duration(DurationType.Quarter, 1));
    }

    [Fact]
    public void Duration_CreateTuplet_WithUndefinedType_ShouldThrow()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Duration.CreateTuplet((DurationType)99, 3, 2)).ParamName.ShouldBe("type");
    }

    [Theory]
    [InlineData(0, 2, "actual")]
    [InlineData(3, 0, "normal")]
    public void Duration_CreateTuplet_WithCountBelowOne_ShouldThrow(int actual, int normal, string param)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Duration.CreateTuplet(DurationType.Eighth, actual, normal)).ParamName.ShouldBe(param);
    }

    [Fact]
    public void Duration_WithUndefinedType_ShouldThrowAtConstruction()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Duration((DurationType)99)).ParamName.ShouldBe("type");
    }

    [Theory]
    [InlineData(3, 8, DurationType.Quarter, 1)]
    [InlineData(1, 4, DurationType.Quarter, 0)]
    [InlineData(7, 8, DurationType.Half, 2)]
    [InlineData(15, 512, DurationType.SixtyFourth, 3)]
    public void Duration_TryFromValue_ShouldFindNotatableDuration(long num, long den, DurationType type, int dots)
    {
        Duration.TryFromValue(Rational.Of(num, den), out var duration).ShouldBeTrue();
        duration.ShouldBe(new Duration(type, dots));
    }

    [Theory]
    [InlineData(5, 8)]
    [InlineData(1, 3)]
    [InlineData(0, 1)]
    public void Duration_TryFromValue_ShouldRejectUnnotatableValue(long num, long den)
    {
        Duration.TryFromValue(Rational.Of(num, den), out var duration).ShouldBeFalse();
        duration.ShouldBeNull();
    }

    [Fact]
    public void Duration_Decompose_ShouldSplitGreedilyLargestFirst()
    {
        Duration.Decompose(Rational.Of(7, 8)).ShouldBe([new Duration(DurationType.Half, 2)]);
        Duration.Decompose(Rational.Of(5, 8)).ShouldBe([new Duration(DurationType.Half), new Duration(DurationType.Eighth)]);
        Duration.Decompose(Rational.Of(5, 4)).ShouldBe([new Duration(DurationType.Whole), new Duration(DurationType.Quarter)]);
    }

    [Theory]
    [InlineData(1, 3)]
    [InlineData(1, 128)]
    [InlineData(0, 1)]
    [InlineData(-1, 4)]
    public void Duration_Decompose_ShouldThrowForUnnotatableValue(long num, long den)
    {
        Should.Throw<ArgumentException>(() => Duration.Decompose(Rational.Of(num, den))).ParamName.ShouldBe("value");
    }
}
