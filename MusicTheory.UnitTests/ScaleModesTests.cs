namespace MusicTheory.UnitTests;

public class ScaleModesTests
{
    private static Scale S(string root, ScaleType type) => new(Note.Parse(root), type);

    private static string Render(Scale scale) => string.Join(", ", scale.GetModes().Select(m => m.ToString()));

    [Fact]
    public void Major_ListsChurchModes() =>
        Render(S("C4", ScaleType.Major)).ShouldBe(
            "C4 Major, D4 Dorian, E4 Phrygian, F4 Lydian, G4 Mixolydian, A4 Aeolian, B4 Locrian");

    [Fact]
    public void MelodicMinor_ListsItsModes() =>
        Render(S("C4", ScaleType.MelodicMinor)).ShouldBe(
            "C4 MelodicMinor, D4 DorianFlat2, Eb4 LydianAugmented, F4 LydianDominant, G4 MixolydianFlat6, A4 LocrianNatural2, B4 Altered");

    [Fact]
    public void Aliases_ResolveToTheirFamily()
    {
        S("A4", ScaleType.NaturalMinor).GetModes()[2].ToString().ShouldBe("C5 Ionian");
        S("D4", ScaleType.Dorian).GetModes()[6].ToString().ShouldBe("C5 Ionian");
    }

    public static TheoryData<ScaleType, string> FamilyMembers()
    {
        var data = new TheoryData<ScaleType, string>();
        ScaleType[] types =
        [
            ScaleType.Ionian, ScaleType.Dorian, ScaleType.Phrygian, ScaleType.Lydian, ScaleType.Mixolydian, ScaleType.Aeolian, ScaleType.Locrian,
            ScaleType.MelodicMinor, ScaleType.DorianFlat2, ScaleType.LydianAugmented, ScaleType.LydianDominant, ScaleType.MixolydianFlat6, ScaleType.LocrianNatural2, ScaleType.Altered,
            ScaleType.HarmonicMinor, ScaleType.LocrianNatural6, ScaleType.IonianAugmented, ScaleType.DorianSharp4, ScaleType.PhrygianDominant, ScaleType.LydianSharp2, ScaleType.Ultralocrian,
        ];
        foreach (var type in types)
            foreach (var tonic in new[] { "C4", "D4", "E4", "F4", "G4", "A4", "B4" })
                data.Add(type, tonic);
        return data;
    }

    [Theory]
    [MemberData(nameof(FamilyMembers))]
    public void Modes_ShareThePitchClassSetOfTheParent(ScaleType type, string tonic)
    {
        var parent = S(tonic, type);
        var expected = parent.GetNotes().Select(n => n.PitchClass).ToHashSet();
        foreach (var mode in parent.GetModes())
            expected.SetEquals(mode.GetNotes().Select(n => n.PitchClass)).ShouldBeTrue();
    }

    [Theory]
    [InlineData(ScaleType.Blues)]
    [InlineData(ScaleType.Chromatic)]
    [InlineData(ScaleType.WholeTone)]
    [InlineData(ScaleType.PentatonicMajor)]
    [InlineData(ScaleType.BebopDominant)]
    [InlineData(ScaleType.HungarianMinor)]
    public void NonFamilyTypes_Throw(ScaleType type) =>
        Should.Throw<InvalidOperationException>(() => S("C4", type).GetModes())
            .Message.ShouldContain(type.ToString());
}
