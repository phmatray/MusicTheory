namespace MusicTheory.UnitTests;

public class ModeCatalogueTests
{
    public static IEnumerable<TheoryDataRow<string, string, string>> Music21Rows() =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", "modes.csv")).Skip(1)
            .Select(l => l.Split(','))
            .Where(r => !r[2].Contains('!'))
            .Select(r => new TheoryDataRow<string, string, string>(r[0], r[1], r[2]));

    private static string Spell(string tonic, ScaleType type) =>
        string.Join(" ", new Scale(Note.Parse(tonic), type).GetNotes());

    [Theory]
    [MemberData(nameof(Music21Rows))]
    public void Modes_MatchMusic21(string tonic, string type, string notes) =>
        Spell(tonic, Enum.Parse<ScaleType>(type)).ShouldBe(notes);

    [Theory]
    [InlineData("C4", ScaleType.HungarianMinor, "C4 D4 Eb4 F#4 G4 Ab4 B4 C5")]
    [InlineData("C4", ScaleType.Hirajoshi, "C4 D4 Eb4 G4 Ab4 C5")]
    [InlineData("C4", ScaleType.DoubleHarmonicMajor, "C4 Db4 E4 F4 G4 Ab4 B4 C5")]
    [InlineData("D4", ScaleType.InSen, "D4 Eb4 G4 A4 C5 D5")]
    [InlineData("C4", ScaleType.Iwato, "C4 Db4 F4 Gb4 Bb4 C5")]
    [InlineData("C4", ScaleType.NeapolitanMinor, "C4 Db4 Eb4 F4 G4 Ab4 B4 C5")]
    [InlineData("C4", ScaleType.NeapolitanMajor, "C4 Db4 Eb4 F4 G4 A4 B4 C5")]
    [InlineData("C4", ScaleType.BebopDominant, "C4 D4 E4 F4 G4 A4 Bb4 B4 C5")]
    [InlineData("C4", ScaleType.BebopMajor, "C4 D4 E4 F4 G4 G#4 A4 B4 C5")]
    [InlineData("D4", ScaleType.BebopDorian, "D4 E4 F4 F#4 G4 A4 B4 C5 D5")]
    [InlineData("G4", ScaleType.Altered, "G4 Ab4 Bb4 Cb5 Db5 Eb5 F5 G5")]
    public void WorldAndBebopScales_SpellExactly(string tonic, ScaleType type, string notes) =>
        Spell(tonic, type).ShouldBe(notes);

    [Fact]
    public void BebopDominant_ResolvesBothNotesOnOneLetter()
    {
        var scale = new Scale(Note.Parse("C4"), ScaleType.BebopDominant);
        scale.GetDegree(Note.Parse("Bb4")).ShouldBe(7);
        scale.GetDegree(Note.Parse("B4")).ShouldBe(8);
    }

    [Fact]
    public void ExistingScaleTypes_KeepTheirNumericValues() =>
        ((int)ScaleType.PentatonicMinor).ShouldBe(15);
}
