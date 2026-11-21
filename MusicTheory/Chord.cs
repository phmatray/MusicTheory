namespace MusicTheory;

/// <summary>
/// The family of a chord's triad.
/// </summary>
public enum ChordQuality
{
    /// <summary>Major third (and a perfect or altered fifth that is not augmented).</summary>
    Major,
    /// <summary>Minor third, perfect fifth.</summary>
    Minor,
    /// <summary>Minor third, diminished fifth (also half-diminished and diminished sevenths).</summary>
    Diminished,
    /// <summary>Major third, augmented fifth.</summary>
    Augmented,
    /// <summary>No third: a 2nd or 4th takes its place (sus2, sus4, 7sus4).</summary>
    Suspended,
    /// <summary>Root and fifth only.</summary>
    Power
}

/// <summary>
/// The chord types, each defined by its intervals above the root (see <see cref="Chord.Intervals"/>).
/// </summary>
public enum ChordType
{
    /// <summary>Major triad (1-3-5)</summary>
    Major,
    /// <summary>Minor triad (1-♭3-5)</summary>
    Minor,
    /// <summary>Diminished triad (1-♭3-♭5)</summary>
    Diminished,
    /// <summary>Augmented triad (1-3-♯5)</summary>
    Augmented,
    /// <summary>Major 7th (1-3-5-7)</summary>
    Major7,
    /// <summary>Minor 7th (1-♭3-5-♭7)</summary>
    Minor7,
    /// <summary>Dominant 7th (1-3-5-♭7)</summary>
    Dominant7,
    /// <summary>Minor major 7th (1-♭3-5-7)</summary>
    MinorMajor7,
    /// <summary>Half-diminished 7th (1-♭3-♭5-♭7)</summary>
    HalfDiminished7,
    /// <summary>Diminished 7th (1-♭3-♭5-𝄫7)</summary>
    Diminished7,
    /// <summary>Augmented 7th (1-3-♯5-♭7)</summary>
    Augmented7,
    /// <summary>Augmented major 7th (1-3-♯5-7)</summary>
    AugmentedMajor7,
    /// <summary>Suspended 2nd (1-2-5)</summary>
    Sus2,
    /// <summary>Suspended 4th (1-4-5)</summary>
    Sus4,
    /// <summary>Suspended 2nd and 4th (1-2-4-5)</summary>
    Sus2Sus4,
    /// <summary>Dominant 7th suspended 4th (1-4-5-♭7)</summary>
    Dominant7Sus4,
    /// <summary>Major 6th (1-3-5-6)</summary>
    Major6,
    /// <summary>Minor 6th (1-♭3-5-6)</summary>
    Minor6,
    /// <summary>Major 6th add 9 (1-3-5-6-9)</summary>
    Major6Add9,
    /// <summary>Major 9th (1-3-5-7-9)</summary>
    Major9,
    /// <summary>Minor 9th (1-♭3-5-♭7-9)</summary>
    Minor9,
    /// <summary>Dominant 9th (1-3-5-♭7-9)</summary>
    Dominant9,
    /// <summary>Major 11th (1-3-5-7-9-11)</summary>
    Major11,
    /// <summary>Minor 11th (1-♭3-5-♭7-9-11)</summary>
    Minor11,
    /// <summary>Dominant 11th (1-3-5-♭7-9-11)</summary>
    Dominant11,
    /// <summary>Major 13th (1-3-5-7-9-11-13)</summary>
    Major13,
    /// <summary>Minor 13th (1-♭3-5-♭7-9-11-13)</summary>
    Minor13,
    /// <summary>Dominant 13th (1-3-5-♭7-9-11-13)</summary>
    Dominant13,
    /// <summary>Major add 9 (1-3-5-9)</summary>
    MajorAdd9,
    /// <summary>Minor add 9 (1-♭3-5-9)</summary>
    MinorAdd9,
    /// <summary>Major add 11 (1-3-5-11)</summary>
    MajorAdd11,
    /// <summary>Minor add 11 (1-♭3-5-11)</summary>
    MinorAdd11,
    /// <summary>Dominant 7th flat 5 (1-3-♭5-♭7)</summary>
    Dominant7Flat5,
    /// <summary>Dominant 7th sharp 5 (1-3-♯5-♭7)</summary>
    Dominant7Sharp5,
    /// <summary>Dominant 7th flat 9 (1-3-5-♭7-♭9)</summary>
    Dominant7Flat9,
    /// <summary>Dominant 7th sharp 9 (1-3-5-♭7-♯9)</summary>
    Dominant7Sharp9,
    /// <summary>Dominant 7th flat 5 flat 9 (1-3-♭5-♭7-♭9)</summary>
    Dominant7Flat5Flat9,
    /// <summary>Dominant 7th flat 5 sharp 9 (1-3-♭5-♭7-♯9)</summary>
    Dominant7Flat5Sharp9,
    /// <summary>Dominant 7th sharp 5 flat 9 (1-3-♯5-♭7-♭9)</summary>
    Dominant7Sharp5Flat9,
    /// <summary>Dominant 7th sharp 5 sharp 9 (1-3-♯5-♭7-♯9)</summary>
    Dominant7Sharp5Sharp9,
    /// <summary>Dominant 7th altered (1-3-♭5-♯5-♭7-♭9)</summary>
    Dominant7Alt,
    /// <summary>Power chord (1-5)</summary>
    Power5
}

/// <summary>
/// The one table of chord types: intervals above the root and the canonical lead-sheet suffix.
/// </summary>
internal static class ChordTypes
{
    private static readonly Dictionary<ChordType, (Interval[] Intervals, string Suffix)> Table = new()
    {
        [ChordType.Major] = (Of("M3 P5"), ""),
        [ChordType.Minor] = (Of("m3 P5"), "m"),
        [ChordType.Diminished] = (Of("m3 d5"), "dim"),
        [ChordType.Augmented] = (Of("M3 A5"), "aug"),
        [ChordType.Major7] = (Of("M3 P5 M7"), "maj7"),
        [ChordType.Minor7] = (Of("m3 P5 m7"), "m7"),
        [ChordType.Dominant7] = (Of("M3 P5 m7"), "7"),
        [ChordType.MinorMajor7] = (Of("m3 P5 M7"), "mMaj7"),
        [ChordType.HalfDiminished7] = (Of("m3 d5 m7"), "m7b5"),
        [ChordType.Diminished7] = (Of("m3 d5 d7"), "dim7"),
        [ChordType.Augmented7] = (Of("M3 A5 m7"), "aug7"),
        [ChordType.AugmentedMajor7] = (Of("M3 A5 M7"), "maj7#5"),
        [ChordType.Sus2] = (Of("M2 P5"), "sus2"),
        [ChordType.Sus4] = (Of("P4 P5"), "sus4"),
        [ChordType.Sus2Sus4] = (Of("M2 P4 P5"), "sus2sus4"),
        [ChordType.Dominant7Sus4] = (Of("P4 P5 m7"), "7sus4"),
        [ChordType.Major6] = (Of("M3 P5 M6"), "6"),
        [ChordType.Minor6] = (Of("m3 P5 M6"), "m6"),
        [ChordType.Major6Add9] = (Of("M3 P5 M6 M9"), "6/9"),
        [ChordType.Major9] = (Of("M3 P5 M7 M9"), "maj9"),
        [ChordType.Minor9] = (Of("m3 P5 m7 M9"), "m9"),
        [ChordType.Dominant9] = (Of("M3 P5 m7 M9"), "9"),
        [ChordType.Major11] = (Of("M3 P5 M7 M9 P11"), "maj11"),
        [ChordType.Minor11] = (Of("m3 P5 m7 M9 P11"), "m11"),
        [ChordType.Dominant11] = (Of("M3 P5 m7 M9 P11"), "11"),
        [ChordType.Major13] = (Of("M3 P5 M7 M9 P11 M13"), "maj13"),
        [ChordType.Minor13] = (Of("m3 P5 m7 M9 P11 M13"), "m13"),
        [ChordType.Dominant13] = (Of("M3 P5 m7 M9 P11 M13"), "13"),
        [ChordType.MajorAdd9] = (Of("M3 P5 M9"), "add9"),
        [ChordType.MinorAdd9] = (Of("m3 P5 M9"), "madd9"),
        [ChordType.MajorAdd11] = (Of("M3 P5 P11"), "add11"),
        [ChordType.MinorAdd11] = (Of("m3 P5 P11"), "madd11"),
        [ChordType.Dominant7Flat5] = (Of("M3 d5 m7"), "7b5"),
        [ChordType.Dominant7Sharp5] = (Of("M3 A5 m7"), "7#5"),
        [ChordType.Dominant7Flat9] = (Of("M3 P5 m7 m9"), "7b9"),
        [ChordType.Dominant7Sharp9] = (Of("M3 P5 m7 A9"), "7#9"),
        [ChordType.Dominant7Flat5Flat9] = (Of("M3 d5 m7 m9"), "7b5b9"),
        [ChordType.Dominant7Flat5Sharp9] = (Of("M3 d5 m7 A9"), "7b5#9"),
        [ChordType.Dominant7Sharp5Flat9] = (Of("M3 A5 m7 m9"), "7#5b9"),
        [ChordType.Dominant7Sharp5Sharp9] = (Of("M3 A5 m7 A9"), "7#5#9"),
        [ChordType.Dominant7Alt] = (Of("M3 d5 A5 m7 m9"), "7alt"),
        [ChordType.Power5] = (Of("P5"), "5"),
    };

    private static Interval[] Of(string names) => names.Split(' ').Select(Interval.Parse).ToArray();

    /// <summary>All chord types, in declaration order.</summary>
    public static IEnumerable<ChordType> All => Table.Keys;

    /// <summary>The intervals above the root.</summary>
    public static IReadOnlyList<Interval> Intervals(ChordType type) => Table[Checked(type)].Intervals;

    /// <summary>The canonical ASCII suffix, e.g. <c>m7b5</c>.</summary>
    public static string Suffix(ChordType type) => Table[Checked(type)].Suffix;

    /// <summary>Throws for a value outside the enum.</summary>
    public static ChordType Checked(ChordType type, string? paramName = null) => Enum.IsDefined(type)
        ? type
        : throw new ArgumentOutOfRangeException(paramName ?? nameof(type), type, "Undefined chord type.");
}

/// <summary>
/// A chord: a root, a <see cref="ChordType"/>, optional extensions and an inversion. Immutable: every method
/// that changes something returns a new chord. Notes are spelled from the root by interval, so the third of
/// F# major is A#, the seventh of Bb7 is Ab and the #9 of E7#9 is F##.
/// </summary>
public sealed record Chord
{
    private readonly Interval[] _extensions;

    /// <summary>The root (its octave places the chord).</summary>
    public Note Root { get; }

    /// <summary>The chord type.</summary>
    public ChordType Type { get; }

    /// <summary>The triad family, derived from <see cref="Type"/>: a minor 7th is minor, a 7sus4 suspended.</summary>
    public ChordQuality Quality => Diatonic.TriadOf(Type) switch
    {
        Triad.Minor => ChordQuality.Minor,
        Triad.Diminished => ChordQuality.Diminished,
        Triad.Augmented => ChordQuality.Augmented,
        Triad.Any => Type == ChordType.Power5 ? ChordQuality.Power : ChordQuality.Suspended,
        _ => ChordQuality.Major,
    };

    /// <summary>Which chord tone is in the bass.</summary>
    public ChordInversion Inversion { get; }

    /// <summary>The extensions added with <see cref="AddExtension"/>, in order.</summary>
    public IReadOnlyList<Interval> Extensions => _extensions;

    /// <summary>
    /// Creates a root-position triad of the given quality (suspended is sus4, power is root and fifth).
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="root"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="quality"/> is undefined.</exception>
    public Chord(Note root, ChordQuality quality) : this(root, quality switch
    {
        ChordQuality.Major => ChordType.Major,
        ChordQuality.Minor => ChordType.Minor,
        ChordQuality.Diminished => ChordType.Diminished,
        ChordQuality.Augmented => ChordType.Augmented,
        ChordQuality.Suspended => ChordType.Sus4,
        ChordQuality.Power => ChordType.Power5,
        _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, "Undefined chord quality."),
    })
    {
    }

    /// <summary>Creates a root-position chord of the given type.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="root"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/> is undefined.</exception>
    public Chord(Note root, ChordType type) : this(root, ChordTypes.Checked(type), [], ChordInversion.Root)
    {
    }

    private Chord(Note root, ChordType type, Interval[] extensions, ChordInversion inversion)
    {
        ArgumentNullException.ThrowIfNull(root);
        Root = root;
        Type = type;
        _extensions = extensions;
        Inversion = inversion;
    }

    /// <summary>
    /// The intervals above the root: the type's, with each extension replacing the chord tone of the same
    /// degree (a ♭9 replaces the 9th, a ♭5 the 5th) or added when the degree is absent.
    /// </summary>
    public IReadOnlyList<Interval> Intervals
    {
        get
        {
            var intervals = ChordTypes.Intervals(Type).ToList();
            foreach (var extension in _extensions)
            {
                int degree = (extension.Number - 1) % 7;
                int at = intervals.FindIndex(i => (i.Number - 1) % 7 == degree);
                intervals.RemoveAll(i => (i.Number - 1) % 7 == degree);
                intervals.Insert(at < 0 ? intervals.Count : at, extension);
            }
            return intervals;
        }
    }

    /// <summary>
    /// Returns a new chord with an extension: <c>AddExtension(9, Minor)</c> on C7 gives C7♭9; on a triad,
    /// <c>AddExtension(7, Minor)</c> gives the dominant seventh. See <see cref="Intervals"/> for the rule.
    /// </summary>
    /// <param name="intervalNumber">The degree, 2 or more (9 for a ninth).</param>
    /// <param name="quality">Its quality.</param>
    /// <exception cref="ArgumentOutOfRangeException">The number is below 2 or does not take that quality.</exception>
    public Chord AddExtension(int intervalNumber, IntervalQuality quality)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(intervalNumber, 2);
        return new Chord(Root, Type, [.. _extensions, new Interval(quality, intervalNumber)], Inversion);
    }

    /// <summary>The chord tones in root position, root first, ascending.</summary>
    public IEnumerable<Note> GetNotes()
    {
        var root = Root;
        return [root, .. Intervals.Select(root.Transpose)];
    }

    /// <summary>Transposes the root (and so every note) by a spelled interval.</summary>
    public Chord Transpose(Interval interval) => new(Root.Transpose(interval), Type, _extensions, Inversion);

    /// <summary>A new chord with the given inversion.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The inversion is undefined or needs more notes than the
    /// chord has (a third inversion needs a seventh).</exception>
    public Chord WithInversion(ChordInversion inversion)
    {
        if (!Enum.IsDefined(inversion))
            throw new ArgumentOutOfRangeException(nameof(inversion), inversion, "Undefined inversion.");
        if ((int)inversion > Intervals.Count)
            throw new ArgumentOutOfRangeException(nameof(inversion), inversion, $"{this} has {Intervals.Count + 1} notes, too few for a {inversion.ToString().ToLowerInvariant()} inversion.");
        return new Chord(Root, Type, _extensions, inversion);
    }

    /// <summary>The bass note: the root, third, fifth or seventh depending on <see cref="Inversion"/>.</summary>
    public Note GetBassNote() => GetNotes().ElementAt((int)Inversion);

    /// <summary>
    /// The notes in inversion order, bass first: the tones below the bass move up an octave
    /// (C/E is E4 G4 C5).
    /// </summary>
    public IEnumerable<Note> GetNotesInInversion()
    {
        var notes = GetNotes().ToList();
        int bass = (int)Inversion;
        var octave = new Interval(IntervalQuality.Perfect, 8);
        return [.. notes.Skip(bass), .. notes.Take(bass).Select(n => n.Transpose(octave))];
    }

    /// <summary>Root and bass only: <c>C/E</c> for C major in first inversion, <c>C</c> in root position.</summary>
    public string GetSlashChordNotation() => Inversion == ChordInversion.Root
        ? Root.GetName()
        : $"{Root.GetName()}/{GetBassNote().GetName()}";

    /// <summary>The same chord on the usual enharmonic root (C# → Db), or null when the root has none.</summary>
    public Chord? GetEnharmonicEquivalent()
    {
        var root = Root.GetEnharmonicEquivalent();
        return root is null ? null : new Chord(root, Type, _extensions, Inversion);
    }

    /// <summary>The chord as a lead-sheet symbol, with its extensions and slash bass.</summary>
    public ChordSymbol ToChordSymbol() =>
        new(Root, Type, Inversion == ChordInversion.Root ? null : GetBassNote(), _extensions);

    /// <summary>The symbol in the given style (ASCII by default): <c>Cmaj7</c>, <c>F#m/A</c>, <c>C7b9</c>.</summary>
    public string GetSymbol(ChordSymbolStyle? style = null) => ToChordSymbol().ToString(style ?? ChordSymbolStyle.Ascii);

    /// <summary>The ASCII symbol; see <see cref="GetSymbol"/>.</summary>
    public override string ToString() => GetSymbol();

    /// <inheritdoc />
    public bool Equals(Chord? other) =>
        other is not null && Root == other.Root && Type == other.Type && Inversion == other.Inversion
        && _extensions.SequenceEqual(other._extensions);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Root, Type, Inversion, _extensions.Length);
}
