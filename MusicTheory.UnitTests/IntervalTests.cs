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

    [Theory]
    [InlineData(IntervalQuality.Minor, 4, "A 4th is perfect, diminished or augmented, never minor.")]
    [InlineData(IntervalQuality.Major, 5, "A 5th is perfect, diminished or augmented, never major.")]
    [InlineData(IntervalQuality.Major, 8, "A octave is perfect, diminished or augmented, never major.")]
    [InlineData(IntervalQuality.Minor, 1, "A unison is perfect, diminished or augmented, never minor.")]
    [InlineData(IntervalQuality.Major, 22, "A 22nd is perfect, diminished or augmented, never major.")]
    [InlineData(IntervalQuality.Perfect, 2, "A 2nd is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Perfect, 3, "A 3rd is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Perfect, 6, "A 6th is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Perfect, 7, "A 7th is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Perfect, 9, "A 9th is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Perfect, 21, "A 21st is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Perfect, 13, "A 13th is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Major, 11, "A 11th is perfect, diminished or augmented, never major.")]
    [InlineData(IntervalQuality.Perfect, 111, "A 111th is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Perfect, 112, "A 112th is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Major, 113, "A 113th is perfect, diminished or augmented, never major.")]
    [InlineData(IntervalQuality.Perfect, 101, "A 101st is major, minor, diminished or augmented, never perfect.")]
    [InlineData(IntervalQuality.Major, 102, "A 102nd is perfect, diminished or augmented, never major.")]
    [InlineData(IntervalQuality.Major, 103, "A 103rd is perfect, diminished or augmented, never major.")]
    public void Interval_WithImpossibleQuality_ShouldThrowAtConstruction(IntervalQuality quality, int number, string message)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Interval(quality, number)).Message.ShouldStartWith(message);
    }

    [Fact]
    public void Interval_WithUndefinedQualityDirectionOrNumber_ShouldThrow()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Interval((IntervalQuality)99, 3)).ParamName.ShouldBe("quality");
        Should.Throw<ArgumentOutOfRangeException>(() => new Interval(IntervalQuality.Major, 3, (Direction)7)).ParamName.ShouldBe("direction");
        Should.Throw<ArgumentOutOfRangeException>(() => new Interval(IntervalQuality.Perfect, 0)).ParamName.ShouldBe("number");
        Should.Throw<ArgumentOutOfRangeException>(() => new Interval(IntervalQuality.Perfect, 1, Direction.Down)).ParamName.ShouldBe("direction");
    }
}