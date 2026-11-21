using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;
using MusicTheory.Toolbox.Tools;
using MusicTheory.Toolbox.Tools.Finder;

namespace MusicTheory.Toolbox.UnitTests;

public class FinderModelTests
{
    [Fact]
    public void Major_and_minor_triads_rank_first()
    {
        FinderModel.Identify([0, 4, 7], Lang.En)[0].Symbol.ShouldBe("C");
        FinderModel.Identify([0, 3, 7], Lang.En)[0].Symbol.ShouldBe("Cm");
        FinderModel.Identify([0, 4, 7], Lang.Fr)[0].Symbol.ShouldBe("Do");
    }

    [Fact]
    public void Labels_differ_between_languages()
    {
        FinderModel.Identify([0, 4, 7], Lang.En)[0].TypeLabel.ShouldBe("major");
        FinderModel.Identify([0, 4, 7], Lang.Fr)[0].TypeLabel.ShouldBe("majeur");
    }

    [Fact]
    public void Empty_set_gives_no_rows() => FinderModel.Identify([], Lang.En).ShouldBeEmpty();

    [Fact]
    public void Cmaj7_lists_scales_with_ionian_or_lydian_and_modes()
    {
        var r = FinderModel.ScalesFor("Cmaj7", Lang.En);
        r.Error.ShouldBeNull();
        r.Scales.ShouldNotBeEmpty();
        r.Scales.Select(s => s.Id).ShouldContain("Lydian");
        var modes = FinderModel.Modes(FinderModel.ScaleOf(r.Root!, "Major")!, Lang.En);
        modes.Count.ShouldBe(7);
        modes[0].Notes.ShouldBe("C D E F G A B");
        modes[1].Label.ShouldBe("D dorian");
        FinderModel.Modes(FinderModel.ScaleOf(r.Root!, "Major")!, Lang.Fr)[1].Label.ShouldBe("Ré dorien");
    }

    [Fact]
    public void Unparsable_chord_returns_the_library_message()
    {
        var r = FinderModel.ScalesFor("Cxyz", Lang.En);
        r.Scales.ShouldBeEmpty();
        r.Error.ShouldNotBeNullOrWhiteSpace();
        Should.Throw<FormatException>(() => ChordSymbol.Parse("Cxyz")).Message.ShouldBe(r.Error);
    }

    [Fact]
    public void Scale_without_modes_gives_an_empty_list() =>
        FinderModel.Modes(new Scale(Note.Parse("C4"), ScaleType.Blues), Lang.En).ShouldBeEmpty();

    [Fact]
    public void Unknown_scale_id_is_null() => FinderModel.ScaleOf(Note.Parse("C4"), "Nope").ShouldBeNull();

    [Theory]
    [InlineData("7,0,4,4,x,12,-1,7", "0,4,7")]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void Pcs_are_sorted_deduplicated_and_clamped(string? input, string? expected) =>
        FinderModel.FormatPitchClasses(FinderModel.ParsePitchClasses(input)).ShouldBe(expected);

    [Fact]
    public void Pcs_round_trip_through_the_url_and_toggle()
    {
        var url = ToolboxUrl.Build(new ToolState("finder", Lang.Fr, new Dictionary<string, string> { ["pcs"] = "0,4,7", ["chord"] = "Cmaj7" }));
        url.ShouldBe("?t=finder&lang=fr&v=1&chord=Cmaj7&pcs=0%2C4%2C7");
        var state = ToolboxUrl.Parse(new Uri("http://x/" + url), null);
        var pcs = FinderModel.ParsePitchClasses(state.Params["pcs"]);
        pcs.ShouldBe([0, 4, 7]);
        FinderModel.Toggle(pcs, 11).ShouldBe([0, 4, 7, 11]);
        FinderModel.Toggle(pcs, 4).ShouldBe([0, 7]);
    }

    [Fact]
    public void Registry_lists_the_finder() =>
        Strings.T(Lang.Fr, ToolRegistry.Find("finder")!.TitleKey).ShouldBe("Chercheur d'accords et de gammes");
}
