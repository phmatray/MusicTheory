namespace MusicTheory.UnitTests;

public class ChordSymbolParseTests
{
    [Theory]
    [InlineData("C")] [InlineData("Am")] [InlineData("D/F#")] [InlineData("Am7b5")] [InlineData("C7#9")]
    [InlineData("Gsus4")] [InlineData("Cadd9")] [InlineData("C6/9")] [InlineData("Bbmaj7")] [InlineData("F#m11")]
    [InlineData("E7alt")] [InlineData("Cdim7")] [InlineData("Caug")] [InlineData("C5")] [InlineData("G7sus4")]
    [InlineData("AbmMaj7")] [InlineData("Eb13")] [InlineData("C6/9/E")] [InlineData("C/Bb")]
    public void Parse_RoundTripsAscii(string text)
    {
        ChordSymbol.Parse(text).ToString(ChordSymbolStyle.Ascii).ShouldBe(text);
    }

    [Fact]
    public void Aliases_NormaliseToTheSameSymbol()
    {
        var a = ChordSymbol.Parse("Bbmaj7");
        ChordSymbol.Parse("B♭Δ7").ShouldBe(a);
        ChordSymbol.Parse("BbM7").ShouldBe(a);
        a.ToString().ShouldBe("Bbmaj7");
        a.ToString(ChordSymbolStyle.Unicode).ShouldBe("B♭maj7");
    }

    [Theory]
    [InlineData("Bø7", ChordType.HalfDiminished7)]
    [InlineData("Bø", ChordType.HalfDiminished7)]
    [InlineData("Bm7(b5)", ChordType.HalfDiminished7)]
    [InlineData("C°7", ChordType.Diminished7)]
    [InlineData("Co7", ChordType.Diminished7)]
    [InlineData("C°", ChordType.Diminished)]
    [InlineData("C7(b9)", ChordType.Dominant7Flat9)]
    [InlineData("C7♭9", ChordType.Dominant7Flat9)]
    [InlineData("Cmin", ChordType.Minor)]
    [InlineData("C-7", ChordType.Minor7)]
    [InlineData("Cma7", ChordType.Major7)]
    [InlineData("CΔ", ChordType.Major7)]
    [InlineData("C+", ChordType.Augmented)]
    [InlineData("Csus", ChordType.Sus4)]
    [InlineData("Cadd2", ChordType.MajorAdd9)]
    [InlineData("C69", ChordType.Major6Add9)]
    [InlineData("Cm(maj7)", ChordType.MinorMajor7)]
    [InlineData("CmM7", ChordType.MinorMajor7)]
    [InlineData("Cm(add9)", ChordType.MinorAdd9)]
    public void Parse_Aliases(string text, ChordType expected)
    {
        ChordSymbol.Parse(text).Type.ShouldBe(expected);
    }

    [Fact]
    public void Unicode_UsesJazzGlyphsAndUnicodeAccidentals()
    {
        ChordSymbol.Parse("Cdim7").ToString(ChordSymbolStyle.Unicode).ShouldBe("C°7");
        ChordSymbol.Parse("Bm7b5").ToString(ChordSymbolStyle.Unicode).ShouldBe("Bø7");
        ChordSymbol.Parse("Caug").ToString(ChordSymbolStyle.Unicode).ShouldBe("C+");
        ChordSymbol.Parse("F#7#9/C#").ToString(ChordSymbolStyle.Unicode).ShouldBe("F♯7♯9/C♯");
    }

    [Fact]
    public void NonChordToneBass_IsKept()
    {
        var s = ChordSymbol.Parse("C/Bb");
        s.Bass.ShouldNotBeNull();
        s.Bass.Name.ShouldBe(NoteName.B);
        s.Bass.Alteration.ShouldBe(Alteration.Flat);
        s.ToChord().Inversion.ShouldBe(ChordInversion.Root);
        ChordSymbol.Parse("D/F#").ToString().ShouldBe("D/F#");
    }

    [Fact]
    public void ChordToneBass_BecomesInversion()
    {
        ChordSymbol.Parse("D/F#").ToChord().Inversion.ShouldBe(ChordInversion.First);
        ChordSymbol.Parse("C/G").ToChord().Inversion.ShouldBe(ChordInversion.Second);
        ChordSymbol.Parse("G7/F").ToChord().Inversion.ShouldBe(ChordInversion.Third);
    }

    [Theory]
    [InlineData("N.C.")] [InlineData("x")] [InlineData("%")] [InlineData("")] [InlineData("   ")]
    [InlineData(null)] [InlineData("Hello")] [InlineData("[Am]")] [InlineData("Cfoo")] [InlineData("C/")]
    public void TryParse_ReturnsFalse_ForNonChords(string? text)
    {
        ChordSymbol.TryParse(text, out var s).ShouldBeFalse();
        s.ShouldBeNull();
    }

    [Fact]
    public void Parse_Throws_FormatException()
    {
        Should.Throw<FormatException>(() => ChordSymbol.Parse("N.C."));
    }

    [Theory]
    [InlineData("B♭")] [InlineData("Bb")] [InlineData("bb")]
    public void NoteParse_AcceptsUnicodeAndLowerCase(string text)
    {
        var n = Note.Parse(text);
        n.Name.ShouldBe(NoteName.B);
        n.Alteration.ShouldBe(Alteration.Flat);
    }

    [Fact]
    public void NoteParse_UnicodeDoubleAccidentalsAndOctave()
    {
        Note.Parse("C𝄪5").ShouldSatisfyAllConditions(n => n.Alteration.ShouldBe(Alteration.DoubleSharp), n => n.Octave.ShouldBe(5));
        Note.Parse("E𝄫").Alteration.ShouldBe(Alteration.DoubleFlat);
        Note.TryParse("Q", out _).ShouldBeFalse();
        Note.TryParse("F#3", out var f).ShouldBeTrue();
        f!.Octave.ShouldBe(3);
    }

    [Fact]
    public void SlashChordNotation_KeepsAccidentals()
    {
        new Chord(new Note(NoteName.F, Alteration.Sharp), ChordType.Major).WithInversion(ChordInversion.First)
            .GetSlashChordNotation().ShouldBe("F#/A#");
    }

    [Fact]
    public void EveryChordType_RoundTripsInBothStyles()
    {
        foreach (var type in Enum.GetValues<ChordType>())
        {
            var s = new ChordSymbol(new Note(NoteName.E, Alteration.Flat), type);
            ChordSymbol.Parse(s.ToString(ChordSymbolStyle.Ascii)).ShouldBe(s);
            ChordSymbol.Parse(s.ToString(ChordSymbolStyle.Unicode)).ShouldBe(s);
        }
    }
}
