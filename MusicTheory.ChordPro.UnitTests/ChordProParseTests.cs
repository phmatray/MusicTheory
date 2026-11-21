namespace MusicTheory.ChordPro.UnitTests;

public class ChordProParseTests
{
    public static IEnumerable<object[]> Fixtures() =>
        Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Fixtures"), "*.cho").Select(f => new object[] { Path.GetFileName(f) });

    [Theory]
    [MemberData(nameof(Fixtures))]
    public void Fixture_round_trips_byte_for_byte(string name)
    {
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", name));

        ChordProDocument.Parse(text).ToString().ShouldBe(text);
    }

    [Fact]
    public void Title_and_chords()
    {
        var doc = ChordProDocument.Parse("{t: X}\n[G]a [C]b [G]c");

        doc.Title.ShouldBe("X");
        doc.Chords.ShouldBe(["G", "C"]);
    }

    [Fact]
    public void Subtitle_capo_and_both_separators()
    {
        var doc = ChordProDocument.Parse("{st: Y}\n{capo 3}\n{soc }");

        doc.Subtitle.ShouldBe("Y");
        doc.Capo.ShouldBe(3);
        ((DirectiveLine)doc.Lines[2]).Value.ShouldBeNull();
    }

    [Fact]
    public void Missing_or_malformed_directives_are_null()
    {
        var doc = ChordProDocument.Parse("{capo: two}\n{}");

        doc.Capo.ShouldBeNull();
        doc.Title.ShouldBeNull();
        doc.Key.ShouldBeNull();
    }

    [Fact]
    public void Key_is_parsed_or_null()
    {
        var flat = ChordProDocument.Parse("{key: Bbm}").Key!;
        flat.Tonic.ShouldBe(Note.Parse("Bb"));
        flat.Mode.ShouldBe(KeyMode.Minor);

        ChordProDocument.Parse("{key: H}").Key.ShouldBeNull();
    }

    [Fact]
    public void Segments_split_at_each_chord()
    {
        var line = (LyricLine)ChordProDocument.Parse("Intro [G]a").Lines[0];

        line.Segments.ShouldBe([new ChordSegment(null, "Intro "), new ChordSegment("G", "a")]);
    }

    [Fact]
    public void Leading_chord_has_no_empty_segment_before_it()
    {
        var line = (LyricLine)ChordProDocument.Parse("[G][C]x[D]").Lines[0];

        line.Segments.ShouldBe([new ChordSegment("G", ""), new ChordSegment("C", "x"), new ChordSegment("D", "")]);
    }

    [Fact]
    public void Unclosed_bracket_is_lyric_text()
    {
        var line = (LyricLine)ChordProDocument.Parse("[G]a [b").Lines[0];

        line.Segments.ShouldBe([new ChordSegment("G", "a [b")]);
        ((LyricLine)ChordProDocument.Parse("a [b").Lines[0]).Segments.ShouldBe([new ChordSegment(null, "a [b")]);
    }

    [Fact]
    public void Short_names_are_normalised()
    {
        var names = new[] { "t", "st", "c", "soc", "eoc", "sov", "eov", "sob", "eob", "sot", "eot", "Define" };
        var doc = ChordProDocument.Parse(string.Join('\n', names.Select(n => "{" + n + "}")));

        doc.Lines.Cast<DirectiveLine>().Select(d => d.Name).ShouldBe(
        [
            "title", "subtitle", "comment", "start_of_chorus", "end_of_chorus", "start_of_verse", "end_of_verse",
            "start_of_bridge", "end_of_bridge", "start_of_tab", "end_of_tab", "define",
        ]);
    }

    [Fact]
    public void Comment_lines_and_unclosed_braces()
    {
        var doc = ChordProDocument.Parse("# [G]x\n{oops [G]y");

        doc.Lines[0].ShouldBeOfType<CommentLine>();
        doc.Lines[1].ShouldBeOfType<LyricLine>();
        doc.Chords.ShouldBe(["G"]);
    }

    [Fact]
    public void Tab_block_is_left_unsegmented_and_directive_inside_keeps_it_open()
    {
        var doc = ChordProDocument.Parse("{sot}\n[x]-0-\n{c: note}\n[y]\n{eot}\n[z]");

        doc.Lines[1].ShouldBeOfType<LyricLine>().Segments.ShouldBe([new ChordSegment(null, "[x]-0-")]);
        doc.Chords.ShouldBe(["z"]);
    }

    [Fact]
    public void Crlf_is_kept_in_raw_but_not_in_segments()
    {
        var doc = ChordProDocument.Parse("[G]a\r\n{t: T}\r\n");

        doc.Lines[0].Raw.ShouldBe("[G]a\r");
        doc.Lines[0].ShouldBeOfType<LyricLine>().Segments.ShouldBe([new ChordSegment("G", "a")]);
        doc.Title.ShouldBe("T");
        doc.ToString().ShouldBe("[G]a\r\n{t: T}\r\n");
    }

    [Fact]
    public void Empty_text_round_trips()
    {
        ChordProDocument.Parse("").ToString().ShouldBe("");
    }
}
