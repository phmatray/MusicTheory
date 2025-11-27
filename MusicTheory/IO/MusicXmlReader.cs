namespace MusicTheory.IO;

using System.Xml.Linq;

/// <summary>
/// Provides functionality to read music theory objects from MusicXML format.
/// </summary>
public static class MusicXmlReader
{
    /// <summary>
    /// Parses a MusicXML pitch element into a Note.
    /// </summary>
    /// <param name="pitchXml">XML string containing a pitch element.</param>
    /// <returns>The parsed Note.</returns>
    public static Note ParsePitch(string pitchXml)
    {
        var element = XElement.Parse(pitchXml);
        return ParsePitchElement(element);
    }

    /// <summary>
    /// Parses a MusicXML key element into a KeySignature.
    /// </summary>
    /// <param name="keyXml">XML string containing a key element.</param>
    /// <returns>The parsed KeySignature.</returns>
    public static KeySignature ParseKey(string keyXml)
    {
        var element = XElement.Parse(keyXml);
        return ParseKeyElement(element);
    }

    /// <summary>
    /// Parses a MusicXML harmony element into a Chord.
    /// </summary>
    /// <param name="harmonyXml">XML string containing a harmony element.</param>
    /// <returns>The parsed Chord.</returns>
    public static Chord ParseHarmony(string harmonyXml)
    {
        var element = XElement.Parse(harmonyXml);
        return ParseHarmonyElement(element);
    }

    /// <summary>
    /// Parses notes from a complete MusicXML score.
    /// </summary>
    /// <param name="scoreXml">XML string containing a score-partwise element.</param>
    /// <returns>A list of parsed notes.</returns>
    public static IReadOnlyList<Note> ParseScore(string scoreXml)
    {
        var notes = new List<Note>();
        var document = XDocument.Parse(scoreXml);

        var noteElements = document.Descendants("note")
            .Where(n => n.Element("pitch") != null); // Exclude rests

        foreach (var noteElement in noteElements)
        {
            var pitchElement = noteElement.Element("pitch");
            if (pitchElement != null)
            {
                notes.Add(ParsePitchElement(pitchElement));
            }
        }

        return notes;
    }

    /// <summary>
    /// Parses the key signature from a MusicXML score.
    /// </summary>
    /// <param name="scoreXml">XML string containing a score-partwise element.</param>
    /// <returns>The key signature, or C major if not found.</returns>
    public static KeySignature ParseScoreKey(string scoreXml)
    {
        var document = XDocument.Parse(scoreXml);
        var keyElement = document.Descendants("key").FirstOrDefault();

        if (keyElement != null)
        {
            return ParseKeyElement(keyElement);
        }

        // Default to C major
        return new KeySignature(new Note(NoteName.C), KeyMode.Major);
    }

    private static Note ParsePitchElement(XElement pitchElement)
    {
        var stepStr = pitchElement.Element("step")?.Value ?? "C";
        var alterStr = pitchElement.Element("alter")?.Value;
        var octaveStr = pitchElement.Element("octave")?.Value ?? "4";

        var noteName = ParseNoteName(stepStr);
        var alteration = alterStr != null
            ? (Alteration)int.Parse(alterStr)
            : Alteration.Natural;
        var octave = int.Parse(octaveStr);

        return new Note(noteName, alteration, octave);
    }

    private static KeySignature ParseKeyElement(XElement keyElement)
    {
        var fifthsStr = keyElement.Element("fifths")?.Value ?? "0";
        var modeStr = keyElement.Element("mode")?.Value ?? "major";

        var fifths = int.Parse(fifthsStr);
        var mode = modeStr.ToLowerInvariant() == "minor" ? KeyMode.Minor : KeyMode.Major;

        var tonic = GetTonicFromFifths(fifths, mode);
        return new KeySignature(tonic, mode);
    }

    private static Chord ParseHarmonyElement(XElement harmonyElement)
    {
        var rootElement = harmonyElement.Element("root");
        var kindElement = harmonyElement.Element("kind");

        var rootStep = rootElement?.Element("root-step")?.Value ?? "C";
        var rootAlter = rootElement?.Element("root-alter")?.Value;
        var kind = kindElement?.Value ?? "major";

        var noteName = ParseNoteName(rootStep);
        var alteration = rootAlter != null
            ? (Alteration)int.Parse(rootAlter)
            : Alteration.Natural;

        var root = new Note(noteName, alteration);
        var chordType = GetChordTypeFromKind(kind);

        return new Chord(root, chordType);
    }

    private static NoteName ParseNoteName(string step)
    {
        return step.ToUpperInvariant() switch
        {
            "C" => NoteName.C,
            "D" => NoteName.D,
            "E" => NoteName.E,
            "F" => NoteName.F,
            "G" => NoteName.G,
            "A" => NoteName.A,
            "B" => NoteName.B,
            _ => NoteName.C
        };
    }

    private static Note GetTonicFromFifths(int fifths, KeyMode mode)
    {
        // Circle of fifths mapping for major keys
        var majorKeys = new Dictionary<int, (NoteName name, Alteration alt)>
        {
            { -7, (NoteName.C, Alteration.Flat) },
            { -6, (NoteName.G, Alteration.Flat) },
            { -5, (NoteName.D, Alteration.Flat) },
            { -4, (NoteName.A, Alteration.Flat) },
            { -3, (NoteName.E, Alteration.Flat) },
            { -2, (NoteName.B, Alteration.Flat) },
            { -1, (NoteName.F, Alteration.Natural) },
            { 0, (NoteName.C, Alteration.Natural) },
            { 1, (NoteName.G, Alteration.Natural) },
            { 2, (NoteName.D, Alteration.Natural) },
            { 3, (NoteName.A, Alteration.Natural) },
            { 4, (NoteName.E, Alteration.Natural) },
            { 5, (NoteName.B, Alteration.Natural) },
            { 6, (NoteName.F, Alteration.Sharp) },
            { 7, (NoteName.C, Alteration.Sharp) }
        };

        // Circle of fifths mapping for minor keys (relative minors)
        var minorKeys = new Dictionary<int, (NoteName name, Alteration alt)>
        {
            { -7, (NoteName.A, Alteration.Flat) },
            { -6, (NoteName.E, Alteration.Flat) },
            { -5, (NoteName.B, Alteration.Flat) },
            { -4, (NoteName.F, Alteration.Natural) },
            { -3, (NoteName.C, Alteration.Natural) },
            { -2, (NoteName.G, Alteration.Natural) },
            { -1, (NoteName.D, Alteration.Natural) },
            { 0, (NoteName.A, Alteration.Natural) },
            { 1, (NoteName.E, Alteration.Natural) },
            { 2, (NoteName.B, Alteration.Natural) },
            { 3, (NoteName.F, Alteration.Sharp) },
            { 4, (NoteName.C, Alteration.Sharp) },
            { 5, (NoteName.G, Alteration.Sharp) },
            { 6, (NoteName.D, Alteration.Sharp) },
            { 7, (NoteName.A, Alteration.Sharp) }
        };

        var keys = mode == KeyMode.Major ? majorKeys : minorKeys;

        if (keys.TryGetValue(fifths, out var tonic))
        {
            return new Note(tonic.name, tonic.alt);
        }

        // Default to C natural
        return new Note(NoteName.C, Alteration.Natural);
    }

    private static ChordType GetChordTypeFromKind(string kind)
    {
        return kind.ToLowerInvariant() switch
        {
            "major" => ChordType.Major,
            "minor" => ChordType.Minor,
            "diminished" => ChordType.Diminished,
            "augmented" => ChordType.Augmented,
            "dominant" => ChordType.Dominant7,
            "major-seventh" => ChordType.Major7,
            "minor-seventh" => ChordType.Minor7,
            "diminished-seventh" => ChordType.Diminished7,
            "half-diminished" => ChordType.HalfDiminished7,
            "major-minor" => ChordType.MinorMajor7,
            "augmented-seventh" => ChordType.Augmented7,
            "dominant-ninth" => ChordType.Dominant9,
            "major-ninth" => ChordType.Major9,
            "minor-ninth" => ChordType.Minor9,
            "dominant-11th" => ChordType.Dominant11,
            "dominant-13th" => ChordType.Dominant13,
            "suspended-second" => ChordType.Major, // Not directly supported, default to Major
            "suspended-fourth" => ChordType.Major, // Not directly supported, default to Major
            "power" => ChordType.Major, // Not directly supported, default to Major
            _ => ChordType.Major
        };
    }
}
