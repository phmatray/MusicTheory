namespace MusicTheory.Abc.UnitTests;

public class AbcParseTests
{
    private static string Bar(Measure m) =>
        string.Join(" ", m.Events.Select(e =>
            (e.IsRest ? "z" : string.Join("+", e.Pitches)) + "/" + e.Duration.Value + (e.Duration.IsTuplet ? "t" : "") + (e.TiedToNext ? "~" : "")));

    private static string[] Bars(string text) => AbcTune.Parse(text).Measures.Select(Bar).ToArray();

    private static string Tune(string body, string header = "M:4/4\nL:1/8\nK:C") => $"X:1\n{header}\n{body}";

    [Fact]
    public void SpeedThePlough()
    {
        var tune = AbcTune.Parse("""
            X:1
            T:Speed the Plough
            T:Other title
            M:4/4
            L:1/8
            Q:1/4=120
            K:G
            GABc dedB|dedB dedB|c2ec B2dB|c2A2 A2BA|
            """);
        tune.ReferenceNumber.ShouldBe(1);
        tune.Title.ShouldBe("Speed the Plough");
        tune.Key.ShouldBe(KeySignature.Parse("G"));
        tune.UnitLength.ShouldBe(Rational.Of(1, 8));
        tune.TimeSignature.ShouldBe(new TimeSignature(4, 4));
        tune.Tempo.ShouldBe(new TempoMap(120, new Duration(DurationType.Quarter)));
        tune.Measures.Count.ShouldBe(4);
        Bar(tune.Measures[0]).ShouldBe("G4/1/8 A4/1/8 B4/1/8 C5/1/8 D5/1/8 E5/1/8 D5/1/8 B4/1/8");
        Bar(tune.Measures[2]).ShouldStartWith("C5/1/4 E5/1/8");
    }

    [Fact]
    public void AccidentalsFollowKeyAndLastToEndOfBar()
    {
        Bars(Tune("|F^F=F F|F F|", "L:1/4\nK:D")).ShouldBe(["F#4/1/4 F#4/1/4 F4/1/4 F4/1/4", "F#4/1/4 F#4/1/4"]);
        // Another octave is not affected; a flat and a double sharp.
        Bars(Tune("_BB b __e ^^f", "L:1/8\nK:C"))[0].ShouldBe("Bb4/1/8 Bb4/1/8 B5/1/8 Ebb5/1/8 F##5/1/8");
    }

    [Fact]
    public void TripletsAndTies()
    {
        var bars = Bars(Tune("(3abc c2-c2|"));
        bars[0].ShouldBe("A5/1/12t B5/1/12t C5/1/12t C5/1/4~ C5/1/4");
        Bars(Tune("c2- |c2|"))[0].ShouldBe("C5/1/4~");
        AbcTune.Parse(Tune("c2-|c2|")).Measures[0].Events[0].TiedToNext.ShouldBeTrue();
    }

    [Fact]
    public void HeaderDefaults()
    {
        AbcTune.Parse("X:2\nK:C\nC").TimeSignature.ShouldBe(new TimeSignature(4, 4));
        AbcTune.Parse("X:2\nM:C|\nK:C\nC").TimeSignature.ShouldBe(new TimeSignature(2, 2));
        AbcTune.Parse("X:2\nM:C\nK:C\nC").TimeSignature.ShouldBe(new TimeSignature(4, 4));
        AbcTune.Parse("X:2\nM:2/4\nK:C\nC").UnitLength.ShouldBe(Rational.Of(1, 16));
        AbcTune.Parse("X:2\nM:2/4\nK:C\nC").TimeSignature.ShouldBe(new TimeSignature(2, 4));
        AbcTune.Parse("X:2\nM:6/8\nK:C\nC").UnitLength.ShouldBe(Rational.Of(1, 8));
        AbcTune.Parse("X:2\nK:C\nC").Title.ShouldBeNull();
        AbcTune.Parse("X:2\nC:Trad.\nK:C\nC").ReferenceNumber.ShouldBe(2);
        AbcTune.Parse("X:2\nK:C\nC").Tempo.ShouldBeNull();
        AbcTune.Parse("X:2\nQ:90\nK:C\nC").Tempo.ShouldBe(new TempoMap(90));
    }

    [Theory]
    [InlineData("G", "G", KeyMode.Major)]
    [InlineData("Gm", "G", KeyMode.Minor)]
    [InlineData("G minor", "G", KeyMode.Minor)]
    [InlineData("G Major", "G", KeyMode.Major)]
    [InlineData("none", "C", KeyMode.Major)]
    [InlineData("Edor", "D", KeyMode.Major)]
    [InlineData("Ador", "G", KeyMode.Major)]
    [InlineData("Dmix", "G", KeyMode.Major)]
    [InlineData("Alyd", "E", KeyMode.Major)]
    [InlineData("Aaeo", "C", KeyMode.Major)]
    [InlineData("Ephr", "C", KeyMode.Major)]
    [InlineData("Bloc", "C", KeyMode.Major)]
    [InlineData("Cion", "C", KeyMode.Major)]
    [InlineData("Bb", "Bb", KeyMode.Major)]
    public void Keys(string k, string tonic, KeyMode mode) =>
        AbcTune.Parse($"X:1\nK:{k}\nC").Key.ShouldBe(new KeySignature(Note.Parse(tonic), mode));

    [Fact]
    public void OctavesLengthsRestsAndChords()
    {
        Bars(Tune("C, c' C,, z|x2 z/ z//", "L:1/4\nK:C")).ShouldBe(
            ["C3/1/4 C6/1/4 C2/1/4 z/1/4", "z/1/2 z/1/8 z/1/16"]);
        Bars(Tune("[CEG]2 [C2E] [CE]3/2|c3/2 c/2 c/ c//"))
            .ShouldBe(["C4+E4+G4/1/4 C4+E4/1/4 C4+E4/3/16", "C5/3/16 C5/1/16 C5/1/16 C5/1/32"]);
    }

    [Fact]
    public void ChordSymbolsDecorationsGraceNotesCommentsAndSlursAreSkipped()
    {
        Bars("X:1\n% a comment\nL:1/4\nK:C\n\"Am\"C !trill!D {e}E| (F G) % tail\n")
            .ShouldBe(["C4/1/4 D4/1/4 E4/1/4", "F4/1/4 G4/1/4"]);
    }

    [Fact]
    public void BarlinesRepeatsAndPickup()
    {
        Bars(Tune("c d|:e2 f2|g4 :| |]", "M:4/4\nL:1/8\nK:C")).ShouldBe(
            ["C5/1/8 D5/1/8", "E5/1/4 F5/1/4", "G5/1/2"]);
        var tune = AbcTune.Parse(Tune("c d|e2 f2 g2 a2|"));
        tune.Measures[0].Capacity.ShouldBe(Rational.Of(1, 4));
        tune.Measures[1].IsComplete.ShouldBeTrue();
    }

    [Fact]
    public void OnlyTheFirstTuneIsRead()
    {
        AbcTune.Parse("%abc\n\nX:1\nK:C\nC\n\nX:2\nK:D\nD").Measures.Count.ShouldBe(1);
        AbcTune.Parse("X:1\nK:C\nC\nX:2\nK:D\nD").Key.Tonic.Name.ShouldBe(NoteName.C);
    }

    [Theory]
    [InlineData("X:1\nK:C\nV:1\nC")]
    [InlineData("X:1\nK:C\nC\nw:la")]
    [InlineData("X:1\nV:2\nK:C\nC")]
    [InlineData("X:1\nw:la\nK:C\nC")]
    public void VoicesAndLyricsAreNotSupported(string text)
    {
        Should.Throw<NotSupportedException>(() => AbcTune.Parse(text));
        AbcTune.TryParse(text, out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("T:x\nK:C\nC")]
    [InlineData("X:1\nT:x")]
    [InlineData("X:1\nx\nK:C")]
    [InlineData("X:1\nT:x\n")]
    [InlineData("X:1\nT:x\n\nC")]
    [InlineData("X:1\nnonsense\nK:C")]
    [InlineData("X:1\nM:4\nK:C")]
    [InlineData("X:1\nM:4/3\nK:C")]
    [InlineData("X:1\nL:1\nK:C")]
    [InlineData("X:1\nQ:fast\nK:C")]
    [InlineData("X:1\nQ:1/3=100\nK:C")]
    [InlineData("X:1\nK:H")]
    [InlineData("X:1\nK:Gfoo")]
    [InlineData("X:1\nK:C\n\"Am C")]
    [InlineData("X:1\nK:C\n-C")]
    [InlineData("X:1\nK:C\nz-")]
    [InlineData("X:1\nK:C\n(5abcde")]
    [InlineData("X:1\nK:C\n[CE")]
    [InlineData("X:1\nK:C\n^")]
    [InlineData("X:1\nK:C\nC5")]
    [InlineData("X:1\nK:C\n(3a3bc")]
    [InlineData("X:1\nK:C\n?")]
    [InlineData("X:1\nK:C\nC8 D")]
    [InlineData("X:1\nK:C\nC,,,,,,")]
    public void BadTunesThrowFormatException(string text)
    {
        Should.Throw<FormatException>(() => AbcTune.Parse(text));
        AbcTune.TryParse(text, out var tune).ShouldBeFalse();
        tune.ShouldBeNull();
    }

    [Fact]
    public void NotatableErrorNamesTheBar() =>
        Should.Throw<FormatException>(() => AbcTune.Parse(Tune("C2|D5")))
            .Message.ShouldStartWith("Bar 2:");

    [Fact]
    public void NullAndValid()
    {
        AbcTune.Parse("X:1\nK:C\n(").Measures.ShouldBeEmpty();
        Should.Throw<ArgumentNullException>(() => AbcTune.Parse(null!));
        AbcTune.TryParse(null, out _).ShouldBeFalse();
        AbcTune.TryParse("X:1\nK:C\nC", out var tune).ShouldBeTrue();
        tune!.Measures.Count.ShouldBe(1);
    }
}
