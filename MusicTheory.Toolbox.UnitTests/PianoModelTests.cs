using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;
using MusicTheory.Toolbox.Tools.Piano;

namespace MusicTheory.Toolbox.UnitTests;

public class PianoModelTests
{
    private static Scale CMajor => new(Note.Parse("C4"), ScaleType.Major);

    [Theory]
    [InlineData(Hand.Right)]
    [InlineData(Hand.Left)]
    public void C_major_fingers_equal_the_library(Hand hand)
    {
        var v = PianoModel.For("C", "Major", hand, 1, Lang.En);
        v.Marks.Select(m => m.Finger).ShouldBe(PianoFingering.ForScale(CMajor, hand));
        v.Marks.Select(m => m.Midi).ShouldBe([60, 62, 64, 65, 67, 69, 71, 72]);
        v.Notice.ShouldBeNull();
    }

    [Fact]
    public void The_hands_differ_where_the_library_differs()
    {
        var rh = PianoModel.For("C", "Major", Hand.Right, 1, Lang.En).Marks.Select(m => m.Finger);
        var lh = PianoModel.For("C", "Major", Hand.Left, 1, Lang.En).Marks.Select(m => m.Finger);
        rh.ShouldNotBe(lh);
    }

    [Fact]
    public void Two_octaves_continue_the_pattern()
    {
        var v = PianoModel.For("G", "Major", Hand.Right, 2, Lang.En);
        v.Marks.Count.ShouldBe(15);
        v.Marks.Select(m => m.Finger).ShouldBe(PianoFingering.ForScale(new Scale(Note.Parse("G4"), ScaleType.Major), Hand.Right, 2));
        v.Marks[^1].Midi.ShouldBe(v.Marks[0].Midi + 24);
    }

    [Fact]
    public void Triad_uses_ForTriad_and_labels_follow_the_language()
    {
        var v = PianoModel.For("C", "Major", Hand.Right, 1, Lang.Fr, triad: true);
        v.Marks.Select(m => m.Label).ShouldBe(["Do", "Mi", "Sol"]);
        v.Marks.Select(m => m.Finger).ShouldBe(PianoFingering.ForTriad([Note.Parse("C4"), Note.Parse("E4"), Note.Parse("G4")], Hand.Right));
    }

    [Fact]
    public void Keys_cover_every_mark_and_edges_are_white()
    {
        var v = PianoModel.For("Db", "Major", Hand.Right, 1, Lang.En);
        v.Keys.Select(k => k.Midi).ShouldBeSubsetOf(Enumerable.Range(0, 128));
        v.Marks.ShouldAllBe(m => v.Keys.Any(k => k.Midi == m.Midi));
        v.Keys[0].IsBlack.ShouldBeFalse();
        v.Keys[^1].IsBlack.ShouldBeFalse();
    }

    [Fact]
    public void Bad_input_falls_back_with_a_notice_and_a_root_the_library_cannot_spell_does_not_throw()
    {
        var v = PianoModel.For("H", "Blues", Hand.Right, 9, Lang.En);
        v.Notice!.ShouldContain("type");
        v.Marks.Count.ShouldBe(8);
        PianoModel.For("", "", Hand.Right, 1, Lang.En).Notice.ShouldBeNull();
        PianoModel.For("G##", "Major", Hand.Right, 1, Lang.En).Notice.ShouldNotBeNull();
    }

    [Fact]
    public void Every_listed_scale_has_a_label_in_both_languages()
    {
        foreach (var t in PianoModel.ScaleTypes)
            foreach (var lang in new[] { Lang.En, Lang.Fr })
                Strings.T(lang, "piano.scale." + t).ShouldNotStartWith("[");
    }

    [Fact]
    public void Url_params_round_trip()
    {
        var s = ToolboxUrl.Parse(new Uri("https://x.test/?t=piano&root=Bb&type=Dorian&hand=lh&oct=2&triad=1&lang=en"), "en");
        var again = ToolboxUrl.Parse(new Uri("https://x.test/" + ToolboxUrl.Build(s)), "en");
        again.Params["hand"].ShouldBe("lh");
        again.Params["oct"].ShouldBe("2");
        again.Params["root"].ShouldBe("Bb");
        again.Params["triad"].ShouldBe("1");
    }
}
