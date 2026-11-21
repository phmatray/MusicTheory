namespace MusicTheory.UnitTests;

/// <summary>Version 2 behaviour of chord identification, diatonic harmony and Roman-numeral analysis.</summary>
public class HarmonyValueTests
{
    private static Note[] Notes(string text) => text.Split(' ').Select(Note.Parse).ToArray();

    [Theory]
    [InlineData("C E G Bb D F A", "C13")]
    [InlineData("C E G B D F", "Cmaj11")]
    [InlineData("C Eb G Bb D", "Cm9")]
    [InlineData("C E G Bb", "C7")]
    [InlineData("C Eb Gb Bbb", "Cdim7")]
    [InlineData("C E G", "C")]
    [InlineData("C D G", "Csus2")]
    [InlineData("C G", "C5")]
    [InlineData("Bb D F", "Bb")]
    [InlineData("Db F Ab", "Db")]
    public void BestMatch_PrefersRicherChords_AndKeepsTheInputSpelling(string notes, string expected) =>
        ChordFinder.BestMatch(Notes(notes))!.Symbol.ShouldBe(expected);

    [Fact]
    public void Identify_EdgeCases()
    {
        ChordFinder.Identify(Notes("C C5")).ShouldBeEmpty();
        ChordFinder.BestMatch(Notes("C C#")).ShouldBeNull();
        Should.Throw<ArgumentNullException>(() => ChordFinder.Identify(null!).ToList());
        Should.Throw<ArgumentNullException>(() => ChordFinder.Identify([new Note(NoteName.C), null!]).ToList());
        Should.Throw<ArgumentNullException>(() => ChordFinder.IdentifyFromPitchClasses(null!).ToList());
        ChordFinder.IdentifyFromPitchClasses([-12, 4, 19]).ShouldContain(m => m.Symbol == "C" && m.ExtraNotes == 0);
        ChordFinder.IdentifyFromPitchClasses([10, 2, 5]).ShouldContain(m => m.Symbol == "Bb");
    }

    [Fact]
    public void ChordMatch_DescribesItself()
    {
        var match = ChordFinder.BestMatch(Notes("C E G D"))!;
        match.ToString().ShouldBe("Cadd9 (extra: 0)");
        match.ToChord().ShouldBe(new Chord(new Note(NoteName.C), ChordType.MajorAdd9));
    }

    [Theory]
    [InlineData("C", KeyMode.Major, "Cmaj7 Dm7 Em7 Fmaj7 G7 Am7 Bm7b5")]
    [InlineData("A", KeyMode.Minor, "Am7 Bm7b5 Cmaj7 Dm7 Em7 Fmaj7 G7")]
    [InlineData("F#", KeyMode.Major, "F#maj7 G#m7 A#m7 Bmaj7 C#7 D#m7 E#m7b5")]
    public void GetDiatonicSeventhChords_StacksTheScale(string tonic, KeyMode mode, string expected) =>
        string.Join(" ", new ChordProgression(new KeySignature(Note.Parse(tonic), mode)).GetDiatonicSeventhChords()).ShouldBe(expected);

    [Fact]
    public void ParseProgression_SeventhsAndMajorSevenths()
    {
        var c = new ChordProgression(KeySignature.Parse("C"));
        string.Join(" ", c.ParseProgression("ii7 - V7 - Imaj7 - vii°7")).ShouldBe("Dm7 G7 Cmaj7 Bm7b5");
        var am = new ChordProgression(KeySignature.Parse("Am"));
        string.Join(" ", am.ParseProgression("iMaj7, iv, VII7")).ShouldBe("AmMaj7 Dm G7");
        Should.Throw<FormatException>(() => c.ParseProgression("viimaj7"))
            .Message.ShouldBe("'viimaj7': a major seventh needs a major or minor triad, and VII is diminished in C.");
        Should.Throw<ArgumentNullException>(() => c.ParseProgression(null!));
        Should.Throw<ArgumentNullException>(() => new ChordProgression(null!));
        Should.Throw<ArgumentOutOfRangeException>(() => c.GetChordByDegree(0));
        Should.Throw<ArgumentOutOfRangeException>(() => c.GetChordByDegree(8));
        c.GetSecondaryDominant(5).GetSymbol().ShouldBe("D");
    }

    [Theory]
    [InlineData("Am", "G#dim7", "vii°7")]
    [InlineData("Am", "AmMaj7", "imaj7")]
    [InlineData("Am", "B7", "V7/V")]
    [InlineData("Am", "D7", "IV7")]
    [InlineData("C", "C6/9/E", "I6/9/3")]
    [InlineData("C", "Gsus4", "Vsus4")]
    [InlineData("C", "Fadd9/A", "IVadd9/3")]
    [InlineData("C", "Caug/G#", "I+64")]
    public void Analyze_CoversEveryRenderingPath(string key, string chord, string expected) =>
        HarmonicAnalyzer.Analyze(ChordSymbol.Parse(chord), KeySignature.Parse(key)).ToString().ShouldBe(expected);
}

public class HarmonyEdgeTests
{
    [Theory]
    [InlineData("C", "C+7", "I+7")]
    [InlineData("C", "C+maj7", "I+maj7")]
    [InlineData("C", "C6/G", "I6/5")]
    [InlineData("C", "C6/A", "I6/6")]
    [InlineData("C", "Cadd9/D", "Iadd9/9")]
    [InlineData("C", "G7/F", "V42")]
    [InlineData("C", "C/C", "I")]
    public void Analyze_NamesTheBassByItsInterval(string key, string chord, string expected) =>
        HarmonicAnalyzer.Analyze(ChordSymbol.Parse(chord), KeySignature.Parse(key)).ToString().ShouldBe(expected);

    [Fact]
    public void RomanNumeral_SecondaryDominantWithoutTarget_RendersAsAPlainNumeral() =>
        new RomanNumeral(5, Alteration.Natural, ChordType.Dominant7, null, HarmonicFunction.SecondaryDominant, null).ToString().ShouldBe("V7");

    [Fact]
    public void KeyDetector_SusAndPowerChordsFitAnyQuality()
    {
        var best = KeyDetector.Infer([ChordSymbol.Parse("Gsus4"), ChordSymbol.Parse("C5"), ChordSymbol.Parse("Fsus2"), ChordSymbol.Parse("C")])[0];
        best.Key.ShouldBe(KeySignature.Parse("C"));
        best.DiatonicHits.ShouldBe(4);
    }

    [Theory]
    [InlineData("Co", ChordType.Diminished)]
    [InlineData("Co7", ChordType.Diminished7)]
    [InlineData("CM", ChordType.Major)]
    [InlineData("CM7", ChordType.Major7)]
    public void ChordSymbol_ParsesShortAliases(string text, ChordType type) =>
        ChordSymbol.Parse(text).Type.ShouldBe(type);

    [Fact]
    public void ChordSymbol_EqualityAndTransposeCoverTheBass()
    {
        var withBass = ChordSymbol.Parse("C/E");
        withBass.ShouldNotBe(ChordSymbol.Parse("C"));
        ChordSymbol.Parse("C").ShouldNotBe(withBass);
        withBass.ShouldNotBe(ChordSymbol.Parse("C/Eb"));
        withBass.ShouldNotBe(ChordSymbol.Parse("C#/E"));
        withBass.ShouldNotBe(ChordSymbol.Parse("D/E"));
        withBass.ShouldNotBe(ChordSymbol.Parse("Cm/E"));
        withBass.ShouldBe(new ChordSymbol(Note.Parse("C2"), ChordType.Major, Note.Parse("E5")));
        withBass.GetHashCode().ShouldBe(new ChordSymbol(Note.Parse("C2"), ChordType.Major, Note.Parse("E5")).GetHashCode());
        withBass.Equals(null).ShouldBeFalse();
        withBass.Transpose(Interval.Parse("P4")).ToString().ShouldBe("F/A");
        ChordSymbol.Parse("C").Transpose(Interval.Parse("P4")).ToString().ShouldBe("F");
    }
}

public class TimeSignatureEdgeTests
{
    [Theory]
    [InlineData(2, 4)] [InlineData(6, 4)] [InlineData(9, 16)] [InlineData(12, 16)] [InlineData(3, 2)] [InlineData(4, 8)]
    public void TimeSignature_SameTopDifferentBottom_HasNoCommonName(int top, int bottom)
    {
        var t = new TimeSignature(top, bottom);
        t.GetCommonName().ShouldBeNull();
        t.GetSymbol().ShouldBe($"{top}/{bottom}");
    }

    [Fact]
    public void Duration_EqualityComparesEveryField()
    {
        new Duration(DurationType.Quarter, 1).ShouldNotBe(new Duration(DurationType.Quarter));
        Duration.CreateTuplet(DurationType.Eighth, 3, 2).ShouldNotBe(Duration.CreateTuplet(DurationType.Eighth, 5, 4));
        Duration.CreateTuplet(DurationType.Eighth, 3, 2).ShouldNotBe(Duration.CreateTuplet(DurationType.Quarter, 3, 2));
        new Duration(DurationType.Quarter).Equals((Duration?)null).ShouldBeFalse();
    }
}
