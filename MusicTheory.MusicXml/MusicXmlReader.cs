using System.Globalization;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MusicTheory.MusicXml;

/// <summary>Reads the first part of partwise MusicXML 3.1 or 4.0 into measures, key and meter.</summary>
public static class MusicXmlReader
{
    /// <summary>
    /// Reads the first <c>&lt;part&gt;</c> of a <c>score-partwise</c> document. Grace and cue notes are skipped, ties
    /// come from <c>&lt;tie&gt;</c> (sound), an <c>implicit="yes"</c> measure becomes a pickup, and the key is the one in
    /// the first measure.
    /// </summary>
    /// <param name="document">The MusicXML document.</param>
    /// <returns>The title, key, meter and measures of the first part.</returns>
    /// <exception cref="NotSupportedException">The root is <c>score-timewise</c>, or the part uses
    /// <c>&lt;backup&gt;</c>, <c>&lt;forward&gt;</c>, a voice other than 1, microtones or a dotted tuplet.</exception>
    /// <exception cref="FormatException">The document is not partwise MusicXML, or a measure holds a note, key or meter
    /// that cannot be read; the message names the measure number.</exception>
    public static MusicXmlScore Read(XDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var root = document.Root;
        if (root?.Name.LocalName == "score-timewise")
            throw new NotSupportedException("score-timewise MusicXML is not supported; only score-partwise is.");
        if (root?.Name.LocalName != "score-partwise")
            throw new FormatException("The document is not score-partwise MusicXML.");

        var part = root.Element("part") ?? throw new FormatException("The score has no <part>.");
        var keyElement = part.XPathSelectElement("measure[1]/attributes/key");
        var key = keyElement is null ? null : ReadKey(keyElement);

        var measures = new List<Measure>();
        TimeSignature? meter = null;
        var divisions = 0;
        foreach (var (m, index) in part.Elements("measure").Select((m, i) => (m, i)))
        {
            var label = (string?)m.Attribute("number") ?? (index + 1).ToString(CultureInfo.InvariantCulture);
            foreach (var attributes in m.Elements("attributes"))
            {
                if (attributes.Element("divisions") is not null)
                    divisions = Int(attributes, "divisions", label);
                if (attributes.Element("time") is { } time)
                    meter = new TimeSignature(Int(time, "beats", label), Int(time, "beat-type", label));
            }

            var timeSignature = meter ?? throw new FormatException($"Measure {label} has no <time> before its first note.");
            var events = ReadEvents(m, divisions, label);
            var filled = events.Aggregate(Rational.Zero, (sum, e) => sum + e.Duration.Value);
            measures.Add(new Measure(timeSignature, events, (string?)m.Attribute("implicit") == "yes" ? filled : null));
        }

        if (measures.Count == 0)
            throw new FormatException("The first <part> has no <measure>.");
        return new MusicXmlScore(root.XPathSelectElement("work/work-title")?.Value, key, measures[0].TimeSignature, measures);
    }

    private static List<RhythmEvent> ReadEvents(XElement measure, int divisions, string label)
    {
        var events = new List<(List<Note> Pitches, Duration Duration, bool Tied)>();
        foreach (var element in measure.Elements())
        {
            switch (element.Name.LocalName)
            {
                case "backup" or "forward":
                    throw new NotSupportedException($"Measure {label}: <{element.Name.LocalName}> (several voices) is not supported.");
                case "note":
                    break;
                default:
                    continue;
            }

            if (element.Element("grace") is not null || element.Element("cue") is not null)
                continue;
            var voice = (string?)element.Element("voice");
            if (voice is not (null or "1"))
                throw new NotSupportedException($"Measure {label}: voice {voice} is not supported; only voice 1 is.");

            var pitchElement = element.Element("pitch");
            if (pitchElement is null && element.Element("rest") is null)
                throw new FormatException($"Measure {label}: a note has neither <pitch> nor <rest>.");
            var pitch = pitchElement is null ? null : ReadPitch(pitchElement, label);

            if (element.Element("chord") is not null)
            {
                if (pitch is null || events.Count == 0)
                    throw new FormatException($"Measure {label}: a <chord/> note must be a pitch that follows another note.");
                events[^1].Pitches.Add(pitch);
                continue;
            }

            var tied = pitch is not null && element.Elements("tie").Any(t => (string?)t.Attribute("type") == "start");
            events.Add((pitch is null ? [] : [pitch], ReadDuration(element, divisions, label), tied));
        }

        return events.ConvertAll(e => new RhythmEvent(e.Pitches, e.Duration, e.Tied));
    }

    private static Note ReadPitch(XElement pitch, string label)
    {
        var step = Array.IndexOf(["C", "D", "E", "F", "G", "A", "B"], (string?)pitch.Element("step"));
        if (step < 0)
            throw new FormatException($"Measure {label}: invalid <step>.");
        var alter = (string?)pitch.Element("alter") ?? "0";
        if (!decimal.TryParse(alter, NumberStyles.Float, CultureInfo.InvariantCulture, out var semitones) || semitones != decimal.Truncate(semitones))
            throw new NotSupportedException($"Measure {label}: <alter>{alter}</alter> (microtones) is not supported.");
        return new Note((NoteName)step, (Alteration)(int)semitones, Int(pitch, "octave", label));
    }

    private static Duration ReadDuration(XElement note, int divisions, string label)
    {
        if (note.Element("type") is not { } typeElement)
        {
            var ticks = Int(note, "duration", label);
            if (divisions > 0 && Duration.TryFromValue(Rational.Of(ticks, 4L * divisions), out var derived))
                return derived;
            throw new FormatException($"Measure {label}: a note without <type> has a <duration> of {ticks} that is not a notatable length at {divisions} divisions.");
        }

        // The order is DurationType's: whole first, 64th last.
        var typeIndex = Array.IndexOf(["whole", "half", "quarter", "eighth", "16th", "32nd", "64th"], typeElement.Value);
        if (typeIndex < 0)
            throw new FormatException($"Measure {label}: unsupported note <type> '{typeElement.Value}'.");
        var type = (DurationType)typeIndex;
        var dots = note.Elements("dot").Count();
        if (note.Element("time-modification") is not { } modification)
            return new Duration(type, dots);
        if (dots > 0)
            throw new NotSupportedException($"Measure {label}: a dotted tuplet note is not supported.");
        return Duration.CreateTuplet(type, Int(modification, "actual-notes", label), Int(modification, "normal-notes", label));
    }

    private static KeySignature ReadKey(XElement key)
    {
        var fifths = Int(key, "fifths", "1");
        if (Math.Abs(fifths) > 7)
            throw new FormatException($"<fifths>{fifths}</fifths> is outside -7..7.");
        var minor = (string?)key.Element("mode") == "minor";
        var result = new KeySignature(new Note(minor ? NoteName.A : NoteName.C), minor ? KeyMode.Minor : KeyMode.Major);
        for (var i = 0; i < Math.Abs(fifths); i++)
            result = fifths > 0 ? result.NextInCircle() : result.PreviousInCircle();
        return result;
    }

    private static int Int(XElement parent, string child, string label) =>
        int.TryParse(parent.Element(child)?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : throw new FormatException($"Measure {label}: missing or invalid <{child}>.");
}
