namespace MusicTheory.UnitTests;

/// <summary>
/// Tests for <see cref="Chord.GetSymbol()"/>.
/// Verifies that all chord types produce their standard symbol strings
/// (e.g. "Cmaj7", "Dm7", "G7", "F#°7", "Bbm(maj7)", etc.).
/// </summary>
public class ChordSymbolTests
{
    // ── Triads ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(NoteName.C, Alteration.Natural, "C")]
    [InlineData(NoteName.G, Alteration.Natural, "G")]
    [InlineData(NoteName.F, Alteration.Sharp,   "F#")]
    [InlineData(NoteName.B, Alteration.Flat,    "Bb")]
    public void MajorTriad_Symbol_IsJustRootName(NoteName name, Alteration alt, string expected)
    {
        var chord = new Chord(new Note(name, alt), ChordType.Major);
        chord.GetSymbol().ShouldBe(expected);
    }

    [Theory]
    [InlineData(NoteName.A, Alteration.Natural, "Am")]
    [InlineData(NoteName.D, Alteration.Natural, "Dm")]
    [InlineData(NoteName.C, Alteration.Sharp,   "C#m")]
    [InlineData(NoteName.E, Alteration.Flat,    "Ebm")]
    public void MinorTriad_Symbol_HasLowercaseM(NoteName name, Alteration alt, string expected)
    {
        var chord = new Chord(new Note(name, alt), ChordType.Minor);
        chord.GetSymbol().ShouldBe(expected);
    }

    [Fact]
    public void DiminishedTriad_Symbol_HasDegreeSign()
    {
        var chord = new Chord(new Note(NoteName.B), ChordType.Diminished);
        chord.GetSymbol().ShouldBe("B°");
    }

    [Fact]
    public void AugmentedTriad_Symbol_HasPlusSign()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.Augmented);
        chord.GetSymbol().ShouldBe("C+");
    }

    // ── Seventh Chords ───────────────────────────────────────────────────────

    [Fact]
    public void Major7_Symbol_IsMaj7()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.Major7);
        chord.GetSymbol().ShouldBe("Cmaj7");
    }

    [Fact]
    public void Minor7_Symbol_IsM7()
    {
        var chord = new Chord(new Note(NoteName.D), ChordType.Minor7);
        chord.GetSymbol().ShouldBe("Dm7");
    }

    [Fact]
    public void Dominant7_Symbol_IsJust7()
    {
        var chord = new Chord(new Note(NoteName.G), ChordType.Dominant7);
        chord.GetSymbol().ShouldBe("G7");
    }

    [Fact]
    public void MinorMajor7_Symbol_IsMMaj7()
    {
        var chord = new Chord(new Note(NoteName.A), ChordType.MinorMajor7);
        chord.GetSymbol().ShouldBe("Am(maj7)");
    }

    [Fact]
    public void HalfDiminished7_Symbol_IsPhiSymbol()
    {
        var chord = new Chord(new Note(NoteName.B), ChordType.HalfDiminished7);
        chord.GetSymbol().ShouldBe("Bø7");
    }

    [Fact]
    public void Diminished7_Symbol_IsDegreeSymbol7()
    {
        var chord = new Chord(new Note(NoteName.B), ChordType.Diminished7);
        chord.GetSymbol().ShouldBe("B°7");
    }

    [Fact]
    public void AugmentedMajor7_Symbol_IsPlusMaj7()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.AugmentedMajor7);
        chord.GetSymbol().ShouldBe("C+maj7");
    }

    // ── Suspended Chords ─────────────────────────────────────────────────────

    [Fact]
    public void Sus2_Symbol_IsSus2()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.Sus2);
        chord.GetSymbol().ShouldBe("Csus2");
    }

    [Fact]
    public void Sus4_Symbol_IsSus4()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.Sus4);
        chord.GetSymbol().ShouldBe("Csus4");
    }

    [Fact]
    public void Dominant7Sus4_Symbol_Is7Sus4()
    {
        var chord = new Chord(new Note(NoteName.G), ChordType.Dominant7Sus4);
        chord.GetSymbol().ShouldBe("G7sus4");
    }

    // ── Sixth Chords ─────────────────────────────────────────────────────────

    [Fact]
    public void Major6_Symbol_Is6()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.Major6);
        chord.GetSymbol().ShouldBe("C6");
    }

    [Fact]
    public void Minor6_Symbol_IsM6()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.Minor6);
        chord.GetSymbol().ShouldBe("Cm6");
    }

    [Fact]
    public void Major6Add9_Symbol_Is69()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.Major6Add9);
        chord.GetSymbol().ShouldBe("C6/9");
    }

    // ── Extended Chords ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(ChordType.Major9,    "Cmaj9")]
    [InlineData(ChordType.Minor9,    "Cm9")]
    [InlineData(ChordType.Dominant9, "C9")]
    [InlineData(ChordType.Major11,   "Cmaj11")]
    [InlineData(ChordType.Minor11,   "Cm11")]
    [InlineData(ChordType.Dominant11,"C11")]
    [InlineData(ChordType.Major13,   "Cmaj13")]
    [InlineData(ChordType.Minor13,   "Cm13")]
    [InlineData(ChordType.Dominant13,"C13")]
    public void ExtendedChord_Symbol_IsCorrect(ChordType type, string expected)
    {
        var chord = new Chord(new Note(NoteName.C), type);
        chord.GetSymbol().ShouldBe(expected);
    }

    // ── Add Chords ────────────────────────────────────────────────────────────

    [Fact]
    public void MajorAdd9_Symbol_IsAdd9()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.MajorAdd9);
        chord.GetSymbol().ShouldBe("Cadd9");
    }

    [Fact]
    public void MinorAdd9_Symbol_IsMAdd9()
    {
        var chord = new Chord(new Note(NoteName.C), ChordType.MinorAdd9);
        chord.GetSymbol().ShouldBe("Cm(add9)");
    }

    // ── Altered Chords ────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ChordType.Dominant7Flat5,       "G7♭5")]
    [InlineData(ChordType.Dominant7Sharp5,      "G7♯5")]
    [InlineData(ChordType.Dominant7Flat9,       "G7♭9")]
    [InlineData(ChordType.Dominant7Sharp9,      "G7♯9")]
    [InlineData(ChordType.Dominant7Alt,         "G7alt")]
    public void AlteredChord_Symbol_IsCorrect(ChordType type, string expected)
    {
        var chord = new Chord(new Note(NoteName.G), type);
        chord.GetSymbol().ShouldBe(expected);
    }

    // ── Power Chords ──────────────────────────────────────────────────────────

    [Fact]
    public void Power5_Symbol_Is5()
    {
        var chord = new Chord(new Note(NoteName.E), ChordType.Power5);
        chord.GetSymbol().ShouldBe("E5");
    }

    // ── Alteration Formatting ─────────────────────────────────────────────────

    [Fact]
    public void SharpRoot_Symbol_UsesHashSign()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.Sharp), ChordType.Major);
        chord.GetSymbol().ShouldContain("#");
    }

    [Fact]
    public void FlatRoot_Symbol_UsesBLowercase()
    {
        var chord = new Chord(new Note(NoteName.B, Alteration.Flat), ChordType.Minor);
        chord.GetSymbol().ShouldStartWith("Bb");
    }

    [Fact]
    public void DoubleSharpRoot_Symbol_UsesDoubleHash()
    {
        var chord = new Chord(new Note(NoteName.C, Alteration.DoubleSharp), ChordType.Major);
        chord.GetSymbol().ShouldContain("##");
    }

    [Fact]
    public void DoubleFlatRoot_Symbol_UsesDoubleBb()
    {
        var chord = new Chord(new Note(NoteName.B, Alteration.DoubleFlat), ChordType.Major);
        chord.GetSymbol().ShouldContain("bb");
    }

    // ── Real-World Symbols ─────────────────────────────────────────────────────

    [Theory]
    [InlineData(NoteName.F, Alteration.Sharp, ChordType.Minor7,   "F#m7")]
    [InlineData(NoteName.E, Alteration.Flat,  ChordType.Major7,   "Ebmaj7")]
    [InlineData(NoteName.A, Alteration.Flat,  ChordType.Dominant7,"Ab7")]
    [InlineData(NoteName.D, Alteration.Natural, ChordType.Major,  "D")]
    [InlineData(NoteName.G, Alteration.Natural, ChordType.Minor,  "Gm")]
    public void CommonChordSymbols_AreCorrect(NoteName name, Alteration alt, ChordType type, string expected)
    {
        var chord = new Chord(new Note(name, alt), type);
        chord.GetSymbol().ShouldBe(expected);
    }
}
