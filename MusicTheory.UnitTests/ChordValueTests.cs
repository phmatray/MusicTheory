namespace MusicTheory.UnitTests;

/// <summary>Version 2 behaviour of <see cref="Chord"/>: immutability, extensions, spelling, equality, symbols.</summary>
public class ChordValueTests
{
    private static readonly Note C4 = new(NoteName.C);

    [Theory]
    [InlineData(ChordType.Major, ChordQuality.Major)] [InlineData(ChordType.Dominant7, ChordQuality.Major)]
    [InlineData(ChordType.Dominant7Flat5, ChordQuality.Major)] [InlineData(ChordType.Dominant7Alt, ChordQuality.Major)]
    [InlineData(ChordType.Minor7, ChordQuality.Minor)] [InlineData(ChordType.MinorMajor7, ChordQuality.Minor)]
    [InlineData(ChordType.HalfDiminished7, ChordQuality.Diminished)] [InlineData(ChordType.Diminished7, ChordQuality.Diminished)]
    [InlineData(ChordType.Augmented7, ChordQuality.Augmented)] [InlineData(ChordType.AugmentedMajor7, ChordQuality.Augmented)]
    [InlineData(ChordType.Sus2, ChordQuality.Suspended)] [InlineData(ChordType.Dominant7Sus4, ChordQuality.Suspended)]
    [InlineData(ChordType.Power5, ChordQuality.Power)]
    public void Quality_FollowsTheTriad(ChordType type, ChordQuality quality) =>
        new Chord(C4, type).Quality.ShouldBe(quality);

    [Theory]
    [InlineData(ChordQuality.Major, ChordType.Major)] [InlineData(ChordQuality.Minor, ChordType.Minor)]
    [InlineData(ChordQuality.Diminished, ChordType.Diminished)] [InlineData(ChordQuality.Augmented, ChordType.Augmented)]
    [InlineData(ChordQuality.Suspended, ChordType.Sus4)] [InlineData(ChordQuality.Power, ChordType.Power5)]
    public void QualityConstructor_BuildsTheMatchingTriad(ChordQuality quality, ChordType type) =>
        new Chord(C4, quality).ShouldBe(new Chord(C4, type));

    [Theory]
    [InlineData("F#4", ChordType.Major, "F#4 A#4 C#5")]
    [InlineData("Bb3", ChordType.Dominant7, "Bb3 D4 F4 Ab4")]
    [InlineData("E3", ChordType.Dominant7Sharp9, "E3 G#3 B3 D4 F##4")]
    [InlineData("C4", ChordType.Diminished7, "C4 Eb4 Gb4 Bbb4")]
    [InlineData("G4", ChordType.Dominant13, "G4 B4 D5 F5 A5 C6 E6")]
    [InlineData("Db4", ChordType.Dominant7Alt, "Db4 F4 Abb4 A4 Cb5 Ebb5")]
    public void GetNotes_SpellsByInterval(string root, ChordType type, string expected) =>
        string.Join(" ", new Chord(Note.Parse(root), type).GetNotes()).ShouldBe(expected);

    [Fact]
    public void AddExtension_ReturnsANewChord_AndLeavesTheOriginalAlone()
    {
        var triad = new Chord(C4, ChordQuality.Major);
        var seventh = triad.AddExtension(7, IntervalQuality.Minor);

        ReferenceEquals(triad, seventh).ShouldBeFalse();
        string.Join(" ", triad.GetNotes()).ShouldBe("C4 E4 G4");
        string.Join(" ", seventh.GetNotes()).ShouldBe("C4 E4 G4 Bb4");
        triad.Extensions.ShouldBeEmpty();
        seventh.Extensions.ShouldBe([Interval.Parse("m7")]);
    }

    [Fact]
    public void AddExtension_ReplacesTheDegreeItAlters()
    {
        var c9 = new Chord(C4, ChordType.Dominant9);
        c9.AddExtension(9, IntervalQuality.Minor).Intervals.ShouldBe(Of("M3 P5 m7 m9"));
        new Chord(C4, ChordType.Dominant7).AddExtension(5, IntervalQuality.Diminished).Intervals.ShouldBe(Of("M3 d5 m7"));
        new Chord(C4, ChordType.Dominant7Alt).AddExtension(5, IntervalQuality.Perfect).Intervals.ShouldBe(Of("M3 P5 m7 m9"));
        new Chord(C4, ChordType.Major).AddExtension(3, IntervalQuality.Minor).Intervals.ShouldBe(Of("m3 P5"));
        new Chord(C4, ChordType.Major6).AddExtension(9, IntervalQuality.Major).Intervals.ShouldBe(Of("M3 P5 M6 M9"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void AddExtension_RejectsAUnisonOrLess(int number) =>
        Should.Throw<ArgumentOutOfRangeException>(() => new Chord(C4, ChordType.Major).AddExtension(number, IntervalQuality.Perfect));

    [Fact]
    public void AddExtension_RejectsAnImpossibleQuality() =>
        Should.Throw<ArgumentOutOfRangeException>(() => new Chord(C4, ChordType.Major).AddExtension(9, IntervalQuality.Perfect));

    [Theory]
    [InlineData(ChordType.Major, IntervalQuality.Minor, 7, "C7")]
    [InlineData(ChordType.Major, IntervalQuality.Major, 7, "Cmaj7")]
    [InlineData(ChordType.Dominant7, IntervalQuality.Minor, 9, "C7b9")]
    [InlineData(ChordType.Dominant7, IntervalQuality.Augmented, 11, "C7#11")]
    [InlineData(ChordType.Major7, IntervalQuality.Augmented, 11, "Cmaj7#11")]
    [InlineData(ChordType.Dominant7, IntervalQuality.Major, 13, "C7add13")]
    public void GetSymbol_FoldsExtensionsIntoTheType_OrWritesThemAsTensions(ChordType type, IntervalQuality quality, int number, string expected) =>
        new Chord(C4, type).AddExtension(number, quality).GetSymbol().ShouldBe(expected);

    [Fact]
    public void ChordWithoutASymbol_ExplainsItself()
    {
        var odd = new Chord(C4, ChordType.Major).AddExtension(2, IntervalQuality.Major);
        Should.Throw<InvalidOperationException>(() => odd.ToChordSymbol())
            .Message.ShouldBe("C4 (M3 P5 M2) has no lead-sheet symbol: its extensions are not tensions.");
        odd.ToString().ShouldBe("C4 (M3 P5 M2)");
    }

    [Fact]
    public void Transpose_KeepsTypeExtensionsAndInversion()
    {
        var chord = new Chord(C4, ChordType.Dominant7).AddExtension(9, IntervalQuality.Minor).WithInversion(ChordInversion.First);
        var up = chord.Transpose(Interval.Parse("M2"));
        up.Root.ShouldBe(Note.Parse("D4"));
        up.Inversion.ShouldBe(ChordInversion.First);
        up.GetSymbol().ShouldBe("D7b9/F#");
        chord.Transpose(Interval.Parse("m-3")).GetSymbol().ShouldBe("A7b9/C#");
    }

    [Fact]
    public void Inversions()
    {
        var c7 = new Chord(C4, ChordType.Dominant7);
        c7.WithInversion(ChordInversion.Third).GetBassNote().ShouldBe(Note.Parse("Bb4"));
        string.Join(" ", c7.WithInversion(ChordInversion.Second).GetNotesInInversion()).ShouldBe("G4 Bb4 C5 E5");
        string.Join(" ", c7.GetNotesInInversion()).ShouldBe("C4 E4 G4 Bb4");
        c7.GetSlashChordNotation().ShouldBe("C");
        c7.WithInversion(ChordInversion.First).GetSlashChordNotation().ShouldBe("C/E");
        c7.WithInversion(ChordInversion.Third).ToString().ShouldBe("C7/Bb");
    }

    [Fact]
    public void GetEnharmonicEquivalent()
    {
        new Chord(Note.Parse("C#4"), ChordType.Minor7).GetEnharmonicEquivalent()!.GetSymbol().ShouldBe("Dbm7");
        new Chord(Note.Parse("D4"), ChordType.Minor7).GetEnharmonicEquivalent().ShouldBeNull();
    }

    [Fact]
    public void Equality_IsByValue()
    {
        var a = new Chord(C4, ChordType.Major).AddExtension(7, IntervalQuality.Minor);
        var b = new Chord(Note.Parse("C4"), ChordType.Major).AddExtension(7, IntervalQuality.Minor);
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
        (a == b).ShouldBeTrue();
        a.Equals(null).ShouldBeFalse();
        a.ShouldNotBe(new Chord(C4, ChordType.Dominant7));                         // same notes, different construction
        a.ShouldNotBe(a.WithInversion(ChordInversion.First));
        a.ShouldNotBe(new Chord(C4, ChordType.Minor).AddExtension(7, IntervalQuality.Minor));
        a.ShouldNotBe(new Chord(Note.Parse("D4"), ChordType.Major).AddExtension(7, IntervalQuality.Minor));
        a.ShouldNotBe(new Chord(C4, ChordType.Major).AddExtension(7, IntervalQuality.Major));
    }

    [Fact]
    public void GetSymbol_Styles()
    {
        var chord = new Chord(Note.Parse("Bb3"), ChordType.HalfDiminished7);
        chord.GetSymbol().ShouldBe("Bbm7b5");
        chord.GetSymbol(ChordSymbolStyle.Unicode).ShouldBe("B♭ø7");
    }

    private static Interval[] Of(string names) => names.Split(' ').Select(Interval.Parse).ToArray();
}
