using System.Runtime.CompilerServices;
using MusicTheory.Guitar;
using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;
using MusicTheory.Toolbox.Tools.Fretboard;

namespace MusicTheory.Toolbox.UnitTests;

public class FretboardModelTests
{
    public static TheoryData<string> Ids() => new(TuningPresets.All.Select(p => p.Id));

    [Theory]
    [MemberData(nameof(Ids))]
    public void Open_strings_equal_the_tuning(string id)
    {
        var preset = TuningPresets.Find(id)!;
        var v = FretboardModel.Build(id, 12, "note", "C", "", Lang.En);
        v.Strings.ShouldBe(preset.Tuning.Strings.Select(n => n.GetName()).ToList());
        v.Dots.Select(d => d.String).Distinct().Order().ShouldBeSubsetOf(Enumerable.Range(0, preset.Tuning.Strings.Count));
    }

    [Fact]
    public void Scale_on_standard_matches_the_library_and_marks_roots()
    {
        var v = FretboardModel.Build("standard", 12, "scale", "C", "Major", Lang.En);
        var expected = new Guitar.Fretboard(Tuning.Standard, 12).Positions(new Scale(Note.Parse("C4"), ScaleType.Major));
        v.Dots.Select(d => (d.String, d.Fret)).ShouldBe(expected.Select(p => (p.String, p.Fret)));
        v.Dots.Where(d => d.IsRoot).ShouldAllBe(d => d.Label == "C");
        v.Dots.Count(d => d.IsRoot).ShouldBe(expected.Count(p => p.Degree == 1));
        v.Notice.ShouldBeNull();
    }

    [Fact]
    public void Ukulele_draws_four_strings_and_reentrant_order_is_kept()
    {
        var v = FretboardModel.Build("ukulele", 12, "scale", "C", "Major", Lang.En);
        v.Strings.ShouldBe(["G", "C", "E", "A"]);
        v.Dots.Select(d => d.String).Distinct().Order().ShouldBe([0, 1, 2, 3]);
        v.Dots.Single(d => d.String == 0 && d.Fret == 0).Label.ShouldBe("G");
    }

    [Fact]
    public void Labels_follow_the_language()
    {
        var v = FretboardModel.Build("standard", 5, "chord", "C", "Major", Lang.Fr);
        v.Dots.Where(d => d.IsRoot).ShouldAllBe(d => d.Label == "Do");
        v.Dots.Select(d => d.Label).Distinct().Order().ShouldBe(["Do", "Mi", "Sol"]);
    }

    [Fact]
    public void Bad_input_falls_back_with_a_notice()
    {
        var v = FretboardModel.Build("nope", 99, "weird", "H#", "x", Lang.En);
        v.Frets.ShouldBe(12);
        v.Strings.Count.ShouldBe(6);
        v.Notice.ShouldNotBeNull();
        v.Dots.Select(d => d.Label).Distinct().ShouldBe(["C"]);
        FretboardModel.Build("", 12, "", "", "", Lang.En).Notice.ShouldBeNull();
        FretboardModel.Build("standard", 12, "scale", "C", "Bogus", Lang.En).Notice!.ShouldContain("type");
    }

    [Theory]
    [InlineData("chord", "G##", "Augmented")]
    [InlineData("scale", "G##", "Major")]
    public void A_root_the_library_cannot_spell_falls_back_instead_of_throwing(string show, string root, string type)
    {
        var v = FretboardModel.Build("standard", 12, show, root, type, Lang.En);
        v.Notice.ShouldNotBeNull();
        v.Dots.ShouldNotBeEmpty();
    }

    [Fact]
    public void Every_listed_type_has_a_label_in_both_languages()
    {
        foreach (var (mode, prefix) in new[] { ("scale", "fretboard.scale."), ("chord", "fretboard.chord.") })
            foreach (var id in FretboardModel.TypeIds(mode))
                foreach (var lang in new[] { Lang.En, Lang.Fr })
                    Strings.T(lang, prefix + id).ShouldNotStartWith("[");
    }

    [Fact]
    public void Url_params_round_trip()
    {
        var url = "https://x.test/?t=fretboard&tuning=ukulele&frets=15&show=chord&root=Bb&type=Minor7&lang=fr";
        var s = ToolboxUrl.Parse(new Uri(url), "en");
        var again = ToolboxUrl.Parse(new Uri("https://x.test/" + ToolboxUrl.Build(s)), "en");
        again.Params["tuning"].ShouldBe("ukulele");
        again.Params["frets"].ShouldBe("15");
        again.Params["show"].ShouldBe("chord");
        again.Params["root"].ShouldBe("Bb");
        again.Params["type"].ShouldBe("Minor7");
    }

    [Fact]
    public void C_major_on_standard_matches_the_golden()
    {
        var v = FretboardModel.Build("standard", 12, "scale", "C", "Major", Lang.En);
        var actual = string.Join("\n", v.Dots.Select(d => $"s{d.String} f{d.Fret} {d.Label} d{d.Degree}{(d.IsRoot ? " root" : "")}")) + "\n";
        Golden("fretboard-c-major-standard.txt", actual);
    }

    private static void Golden(string file, string actual, [CallerFilePath] string source = "")
    {
        if (Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1")
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(source)!, "Golden", file), actual);
        var expected = Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1"
            ? actual
            : File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Golden", file)).ReplaceLineEndings("\n");
        actual.ShouldBe(expected);
    }
}
