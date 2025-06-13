namespace MusicTheory.UnitTests;

public class IntervalInversionTests
{
    [Fact]
    public void Interval_Invert_PerfectUnison_ShouldReturnPerfectOctave()
    {
        // Arrange
        var unison = new Interval(IntervalQuality.Perfect, 1);
        
        // Act
        var inverted = unison.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Perfect);
        inverted.Number.ShouldBe(8);
    }
    
    [Fact]
    public void Interval_Invert_MajorSecond_ShouldReturnMinorSeventh()
    {
        // Arrange
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        
        // Act
        var inverted = majorSecond.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Minor);
        inverted.Number.ShouldBe(7);
    }
    
    [Fact]
    public void Interval_Invert_MinorThird_ShouldReturnMajorSixth()
    {
        // Arrange
        var minorThird = new Interval(IntervalQuality.Minor, 3);
        
        // Act
        var inverted = minorThird.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Major);
        inverted.Number.ShouldBe(6);
    }
    
    [Fact]
    public void Interval_Invert_PerfectFourth_ShouldReturnPerfectFifth()
    {
        // Arrange
        var perfectFourth = new Interval(IntervalQuality.Perfect, 4);
        
        // Act
        var inverted = perfectFourth.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Perfect);
        inverted.Number.ShouldBe(5);
    }
    
    [Fact]
    public void Interval_Invert_PerfectFifth_ShouldReturnPerfectFourth()
    {
        // Arrange
        var perfectFifth = new Interval(IntervalQuality.Perfect, 5);
        
        // Act
        var inverted = perfectFifth.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Perfect);
        inverted.Number.ShouldBe(4);
    }
    
    [Fact]
    public void Interval_Invert_MajorSixth_ShouldReturnMinorThird()
    {
        // Arrange
        var majorSixth = new Interval(IntervalQuality.Major, 6);
        
        // Act
        var inverted = majorSixth.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Minor);
        inverted.Number.ShouldBe(3);
    }
    
    [Fact]
    public void Interval_Invert_MinorSeventh_ShouldReturnMajorSecond()
    {
        // Arrange
        var minorSeventh = new Interval(IntervalQuality.Minor, 7);
        
        // Act
        var inverted = minorSeventh.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Major);
        inverted.Number.ShouldBe(2);
    }
    
    [Fact]
    public void Interval_Invert_PerfectOctave_ShouldReturnPerfectUnison()
    {
        // Arrange
        var octave = new Interval(IntervalQuality.Perfect, 8);
        
        // Act
        var inverted = octave.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Perfect);
        inverted.Number.ShouldBe(1);
    }
    
    [Fact]
    public void Interval_Invert_AugmentedFourth_ShouldReturnDiminishedFifth()
    {
        // Arrange
        var augmentedFourth = new Interval(IntervalQuality.Augmented, 4);
        
        // Act
        var inverted = augmentedFourth.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Diminished);
        inverted.Number.ShouldBe(5);
    }
    
    [Fact]
    public void Interval_Invert_DiminishedFifth_ShouldReturnAugmentedFourth()
    {
        // Arrange
        var diminishedFifth = new Interval(IntervalQuality.Diminished, 5);
        
        // Act
        var inverted = diminishedFifth.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Augmented);
        inverted.Number.ShouldBe(4);
    }
    
    [Theory]
    [InlineData(IntervalQuality.Major, IntervalQuality.Minor)]
    [InlineData(IntervalQuality.Minor, IntervalQuality.Major)]
    [InlineData(IntervalQuality.Augmented, IntervalQuality.Diminished)]
    [InlineData(IntervalQuality.Diminished, IntervalQuality.Augmented)]
    [InlineData(IntervalQuality.Perfect, IntervalQuality.Perfect)]
    public void Interval_Invert_QualityInversionRules(IntervalQuality original, IntervalQuality expectedInverted)
    {
        // Arrange
        var interval = new Interval(original, 2); // Using second as example for non-perfect intervals
        if (original == IntervalQuality.Perfect)
        {
            interval = new Interval(original, 4); // Use fourth for perfect intervals
        }
        
        // Act
        var inverted = interval.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(expectedInverted);
    }
    
    [Fact]
    public void Interval_Invert_CompoundInterval_ShouldReduceToSimpleInterval()
    {
        // Arrange
        var majorNinth = new Interval(IntervalQuality.Major, 9);
        
        // Act
        var inverted = majorNinth.Invert();
        
        // Assert
        inverted.Quality.ShouldBe(IntervalQuality.Minor);
        inverted.Number.ShouldBe(7); // Ninth inverts to seventh (within octave)
    }
    
    [Fact]
    public void Interval_Invert_ShouldPreserveSemitoneSum()
    {
        // Arrange
        var majorThird = new Interval(IntervalQuality.Major, 3);
        
        // Act
        var inverted = majorThird.Invert();
        
        // Assert
        // Major third (4 semitones) + Minor sixth (8 semitones) = 12 semitones (octave)
        (majorThird.Semitones + inverted.Semitones).ShouldBe(12);
    }
    
    [Theory]
    [InlineData(1, 8)]
    [InlineData(2, 7)]
    [InlineData(3, 6)]
    [InlineData(4, 5)]
    [InlineData(5, 4)]
    [InlineData(6, 3)]
    [InlineData(7, 2)]
    [InlineData(8, 1)]
    public void Interval_Invert_NumberInversionRule(int originalNumber, int expectedInvertedNumber)
    {
        // Arrange
        var quality = (originalNumber == 1 || originalNumber == 4 || originalNumber == 5 || originalNumber == 8) 
            ? IntervalQuality.Perfect 
            : IntervalQuality.Major;
        var interval = new Interval(quality, originalNumber);
        
        // Act
        var inverted = interval.Invert();
        
        // Assert
        inverted.Number.ShouldBe(expectedInvertedNumber);
    }
}