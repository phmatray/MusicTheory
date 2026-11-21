namespace MusicTheory.UnitTests;

public class SolfegeTests
{
    [Theory]
    [InlineData("Ré dièse 4", "D#4")]
    [InlineData("Re#4", "D#4")]
    [InlineData("Sib3", "Bb3")]
    [InlineData("sol", "G4")]
    [InlineData("Do double bémol", "Cbb4")]
    [InlineData("Fa𝄪2", "F##2")]
    [InlineData("La double dièse", "A##4")]
    [InlineData("Mi bemol 3", "Eb3")]
    public void FixedDo_ParsesNames(string text, string expected) =>
        Note.Parse(text, NoteNotation.FixedDo).ShouldBe(Note.Parse(expected));

    [Theory]
    [InlineData("D#4", SolfegeStyle.Words, "Ré dièse")]
    [InlineData("Bbb", SolfegeStyle.Words, "Si double bémol")]
    [InlineData("F#", SolfegeStyle.Unicode, "Fa♯")]
    [InlineData("Eb", SolfegeStyle.Ascii, "Mib")]
    [InlineData("C", SolfegeStyle.Words, "Do")]
    [InlineData("G##", SolfegeStyle.Words, "Sol double dièse")]
    public void FixedDo_FormatsNames(string note, SolfegeStyle style, string expected) =>
        Solfege.FixedDo(Note.Parse(note), style).ShouldBe(expected);

    [Fact]
    public void FixedDo_RoundTripsEverySpelling()
    {
        foreach (var name in Enum.GetValues<NoteName>())
            foreach (var alt in Enum.GetValues<Alteration>())
                foreach (var style in Enum.GetValues<SolfegeStyle>())
                {
                    var n = new Note(name, alt);
                    Note.Parse(Solfege.FixedDo(n, style), NoteNotation.FixedDo).ShouldBe(n);
                }
    }

    [Theory]
    [InlineData("G", "G", "do")]
    [InlineData("G", "F#", "ti")]
    [InlineData("G", "F", "te")]
    [InlineData("G", "C#", "fi")]
    [InlineData("Eb", "Gb", "me")]
    [InlineData("C", "E#", null)]
    [InlineData("C", "E##", null)]
    [InlineData("C", "C#", "di")]
    [InlineData("C", "Db", "ra")]
    public void MovableDo_NamesDegrees(string key, string note, string? expected) =>
        Solfege.MovableDo(Note.Parse(note), KeySignature.Parse(key)).ShouldBe(expected);

    [Fact]
    public void MovableDo_Minor()
    {
        var a = KeySignature.Parse("Am");
        Solfege.MovableDo(Note.Parse("A"), a).ShouldBe("la");
        Solfege.MovableDo(Note.Parse("A"), a, MinorSolfege.DoBased).ShouldBe("do");
        Solfege.MovableDo(Note.Parse("G#"), a).ShouldBe("si");
    }

    [Fact]
    public void MovableDo_RejectsNulls()
    {
        Should.Throw<ArgumentNullException>(() => Solfege.MovableDo(null!, KeySignature.Parse("C")));
        Should.Throw<ArgumentNullException>(() => Solfege.MovableDo(Note.Parse("C"), null!));
    }

    [Theory]
    [InlineData("fi", "C#4")]
    [InlineData("FI", "C#4")]
    [InlineData(" te ", "F4")]
    [InlineData("do", "G4")]
    public void TryParseMovableDo_ReadsSyllables(string text, string expected)
    {
        Solfege.TryParseMovableDo(text, KeySignature.Parse("G"), out var n).ShouldBeTrue();
        n.ShouldBe(Note.Parse(expected));
    }

    [Fact]
    public void TryParseMovableDo_Minor()
    {
        Solfege.TryParseMovableDo("la", KeySignature.Parse("Am"), out var n).ShouldBeTrue();
        n.ShouldBe(Note.Parse("A4"));
    }

    [Theory]
    [InlineData("xx")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParseMovableDo_RejectsUnknown(string? text) =>
        Solfege.TryParseMovableDo(text, KeySignature.Parse("G"), out _).ShouldBeFalse();

    [Fact]
    public void TryParseMovableDo_RejectsTripleAccidentals() =>
        Solfege.TryParseMovableDo("di", new KeySignature(Note.Parse("C##"), KeyMode.Major), out _).ShouldBeFalse();

    [Fact]
    public void FixedDo_RejectsNullNote() =>
        Should.Throw<ArgumentNullException>(() => Solfege.FixedDo(null!));

    [Theory]
    [InlineData("Ti")]
    [InlineData("Do dièse x")]
    [InlineData("")]
    [InlineData("H")]
    public void FixedDo_RejectsNonNames(string text) =>
        Note.TryParse(text, NoteNotation.FixedDo, out _).ShouldBeFalse();
}
