namespace MusicTheory.UnitTests;

/// <summary>Tests for <see cref="ChordScale"/>.</summary>
public class ChordScaleTests
{
    private static List<ScaleType> Types(string symbol) =>
        ChordScale.ScalesFor(ChordSymbol.Parse(symbol)).Select(s => s.Type).ToList();

    [Fact]
    public void G7_ContainsDominantScalesButNotAltered()
    {
        var types = Types("G7");
        types.ShouldContain(ScaleType.Mixolydian);
        types.ShouldContain(ScaleType.BebopDominant);
        types.ShouldContain(ScaleType.LydianDominant);
        types.ShouldNotContain(ScaleType.Altered);
    }

    [Fact]
    public void G7Alt_ContainsAltered() => Types("G7alt").ShouldContain(ScaleType.Altered);

    [Fact]
    public void C7b9_ContainsPhrygianDominantNotMixolydian()
    {
        var types = Types("C7b9");
        types.ShouldContain(ScaleType.PhrygianDominant);
        types.ShouldNotContain(ScaleType.Mixolydian);
    }

    [Fact]
    public void Null_Throws() => Should.Throw<ArgumentNullException>(() => ChordScale.ScalesFor(null!));

    [Fact]
    public void SlashBass_IsIgnored() => Types("G7/B").ShouldBe(Types("G7"));

    [Fact]
    public void UnspellableScales_AreSkipped() => Types("B#").ShouldNotBeEmpty();

    [Fact]
    public void Cmaj7_LydianBeforeMajor()
    {
        var types = Types("Cmaj7");
        types.IndexOf(ScaleType.Lydian).ShouldBeGreaterThanOrEqualTo(0);
        types.IndexOf(ScaleType.Lydian).ShouldBeLessThan(types.IndexOf(ScaleType.Major));
    }

    [Fact]
    public void G7_FirstIsLydianDominant() => Types("G7")[0].ShouldBe(ScaleType.LydianDominant);

    [Fact]
    public void Dm7_DorianBeforeAeolian()
    {
        var types = Types("Dm7");
        types.ShouldContain(ScaleType.Phrygian);
        types.ShouldContain(ScaleType.BebopDorian);
        types.IndexOf(ScaleType.Dorian).ShouldBeGreaterThanOrEqualTo(0);
        types.IndexOf(ScaleType.Dorian).ShouldBeLessThan(types.IndexOf(ScaleType.Aeolian));
    }

    [Theory]
    [MemberData(nameof(AllChordTypes))]
    public void EveryResult_ContainsEveryChordTone_AndIsNotChromatic(ChordType type)
    {
        var chord = new ChordSymbol(new Note(NoteName.C, Alteration.Natural, 4), type);
        var tones = chord.ToChord().GetNotes().Select(n => n.PitchClass).ToHashSet();
        foreach (var scale in ChordScale.ScalesFor(chord))
        {
            scale.Type.ShouldNotBe(ScaleType.Chromatic);
            tones.IsSubsetOf(scale.GetNotes().Select(n => n.PitchClass)).ShouldBeTrue();
        }
    }

    public static TheoryData<ChordType> AllChordTypes() => new(Enum.GetValues<ChordType>());
}
