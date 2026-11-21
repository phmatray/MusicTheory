namespace MusicTheory.Guitar.UnitTests;

public class FretboardTests
{
    private static Fretboard Neck(int frets = 12) => new(Tuning.Standard, frets);

    private static (int, int, string)[] Spell(IEnumerable<FretPosition> ps) => ps.Select(p => (p.String, p.Fret, p.Note.ToString())).ToArray();

    [Fact]
    public void NoteAt_NamesFrets()
    {
        Neck().NoteAt(1, 3).ShouldBe(Note.Parse("C3"));
        Neck().NoteAt(0, 2).ShouldBe(Note.Parse("F#2"));
    }

    [Theory]
    [InlineData(0, 13)]
    [InlineData(0, -1)]
    [InlineData(6, 0)]
    [InlineData(-1, 0)]
    public void NoteAt_OffTheBoard_Throws(int s, int f) => Should.Throw<ArgumentOutOfRangeException>(() => Neck().NoteAt(s, f));

    [Fact]
    public void Constructor_Validates()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Fretboard(Tuning.Standard, 0));
        Should.Throw<ArgumentOutOfRangeException>(() => new Fretboard(Tuning.Standard, 64));
        Should.Throw<ArgumentNullException>(() => new Fretboard(null!));
        Should.Throw<ArgumentOutOfRangeException>(() => Neck() with { Frets = 0 });
        Should.Throw<ArgumentNullException>(() => Neck() with { Tuning = null! });
        (Neck() with { Frets = 5 }).Frets.ShouldBe(5);
        (Neck() with { Tuning = Tuning.Bass }).Tuning.ShouldBe(Tuning.Bass);
        new Fretboard(Tuning.Standard).Frets.ShouldBe(24);
    }

    [Fact]
    public void Positions_Note()
    {
        var ps = Neck().Positions(Note.Parse("A"));
        Spell(ps).ShouldBe([(0, 5, "A2"), (1, 0, "A2"), (1, 12, "A3"), (2, 7, "A3"), (3, 2, "A3"), (4, 10, "A4"), (5, 5, "A4")]);
        ps.ShouldAllBe(p => p.Degree == null);
    }

    [Fact]
    public void Positions_Scale_SpelledAsScale()
    {
        var ps = Neck().Positions(new Scale(Note.Parse("D"), ScaleType.Major));
        var p = ps.Single(x => x.String == 0 && x.Fret == 2);
        p.Note.ShouldBe(Note.Parse("F#2"));
        p.Degree.ShouldBe(3);
        ps.ShouldNotContain(x => x.Note.Name == NoteName.G && x.Note.Alteration == Alteration.Flat);
    }

    [Fact]
    public void Positions_Chord_DegreesAreIntervalNumbers()
    {
        var ps = Neck().Positions(new Chord(Note.Parse("G"), ChordType.Dominant7));
        ps.Single(x => x is { String: 0, Fret: 3 }).ShouldBe(new FretPosition(0, 3, Note.Parse("G2"), 1));
        ps.Single(x => x is { String: 0, Fret: 1 }).ShouldBe(new FretPosition(0, 1, Note.Parse("F2"), 7));
        ps.Select(x => x.Degree).Distinct().Order().ShouldBe([1, 3, 5, 7]);
        ps.Select(x => x.Note.PitchClass).Distinct().Order().ShouldBe([2, 5, 7, 11]);
    }

    [Fact]
    public void Positions_BassPentatonic_ThreePerString()
    {
        var bass = new Fretboard(Tuning.Bass, 5);
        bass.Positions(new Scale(Note.Parse("A"), ScaleType.PentatonicMinor)).Count.ShouldBe(12);
    }

    [Fact]
    public void Positions_NullArguments_Throw()
    {
        Should.Throw<ArgumentNullException>(() => Neck().Positions((Note)null!));
        Should.Throw<ArgumentNullException>(() => Neck().Positions((Scale)null!));
        Should.Throw<ArgumentNullException>(() => Neck().Positions((Chord)null!));
    }
}
