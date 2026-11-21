using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.Tools;
using MusicTheory.Toolbox.Tools.Circle;

namespace MusicTheory.Toolbox.UnitTests;

public class CircleModelTests
{
    [Fact]
    public void G_major_in_french()
    {
        var v = CircleModel.For("G", Lang.Fr);
        v.Fallback.ShouldBeFalse();
        v.Key.Title.ShouldBe("Sol majeur");
        v.Key.Signature.ShouldBe("1 dièse");
        v.Key.RelativeMinor.ShouldBe("Mi mineur");
        string.Join(" ", v.Key.ScaleNotes).ShouldBe("Sol La Si Do Ré Mi Fa dièse");
    }

    [Fact]
    public void G_major_in_english()
    {
        var v = CircleModel.For("G", Lang.En);
        v.Key.Title.ShouldBe("G major");
        v.Key.RelativeMinor.ShouldBe("E minor");
        string.Join(" ", v.Key.ScaleNotes).ShouldBe("G A B C D E F#");
    }

    [Fact]
    public void Ring_has_twelve_keys_with_flat_signatures()
    {
        var keys = CircleModel.Keys(Lang.En);
        keys.Count.ShouldBe(12);
        keys.Select(k => k.Accidentals).ShouldBe([0, 1, 2, 3, 4, 5, 6, -5, -4, -3, -2, -1]);
        CircleModel.For("Eb", Lang.En).Key.Signature.ShouldBe("3 flats");
        CircleModel.For("Em", Lang.En).Key.Id.ShouldBe("G");
    }

    [Theory]
    [InlineData("H")]
    [InlineData("")]
    [InlineData("Z")]
    public void Unknown_key_falls_back_to_C_with_a_notice(string key)
    {
        var v = CircleModel.For(key, Lang.En);
        v.Fallback.ShouldBeTrue();
        v.Key.Id.ShouldBe("C");
    }

    [Fact]
    public void Every_ring_key_spells_a_seven_note_scale_from_its_tonic_in_both_languages()
    {
        foreach (var lang in new[] { Lang.En, Lang.Fr })
            foreach (var k in CircleModel.Keys(lang))
            {
                k.ScaleNotes.Count.ShouldBe(7);
                k.ScaleNotes.Distinct().Count().ShouldBe(7);
            }
        string.Join(" ", CircleModel.For("F#", Lang.En).Key.ScaleNotes).ShouldBe("F# G# A# B C# D# E#");
        string.Join(" ", CircleModel.For("Db", Lang.Fr).Key.ScaleNotes).ShouldBe("Ré bémol Mi bémol Fa Sol bémol La bémol Si bémol Do");
    }

    [Fact]
    public void Scale_abc_for_G_major_has_the_key_header_and_the_eight_scale_notes()
    {
        var abc = CircleModel.For("G", Lang.En).ScaleAbc;
        abc.ShouldContain("K:G");
        var notes = MusicTheory.Abc.AbcTune.Parse(abc).Measures.SelectMany(m => m.Events).Select(e => e.Pitches.Single()).ToArray();
        notes.ShouldBe("G4 A4 B4 C5 D5 E5 F#5 G5".Split(' ').Select(Note.Parse).ToArray());
    }

    [Fact]
    public void Every_key_has_a_parseable_scale_abc() =>
        CircleModel.Keys(Lang.En).ToList().ForEach(k => MusicTheory.Abc.AbcTune.TryParse(k.ScaleAbc, out var _).ShouldBeTrue());

    [Fact]
    public void No_key_is_C_without_a_notice() => CircleModel.For(null, Lang.En).Fallback.ShouldBeFalse();

    [Fact]
    public void Registry_lists_the_circle_with_a_title_in_both_languages()
    {
        var tool = ToolRegistry.Find("circle").ShouldNotBeNull();
        Strings.T(Lang.Fr, tool.TitleKey).ShouldBe("Cercle des quintes");
        ToolRegistry.Find("nope").ShouldBeNull();
    }
}
