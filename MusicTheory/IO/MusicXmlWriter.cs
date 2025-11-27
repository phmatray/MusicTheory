namespace MusicTheory.IO;

using System.Text;
using System.Xml;
using System.Xml.Linq;

/// <summary>
/// Provides functionality to write music theory objects to MusicXML format.
/// </summary>
public static class MusicXmlWriter
{
    /// <summary>
    /// Converts a Note to MusicXML pitch element.
    /// </summary>
    /// <param name="note">The note to convert.</param>
    /// <returns>XML string representing the pitch.</returns>
    public static string NoteToXml(Note note)
    {
        var pitch = new XElement("pitch",
            new XElement("step", note.Name.ToString()),
            note.Alteration != Alteration.Natural
                ? new XElement("alter", (int)note.Alteration)
                : null,
            new XElement("octave", note.Octave)
        );

        return pitch.ToString();
    }

    /// <summary>
    /// Converts a KeySignature to MusicXML key element.
    /// </summary>
    /// <param name="key">The key signature to convert.</param>
    /// <returns>XML string representing the key.</returns>
    public static string KeyToXml(KeySignature key)
    {
        var fifths = key.AccidentalCount;
        var mode = key.Mode == KeyMode.Major ? "major" : "minor";

        var keyElement = new XElement("key",
            new XElement("fifths", fifths),
            new XElement("mode", mode)
        );

        return keyElement.ToString();
    }

    /// <summary>
    /// Converts a Chord to MusicXML harmony element.
    /// </summary>
    /// <param name="chord">The chord to convert.</param>
    /// <returns>XML string representing the harmony.</returns>
    public static string ChordToXml(Chord chord)
    {
        var rootStep = new XElement("root-step", chord.Root.Name.ToString());

        XElement? rootAlter = null;
        if (chord.Root.Alteration != Alteration.Natural)
        {
            rootAlter = new XElement("root-alter", (int)chord.Root.Alteration);
        }

        var kind = GetChordKind(chord);

        var root = new XElement("root", rootStep, rootAlter);

        var harmony = new XElement("harmony",
            root,
            new XElement("kind", kind)
        );

        return harmony.ToString();
    }

    /// <summary>
    /// Converts a Scale to MusicXML representation.
    /// </summary>
    /// <param name="scale">The scale to convert.</param>
    /// <returns>XML string representing the scale notes.</returns>
    public static string ScaleToXml(Scale scale)
    {
        var notes = scale.GetNotes().ToList();
        var scaleElement = new XElement("scale",
            new XAttribute("type", scale.Type.ToString()),
            new XElement("root", scale.Root.Name.ToString()),
            new XElement("notes",
                notes.Select(n => new XElement("note",
                    new XElement("step", n.Name.ToString()),
                    n.Alteration != Alteration.Natural
                        ? new XElement("alter", (int)n.Alteration)
                        : null,
                    new XElement("octave", n.Octave)
                ))
            )
        );

        return scaleElement.ToString();
    }

    /// <summary>
    /// Creates a complete MusicXML document with a simple melody.
    /// </summary>
    /// <param name="notes">The notes of the melody.</param>
    /// <param name="key">The key signature.</param>
    /// <param name="title">Optional title for the score.</param>
    /// <returns>Complete MusicXML document string.</returns>
    public static string CreateScore(IEnumerable<Note> notes, KeySignature key, string? title = null)
    {
        var notesList = notes.ToList();

        var scorePartwise = new XElement("score-partwise",
            new XAttribute("version", "4.0"),
            title != null ? new XElement("work",
                new XElement("work-title", title)
            ) : null,
            new XElement("part-list",
                new XElement("score-part",
                    new XAttribute("id", "P1"),
                    new XElement("part-name", "Music")
                )
            ),
            new XElement("part",
                new XAttribute("id", "P1"),
                new XElement("measure",
                    new XAttribute("number", "1"),
                    new XElement("attributes",
                        new XElement("divisions", "1"),
                        XElement.Parse(KeyToXml(key)),
                        new XElement("time",
                            new XElement("beats", "4"),
                            new XElement("beat-type", "4")
                        )
                    ),
                    notesList.Select(n => CreateNoteElement(n))
                )
            )
        );

        var declaration = new XDeclaration("1.0", "UTF-8", null);
        var doctype = "<!DOCTYPE score-partwise PUBLIC \"-//Recordare//DTD MusicXML 4.0 Partwise//EN\" \"http://www.musicxml.org/dtds/partwise.dtd\">";

        var sb = new StringBuilder();
        sb.AppendLine(declaration.ToString());
        sb.AppendLine(doctype);
        sb.Append(scorePartwise.ToString());

        return sb.ToString();
    }

    private static XElement CreateNoteElement(Note note)
    {
        return new XElement("note",
            XElement.Parse(NoteToXml(note)),
            new XElement("duration", "1"),
            new XElement("type", "quarter")
        );
    }

    private static string GetChordKind(Chord chord)
    {
        // MusicXML chord kind values
        return chord.Type switch
        {
            ChordType.Major => "major",
            ChordType.Minor => "minor",
            ChordType.Diminished => "diminished",
            ChordType.Augmented => "augmented",
            ChordType.Major7 => "major-seventh",
            ChordType.Minor7 => "minor-seventh",
            ChordType.Dominant7 => "dominant",
            ChordType.Diminished7 => "diminished-seventh",
            ChordType.HalfDiminished7 => "half-diminished",
            ChordType.MinorMajor7 => "major-minor",
            ChordType.Augmented7 => "augmented-seventh",
            ChordType.AugmentedMajor7 => "major-seventh", // Not directly supported
            ChordType.Dominant9 => "dominant-ninth",
            ChordType.Major9 => "major-ninth",
            ChordType.Minor9 => "minor-ninth",
            ChordType.Dominant11 => "dominant-11th",
            ChordType.Dominant13 => "dominant-13th",
            _ => chord.Quality switch
            {
                ChordQuality.Major => "major",
                ChordQuality.Minor => "minor",
                ChordQuality.Diminished => "diminished",
                ChordQuality.Augmented => "augmented",
                _ => "major"
            }
        };
    }
}
