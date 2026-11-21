namespace MusicTheory.UnitTests;

/// <summary>Version 2 behaviour of <see cref="Interval"/>: every quality, direction, names, parsing.</summary>
public class IntervalValueTests
{
    [Theory]
    [InlineData("P1", 0)] [InlineData("A1", 1)] [InlineData("AA1", 2)] [InlineData("d1", -1)] [InlineData("dd1", -2)]
    [InlineData("AAAA1", 4)] [InlineData("ddddd1", -5)]
    [InlineData("m2", 1)] [InlineData("M2", 2)] [InlineData("d2", 0)] [InlineData("dd2", -1)] [InlineData("A2", 3)] [InlineData("AA2", 4)]
    [InlineData("m3", 3)] [InlineData("M3", 4)] [InlineData("d3", 2)] [InlineData("A3", 5)]
    [InlineData("P4", 5)] [InlineData("A4", 6)] [InlineData("d4", 4)] [InlineData("AAAAA4", 10)]
    [InlineData("P5", 7)] [InlineData("d5", 6)] [InlineData("ddddd5", 2)] [InlineData("A5", 8)]
    [InlineData("m6", 8)] [InlineData("M6", 9)] [InlineData("m7", 10)] [InlineData("M7", 11)] [InlineData("d7", 9)]
    [InlineData("P8", 12)] [InlineData("d8", 11)] [InlineData("A8", 13)]
    [InlineData("m9", 13)] [InlineData("M9", 14)] [InlineData("A11", 18)] [InlineData("P12", 19)] [InlineData("M13", 21)] [InlineData("P15", 24)]
    public void Semitones_ForEveryQuality(string name, int semitones)
    {
        var interval = Interval.Parse(name);
        interval.Semitones.ShouldBe(semitones);
        interval.Name.ShouldBe(name);
        interval.ToString().ShouldBe(name);
    }

    [Theory]
    [InlineData("F##4", "Bbb4", "ddd4")]
    [InlineData("Fbb4", "B##4", "AAAAA4")]
    [InlineData("B##4", "Fbb5", "ddddd5")]
    [InlineData("Cbb4", "C##4", "AAAA1")]
    [InlineData("E##4", "Fbb4", "dddd2")]
    public void Between_ReachesQuintupleWhereMusic21Cannot(string from, string to, string expected)
    {
        var interval = Interval.Between(Note.Parse(from), Note.Parse(to));
        interval.ToString().ShouldBe(expected);
        Note.Parse(from).Transpose(interval).ShouldBe(Note.Parse(to));
    }

    [Fact]
    public void Between_NeverThrowsForTwoValidNotes()
    {
        var notes = Enum.GetValues<NoteName>()
            .SelectMany(n => Enum.GetValues<Alteration>().Select(a => new Note(n, a, 4)))
            .ToList();
        foreach (var a in notes)
            foreach (var b in notes)
                Should.NotThrow(() => a.Transpose(Interval.Between(a, b)).ShouldBe(b));
    }

    [Fact]
    public void Between_RejectsNull()
    {
        Should.Throw<ArgumentNullException>(() => Interval.Between(null!, new Note(NoteName.C)));
        Should.Throw<ArgumentNullException>(() => Interval.Between(new Note(NoteName.C), null!));
    }

    [Theory]
    [InlineData("M3", "M-3")] [InlineData("P-5", "P5")] [InlineData("A1", "d1")] [InlineData("d1", "A1")] [InlineData("P1", "P1")]
    [InlineData("AA1", "dd1")]
    public void Reverse_FlipsDirection_AndMirrorsUnisons(string name, string expected) =>
        Interval.Parse(name).Reverse().ShouldBe(Interval.Parse(expected));

    [Theory]
    [InlineData("M9", "M2")] [InlineData("P15", "P8")] [InlineData("P8", "P8")] [InlineData("M3", "M3")]
    [InlineData("A11", "A4")] [InlineData("m-10", "m-3")] [InlineData("P22", "P8")]
    public void ToSimple_ReducesToAnOctaveOrLess(string name, string expected) =>
        Interval.Parse(name).ToSimple().ShouldBe(Interval.Parse(expected));

    [Theory]
    [InlineData("M3", "m6")] [InlineData("m3", "M6")] [InlineData("P4", "P5")] [InlineData("A4", "d5")] [InlineData("d5", "A4")]
    [InlineData("AA2", "dd7")] [InlineData("dd7", "AA2")] [InlineData("P1", "P8")] [InlineData("P8", "P1")] [InlineData("d1", "A8")]
    [InlineData("M10", "m6")] [InlineData("M-3", "m-6")] [InlineData("P-8", "P1")]
    public void Invert(string name, string expected) =>
        Interval.Parse(name).Invert().ShouldBe(Interval.Parse(expected));

    [Fact]
    public void Flags()
    {
        Interval.Parse("M9").IsCompound.ShouldBeTrue();
        Interval.Parse("P8").IsCompound.ShouldBeFalse();
        Interval.Parse("AA4").IsAugmented.ShouldBeTrue();
        Interval.Parse("M3").IsAugmented.ShouldBeFalse();
        Interval.Parse("dd3").IsDiminished.ShouldBeTrue();
        Interval.Parse("m3").IsDiminished.ShouldBeFalse();
    }

    [Fact]
    public void IsEnharmonicWith_ComparesDirectedSemitones()
    {
        Interval.Parse("A4").IsEnharmonicWith(Interval.Parse("d5")).ShouldBeTrue();
        Interval.Parse("M3").IsEnharmonicWith(Interval.Parse("M-3")).ShouldBeFalse();
        Interval.Parse("M3").IsEnharmonicWith(null).ShouldBeFalse();
        Interval.Parse("M-3").DirectedSemitones.ShouldBe(-4);
    }

    [Fact]
    public void Equality_IsByValue_AndIncludesDirection()
    {
        Interval.Parse("M3").ShouldBe(new Interval(IntervalQuality.Major, 3));
        Interval.Parse("M3").ShouldNotBe(Interval.Parse("M-3"));
        new HashSet<Interval> { Interval.Parse("P5"), new Interval(IntervalQuality.Perfect, 5) }.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData("")] [InlineData(null)] [InlineData("M")] [InlineData("3")] [InlineData("X3")] [InlineData("Pm3")] [InlineData("dddddd5")]
    [InlineData("P3")] [InlineData("M5")] [InlineData("m1")] [InlineData("P0")] [InlineData("P-1")] [InlineData("M-")] [InlineData("M+3")]
    [InlineData("M 3")] [InlineData("M3x")] [InlineData("M99999999999")]
    public void TryParse_RejectsNonIntervals(string? text)
    {
        Interval.TryParse(text, out var interval).ShouldBeFalse();
        interval.ShouldBeNull();
    }

    [Fact]
    public void Parse_ThrowsFormatException_AndTrims()
    {
        Should.Throw<FormatException>(() => Interval.Parse("P3")).Message.ShouldBe("Not an interval: 'P3'.");
        Interval.Parse(" m-7 ").ShouldBe(new Interval(IntervalQuality.Minor, 7, Direction.Down));
    }
}
