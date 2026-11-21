using System.Xml.Linq;
using System.Xml.XPath;

namespace MusicTheory.MusicXml.UnitTests;

public class MusicXmlWriterTests
{
    private static readonly TimeSignature Common = new(4, 4);

    private static Duration D(DurationType t, int dots = 0) => new(t, dots);

    private static RhythmEvent N(string pitch, Duration d, bool tied = false) => new([Note.Parse(pitch)], d, tied);

    private static XDocument Write(IReadOnlyList<Measure> m, KeySignature? key = null, string? title = null) =>
        MusicXmlWriter.Write(m, key, title);

    private static string Value(XDocument d, string xpath) => d.XPathSelectElement(xpath)!.Value;

    [Fact]
    public void Root_is_partwise_4_0_with_doctype_and_one_part()
    {
        var doc = Write([new Measure(Common, [RhythmEvent.Rest(D(DurationType.Whole))])]);

        doc.Root!.Name.LocalName.ShouldBe("score-partwise");
        doc.Root.Attribute("version")!.Value.ShouldBe("4.0");
        doc.DocumentType!.PublicId.ShouldBe("-//Recordare//DTD MusicXML 4.0 Partwise//EN");
        doc.XPathSelectElements("/score-partwise/part-list/score-part[@id='P1']").Count().ShouldBe(1);
        doc.XPathSelectElements("/score-partwise/part[@id='P1']").Count().ShouldBe(1);
        doc.XPathSelectElement("//work").ShouldBeNull();
        doc.XPathSelectElement("//key").ShouldBeNull();
        doc.XPathSelectElement("//rest").ShouldNotBeNull();
    }

    [Fact]
    public void Dotted_sharp_quarter_matches_the_spec_serialization()
    {
        // A dotted quarter and an eighth force divisions 2.
        var doc = Write([new Measure(Common, [N("F#4", D(DurationType.Quarter, 1)), N("G4", D(DurationType.Eighth)), RhythmEvent.Rest(D(DurationType.Half))])]);

        Value(doc, "//divisions").ShouldBe("2");
        var first = doc.XPathSelectElement("//note")!;
        first.ToString(SaveOptions.DisableFormatting).ShouldBe(
            "<note><pitch><step>F</step><alter>1</alter><octave>4</octave></pitch><duration>3</duration><type>quarter</type><dot /></note>");
        doc.XPathSelectElements("//note")!.Last().ToString(SaveOptions.DisableFormatting)
            .ShouldBe("<note><rest /><duration>4</duration><type>half</type></note>");
    }

    [Fact]
    public void Natural_has_no_alter_and_flat_is_negative()
    {
        var doc = Write([new Measure(Common, [N("C4", D(DurationType.Half)), N("Bb3", D(DurationType.Half))])]);

        doc.XPathSelectElements("//pitch/alter").Count().ShouldBe(1);
        Value(doc, "//note[2]/pitch/alter").ShouldBe("-1");
    }

    [Theory]
    [InlineData("Bb", -2, "major")]
    [InlineData("A", 0, "minor")]
    [InlineData("F#", 6, "major")]
    public void Key_writes_fifths_and_mode(string key, int fifths, string mode)
    {
        var doc = Write([new Measure(Common, [RhythmEvent.Rest(D(DurationType.Whole))])], KeySignature.Parse(mode == "minor" ? key + "m" : key));

        Value(doc, "//key/fifths").ShouldBe(fifths.ToString());
        Value(doc, "//key/mode").ShouldBe(mode);
    }

    [Fact]
    public void Chord_writes_chord_flag_on_following_notes()
    {
        var chord = new RhythmEvent([Note.Parse("C4"), Note.Parse("E4"), Note.Parse("G4")], D(DurationType.Whole));
        var doc = Write([new Measure(Common, [chord])]);

        var notes = doc.XPathSelectElements("//note").ToList();
        notes.Count.ShouldBe(3);
        notes[0].Element("chord").ShouldBeNull();
        notes[1].Element("chord").ShouldNotBeNull();
        notes[2].Element("chord").ShouldNotBeNull();
        notes[1].Element("duration")!.Value.ShouldBe("4");
    }

    [Fact]
    public void Tie_writes_start_then_stop_across_a_barline()
    {
        var bars = Measure.Split(Common, [N("C4", new Duration(DurationType.Whole, 1))]);
        var doc = Write(bars);

        var measures = doc.XPathSelectElements("//measure").ToList();
        measures.Count.ShouldBe(2);
        measures[0].XPathSelectElement("note/tie")!.Attribute("type")!.Value.ShouldBe("start");
        measures[0].XPathSelectElement("note/notations/tied")!.Attribute("type")!.Value.ShouldBe("start");
        measures[1].XPathSelectElement("note/tie")!.Attribute("type")!.Value.ShouldBe("stop");
        measures[1].XPathSelectElement("note/notations/tied")!.Attribute("type")!.Value.ShouldBe("stop");
    }

    [Fact]
    public void Middle_of_a_tie_chain_writes_stop_and_start()
    {
        var doc = Write([new Measure(Common, [N("C4", D(DurationType.Half), true), N("C4", D(DurationType.Quarter), true), N("C4", D(DurationType.Quarter))])]);

        var middle = doc.XPathSelectElements("//note").ElementAt(1);
        middle.Elements("tie").Select(t => t.Attribute("type")!.Value).ShouldBe(["stop", "start"]);
        middle.XPathSelectElements("notations/tied").Count().ShouldBe(2);
    }

    [Fact]
    public void Triplet_writes_time_modification_and_divisions_three()
    {
        var third = Duration.CreateTuplet(DurationType.Eighth, 3, 2);
        var doc = Write([new Measure(new TimeSignature(6, 8), [N("C4", third), N("D4", third), N("E4", third), RhythmEvent.Rest(D(DurationType.Quarter, 1))])]);

        Value(doc, "//divisions").ShouldBe("6");
        Value(doc, "//note[1]/time-modification/actual-notes").ShouldBe("3");
        Value(doc, "//note[1]/time-modification/normal-notes").ShouldBe("2");
        Value(doc, "//note[1]/duration").ShouldBe("2");
        Value(doc, "//note[4]/duration").ShouldBe("9");
    }

    [Fact]
    public void Time_appears_only_when_the_meter_changes()
    {
        var m1 = new Measure(Common, [RhythmEvent.Rest(D(DurationType.Whole))]);
        var m2 = new Measure(Common, [RhythmEvent.Rest(D(DurationType.Whole))]);
        var m3 = new Measure(new TimeSignature(3, 4), [RhythmEvent.Rest(D(DurationType.Half, 1))]);
        var doc = Write([m1, m2, m3]);

        var measures = doc.XPathSelectElements("//measure").ToList();
        measures[0].XPathSelectElement("attributes/time/beats")!.Value.ShouldBe("4");
        measures[0].XPathSelectElement("attributes/time/beat-type")!.Value.ShouldBe("4");
        measures[1].Element("attributes").ShouldBeNull();
        measures[2].XPathSelectElement("attributes/time/beats")!.Value.ShouldBe("3");
        measures[2].XPathSelectElement("attributes/divisions").ShouldBeNull();
    }

    [Fact]
    public void Pickup_is_implicit_and_numbered_zero()
    {
        var bars = Measure.Split(new TimeSignature(3, 4), [N("C4", D(DurationType.Quarter)), N("D4", D(DurationType.Half, 1))], Rational.Of(1, 4));
        var doc = Write(bars);

        var measures = doc.XPathSelectElements("//measure").ToList();
        measures[0].Attribute("number")!.Value.ShouldBe("0");
        measures[0].Attribute("implicit")!.Value.ShouldBe("yes");
        measures[1].Attribute("number")!.Value.ShouldBe("1");
        measures[1].Attribute("implicit").ShouldBeNull();
    }

    [Fact]
    public void Title_goes_in_work_title()
    {
        var doc = Write([new Measure(Common, [RhythmEvent.Rest(D(DurationType.Whole))])], title: "Minuet");

        Value(doc, "/score-partwise/work/work-title").ShouldBe("Minuet");
    }

    [Theory]
    [InlineData(DurationType.Whole, "whole")]
    [InlineData(DurationType.Half, "half")]
    [InlineData(DurationType.Quarter, "quarter")]
    [InlineData(DurationType.Eighth, "eighth")]
    [InlineData(DurationType.Sixteenth, "16th")]
    [InlineData(DurationType.ThirtySecond, "32nd")]
    [InlineData(DurationType.SixtyFourth, "64th")]
    public void Every_duration_type_has_its_musicxml_name(DurationType type, string name)
    {
        var doc = Write([new Measure(Common, [RhythmEvent.Rest(D(type))])]);

        Value(doc, "//note/type").ShouldBe(name);
        Value(doc, "//divisions").ShouldBe((type == DurationType.Whole || type == DurationType.Half || type == DurationType.Quarter ? 1 : (int)Math.Pow(2, (int)type - 2)).ToString());
    }

    [Fact]
    public void Invalid_arguments_throw()
    {
        Should.Throw<ArgumentNullException>(() => MusicXmlWriter.Write(null!));
        Should.Throw<ArgumentException>(() => MusicXmlWriter.Write([]));
    }
}
