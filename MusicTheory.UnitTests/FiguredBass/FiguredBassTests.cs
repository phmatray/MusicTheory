namespace MusicTheory.UnitTests.FiguredBass;

using MusicTheory.FiguredBass;

public class FiguredBassTests
{
    #region Parsing

    [Fact]
    public void FiguredBass_Parse_Empty_ShouldReturn_RootPosition()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("");

        // Assert - Empty = 5/3 = root position
        figure.Inversion.ShouldBe(ChordInversion.Root);
        figure.IsSeventhChord.ShouldBeFalse();
    }

    [Fact]
    public void FiguredBass_Parse_6_ShouldReturn_FirstInversion()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("6");

        // Assert - 6 = first inversion triad
        figure.Inversion.ShouldBe(ChordInversion.First);
        figure.IsSeventhChord.ShouldBeFalse();
    }

    [Fact]
    public void FiguredBass_Parse_64_ShouldReturn_SecondInversion()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("6/4");

        // Assert - 6/4 = second inversion triad
        figure.Inversion.ShouldBe(ChordInversion.Second);
        figure.IsSeventhChord.ShouldBeFalse();
    }

    [Fact]
    public void FiguredBass_Parse_7_ShouldReturn_RootPositionSeventh()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("7");

        // Assert - 7 = root position seventh chord
        figure.Inversion.ShouldBe(ChordInversion.Root);
        figure.IsSeventhChord.ShouldBeTrue();
    }

    [Fact]
    public void FiguredBass_Parse_65_ShouldReturn_FirstInversionSeventh()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("6/5");

        // Assert - 6/5 = first inversion seventh chord
        figure.Inversion.ShouldBe(ChordInversion.First);
        figure.IsSeventhChord.ShouldBeTrue();
    }

    [Fact]
    public void FiguredBass_Parse_43_ShouldReturn_SecondInversionSeventh()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("4/3");

        // Assert - 4/3 = second inversion seventh chord
        figure.Inversion.ShouldBe(ChordInversion.Second);
        figure.IsSeventhChord.ShouldBeTrue();
    }

    [Fact]
    public void FiguredBass_Parse_42_ShouldReturn_ThirdInversionSeventh()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("4/2");

        // Assert - 4/2 = third inversion seventh chord
        figure.Inversion.ShouldBe(ChordInversion.Third);
        figure.IsSeventhChord.ShouldBeTrue();
    }

    [Fact]
    public void FiguredBass_Parse_2_ShouldReturn_ThirdInversionSeventh()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("2");

        // Assert - 2 = abbreviated 4/2 = third inversion seventh chord
        figure.Inversion.ShouldBe(ChordInversion.Third);
        figure.IsSeventhChord.ShouldBeTrue();
    }

    #endregion

    #region Alterations

    [Fact]
    public void FiguredBass_Parse_Sharp6_ShouldHave_AlteredThird()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("#6");

        // Assert
        figure.Inversion.ShouldBe(ChordInversion.First);
        figure.HasAlteration(6).ShouldBeTrue();
        figure.GetAlteration(6).ShouldBe(Alteration.Sharp);
    }

    [Fact]
    public void FiguredBass_Parse_Flat7_ShouldHave_AlteredSeventh()
    {
        // Arrange & Act
        var figure = FiguredBassNotation.Parse("b7");

        // Assert
        figure.IsSeventhChord.ShouldBeTrue();
        figure.HasAlteration(7).ShouldBeTrue();
        figure.GetAlteration(7).ShouldBe(Alteration.Flat);
    }

    [Fact]
    public void FiguredBass_Parse_Natural3_ShouldHave_NaturalizedThird()
    {
        // Arrange & Act - Natural sign typically shown as N or n in ASCII
        var figure = FiguredBassNotation.Parse("n3");

        // Assert
        figure.HasAlteration(3).ShouldBeTrue();
        figure.GetAlteration(3).ShouldBe(Alteration.Natural);
    }

    #endregion

    #region Realization

    [Fact]
    public void FiguredBass_Realize_RootPosition_ShouldReturn_RootPositionChord()
    {
        // Arrange
        var bassNote = new Note(NoteName.C, Alteration.Natural, 3);
        var figure = FiguredBassNotation.Parse("");
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var chord = FiguredBassRealization.Realize(bassNote, figure, key);

        // Assert - C major triad in root position
        chord.Root.Name.ShouldBe(NoteName.C);
        chord.Quality.ShouldBe(ChordQuality.Major);
    }

    [Fact]
    public void FiguredBass_Realize_6_ShouldReturn_FirstInversionChord()
    {
        // Arrange
        var bassNote = new Note(NoteName.E, Alteration.Natural, 3);
        var figure = FiguredBassNotation.Parse("6");
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var chord = FiguredBassRealization.Realize(bassNote, figure, key);

        // Assert - E in bass with 6 = C major first inversion
        chord.Root.Name.ShouldBe(NoteName.C);
        chord.Quality.ShouldBe(ChordQuality.Major);
    }

    [Fact]
    public void FiguredBass_Realize_64_ShouldReturn_SecondInversionChord()
    {
        // Arrange
        var bassNote = new Note(NoteName.G, Alteration.Natural, 3);
        var figure = FiguredBassNotation.Parse("6/4");
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var chord = FiguredBassRealization.Realize(bassNote, figure, key);

        // Assert - G in bass with 6/4 = C major second inversion
        chord.Root.Name.ShouldBe(NoteName.C);
        chord.Quality.ShouldBe(ChordQuality.Major);
    }

    [Fact]
    public void FiguredBass_Realize_7_ShouldReturn_SeventhChord()
    {
        // Arrange - G bass with 7 = G7 (dominant seventh in C)
        var bassNote = new Note(NoteName.G, Alteration.Natural, 3);
        var figure = FiguredBassNotation.Parse("7");
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var chord = FiguredBassRealization.Realize(bassNote, figure, key);

        // Assert
        chord.Root.Name.ShouldBe(NoteName.G);
    }

    [Fact]
    public void FiguredBass_Realize_65_ShouldReturn_FirstInversionSeventh()
    {
        // Arrange - B bass with 6/5 = G7 first inversion
        var bassNote = new Note(NoteName.B, Alteration.Natural, 3);
        var figure = FiguredBassNotation.Parse("6/5");
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var chord = FiguredBassRealization.Realize(bassNote, figure, key);

        // Assert - B in bass with 6/5 in key of C = G7 first inversion
        chord.Root.Name.ShouldBe(NoteName.G);
    }

    #endregion

    #region ToString

    [Fact]
    public void FiguredBass_ToString_RootPosition_ShouldReturn_Empty()
    {
        // Arrange
        var figure = FiguredBassNotation.Parse("");

        // Act
        var str = figure.ToString();

        // Assert - Root position often shows as empty or "5/3"
        str.ShouldBeOneOf("", "5/3");
    }

    [Fact]
    public void FiguredBass_ToString_FirstInversion_ShouldReturn_6()
    {
        // Arrange
        var figure = FiguredBassNotation.Parse("6");

        // Act
        var str = figure.ToString();

        // Assert
        str.ShouldBe("6");
    }

    [Fact]
    public void FiguredBass_ToString_7_ShouldReturn_7()
    {
        // Arrange
        var figure = FiguredBassNotation.Parse("7");

        // Act
        var str = figure.ToString();

        // Assert
        str.ShouldBe("7");
    }

    #endregion

    #region Common Progressions

    [Fact]
    public void FiguredBass_CadentialSixFour()
    {
        // Arrange - The cadential 6/4: I6/4 - V - I
        // Bass: G (with 6/4) - G (with 5/3 or 7) - C
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        var bass1 = new Note(NoteName.G, Alteration.Natural, 3);
        var figure1 = FiguredBassNotation.Parse("6/4");

        var bass2 = new Note(NoteName.G, Alteration.Natural, 3);
        var figure2 = FiguredBassNotation.Parse(""); // V in root position

        var bass3 = new Note(NoteName.C, Alteration.Natural, 3);
        var figure3 = FiguredBassNotation.Parse(""); // I in root position

        // Act
        var chord1 = FiguredBassRealization.Realize(bass1, figure1, key);
        var chord2 = FiguredBassRealization.Realize(bass2, figure2, key);
        var chord3 = FiguredBassRealization.Realize(bass3, figure3, key);

        // Assert
        chord1.Root.Name.ShouldBe(NoteName.C); // C major 6/4
        chord2.Root.Name.ShouldBe(NoteName.G); // G major
        chord3.Root.Name.ShouldBe(NoteName.C); // C major
    }

    #endregion
}
