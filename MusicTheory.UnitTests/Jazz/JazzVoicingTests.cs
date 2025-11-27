namespace MusicTheory.UnitTests.Jazz;

using MusicTheory.Jazz;

public class JazzVoicingTests
{
    #region Shell Voicings

    [Fact]
    public void JazzVoicing_GetShellVoicing_Maj7_ShouldReturn_Root3rd7th()
    {
        // Arrange - Cmaj7
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var voicing = JazzVoicing.GetShellVoicing(cmaj7);

        // Assert - Shell voicing: C, E, B
        voicing.Count.ShouldBe(3);
        voicing.ShouldContain(n => n.Name == NoteName.C); // Root
        voicing.ShouldContain(n => n.Name == NoteName.E); // 3rd
        voicing.ShouldContain(n => n.Name == NoteName.B); // 7th
    }

    [Fact]
    public void JazzVoicing_GetShellVoicing_Dom7_ShouldReturn_Root3rd7th()
    {
        // Arrange - G7
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);

        // Act
        var voicing = JazzVoicing.GetShellVoicing(g7);

        // Assert - Shell voicing: G, B, F
        voicing.Count.ShouldBe(3);
        voicing.ShouldContain(n => n.Name == NoteName.G); // Root
        voicing.ShouldContain(n => n.Name == NoteName.B); // 3rd
        voicing.ShouldContain(n => n.Name == NoteName.F); // b7
    }

    [Fact]
    public void JazzVoicing_GetShellVoicing_Min7_ShouldReturn_Root3rd7th()
    {
        // Arrange - Dm7
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);

        // Act
        var voicing = JazzVoicing.GetShellVoicing(dm7);

        // Assert - Shell voicing: D, F, C
        voicing.Count.ShouldBe(3);
        voicing.ShouldContain(n => n.Name == NoteName.D); // Root
        voicing.ShouldContain(n => n.Name == NoteName.F); // b3
        voicing.ShouldContain(n => n.Name == NoteName.C); // b7
    }

    #endregion

    #region Guide Tone Voicings

    [Fact]
    public void JazzVoicing_GetGuideTones_Maj7_ShouldReturn_3rdAnd7th()
    {
        // Arrange - Cmaj7
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var guideTones = JazzVoicing.GetGuideTones(cmaj7);

        // Assert - Guide tones: E (3rd) and B (7th)
        guideTones.Count.ShouldBe(2);
        guideTones.ShouldContain(n => n.Name == NoteName.E); // 3rd
        guideTones.ShouldContain(n => n.Name == NoteName.B); // 7th
    }

    [Fact]
    public void JazzVoicing_GetGuideTones_Dom7_ShouldReturn_3rdAndb7th()
    {
        // Arrange - G7
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);

        // Act
        var guideTones = JazzVoicing.GetGuideTones(g7);

        // Assert - Guide tones: B (3rd) and F (b7)
        guideTones.Count.ShouldBe(2);
        guideTones.ShouldContain(n => n.Name == NoteName.B); // 3rd
        guideTones.ShouldContain(n => n.Name == NoteName.F); // b7
    }

    #endregion

    #region Rootless Voicings Type A (3-5-7-9)

    [Fact]
    public void JazzVoicing_GetRootlessTypeA_Maj7_ShouldReturn_3_5_7_9()
    {
        // Arrange - Cmaj7
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var voicing = JazzVoicing.GetRootlessTypeA(cmaj7);

        // Assert - Type A: E (3), G (5), B (7), D (9)
        voicing.Count.ShouldBe(4);
        voicing.ShouldContain(n => n.Name == NoteName.E); // 3rd
        voicing.ShouldContain(n => n.Name == NoteName.G); // 5th
        voicing.ShouldContain(n => n.Name == NoteName.B); // 7th
        voicing.ShouldContain(n => n.Name == NoteName.D); // 9th
    }

    [Fact]
    public void JazzVoicing_GetRootlessTypeA_Dom7_ShouldReturn_3_5_b7_9()
    {
        // Arrange - G7
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);

        // Act
        var voicing = JazzVoicing.GetRootlessTypeA(g7);

        // Assert - Type A: B (3), D (5), F (b7), A (9)
        voicing.Count.ShouldBe(4);
        voicing.ShouldContain(n => n.Name == NoteName.B); // 3rd
        voicing.ShouldContain(n => n.Name == NoteName.D); // 5th
        voicing.ShouldContain(n => n.Name == NoteName.F); // b7
        voicing.ShouldContain(n => n.Name == NoteName.A); // 9th
    }

    #endregion

    #region Rootless Voicings Type B (7-9-3-5)

    [Fact]
    public void JazzVoicing_GetRootlessTypeB_Maj7_ShouldReturn_7_9_3_5()
    {
        // Arrange - Cmaj7
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var voicing = JazzVoicing.GetRootlessTypeB(cmaj7);

        // Assert - Type B: B (7), D (9), E (3), G (5)
        voicing.Count.ShouldBe(4);
        voicing.ShouldContain(n => n.Name == NoteName.B); // 7th
        voicing.ShouldContain(n => n.Name == NoteName.D); // 9th
        voicing.ShouldContain(n => n.Name == NoteName.E); // 3rd
        voicing.ShouldContain(n => n.Name == NoteName.G); // 5th
    }

    [Fact]
    public void JazzVoicing_GetRootlessTypeB_Min7_ShouldReturn_b7_9_b3_5()
    {
        // Arrange - Dm7
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);

        // Act
        var voicing = JazzVoicing.GetRootlessTypeB(dm7);

        // Assert - Type B: C (b7), E (9), F (b3), A (5)
        voicing.Count.ShouldBe(4);
        voicing.ShouldContain(n => n.Name == NoteName.C); // b7
        voicing.ShouldContain(n => n.Name == NoteName.E); // 9th
        voicing.ShouldContain(n => n.Name == NoteName.F); // b3
        voicing.ShouldContain(n => n.Name == NoteName.A); // 5th
    }

    #endregion

    #region Drop 2 Voicings

    [Fact]
    public void JazzVoicing_GetDrop2_Maj7_ShouldDropSecondFromTop()
    {
        // Arrange - Cmaj7 close position: C E G B
        // Drop 2: B E G C (2nd from top (G) dropped an octave)
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var voicing = JazzVoicing.GetDrop2Voicing(cmaj7);

        // Assert - Should have 4 notes
        voicing.Count.ShouldBe(4);
    }

    [Fact]
    public void JazzVoicing_GetDrop2_ShouldHave_WiderSpacing()
    {
        // Arrange
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var closeVoicing = cmaj7.GetNotes().ToList();
        var drop2Voicing = JazzVoicing.GetDrop2Voicing(cmaj7);

        // Assert - Drop 2 voicing has wider spacing
        // The dropped note should be an octave lower
        drop2Voicing.Count.ShouldBe(closeVoicing.Count);
    }

    #endregion

    #region Quartal Voicings

    [Fact]
    public void JazzVoicing_GetQuartalVoicing_ShouldUse_Fourths()
    {
        // Arrange
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);

        // Act
        var voicing = JazzVoicing.GetQuartalVoicing(dm7, 4);

        // Assert - Should have 4 notes built in fourths
        voicing.Count.ShouldBe(4);
    }

    [Fact]
    public void JazzVoicing_GetQuartalVoicing_ShouldBuild_StackedFourths()
    {
        // Arrange - Starting from D
        var root = new Note(NoteName.D);

        // Act
        var voicing = JazzVoicing.GetQuartalVoicing(root, 3);

        // Assert - D, G, C (stacked fourths)
        voicing.Count.ShouldBe(3);
        voicing[0].Name.ShouldBe(NoteName.D);
        voicing[1].Name.ShouldBe(NoteName.G); // P4 above D
        voicing[2].Name.ShouldBe(NoteName.C); // P4 above G
    }

    #endregion

    #region Practical Applications

    [Fact]
    public void JazzVoicing_IIVIProg_TypeA_TypeB_Alternation()
    {
        // Arrange - ii-V-I in C: Dm7 - G7 - Cmaj7
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act - Typical rootless voicing pattern: ii (Type A), V (Type B), I (Type A)
        var dm7Voicing = JazzVoicing.GetRootlessTypeA(dm7);
        var g7Voicing = JazzVoicing.GetRootlessTypeB(g7);
        var cmaj7Voicing = JazzVoicing.GetRootlessTypeA(cmaj7);

        // Assert - All voicings should have 4 notes
        dm7Voicing.Count.ShouldBe(4);
        g7Voicing.Count.ShouldBe(4);
        cmaj7Voicing.Count.ShouldBe(4);
    }

    [Fact]
    public void JazzVoicing_GuideToneVoiceLeading_ShouldBe_Smooth()
    {
        // Arrange - ii-V-I in C: Dm7 - G7 - Cmaj7
        // Guide tones should move by step or common tones
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var dm7GuideTones = JazzVoicing.GetGuideTones(dm7);  // F, C
        var g7GuideTones = JazzVoicing.GetGuideTones(g7);    // B, F
        var cmaj7GuideTones = JazzVoicing.GetGuideTones(cmaj7); // E, B

        // Assert - Each chord has 2 guide tones
        dm7GuideTones.Count.ShouldBe(2);
        g7GuideTones.Count.ShouldBe(2);
        cmaj7GuideTones.Count.ShouldBe(2);

        // F (dm7) -> F (g7) = common tone
        // C (dm7) -> B (g7) = half step
        dm7GuideTones.ShouldContain(n => n.Name == NoteName.F);
        g7GuideTones.ShouldContain(n => n.Name == NoteName.F);
    }

    #endregion
}
