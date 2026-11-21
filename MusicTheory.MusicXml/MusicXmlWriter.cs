using System.Xml.Linq;

namespace MusicTheory.MusicXml;

/// <summary>Writes measures as single-part partwise MusicXML 4.0.</summary>
public static class MusicXmlWriter
{
    /// <summary>
    /// Builds a partwise MusicXML 4.0 document with one part. <c>divisions</c>, the optional key and the meter are
    /// written in the first measure; the meter is written again only when it changes. A measure shorter than its
    /// meter is a pickup: it is written as <c>implicit="yes"</c> and numbered 0.
    /// </summary>
    /// <param name="measures">The measures of the part, at least one.</param>
    /// <param name="key">The key, or <see langword="null"/> to write none.</param>
    /// <param name="title">The work title, or <see langword="null"/> to write none.</param>
    /// <returns>The document, ready to <see cref="XDocument.Save(string)"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="measures"/> is empty.</exception>
    public static XDocument Write(IReadOnlyList<Measure> measures, KeySignature? key = null, string? title = null)
    {
        ArgumentNullException.ThrowIfNull(measures);
        if (measures.Count == 0)
            throw new ArgumentException("At least one measure is required.", nameof(measures));

        var divisions = 1L;
        foreach (var e in measures.SelectMany(m => m.Events))
            divisions = Lcm(divisions, (e.Duration.Value * Rational.Of(4)).Denominator);

        var part = new XElement("part", new XAttribute("id", "P1"));
        var number = IsPickup(measures[0]) ? 0 : 1;
        TimeSignature? meter = null;
        var tiedIn = false;
        foreach (var measure in measures)
        {
            var element = new XElement("measure", new XAttribute("number", number++));
            if (IsPickup(measure))
                element.Add(new XAttribute("implicit", "yes"));

            var attributes = new XElement("attributes");
            if (meter is null)
            {
                attributes.Add(new XElement("divisions", divisions));
                if (key is not null)
                    attributes.Add(new XElement("key",
                        new XElement("fifths", key.AccidentalCount),
                        new XElement("mode", key.Mode == KeyMode.Major ? "major" : "minor")));
            }
            if (measure.TimeSignature != meter)
                attributes.Add(new XElement("time",
                    new XElement("beats", measure.TimeSignature.Numerator),
                    new XElement("beat-type", measure.TimeSignature.Denominator)));
            meter = measure.TimeSignature;
            element.Add(attributes);

            foreach (var e in measure.Events)
            {
                AddEvent(element, e, divisions, tiedIn);
                tiedIn = e.TiedToNext;
            }
            part.Add(element);
        }

        var root = new XElement("score-partwise", new XAttribute("version", "4.0"));
        if (title is not null)
            root.Add(new XElement("work", new XElement("work-title", title)));
        root.Add(
            new XElement("part-list", new XElement("score-part", new XAttribute("id", "P1"), new XElement("part-name", "Music"))),
            part);

        return new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XDocumentType("score-partwise", "-//Recordare//DTD MusicXML 4.0 Partwise//EN", "http://www.musicxml.org/dtds/partwise.dtd", null),
            root);
    }

    private static bool IsPickup(Measure measure) => measure.Capacity < measure.TimeSignature.MeasureLength;

    private static void AddEvent(XElement measure, RhythmEvent e, long divisions, bool tiedIn)
    {
        if (e.IsRest)
        {
            measure.Add(Note(e, divisions, null, false, false, false));
            return;
        }

        for (var i = 0; i < e.Pitches.Count; i++)
            measure.Add(Note(e, divisions, e.Pitches[i], i > 0, tiedIn, e.TiedToNext));
    }

    private static XElement Note(RhythmEvent e, long divisions, Note? pitch, bool chord, bool stop, bool start)
    {
        var d = e.Duration;
        var note = new XElement("note");
        if (chord)
            note.Add(new XElement("chord"));
        if (pitch is null)
            note.Add(new XElement("rest"));
        else
        {
            var step = new XElement("pitch", new XElement("step", pitch.Name.ToString()));
            if (pitch.Alteration != Alteration.Natural)
                step.Add(new XElement("alter", (int)pitch.Alteration));
            step.Add(new XElement("octave", pitch.Octave));
            note.Add(step);
        }
        note.Add(new XElement("duration", (d.Value * Rational.Of(4 * divisions)).Numerator));
        if (stop)
            note.Add(new XElement("tie", new XAttribute("type", "stop")));
        if (start)
            note.Add(new XElement("tie", new XAttribute("type", "start")));
        note.Add(new XElement("type", TypeName(d.Type)));
        for (var i = 0; i < d.Dots; i++)
            note.Add(new XElement("dot"));
        if (d.Tuplet is var (actual, normal))
            note.Add(new XElement("time-modification",
                new XElement("actual-notes", actual),
                new XElement("normal-notes", normal)));
        if (stop || start)
        {
            var notations = new XElement("notations");
            if (stop)
                notations.Add(new XElement("tied", new XAttribute("type", "stop")));
            if (start)
                notations.Add(new XElement("tied", new XAttribute("type", "start")));
            note.Add(notations);
        }
        return note;
    }

    private static string TypeName(DurationType type) => type switch
    {
        DurationType.Whole => "whole",
        DurationType.Half => "half",
        DurationType.Quarter => "quarter",
        DurationType.Eighth => "eighth",
        DurationType.Sixteenth => "16th",
        DurationType.ThirtySecond => "32nd",
        _ => "64th"
    };

    private static long Lcm(long a, long b)
    {
        var (x, y) = (a, b);
        while (y != 0)
            (x, y) = (y, x % y);
        return a / x * b;
    }
}
