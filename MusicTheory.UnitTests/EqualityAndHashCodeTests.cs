namespace MusicTheory.UnitTests;

public class EqualityAndHashCodeTests
{
    [Fact]
    public void Duration_Equals_Object_ShouldWorkCorrectly()
    {
        // Arrange
        var duration1 = new Duration(DurationType.Quarter, 1);
        var duration2 = new Duration(DurationType.Quarter, 1);
        var duration3 = new Duration(DurationType.Half);
        object objDuration = duration2;
        object? objNull = null;
        object objString = "not a duration";
        
        // Act & Assert
        duration1.Equals(objDuration).ShouldBeTrue();
        duration1.Equals(objNull).ShouldBeFalse();
        duration1.Equals(objString).ShouldBeFalse();
        duration1.Equals(duration3).ShouldBeFalse();
    }

    [Fact]
    public void Duration_GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var duration1 = new Duration(DurationType.Quarter, 1);
        var duration2 = new Duration(DurationType.Quarter, 1);
        var duration3 = new Duration(DurationType.Quarter, 2);
        
        // Act & Assert
        duration1.GetHashCode().ShouldBe(duration2.GetHashCode());
        duration1.GetHashCode().ShouldNotBe(duration3.GetHashCode());
    }

    [Fact]
    public void TimeSignature_CommonTime_ShouldReturn4_4()
    {
        // Act
        var commonTime = TimeSignature.CommonTime;
        
        // Assert
        commonTime.Numerator.ShouldBe(4);
        commonTime.Denominator.ShouldBe(4);
    }

    [Fact]
    public void TimeSignature_CutTime_ShouldReturn2_2()
    {
        // Act
        var cutTime = TimeSignature.CutTime;
        
        // Assert
        cutTime.Numerator.ShouldBe(2);
        cutTime.Denominator.ShouldBe(2);
    }

    [Fact]
    public void TimeSignature_Equals_Object_ShouldWorkCorrectly()
    {
        // Arrange
        var ts1 = new TimeSignature(3, 4);
        var ts2 = new TimeSignature(3, 4);
        var ts3 = new TimeSignature(4, 4);
        object objTs = ts2;
        object? objNull = null;
        object objString = "not a time signature";
        
        // Act & Assert
        ts1.Equals(objTs).ShouldBeTrue();
        ts1.Equals(objNull).ShouldBeFalse();
        ts1.Equals(objString).ShouldBeFalse();
        ts1.Equals(ts3).ShouldBeFalse();
    }

    [Fact]
    public void TimeSignature_GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var ts1 = new TimeSignature(3, 4);
        var ts2 = new TimeSignature(3, 4);
        var ts3 = new TimeSignature(4, 4);
        
        // Act & Assert
        ts1.GetHashCode().ShouldBe(ts2.GetHashCode());
        ts1.GetHashCode().ShouldNotBe(ts3.GetHashCode());
    }

    [Fact]
    public void TimeSignature_EqualityOperators_WithNulls()
    {
        // Arrange
        TimeSignature? ts1 = new TimeSignature(4, 4);
        TimeSignature? ts2 = null;
        TimeSignature? ts3 = null;
        
        // Act & Assert
        (ts1 == ts2).ShouldBeFalse();
        (ts2 == ts1).ShouldBeFalse();
        (ts2 == ts3).ShouldBeTrue();
        (ts1 != ts2).ShouldBeTrue();
        (ts2 != ts3).ShouldBeFalse();
    }

    [Fact]
    public void Duration_Equals_WithTuplets()
    {
        // Arrange
        var tuplet1 = Duration.CreateTuplet(DurationType.Quarter, 3, 2);
        var tuplet2 = Duration.CreateTuplet(DurationType.Quarter, 3, 2);
        var tuplet3 = Duration.CreateTuplet(DurationType.Quarter, 5, 4);
        var regular = new Duration(DurationType.Quarter);
        
        // Act & Assert
        tuplet1.Equals(tuplet2).ShouldBeTrue();
        tuplet1.Equals(tuplet3).ShouldBeFalse();
        tuplet1.Equals(regular).ShouldBeFalse();
    }

    [Fact]
    public void Duration_GetHashCode_WithTuplets()
    {
        // Arrange
        var tuplet1 = Duration.CreateTuplet(DurationType.Eighth, 3, 2);
        var tuplet2 = Duration.CreateTuplet(DurationType.Eighth, 3, 2);
        var tuplet3 = Duration.CreateTuplet(DurationType.Eighth, 5, 4);
        
        // Act & Assert
        tuplet1.GetHashCode().ShouldBe(tuplet2.GetHashCode());
        tuplet1.GetHashCode().ShouldNotBe(tuplet3.GetHashCode());
    }
}