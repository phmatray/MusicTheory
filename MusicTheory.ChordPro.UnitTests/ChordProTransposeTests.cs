namespace MusicTheory.ChordPro.UnitTests;

public class ChordProTransposeTests
{
    private const string Grace = "{title: Amazing Grace}\n{key: G}\n[G]Amazing [G7]grace, how [C]sweet the [G]sound";

    public static IEnumerable<object[]> Fixtures() => ChordProParseTests.Fixtures();

    private static string Fixture(string name) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", name));

    [Fact]
    public void Up_two_semitones_moves_key_and_chords_only()
    {
        ChordProDocument.Parse(Grace).Transpose(2).ToString()
            .ShouldBe("{title: Amazing Grace}\n{key: A}\n[A]Amazing [A7]grace, how [D]sweet the [A]sound");
    }

    [Fact]
    public void Theoretical_key_zero_is_identity_and_two_is_practical()
    {
        const string text = "{key: A#}\n[A#]a [D#]b";
        ChordProDocument.Parse(text).Transpose(0).ToString().ShouldBe(text);
        ChordProDocument.Parse(text).Transpose(2).ToString().ShouldBe("{key: C}\n[C]a [F]b");
    }

    [Fact]
    public void To_Bb_spells_flats()
    {
        ChordProDocument.Parse(Grace).Transpose(new KeySignature(Note.Parse("Bb"), KeyMode.Major)).ToString()
            .ShouldBe("{title: Amazing Grace}\n{key: Bb}\n[Bb]Amazing [Bb7]grace, how [Eb]sweet the [Bb]sound");
    }

    [Fact]
    public void Annotations_and_tab_survive()
    {
        ChordProDocument.Parse(Fixture("annotations-tab.cho")).Transpose(2).ToString().ShouldBe(
            "{title: Mixed}\n{key: A}\n[A]Go [*Riff]on [N.C.]now [%]\n{sot}\n[G]e|--0--|  [C] not a chord here\n{eot}\n[D]after\n");
    }

    [Fact]
    public void Crlf_is_preserved_on_changed_lines()
    {
        ChordProDocument.Parse("{key: G}\r\n[G]a\r\n").Transpose(2).ToString().ShouldBe("{key: A}\r\n[A]a\r\n");
    }

    [Fact]
    public void Without_a_key_only_chords_move()
    {
        ChordProDocument.Parse("{key: H}\n[C]a").Transpose(2).ToString().ShouldBe("{key: H}\n[D]a");
    }

    [Fact]
    public void Verbatim_keeps_the_writers_suffix()
    {
        const string text = "{key: G}\n[G-7]a [CΔ7]b";
        ChordProDocument.Parse(text).Transpose(2, SuffixHandling.Verbatim).ToString().ShouldBe("{key: A}\n[A-7]a [DΔ7]b");
        ChordProDocument.Parse(text).Transpose(2).ToString().ShouldBe("{key: A}\n[Am7]a [Dmaj7]b");
    }

    [Fact]
    public void KeepUnparsed_moves_root_and_bass_of_unreadable_chords()
    {
        const string text = "{key: G}\n[Gfoo/B]a [*Riff] [N.C.]";
        ChordProDocument.Parse(text).Transpose(2, SuffixHandling.KeepUnparsed).ToString().ShouldBe("{key: A}\n[Afoo/C#]a [*Riff] [N.C.]");
        ChordProDocument.Parse(text).Transpose(2).ToString().ShouldBe("{key: A}\n[Gfoo/B]a [*Riff] [N.C.]");
        ChordProDocument.Parse(text).Transpose(new KeySignature(Note.Parse("Bb"), KeyMode.Major), SuffixHandling.KeepUnparsed).ToString()
            .ShouldBe("{key: Bb}\n[Bbfoo/D]a [*Riff] [N.C.]");
    }

    [Fact]
    public void Key_target_without_key_throws()
    {
        Should.Throw<InvalidOperationException>(() => ChordProDocument.Parse("[G]a").Transpose(new KeySignature(Note.Parse("A"), KeyMode.Major)));
    }

    [Theory]
    [MemberData(nameof(Fixtures))]
    public void Zero_is_identity(string name) => ChordProDocument.Parse(Fixture(name)).Transpose(0).ToString().ShouldBe(Fixture(name));

    [Theory]
    [MemberData(nameof(Fixtures))]
    public void Octave_keeps_letters(string name)
    {
        var doc = ChordProDocument.Parse(Fixture(name));
        doc.Transpose(12).Chords.ShouldBe(doc.Chords);
    }
}
