using System.Diagnostics.CodeAnalysis;

namespace MusicTheory;

/// <summary>
/// Represents the mode of a key (major or minor).
/// </summary>
public enum KeyMode
{
    /// <summary>Major mode</summary>
    Major,
    /// <summary>Minor mode</summary>
    Minor
}

/// <summary>
/// A key: a tonic and a mode, with its signature computed on the line of fifths. Theoretical keys are
/// supported up to two accidentals per note (G# major has F##); equality ignores the tonic's octave.
/// </summary>
public sealed record KeySignature
{
    // Order in which sharps (and, reversed, flats) enter a signature.
    private static readonly NoteName[] SharpOrder = [NoteName.F, NoteName.C, NoteName.G, NoteName.D, NoteName.A, NoteName.E, NoteName.B];
    private static readonly NoteName[] FlatOrder = [NoteName.B, NoteName.E, NoteName.A, NoteName.D, NoteName.G, NoteName.C, NoteName.F];

    /// <summary>The tonic.</summary>
    public Note Tonic { get; }

    /// <summary>Major or minor.</summary>
    public KeyMode Mode { get; }

    /// <summary>
    /// Accidentals in the signature: sharps positive, flats negative. A double sharp counts twice, so G#
    /// major is 8.
    /// </summary>
    public int AccidentalCount { get; }

    /// <summary>The letters the signature alters, in the order they enter it (F C G… for sharps).</summary>
    public IReadOnlyList<NoteName> AlteredNotes { get; }

    /// <summary>Creates a key.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="tonic"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The mode is undefined, or the key needs triple
    /// accidentals (more than 14 sharps or flats, e.g. Cbb minor).</exception>
    public KeySignature(Note tonic, KeyMode mode)
    {
        ArgumentNullException.ThrowIfNull(tonic);
        if (!Enum.IsDefined(mode))
            throw new ArgumentOutOfRangeException(nameof(mode), mode, "Undefined key mode.");
        Tonic = tonic;
        Mode = mode;
        AccidentalCount = PositionOf(tonic, mode);
        if (Math.Abs(AccidentalCount) > 14)
            // Stryker disable once Equality: the count is beyond ±14 here, never 0, so > and >= agree.
            throw new ArgumentOutOfRangeException(nameof(tonic), tonic, $"{ToString()} needs {Math.Abs(AccidentalCount)} {(AccidentalCount > 0 ? "sharps" : "flats")}; at most 14 (two per note) are supported.");
        // Stryker disable once Equality: with no accidentals nothing is taken from either order, so >= and > agree.
        var order = AccidentalCount >= 0 ? SharpOrder : FlatOrder;
        AlteredNotes = order.Take(Math.Min(Math.Abs(AccidentalCount), 7)).ToList();
    }

    /// <summary>The accidental the signature puts on <paramref name="noteName"/> (F## in G# major).</summary>
    public Alteration GetAlteration(NoteName noteName)
    {
        int count = Math.Abs(AccidentalCount);
        var order = AccidentalCount >= 0 ? SharpOrder : FlatOrder;
        int times = (count - Array.IndexOf(order, noteName) + 6) / 7;
        return (Alteration)(AccidentalCount >= 0 ? times : -times);
    }

    /// <summary>The key a fifth above, same mode (C → G, C# → Ab).</summary>
    public KeySignature NextInCircle() => GetKeyFromPosition(Wrap(AccidentalCount + 1), Mode);

    /// <summary>The key a fifth below, same mode (C → F, Cb → E).</summary>
    public KeySignature PreviousInCircle() => GetKeyFromPosition(Wrap(AccidentalCount - 1), Mode);

    /// <summary>The relative key: the major's 6th degree, the minor's 3rd (C ↔ Am, C# ↔ A#m).</summary>
    public KeySignature GetRelative() => Mode == KeyMode.Major
        ? new KeySignature(Tonic.Transpose(new Interval(IntervalQuality.Minor, 3, Direction.Down)), KeyMode.Minor)
        : new KeySignature(Tonic.Transpose(new Interval(IntervalQuality.Minor, 3)), KeyMode.Major);

    /// <summary>The parallel key: same tonic, other mode.</summary>
    public KeySignature GetParallel() => new(Tonic, Mode == KeyMode.Major ? KeyMode.Minor : KeyMode.Major);

    /// <summary>
    /// The enharmonic keys with at most seven accidentals: B ↔ Cb, F# ↔ Gb, C# ↔ Db, and in minor
    /// G#m ↔ Abm, D#m ↔ Ebm, A#m ↔ Bbm.
    /// </summary>
    public IEnumerable<KeySignature> GetEnharmonicEquivalents() =>
        new[] { AccidentalCount - 12, AccidentalCount + 12 }
            .Where(p => Math.Abs(p) <= 7)
            .Select(p => GetKeyFromPosition(p, Mode))
            .ToList();

    /// <summary>
    /// Gets the dominant key (fifth above).
    /// </summary>
    /// <returns>The dominant key signature.</returns>
    public KeySignature GetDominant()
    {
        return NextInCircle();
    }

    /// <summary>
    /// Gets the subdominant key (fifth below).
    /// </summary>
    /// <returns>The subdominant key signature.</returns>
    public KeySignature GetSubdominant()
    {
        return PreviousInCircle();
    }

    /// <summary>
    /// Parses a key name: a tonic (any case, ASCII or Unicode accidentals) optionally followed by
    /// <c>m</c>/<c>min</c>/<c>minor</c> or <c>maj</c>/<c>major</c>/<c>M</c>, e.g. <c>Eb</c>, <c>E♭ minor</c>, <c>F#m</c>, <c>C major</c>.
    /// </summary>
    /// <exception cref="FormatException">The text is not a key.</exception>
    public static KeySignature Parse(string text) =>
        TryParse(text, out var key) ? key : throw new FormatException($"Not a key: '{text}'.");

    /// <summary>Tries to parse a key name; see <see cref="Parse"/>.</summary>
    public static bool TryParse(string? text, [NotNullWhen(true)] out KeySignature? key)
    {
        key = null;
        var s = text.AsSpan().Trim();
        if (!Note.TryParsePitchClass(s, out var tonic, out var consumed))
            return false;

        var rest = s[consumed..].Trim().ToString();
        KeyMode? mode = rest == "M" ? KeyMode.Major : rest.ToLowerInvariant() switch
        {
            "" or "maj" or "major" => KeyMode.Major,
            "m" or "min" or "minor" => KeyMode.Minor,
            _ => null,
        };
        if (mode is null || Math.Abs(PositionOf(tonic, mode.Value)) > 7)
            return false;

        key = new KeySignature(tonic, mode.Value);
        return true;
    }

    /// <summary>
    /// The key <paramref name="semitones"/> above (or below) this one, same mode, spelled with the fewest
    /// accidentals: G +1 is Ab major, not G# major. When both spellings have six accidentals
    /// (F#/Gb major, D#/Eb minor) the side of this key wins, and flats win from a key with none.
    /// </summary>
    public KeySignature TransposeBySemitones(int semitones)
    {
        int target = MusicTheoryUtilities.GetPitchClass(Tonic) + semitones;
        return Spellings(target)
            .Select(n => new KeySignature(n, Mode))
            .OrderBy(k => Math.Abs(k.AccidentalCount))
            .ThenBy(k => AccidentalCount > 0 ? -k.AccidentalCount : k.AccidentalCount)
            .First();
    }

    /// <summary>
    /// Spells a pitch class (0 = C) in this key: scale notes (and, in minor, the raised 6th and 7th) use
    /// the key's spelling; other black keys are sharps in sharp keys and flats in flat keys; in C major /
    /// A minor they are Db Eb F# Ab Bb. The returned note is in octave 4.
    /// </summary>
    public Note Spell(int pitchClass)
    {
        int pc = Mod12(pitchClass);
        int tonicPc = MusicTheoryUtilities.GetPitchClass(Tonic);
        int[] steps = Mode == KeyMode.Major ? [0, 2, 4, 5, 7, 9, 11] : [0, 2, 3, 5, 7, 8, 10, 9, 11];
        for (int i = 0; i < steps.Length; i++)
        {
            int degree = i < 7 ? i : i - 2; // minor: 7, 8 are the raised 6th and 7th
            var letter = (NoteName)(((int)Tonic.Name + degree) % 7);
            if (Mod12(tonicPc + steps[i]) != pc)
                continue;
            int alteration = Mod12(pc - MusicTheoryConstants.SemitonesFromC[(int)letter] + 6) - 6;
            if (Math.Abs(alteration) <= 2)
                return new Note(letter, (Alteration)alteration);
        }

        int natural = Array.IndexOf(MusicTheoryConstants.SemitonesFromC, pc);
        if (natural >= 0)
            return new Note((NoteName)natural);

        bool sharp = AccidentalCount > 0 || (AccidentalCount == 0 && pc == 6);
        return sharp
            ? new Note((NoteName)Array.IndexOf(MusicTheoryConstants.SemitonesFromC, pc - 1), Alteration.Sharp)
            : new Note((NoteName)Array.IndexOf(MusicTheoryConstants.SemitonesFromC, pc + 1), Alteration.Flat);
    }

    /// <summary>The key name, e.g. <c>Ab</c>, <c>F#m</c> or (Unicode) <c>A♭</c>.</summary>
    public string ToString(AccidentalStyle style) =>
        ChordSymbol.Spell(Tonic, style) + (Mode == KeyMode.Minor ? "m" : "");

    /// <summary>The key name in ASCII, e.g. <c>Ab</c> or <c>F#m</c>.</summary>
    public override string ToString() => ToString(AccidentalStyle.Ascii);

    private static IEnumerable<Note> Spellings(int pitchClass)
    {
        int pc = Mod12(pitchClass);
        for (int letter = 0; letter < 7; letter++)
            for (int alt = -1; alt <= 1; alt++)
                if (Mod12(MusicTheoryConstants.SemitonesFromC[letter] + alt) == pc)
                    yield return new Note((NoteName)letter, (Alteration)alt);
    }

    private static int Mod12(int n) => ((n % 12) + 12) % 12;

    /// <summary>
    /// Position on the line of fifths (sharps positive): F=-1 C=0 G=1 D=2 A=3 E=4 B=5, ±7 per sharp/flat,
    /// minor keys three fifths below their relative major.
    /// </summary>
    private static int PositionOf(Note tonic, KeyMode mode)
    {
        int[] lineOfFifths = [0, 2, 4, -1, 1, 3, 5]; // C D E F G A B
        return lineOfFifths[(int)tonic.Name] + 7 * (int)tonic.Alteration - (mode == KeyMode.Minor ? 3 : 0);
    }

    // Past seven accidentals, the circle continues on the enharmonic side (8 sharps = 4 flats).
    private static int Wrap(int position) => position > 7 ? position - 12 : position < -7 ? position + 12 : position;

    /// <inheritdoc />
    public bool Equals(KeySignature? other) =>
        other is not null && Mode == other.Mode && Tonic.Name == other.Tonic.Name && Tonic.Alteration == other.Tonic.Alteration;

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Mode, Tonic.Name, Tonic.Alteration);

    /// <summary>The key whose signature has <paramref name="position"/> accidentals (sharps positive).</summary>
    private static KeySignature GetKeyFromPosition(int position, KeyMode mode)
    {
        // The major key with this signature sits on the line of fifths (F C G D A E B, one more accidental
        // every seven steps); the minor key with the same signature is its relative, a minor 3rd below.
        int step = position + 1;
        var major = new Note(SharpOrder[MusicTheoryUtilities.Mod7(step)], (Alteration)MusicTheoryUtilities.FloorDiv(step, 7));
        return new KeySignature(mode == KeyMode.Minor ? major.Transpose(new Interval(IntervalQuality.Minor, 3, Direction.Down)) : major, mode);
    }
}
