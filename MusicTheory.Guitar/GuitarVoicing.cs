namespace MusicTheory.Guitar;

/// <summary>Where a voicing comes from.</summary>
public enum VoicingSource
{
    /// <summary>A common open or first-position shape from the built-in table.</summary>
    Curated,
    /// <summary>A movable barre shape (E-form, A-form, D-form) placed on the neck.</summary>
    MovableShape,
    /// <summary>Found by searching the fretboard for the chord's notes.</summary>
    Generated
}

/// <summary>An index-finger barre across a range of strings.</summary>
/// <param name="Fret">The barred fret (relative to the capo).</param>
/// <param name="LowString">Lowest barred string (0 = lowest-pitched string).</param>
/// <param name="HighString">Highest barred string.</param>
public sealed record Barre(int Fret, int LowString, int HighString);

/// <summary>
/// A playable guitar voicing of a chord.
/// </summary>
/// <param name="Chord">The chord the shape plays (with a capo, the chord that sounds is transposed up by it).</param>
/// <param name="Frets">Per string, low to high: null = muted, 0 = open, otherwise the fret (relative to the capo).</param>
/// <param name="Fingers">Per string: null when not fretted, otherwise finger 1 (index) to 4 (little).</param>
/// <param name="Barre">The index-finger barre, if any.</param>
/// <param name="BaseFret">First fret of a 4-fret diagram window: 1 when the shape fits in frets 1–4.</param>
/// <param name="Source">Curated table, movable shape or generated.</param>
/// <param name="ShapeName">"open", "E-form", "A-form", "D-form", or null for generated voicings.</param>
/// <param name="Difficulty">Ranking score, lower is easier.</param>
public sealed record GuitarVoicing(
    ChordSymbol Chord,
    IReadOnlyList<int?> Frets,
    IReadOnlyList<int?> Fingers,
    Barre? Barre,
    int BaseFret,
    VoicingSource Source,
    string? ShapeName,
    double Difficulty)
{
    /// <summary>
    /// Tab notation low to high: <c>x32010</c>; frets above 9 are space-separated: <c>x 9 11 9 10 9</c>.
    /// </summary>
    public string ToTab()
    {
        var parts = Frets.Select(f => f?.ToString() ?? "x").ToList();
        return string.Join(parts.Any(p => p.Length > 1) ? " " : "", parts);
    }

    /// <summary>The sounding note per string (null when muted), spelled after the chord where possible.</summary>
    public IReadOnlyList<Note?> Sound(Tuning tuning, int capo = 0)
    {
        var chordTones = Chord.ToChord().GetNotes().Append(Chord.Bass).OfType<Note>().ToList();
        bool flats = Chord.Root.Alteration < 0;
        return Frets.Select((f, s) =>
        {
            if (f is not int fret)
                return null;
            int midi = tuning.Strings[s].MidiNumber + capo + fret;
            var tone = chordTones.FirstOrDefault(t => MusicTheoryUtilities.GetPitchClass(t) == midi % 12);
            if (tone is null)
                return Note.FromMidiNumber(midi, flats);
            int octave = (midi - MusicTheoryConstants.SemitonesFromC[(int)tone.Name] - (int)tone.Alteration) / 12 - 1;
            return new Note(tone.Name, tone.Alteration, octave);
        }).ToList();
    }
}

/// <summary>Options for <see cref="IGuitarVoicingProvider.GetVoicings"/>.</summary>
/// <param name="Tuning">Tuning, default <see cref="Guitar.Tuning.Standard"/>.</param>
/// <param name="Capo">Capo fret; frets are relative to it and the shape names the chord before the capo.</param>
/// <param name="MaxFret">Highest fret (absolute) a voicing may use.</param>
/// <param name="MaxStretch">Fret window a hand covers: fretted notes span at most this many frets.</param>
/// <param name="Max">Maximum number of voicings returned.</param>
/// <param name="IncludeSlashBass">Put a slash chord's bass lowest; when false the bass is ignored.</param>
public sealed record VoicingOptions(Tuning? Tuning = null, int Capo = 0, int MaxFret = 15,
                                    int MaxStretch = 4, int Max = 8, bool IncludeSlashBass = true);
