using Shouldly;

namespace MusicTheory.UnitTests;

public class TemperamentTests
{
    private static readonly Note C4 = Note.Parse("C4");

    private static double Ratio(Temperament t, string note, Note tonic) =>
        t.GetFrequency(Note.Parse(note), tonic) / t.GetFrequency(tonic, tonic);

    [Fact]
    public void Pythagorean_FifthAndSecond()
    {
        Ratio(Temperament.Pythagorean, "G4", C4).ShouldBe(1.5, 1e-9);
        Ratio(Temperament.Pythagorean, "D4", C4).ShouldBe(1.125, 1e-9);
    }

    [Fact]
    public void Just_ThirdSixthFifth()
    {
        Ratio(Temperament.JustIntonation, "E4", C4).ShouldBe(1.25, 1e-9);
        Ratio(Temperament.JustIntonation, "A4", C4).ShouldBe(5.0 / 3, 1e-9);
        Ratio(Temperament.JustIntonation, "G4", C4).ShouldBe(1.5, 1e-9);
    }

    [Theory]
    [InlineData("Db4", 256.0 / 243, 16.0 / 15)]
    [InlineData("Eb4", 32.0 / 27, 6.0 / 5)]
    [InlineData("E4", 81.0 / 64, 5.0 / 4)]
    [InlineData("F4", 4.0 / 3, 4.0 / 3)]
    [InlineData("F#4", 729.0 / 512, 45.0 / 32)]
    [InlineData("Ab4", 128.0 / 81, 8.0 / 5)]
    [InlineData("A4", 27.0 / 16, 5.0 / 3)]
    [InlineData("Bb4", 16.0 / 9, 9.0 / 5)]
    [InlineData("B4", 243.0 / 128, 15.0 / 8)]
    public void Pythagorean_AndJust_EveryDegree(string note, double pythagorean, double just)
    {
        Ratio(Temperament.Pythagorean, note, C4).ShouldBe(pythagorean, 1e-9);
        Ratio(Temperament.JustIntonation, note, C4).ShouldBe(just, 1e-9);
    }

    [Fact]
    public void Meantone_PureThirdAndNarrowFifth()
    {
        Ratio(Temperament.QuarterCommaMeantone, "E4", C4).ShouldBe(1.25, 1e-9);
        Ratio(Temperament.QuarterCommaMeantone, "G4", C4).ShouldBe(Math.Pow(5, 0.25), 1e-9);
    }

    [Fact]
    public void Meantone_OffsetsMatchReference()
    {
        double[] expected = [0, -23.95, -6.84, 10.26, -13.69, 3.42, -20.53, -3.42, -27.37, -10.26, 6.84, -17.11];
        for (var d = 0; d < 12; d++)
        {
            Temperament.QuarterCommaMeantone.OffsetCents(d).ShouldBe(expected[d], 0.01);
        }
    }

    [Theory]
    [InlineData("C4")]
    [InlineData("G4")]
    [InlineData("Eb4")]
    public void A4_IsAnchored(string tonic)
    {
        var tonicNote = Note.Parse(tonic);
        var a4 = Note.Parse("A4");
        Temperament.Equal.GetFrequency(a4, tonicNote, 415).ShouldBe(415, 1e-9);
        Temperament.Pythagorean.GetFrequency(a4, tonicNote, 415).ShouldBe(415, 1e-9);
        Temperament.JustIntonation.GetFrequency(a4, tonicNote, 415).ShouldBe(415, 1e-9);
        Temperament.QuarterCommaMeantone.GetFrequency(a4, tonicNote, 415).ShouldBe(415, 1e-9);
    }

    [Fact]
    public void Equal_MatchesNoteFrequency()
    {
        foreach (var tonic in new[] { C4, Note.Parse("F#3") })
        {
            for (var midi = 36; midi <= 96; midi++)
            {
                var n = Note.FromMidiNumber(midi);
                Temperament.Equal.GetFrequency(n, tonic).ShouldBe(n.GetFrequency(), 1e-9);
            }
        }
    }

    [Fact]
    public void TableMovesWithTonic() =>
        Ratio(Temperament.Pythagorean, "D5", Note.Parse("G4")).ShouldBe(1.5, 1e-9);

    [Fact]
    public void Validation()
    {
        Should.Throw<ArgumentNullException>(() => Temperament.Equal.GetFrequency(null!, C4));
        Should.Throw<ArgumentNullException>(() => Temperament.Equal.GetFrequency(C4, null!));
        Should.Throw<ArgumentOutOfRangeException>(() => Temperament.Equal.OffsetCents(12));
        Should.Throw<ArgumentOutOfRangeException>(() => Temperament.Equal.OffsetCents(-1));
    }

    [Fact]
    public void ToString_IsName() => Temperament.JustIntonation.ToString().ShouldBe(Temperament.JustIntonation.Name);
}
