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
