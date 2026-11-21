namespace MusicTheory.Guitar;

/// <summary>One sounding position on a <see cref="Fretboard"/>.</summary>
/// <param name="String">Index into the tuning, 0 = lowest string as listed.</param>
/// <param name="Fret">Fret above the nut, 0 = open string.</param>
/// <param name="Note">The note sounding there, spelled as the searched note, scale or chord spells it.</param>
/// <param name="Degree">1-based scale degree, or the chord tone's interval number (1, 3, 5, 7, 9…); null for a single note.</param>
public sealed record FretPosition(int String, int Fret, Note Note, int? Degree);

/// <summary>A tuning and a fret range: names every fret and finds the positions of a note, scale or chord.</summary>
/// <param name="Tuning">The open strings.</param>
/// <param name="Frets">Frets above the nut, so 12 gives 13 positions per string.</param>
public sealed record Fretboard(Tuning Tuning, int Frets = 24)
{
    /// <summary>The tuning; never null.</summary>
    public Tuning Tuning { get; init => field = Check(value, Frets, value); } = Check(Tuning, Frets, Tuning);

    /// <summary>Frets above the nut; at least 1, and the highest open string plus this stays within MIDI 127.</summary>
    public int Frets { get; init => field = Check(Tuning, value, value); } = Check(Tuning, Frets, Frets);

    private static T Check<T>(Tuning tuning, int frets, T result)
    {
        ArgumentNullException.ThrowIfNull(tuning, nameof(Tuning));
        if (frets < 1 || tuning.Strings.Max(s => s.MidiNumber) + frets > 127)
            throw new ArgumentOutOfRangeException(nameof(Frets), frets, "Frets must be at least 1 and keep the highest string within MIDI 127.");
        return result;
    }

    /// <summary>The note at a string and fret, sharps for black keys like <see cref="Note.FromMidiNumber"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The string or fret is off the board.</exception>
    public Note NoteAt(int @string, int fret) => Note.FromMidiNumber(Midi(@string, fret));

    /// <summary>Every position of <paramref name="note"/>'s pitch class, spelled with its letter; Degree is null.</summary>
    public IReadOnlyList<FretPosition> Positions(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        return Find([(note, null)]);
    }

    /// <summary>Every position of a scale tone, spelled as the scale, Degree = 1-based scale degree.</summary>
    public IReadOnlyList<FretPosition> Positions(Scale scale)
    {
        ArgumentNullException.ThrowIfNull(scale);
        return Find(scale.GetNotes().SkipLast(1).Select((n, i) => (n, (int?)(i + 1))).ToList());
    }

    /// <summary>Every position of a chord tone, spelled as the chord, Degree = the tone's interval number (root is 1).</summary>
    public IReadOnlyList<FretPosition> Positions(Chord chord)
    {
        ArgumentNullException.ThrowIfNull(chord);
        var notes = chord.GetNotes().ToList();
        return Find([(notes[0], 1), .. notes.Skip(1).Zip(chord.Intervals, (n, iv) => (n, (int?)iv.Number))]);
    }

    private int Midi(int @string, int fret)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(@string);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(@string, Tuning.Strings.Count);
        ArgumentOutOfRangeException.ThrowIfNegative(fret);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(fret, Frets);
        return Tuning.Strings[@string].MidiNumber + fret;
    }

    private List<FretPosition> Find(IReadOnlyList<(Note Note, int? Degree)> targets)
    {
        var found = new List<FretPosition>();
        for (int s = 0; s < Tuning.Strings.Count; s++)
            for (int f = 0; f <= Frets; f++)
            {
                int midi = Midi(s, f);
                foreach (var (target, degree) in targets.Where(t => t.Note.PitchClass == midi % 12))
                    found.Add(new FretPosition(s, f, Note.FromMidiNumber(midi).Respell(target.Name)!, degree));
            }
        return found;
    }
}
