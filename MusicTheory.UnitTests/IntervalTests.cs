namespace MusicTheory.UnitTests;

public class IntervalTests
{
    [Fact]
    public void Interval_ShouldHaveQualityAndNumber_WhenCreated()
    {
        var interval = new Interval(IntervalQuality.Perfect, 5);
        
        interval.Quality.ShouldBe(IntervalQuality.Perfect);
        interval.Number.ShouldBe(5);
    }

    [Theory]
    [InlineData(IntervalQuality.Perfect, 1, 0)]  // Perfect unison
    [InlineData(IntervalQuality.Minor, 2, 1)]     // Minor second
    [InlineData(IntervalQuality.Major, 2, 2)]     // Major second
    [InlineData(IntervalQuality.Minor, 3, 3)]     // Minor third
    [InlineData(IntervalQuality.Major, 3, 4)]     // Major third
    [InlineData(IntervalQuality.Perfect, 4, 5)]   // Perfect fourth
    [InlineData(IntervalQuality.Augmented, 4, 6)] // Augmented fourth (tritone)
    [InlineData(IntervalQuality.Perfect, 5, 7)]   // Perfect fifth
    [InlineData(IntervalQuality.Minor, 6, 8)]     // Minor sixth
    [InlineData(IntervalQuality.Major, 6, 9)]     // Major sixth
    [InlineData(IntervalQuality.Minor, 7, 10)]    // Minor seventh
    [InlineData(IntervalQuality.Major, 7, 11)]    // Major seventh
    [InlineData(IntervalQuality.Perfect, 8, 12)]  // Perfect octave
    public void Interval_ShouldCalculateSemitones_ForCommonIntervals(IntervalQuality quality, int number, int expectedSemitones)
    {
        var interval = new Interval(quality, number);
        
        interval.Semitones.ShouldBe(expectedSemitones);
    }

    [Fact]
    public void Interval_ShouldBeCreated_BetweenTwoNotes()
    {
        var c4 = new Note(NoteName.C, Alteration.Natural, 4);
        var g4 = new Note(NoteName.G, Alteration.Natural, 4);
        
        var interval = Interval.Between(c4, g4);
        
        interval.Quality.ShouldBe(IntervalQuality.Perfect);
        interval.Number.ShouldBe(5);
        interval.Semitones.ShouldBe(7);
    }

    [Fact]
    public void Interval_CalculateSemitones_ForInvalidQualityCombinations_ShouldThrow()
    {
        // Arrange
        var invalidMinorFourth = new Interval(IntervalQuality.Minor, 4); // 4th can't be minor
        var invalidMajorFifth = new Interval(IntervalQuality.Major, 5);  // 5th can't be major
        var invalidPerfectSecond = new Interval(IntervalQuality.Perfect, 2); // 2nd can't be perfect
        var invalidPerfectThird = new Interval(IntervalQuality.Perfect, 3);  // 3rd can't be perfect
        var invalidPerfectSixth = new Interval(IntervalQuality.Perfect, 6);  // 6th can't be perfect
        var invalidPerfectSeventh = new Interval(IntervalQuality.Perfect, 7); // 7th can't be perfect
        
        // Act & Assert
        Action act1 = () => { var _ = invalidMinorFourth.Semitones; };
        Action act2 = () => { var _ = invalidMajorFifth.Semitones; };
        Action act3 = () => { var _ = invalidPerfectSecond.Semitones; };
        Action act4 = () => { var _ = invalidPerfectThird.Semitones; };
        Action act5 = () => { var _ = invalidPerfectSixth.Semitones; };
        Action act6 = () => { var _ = invalidPerfectSeventh.Semitones; };
        
        act1.ShouldThrow<InvalidOperationException>().Message.ShouldBe("Interval 4 cannot be minor");
        act2.ShouldThrow<InvalidOperationException>().Message.ShouldBe("Interval 5 cannot be major");
        act3.ShouldThrow<InvalidOperationException>().Message.ShouldBe("Interval 2 cannot be perfect");
        act4.ShouldThrow<InvalidOperationException>().Message.ShouldBe("Interval 3 cannot be perfect");
        act5.ShouldThrow<InvalidOperationException>().Message.ShouldBe("Interval 6 cannot be perfect");
        act6.ShouldThrow<InvalidOperationException>().Message.ShouldBe("Interval 7 cannot be perfect");
    }

    [Fact]
    public void Interval_CalculateSemitones_WithUnknownQuality_ShouldThrow()
    {
        // This tests the default case in the switch expression
        // We need to use reflection to create an invalid enum value
        var interval = new Interval((IntervalQuality)99, 3);
        
        // Act
        Action act = () => { var _ = interval.Semitones; };
        
        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }
}