namespace MusicTheory.UnitTests;

/// <summary>Tensions on lead-sheet symbols: <c>C7b9#11</c>, <c>Cmaj7(#11)</c>, <c>C7(b9,#11)</c>.</summary>
public class ChordSymbolTensionTests
{
    [Theory]
    [InlineData("C7b9#11", ChordType.Dominant7Flat9, "#11")]
    [InlineData("C7(b9,#11)", ChordType.Dominant7Flat9, "#11")]
    [InlineData("Cmaj7#11", ChordType.Major7, "#11")]
    [InlineData("Cmaj7(#11)", ChordType.Major7, "#11")]
    [InlineData("C13#11", ChordType.Dominant13, "#11")]
    [InlineData("C7#9b13", ChordType.Dominant7Sharp9, "b13")]
    [InlineData("Cm7b9", ChordType.Minor7, "b9")]
    [InlineData("C7(13)", ChordType.Dominant7, "13")]
    [InlineData("C7add13", ChordType.Dominant7, "13")]
    [InlineData("Cadd13", ChordType.Major, "13")]
    [InlineData("C9#11", ChordType.Dominant9, "#11")]
    [InlineData("Cm#5", ChordType.Minor, "#5")]
    [InlineData("C7(9)", ChordType.Dominant7, "9")]
    [InlineData("C7(11)", ChordType.Dominant7, "11")]
    [InlineData("Cmaj7b5", ChordType.Major7, "b5")]
    public void Parse_ReadsTheLongestTypeThenTensions(string text, ChordType type, string tension)
    {
        var symbol = ChordSymbol.Parse(text);
        symbol.Type.ShouldBe(type);
        symbol.Tensions.Count.ShouldBe(1);
        symbol.Tensions[0].ShouldBe(Tension(tension));
    }

    [Fact]
    public void Parse_TwoTensions()
    {
        var symbol = ChordSymbol.Parse("C13(b9,#11)");
        symbol.Type.ShouldBe(ChordType.Dominant13);
        symbol.Tensions.ShouldBe([Tension("b9"), Tension("#11")]);
    }

    [Theory]
    [InlineData("C75")]
    [InlineData("C7add#11")]
    [InlineData("C7b11")]
    [InlineData("C7#13")]
    [InlineData("C7xyz")]
    [InlineData("C7add")]
    public void TryParse_RejectsWhatIsNotATension(string text) =>
        ChordSymbol.TryParse(text, out _).ShouldBeFalse();

    [Theory]
    [InlineData("C7b9#11", "C7b9#11")]
    [InlineData("C7(b9,#11)", "C7b9#11")]
    [InlineData("Cmaj7(#11)", "Cmaj7#11")]
    [InlineData("C7(13)", "C7add13")]
    [InlineData("Ebm9/Bb", "Ebm9/Bb")]
    [InlineData("F#7#9b13/A#", "F#7#9b13/A#")]
    public void ToString_IsCanonical_AndRoundTrips(string text, string canonical)
    {
        var symbol = ChordSymbol.Parse(text);
        symbol.ToString().ShouldBe(canonical);
        ChordSymbol.Parse(symbol.ToString()).ShouldBe(symbol);
    }

    [Fact]
    public void ToString_Unicode_AppliesToTensions() =>
        ChordSymbol.Parse("Bb7b9#11").ToString(ChordSymbolStyle.Unicode).ShouldBe("B♭7♭9♯11");

    [Fact]
    public void ToChord_AppliesTensions()
    {
        string.Join(" ", ChordSymbol.Parse("C7b9#11").ToChord().GetNotes()).ShouldBe("C4 E4 G4 Bb4 Db5 F#5");
        string.Join(" ", ChordSymbol.Parse("C13b9").ToChord().GetNotes()).ShouldBe("C4 E4 G4 Bb4 Db5 F5 A5");
        ChordSymbol.Parse("C7b9#11/E").ToChord().Inversion.ShouldBe(ChordInversion.First);
    }

    [Fact]
    public void TransposeAndVoice_KeepTensions()
    {
        ChordSymbol.Parse("C7b9#11").Transpose(Interval.Parse("M2")).ToString().ShouldBe("D7b9#11");
        ChordSymbol.Parse("C7#11").Transpose(2, KeySignature.Parse("D")).ToString().ShouldBe("D7#11");
        string.Join(" ", ChordSymbol.Parse("C7#11").Voice(3)).ShouldBe("C3 E3 G3 Bb3 F#4");
    }

    [Fact]
    public void Equality_IncludesTensions()
    {
        ChordSymbol.Parse("C7b9#11").ShouldBe(ChordSymbol.Parse("C7(b9,#11)"));
        ChordSymbol.Parse("C7b9#11").GetHashCode().ShouldBe(ChordSymbol.Parse("C7(b9,#11)").GetHashCode());
        ChordSymbol.Parse("C7#11").ShouldNotBe(ChordSymbol.Parse("C7"));
        ChordSymbol.Parse("C7#11").ShouldNotBe(ChordSymbol.Parse("C7b13"));
    }

    [Fact]
    public void Construction_ValidatesEveryField()
    {
        var c = new Note(NoteName.C);
        Should.Throw<ArgumentNullException>(() => new ChordSymbol(null!, ChordType.Major));
        Should.Throw<ArgumentOutOfRangeException>(() => new ChordSymbol(c, (ChordType)99));
        Should.Throw<ArgumentException>(() => new ChordSymbol(c, ChordType.Major, null, [Interval.Parse("M3")]))
            .Message.ShouldStartWith("Not a chord tension: M3. Use b5, #5, b9, 9, #9, 11, #11, b13 or 13.");
        Should.Throw<ArgumentException>(() => new ChordSymbol(c, ChordType.Major, null, [null!]))
            .Message.ShouldStartWith("Not a chord tension: null.");
        var symbol = new ChordSymbol(c, ChordType.Dominant7, null, [Tension("b9")]);
        (symbol with { Tensions = null! }).Tensions.ShouldBeEmpty();
        Should.Throw<ArgumentNullException>(() => symbol with { Root = null! });
        Should.Throw<ArgumentOutOfRangeException>(() => symbol with { Type = (ChordType)99 });
        Should.Throw<ArgumentException>(() => symbol with { Tensions = [Interval.Parse("P5")] });
        new ChordSymbol(c, ChordType.Major).Tensions.ShouldBeEmpty();
    }

    [Fact]
    public void ToString_RejectsANullStyle() =>
        Should.Throw<ArgumentNullException>(() => ChordSymbol.Parse("C").ToString(null!));

    private static Interval Tension(string text) => text switch
    {
        "b5" => Interval.Parse("d5"), "#5" => Interval.Parse("A5"),
        "b9" => Interval.Parse("m9"), "9" => Interval.Parse("M9"), "#9" => Interval.Parse("A9"),
        "11" => Interval.Parse("P11"), "#11" => Interval.Parse("A11"),
        "b13" => Interval.Parse("m13"), _ => Interval.Parse("M13"),
    };
}
