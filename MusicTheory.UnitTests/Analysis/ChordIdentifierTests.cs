namespace MusicTheory.UnitTests.Analysis;

using MusicTheory.Analysis;

public class ChordIdentifierTests
{
    #region Basic Triad Identification (Root Position)

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_CMajorTriad()
    {
        // Arrange - C, E, G
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Major);
        result.Inversion.ShouldBe(ChordInversion.Root);
        result.Confidence.ShouldBeGreaterThan(0.9);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_AMinorTriad()
    {
        // Arrange - A, C, E
        var notes = new[]
        {
            new Note(NoteName.A, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Natural, 5),
            new Note(NoteName.E, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.A);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Minor);
        result.Inversion.ShouldBe(ChordInversion.Root);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_BDiminishedTriad()
    {
        // Arrange - B, D, F
        var notes = new[]
        {
            new Note(NoteName.B, Alteration.Natural, 4),
            new Note(NoteName.D, Alteration.Natural, 5),
            new Note(NoteName.F, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.B);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Diminished);
        result.Inversion.ShouldBe(ChordInversion.Root);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_CAugmentedTriad()
    {
        // Arrange - C, E, G#
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Sharp, 4)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Augmented);
        result.Inversion.ShouldBe(ChordInversion.Root);
    }

    [Theory]
    [InlineData(NoteName.C, NoteName.E, NoteName.G, ChordQuality.Major)]       // C major
    [InlineData(NoteName.D, NoteName.F, NoteName.A, ChordQuality.Minor)]       // D minor
    [InlineData(NoteName.E, NoteName.G, NoteName.B, ChordQuality.Minor)]       // E minor
    [InlineData(NoteName.F, NoteName.A, NoteName.C, ChordQuality.Major)]       // F major
    [InlineData(NoteName.G, NoteName.B, NoteName.D, ChordQuality.Major)]       // G major
    [InlineData(NoteName.A, NoteName.C, NoteName.E, ChordQuality.Minor)]       // A minor
    public void ChordIdentifier_Identify_ShouldIdentify_DiatonicTriads(
        NoteName root, NoteName third, NoteName fifth, ChordQuality expectedQuality)
    {
        // Arrange
        var notes = new[]
        {
            new Note(root, Alteration.Natural, 4),
            new Note(third, Alteration.Natural, 4),
            new Note(fifth, Alteration.Natural, 4)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(root);
        result.IdentifiedChord.Quality.ShouldBe(expectedQuality);
    }

    #endregion

    #region Notes in Different Order

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_TriadInAnyNoteOrder()
    {
        // Arrange - G, C, E (C major but notes not in order)
        var notes = new[]
        {
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Major);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldHandle_NotesAcrossOctaves()
    {
        // Arrange - C4, G4, E5 (spread voicing)
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Major);
    }

    #endregion

    #region Error Cases

    [Fact]
    public void ChordIdentifier_Identify_ShouldThrow_ForFewerThanThreeNotes()
    {
        // Arrange
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4)
        };

        // Act
        Action act = () => ChordIdentifier.Identify(notes);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldThrow_ForEmptyCollection()
    {
        // Arrange
        var notes = Array.Empty<Note>();

        // Act
        Action act = () => ChordIdentifier.Identify(notes);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldThrow_ForNullCollection()
    {
        // Act
        Action act = () => ChordIdentifier.Identify(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    #endregion

    #region Duplicate Notes

    [Fact]
    public void ChordIdentifier_Identify_ShouldHandle_DuplicateNotes()
    {
        // Arrange - C, E, G, C (octave doubling)
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Major);
    }

    #endregion

    #region Sharp/Flat Chords

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_FSharpMinor()
    {
        // Arrange - F#, A, C#
        var notes = new[]
        {
            new Note(NoteName.F, Alteration.Sharp, 4),
            new Note(NoteName.A, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Sharp, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.F);
        result.IdentifiedChord.Root.Alteration.ShouldBe(Alteration.Sharp);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Minor);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_BbMajor()
    {
        // Arrange - Bb, D, F
        var notes = new[]
        {
            new Note(NoteName.B, Alteration.Flat, 4),
            new Note(NoteName.D, Alteration.Natural, 5),
            new Note(NoteName.F, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.B);
        result.IdentifiedChord.Root.Alteration.ShouldBe(Alteration.Flat);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Major);
    }

    #endregion

    #region Inversions

    [Fact]
    public void ChordIdentifier_Identify_ShouldDetect_FirstInversion()
    {
        // Arrange - E, G, C (C major first inversion - bass is E/3rd)
        var notes = new[]
        {
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Major);
        result.Inversion.ShouldBe(ChordInversion.First);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldDetect_SecondInversion()
    {
        // Arrange - G, C, E (C major second inversion - bass is G/5th)
        var notes = new[]
        {
            new Note(NoteName.G, Alteration.Natural, 3),
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Major);
        result.Inversion.ShouldBe(ChordInversion.Second);
    }

    [Theory]
    [InlineData(NoteName.C, NoteName.E, NoteName.G, ChordInversion.Root)]   // C in bass = root
    [InlineData(NoteName.E, NoteName.G, NoteName.C, ChordInversion.First)]  // E in bass = first
    [InlineData(NoteName.G, NoteName.C, NoteName.E, ChordInversion.Second)] // G in bass = second
    public void ChordIdentifier_Identify_ShouldDetect_AllTriadInversions(
        NoteName bassNote, NoteName middleNote, NoteName topNote, ChordInversion expectedInversion)
    {
        // Arrange - Different voicings with bass note determining octave
        var bassOctave = 3;
        var middleOctave = bassNote == NoteName.E ? 4 : (bassNote == NoteName.G ? 4 : 4);
        var topOctave = bassNote == NoteName.E ? 4 : (bassNote == NoteName.G ? 4 : 4);

        var notes = new[]
        {
            new Note(bassNote, Alteration.Natural, bassOctave),
            new Note(middleNote, Alteration.Natural, middleOctave),
            new Note(topNote, Alteration.Natural, topOctave + 1)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Quality.ShouldBe(ChordQuality.Major);
        result.Inversion.ShouldBe(expectedInversion);
    }

    #endregion

    #region Seventh Chords

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_DominantSeventh()
    {
        // Arrange - G, B, D, F (G7)
        var notes = new[]
        {
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.B, Alteration.Natural, 4),
            new Note(NoteName.D, Alteration.Natural, 5),
            new Note(NoteName.F, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.G);
        result.IdentifiedChord.Type.ShouldBe(ChordType.Dominant7);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_MajorSeventh()
    {
        // Arrange - C, E, G, B (Cmaj7)
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.B, Alteration.Natural, 4)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.C);
        result.IdentifiedChord.Type.ShouldBe(ChordType.Major7);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_MinorSeventh()
    {
        // Arrange - A, C, E, G (Am7)
        var notes = new[]
        {
            new Note(NoteName.A, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Natural, 5),
            new Note(NoteName.E, Alteration.Natural, 5),
            new Note(NoteName.G, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.A);
        result.IdentifiedChord.Type.ShouldBe(ChordType.Minor7);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_HalfDiminishedSeventh()
    {
        // Arrange - B, D, F, A (Bm7b5 / B half-diminished)
        var notes = new[]
        {
            new Note(NoteName.B, Alteration.Natural, 4),
            new Note(NoteName.D, Alteration.Natural, 5),
            new Note(NoteName.F, Alteration.Natural, 5),
            new Note(NoteName.A, Alteration.Natural, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.B);
        result.IdentifiedChord.Type.ShouldBe(ChordType.HalfDiminished7);
    }

    [Fact]
    public void ChordIdentifier_Identify_ShouldIdentify_DiminishedSeventh()
    {
        // Arrange - B, D, F, Ab (B°7 / B fully diminished)
        var notes = new[]
        {
            new Note(NoteName.B, Alteration.Natural, 4),
            new Note(NoteName.D, Alteration.Natural, 5),
            new Note(NoteName.F, Alteration.Natural, 5),
            new Note(NoteName.A, Alteration.Flat, 5)
        };

        // Act
        var result = ChordIdentifier.Identify(notes);

        // Assert
        result.IdentifiedChord.Root.Name.ShouldBe(NoteName.B);
        result.IdentifiedChord.Type.ShouldBe(ChordType.Diminished7);
    }

    #endregion
}
