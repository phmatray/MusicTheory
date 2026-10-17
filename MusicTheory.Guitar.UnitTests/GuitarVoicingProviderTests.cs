namespace MusicTheory.Guitar.UnitTests;

public class GuitarVoicingProviderTests
{
    private static readonly GuitarVoicingProvider Provider = new();

    private static IReadOnlyList<GuitarVoicing> Voicings(string chord, VoicingOptions? options = null) =>
        Provider.GetVoicings(ChordSymbol.Parse(chord), options);

    [Theory]
    [InlineData("C", "x32010")]
    [InlineData("Am", "x02210")]
    [InlineData("F", "133211")]
    [InlineData("Bb", "x13331")]
    [InlineData("G", "320003")]
    [InlineData("E", "022100")]
    [InlineData("D", "xx0232")]
    public void FirstVoicing_IsTheCommonShape(string chord, string tab)
    {
        Voicings(chord)[0].ToTab().ShouldBe(tab);
    }

    [Fact]
    public void F_IsAFullBarre()
    {
        var f = Voicings("F")[0];
        f.Barre.ShouldBe(new Barre(1, 0, 5));
        f.Fingers.ShouldBe([1, 3, 4, 2, 1, 1]);
        f.BaseFret.ShouldBe(1);
    }

    [Fact]
    public void Bb_IsTheAFormBarre_BeforeTheSixthFretEForm()
    {
        var list = Voicings("Bb");
        var aForm = list[0];
        aForm.Source.ShouldBe(VoicingSource.MovableShape);
        aForm.ShapeName.ShouldBe("A-form");
        aForm.BaseFret.ShouldBe(1);
        aForm.Barre.ShouldBe(new Barre(1, 1, 5));
        var eForm = list.ToList().FindIndex(v => v.ToTab() == "688766");
        if (eForm >= 0) eForm.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void FSharpMinor7_HasEFormAndAForm()
    {
        var tabs = Voicings("F#m7").Select(v => v.ToTab()).ToList();
        tabs.ShouldContain("242222");
        tabs.ShouldContain("x 9 11 9 10 9");
        tabs.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void SlashChord_PutsTheBassLowest()
    {
        var list = Voicings("D/F#");
        list.ShouldNotBeEmpty();
        list.Select(v => v.ToTab()).ShouldContain("2x0232");
        foreach (var v in list)
        {
            var lowest = v.Sound(Tuning.Standard).First(n => n is not null)!;
            MusicTheoryUtilities.GetPitchClass(lowest).ShouldBe(6);
        }
    }

    [Fact]
    public void G7_KeepsItsSymbol()
    {
        var g7 = Voicings("G7")[0];
        g7.ToTab().ShouldBe("320001");
        g7.Chord.ToString().ShouldBe("G7");
    }

    [Fact]
    public void Capo_FretsAreRelative_SoundIsTransposed()
    {
        var g = Voicings("G", new VoicingOptions(Capo: 2))[0];
        g.ToTab().ShouldBe("320003");
        g.Sound(Tuning.Standard, capo: 2).OfType<Note>().Select(MusicTheoryUtilities.GetPitchClass).ToHashSet()
            .ShouldBe(new HashSet<int> { 9, 1, 4 }, ignoreOrder: true); // A C# E
    }

    [Fact]
    public void Order_IsDeterministic()
    {
        Voicings("Ebmaj7").Select(v => v.ToTab()).ShouldBe(Voicings("Ebmaj7").Select(v => v.ToTab()));
    }

    [Fact]
    public void OtherTunings_StillProduceVoicings()
    {
        Voicings("D", new VoicingOptions(Tuning: Tuning.DADGAD)).ShouldNotBeEmpty();
        Voicings("G", new VoicingOptions(Tuning: Tuning.OpenG))[0].Source.ShouldBe(VoicingSource.Generated);
    }

    public static TheoryData<ChordType> AllTypes() => new(Enum.GetValues<ChordType>());

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void Coverage_EveryTypeAndRoot_HasAValidVoicing(ChordType type)
    {
        string[] roots = ["C", "Db", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B"];
        foreach (var r in roots)
        {
            var chord = new ChordSymbol(Note.Parse(r), type);
            var list = Provider.GetVoicings(chord);
            list.ShouldNotBeEmpty($"{chord}");
            foreach (var v in list)
                AssertValid(v, chord);
        }
    }

    private static void AssertValid(GuitarVoicing v, ChordSymbol chord)
    {
        var tab = $"{chord} {v.ToTab()}";
        v.Frets.Count.ShouldBe(6);
        v.Frets.ShouldAllBe(f => f == null || (f >= 0 && f <= 15), tab);
        var fretted = Enumerable.Range(0, 6).Where(s => v.Frets[s] > 0).ToList();
        if (fretted.Count > 0)
            (fretted.Max(s => v.Frets[s]!.Value) - fretted.Min(s => v.Frets[s]!.Value)).ShouldBeLessThan(4, tab);

        // Fingers: 1..4 exactly on fretted strings; one finger per fret except the declared barre; higher fret, higher finger.
        for (int s = 0; s < 6; s++)
            (v.Fingers[s] is not null).ShouldBe(v.Frets[s] > 0, tab);
        v.Fingers.OfType<int>().ShouldAllBe(f => f >= 1 && f <= 4, tab);
        foreach (var group in fretted.GroupBy(s => v.Fingers[s]))
            if (group.Count() > 1)
            {
                v.Barre.ShouldNotBeNull(tab);
                group.Key.ShouldBe(1, tab);
                group.ShouldAllBe(s => v.Frets[s] == v.Barre!.Fret, tab);
            }
        foreach (var a in fretted)
            foreach (var b in fretted)
                if (v.Frets[a] < v.Frets[b])
                    v.Fingers[a]!.Value.ShouldBeLessThan(v.Fingers[b]!.Value, tab);

        // Sound: chord tones only, lowest is root (or bass), 3rd / sus tone present.
        var chordPcs = new Chord(chord.Root, chord.Type).GetNotes().Select(MusicTheoryUtilities.GetPitchClass).ToHashSet();
        var sound = v.Sound(Tuning.Standard).OfType<Note>().Select(MusicTheoryUtilities.GetPitchClass).ToList();
        sound.ShouldAllBe(pc => chordPcs.Contains(pc), tab);
        sound[0].ShouldBe(MusicTheoryUtilities.GetPitchClass(chord.Root), tab);
        var notes = new Chord(chord.Root, chord.Type).GetNotes().ToList();
        if (chord.Type != ChordType.Power5)
            sound.ShouldContain(MusicTheoryUtilities.GetPitchClass(notes[1]), tab);
    }
}
