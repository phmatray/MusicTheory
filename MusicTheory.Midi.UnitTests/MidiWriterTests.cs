using MusicTheory.Midi;

namespace MusicTheory.Midi.UnitTests;

public class MidiWriterTests
{
    private static readonly Note C4 = Note.Parse("C4");
    private static readonly Note E4 = Note.Parse("E4");
    private static readonly Note G4 = Note.Parse("G4");
    private static Duration Q => new(DurationType.Quarter);

    private static RhythmEvent Ev(Duration d, bool tied = false, params Note[] notes) => new(notes, d, tied);

    private static Measure Bar(params RhythmEvent[] events) => new(TimeSignature.CommonTime, events);

    private static string Write(IReadOnlyList<Measure> measures, TempoMap? tempo = null, KeySignature? key = null, MidiWriteOptions? options = null)
    {
        using var ms = new MemoryStream();
        MidiWriter.Write(ms, measures, tempo ?? new TempoMap(120), key, options);
        return Convert.ToHexString(ms.ToArray());
    }

    private const string Header = "4D546864000000060001000201E0";

    [Fact]
    public void Header_IsFormat1WithTwoTracksAndDivision480() =>
        Write([Bar(Ev(Q, false, C4))]).ShouldStartWith(Header);

    [Fact]
    public void OneBarOfQuarter_WritesMetaTrackAndNoteTrack() =>
        Write([Bar(Ev(Q, false, C4))]).ShouldBe(
            Header +
            "4D54726B00000013" + "00FF58040402180800FF510307A12000FF2F00" +
            "4D54726B0000000E" + "00903C5A" + "8360803C00" + "8B20FF2F00");

    [Fact]
    public void TiedQuarters_MakeOneNote()
    {
        var hex = Write([Bar(Ev(Q, true, C4), Ev(Q, false, C4))]);
        hex.ShouldContain("00903C5A" + "8740803C00");
        hex.Split("903C5A").Length.ShouldBe(2);
    }

    [Fact]
    public void TieAtTheEndOfTheScore_StillClosesTheNote() =>
        Write([Bar(Ev(Q, true, C4))]).ShouldContain("8360803C00");

    [Fact]
    public void Chord_OpensEveryPitchAtOnce_AndRestWritesNothing()
    {
        var hex = Write([Bar(RhythmEvent.Rest(Q), Ev(Q, false, C4, E4, C4), RhythmEvent.Rest(Q))], options: new(480, 2, 100));
        hex.ShouldContain("8360923C6400924064");
        hex.ShouldContain("8360823C0000824000");
    }

    [Fact]
    public void TiedChord_KeepsSharedPitchesOpen()
    {
        var hex = Write([Bar(Ev(Q, true, C4, E4), Ev(Q, false, C4, G4))]);
        hex.ShouldContain("00903C5A0090405A");
        hex.ShouldContain("8360804000" + "0090435A");
        hex.ShouldContain("8360803C0000804300");
        hex.Split("903C5A").Length.ShouldBe(2);
    }

    [Fact]
    public void KeySignature_WritesSharpsAndMode()
    {
        Write([Bar(Ev(Q, false, C4))], key: KeySignature.Parse("D")).ShouldContain("00FF58040402180800FF59020200" + "00FF510307A120");
        Write([Bar(Ev(Q, false, C4))], key: new KeySignature(Note.Parse("D4"), KeyMode.Minor)).ShouldContain("00FF5902FF01");
    }

    [Fact]
    public void MeterAndTempoChanges_LandOnTheirTicks()
    {
        var bars = new[]
        {
            Bar(Ev(new Duration(DurationType.Whole), false, C4)),
            new Measure(new TimeSignature(3, 4), [Ev(new Duration(DurationType.Half, 1), false, C4)]),
        };
        var tempo = new TempoMap(120).WithChange(Rational.One, 60, new Duration(DurationType.Quarter, 1));
        Write(bars, tempo).ShouldContain(
            "4D54726B00000023" + "00FF58040402180800FF510307A120" +
            "8F00FF58040302180800FF51030A2C2B" + "00FF2F00");
    }

    [Fact]
    public void SeptupletNeedsADivisionDivisibleBySeven()
    {
        var seven = Enumerable.Repeat(Ev(Duration.CreateTuplet(DurationType.Quarter, 7, 4), false, C4), 7).ToArray();
        var ex = Should.Throw<ArgumentException>(() => Write([Bar(seven)]));
        ex.Message.ShouldContain("3360");
        Write([Bar(seven)], options: new(3360)).ShouldStartWith("4D54686400000006000100020D20");
    }

    [Fact]
    public void KeyWithEightSharps_Throws() =>
        Should.Throw<ArgumentException>(() => Write([Bar(Ev(Q, false, C4))], key: new KeySignature(Note.Parse("G#4"), KeyMode.Major)));

    [Fact]
    public void NoMeasures_Throws() =>
        Should.Throw<ArgumentException>(() => Write([]));

    [Theory]
    [InlineData(0, 0, 90)]
    [InlineData(40000, 0, 90)]
    [InlineData(480, -1, 90)]
    [InlineData(480, 16, 90)]
    [InlineData(480, 0, 0)]
    [InlineData(480, 0, 128)]
    public void OptionsOutOfRange_Throw(int ticks, int channel, int velocity) =>
        Should.Throw<ArgumentOutOfRangeException>(() => Write([Bar(Ev(Q, false, C4))], options: new(ticks, channel, velocity)));

    [Theory]
    [InlineData(1)]
    [InlineData(1e9)]
    public void TempoMidiCannotHold_Throws(double bpm) =>
        Should.Throw<ArgumentException>(() => Write([Bar(Ev(Q, false, C4))], new TempoMap(bpm)));

    [Fact]
    public void GapTooLongForADeltaTime_Throws() =>
        Should.Throw<ArgumentException>(() => Write([Bar(Ev(Q, false, C4))], new TempoMap(120).WithChange(Rational.Of(10_000_000), 90), options: new(32767)));

    [Fact]
    public void NullArguments_Throw()
    {
        var bars = new[] { Bar(Ev(Q, false, C4)) };
        Should.Throw<ArgumentNullException>(() => MidiWriter.Write(null!, bars, new TempoMap(120)));
        Should.Throw<ArgumentNullException>(() => MidiWriter.Write(new MemoryStream(), null!, new TempoMap(120)));
        Should.Throw<ArgumentNullException>(() => MidiWriter.Write(new MemoryStream(), bars, null!));
    }
}
