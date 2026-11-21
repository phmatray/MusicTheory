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
    [InlineData("Ti")]
    [InlineData("Do dièse x")]
    [InlineData("")]
    [InlineData("H")]
    public void FixedDo_RejectsNonNames(string text) =>
        Note.TryParse(text, NoteNotation.FixedDo, out _).ShouldBeFalse();
}
