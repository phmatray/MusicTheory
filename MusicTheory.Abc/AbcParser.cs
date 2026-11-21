using System.Globalization;

namespace MusicTheory.Abc;

internal sealed class AbcParser
{
    private readonly string _s;
    private readonly KeySignature _key;
    private readonly Rational _unit;
    private readonly List<List<RhythmEvent>> _bars = [];
    private readonly Dictionary<(NoteName, int), Alteration> _accidentals = [];
    private List<RhythmEvent> _current = [];
    private int _p;
    private int _tuplet;

    private AbcParser(string body, KeySignature key, Rational unit)
    {
        _s = body + '\0'; // a sentinel, so no read needs a bounds check
        _key = key;
        _unit = unit;
    }

    internal static AbcTune Parse(string text)
    {
        var lines = text.Split('\n').Where(l => !l.TrimStart().StartsWith('%')).Select(StripComment).ToList();
        var i = 0;
        while (i < lines.Count && lines[i].Length == 0)
            i++;
        if (i == lines.Count || !lines[i].StartsWith("X:", StringComparison.Ordinal))
            throw new FormatException("An ABC tune starts with an X: line.");

        var reference = 0;
        string? title = null;
        TimeSignature? meter = null;
        Rational? unit = null;
        TempoMap? tempo = null;
        KeySignature? key = null;
        for (; i < lines.Count && lines[i].Length > 0 && key is null; i++)
        {
            var line = lines[i];
            if (line.Length < 2 || line[1] != ':')
                throw new FormatException($"Not a header field: '{line}'.");
            var value = line[2..].Trim();
            switch (line[0])
            {
                case 'X': reference = int.Parse(value, CultureInfo.InvariantCulture); break;
                case 'T': title ??= value; break;
                case 'M': meter = ParseMeter(value); break;
                case 'L': unit = ParseFraction(value); break;
                case 'Q': tempo = ParseTempo(value); break;
                case 'K': key = ParseKey(value); break;
                case 'V' or 'w': throw Unsupported(line[0]);
            }
        }

        if (key is null)
            throw new FormatException("An ABC tune needs a K: line.");

        var body = new List<string>();
        for (; i < lines.Count && lines[i].Length > 0 && !lines[i].StartsWith("X:", StringComparison.Ordinal); i++)
        {
            if (lines[i].StartsWith("V:", StringComparison.Ordinal) || lines[i].StartsWith("w:", StringComparison.Ordinal))
                throw Unsupported(lines[i][0]);
            body.Add(lines[i]);
        }

        meter ??= TimeSignature.CommonTime;
        var length = unit ?? (meter.MeasureLength < Rational.Of(3, 4) ? Rational.Of(1, 16) : Rational.Of(1, 8));
        var bars = new AbcParser(string.Join(' ', body), key, length).Read();
        var measures = new List<Measure>();
        foreach (var events in bars)
        {
            var measure = new Measure(meter, events);
            // A first bar shorter than the meter is a pickup.
            measures.Add(measures.Count == 0 && !measure.IsComplete ? new Measure(meter, events, measure.Filled) : measure);
        }

        return new AbcTune(reference, title, meter, length, key, tempo, measures);
    }

    private static NotSupportedException Unsupported(char field) =>
        new($"The {field}: field (voices and lyrics) is not supported.");

    private static string StripComment(string line)
    {
        var at = line.IndexOf('%');
        return (at < 0 ? line : line[..at]).Trim();
    }

    private static TimeSignature ParseMeter(string value)
    {
        if (value == "C")
            return TimeSignature.CommonTime;
        if (value == "C|")
            return TimeSignature.CutTime;
        var parts = value.Split('/');
        if (parts.Length != 2)
            throw new FormatException($"Not a meter: '{value}'.");
        // Not through Rational, which reduces 2/4 to 1/2.
        return new TimeSignature(int.Parse(parts[0], CultureInfo.InvariantCulture), int.Parse(parts[1], CultureInfo.InvariantCulture));
    }

    private static Rational ParseFraction(string value)
    {
        var parts = value.Split('/');
        if (parts.Length != 2)
            throw new FormatException($"Not a fraction: '{value}'.");
        return Rational.Of(long.Parse(parts[0], CultureInfo.InvariantCulture), long.Parse(parts[1], CultureInfo.InvariantCulture));
    }

    private static TempoMap ParseTempo(string value)
    {
        // "1/4=120", or a bare "120" meaning quarter notes.
        var eq = value.IndexOf('=');
        var bpm = double.Parse(value[(eq + 1)..], NumberStyles.Float, CultureInfo.InvariantCulture);
        if (eq < 0)
            return new TempoMap(bpm);
        return Duration.TryFromValue(ParseFraction(value[..eq].Trim()), out var beat)
            ? new TempoMap(bpm, beat)
            : throw new FormatException($"Not a beat unit: '{value}'.");
    }

    private static KeySignature ParseKey(string value)
    {
        if (value == "none")
            return new KeySignature(new Note(NoteName.C), KeyMode.Major);
        var span = value.AsSpan();
        if (!Note.TryParsePitchClass(span, out var tonic, out var consumed))
            throw new FormatException($"Not a key: '{value}'.");
        var word = span[consumed..].Trim().ToString().Split(' ')[0].ToLowerInvariant();
        if (word == "m" || word.StartsWith("min", StringComparison.Ordinal))
            return new KeySignature(tonic, KeyMode.Minor);

        // Each mode is the major key this many fifths from its tonic.
        string[] modes = ["", "maj", "ion", "lyd", "mix", "dor", "aeo", "phr", "loc"];
        int[] shifts = [0, 0, 0, 1, -1, -2, -3, -4, -5];
        var at = Array.IndexOf(modes, word.Length > 3 ? word[..3] : word);
        if (at < 0)
            throw new FormatException($"Not a key: '{value}'.");
        var fifths = shifts[at];
        var key = new KeySignature(tonic, KeyMode.Major);
        for (; fifths > 0; fifths--)
            key = key.NextInCircle();
        for (; fifths < 0; fifths++)
            key = key.PreviousInCircle();
        return key;
    }

    private List<List<RhythmEvent>> Read()
    {
        while (_s[_p] != '\0')
        {
            var c = _s[_p];
            if (char.IsWhiteSpace(c) || c == ')')
                _p++;
            else if (c == '"')
                Skip('"');
            else if (c == '!')
                Skip('!');
            else if (c == '{')
                Skip('}');
            else if ("|:".Contains(c))
                Bar();
            else if (c == '-')
                Tie();
            else if (c == '(')
                OpenParen();
            else if ("zx".Contains(c))
            {
                _p++;
                Add([], ReadLength());
            }
            else if (c == '[')
                Chord();
            else
                Add([ReadNote()], ReadLength());
        }

        if (_current.Count > 0)
            _bars.Add(_current);
        return _bars;
    }

    private FormatException Fail(string message) => new($"Bar {_bars.Count + 1}: {message}");

    /// <summary>Moves past the next <paramref name="close"/> after the character at the cursor.</summary>
    private void Skip(char close)
    {
        var at = _s.IndexOf(close, _p + 1);
        _p = at < 0 ? throw Fail($"'{_s[_p]}' is never closed.") : at + 1;
    }

    private void Bar()
    {
        while ("|:]".Contains(_s[_p]))
            _p++;
        if (_current.Count > 0)
            _bars.Add(_current);
        _current = [];
        _accidentals.Clear();
    }

    private void Tie()
    {
        _p++;
        if (_current.Count == 0 || _current[^1].IsRest)
            throw Fail("a tie needs a note before it.");
        _current[^1] = _current[^1] with { TiedToNext = true };
    }

    private void OpenParen()
    {
        _p++;
        if (!char.IsAsciiDigit(_s[_p]))
            return; // a slur, which does not change the notes
        _tuplet = _s[_p] == '3' ? 3 : throw Fail($"only triplets '(3' are supported, not '({_s[_p]}'.");
        _p++;
    }

    private void Chord()
    {
        _p++;
        List<Note> pitches = [];
        var inner = Rational.One;
        while (_s[_p] != ']')
        {
            pitches.Add(ReadNote());
            var length = ReadLength();
            if (pitches.Count == 1)
                inner = length;
        }

        _p++;
        Add(pitches, inner * ReadLength());
    }

    private Note ReadNote()
    {
        Alteration? written = null;
        if (_s[_p] == '=')
        {
            written = Alteration.Natural;
            _p++;
        }
        else if ("^_".Contains(_s[_p]))
        {
            var sign = _s[_p];
            var count = 0;
            for (; _s[_p] == sign; _p++)
                count++;
            written = (Alteration)(sign == '^' ? count : -count);
        }

        var letter = _s[_p];
        if (!"ABCDEFGabcdefg".Contains(letter))
            throw Fail($"unexpected '{(letter == '\0' ? "end of tune" : letter)}'.");
        _p++;
        var name = Enum.Parse<NoteName>(char.ToUpperInvariant(letter).ToString());
        var octave = char.IsUpper(letter) ? 4 : 5;
        for (; ",'".Contains(_s[_p]); _p++)
            octave += _s[_p] == '\'' ? 1 : -1;

        // An accidental lasts to the end of the bar, for that letter in that octave.
        var alteration = written ?? (_accidentals.TryGetValue((name, octave), out var seen) ? seen : _key.GetAlteration(name));
        _accidentals[(name, octave)] = alteration;
        return new Note(name, alteration, octave);
    }

    /// <summary>Reads <c>2</c>, <c>3/2</c>, <c>/2</c>, <c>/</c> or <c>//</c> as a factor of the unit length.</summary>
    private Rational ReadLength()
    {
        var numerator = ReadInt() ?? 1;
        var denominator = 1L;
        if (_s[_p] == '/')
        {
            var slashes = 0;
            for (; _s[_p] == '/'; _p++)
                slashes++;
            denominator = ReadInt() ?? 1L << slashes;
        }

        return Rational.Of(numerator, denominator);
    }

    private long? ReadInt()
    {
        var start = _p;
        while (char.IsAsciiDigit(_s[_p]))
            _p++;
        return _p > start ? long.Parse(_s.AsSpan(start.._p), CultureInfo.InvariantCulture) : null;
    }

    private void Add(IReadOnlyList<Note> pitches, Rational factor)
    {
        var value = factor * _unit;
        var tuplet = _tuplet > 0;
        if (tuplet)
            _tuplet--;
        if (!Duration.TryFromValue(value, out var duration) || (tuplet && duration.Dots > 0))
            throw Fail($"a length of {value} whole notes cannot be notated{(tuplet ? " in a triplet" : "")}.");
        _current.Add(new RhythmEvent(pitches, tuplet ? Duration.CreateTuplet(duration.Type, 3, 2) : duration));
    }
}
