using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;
using MusicTheory.Toolbox.Tools.Analysis;

namespace MusicTheory.Toolbox.UnitTests;

public class AnalysisModelTests
{
    [Fact]
    public void Authentic_ending_in_C()
    {
        var r = AnalysisModel.Run("C Am F G7 C", "C", Lang.En);
        r.Error.ShouldBeNull();
        r.Detected.ShouldBeFalse();
        r.Rows.Select(x => x.Numeral).ShouldBe(["I", "vi", "IV", "V7", "I"]);
        r.Rows.All(x => x.Function == "Diatonic").ShouldBeTrue();
        r.Rows[4].Cadence.ShouldBe("Authentic cadence");
        r.Cadences.ShouldContain(new CadenceMark(5, "Authentic cadence"));
    }

    [Fact]
    public void Numerals_match_the_library()
    {
        var chords = "C Am F G7 C".Split(' ').Select(ChordSymbol.Parse).ToList();
        var key = KeySignature.Parse("C");
        AnalysisModel.Run("C|Am|F|G7|C", "C", Lang.En).Rows.Select(x => x.Numeral)
            .ShouldBe(HarmonicAnalyzer.Analyze(chords, key).Select(n => n.ToString()));
    }

    [Fact]
    public void Without_a_key_it_is_detected_and_ranked()
    {
        var r = AnalysisModel.Run("C Am F G7 C", null, Lang.Fr);
        r.Detected.ShouldBeTrue();
        r.Candidates.ShouldNotBeEmpty();
        r.UsedKey.ShouldBe(r.Candidates[0].Name);
        r.UsedKey.ShouldBe("Do majeur");
    }

    [Fact]
    public void A_modulating_progression_has_regions_with_a_pivot()
    {
        var r = AnalysisModel.Run("C F G C D G A D G D", null, Lang.En);
        r.Regions.Count.ShouldBeGreaterThan(1);
        r.Regions[0].Start.ShouldBe(1);
        r.Regions[^1].End.ShouldBe(10);
        r.Regions.Skip(1).Any(x => x.PivotIndex is not null).ShouldBeTrue();
    }

    [Theory]
    [InlineData("C Am Zz G7", "Zz")]
    [InlineData("C H7", "H7")]
    public void A_bad_token_is_named(string text, string token)
    {
        var r = AnalysisModel.Run(text, "C", Lang.En);
        r.Error.ShouldNotBeNull().ShouldContain(token);
        r.Rows.ShouldBeEmpty();
    }

    [Fact]
    public void A_bad_key_and_empty_input_do_not_throw()
    {
        AnalysisModel.Run("C G", "Q#", Lang.En).Error.ShouldNotBeNull();
        var empty = AnalysisModel.Run("  ", null, Lang.En);
        empty.Error.ShouldBeNull();
        empty.Rows.ShouldBeEmpty();
    }

    [Fact]
    public void Too_long_input_is_refused_and_a_chosen_key_hides_detected_regions()
    {
        AnalysisModel.Run(string.Join(' ', Enumerable.Repeat("C", AnalysisModel.MaxChords + 1)), "C", Lang.En).Error.ShouldNotBeNull();
        AnalysisModel.Run("C Am F G7 C", "A minor", Lang.En).Regions.ShouldBeEmpty();
    }

    [Fact]
    public void Lower_case_roots_are_accepted()
    {
        AnalysisModel.Run("c am f g7 c", "C", Lang.En).Error.ShouldBeNull();
    }

    [Fact]
    public void Every_cadence_type_and_function_has_both_labels()
    {
        foreach (var lang in new[] { Lang.En, Lang.Fr })
        {
            foreach (var t in Enum.GetValues<CadenceType>())
                Strings.T(lang, "analysis.cad." + t).ShouldNotStartWith("[");
            foreach (var f in Enum.GetValues<HarmonicFunction>())
                Strings.T(lang, "analysis.fn." + f).ShouldNotStartWith("[");
        }
    }

    [Fact]
    public void Chords_with_bars_and_spaces_round_trip_through_the_url()
    {
        var state = new ToolState("analysis", Lang.Fr, new Dictionary<string, string> { ["chords"] = "C | Am F|G7", ["key"] = "C" });
        var parsed = ToolboxUrl.Parse(new Uri("https://x/" + ToolboxUrl.Build(state)), null);
        parsed.ShouldBe(state with { Params = parsed.Params });
        parsed.Params["chords"].ShouldBe("C | Am F|G7");
        parsed.Params["key"].ShouldBe("C");
    }
}
