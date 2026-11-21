namespace MusicTheory.UnitTests;

/// <summary>Version 2 behaviour of <see cref="Note"/>: validation, value equality, tuning, respelling, German names.</summary>
public class NoteValueTests
{
    [Fact]
    public void Constructor_RejectsUndefinedValuesAndOctavesOutOfRange()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Note((NoteName)7)).ParamName.ShouldBe("name");
        Should.Throw<ArgumentOutOfRangeException>(() => new Note(NoteName.C, (Alteration)3)).ParamName.ShouldBe("alteration");
        Should.Throw<ArgumentOutOfRangeException>(() => new Note(NoteName.C, Alteration.Natural, -2)).ParamName.ShouldBe("octave");
        Should.Throw<ArgumentOutOfRangeException>(() => new Note(NoteName.C, Alteration.Natural, 10)).ParamName.ShouldBe("octave");
        new Note(NoteName.C, Alteration.Natural, Note.MinOctave).Octave.ShouldBe(-1);
        new Note(NoteName.G, Alteration.Natural, Note.MaxOctave).MidiNumber.ShouldBe(127);
    }

    [Fact]
    public void Equality_IsByValue()
    {
        var a = new Note(NoteName.C, Alteration.Sharp, 4);
        var b = Note.Parse("C#4");
        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
        new HashSet<Note> { a, b }.Count.ShouldBe(1);
        a.ShouldNotBe(new Note(NoteName.D, Alteration.Flat, 4));
        a.ShouldNotBe(new Note(NoteName.C, Alteration.Sharp, 5));
    }

    [Theory]
    [InlineData("C4", 0)]
    [InlineData("B#3", 0)]
    [InlineData("Cb4", 11)]
    [InlineData("Dbb4", 0)]
    [InlineData("F##2", 7)]
    public void PitchClass_IgnoresOctaveAndWraps(string note, int pitchClass) =>
        Note.Parse(note).PitchClass.ShouldBe(pitchClass);

    [Theory]
    [InlineData(440.0, 440.0)]
    [InlineData(442.0, 442.0)]
    [InlineData(415.0, 415.0)]
    public void GetFrequency_FollowsTheConcertPitch(double a4, double expectedA4)
    {
        new Note(NoteName.A).GetFrequency(a4).ShouldBe(expectedA4, 1e-9);
        new Note(NoteName.A, Alteration.Natural, 5).GetFrequency(a4).ShouldBe(2 * expectedA4, 1e-9);
        new Note(NoteName.C).GetFrequency(a4).ShouldBe(expectedA4 * Math.Pow(2, -9 / 12.0), 1e-9);
    }

    [Fact]
    public void Frequency_IsConcertPitch()
    {
        new Note(NoteName.A).Frequency.ShouldBe(Note.StandardA4);
        new Note(NoteName.C).Frequency.ShouldBe(261.6255653005986, 1e-9);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-440.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void GetFrequency_RejectsNonPositiveOrNonFiniteA4(double a4) =>
        Should.Throw<ArgumentOutOfRangeException>(() => new Note(NoteName.A).GetFrequency(a4)).ParamName.ShouldBe("a4");

    [Theory]
    [InlineData("C4", 1, "C#4")]
    [InlineData("C4", 3, "D#4")]
    [InlineData("E4", 1, "F4")]
    [InlineData("B4", 1, "C5")]
    [InlineData("C4", -1, "B3")]
    [InlineData("C4", -13, "B2")]
    [InlineData("Cb4", 0, "B3")]
    [InlineData("A4", 13, "A#5")]
    public void TransposeBySemitones_SpellsBlackKeysWithSharps(string from, int semitones, string expected) =>
        Note.Parse(from).TransposeBySemitones(semitones).ShouldBe(Note.Parse(expected));

    [Theory]
    [InlineData("C4", NoteName.B, "B#3")]
    [InlineData("C4", NoteName.D, "Dbb4")]
    [InlineData("C4", NoteName.C, "C4")]
    [InlineData("B3", NoteName.C, "Cb4")]
    [InlineData("F#4", NoteName.G, "Gb4")]
    [InlineData("A##4", NoteName.B, "B4")]
    public void Respell_KeepsThePitch(string note, NoteName letter, string expected)
    {
        var respelled = Note.Parse(note).Respell(letter);
        respelled.ShouldBe(Note.Parse(expected));
        respelled!.IsEnharmonicWith(Note.Parse(note)).ShouldBeTrue();
    }

    [Theory]
    [InlineData("C4", NoteName.E)]
    [InlineData("C4", NoteName.G)]
    [InlineData("C-1", NoteName.B)]
    [InlineData("B9", NoteName.C)]
    public void Respell_ReturnsNull_WhenTheSpellingIsImpossible(string note, NoteName letter) =>
        Note.Parse(note).Respell(letter).ShouldBeNull();

    [Fact]
    public void Respell_RejectsAnUndefinedLetter() =>
        Should.Throw<ArgumentOutOfRangeException>(() => new Note(NoteName.C).Respell((NoteName)9)).ParamName.ShouldBe("letter");

    [Theory]
    [InlineData("C#4", "Db4")] [InlineData("Db4", "C#4")] [InlineData("D#4", "Eb4")] [InlineData("Eb4", "D#4")]
    [InlineData("F#4", "Gb4")] [InlineData("Gb4", "F#4")] [InlineData("G#4", "Ab4")] [InlineData("Ab4", "G#4")]
    [InlineData("A#4", "Bb4")] [InlineData("Bb4", "A#4")]
    [InlineData("E4", "Fb4")] [InlineData("F4", "E#4")] [InlineData("B4", "Cb5")] [InlineData("C4", "B#3")]
    [InlineData("E#4", "F4")] [InlineData("Fb4", "E4")] [InlineData("B#4", "C5")] [InlineData("Cb4", "B3")]
    [InlineData("C##4", "D4")] [InlineData("D##4", "E4")] [InlineData("E##4", "F#4")] [InlineData("F##4", "G4")]
    [InlineData("G##4", "A4")] [InlineData("A##4", "B4")] [InlineData("B##4", "C#5")]
    [InlineData("Cbb4", "Bb3")] [InlineData("Dbb4", "C4")] [InlineData("Ebb4", "D4")] [InlineData("Fbb4", "Eb4")]
    [InlineData("Gbb4", "F4")] [InlineData("Abb4", "G4")] [InlineData("Bbb4", "A4")]
    public void GetEnharmonicEquivalent_GivesTheUsualAlternative(string note, string expected) =>
        Note.Parse(note).GetEnharmonicEquivalent().ShouldBe(Note.Parse(expected));

    [Theory]
    [InlineData("D4")] [InlineData("G4")] [InlineData("A4")]
    [InlineData("B9")] [InlineData("C-1")]
    public void GetEnharmonicEquivalent_IsNull_ForDGA_AndAtTheRangeEdges(string note) =>
        Note.Parse(note).GetEnharmonicEquivalent().ShouldBeNull();

    [Theory]
    [InlineData("E#4", "F4")] [InlineData("Fb4", "E4")] [InlineData("B#4", "C5")] [InlineData("Cb4", "B3")]
    [InlineData("C##4", "D4")] [InlineData("Dbb4", "C4")]
    [InlineData("C#4", "C#4")] [InlineData("Db4", "Db4")] [InlineData("C4", "C4")] [InlineData("E4", "E4")]
    [InlineData("B#9", "B#9")] [InlineData("Cb-1", "Cb-1")]
    public void SimplifyEnharmonic_RemovesOnlyAwkwardSpellings(string note, string expected) =>
        Note.Parse(note).SimplifyEnharmonic().ShouldBe(Note.Parse(expected));

    [Fact]
    public void GetAllEnharmonicEquivalents_CrossesTheOctaveWhenTheLetterDoes()
    {
        Note.Parse("C4").GetAllEnharmonicEquivalents().ShouldBe([Note.Parse("Dbb4"), Note.Parse("B#3")], ignoreOrder: true);
        Note.Parse("G#4").GetAllEnharmonicEquivalents().ShouldBe([Note.Parse("Ab4")]);
    }

    [Theory]
    [InlineData("c", "C4")] [InlineData("cis", "C#4")] [InlineData("cisis", "C##4")] [InlineData("ces", "Cb4")] [InlineData("ceses", "Cbb4")]
    [InlineData("d", "D4")] [InlineData("dis", "D#4")] [InlineData("disis", "D##4")] [InlineData("des", "Db4")] [InlineData("deses", "Dbb4")]
    [InlineData("e", "E4")] [InlineData("eis", "E#4")] [InlineData("eisis", "E##4")] [InlineData("es", "Eb4")] [InlineData("eses", "Ebb4")]
    [InlineData("f", "F4")] [InlineData("fis", "F#4")] [InlineData("fisis", "F##4")] [InlineData("fes", "Fb4")] [InlineData("feses", "Fbb4")]
    [InlineData("g", "G4")] [InlineData("gis", "G#4")] [InlineData("gisis", "G##4")] [InlineData("ges", "Gb4")] [InlineData("geses", "Gbb4")]
    [InlineData("a", "A4")] [InlineData("ais", "A#4")] [InlineData("aisis", "A##4")] [InlineData("as", "Ab4")] [InlineData("asas", "Abb4")]
    [InlineData("h", "B4")] [InlineData("his", "B#4")] [InlineData("hisis", "B##4")] [InlineData("b", "Bb4")] [InlineData("hes", "Bb4")] [InlineData("heses", "Bbb4")]
    [InlineData("Fis3", "F#3")] [InlineData("H2", "B2")] [InlineData("ES5", "Eb5")] [InlineData(" B ", "Bb4")]
    public void Parse_German(string text, string expected) =>
        Note.Parse(text, NoteNotation.German).ShouldBe(Note.Parse(expected));

    [Theory]
    [InlineData("x")] [InlineData("")] [InlineData("cisx")] [InlineData("H10")] [InlineData(null)]
    public void TryParse_German_RejectsNonNotes(string? text)
    {
        Note.TryParse(text, NoteNotation.German, out var note).ShouldBeFalse();
        note.ShouldBeNull();
    }

    [Fact]
    public void Parse_German_ThrowsFormatException() =>
        Should.Throw<FormatException>(() => Note.Parse("x", NoteNotation.German)).Message.ShouldBe("Not a note in German notation: 'x'.");

    [Theory]
    [InlineData("C-1", "C-1")]
    [InlineData("c#4", "C#4")]
    [InlineData("B♭", "Bb4")]
    [InlineData("F𝄪3", "F##3")]
    [InlineData("E𝄫", "Ebb4")]
    [InlineData(" Bb3 ", "Bb3")]
    public void Parse_English_AcceptsCaseUnicodeAndSignedOctaves(string text, string expected) =>
        Note.Parse(text).ShouldBe(Note.Parse(expected));

    [Theory]
    [InlineData("C###")]
    [InlineData("Cbbb")]
    [InlineData("C#b")]
    [InlineData("C+4")]
    public void TryParse_English_RejectsMoreThanTwoAccidentalsOrMixedSigns(string text) =>
        Note.TryParse(text, out _).ShouldBeFalse();

    [Fact]
    public void TryParsePitchClass_ReportsWhatItRead()
    {
        Note.TryParsePitchClass("F#m7", out var note, out int consumed).ShouldBeTrue();
        note.ShouldBe(new Note(NoteName.F, Alteration.Sharp));
        consumed.ShouldBe(2);
        Note.TryParsePitchClass("", out note, out consumed).ShouldBeFalse();
        note.ShouldBeNull();
        consumed.ShouldBe(0);
        Note.TryParsePitchClass("x", out _, out _).ShouldBeFalse();
    }

    [Fact]
    public void Transpose_RejectsNullAndUnspellableResults()
    {
        Should.Throw<ArgumentNullException>(() => new Note(NoteName.C).Transpose(null!));
        Should.Throw<InvalidOperationException>(() => Note.Parse("B##4").Transpose(Interval.Parse("M2")))
            .Message.ShouldBe("B##4 transposed by M2 needs 3 sharps on C; at most two are supported.");
        Should.Throw<InvalidOperationException>(() => Note.Parse("Fbb4").Transpose(Interval.Parse("A-2")))
            .Message.ShouldBe("Fbb4 transposed by A-2 needs 4 flats on E; at most two are supported.");
        Should.Throw<ArgumentOutOfRangeException>(() => Note.Parse("C9").Transpose(Interval.Parse("P8")));
    }

    [Fact]
    public void TransposeBySemitones_InAKey_RejectsANullKey() =>
        Should.Throw<ArgumentNullException>(() => new Note(NoteName.C).TransposeBySemitones(1, null!));

    [Fact]
    public void ToStringAndGetName()
    {
        Note.Parse("F##-1").ToString().ShouldBe("F##-1");
        Note.Parse("Bb3").GetName(AccidentalStyle.Unicode).ShouldBe("B♭");
        Note.Parse("Ebb3").GetName(AccidentalStyle.Unicode).ShouldBe("E𝄫");
        Note.Parse("G##3").GetName(AccidentalStyle.Unicode).ShouldBe("G𝄪");
        Note.Parse("G#3").GetName(AccidentalStyle.Unicode).ShouldBe("G♯");
    }
}
