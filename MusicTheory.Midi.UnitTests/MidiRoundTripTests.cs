using MusicTheory.Midi;

namespace MusicTheory.Midi.UnitTests;

public class MidiRoundTripTests
{
    private static readonly Note C4 = Note.Parse("C4");
    private static readonly Note E4 = Note.Parse("E4");
    private static readonly Note G4 = Note.Parse("G4");
    private static Duration Q => new(DurationType.Quarter);
    private static Duration H => new(DurationType.Half);

    private static RhythmEvent Ev(Duration d, bool tied = false, params Note[] notes) => new(notes, d, tied);

    private static MidiImport RoundTrip(IReadOnlyList<Measure> measures, TempoMap tempo, KeySignature? key = null, Rational? grid = null, MidiWriteOptions? options = null)
    {
        using var ms = new MemoryStream();
        MidiWriter.Write(ms, measures, tempo, key, options);
        ms.Position = 0;
        return MidiReader.Read(ms, grid);
    }

    private static MidiImport ReadHex(string hex, Rational? grid = null) =>
        MidiReader.Read(new MemoryStream(Convert.FromHexString(hex)), grid);

    private const string Hdr = "4D546864000000060000000101E0";

    private static string Trk(string body) => "4D54726B" + (body.Length / 2).ToString("X8") + body;

    private static void ShouldMatch(MidiImport import, IReadOnlyList<Measure> measures, TempoMap tempo)
    {
        import.TimeSignature.ShouldBe(measures[0].TimeSignature);
        import.Measures.Count.ShouldBe(measures.Count);
        for (int i = 0; i < measures.Count; i++)
        {
            import.Measures[i].Events.Count.ShouldBe(measures[i].Events.Count);
            for (int j = 0; j < measures[i].Events.Count; j++)
            {
                var (got, want) = (import.Measures[i].Events[j], measures[i].Events[j]);
                got.Duration.ShouldBe(want.Duration);
                got.TiedToNext.ShouldBe(want.TiedToNext);
                got.Pitches.ShouldBe(want.Pitches);
            }
        }

        var position = Rational.Zero;
        foreach (var m in measures)
        {
            import.Tempo.SecondsAt(position).ShouldBe(tempo.SecondsAt(position), 1e-6);
            position += m.Capacity;
        }
    }

    [Fact]
    public void FourFour_WithTiesChordsRestsKeyAndTwoTempoChanges_RoundTrips()
    {
        var bars = new[]
        {
            new Measure(TimeSignature.CommonTime, [Ev(Q, false, C4), RhythmEvent.Rest(Q), Ev(Q, false, C4, E4, G4), Ev(Q, true, E4)]),
            new Measure(TimeSignature.CommonTime, [Ev(H, false, E4), Ev(H, false, G4)]),
        };
        var tempo = new TempoMap(96).WithChange(Rational.Of(1), 120).WithChange(Rational.Of(3, 2), 72);
        var key = KeySignature.Parse("D");
        var import = RoundTrip(bars, tempo, key);
        import.Key.ShouldBe(key);
        import.Tempo.Changes.Count.ShouldBe(3);
        ShouldMatch(import, bars, tempo);
    }

    [Fact]
    public void ThreeFour_RoundTrips()
    {
        var bars = new[] { new Measure(new TimeSignature(3, 4), [Ev(H, false, C4), Ev(Q, false, E4)]) };
        var tempo = new TempoMap(100);
        ShouldMatch(RoundTrip(bars, tempo, KeySignature.Parse("F")), bars, tempo);
    }

    [Fact]
    public void SixEight_RoundTrips()
    {
        var dottedQuarter = new Duration(DurationType.Quarter, 1);
        var bars = new[] { new Measure(new TimeSignature(6, 8), [Ev(dottedQuarter, false, C4), Ev(dottedQuarter, false, G4)]) };
        var tempo = new TempoMap(60);
        ShouldMatch(RoundTrip(bars, tempo), bars, tempo);
    }

    [Fact]
    public void TieAcrossBarline_IsRestored()
    {
        var bars = new[]
        {
            new Measure(TimeSignature.CommonTime, [RhythmEvent.Rest(H), Ev(Q, false, C4), Ev(Q, true, E4)]),
            new Measure(TimeSignature.CommonTime, [Ev(H, false, E4)]),
        };
        var tempo = new TempoMap(120);
        ShouldMatch(RoundTrip(bars, tempo), bars, tempo);
    }

    [Fact]
    public void SevenTuplet_RoundTripsAt3360_WhenTheGridFitsIt()
    {
        var tuplet = Duration.CreateTuplet(DurationType.Quarter, 7, 4);
        var bars = new[] { new Measure(TimeSignature.CommonTime, Enumerable.Repeat(Ev(tuplet, false, C4), 7).ToArray()) };
        var tempo = new TempoMap(120);
        ShouldMatch(RoundTrip(bars, tempo, grid: Rational.Of(1, 7), options: new(3360)), bars, tempo);
    }

    [Fact]
    public void NoteLongerThanAnyPlainDuration_BecomesTiedParts()
    {
        // 600 ticks = 5/16 of a whole note: a quarter tied to a sixteenth.
        var import = ReadHex(Hdr + Trk("00903C40" + "8458803C00" + "00FF2F00"));
        import.Measures.Single().Events.Select(e => (e.Duration.Value, e.TiedToNext)).ShouldBe(
            [(Rational.Of(1, 4), true), (Rational.Of(1, 16), false)]);
    }

    [Fact]
    public void NoteOnWithVelocityZero_ReadsAsNoteOff_AndRunningStatusIsRead()
    {
        // 90 3C 40 | +480: 3C 00 (running status, velocity 0) | +0: 40 40 | +480: 40 00 | end.
        var import = ReadHex(Hdr + Trk("00903C40" + "83603C00" + "00" + "4040" + "836040" + "00" + "00FF2F00"));
        import.Measures.Single().Events.Select(e => (e.Pitches.Single().ToString(), e.Duration.Value))
            .ShouldBe([("C4", Rational.Of(1, 4)), ("E4", Rational.Of(1, 4))]);
    }

    [Fact]
    public void NoNotesMeterKeyOrTempo_ReadsAsEmpty4_4At120()
    {
        var import = ReadHex(Hdr + Trk("00FF2F00"));
        import.TimeSignature.ShouldBe(TimeSignature.CommonTime);
        import.Key.ShouldBeNull();
        import.Tempo.ShouldBe(new TempoMap(120));
        import.Measures.ShouldBeEmpty();
    }

    [Fact]
    public void LeadingGap_BecomesARest_AndOtherEventsAreSkipped()
    {
        // Program change, sysex, a text meta event, then a quarter note after a quarter of silence.
        var import = ReadHex(Hdr + Trk("00C005" + "00F00243F7" + "00FF010161" + "8360903C40" + "83603C00" + "00FF2F00"));
        import.Measures.Single().Events.Select(e => e.IsRest).ShouldBe([true, false]);
    }

    [Fact]
    public void NoteLeftOpen_EndsWithTheTrack_AndNoteOffWithoutOnIsIgnored()
    {
        var import = ReadHex(Hdr + Trk("00803C00" + "00903C40" + "8360FF2F00"));
        import.Measures.Single().Events.Single().Duration.ShouldBe(Q);
    }

    [Fact]
    public void ZeroLengthNote_LastsOneGridStep()
    {
        var import = ReadHex(Hdr + Trk("00903C40" + "00803C00" + "00FF2F00"));
        import.Measures.Single().Events.Single().Duration.Value.ShouldBe(Rational.Of(1, 64));
    }

    [Fact]
    public void MeterKeyAndTempoEvents_AreRead()
    {
        var import = ReadHex(Hdr + Trk("00FF58040302180 8".Replace(" ", "") + "00FF59020000" + "00FF510303D090" + "00903C40" + "83603C00" + "00FF2F00"));
        import.TimeSignature.ShouldBe(new TimeSignature(3, 4));
        import.Key.ShouldBe(KeySignature.Parse("C"));
        import.Tempo.BpmAt(Rational.Zero).ShouldBe(240, 1e-9);
    }

    [Fact]
    public void MergesNotesFromAllTracks()
    {
        var hex = "4D546864000000060001000201E0" +
            Trk("00903C40" + "83603C00" + "00FF2F00") +
            Trk("8360904040" + "83604000" + "00FF2F00");
        ReadHex(hex).Measures.Single().Events.Select(e => e.IsRest).ShouldBe([false, false]);
    }

    [Fact]
    public void KeyEvent_SpellsPitchClasses_AndReadsMinorFlatKeys()
    {
        var bars = new[] { new Measure(TimeSignature.CommonTime, [Ev(Q, false, Note.Parse("F#4"))]) };
        RoundTrip(bars, new TempoMap(120), KeySignature.Parse("D")).Measures[0].Events[0].Pitches[0].ToString().ShouldBe("F#4");
        var flat = ReadHex(Hdr + Trk("00FF5902FD01" + "00903E40" + "83603E00" + "00FF2F00"));
        flat.Key.ShouldBe(new KeySignature(Note.Parse("C4"), KeyMode.Minor));
        flat.Measures[0].Events[0].Pitches[0].ToString().ShouldBe("D4");
    }

    [Fact]
    public void NoKey_SpellsWithTheDefaultSpelling()
    {
        var import = ReadHex(Hdr + Trk("00903D40" + "83603D00" + "00FF2F00"));
        import.Measures[0].Events[0].Pitches[0].ToString().ShouldBe("C#4");
    }

    [Fact]
    public void UnevenOverlap_Throws()
    {
        Should.Throw<NotSupportedException>(() => ReadHex(Hdr + Trk("00903C40" + "00904040" + "83603C00" + "83604000" + "00FF2F00")))
            .Message.ShouldContain("tick 0");
        Should.Throw<NotSupportedException>(() => ReadHex(Hdr + Trk("00903C40" + "78904040" + "78803C00" + "78804000" + "00FF2F00")))
            .Message.ShouldContain("tick 120");
    }

    [Fact]
    public void SmpteDivision_Throws() =>
        Should.Throw<NotSupportedException>(() => ReadHex("4D5468640000000600010001E728")).Message.ShouldContain("SMPTE");

    [Fact]
    public void Format2_Throws() =>
        Should.Throw<NotSupportedException>(() => ReadHex("4D546864000000060002000101E0")).Message.ShouldContain("format 2");

    [Fact]
    public void ZeroTracks_ReadsAsEmpty() =>
        ReadHex("4D546864000000060000000001E0").Measures.ShouldBeEmpty();

    [Fact]
    public void BadStructure_ThrowsFormatException()
    {
        Should.Throw<FormatException>(() => ReadHex("00")).Message.ShouldContain("MThd");
        Should.Throw<FormatException>(() => ReadHex("4D546864000000060000000100" + "00")).Message.ShouldContain("division");
        Should.Throw<FormatException>(() => ReadHex(Hdr + "4D546864")).Message.ShouldContain("MTrk");
        Should.Throw<FormatException>(() => ReadHex(Hdr + "4D54726B00000010" + "00FF2F00")).Message.ShouldContain("longer");
        Should.Throw<FormatException>(() => ReadHex(Hdr + Trk("003C"))).Message.ShouldContain("status");
        Should.Throw<FormatException>(() => ReadHex(Hdr + Trk("0090"))).Message.ShouldContain("middle");
    }

    [Fact]
    public void NonPositiveGrid_Throws() =>
        Should.Throw<ArgumentOutOfRangeException>(() => MidiReader.Read(new MemoryStream(), Rational.Zero));

    [Fact]
    public void NullStream_Throws() =>
        Should.Throw<ArgumentNullException>(() => MidiReader.Read(null!));
}
