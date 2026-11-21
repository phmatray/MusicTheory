namespace MusicTheory.UnitTests;

/// <summary>Version 2 behaviour of <see cref="KeySignature"/> and <see cref="Scale"/>.</summary>
public class KeyAndScaleValueTests
{
    [Fact]
    public void KeySignature_Validates()
    {
        Should.Throw<ArgumentNullException>(() => new KeySignature(null!, KeyMode.Major));
        Should.Throw<ArgumentOutOfRangeException>(() => new KeySignature(new Note(NoteName.C), (KeyMode)5)).ParamName.ShouldBe("mode");
        Should.Throw<ArgumentOutOfRangeException>(() => new KeySignature(Note.Parse("Cbb"), KeyMode.Minor))
            .Message.ShouldStartWith("Cbbm needs 17 flats; at most 14 (two per note) are supported.");
        Should.Throw<ArgumentOutOfRangeException>(() => new KeySignature(Note.Parse("B##"), KeyMode.Major))
            .Message.ShouldStartWith("B## needs 19 sharps; at most 14 (two per note) are supported.");
    }

    [Fact]
    public void KeySignature_TheoreticalKeysCarryDoubleAccidentals()
    {
        var gSharp = new KeySignature(Note.Parse("G#"), KeyMode.Major);
        gSharp.AccidentalCount.ShouldBe(8);
        gSharp.GetAlteration(NoteName.F).ShouldBe(Alteration.DoubleSharp);
        gSharp.GetAlteration(NoteName.C).ShouldBe(Alteration.Sharp);
        gSharp.AlteredNotes.Count.ShouldBe(7);
        var fFlat = new KeySignature(Note.Parse("Fb"), KeyMode.Major);
        fFlat.GetAlteration(NoteName.B).ShouldBe(Alteration.DoubleFlat);
        fFlat.GetAlteration(NoteName.F).ShouldBe(Alteration.Flat);
        KeySignature.Parse("C").GetAlteration(NoteName.F).ShouldBe(Alteration.Natural);
    }

    [Fact]
    public void KeySignature_EqualityIgnoresTheTonicOctave()
    {
        var a = new KeySignature(new Note(NoteName.E, Alteration.Flat, 2), KeyMode.Minor);
        var b = KeySignature.Parse("Ebm");
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
        a.Equals(null).ShouldBeFalse();
        a.ShouldNotBe(KeySignature.Parse("Eb"));
        a.ShouldNotBe(KeySignature.Parse("Em"));
        a.ShouldNotBe(KeySignature.Parse("D#m"));
    }

    [Theory]
    [InlineData("C#", "A#m")] [InlineData("Cb", "Abm")] [InlineData("A#m", "C#")] [InlineData("Bbm", "Db")]
    public void KeySignature_GetRelative_SpellsByInterval(string key, string relative) =>
        KeySignature.Parse(key).GetRelative().ShouldBe(KeySignature.Parse(relative));

    [Theory]
    [InlineData("B", "Cb")] [InlineData("Cb", "B")] [InlineData("F#", "Gb")] [InlineData("Gb", "F#")]
    [InlineData("C#", "Db")] [InlineData("Db", "C#")]
    [InlineData("G#m", "Abm")] [InlineData("Abm", "G#m")] [InlineData("D#m", "Ebm")] [InlineData("A#m", "Bbm")] [InlineData("Bbm", "A#m")]
    public void KeySignature_GetEnharmonicEquivalents(string key, string equivalent) =>
        KeySignature.Parse(key).GetEnharmonicEquivalents().ShouldBe([KeySignature.Parse(equivalent)]);

    [Theory]
    [InlineData("C")] [InlineData("E")] [InlineData("Am")] [InlineData("Bb")]
    public void KeySignature_GetEnharmonicEquivalents_IsEmptyAwayFromTheEdges(string key) =>
        KeySignature.Parse(key).GetEnharmonicEquivalents().ShouldBeEmpty();

    [Theory]
    [InlineData("Eb", "Eb", KeyMode.Major)] [InlineData("E♭ minor", "Eb", KeyMode.Minor)] [InlineData("F#m", "F#", KeyMode.Minor)]
    [InlineData("C major", "C", KeyMode.Major)] [InlineData("CM", "C", KeyMode.Major)] [InlineData("c min", "C", KeyMode.Minor)]
    [InlineData(" g maj ", "G", KeyMode.Major)]
    public void KeySignature_Parse(string text, string tonic, KeyMode mode) =>
        KeySignature.Parse(text).ShouldBe(new KeySignature(Note.Parse(tonic), mode));

    [Theory]
    [InlineData("")] [InlineData(null)] [InlineData("X")] [InlineData("C dorian")] [InlineData("G#")] [InlineData("Fbm")] [InlineData("Cbbm")]
    public void KeySignature_TryParse_RejectsUnknownAndTheoreticalKeys(string? text)
    {
        KeySignature.TryParse(text, out var key).ShouldBeFalse();
        key.ShouldBeNull();
    }

    [Fact]
    public void KeySignature_Parse_ThrowsFormatException() =>
        Should.Throw<FormatException>(() => KeySignature.Parse("H")).Message.ShouldBe("Not a key: 'H'.");

    [Theory]
    [InlineData("E", KeyMode.Major, 3, "D#")]   // a scale note
    [InlineData("E", KeyMode.Major, 10, "A#")]  // a black key outside the scale, sharp key: sharp
    [InlineData("Eb", KeyMode.Major, 4, "E")]   // a white key outside the scale
    [InlineData("Eb", KeyMode.Major, 6, "Gb")]  // a black key outside the scale, flat key: flat
    [InlineData("C", KeyMode.Major, 6, "F#")]   // C major sharpens only F
    [InlineData("C", KeyMode.Major, 1, "Db")]
    [InlineData("A", KeyMode.Minor, 8, "G#")]   // the raised 7th of A minor
    [InlineData("A", KeyMode.Minor, 6, "F#")]   // the raised 6th
    [InlineData("D##", KeyMode.Minor, 3, "D#")] // the raised 7th would be C###: the black-key rule spells it
    public void KeySignature_Spell(string tonic, KeyMode mode, int pitchClass, string expected) =>
        new KeySignature(Note.Parse(tonic), mode).Spell(pitchClass).GetName().ShouldBe(expected);

    [Fact]
    public void Scale_Validates()
    {
        Should.Throw<ArgumentNullException>(() => new Scale(null!, ScaleType.Major));
        Should.Throw<ArgumentOutOfRangeException>(() => new Scale(new Note(NoteName.C), (ScaleType)99)).ParamName.ShouldBe("type");
        var c = new Scale(new Note(NoteName.C), ScaleType.Major);
        Should.Throw<ArgumentNullException>(() => c.Contains(null!));
        Should.Throw<ArgumentNullException>(() => c.GetDegree(null!));
        Should.Throw<ArgumentNullException>(() => c.GetNextNoteInScale(null!));
    }

    [Fact]
    public void Scale_ValueSemantics()
    {
        var a = new Scale(Note.Parse("D4"), ScaleType.Dorian);
        a.ShouldBe(new Scale(new Note(NoteName.D), ScaleType.Dorian));
        a.ToString().ShouldBe("D4 Dorian");
        a.Intervals.Select(i => i.Name).ShouldBe(["P1", "M2", "m3", "P4", "P5", "M6", "m7", "P8"]);
        a.Transpose(Interval.Parse("M-2")).ShouldBe(new Scale(Note.Parse("C4"), ScaleType.Dorian));
    }

    [Theory]
    [InlineData("Db4", ScaleType.PentatonicMajor, "Db4 Eb4 F4 Ab4 Bb4 Db5")]
    [InlineData("Eb4", ScaleType.WholeTone, "Eb4 F4 G4 A4 B4 C#5 Eb5")]
    [InlineData("F4", ScaleType.Blues, "F4 Ab4 Bb4 B4 C5 Eb5 F5")]
    [InlineData("Bb4", ScaleType.Chromatic, "Bb4 B4 C5 C#5 D5 Eb5 E5 F5 F#5 G5 G#5 A5 Bb5")]
    public void Scale_SpellsEveryTypeByInterval(string root, ScaleType type, string expected) =>
        string.Join(" ", new Scale(Note.Parse(root), type).GetNotes()).ShouldBe(expected);

    [Fact]
    public void Scale_GetNextNoteInScale_FollowsTheGivenOctave()
    {
        var c = new Scale(new Note(NoteName.C), ScaleType.Major);
        c.GetNextNoteInScale(Note.Parse("B2")).ShouldBe(Note.Parse("C3"));
        c.GetNextNoteInScale(Note.Parse("E6")).ShouldBe(Note.Parse("F6"));
        c.GetDegree(Note.Parse("C7")).ShouldBe(1);
        c.GetDegree(Note.Parse("C#4")).ShouldBeNull();
    }

    [Fact]
    public void Scale_GetNotes_RefusesUnspellableTonics() =>
        Should.Throw<InvalidOperationException>(() => new Scale(Note.Parse("B##4"), ScaleType.Major).GetNotes());
}
