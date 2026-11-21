using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;

namespace MusicTheory.Toolbox.UnitTests;

public class ToolboxUrlTests
{
    private static string Canon(string url, string browser = "en") =>
        ToolboxUrl.Build(ToolboxUrl.Parse(new Uri(url), browser));

    [Fact]
    public void Build_writes_canonical_order_and_round_trips()
    {
        var canonical = Canon("https://x.test/toolbox/?key=G&v=1&lang=fr&t=circle&a=1");
        canonical.ShouldBe("?t=circle&lang=fr&v=1&a=1&key=G");
        Canon("https://x.test/" + canonical).ShouldBe(canonical);
    }

    [Fact]
    public void Missing_lang_follows_the_browser()
    {
        Canon("https://x.test/?t=circle", "fr-BE").ShouldBe("?t=circle&lang=fr&v=1");
        Canon("https://x.test/?t=circle", "de").ShouldBe("?t=circle&lang=en&v=1");
    }

    [Fact]
    public void Junk_never_throws_and_falls_back_to_english()
    {
        Canon("https://x.test/?t=&&=x&lang=zz").ShouldBe("?lang=en&v=1");
        var junk = Canon("https://x.test/?%E0%A4%A&key=%ZZ&key=G&v=9#frag");
        junk.ShouldStartWith("?lang=en&v=1");
        junk.ShouldContain("&key=G");
        Canon("https://x.test/").ShouldBe("?lang=en&v=1");
    }

    [Fact]
    public void Fragment_before_the_query_is_not_parsed_and_lang_must_be_exact()
    {
        Canon("https://x.test/#/a?b=1").ShouldBe("?lang=en&v=1");
        Canon("https://x.test/?lang=fruit", "fr").ShouldBe("?lang=fr&v=1");
        Canon("https://x.test/?lang=FR", "en").ShouldBe("?lang=fr&v=1");
    }

    [Fact]
    public void Keys_are_lowercased_and_values_trimmed()
    {
        Canon("https://x.test/?T=Circle&KEY=%20G%20").ShouldBe("?t=circle&lang=en&v=1&key=G");
    }

    [Fact]
    public void Switching_language_keeps_every_other_parameter()
    {
        var state = ToolboxUrl.Parse(new Uri("https://x.test/?t=circle&key=G&lang=en"), "en");
        ToolboxUrl.Build(state with { Lang = Lang.Fr }).ShouldBe("?t=circle&lang=fr&v=1&key=G");
    }
}
