using System.Diagnostics.CodeAnalysis;

namespace MusicTheory;

/// <summary>How <see cref="Solfege.FixedDo"/> writes accidentals.</summary>
public enum SolfegeStyle
{
    /// <summary>French words: <c>Ré dièse</c>.</summary>
    Words,
    /// <summary>ASCII symbols: <c>Ré#</c>.</summary>
    Ascii,
    /// <summary>Unicode symbols: <c>Ré♯</c>.</summary>
    Unicode
}

/// <summary>Which note a minor key calls <c>do</c> in movable do.</summary>
public enum MinorSolfege
{
    /// <summary>The relative major's tonic is do, so A minor starts on <c>la</c> (Kodály).</summary>
    LaBased,
    /// <summary>The minor tonic is do.</summary>
    DoBased
}

/// <summary>
/// Solfège note names: fixed do (<c>Do</c> is C) and movable do (the syllable names a scale degree of a key).
/// Fixed do is read through <see cref="Note.Parse(string, NoteNotation)"/> with <see cref="NoteNotation.FixedDo"/>.
/// </summary>
public static class Solfege
{
    // Longest first so "sol" is tried before the two-letter syllables. Ré and Re both mean D.
    private static (string Text, NoteName Letter)[] Syllables() => new[]
    {
        ("sol", NoteName.G), ("do", NoteName.C), ("ré", NoteName.D), ("re", NoteName.D),
        ("mi", NoteName.E), ("fa", NoteName.F), ("la", NoteName.A), ("si", NoteName.B),
    };

    // Longest first so "double dièse" wins over "dièse".
    private static (string Text, int Alteration)[] AccidentalWords() => new[]
    {
        ("double dièse", 2), ("double diese", 2), ("double bémol", -2), ("double bemol", -2),
        ("dièse", 1), ("diese", 1), ("bémol", -1), ("bemol", -1),
    };

    // Movable-do syllables by degree from do; columns are the alteration -1, 0, +1 against the key's own spelling.
    private static string?[][] MovableTable() => new[]
    {
        new string?[] { null, "do", "di" }, new string?[] { "ra", "re", "ri" }, new string?[] { "me", "mi", null },
        new string?[] { null, "fa", "fi" }, new string?[] { "se", "sol", "si" }, new string?[] { "le", "la", "li" },
        new string?[] { "te", "ti", null },
    };

    // The major key whose tonic is do: the relative major (la-based minor) or the parallel major (do-based minor).
    private static KeySignature DoKey(KeySignature key, MinorSolfege minor) =>
        key.Mode == KeyMode.Major ? key : minor == MinorSolfege.LaBased ? key.GetRelative() : key.GetParallel();

    /// <summary>
    /// The movable-do syllable of <paramref name="note"/> in <paramref name="key"/> (<c>fi</c> for C♯ in G major),
    /// or null when the note has no standard syllable (raised mi, anything beyond one accidental off the key).
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="note"/> or <paramref name="key"/> is null.</exception>
    public static string? MovableDo(Note note, KeySignature key, MinorSolfege minor = MinorSolfege.LaBased)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(key);
        var doKey = DoKey(key, minor);
        int degree = ((int)note.Name - (int)doKey.Tonic.Name + 7) % 7;
        int offset = (int)note.Alteration - (int)doKey.GetAlteration(note.Name);
        return Math.Abs(offset) > 1 ? null : MovableTable()[degree][offset + 1];
    }

    /// <summary>Reads a movable-do syllable (any case) in <paramref name="key"/>; the note is in octave 4. Never throws.</summary>
    public static bool TryParseMovableDo(string? text, KeySignature key, [NotNullWhen(true)] out Note? note,
        MinorSolfege minor = MinorSolfege.LaBased)
    {
        note = null;
        if (text is null)
            return false;
        var table = MovableTable();
        var doKey = DoKey(key, minor);
        for (int degree = 0; degree < 7; degree++)
            for (int offset = -1; offset <= 1; offset++)
            {
                if (!string.Equals(table[degree][offset + 1], text.Trim(), StringComparison.OrdinalIgnoreCase))
                    continue;
                var letter = (NoteName)(((int)doKey.Tonic.Name + degree) % 7);
                int alteration = (int)doKey.GetAlteration(letter) + offset;
                if (Math.Abs(alteration) > 2)
                    return false;
                note = new Note(letter, (Alteration)alteration);
                return true;
            }
        return false;
    }

    /// <summary>The fixed-do name of <paramref name="note"/>, without octave: <c>Ré dièse</c>, <c>Si♭</c>.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="note"/> is null.</exception>
    public static string FixedDo(Note note, SolfegeStyle style = SolfegeStyle.Words)
    {
        ArgumentNullException.ThrowIfNull(note);
        string syllable = note.Name switch
        {
            NoteName.C => "Do", NoteName.D => "Ré", NoteName.E => "Mi", NoteName.F => "Fa",
            NoteName.G => "Sol", NoteName.A => "La", _ => "Si",
        };
        // Indexed by alteration + 2 (double flat .. double sharp).
        string[] suffixes = style == SolfegeStyle.Words ? new[] { " double bémol", " bémol", "", " dièse", " double dièse" }
            : style == SolfegeStyle.Ascii ? new[] { "bb", "b", "", "#", "##" }
            : new[] { "𝄫", "♭", "", "♯", "𝄪" };
        return syllable + suffixes[(int)note.Alteration + 2];
    }

    internal static bool TryParseFixedDoPitchClass(ReadOnlySpan<char> s, [NotNullWhen(true)] out Note? note, out int consumed)
    {
        note = null;
        consumed = 0;
        foreach (var (text, letter) in Syllables())
        {
            if (!s.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                continue;
            int i = text.Length;
            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
            int alteration = 0;
            bool word = false;
            foreach (var (name, value) in AccidentalWords())
            {
                if (s[i..].StartsWith(name, StringComparison.OrdinalIgnoreCase))
                {
                    alteration = value;
                    i += name.Length;
                    word = true;
                    break;
                }
            }
            if (!word)
            {
                // Symbols: reuse the English reader on a dummy letter so the accidental rules stay in one place.
                Note.TryParsePitchClass(("C" + s[i..].ToString()).AsSpan(), out var symbols, out var used);
                alteration = (int)symbols!.Alteration;
                i += used - 1;
            }
            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
            note = new Note(letter, (Alteration)alteration);
            consumed = i;
            return true;
        }
        return false;
    }
}
