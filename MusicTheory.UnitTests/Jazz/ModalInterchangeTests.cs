namespace MusicTheory.UnitTests.Jazz;

using MusicTheory.Jazz;

public class ModalInterchangeTests
{
    // Helper to create C major key signature
    private static KeySignature CMajor => new(new Note(NoteName.C), KeyMode.Major);

    #region IsBorrowedChord

    [Fact]
    public void ModalInterchange_IsBorrowedChord_bVIInMajor_ShouldReturn_True()
    {
        // Arrange - Ab major chord in C major is borrowed from C minor
        var abMajor = new Chord(new Note(NoteName.A, Alteration.Flat), ChordQuality.Major);
        var cMajor = CMajor;

        // Act & Assert
        ModalInterchange.IsBorrowedChord(abMajor, cMajor).ShouldBeTrue();
    }

    [Fact]
    public void ModalInterchange_IsBorrowedChord_IVInMajor_ShouldReturn_False()
    {
        // Arrange - F major in C major is diatonic, not borrowed
        var fMajor = new Chord(new Note(NoteName.F), ChordQuality.Major);
        var cMajor = CMajor;

        // Act & Assert
        ModalInterchange.IsBorrowedChord(fMajor, cMajor).ShouldBeFalse();
    }

    [Fact]
    public void ModalInterchange_IsBorrowedChord_ivInMajor_ShouldReturn_True()
    {
        // Arrange - F minor in C major is borrowed from C minor
        var fMinor = new Chord(new Note(NoteName.F), ChordQuality.Minor);
        var cMajor = CMajor;

        // Act & Assert
        ModalInterchange.IsBorrowedChord(fMinor, cMajor).ShouldBeTrue();
    }

    [Fact]
    public void ModalInterchange_IsBorrowedChord_bVIIInMajor_ShouldReturn_True()
    {
        // Arrange - Bb major in C major is borrowed from C minor/mixolydian
        var bbMajor = new Chord(new Note(NoteName.B, Alteration.Flat), ChordQuality.Major);
        var cMajor = CMajor;

        // Act & Assert
        ModalInterchange.IsBorrowedChord(bbMajor, cMajor).ShouldBeTrue();
    }

    #endregion

    #region GetSourceMode

    [Fact]
    public void ModalInterchange_GetSourceMode_bVIInMajor_ShouldReturn_Aeolian()
    {
        // Arrange - Ab major is borrowed from Aeolian (natural minor)
        var abMajor = new Chord(new Note(NoteName.A, Alteration.Flat), ChordQuality.Major);
        var cMajor = CMajor;

        // Act
        var sourceMode = ModalInterchange.GetSourceMode(abMajor, cMajor);

        // Assert
        sourceMode.ShouldBe(BorrowedChordSource.Aeolian);
    }

    [Fact]
    public void ModalInterchange_GetSourceMode_bVIIInMajor_ShouldReturn_Mixolydian()
    {
        // Arrange - Bb major could be from Mixolydian
        var bbMajor = new Chord(new Note(NoteName.B, Alteration.Flat), ChordQuality.Major);
        var cMajor = CMajor;

        // Act
        var sourceMode = ModalInterchange.GetSourceMode(bbMajor, cMajor);

        // Assert - bVII can come from Mixolydian or Aeolian
        sourceMode.ShouldBeOneOf(BorrowedChordSource.Mixolydian, BorrowedChordSource.Aeolian);
    }

    [Fact]
    public void ModalInterchange_GetSourceMode_DiatonicChord_ShouldReturn_None()
    {
        // Arrange - G major is diatonic in C major
        var gMajor = new Chord(new Note(NoteName.G), ChordQuality.Major);
        var cMajor = CMajor;

        // Act
        var sourceMode = ModalInterchange.GetSourceMode(gMajor, cMajor);

        // Assert
        sourceMode.ShouldBe(BorrowedChordSource.None);
    }

    #endregion

    #region GetCommonBorrowedChords

    [Fact]
    public void ModalInterchange_GetCommonBorrowedChords_CMajor_ShouldInclude_bVI()
    {
        // Arrange
        var cMajor = CMajor;

        // Act
        var borrowedChords = ModalInterchange.GetCommonBorrowedChords(cMajor);

        // Assert - Should include Ab major (bVI)
        borrowedChords.ShouldContain(c =>
            c.Root.Name == NoteName.A &&
            c.Root.Alteration == Alteration.Flat &&
            c.Quality == ChordQuality.Major);
    }

    [Fact]
    public void ModalInterchange_GetCommonBorrowedChords_CMajor_ShouldInclude_iv()
    {
        // Arrange
        var cMajor = CMajor;

        // Act
        var borrowedChords = ModalInterchange.GetCommonBorrowedChords(cMajor);

        // Assert - Should include F minor (iv)
        borrowedChords.ShouldContain(c =>
            c.Root.Name == NoteName.F &&
            c.Root.Alteration == Alteration.Natural &&
            c.Quality == ChordQuality.Minor);
    }

    [Fact]
    public void ModalInterchange_GetCommonBorrowedChords_CMajor_ShouldInclude_bVII()
    {
        // Arrange
        var cMajor = CMajor;

        // Act
        var borrowedChords = ModalInterchange.GetCommonBorrowedChords(cMajor);

        // Assert - Should include Bb major (bVII)
        borrowedChords.ShouldContain(c =>
            c.Root.Name == NoteName.B &&
            c.Root.Alteration == Alteration.Flat &&
            c.Quality == ChordQuality.Major);
    }

    [Fact]
    public void ModalInterchange_GetCommonBorrowedChords_CMajor_ShouldInclude_bIII()
    {
        // Arrange
        var cMajor = CMajor;

        // Act
        var borrowedChords = ModalInterchange.GetCommonBorrowedChords(cMajor);

        // Assert - Should include Eb major (bIII)
        borrowedChords.ShouldContain(c =>
            c.Root.Name == NoteName.E &&
            c.Root.Alteration == Alteration.Flat &&
            c.Quality == ChordQuality.Major);
    }

    #endregion

    #region GetRomanNumeralForBorrowed

    [Fact]
    public void ModalInterchange_GetRomanNumeral_bVIInMajor_ShouldReturn_bVI()
    {
        // Arrange
        var abMajor = new Chord(new Note(NoteName.A, Alteration.Flat), ChordQuality.Major);
        var cMajor = CMajor;

        // Act
        var romanNumeral = ModalInterchange.GetRomanNumeral(abMajor, cMajor);

        // Assert
        romanNumeral.ShouldBe("bVI");
    }

    [Fact]
    public void ModalInterchange_GetRomanNumeral_ivInMajor_ShouldReturn_iv()
    {
        // Arrange
        var fMinor = new Chord(new Note(NoteName.F), ChordQuality.Minor);
        var cMajor = CMajor;

        // Act
        var romanNumeral = ModalInterchange.GetRomanNumeral(fMinor, cMajor);

        // Assert
        romanNumeral.ShouldBe("iv");
    }

    [Fact]
    public void ModalInterchange_GetRomanNumeral_bVIIInMajor_ShouldReturn_bVII()
    {
        // Arrange
        var bbMajor = new Chord(new Note(NoteName.B, Alteration.Flat), ChordQuality.Major);
        var cMajor = CMajor;

        // Act
        var romanNumeral = ModalInterchange.GetRomanNumeral(bbMajor, cMajor);

        // Assert
        romanNumeral.ShouldBe("bVII");
    }

    #endregion

    #region Practical Applications

    [Fact]
    public void ModalInterchange_PlagalMinorCadence_ivToI()
    {
        // Arrange - The classic "Amen" cadence variant with borrowed iv
        var fMinor = new Chord(new Note(NoteName.F), ChordQuality.Minor);
        var cMajorChord = new Chord(new Note(NoteName.C), ChordQuality.Major);
        var key = CMajor;

        // Act & Assert
        ModalInterchange.IsBorrowedChord(fMinor, key).ShouldBeTrue();
        ModalInterchange.IsBorrowedChord(cMajorChord, key).ShouldBeFalse();
    }

    [Fact]
    public void ModalInterchange_DeceptiveCadence_VtobVI()
    {
        // Arrange - V to bVI (borrowed deceptive resolution)
        var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        var abMajor = new Chord(new Note(NoteName.A, Alteration.Flat), ChordQuality.Major);
        var key = CMajor;

        // Act & Assert
        ModalInterchange.IsBorrowedChord(g7, key).ShouldBeFalse(); // G7 is diatonic V7
        ModalInterchange.IsBorrowedChord(abMajor, key).ShouldBeTrue(); // Ab is borrowed bVI
    }

    #endregion
}
