using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;
using MusicTheory.Toolbox.Tools.Transposer;

namespace MusicTheory.Toolbox.UnitTests;

public class TransposerModelTests
{
    private static TransposeResult Run(string inst, TransposeDirection d, string key, string chords = "", Lang lang = Lang.En) =>
        TransposerModel.Run(Instruments.Find(inst), d, KeySignature.Parse(key), chords, lang);

    [Theory]
    [InlineData("bb", "D")]
    [InlineData("eb", "A")]
    [InlineData("f", "G")]
    [InlineData("bbt", "D")]
    [InlineData("c", "C")]
    public void Concert_C_to_written_and_back(string inst, string written)
    {
        Run(inst, TransposeDirection.ConcertToWritten, "C").TargetKey.ShouldBe(KeySignature.Parse(written));
        Run(inst, TransposeDirection.WrittenToConcert, written).TargetKey.ShouldBe(KeySignature.Parse("C"));
    }

    [Fact]
    public void The_table_is_pinned()
    {
        Instruments.All.ToDictionary(i => i.Id, i => i.WrittenAboveConcertSemitones)
            .ShouldBe(new Dictionary<string, int> { ["c"] = 0, ["bb"] = 2, ["eb"] = 9, ["f"] = 7, ["bbt"] = 14 });
    }

    [Fact]
    public void Chords_take_the_spelling_of_the_target_key()
    {
        var r = Run("bb", TransposeDirection.ConcertToWritten, "Bb", "Bbmaj7 Gm7 C7");
        r.Chords.ShouldBe(["Cmaj7", "Am7", "D7"]);
        r.Notices.ShouldBeEmpty();
    }

    [Fact]
    public void A_theoretical_key_shows_its_equivalent_with_a_notice()
    {
        var r = Run("bb", TransposeDirection.ConcertToWritten, "A#");
        r.Notices.Count.ShouldBe(1);
        r.Notices[0].ShouldContain("Bb");
        r.TargetKey.ShouldBe(KeySignature.Parse("C"));
    }

    [Fact]
    public void An_unparsable_chord_is_kept_with_a_notice()
    {
        var r = Run("bb", TransposeDirection.ConcertToWritten, "C", "C Xyz|9 G", Lang.Fr);
        r.Chords.ShouldBe(["D", "Xyz|9", "A"]);
        r.Notices.ShouldHaveSingleItem().ShouldContain("Xyz|9");
    }

    [Fact]
    public void French_instrument_names_use_fixed_do()
    {
        TransposerModel.Name(Instruments.Find("bb"), Lang.Fr).ShouldContain("Si bémol");
    }

    [Fact]
    public void State_round_trips_through_the_url()
    {
        var state = ToolboxUrl.Parse(new Uri("https://x.test/?t=transpose&inst=bb&dir=cw&key=C&chords=Bb%20Gm7%20C7&lang=fr"), "en");
        state.Params["chords"].ShouldBe("Bb Gm7 C7");
        var again = ToolboxUrl.Parse(new Uri("https://x.test/" + ToolboxUrl.Build(state)), "en");
        again.Params.ShouldBe(state.Params);
        again.Lang.ShouldBe(Lang.Fr);
    }
}
