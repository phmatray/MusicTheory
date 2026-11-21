using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Piano;

/// <summary>A marked key: MIDI number, note label and the finger the library gives it.</summary>
public sealed record PianoMark(int Midi, string Label, int Finger);

public sealed record PianoView(IReadOnlyList<PianoKey> Keys, IReadOnlyList<PianoMark> Marks, string? Notice);

public static class PianoModel
{
    public const int MaxOctaves = 2;

    /// <summary>Only the heptatonic scales have a standard fingering.</summary>
    public static readonly IReadOnlyList<ScaleType> ScaleTypes =
    [
        ScaleType.Major, ScaleType.NaturalMinor, ScaleType.HarmonicMinor, ScaleType.MelodicMinor,
        ScaleType.Dorian, ScaleType.Phrygian, ScaleType.Lydian, ScaleType.Mixolydian, ScaleType.Locrian,
    ];

    /// <summary>Fingered keys for a scale (or, with <paramref name="triad"/>, its tonic triad). Anything invalid falls back (C, Major, 1 octave) and sets the notice.</summary>
    public static PianoView For(string? root, string? type, Hand hand, int octaves, Lang lang, bool triad = false)
    {
        var bad = new List<string>();
        var found = ScaleTypes.Cast<ScaleType?>().FirstOrDefault(t => string.Equals(t.ToString(), type, StringComparison.OrdinalIgnoreCase));
        var scaleType = found ?? ScaleType.Major;
        if (found is null && !string.IsNullOrEmpty(type)) bad.Add("type");
        if (octaves < 1 || octaves > MaxOctaves) { octaves = 1; bad.Add("oct"); }
        var tonic = Note.TryParse(root + "4", out var parsed) ? parsed : Note.Parse("C4");
        if (parsed is null && !string.IsNullOrEmpty(root)) bad.Add("root");

        List<PianoMark> marks;
        try { marks = Marks(tonic, scaleType, hand, octaves, lang, triad); }
        catch (Exception e) when (e is InvalidOperationException or ArgumentException)
        {
            // A root needing triple accidentals, or beyond MIDI: show C instead of throwing.
            bad.Add("root");
            marks = Marks(Note.Parse("C4"), scaleType, hand, octaves, lang, triad);
        }

        var low = marks.Min(m => m.Midi) - 1;
        var high = marks.Max(m => m.Midi) + 1;
        if (IsBlack(low)) low--;
        if (IsBlack(high)) high++;
        var notice = bad.Count == 0 ? null : Strings.T(lang, "piano.invalid", string.Join(", ", bad.Distinct()));
        return new PianoView(PianoLayout.Keys(low, high), marks, notice);
    }

    private static bool IsBlack(int midi) => (midi % 12) is 1 or 3 or 6 or 8 or 10;

    private static List<PianoMark> Marks(Note tonic, ScaleType type, Hand hand, int octaves, Lang lang, bool triad)
    {
        var notes = new Scale(tonic, type).GetNotes().ToList();   // 8 notes, tonic to octave
        if (triad)
        {
            var chord = new[] { notes[0], notes[2], notes[4] };
            var f = PianoFingering.ForTriad(chord, hand);
            return chord.Select((n, i) => new PianoMark(n.MidiNumber, NoteNames.Format(n, lang), f[i])).ToList();
        }

        var fingers = PianoFingering.ForScale(new Scale(tonic, type), hand, octaves);
        return Enumerable.Range(0, 7 * octaves + 1)
            .Select(k => (Note: notes[k % 7], Shift: 12 * (k / 7), K: k))
            .Select(x => new PianoMark(x.Note.MidiNumber + x.Shift, NoteNames.Format(x.Note, lang), fingers[x.K]))
            .ToList();
    }
}
