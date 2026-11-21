using System.Runtime.CompilerServices;
using MusicTheory.Guitar;
using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;
using MusicTheory.Toolbox.Tools.Voicings;

namespace MusicTheory.Toolbox.UnitTests;

public class VoicingsModelTests
{
    [Fact]
    public void C_on_standard_matches_the_provider_and_the_golden()
    {
        var v = VoicingsModel.For("C", "standard", Lang.En);
        var expected = new GuitarVoicingProvider().GetVoicings(ChordSymbol.Parse("C"), new VoicingOptions(Tuning: Tuning.Standard));
        v.Cards.Select(c => c.Tab).ShouldBe(expected.Select(e => e.ToTab()));
        v.Cards.ShouldNotBeEmpty();
        v.Error.ShouldBeNull();
        var actual = string.Join("\n", v.Cards.Select(c =>
            $"{c.Tab} base{c.Voicing.BaseFret} fingers[{string.Join(",", c.Voicing.Fingers.Select(f => f?.ToString() ?? "-"))}] barre{(c.Voicing.Barre is { } b ? $" f{b.Fret} s{b.LowString}-{b.HighString}" : "-")} {c.Shape}")) + "\n";
        Golden("voicings-c-standard.txt", actual);
    }

    [Fact]
    public void Ukulele_gives_four_string_shapes()
    {
        var v = VoicingsModel.For("C", "ukulele", Lang.En);
        v.Cards.ShouldNotBeEmpty();
        v.Cards.ShouldAllBe(c => c.Voicing.Frets.Count == 4 && c.Layout.Strings.Count == 4);
    }

    [Fact]
    public void A_bad_chord_returns_the_library_message()
    {
        var v = VoicingsModel.For("H#m", "standard", Lang.En);
        v.Cards.ShouldBeEmpty();
        var message = Should.Throw<FormatException>(() => ChordSymbol.Parse("H#m")).Message;
        v.Error.ShouldBe(message);
    }

    [Fact]
    public void An_empty_chord_means_C_and_an_unknown_tuning_falls_back_with_a_notice()
    {
        VoicingsModel.For("", "", Lang.En).ChordText.ShouldBe("C");
        VoicingsModel.For("", "", Lang.En).Notice.ShouldBeNull();
        var v = VoicingsModel.For("C", "nope", Lang.Fr);
        v.Tuning.Id.ShouldBe("standard");
        v.Notice.ShouldNotBeNull();
    }

    [Fact]
    public void A_chord_the_tuning_cannot_play_is_an_empty_state_not_an_error()
    {
        var v = VoicingsModel.For("C13#11", "mandolin", Lang.En);
        v.Error.ShouldBeNull();
    }

    [Fact]
    public void Shape_names_follow_the_language()
    {
        VoicingsModel.For("C", "standard", Lang.Fr).Cards.Select(c => c.Shape).ShouldAllBe(s => !s.StartsWith("["));
        VoicingsModel.For("C", "standard", Lang.Fr).Cards.Select(c => c.Shape).ShouldContain("Position ouverte");
    }

    [Fact]
    public void Url_params_round_trip()
    {
        var s = ToolboxUrl.Parse(new Uri("https://x.test/?t=voicings&chord=F%23m7&tuning=ukulele&lang=fr"), "en");
        var again = ToolboxUrl.Parse(new Uri("https://x.test/" + ToolboxUrl.Build(s)), "en");
        again.Params["chord"].ShouldBe("F#m7");
        again.Params["tuning"].ShouldBe("ukulele");
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
