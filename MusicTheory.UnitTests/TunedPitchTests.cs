using Shouldly;

namespace MusicTheory.UnitTests;

public class TunedPitchTests
{
    private static readonly Note C4 = Note.Parse("C4");

    [Fact]
    public void GetFrequency_applies_the_cents_offset()
    {
        new TunedPitch(C4, 50).GetFrequency().ShouldBe(C4.GetFrequency() * Math.Pow(2, 50 / 1200.0), 1e-9);
        new TunedPitch(Note.Parse("A4"), 0).GetFrequency(442).ShouldBe(442, 1e-9);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(-100)]
    [InlineData(double.NaN)]
    public void Constructor_rejects_cents_outside_open_interval(double cents) =>
        Should.Throw<ArgumentOutOfRangeException>(() => new TunedPitch(C4, cents));

    [Fact]
    public void Constructor_rejects_null_note() =>
        Should.Throw<ArgumentNullException>(() => new TunedPitch(null!, 0));

    [Fact]
    public void CentsBetween_measures_in_cents()
    {
        TunedPitch.CentsBetween(440, 880).ShouldBe(1200, 1e-9);
        TunedPitch.CentsBetween(440, 440).ShouldBe(0, 1e-9);
    }

    [Theory]
    [InlineData(0, 440)]
    [InlineData(440, 0)]
    [InlineData(double.NaN, 440)]
    [InlineData(440, double.NaN)]
    public void CentsBetween_rejects_bad_frequencies(double from, double to) =>
        Should.Throw<ArgumentOutOfRangeException>(() => TunedPitch.CentsBetween(from, to));

    [Fact]
    public void ToString_shows_signed_cents()
    {
        new TunedPitch(Note.Parse("A4"), 23.44).ToString().ShouldBe("A4 +23.4¢");
        new TunedPitch(Note.Parse("A4"), -12.34).ToString().ShouldBe("A4 -12.3¢");
        new TunedPitch(Note.Parse("A4"), -0.01).ToString().ShouldBe("A4 +0.0¢");
    }

    [Fact]
    public void FromFrequency_reads_concert_pitch()
    {
        var r = Note.FromFrequency(440);
        r.Note.ToString().ShouldBe("A4");
        r.Cents.ShouldBe(0, 1e-9);
    }

    [Fact]
    public void FromFrequency_reads_cents_sharp()
    {
        var r = Note.FromFrequency(446);
        r.Note.ToString().ShouldBe("A4");
        r.Cents.ShouldBe(23.45, 0.01);
    }

    [Fact]
    public void FromFrequency_honours_a4_and_spelling()
    {
        var sharp = Note.FromFrequency(440, a4: 415);
        sharp.Note.ToString().ShouldBe("A#4");
        sharp.Cents.ShouldBe(1.27, 0.01);
        Note.FromFrequency(440, a4: 415, preferFlats: true).Note.ToString().ShouldBe("Bb4");
        Note.FromFrequency(440, preferFlats: true).Note.ToString().ShouldBe("A4");
    }

    [Fact]
    public void FromFrequency_reads_middle_c()
    {
        var r = Note.FromFrequency(261.6256);
        r.Note.ToString().ShouldBe("C4");
        Math.Abs(r.Cents).ShouldBeLessThan(0.01);
    }

    [Fact]
    public void FromFrequency_rounds_an_exact_quarter_tone_up_to_minus_fifty()
    {
        var r = Note.FromFrequency(440 * Math.Pow(2, 0.5 / 12));
        r.Note.ToString().ShouldBe("A#4");
        r.Cents.ShouldBe(-50, 1e-9);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(20000)]
    [InlineData(1)]
    public void FromFrequency_rejects_unreadable_frequencies(double hz) =>
        Should.Throw<ArgumentOutOfRangeException>(() => Note.FromFrequency(hz));

    [Theory]
    [InlineData(0)]
    [InlineData(double.NaN)]
    public void FromFrequency_rejects_a_bad_a4(double a4) =>
        Should.Throw<ArgumentOutOfRangeException>(() => Note.FromFrequency(440, a4: a4));
}
