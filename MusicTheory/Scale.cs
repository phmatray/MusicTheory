namespace MusicTheory;

/// <summary>
/// Scale types, each defined by its intervals above the tonic.
/// </summary>
public enum ScaleType
{
    /// <summary>Major (P1 M2 M3 P4 P5 M6 M7)</summary>
    Major,
    /// <summary>Natural minor (P1 M2 m3 P4 P5 m6 m7)</summary>
    NaturalMinor,
    /// <summary>Harmonic minor (P1 M2 m3 P4 P5 m6 M7)</summary>
    HarmonicMinor,
    /// <summary>Melodic minor, ascending form (P1 M2 m3 P4 P5 M6 M7)</summary>
    MelodicMinor,
    /// <summary>Ionian mode, the major scale</summary>
    Ionian,
    /// <summary>Dorian mode (P1 M2 m3 P4 P5 M6 m7)</summary>
    Dorian,
    /// <summary>Phrygian mode (P1 m2 m3 P4 P5 m6 m7)</summary>
    Phrygian,
    /// <summary>Lydian mode (P1 M2 M3 A4 P5 M6 M7)</summary>
    Lydian,
    /// <summary>Mixolydian mode (P1 M2 M3 P4 P5 M6 m7)</summary>
    Mixolydian,
    /// <summary>Aeolian mode, the natural minor scale</summary>
    Aeolian,
    /// <summary>Locrian mode (P1 m2 m3 P4 d5 m6 m7)</summary>
    Locrian,
    /// <summary>Chromatic scale, ascending with sharps (P1 A1 M2 A2 M3 P4 A4 P5 A5 M6 A6 M7)</summary>
    Chromatic,
    /// <summary>Whole-tone scale (P1 M2 M3 A4 A5 A6)</summary>
    WholeTone,
    /// <summary>Blues scale (P1 m3 P4 A4 P5 m7)</summary>
    Blues,
    /// <summary>Major pentatonic (P1 M2 M3 P5 M6)</summary>
    PentatonicMajor,
    /// <summary>Minor pentatonic (P1 m3 P4 P5 m7)</summary>
    PentatonicMinor
}

/// <summary>
/// A scale on a tonic. Notes are spelled from the tonic by interval, so F# major has E# and Gb major has Cb.
/// </summary>
/// <remarks>Every scale's notes run from the tonic to the tonic an octave above, both included.</remarks>
public sealed record Scale
{
    private static readonly Dictionary<ScaleType, Interval[]> Formulas = new()
    {
        [ScaleType.Major] = Of("M2 M3 P4 P5 M6 M7"),
        [ScaleType.Ionian] = Of("M2 M3 P4 P5 M6 M7"),
        [ScaleType.NaturalMinor] = Of("M2 m3 P4 P5 m6 m7"),
        [ScaleType.Aeolian] = Of("M2 m3 P4 P5 m6 m7"),
        [ScaleType.HarmonicMinor] = Of("M2 m3 P4 P5 m6 M7"),
        [ScaleType.MelodicMinor] = Of("M2 m3 P4 P5 M6 M7"),
        [ScaleType.Dorian] = Of("M2 m3 P4 P5 M6 m7"),
        [ScaleType.Phrygian] = Of("m2 m3 P4 P5 m6 m7"),
        [ScaleType.Lydian] = Of("M2 M3 A4 P5 M6 M7"),
        [ScaleType.Mixolydian] = Of("M2 M3 P4 P5 M6 m7"),
        [ScaleType.Locrian] = Of("m2 m3 P4 d5 m6 m7"),
        [ScaleType.Chromatic] = Of("A1 M2 A2 M3 P4 A4 P5 A5 M6 A6 M7"),
        [ScaleType.WholeTone] = Of("M2 M3 A4 A5 A6"),
        [ScaleType.Blues] = Of("m3 P4 A4 P5 m7"),
        [ScaleType.PentatonicMajor] = Of("M2 M3 P5 M6"),
        [ScaleType.PentatonicMinor] = Of("m3 P4 P5 m7"),
    };

    private static Interval[] Of(string names) =>
        $"P1 {names} P8".Split(' ').Select(Interval.Parse).ToArray();

    /// <summary>The tonic.</summary>
    public Note Root { get; }

    /// <summary>The scale type.</summary>
    public ScaleType Type { get; }

    /// <summary>Creates a scale.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="root"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/> is undefined.</exception>
    public Scale(Note root, ScaleType type)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), type, "Undefined scale type.");
        Root = root;
        Type = type;
    }

    /// <summary>The intervals above the tonic, from P1 to P8.</summary>
    public IReadOnlyList<Interval> Intervals => Formulas[Type];

    /// <summary>The notes from the tonic to its octave, both included: C major is C4 D4 E4 F4 G4 A4 B4 C5.</summary>
    /// <exception cref="InvalidOperationException">A degree needs more than two accidentals (B## major).</exception>
    public IEnumerable<Note> GetNotes()
    {
        var root = Root;
        return Formulas[Type].Select(root.Transpose).ToList();
    }

    /// <summary>The same scale on a tonic transposed by <paramref name="interval"/>.</summary>
    public Scale Transpose(Interval interval) => new(Root.Transpose(interval), Type);

    /// <summary>
    /// The scale note after <paramref name="note"/>, in the right octave: in C major, B4 is followed by C5.
    /// The octave of <paramref name="note"/> is free; only its spelling must belong to the scale.
    /// </summary>
    /// <exception cref="ArgumentException">The note is not in the scale.</exception>
    public Note GetNextNoteInScale(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        var notes = GetNotes().ToList();
        int index = IndexOf(notes, note)
            ?? throw new ArgumentException($"{note.GetName()} is not in {Root.GetName()} {Type}.", nameof(note));
        return note.Transpose(Interval.Between(notes[index], notes[index + 1]));
    }

    /// <summary>True when the scale contains a note with this spelling (any octave).</summary>
    public bool Contains(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        return IndexOf(GetNotes().ToList(), note) is not null;
    }

    /// <summary>The 1-based degree of a note with this spelling (any octave), or null when absent.</summary>
    public int? GetDegree(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        return IndexOf(GetNotes().ToList(), note) + 1;
    }

    // Index among the distinct degrees (the closing octave excluded).
    private static int? IndexOf(List<Note> notes, Note note)
    {
        int index = notes.FindIndex(0, notes.Count - 1, n => n.Name == note.Name && n.Alteration == note.Alteration);
        return index < 0 ? null : index;
    }

    /// <summary>The scale as <c>C4 Major</c>.</summary>
    public override string ToString() => $"{Root} {Type}";
}
