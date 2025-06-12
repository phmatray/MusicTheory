namespace MusicTheory.UnitTests;

public class CircleOfFifthsTests
{
    [Theory]
    [InlineData(NoteName.C, KeyMode.Major, NoteName.G)] // C -> G
    [InlineData(NoteName.G, KeyMode.Major, NoteName.D)] // G -> D
    [InlineData(NoteName.D, KeyMode.Major, NoteName.A)] // D -> A
    [InlineData(NoteName.A, KeyMode.Major, NoteName.E)] // A -> E
    [InlineData(NoteName.E, KeyMode.Major, NoteName.B)] // E -> B
    [InlineData(NoteName.F, KeyMode.Major, NoteName.C)] // F -> C
    public void KeySignature_NextInCircle_ShouldReturnCorrectKey(NoteName startTonic, KeyMode mode, NoteName expectedTonic)
    {
        // Arrange
        var keySignature = new KeySignature(new Note(startTonic), mode);
        
        // Act
        var nextKey = keySignature.NextInCircle();
        
        // Assert
        nextKey.Tonic.Name.Should().Be(expectedTonic);
        nextKey.Mode.Should().Be(mode);
    }

    [Theory]
    [InlineData(NoteName.C, KeyMode.Major, NoteName.F)] // C -> F
    [InlineData(NoteName.G, KeyMode.Major, NoteName.C)] // G -> C
    [InlineData(NoteName.D, KeyMode.Major, NoteName.G)] // D -> G
    [InlineData(NoteName.A, KeyMode.Major, NoteName.D)] // A -> D
    [InlineData(NoteName.E, KeyMode.Major, NoteName.A)] // E -> A
    [InlineData(NoteName.B, KeyMode.Major, NoteName.E)] // B -> E
    public void KeySignature_PreviousInCircle_ShouldReturnCorrectKey(NoteName startTonic, KeyMode mode, NoteName expectedTonic)
    {
        // Arrange
        var keySignature = new KeySignature(new Note(startTonic), mode);
        
        // Act
        var previousKey = keySignature.PreviousInCircle();
        
        // Assert
        previousKey.Tonic.Name.Should().Be(expectedTonic);
        previousKey.Mode.Should().Be(mode);
    }

    [Fact]
    public void KeySignature_NextInCircle_ShouldHandleSharpKeys()
    {
        // Arrange
        var bMajor = new KeySignature(new Note(NoteName.B), KeyMode.Major);
        
        // Act
        var nextKey = bMajor.NextInCircle();
        
        // Assert
        nextKey.Tonic.Name.Should().Be(NoteName.F);
        nextKey.Tonic.Alteration.Should().Be(Alteration.Sharp);
        nextKey.AccidentalCount.Should().Be(6);
    }

    [Fact]
    public void KeySignature_NextInCircle_ShouldWrapAroundAtCSharp()
    {
        // Arrange
        var cSharpMajor = new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Major);
        
        // Act
        var nextKey = cSharpMajor.NextInCircle();
        
        // Assert - C# major (7 sharps) wraps to Db major (5 flats)
        nextKey.Tonic.Name.Should().Be(NoteName.D);
        nextKey.Tonic.Alteration.Should().Be(Alteration.Flat);
        nextKey.AccidentalCount.Should().Be(-5);
    }

    [Fact]
    public void KeySignature_PreviousInCircle_ShouldWrapAroundAtCFlat()
    {
        // Arrange
        var cFlatMajor = new KeySignature(new Note(NoteName.C, Alteration.Flat), KeyMode.Major);
        
        // Act
        var previousKey = cFlatMajor.PreviousInCircle();
        
        // Assert - Cb major (7 flats) wraps to B major (5 sharps)
        previousKey.Tonic.Name.Should().Be(NoteName.B);
        previousKey.Tonic.Alteration.Should().Be(Alteration.Natural);
        previousKey.AccidentalCount.Should().Be(5);
    }

    [Theory]
    [InlineData(NoteName.C, KeyMode.Major, NoteName.A, KeyMode.Minor)] // C major -> A minor
    [InlineData(NoteName.G, KeyMode.Major, NoteName.E, KeyMode.Minor)] // G major -> E minor
    [InlineData(NoteName.D, KeyMode.Major, NoteName.B, KeyMode.Minor)] // D major -> B minor
    [InlineData(NoteName.A, KeyMode.Minor, NoteName.C, KeyMode.Major)] // A minor -> C major
    [InlineData(NoteName.E, KeyMode.Minor, NoteName.G, KeyMode.Major)] // E minor -> G major
    public void KeySignature_GetRelative_ShouldReturnCorrectRelativeKey(
        NoteName startTonic, KeyMode startMode, NoteName expectedTonic, KeyMode expectedMode)
    {
        // Arrange
        var keySignature = new KeySignature(new Note(startTonic), startMode);
        
        // Act
        var relativeKey = keySignature.GetRelative();
        
        // Assert
        relativeKey.Tonic.Name.Should().Be(expectedTonic);
        relativeKey.Mode.Should().Be(expectedMode);
        relativeKey.AccidentalCount.Should().Be(keySignature.AccidentalCount);
    }

    [Theory]
    [InlineData(NoteName.C, KeyMode.Major, KeyMode.Minor)] // C major -> C minor
    [InlineData(NoteName.A, KeyMode.Minor, KeyMode.Major)] // A minor -> A major
    [InlineData(NoteName.G, KeyMode.Major, KeyMode.Minor)] // G major -> G minor
    public void KeySignature_GetParallel_ShouldReturnSameTonicDifferentMode(
        NoteName tonic, KeyMode startMode, KeyMode expectedMode)
    {
        // Arrange
        var keySignature = new KeySignature(new Note(tonic), startMode);
        
        // Act
        var parallelKey = keySignature.GetParallel();
        
        // Assert
        parallelKey.Tonic.Name.Should().Be(tonic);
        parallelKey.Tonic.Alteration.Should().Be(keySignature.Tonic.Alteration);
        parallelKey.Mode.Should().Be(expectedMode);
    }

    [Fact]
    public void KeySignature_GetEnharmonicEquivalents_ForFSharpMajor()
    {
        // Arrange
        var fSharpMajor = new KeySignature(new Note(NoteName.F, Alteration.Sharp), KeyMode.Major);
        
        // Act
        var equivalents = fSharpMajor.GetEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.Should().HaveCount(1);
        equivalents[0].Tonic.Name.Should().Be(NoteName.G);
        equivalents[0].Tonic.Alteration.Should().Be(Alteration.Flat);
        equivalents[0].Mode.Should().Be(KeyMode.Major);
    }

    [Fact]
    public void KeySignature_GetEnharmonicEquivalents_ForCSharpMajor()
    {
        // Arrange
        var cSharpMajor = new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Major);
        
        // Act
        var equivalents = cSharpMajor.GetEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.Should().HaveCount(1);
        equivalents[0].Tonic.Name.Should().Be(NoteName.D);
        equivalents[0].Tonic.Alteration.Should().Be(Alteration.Flat);
        equivalents[0].Mode.Should().Be(KeyMode.Major);
    }

    [Fact]
    public void KeySignature_GetEnharmonicEquivalents_ForNonEnharmonicKey()
    {
        // Arrange
        var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
        
        // Act
        var equivalents = cMajor.GetEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.Should().BeEmpty();
    }

    [Theory]
    [InlineData(NoteName.C, KeyMode.Major, NoteName.G)] // C major -> G major (dominant)
    [InlineData(NoteName.G, KeyMode.Major, NoteName.D)] // G major -> D major
    [InlineData(NoteName.A, KeyMode.Minor, NoteName.E)] // A minor -> E minor
    public void KeySignature_GetDominant_ShouldReturnFifthAbove(
        NoteName tonic, KeyMode mode, NoteName expectedDominantTonic)
    {
        // Arrange
        var keySignature = new KeySignature(new Note(tonic), mode);
        
        // Act
        var dominant = keySignature.GetDominant();
        
        // Assert
        dominant.Tonic.Name.Should().Be(expectedDominantTonic);
        dominant.Mode.Should().Be(mode);
    }

    [Theory]
    [InlineData(NoteName.C, KeyMode.Major, NoteName.F)] // C major -> F major (subdominant)
    [InlineData(NoteName.G, KeyMode.Major, NoteName.C)] // G major -> C major
    [InlineData(NoteName.A, KeyMode.Minor, NoteName.D)] // A minor -> D minor
    public void KeySignature_GetSubdominant_ShouldReturnFifthBelow(
        NoteName tonic, KeyMode mode, NoteName expectedSubdominantTonic)
    {
        // Arrange
        var keySignature = new KeySignature(new Note(tonic), mode);
        
        // Act
        var subdominant = keySignature.GetSubdominant();
        
        // Assert
        subdominant.Tonic.Name.Should().Be(expectedSubdominantTonic);
        subdominant.Mode.Should().Be(mode);
    }

    [Fact]
    public void KeySignature_CircleNavigation_ShouldBeConsistent()
    {
        // Arrange
        var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
        
        // Act - Go around the entire circle
        var current = cMajor;
        for (int i = 0; i < 12; i++)
        {
            current = current.NextInCircle();
        }
        
        // Assert - After 12 steps we should be at an enharmonic equivalent
        // C -> G -> D -> A -> E -> B -> F# -> C# -> Db -> Ab -> Eb -> Bb -> F
        current.Tonic.Name.Should().Be(NoteName.F);
        current.Tonic.Alteration.Should().Be(Alteration.Natural);
        current.AccidentalCount.Should().Be(-1); // F major has 1 flat
    }
}