using System.Diagnostics.CodeAnalysis;

namespace MusicTheory;

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
