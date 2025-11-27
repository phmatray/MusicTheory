namespace MusicTheory.UnitTests.Jazz;

using MusicTheory.Jazz;

public class ChordScaleTheoryTests
{
    #region GetScalesForChord

    [Fact]
    public void ChordScaleTheory_GetScales_Maj7_ShouldInclude_Ionian()
    {
        // Arrange - Cmaj7 chord
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);

        // Act
        var scales = ChordScaleTheory.GetScalesForChord(cmaj7);

        // Assert
        scales.ShouldContain(s => s.Type == ScaleType.Major);
    }

    [Fact]
    public void ChordScaleTheory_GetScales_Min7_ShouldInclude_Dorian()
    {
        // Arrange - Dm7 chord
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);

        // Act
        var scales = ChordScaleTheory.GetScalesForChord(dm7);

        // Assert
        scales.ShouldContain(s => s.Type == ScaleType.Dorian);
    }

    [Fact]
    public void ChordScaleTheory_GetScales_Dom7_ShouldInclude_Mixolydian()
    {
        // Arrange - G7 chord
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);

        // Act
        var scales = ChordScaleTheory.GetScalesForChord(g7);

        // Assert
        scales.ShouldContain(s => s.Type == ScaleType.Mixolydian);
    }

    [Fact]
    public void ChordScaleTheory_GetScales_HalfDim7_ShouldInclude_Locrian()
    {
        // Arrange - Bm7b5 chord
        var bm7b5 = new Chord(new Note(NoteName.B), ChordType.HalfDiminished7);

        // Act
        var scales = ChordScaleTheory.GetScalesForChord(bm7b5);

        // Assert
        scales.ShouldContain(s => s.Type == ScaleType.Locrian);
    }

    [Fact]
    public void ChordScaleTheory_GetScales_Dim7_ShouldInclude_DiminishedWH()
    {
        // Arrange - Cdim7 chord
        var cdim7 = new Chord(new Note(NoteName.C), ChordType.Diminished7);

        // Act
        var scales = ChordScaleTheory.GetScalesForChord(cdim7);

        // Assert
        scales.ShouldContain(s => s.Type == ScaleType.DiminishedWholeHalf);
    }

    #endregion

    #region GetScalesForChord with Key Context

    [Fact]
    public void ChordScaleTheory_GetScales_IIM7InMajor_ShouldReturn_Dorian()
    {
        // Arrange - Dm7 as ii in C major
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);
        var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var scale = ChordScaleTheory.GetPrimaryScale(dm7, cMajor);

        // Assert - Dorian is the primary scale for ii-7 in major
        scale.Type.ShouldBe(ScaleType.Dorian);
    }

    [Fact]
    public void ChordScaleTheory_GetScales_V7InMajor_ShouldReturn_Mixolydian()
    {
        // Arrange - G7 as V in C major
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var scale = ChordScaleTheory.GetPrimaryScale(g7, cMajor);

        // Assert
        scale.Type.ShouldBe(ScaleType.Mixolydian);
    }

    [Fact]
    public void ChordScaleTheory_GetScales_IMaj7InMajor_ShouldReturn_Ionian()
    {
        // Arrange - Cmaj7 as I in C major
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);
        var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var scale = ChordScaleTheory.GetPrimaryScale(cmaj7, cMajor);

        // Assert
        scale.Type.ShouldBe(ScaleType.Major); // Ionian = Major
    }

    #endregion

    #region GetAvoidNotes

    [Fact]
    public void ChordScaleTheory_GetAvoidNotes_Ionian_ShouldReturn_4th()
    {
        // Arrange - C Ionian on Cmaj7
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);
        var cIonian = new Scale(new Note(NoteName.C), ScaleType.Major);

        // Act
        var avoidNotes = ChordScaleTheory.GetAvoidNotes(cmaj7, cIonian);

        // Assert - 4th degree (F) is avoid note in Ionian
        avoidNotes.ShouldContain(n => n.Name == NoteName.F && n.Alteration == Alteration.Natural);
    }

    [Fact]
    public void ChordScaleTheory_GetAvoidNotes_Dorian_ShouldReturn_6th()
    {
        // Arrange - D Dorian on Dm7
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);
        var dDorian = new Scale(new Note(NoteName.D), ScaleType.Dorian);

        // Act
        var avoidNotes = ChordScaleTheory.GetAvoidNotes(dm7, dDorian);

        // Assert - 6th degree (B) is avoid note in Dorian (minor 9th above 5th)
        avoidNotes.ShouldContain(n => n.Name == NoteName.B && n.Alteration == Alteration.Natural);
    }

    [Fact]
    public void ChordScaleTheory_GetAvoidNotes_Mixolydian_ShouldReturn_4th()
    {
        // Arrange - G Mixolydian on G7
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var gMixolydian = new Scale(new Note(NoteName.G), ScaleType.Mixolydian);

        // Act
        var avoidNotes = ChordScaleTheory.GetAvoidNotes(g7, gMixolydian);

        // Assert - 4th degree (C) is avoid note in Mixolydian
        avoidNotes.ShouldContain(n => n.Name == NoteName.C && n.Alteration == Alteration.Natural);
    }

    #endregion

    #region GetColorTones

    [Fact]
    public void ChordScaleTheory_GetColorTones_Maj7_ShouldInclude_9And13()
    {
        // Arrange
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);
        var cIonian = new Scale(new Note(NoteName.C), ScaleType.Major);

        // Act
        var colorTones = ChordScaleTheory.GetColorTones(cmaj7, cIonian);

        // Assert - 9 (D) and 13 (A) are color tones
        colorTones.ShouldContain(n => n.Name == NoteName.D); // 9th
        colorTones.ShouldContain(n => n.Name == NoteName.A); // 13th
    }

    [Fact]
    public void ChordScaleTheory_GetColorTones_Dom7_ShouldInclude_9And13()
    {
        // Arrange
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var gMixolydian = new Scale(new Note(NoteName.G), ScaleType.Mixolydian);

        // Act
        var colorTones = ChordScaleTheory.GetColorTones(g7, gMixolydian);

        // Assert - 9 (A) and 13 (E) are color tones
        colorTones.ShouldContain(n => n.Name == NoteName.A); // 9th
        colorTones.ShouldContain(n => n.Name == NoteName.E); // 13th
    }

    #endregion

    #region Jazz Scale Types

    [Fact]
    public void ChordScaleTheory_GetScales_AlteredDom7_ShouldInclude_Altered()
    {
        // Arrange - G7alt chord
        var g7alt = new Chord(new Note(NoteName.G), ChordType.Dominant7Alt);

        // Act
        var scales = ChordScaleTheory.GetScalesForChord(g7alt);

        // Assert
        scales.ShouldContain(s => s.Type == ScaleType.Altered);
    }

    [Fact]
    public void ChordScaleTheory_GetScales_MinMaj7_ShouldInclude_MelodicMinor()
    {
        // Arrange - CmMaj7 chord
        var cmMaj7 = new Chord(new Note(NoteName.C), ChordType.MinorMajor7);

        // Act
        var scales = ChordScaleTheory.GetScalesForChord(cmMaj7);

        // Assert
        scales.ShouldContain(s => s.Type == ScaleType.MelodicMinor);
    }

    #endregion

    #region II-V-I Chord Scale Analysis

    [Fact]
    public void ChordScaleTheory_IIVI_InCMajor_ShouldReturn_CorrectScales()
    {
        // Arrange - ii-V-I in C: Dm7 - G7 - Cmaj7
        var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var cmaj7 = new Chord(new Note(NoteName.C), ChordType.Major7);
        var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var dm7Scale = ChordScaleTheory.GetPrimaryScale(dm7, cMajor);
        var g7Scale = ChordScaleTheory.GetPrimaryScale(g7, cMajor);
        var cmaj7Scale = ChordScaleTheory.GetPrimaryScale(cmaj7, cMajor);

        // Assert
        dm7Scale.Type.ShouldBe(ScaleType.Dorian);       // D Dorian for ii-7
        g7Scale.Type.ShouldBe(ScaleType.Mixolydian);    // G Mixolydian for V7
        cmaj7Scale.Type.ShouldBe(ScaleType.Major);      // C Ionian for Imaj7
    }

    #endregion
}
