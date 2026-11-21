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
        nextKey.Tonic.Name.ShouldBe(expectedTonic);
        nextKey.Mode.ShouldBe(mode);
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
        previousKey.Tonic.Name.ShouldBe(expectedTonic);
        previousKey.Mode.ShouldBe(mode);
    }

    [Fact]
    public void KeySignature_NextInCircle_ShouldHandleSharpKeys()
    {
        // Arrange
        var bMajor = new KeySignature(new Note(NoteName.B), KeyMode.Major);
        
        // Act
        var nextKey = bMajor.NextInCircle();
        
        // Assert
        nextKey.Tonic.Name.ShouldBe(NoteName.F);
        nextKey.Tonic.Alteration.ShouldBe(Alteration.Sharp);
        nextKey.AccidentalCount.ShouldBe(6);
    }

    [Fact]
    public void KeySignature_NextInCircle_ShouldWrapAroundAtCSharp()
    {
        // C# major's dominant is G# major (8 sharps), written as its enharmonic Ab major (4 flats).
        var nextKey = new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Major).NextInCircle();

        nextKey.ShouldBe(new KeySignature(new Note(NoteName.A, Alteration.Flat), KeyMode.Major));
        nextKey.AccidentalCount.ShouldBe(-4);
    }

    [Fact]
    public void KeySignature_PreviousInCircle_ShouldWrapAroundAtCFlat()
    {
        // Cb major's subdominant is Fb major (8 flats), written as its enharmonic E major (4 sharps).
        var previousKey = new KeySignature(new Note(NoteName.C, Alteration.Flat), KeyMode.Major).PreviousInCircle();

        previousKey.ShouldBe(new KeySignature(new Note(NoteName.E), KeyMode.Major));
        previousKey.AccidentalCount.ShouldBe(4);
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
        relativeKey.Tonic.Name.ShouldBe(expectedTonic);
        relativeKey.Mode.ShouldBe(expectedMode);
        relativeKey.AccidentalCount.ShouldBe(keySignature.AccidentalCount);
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
        parallelKey.Tonic.Name.ShouldBe(tonic);
        parallelKey.Tonic.Alteration.ShouldBe(keySignature.Tonic.Alteration);
        parallelKey.Mode.ShouldBe(expectedMode);
    }

    [Fact]
    public void KeySignature_GetEnharmonicEquivalents_ForFSharpMajor()
    {
        // Arrange
        var fSharpMajor = new KeySignature(new Note(NoteName.F, Alteration.Sharp), KeyMode.Major);
        
        // Act
        var equivalents = fSharpMajor.GetEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.Count.ShouldBe(1);
        equivalents[0].Tonic.Name.ShouldBe(NoteName.G);
        equivalents[0].Tonic.Alteration.ShouldBe(Alteration.Flat);
        equivalents[0].Mode.ShouldBe(KeyMode.Major);
    }

    [Fact]
    public void KeySignature_GetEnharmonicEquivalents_ForCSharpMajor()
    {
        // Arrange
        var cSharpMajor = new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Major);
        
        // Act
        var equivalents = cSharpMajor.GetEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.Count.ShouldBe(1);
        equivalents[0].Tonic.Name.ShouldBe(NoteName.D);
        equivalents[0].Tonic.Alteration.ShouldBe(Alteration.Flat);
        equivalents[0].Mode.ShouldBe(KeyMode.Major);
    }

    [Fact]
    public void KeySignature_GetEnharmonicEquivalents_ForNonEnharmonicKey()
    {
        // Arrange
        var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
        
        // Act
        var equivalents = cMajor.GetEnharmonicEquivalents().ToList();
        
        // Assert
        equivalents.ShouldBeEmpty();
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
        dominant.Tonic.Name.ShouldBe(expectedDominantTonic);
        dominant.Mode.ShouldBe(mode);
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
        subdominant.Tonic.Name.ShouldBe(expectedSubdominantTonic);
        subdominant.Mode.ShouldBe(mode);
    }

    [Theory]
    [InlineData(KeyMode.Major)]
    [InlineData(KeyMode.Minor)]
    public void KeySignature_CircleNavigation_TwelveFifthsComeHome(KeyMode mode)
    {
        var start = new KeySignature(new Note(mode == KeyMode.Major ? NoteName.C : NoteName.A), mode);
        var up = start;
        var down = start;
        for (int i = 0; i < 12; i++)
        {
            var next = up.NextInCircle();
            ((next.Tonic.PitchClass - up.Tonic.PitchClass + 12) % 12).ShouldBe(7, $"{up} to {next} is a fifth");
            Math.Abs(next.AccidentalCount).ShouldBeLessThanOrEqualTo(7);
            up = next;
            down = down.PreviousInCircle();
        }
        up.ShouldBe(start);
        down.ShouldBe(start);
    }
}