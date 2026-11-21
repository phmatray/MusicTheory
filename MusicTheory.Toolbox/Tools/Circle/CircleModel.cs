using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Circle;

/// <summary>One key of the ring, already spelled for a language.</summary>
public sealed record CircleKey(string Id, string Title, string RelativeMinor, int Accidentals, string Signature, IReadOnlyList<string> ScaleNotes);

/// <summary>The chosen key, and whether the requested one was unknown (so C is shown instead).</summary>
public sealed record CircleView(CircleKey Key, bool Fallback);

public static class CircleModel
{
    // The ring, clockwise from C. F# (not Gb) at the bottom: the library spells the six-accidental key with sharps.
    private static readonly string[] Tonics = ["C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F"];

    /// <summary>The twelve major keys, clockwise from C.</summary>
    public static IReadOnlyList<CircleKey> Keys(Lang lang) => Tonics.Select(t => Build(Note.Parse(t + "4"), lang)).ToList();

    /// <summary>The key named by <paramref name="key"/> (<c>G</c>, <c>Eb</c>, <c>Em</c> = its relative major); C when it is not one of the twelve.</summary>
    public static CircleView For(string? key, Lang lang)
    {
        var keys = Keys(lang);
        if (KeySignature.TryParse(key, out var parsed))
        {
            var major = parsed.Mode == KeyMode.Minor ? parsed.GetRelative() : parsed;
            var index = Array.FindIndex(Tonics, t => Note.Parse(t + "4").PitchClass == major.Tonic.PitchClass);
            if (index >= 0) return new CircleView(keys[index], false);
        }
        return new CircleView(keys[0], key is not null);
    }

    private static CircleKey Build(Note tonic, Lang lang)
    {
        var major = new KeySignature(tonic, KeyMode.Major);
        var n = major.AccidentalCount;
        var notes = new Scale(tonic, ScaleType.Major).GetNotes().Take(7).Select(x => NoteNames.Format(x, lang)).ToList();
        var signature = n switch
        {
            0 => Strings.T(lang, "circle.sig.none"),
            1 => Strings.T(lang, "circle.sig.sharp.one"),
            > 1 => Strings.T(lang, "circle.sig.sharp.many", n),
            -1 => Strings.T(lang, "circle.sig.flat.one"),
            _ => Strings.T(lang, "circle.sig.flat.many", -n),
        };
        return new CircleKey(
            tonic.GetName(),
            Strings.T(lang, "circle.major", NoteNames.Format(tonic, lang)),
            Strings.T(lang, "circle.minor", NoteNames.Format(major.GetRelative().Tonic, lang)),
            n, signature, notes);
    }
}
