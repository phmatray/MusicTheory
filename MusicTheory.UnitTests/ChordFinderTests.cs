namespace MusicTheory.UnitTests;

/// <summary>
/// Tests for <see cref="ChordFinder"/>.
/// Verifies chord identification from note sets.
/// </summary>
public class ChordFinderTests
{
    // ── Basic Triads ─────────────────────────────────────────────────────────

    [Fact]
    public void Identify_CMajorTriad_FindsCMajor()
    {
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4)
        };

        var matches = ChordFinder.Identify(notes).ToList();

        matches.ShouldNotBeEmpty();
        matches.ShouldContain(m => m.Root.Name == NoteName.C && m.Type == ChordType.Major);
    }

    [Fact]
    public void Identify_AMinorTriad_FindsAMinor()
    {
        var notes = new[]
        {
            new Note(NoteName.A, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Natural, 5),
            new Note(NoteName.E, Alteration.Natural, 5)
        };

        var matches = ChordFinder.Identify(notes).ToList();

        matches.ShouldNotBeEmpty();
        matches.ShouldContain(m => m.Root.Name == NoteName.A && m.Type == ChordType.Minor);
    }

    [Fact]
    public void Identify_BDiminishedTriad_FindsBDiminished()
    {
        var notes = new[]
        {
            new Note(NoteName.B, Alteration.Natural, 4),
            new Note(NoteName.D, Alteration.Natural, 5),
            new Note(NoteName.F, Alteration.Natural, 5)
        };

        var matches = ChordFinder.Identify(notes).ToList();

        matches.ShouldNotBeEmpty();
        matches.ShouldContain(m => m.Root.Name == NoteName.B && m.Type == ChordType.Diminished);
    }

    // ── Seventh Chords ───────────────────────────────────────────────────────

    [Fact]
    public void Identify_CMaj7_FindsCorrectChord()
    {
        // C E G B
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.B, Alteration.Natural, 4)
        };

        var matches = ChordFinder.Identify(notes).ToList();

        matches.ShouldContain(m => m.Root.Name == NoteName.C && m.Type == ChordType.Major7);
    }

    [Fact]
    public void Identify_G7_FindsDominantSeventh()
    {
        // G B D F
        var notes = new[]
        {
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.B, Alteration.Natural, 4),
            new Note(NoteName.D, Alteration.Natural, 5),
            new Note(NoteName.F, Alteration.Natural, 5)
        };

        var matches = ChordFinder.Identify(notes).ToList();

        matches.ShouldContain(m => m.Root.Name == NoteName.G && m.Type == ChordType.Dominant7);
    }

    // ── Octave Independence ───────────────────────────────────────────────────

    [Fact]
    public void Identify_SameChordDifferentOctaves_StillIdentifies()
    {
        // C in octave 4, E in octave 5, G in octave 6 — same pitch classes
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 5),
            new Note(NoteName.G, Alteration.Natural, 6)
        };

        var matches = ChordFinder.Identify(notes).ToList();

        matches.ShouldContain(m => m.Root.Name == NoteName.C && m.Type == ChordType.Major);
    }

    // ── Order Independence ────────────────────────────────────────────────────

    [Fact]
    public void Identify_NotesInAnyOrder_StillIdentifies()
    {
        // G E C (reversed)
        var notes = new[]
        {
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Natural, 4)
        };

        var matches = ChordFinder.Identify(notes).ToList();

        matches.ShouldContain(m => m.Root.Name == NoteName.C && m.Type == ChordType.Major);
    }

    // ── Extra Notes / Tensions ────────────────────────────────────────────────

    [Fact]
    public void Identify_ChordWithAddedTension_StillFindsBaseChord()
    {
        // C E G D → Cadd9 AND also C major (with extra note)
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.D, Alteration.Natural, 5)
        };

        var matches = ChordFinder.Identify(notes).ToList();

        // Should find Cadd9 (exact match) and C major (extra D)
        matches.ShouldContain(m => m.Root.Name == NoteName.C && m.Type == ChordType.MajorAdd9);
        matches.ShouldContain(m => m.Root.Name == NoteName.C && m.Type == ChordType.Major);
    }

    [Fact]
    public void Identify_ChordWithExtraNotes_ExtraNotesCountIsCorrect()
    {
        // C E G — exact match for C major
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4)
        };

        var exact = ChordFinder.Identify(notes)
            .FirstOrDefault(m => m.Root.Name == NoteName.C && m.Type == ChordType.Major);

        exact.ShouldNotBeNull();
        exact!.ExtraNotes.ShouldBe(0);
    }

    // ── BestMatch ─────────────────────────────────────────────────────────────

    [Fact]
    public void BestMatch_CMajorTriad_ReturnsCMajor()
    {
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4)
        };

        var best = ChordFinder.BestMatch(notes);

        best.ShouldNotBeNull();
        best!.Root.Name.ShouldBe(NoteName.C);
        best.Type.ShouldBe(ChordType.Major);
    }

    [Fact]
    public void BestMatch_SingleNote_ReturnsNull()
    {
        var notes = new[] { new Note(NoteName.C) };

        var best = ChordFinder.BestMatch(notes);

        best.ShouldBeNull();
    }

    [Fact]
    public void BestMatch_EmptyNotes_ReturnsNull()
    {
        var best = ChordFinder.BestMatch(Array.Empty<Note>());

        best.ShouldBeNull();
    }

    // ── ChordMatch helpers ────────────────────────────────────────────────────

    [Fact]
    public void ChordMatch_ToChord_ReturnsCorrectChord()
    {
        var match = new ChordMatch(new Note(NoteName.C), ChordType.Major7, 0);
        var chord = match.ToChord();

        chord.Root.Name.ShouldBe(NoteName.C);
        chord.Type.ShouldBe(ChordType.Major7);
    }

    [Fact]
    public void ChordMatch_Symbol_ReturnsCorrectSymbol()
    {
        var match = new ChordMatch(new Note(NoteName.G), ChordType.Dominant7, 0);
        match.Symbol.ShouldBe("G7");
    }

    // ── IdentifyFromPitchClasses ──────────────────────────────────────────────

    [Fact]
    public void IdentifyFromPitchClasses_CMajor_Identifies()
    {
        // 0=C, 4=E, 7=G
        var matches = ChordFinder.IdentifyFromPitchClasses([0, 4, 7]).ToList();

        matches.ShouldContain(m => m.Root.Name == NoteName.C && m.Type == ChordType.Major);
    }

    [Fact]
    public void IdentifyFromPitchClasses_BeyondOctave_Normalises()
    {
        // 12=C (octave), 16=E (octave), 19=G (octave)
        var matches = ChordFinder.IdentifyFromPitchClasses([12, 16, 19]).ToList();

        matches.ShouldContain(m => m.Root.Name == NoteName.C && m.Type == ChordType.Major);
    }
}
