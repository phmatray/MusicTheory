namespace MusicTheory.UnitTests;

public class HarmonicAnalyzerTests
{
    private static string Line(string key, params string[] chords) =>
        string.Join(", ", HarmonicAnalyzer.Analyze(chords.Select(ChordSymbol.Parse), KeySignature.Parse(key)));

    private static RomanNumeral One(string key, string chord) =>
        HarmonicAnalyzer.Analyze(ChordSymbol.Parse(chord), KeySignature.Parse(key));

    [Theory]
    [InlineData("C", "I, V, vi, IV", "C", "G", "Am", "F")]
    [InlineData("C", "ii7, V7, Imaj7", "Dm7", "G7", "Cmaj7")]
    [InlineData("G", "I, V, vi, IV", "G", "D", "Em", "C")]
    [InlineData("G", "V6", "D/F#")]
    [InlineData("G", "I64", "G/D")]
    [InlineData("C", "V65, V43, V42", "G7/B", "G7/D", "G7/F")]
    [InlineData("Am", "i, iv, V7, i", "Am", "Dm", "E7", "Am")]
    [InlineData("C", "vii°, viiø7", "Bdim", "Bm7b5")]
    public void Analyze_Line(string key, string expected, params string[] chords)
    {
        Line(key, chords).ShouldBe(expected);
    }

    [Theory]
    [InlineData("Fm", "iv", 4, Alteration.Natural)]
    [InlineData("Bb", "bVII", 7, Alteration.Flat)]
    [InlineData("Ab", "bVI", 6, Alteration.Flat)]
    [InlineData("Eb", "bIII", 3, Alteration.Flat)]
    [InlineData("Cm", "i", 1, Alteration.Natural)]
    public void Analyze_BorrowedInCMajor(string chord, string expected, int degree, Alteration alteration)
    {
        var rn = One("C", chord);
        rn.ToString().ShouldBe(expected);
        rn.Function.ShouldBe(HarmonicFunction.Borrowed);
        rn.Degree.ShouldBe(degree);
        rn.DegreeAlteration.ShouldBe(alteration);
    }

    [Theory]
    [InlineData("D7", "V7/V", 5)]
    [InlineData("E7", "V7/vi", 6)]
    [InlineData("A", "V/ii", 2)]
    public void Analyze_SecondaryDominantInCMajor(string chord, string expected, int target)
    {
        var rn = One("C", chord);
        rn.ToString().ShouldBe(expected);
        rn.Function.ShouldBe(HarmonicFunction.SecondaryDominant);
        rn.TargetDegree.ShouldBe(target);
    }

    [Fact]
    public void Analyze_PicardyThirdInMinor_IsBorrowed()
    {
        One("Am", "A").ShouldSatisfyAllConditions(r => r.ToString().ShouldBe("I"), r => r.Function.ShouldBe(HarmonicFunction.Borrowed));
        One("Am", "G#dim").ToString().ShouldBe("vii°");
    }

    [Fact]
    public void Analyze_IsByPitchClass()
    {
        One("Eb", "Gm").ToString().ShouldBe("iii");
        One("Eb", "F##m").ToString().ShouldBe("iii");
    }

    [Theory]
    [InlineData("C", "Db", "bII")]
    [InlineData("C", "F#m", "#iv")]
    [InlineData("Am", "F#", "#VI")]
    public void Analyze_Chromatic_UsesKeySpelling(string key, string chord, string expected)
    {
        var rn = One(key, chord);
        rn.Function.ShouldBe(HarmonicFunction.Chromatic);
        rn.ToString().ShouldBe(expected);
    }
}
