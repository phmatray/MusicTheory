namespace MusicTheory.UnitTests;

public class IntervalTests
{
    [Fact]
    public void Interval_ShouldHaveQualityAndNumber_WhenCreated()
    {
        var interval = new Interval(IntervalQuality.Perfect, 5);
        
        interval.Quality.Should().Be(IntervalQuality.Perfect);
        interval.Number.Should().Be(5);
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
        
        interval.Semitones.Should().Be(expectedSemitones);
    }

    [Fact]
    public void Interval_ShouldBeCreated_BetweenTwoNotes()
    {
        var c4 = new Note(NoteName.C, Alteration.Natural, 4);
        var g4 = new Note(NoteName.G, Alteration.Natural, 4);
        
        var interval = Interval.Between(c4, g4);
        
        interval.Quality.Should().Be(IntervalQuality.Perfect);
        interval.Number.Should().Be(5);
        interval.Semitones.Should().Be(7);
    }
}