using System.Globalization;
using System.Xml.Linq;

namespace MusicTheory.MusicXml.UnitTests;

/// <summary>Reads files written by music21 and compares them with the notes music21 reports for the same stream.</summary>
public class MusicXmlReferenceTests
{
    private static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "Reference");

    public static TheoryData<string, string, int, string, string> Rows()
    {
        var data = new TheoryData<string, string, int, string, string>();
        foreach (var line in File.ReadLines(Path.Combine(Dir, "musicxml.csv")).Skip(1))
        {
            var c = line.Split(',');
            data.Add(c[0], c[1], int.Parse(c[2], CultureInfo.InvariantCulture), c[3], c[4]);
        }

        return data;
    }

    private static string Token(RhythmEvent e) =>
        (e.IsRest ? "r" : string.Join('+', e.Pitches.Select(p => p.ToString()))) + ":" + e.Duration.Value + (e.TiedToNext ? "~" : "");

    [Theory]
    [MemberData(nameof(Rows))]
    public void Reads_what_music21_wrote(string file, string meter, int fifths, string mode, string notes)
    {
        var score = MusicXmlReader.Read(XDocument.Load(Path.Combine(Dir, file)));

        score.TimeSignature.ShouldBe(new TimeSignature(int.Parse(meter[..meter.IndexOf('/')], CultureInfo.InvariantCulture), int.Parse(meter[(meter.IndexOf('/') + 1)..], CultureInfo.InvariantCulture)));
        var expectedKey = new KeySignature(new Note(mode == "minor" ? NoteName.A : NoteName.C), mode == "minor" ? KeyMode.Minor : KeyMode.Major);
        for (var i = 0; i < Math.Abs(fifths); i++)
            expectedKey = fifths > 0 ? expectedKey.NextInCircle() : expectedKey.PreviousInCircle();
        score.Key.ShouldBe(expectedKey);
        string.Join(' ', score.Measures.SelectMany(m => m.Events).Select(Token)).ShouldBe(notes);
    }

    [Fact]
    public void An_invisible_rest_outside_a_pickup_is_kept()
    {
        const string bar = "<attributes><divisions>1</divisions><time><beats>1</beats><beat-type>4</beat-type></time></attributes>" +
                           "<note print-object=\"no\"><rest/><duration>1</duration><type>quarter</type></note>";
        var score = MusicXmlReader.Read(XDocument.Parse($"<score-partwise><part><measure>{bar}</measure></part></score-partwise>"));

        score.Measures[0].Events.Count.ShouldBe(1);
    }

    [Fact]
    public void A_music21_pickup_is_an_implicit_measure()
    {
        var score = MusicXmlReader.Read(XDocument.Load(Path.Combine(Dir, "pickup.musicxml")));

        score.Measures[0].Capacity.ShouldBe(Rational.Of(1, 4));
        score.Measures.Count.ShouldBe(3);
    }
}
