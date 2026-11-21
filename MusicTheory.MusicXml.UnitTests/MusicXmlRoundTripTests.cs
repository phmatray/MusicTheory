using System.Xml.Linq;

namespace MusicTheory.MusicXml.UnitTests;

public class MusicXmlRoundTripTests
{
    private static readonly TimeSignature Common = new(4, 4);

    private const string Meter44 = "<attributes><divisions>2</divisions><time><beats>4</beats><beat-type>4</beat-type></time></attributes>";

    private static Duration D(DurationType t, int dots = 0) => new(t, dots);

    private static RhythmEvent N(string pitch, Duration d, bool tied = false) => new([Note.Parse(pitch)], d, tied);

    private static RhythmEvent Chord(Duration d, bool tied, params string[] pitches) => new(pitches.Select(Note.Parse).ToArray(), d, tied);

    private static MusicXmlScore RoundTrip(IReadOnlyList<Measure> measures, KeySignature? key = null, string? title = null) =>
        MusicXmlReader.Read(MusicXmlWriter.Write(measures, key, title));

    private static MusicXmlScore ReadPart(string measures) =>
        MusicXmlReader.Read(XDocument.Parse($"<score-partwise version=\"3.1\"><part-list/><part id=\"P1\">{measures}</part></score-partwise>"));

    private static string Nt(string body) => $"<note>{body}</note>";

    private static void ShouldMatch(IReadOnlyList<Measure> actual, IReadOnlyList<Measure> expected)
    {
        actual.Count.ShouldBe(expected.Count);
        for (var i = 0; i < expected.Count; i++)
        {
            actual[i].TimeSignature.ShouldBe(expected[i].TimeSignature);
            actual[i].Capacity.ShouldBe(expected[i].Capacity);
            actual[i].Events.Count.ShouldBe(expected[i].Events.Count);
            for (var j = 0; j < expected[i].Events.Count; j++)
            {
                actual[i].Events[j].Pitches.ShouldBe(expected[i].Events[j].Pitches);
                actual[i].Events[j].Duration.ShouldBe(expected[i].Events[j].Duration);
                actual[i].Events[j].TiedToNext.ShouldBe(expected[i].Events[j].TiedToNext);
            }
        }
    }

    [Fact]
    public void Common_time_with_chords_ties_rests_and_title_round_trips()
    {
        Measure[] measures =
        [
            new(Common, [N("F#4", D(DurationType.Quarter, 1), tied: true), N("F#4", D(DurationType.Eighth)), Chord(D(DurationType.Quarter), true, "C4", "E4", "G4"), RhythmEvent.Rest(D(DurationType.Quarter))]),
            new(Common, [Chord(D(DurationType.Half), false, "C4", "E4", "G4"), N("Bb3", D(DurationType.Half))])
        ];

        var score = RoundTrip(measures, KeySignature.Parse("F"), "Minuet");

        score.Title.ShouldBe("Minuet");
        score.Key.ShouldBe(KeySignature.Parse("F"));
        score.TimeSignature.ShouldBe(Common);
        ShouldMatch(score.Measures, measures);
    }

    [Fact]
    public void Three_four_with_a_pickup_round_trips()
    {
        var three = new TimeSignature(3, 4);
        Measure[] measures =
        [
            new(three, [N("D4", D(DurationType.Quarter))], Rational.Of(1, 4)),
            new(three, [N("G4", D(DurationType.Half)), N("A4", D(DurationType.Quarter))]),
            new(three, [N("B4", D(DurationType.Half, 1))])
        ];

        var score = RoundTrip(measures, KeySignature.Parse("G"));

        ShouldMatch(score.Measures, measures);
        score.Measures[0].Capacity.ShouldBe(Rational.Of(1, 4));
    }

    [Fact]
    public void Six_eight_with_triplets_round_trips()
    {
        var six = new TimeSignature(6, 8);
        var triplet = Duration.CreateTuplet(DurationType.Eighth, 3, 2);
        Measure[] measures =
        [
            new(six, [N("C5", D(DurationType.Quarter, 1)), N("D5", triplet), N("E5", triplet), N("F5", triplet)])
        ];

        ShouldMatch(RoundTrip(measures).Measures, measures);
    }

    [Fact]
    public void Every_duration_type_and_a_meter_change_round_trip()
    {
        var types = Enum.GetValues<DurationType>();
        var measures = types.Select(t => new Measure(Common, [RhythmEvent.Rest(D(t))])).ToList();
        measures.Add(new Measure(new TimeSignature(3, 4), [RhythmEvent.Rest(D(DurationType.Half, 1))]));

        var score = RoundTrip(measures);

        ShouldMatch(score.Measures, measures);
        score.Key.ShouldBeNull();
        score.Title.ShouldBeNull();
    }

    [Theory]
    [InlineData("C", 0, "major")]
    [InlineData("Bb", -2, "major")]
    [InlineData("F#", 6, "major")]
    [InlineData("A", 0, "minor")]
    [InlineData("D", -1, "minor")]
    [InlineData("C#", 4, "minor")]
    public void Key_is_rebuilt_from_fifths_and_mode(string text, int fifths, string mode)
    {
        var key = KeySignature.Parse(mode == "minor" ? text + "m" : text);
        key.AccidentalCount.ShouldBe(fifths);

        RoundTrip([new Measure(Common, [RhythmEvent.Rest(D(DurationType.Whole))])], key).Key.ShouldBe(key);
    }

    [Fact]
    public void Fifths_beyond_seven_is_a_format_error()
    {
        var xml = $"<measure number=\"1\"><attributes><divisions>1</divisions><key><fifths>8</fifths></key><time><beats>4</beats><beat-type>4</beat-type></time></attributes></measure>";

        Should.Throw<FormatException>(() => ReadPart(xml));
    }

    [Fact]
    public void Note_without_type_derives_its_duration_from_divisions()
    {
        var score = ReadPart($"<measure number=\"1\">{Meter44}{Nt("<pitch><step>C</step><octave>4</octave></pitch><duration>3</duration>")}</measure>");

        score.Measures[0].Events[0].Duration.ShouldBe(D(DurationType.Quarter, 1));
    }

    [Fact]
    public void Non_notatable_duration_names_the_measure()
    {
        var ex = Should.Throw<FormatException>(() =>
            ReadPart($"<measure number=\"7\">{Meter44}{Nt("<pitch><step>C</step><octave>4</octave></pitch><duration>5</duration>")}</measure>"));

        ex.Message.ShouldContain("Measure 7");
    }

    [Fact]
    public void Note_without_type_or_divisions_is_a_format_error()
    {
        const string time = "<attributes><time><beats>4</beats><beat-type>4</beat-type></time></attributes>";

        Should.Throw<FormatException>(() =>
            ReadPart($"<measure number=\"1\">{time}{Nt("<pitch><step>C</step><octave>4</octave></pitch><duration>4</duration>")}</measure>"));
    }

    [Fact]
    public void Note_without_type_or_duration_is_a_format_error()
    {
        Should.Throw<FormatException>(() =>
            ReadPart($"<measure number=\"1\">{Meter44}{Nt("<rest/>")}</measure>")).Message.ShouldContain("<duration>");
    }

    [Fact]
    public void Grace_and_cue_notes_are_skipped_and_voice_one_is_read()
    {
        const string pitch = "<pitch><step>C</step><octave>4</octave></pitch>";
        var score = ReadPart($"<measure number=\"1\">{Meter44}{Nt("<grace/>" + pitch + "<type>eighth</type>")}{Nt("<cue/>" + pitch + "<duration>1</duration><type>eighth</type>")}" +
            $"{Nt(pitch + "<duration>8</duration><type>whole</type><voice>1</voice>")}<barline/></measure>");

        score.Measures[0].Events.Count.ShouldBe(1);
        score.Measures[0].Events[0].Duration.ShouldBe(D(DurationType.Whole));
    }

    [Theory]
    [InlineData("<backup><duration>4</duration></backup>")]
    [InlineData("<forward><duration>4</duration></forward>")]
    [InlineData("<note><pitch><step>C</step><octave>4</octave></pitch><duration>4</duration><type>quarter</type><voice>2</voice></note>")]
    public void Several_voices_are_not_supported(string body)
    {
        Should.Throw<NotSupportedException>(() => ReadPart($"<measure number=\"1\">{Meter44}{body}</measure>"));
    }

    [Theory]
    [InlineData("0.5")]
    [InlineData("-1.5")]
    [InlineData("x")]
    public void Microtones_are_not_supported(string alter)
    {
        var note = Nt($"<pitch><step>C</step><alter>{alter}</alter><octave>4</octave></pitch><duration>4</duration><type>quarter</type>");

        Should.Throw<NotSupportedException>(() => ReadPart($"<measure number=\"1\">{Meter44}{note}</measure>"));
    }

    [Fact]
    public void Whole_number_alter_with_a_decimal_point_is_read()
    {
        var note = Nt("<pitch><step>E</step><alter>-1.0</alter><octave>4</octave></pitch><duration>4</duration><type>quarter</type>");

        ReadPart($"<measure number=\"1\">{Meter44}{note}</measure>").Measures[0].Events[0].Pitches[0].ShouldBe(Music("Eb4"));
    }

    private static Note Music(string text) => Note.Parse(text);

    [Theory]
    [InlineData("<pitch><step>H</step><octave>4</octave></pitch><duration>4</duration><type>quarter</type>", typeof(FormatException))]
    [InlineData("<duration>4</duration><type>quarter</type>", typeof(FormatException))]
    [InlineData("<rest/><duration>4</duration><type>breve</type>", typeof(FormatException))]
    [InlineData("<rest/><duration>4</duration><type>eighth</type><dot/><time-modification><actual-notes>3</actual-notes><normal-notes>2</normal-notes></time-modification>", typeof(NotSupportedException))]
    public void Unreadable_notes_are_rejected(string body, Type expected)
    {
        Should.Throw(() => ReadPart($"<measure number=\"1\">{Meter44}{Nt(body)}</measure>"), expected);
    }

    [Fact]
    public void A_tied_rest_is_read_as_an_untied_rest()
    {
        var score = ReadPart($"<measure number=\"1\">{Meter44}{Nt("<rest/><duration>4</duration><tie type=\"start\"/><type>half</type>")}</measure>");

        score.Measures[0].Events[0].TiedToNext.ShouldBeFalse();
    }

    [Fact]
    public void Implicit_measure_is_a_pickup_and_a_missing_number_falls_back_to_the_position()
    {
        var score = ReadPart($"<measure implicit=\"yes\">{Meter44}{Nt("<rest/><duration>2</duration><type>quarter</type>")}</measure>" +
            Measure("2", "<rest/><duration>8</duration><type>whole</type>"));

        score.Measures[0].Capacity.ShouldBe(Rational.Of(1, 4));
        score.Measures[1].Capacity.ShouldBe(Rational.One);
        Should.Throw<FormatException>(() => ReadPart($"<measure>{Meter44}{Nt("<rest/>")}</measure>")).Message.ShouldContain("Measure 1");
    }

    private static string Measure(string number, string note) => $"<measure number=\"{number}\">{Nt(note)}</measure>";

    [Fact]
    public void Measure_without_a_meter_is_a_format_error()
    {
        Should.Throw<FormatException>(() => ReadPart(Measure("1", "<rest/><duration>4</duration><type>whole</type>"))).Message.ShouldContain("<time>");
    }

    [Fact]
    public void Missing_time_part_is_a_format_error()
    {
        var xml = "<measure number=\"1\"><attributes><time><beats>4</beats></time></attributes></measure>";

        Should.Throw<FormatException>(() => ReadPart(xml));
    }

    [Fact]
    public void Timewise_scores_are_not_supported()
    {
        var doc = XDocument.Parse("<score-timewise version=\"4.0\"><part-list/></score-timewise>");

        Should.Throw<NotSupportedException>(() => MusicXmlReader.Read(doc)).Message.ShouldContain("score-timewise");
    }

    [Fact]
    public void Structurally_wrong_documents_are_format_errors()
    {
        Should.Throw<ArgumentNullException>(() => MusicXmlReader.Read(null!));
        Should.Throw<FormatException>(() => MusicXmlReader.Read(new XDocument()));
        Should.Throw<FormatException>(() => MusicXmlReader.Read(XDocument.Parse("<html/>")));
        Should.Throw<FormatException>(() => MusicXmlReader.Read(XDocument.Parse("<score-partwise/>")));
        Should.Throw<FormatException>(() => ReadPart(""));
    }
}
