namespace MusicTheory.UnitTests.Jazz;

using MusicTheory.Jazz;

public class TritoneSubstitutionTests
{
    #region GetSubstitute

    [Fact]
    public void TritoneSubstitution_GetSubstitute_G7_ShouldReturn_Db7()
    {
        // Arrange - G7 (G, B, D, F)
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);

        // Act
        var substitute = TritoneSubstitution.GetSubstitute(g7);

        // Assert - Should be Db7 (tritone away from G)
        substitute.Root.Name.ShouldBe(NoteName.D);
        substitute.Root.Alteration.ShouldBe(Alteration.Flat);
        substitute.Type.ShouldBe(ChordType.Dominant7);
    }

    [Fact]
    public void TritoneSubstitution_GetSubstitute_C7_ShouldReturn_Gb7()
    {
        // Arrange
        var c7 = new Chord(new Note(NoteName.C), ChordType.Dominant7);

        // Act
        var substitute = TritoneSubstitution.GetSubstitute(c7);

        // Assert - Gb is tritone from C
        substitute.Root.Name.ShouldBe(NoteName.G);
        substitute.Root.Alteration.ShouldBe(Alteration.Flat);
        substitute.Type.ShouldBe(ChordType.Dominant7);
    }

    [Fact]
    public void TritoneSubstitution_GetSubstitute_D7_ShouldReturn_Ab7()
    {
        // Arrange
        var d7 = new Chord(new Note(NoteName.D), ChordType.Dominant7);

        // Act
        var substitute = TritoneSubstitution.GetSubstitute(d7);

        // Assert
        substitute.Root.Name.ShouldBe(NoteName.A);
        substitute.Root.Alteration.ShouldBe(Alteration.Flat);
        substitute.Type.ShouldBe(ChordType.Dominant7);
    }

    [Theory]
    [InlineData(NoteName.C, Alteration.Natural, NoteName.G, Alteration.Flat)]   // C7 -> Gb7
    [InlineData(NoteName.G, Alteration.Natural, NoteName.D, Alteration.Flat)]   // G7 -> Db7
    [InlineData(NoteName.D, Alteration.Natural, NoteName.A, Alteration.Flat)]   // D7 -> Ab7
    [InlineData(NoteName.A, Alteration.Natural, NoteName.E, Alteration.Flat)]   // A7 -> Eb7
    [InlineData(NoteName.E, Alteration.Natural, NoteName.B, Alteration.Flat)]   // E7 -> Bb7
    [InlineData(NoteName.B, Alteration.Natural, NoteName.F, Alteration.Natural)] // B7 -> F7
    [InlineData(NoteName.F, Alteration.Natural, NoteName.B, Alteration.Natural)] // F7 -> B7
    public void TritoneSubstitution_GetSubstitute_ShouldReturn_TritoneRoot(
        NoteName originalRoot, Alteration originalAlt,
        NoteName expectedRoot, Alteration expectedAlt)
    {
        // Arrange
        var original = new Chord(new Note(originalRoot, originalAlt), ChordType.Dominant7);

        // Act
        var substitute = TritoneSubstitution.GetSubstitute(original);

        // Assert
        substitute.Root.Name.ShouldBe(expectedRoot);
        substitute.Root.Alteration.ShouldBe(expectedAlt);
    }

    #endregion

    #region CanSubstitute

    [Fact]
    public void TritoneSubstitution_CanSubstitute_Dominant7_ShouldReturn_True()
    {
        // Arrange
        var dominant7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);

        // Act & Assert
        TritoneSubstitution.CanSubstitute(dominant7).ShouldBeTrue();
    }

    [Fact]
    public void TritoneSubstitution_CanSubstitute_Major7_ShouldReturn_False()
    {
        // Arrange - Major 7 chords don't typically get tritone substituted
        var major7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act & Assert
        TritoneSubstitution.CanSubstitute(major7).ShouldBeFalse();
    }

    [Fact]
    public void TritoneSubstitution_CanSubstitute_MajorTriad_ShouldReturn_True()
    {
        // Arrange - Major triads can act as dominants
        var majorTriad = new Chord(new Note(NoteName.G), ChordQuality.Major);

        // Act & Assert
        TritoneSubstitution.CanSubstitute(majorTriad).ShouldBeTrue();
    }

    [Fact]
    public void TritoneSubstitution_CanSubstitute_MinorTriad_ShouldReturn_False()
    {
        // Arrange
        var minorTriad = new Chord(new Note(NoteName.A), ChordQuality.Minor);

        // Act & Assert
        TritoneSubstitution.CanSubstitute(minorTriad).ShouldBeFalse();
    }

    #endregion

    #region AreTritoneRelated

    [Fact]
    public void TritoneSubstitution_AreTritoneRelated_G7AndDb7_ShouldReturn_True()
    {
        // Arrange
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var db7 = new Chord(new Note(NoteName.D, Alteration.Flat), ChordType.Dominant7);

        // Act & Assert
        TritoneSubstitution.AreTritoneRelated(g7, db7).ShouldBeTrue();
    }

    [Fact]
    public void TritoneSubstitution_AreTritoneRelated_G7AndC7_ShouldReturn_False()
    {
        // Arrange
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var c7 = new Chord(new Note(NoteName.C), ChordType.Dominant7);

        // Act & Assert
        TritoneSubstitution.AreTritoneRelated(g7, c7).ShouldBeFalse();
    }

    [Fact]
    public void TritoneSubstitution_AreTritoneRelated_ShouldBe_Symmetric()
    {
        // Arrange
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var db7 = new Chord(new Note(NoteName.D, Alteration.Flat), ChordType.Dominant7);

        // Act & Assert - Both directions should return true
        TritoneSubstitution.AreTritoneRelated(g7, db7).ShouldBeTrue();
        TritoneSubstitution.AreTritoneRelated(db7, g7).ShouldBeTrue();
    }

    #endregion

    #region GetSharedGuideTones

    [Fact]
    public void TritoneSubstitution_GetSharedGuideTones_ShouldReturn_ThirdAndSeventh()
    {
        // Arrange - G7 (G, B, D, F) and Db7 (Db, F, Ab, Cb/B)
        // The tritone B-F is shared (enharmonically)
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var db7 = new Chord(new Note(NoteName.D, Alteration.Flat), ChordType.Dominant7);

        // Act
        var guideTones = TritoneSubstitution.GetSharedGuideTones(g7, db7);

        // Assert - Should have 2 guide tones (B/Cb and F)
        guideTones.Count.ShouldBe(2);
    }

    #endregion

    #region Practical Application

    [Fact]
    public void TritoneSubstitution_iiVI_WithSubV_ShouldCreate_ChromaticBassLine()
    {
        // Arrange - ii-V-I in C: Dm7 - G7 - Cmaj7
        // With tritone sub: Dm7 - Db7 - Cmaj7 (chromatic bass: D - Db - C)
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var subV = TritoneSubstitution.GetSubstitute(g7);

        // Assert - Bass line should be chromatic
        var bassNotes = new[] { dm7.Root, subV.Root, cmaj7.Root };

        // D (2) -> Db (1) -> C (0) - chromatic descent
        bassNotes[0].Name.ShouldBe(NoteName.D);
        bassNotes[1].Name.ShouldBe(NoteName.D);
        bassNotes[1].Alteration.ShouldBe(Alteration.Flat);
        bassNotes[2].Name.ShouldBe(NoteName.C);
    }

    #endregion
}
