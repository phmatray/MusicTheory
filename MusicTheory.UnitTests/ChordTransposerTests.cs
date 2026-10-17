namespace MusicTheory.UnitTests;

public class ChordTransposerTests
{
    private static KeySignature K(string key) => KeySignature.Parse(key);

    [Theory]
    [InlineData("D/F#", "D", "Eb", "Eb/G")]
    [InlineData("G", "G", "Ab", "Ab")]
    [InlineData("D7", "G", "Ab", "Eb7")]
    [InlineData("Em", "G", "Ab", "Fm")]
    [InlineData("Am7", "C", "Bb", "Gm7")]
    public void Transpose_FromKeyToKey(string chord, string from, string to, string expected)
    {
        ChordTransposer.Transpose(chord, K(from), K(to)).ShouldBe(expected);
    }

    [Theory]
    [InlineData("A", 1, "Eb", "Bb")]
    [InlineData("A", 1, "E", "A#")]
    [InlineData("A", 1, "F", "Bb")]
    [InlineData("F#m", 2, "A", "G#m")]
    [InlineData("C/E", 3, "Eb", "Eb/G")]
    [InlineData("Am/G", -2, "G", "Gm/F")]
    public void Transpose_BySemitones_InTargetKey(string chord, int semitones, string key, string expected)
    {
        ChordTransposer.Transpose(chord, semitones, K(key)).ShouldBe(expected);
    }

    [Theory]
    [InlineData("A", 1, "Bb")]
    [InlineData("Bb", 2, "C")]
    [InlineData("Bb", 1, "B")]
    [InlineData("F#m", 2, "G#m")]
    [InlineData("Eb7", 1, "E7")]
    [InlineData("C", 1, "Db")]
    public void Transpose_WithoutKey_NeverPicksASharpForAFlatChord(string chord, int semitones, string expected)
    {
        ChordTransposer.Transpose(chord, semitones).ShouldBe(expected);
    }

    [Theory]
    [InlineData("N.C.")] [InlineData("xyz")] [InlineData("%")]
    public void Transpose_NonChord_ReturnsInputUnchanged(string text)
    {
        ChordTransposer.Transpose(text, 5).ShouldBe(text);
        ChordTransposer.Transpose(text, K("C"), K("D")).ShouldBe(text);
    }

    [Fact]
    public void Transpose_KeepsUnicodeInput_Unicode()
    {
        ChordTransposer.Transpose("A♭maj7", 2, K("Bb")).ShouldBe("B♭maj7");
        ChordTransposer.Transpose("Bø7", 2).ShouldBe("Dbø7"); // natural root, no key: Db Eb F# Ab Bb
    }

    [Fact]
    public void KeyTranspose_PicksFewestAccidentals()
    {
        var ab = K("G").TransposeBySemitones(1);
        ab.Tonic.Name.ShouldBe(NoteName.A);
        ab.Tonic.Alteration.ShouldBe(Alteration.Flat);
        ab.Mode.ShouldBe(KeyMode.Major);
        ab.ToString(AccidentalStyle.Unicode).ShouldBe("A♭");

        K("C").TransposeBySemitones(6).ToString().ShouldBe("Gb"); // tie: flats from a key with no accidentals
        K("F#").TransposeBySemitones(0).ToString().ShouldBe("F#");
        K("F#").TransposeBySemitones(12).ToString().ShouldBe("F#");
        K("Gb").TransposeBySemitones(0).ToString().ShouldBe("Gb");
        K("Am").TransposeBySemitones(1).ToString().ShouldBe("Bbm");
        K("Em").TransposeBySemitones(-1).ToString().ShouldBe("D#m"); // tie: keeps the sharp side
        K("Fm").TransposeBySemitones(-2).ToString().ShouldBe("Ebm");
    }

    [Fact]
    public void DiatonicChordsOfC_TransposeToEveryMajorKey_LikeChordProgression()
    {
        var c = K("C");
        var source = new ChordProgression(c).GetDiatonicChords()
            .Select(ch => new ChordSymbol(ch.Root, ch.Type)).ToList();

        for (int semitones = 0; semitones < 12; semitones++)
        {
            var key = c.TransposeBySemitones(semitones);
            var expected = new ChordProgression(key).GetDiatonicChords().Select(ch => new ChordSymbol(ch.Root, ch.Type).ToString());
            source.Select(s => s.Transpose(c, key).ToString()).ShouldBe(expected, $"key {key}");
        }
    }

    [Theory]
    [InlineData("Eb", NoteName.E, Alteration.Flat, KeyMode.Major)]
    [InlineData("E♭", NoteName.E, Alteration.Flat, KeyMode.Major)]
    [InlineData("Ebm", NoteName.E, Alteration.Flat, KeyMode.Minor)]
    [InlineData("E♭ minor", NoteName.E, Alteration.Flat, KeyMode.Minor)]
    [InlineData("ebm", NoteName.E, Alteration.Flat, KeyMode.Minor)]
    [InlineData("C major", NoteName.C, Alteration.Natural, KeyMode.Major)]
    [InlineData("F# minor", NoteName.F, Alteration.Sharp, KeyMode.Minor)]
    public void KeyParse_Accepts(string text, NoteName name, Alteration alteration, KeyMode mode)
    {
        var key = K(text);
        key.Tonic.Name.ShouldBe(name);
        key.Tonic.Alteration.ShouldBe(alteration);
        key.Mode.ShouldBe(mode);
    }

    [Theory]
    [InlineData("H#q")] [InlineData("")] [InlineData(null)] [InlineData("C dorian")] [InlineData("Fbb")]
    public void KeyParse_Rejects(string? text)
    {
        KeySignature.TryParse(text, out _).ShouldBeFalse();
    }

    [Fact]
    public void KeySignature_FlatMinorKeys_HaveFlats()
    {
        K("Fm").AccidentalCount.ShouldBe(-4);
        K("Cm").AccidentalCount.ShouldBe(-3);
        K("Abm").AccidentalCount.ShouldBe(-7);
        K("Db").GetRelative().ToString().ShouldBe("Bbm");
    }

    [Fact]
    public void Note_TransposeBySemitones_InKey_KeepsOctave()
    {
        var n = new Note(NoteName.B, Alteration.Natural, 3).TransposeBySemitones(1, K("F"));
        n.ToString().ShouldBe("C4");
        new Note(NoteName.G, Alteration.Natural, 4).TransposeBySemitones(1, K("Ab")).ToString().ShouldBe("Ab4");
    }
}
