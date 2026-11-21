using System.Diagnostics.CodeAnalysis;

namespace MusicTheory;

/// <summary>
/// The seven note letters.
/// </summary>
public enum NoteName
{
    /// <summary>C</summary>
    C,
    /// <summary>D</summary>
    D,
    /// <summary>E</summary>
    E,
    /// <summary>F</summary>
    F,
    /// <summary>G</summary>
    G,
    /// <summary>A</summary>
    A,
    /// <summary>B</summary>
    B
}

/// <summary>
/// Accidentals, valued in semitones.
/// </summary>
public enum Alteration
{
    /// <summary>Double flat (𝄫), two semitones down.</summary>
    DoubleFlat = -2,
    /// <summary>Flat (♭), one semitone down.</summary>
    Flat = -1,
    /// <summary>Natural (♮).</summary>
    Natural = 0,
    /// <summary>Sharp (♯), one semitone up.</summary>
    Sharp = 1,
    /// <summary>Double sharp (𝄪), two semitones up.</summary>
    DoubleSharp = 2
}

/// <summary>
/// Note-naming convention accepted by <see cref="Note.Parse(string, NoteNotation)"/>.
/// </summary>
public enum NoteNotation
{
    /// <summary>English letters with accidentals: <c>Bb4</c>, <c>F#</c>, <c>B♭</c>.</summary>
    English,
    /// <summary>German names: <c>H</c> is B, <c>B</c> is B♭, suffixes <c>-is</c> and <c>-es</c> (<c>Fis</c>, <c>Es</c>, <c>As</c>, <c>Heses</c>).</summary>
    German,
    /// <summary>Fixed-do solfège: <c>Do</c>, <c>Ré dièse 4</c>, <c>Sib3</c>; <c>Do</c> is always C. See <see cref="Solfege"/>.</summary>
    FixedDo
}

/// <summary>
/// A spelled pitch: a letter, an accidental and an octave in scientific pitch notation (middle C is C4).
/// Two notes are equal when all three match, so <c>C#4</c> and <c>Db4</c> are different notes that are
/// enharmonic (<see cref="IsEnharmonicWith"/>).
/// </summary>
public sealed record Note
{
    /// <summary>The lowest octave a note may have (C-1 is MIDI 0).</summary>
    public const int MinOctave = -1;

    /// <summary>The highest octave a note may have (G9 is MIDI 127).</summary>
    public const int MaxOctave = 9;

    /// <summary>Concert pitch: A4 = 440 Hz.</summary>
    public const double StandardA4 = 440.0;

    /// <summary>The letter.</summary>
    public NoteName Name { get; }

    /// <summary>The accidental.</summary>
    public Alteration Alteration { get; }

    /// <summary>The octave, from <see cref="MinOctave"/> to <see cref="MaxOctave"/>. Middle C is in octave 4.</summary>
    public int Octave { get; }

    /// <summary>
    /// Creates a note.
    /// </summary>
    /// <param name="name">The letter.</param>
    /// <param name="alteration">The accidental, natural by default.</param>
    /// <param name="octave">The octave, 4 by default.</param>
    /// <exception cref="ArgumentOutOfRangeException">An undefined letter or accidental, or an octave outside
    /// <see cref="MinOctave"/>..<see cref="MaxOctave"/>.</exception>
    public Note(NoteName name, Alteration alteration = Alteration.Natural, int octave = 4)
    {
        if (!Enum.IsDefined(name))
            throw new ArgumentOutOfRangeException(nameof(name), name, "Undefined note name.");
        if (!Enum.IsDefined(alteration))
            throw new ArgumentOutOfRangeException(nameof(alteration), alteration, "Undefined alteration.");
        if (octave is < MinOctave or > MaxOctave)
            throw new ArgumentOutOfRangeException(nameof(octave), octave, $"Octave must be between {MinOctave} and {MaxOctave}.");
        Name = name;
        Alteration = alteration;
        Octave = octave;
    }

    /// <summary>The pitch class, 0 (C) to 11 (B), octave ignored: <c>B#</c> is 0, <c>Cb</c> is 11.</summary>
    public int PitchClass => MusicTheoryUtilities.Mod12(MusicTheoryConstants.SemitonesFromC[(int)Name] + (int)Alteration);

    /// <summary>Semitones above C0 (negative below it, e.g. Cb0).</summary>
    internal int Semitones => MusicTheoryUtilities.GetTotalSemitones(this);

    /// <summary>Diatonic steps above C0: the note's line or space on the staff.</summary>
    internal int Step => Octave * 7 + (int)Name;

    /// <summary>The frequency in Hz in twelve-tone equal temperament at concert pitch (A4 = 440 Hz).</summary>
    public double Frequency => GetFrequency();

    /// <summary>
    /// The frequency in Hz in twelve-tone equal temperament, tuned to <paramref name="a4"/>: 442 for many
    /// orchestras, 415 for baroque pitch.
    /// </summary>
    /// <param name="a4">The frequency of A4 in Hz.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="a4"/> is not a positive, finite number.</exception>
    public double GetFrequency(double a4 = StandardA4)
    {
        if (!double.IsFinite(a4) || a4 <= 0)
            throw new ArgumentOutOfRangeException(nameof(a4), a4, "A4 must be a positive, finite frequency.");
        const int a4Semitones = 4 * 12 + 9;
        return a4 * Math.Pow(2.0, (Semitones - a4Semitones) / 12.0);
    }

    /// <summary>The MIDI note number: C4 (middle C) is 60.</summary>
    /// <exception cref="InvalidOperationException">The note is outside MIDI's 0..127.</exception>
    public int MidiNumber
    {
        get
        {
            int midi = Semitones + 12;
            if (midi is < 0 or > 127)
                throw new InvalidOperationException($"{this} is outside the MIDI range 0..127.");
            return midi;
        }
    }

    /// <summary>
    /// Transposes by a spelled interval, honouring its direction: C4 up an augmented 4th is F#4, E4 down a
    /// major 3rd is C4. The letter moves by the interval number, so the spelling is exact.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The result needs more than two accidentals (B## up a major 2nd).</exception>
    /// <exception cref="ArgumentOutOfRangeException">The result is outside the octave range.</exception>
    public Note Transpose(Interval interval)
    {
        ArgumentNullException.ThrowIfNull(interval);
        int sign = interval.Direction == Direction.Down ? -1 : 1;
        return FromStep(Step + sign * (interval.Number - 1), Semitones + sign * interval.Semitones,
            () => $"{this} transposed by {interval}");
    }

    /// <summary>
    /// Transposes by semitones, spelling black keys with sharps (C#, D#, F#, G#, A#). Use
    /// <see cref="TransposeBySemitones(int, KeySignature)"/> to spell in a key.
    /// </summary>
    public Note TransposeBySemitones(int semitones)
    {
        int target = Semitones + semitones;
        int pc = MusicTheoryUtilities.Mod12(target);
        int letter = Array.IndexOf(MusicTheoryConstants.SemitonesFromC, pc);
        var alteration = Alteration.Natural;
        if (letter < 0)
        {
            letter = Array.IndexOf(MusicTheoryConstants.SemitonesFromC, pc - 1);
            alteration = Alteration.Sharp;
        }
        return new Note((NoteName)letter, alteration, MusicTheoryUtilities.FloorDiv(target - pc, 12));
    }

    /// <summary>
    /// Transposes by semitones and spells the result in <paramref name="targetKey"/>
    /// (see <see cref="KeySignature.Spell"/>): A +1 in Eb major is Bb, in E major A#.
    /// </summary>
    public Note TransposeBySemitones(int semitones, KeySignature targetKey)
    {
        ArgumentNullException.ThrowIfNull(targetKey);
        int target = Semitones + semitones;
        var spelled = targetKey.Spell(target);
        int offset = MusicTheoryConstants.SemitonesFromC[(int)spelled.Name] + (int)spelled.Alteration;
        return new Note(spelled.Name, spelled.Alteration, MusicTheoryUtilities.FloorDiv(target - offset, 12));
    }

    /// <summary>
    /// The same pitch written with <paramref name="letter"/>, or null when that takes more than two
    /// accidentals: C4 respelled with B is B#3, with D is Dbb4, with E is null.
    /// </summary>
    public Note? Respell(NoteName letter)
    {
        if (!Enum.IsDefined(letter))
            throw new ArgumentOutOfRangeException(nameof(letter), letter, "Undefined note name.");
        // Stryker disable once Arithmetic: Mod12(x + 6) and Mod12(x - 6) are the same number.
        int alteration = MusicTheoryUtilities.Mod12(Semitones - MusicTheoryConstants.SemitonesFromC[(int)letter] + 6) - 6;
        if (Math.Abs(alteration) > 2)
            return null;
        int octave = MusicTheoryUtilities.FloorDiv(Semitones - MusicTheoryConstants.SemitonesFromC[(int)letter] - alteration, 12);
        return octave is < MinOctave or > MaxOctave ? null : new Note(letter, (Alteration)alteration, octave);
    }

    /// <summary>
    /// The nearest 12-TET note to <paramref name="hz"/> and the deviation in cents, in [-50, +50): an exact
    /// quarter tone above a note reads as the next note at -50. Spelled like <see cref="FromMidiNumber"/>.
    /// </summary>
    /// <param name="hz">The measured frequency in Hz.</param>
    /// <param name="a4">The frequency of A4 in Hz.</param>
    /// <param name="preferFlats">Spell black keys with flats instead of sharps.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="hz"/> or <paramref name="a4"/> is not a positive, finite number, or the nearest note is outside MIDI's 0..127.
    /// </exception>
    public static TunedPitch FromFrequency(double hz, double a4 = StandardA4, bool preferFlats = false)
    {
        if (!double.IsFinite(hz) || hz <= 0)
            throw new ArgumentOutOfRangeException(nameof(hz), hz, "Frequency must be positive and finite.");
        if (!double.IsFinite(a4) || a4 <= 0)
            throw new ArgumentOutOfRangeException(nameof(a4), a4, "A4 must be a positive, finite frequency.");
        double exact = 69 + 12 * Math.Log2(hz / a4);
        int midi = (int)Math.Floor(exact + 0.5);
        if (midi is < 0 or > 127)
            throw new ArgumentOutOfRangeException(nameof(hz), hz, "The nearest note is outside the MIDI range 0..127.");
        var note = FromMidiNumber(midi, preferFlats);
        // FromMidiNumber always spells MIDI 10 mod 12 as Bb; the tuning contract wants A# unless flats are preferred.
        if (!preferFlats && midi % 12 == 10)
            note = new Note(NoteName.A, Alteration.Sharp, note.Octave);
        return new TunedPitch(note, (exact - midi) * 100);
    }

    /// <summary>
    /// The note for a MIDI number. Black keys are C#, Eb, F#, Ab, Bb by default, or Db, Eb, Gb, Ab, Bb with
    /// <paramref name="preferFlats"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="midiNumber"/> is outside 0..127.</exception>
    public static Note FromMidiNumber(int midiNumber, bool preferFlats = false)
    {
        if (midiNumber is < 0 or > 127)
            throw new ArgumentOutOfRangeException(nameof(midiNumber), midiNumber, "MIDI note number must be between 0 and 127.");

        int octave = midiNumber / 12 - 1;
        var (name, alteration) = (midiNumber % 12) switch
        {
            0 => (NoteName.C, Alteration.Natural),
            1 => preferFlats ? (NoteName.D, Alteration.Flat) : (NoteName.C, Alteration.Sharp),
            2 => (NoteName.D, Alteration.Natural),
            3 => (NoteName.E, Alteration.Flat),
            4 => (NoteName.E, Alteration.Natural),
            5 => (NoteName.F, Alteration.Natural),
            6 => preferFlats ? (NoteName.G, Alteration.Flat) : (NoteName.F, Alteration.Sharp),
            7 => (NoteName.G, Alteration.Natural),
            8 => (NoteName.A, Alteration.Flat),
            9 => (NoteName.A, Alteration.Natural),
            10 => (NoteName.B, Alteration.Flat),
            _ => (NoteName.B, Alteration.Natural),
        };
        return new Note(name, alteration, octave);
    }

    /// <summary>The note as <c>C#4</c>, <c>Bb3</c>, <c>F##5</c>.</summary>
    public override string ToString() => GetName() + Octave;

    /// <summary>The note name without octave, e.g. <c>Bb</c> or (Unicode) <c>B♭</c>.</summary>
    public string GetName(AccidentalStyle style = AccidentalStyle.Ascii) => ChordSymbol.Spell(this, style);

    /// <summary>
    /// True when <paramref name="other"/> sounds the same pitch: C#4 and Db4, B#3 and C4. Notes an octave
    /// apart are not enharmonic.
    /// </summary>
    public bool IsEnharmonicWith(Note? other) => other is not null && Semitones == other.Semitones;

    /// <summary>
    /// The usual alternative spelling of this pitch: C# ↔ Db, E ↔ Fb, B# → C, C## → D, Cbb → Bb.
    /// Null for D, G and A, which have no common alternative.
    /// </summary>
    public Note? GetEnharmonicEquivalent()
    {
        if (Alteration == Alteration.Natural && Name is NoteName.D or NoteName.G or NoteName.A)
            return null;
        // Sharps, E and B move to the next letter (C# -> Db, E -> Fb); flats, F and C to the previous one.
        bool next = Alteration > 0 || (Alteration == Alteration.Natural && Name is NoteName.E or NoteName.B);
        return Respell(next ? (NoteName)(((int)Name + 1) % 7) : (NoteName)(((int)Name + 6) % 7));
    }

    /// <summary>Every other spelling of this pitch with at most two accidentals: C4 gives B#3 and Dbb4.</summary>
    public IEnumerable<Note> GetAllEnharmonicEquivalents() =>
        Enum.GetValues<NoteName>().Where(l => l != Name).Select(Respell).OfType<Note>().ToList();

    /// <summary>
    /// The plainest spelling: double accidentals and E#, Fb, B#, Cb become their natural or single-accidental
    /// equivalent; every other note is returned as is.
    /// </summary>
    public Note SimplifyEnharmonic()
    {
        bool awkward = Math.Abs((int)Alteration) == 2
            || (Alteration == Alteration.Sharp && Name is NoteName.E or NoteName.B)
            || (Alteration == Alteration.Flat && Name is NoteName.F or NoteName.C);
        return awkward ? GetEnharmonicEquivalent() ?? this : this;
    }

    /// <summary>
    /// Parses a note in English notation: a letter in any case, up to two accidentals in ASCII (<c>#</c>,
    /// <c>b</c>) or Unicode (♯ ♭ 𝄪 𝄫), and an optional octave (default 4), e.g. <c>C#4</c>, <c>Bb3</c>, <c>F##</c>, <c>B♭</c>.
    /// </summary>
    /// <exception cref="FormatException">The text is not a note.</exception>
    public static Note Parse(string noteString) => Parse(noteString, NoteNotation.English);

    /// <summary>Parses a note in the given notation; see <see cref="NoteNotation"/>.</summary>
    /// <exception cref="FormatException">The text is not a note in that notation.</exception>
    public static Note Parse(string noteString, NoteNotation notation) =>
        TryParse(noteString, notation, out var note)
            ? note
            : throw new FormatException($"Not a note in {notation} notation: '{noteString}'.");

    /// <summary>Tries to parse a note in English notation; see <see cref="Parse(string)"/>.</summary>
    public static bool TryParse(string? text, [NotNullWhen(true)] out Note? note) => TryParse(text, NoteNotation.English, out note);

    /// <summary>Tries to parse a note in the given notation; never throws.</summary>
    public static bool TryParse(string? text, NoteNotation notation, [NotNullWhen(true)] out Note? note)
    {
        note = null;
        var s = text.AsSpan().Trim();
        Note? pitch;
        int consumed;
        bool read = notation switch
        {
            NoteNotation.German => TryParseGermanPitchClass(s, out pitch, out consumed),
            NoteNotation.FixedDo => Solfege.TryParseFixedDoPitchClass(s, out pitch, out consumed),
            _ => TryParsePitchClass(s, out pitch, out consumed),
        };
        if (!read)
            return false;
        if (consumed == s.Length)
        {
            note = pitch;
            return true;
        }
        if (s[consumed] == '+' || !int.TryParse(s[consumed..], System.Globalization.NumberStyles.AllowLeadingSign,
                System.Globalization.CultureInfo.InvariantCulture, out var octave) || octave is < MinOctave or > MaxOctave)
            return false;
        note = new Note(pitch.Name, pitch.Alteration, octave);
        return true;
    }

    /// <summary>
    /// Reads an English note letter (any case) and up to two accidentals at the start of <paramref name="s"/>.
    /// </summary>
    /// <param name="s">The text to read from; anything after the note is ignored.</param>
    /// <param name="note">The note read, in octave 4.</param>
    /// <param name="consumed">How many characters the note used.</param>
    /// <returns>True when <paramref name="s"/> starts with a note letter.</returns>
    public static bool TryParsePitchClass(ReadOnlySpan<char> s, [NotNullWhen(true)] out Note? note, out int consumed)
    {
        note = null;
        consumed = 0;
        if (s.IsEmpty)
            return false;

        int letter = "CDEFGAB".IndexOf(char.ToUpperInvariant(s[0]));
        if (letter < 0)
            return false;

        int i = 1, alteration = 0;
        while (i < s.Length)
        {
            int step, width = 1;
            if (s[i] is '#' or '♯') step = 1;
            else if (s[i] is 'b' or '♭') step = -1;
            else if (s[i..].StartsWith("𝄪")) { step = 2; width = 2; }
            else if (s[i..].StartsWith("𝄫")) { step = -2; width = 2; }
            else break;

            if ((alteration != 0 && Math.Sign(step) != Math.Sign(alteration)) || Math.Abs(alteration + step) > 2)
                break;
            alteration += step;
            i += width;
        }

        note = new Note((NoteName)letter, (Alteration)alteration);
        consumed = i;
        return true;
    }

    // German names, longest first so "eses" wins over "es" and "heses" over "h". H is B, B is B flat.
    private static (string Name, NoteName Letter, Alteration Alteration)[] German() => new (string, NoteName, Alteration)[]
    {
        ("ceses", NoteName.C, Alteration.DoubleFlat), ("ces", NoteName.C, Alteration.Flat), ("c", NoteName.C, Alteration.Natural),
        ("cis", NoteName.C, Alteration.Sharp), ("cisis", NoteName.C, Alteration.DoubleSharp),
        ("deses", NoteName.D, Alteration.DoubleFlat), ("des", NoteName.D, Alteration.Flat), ("d", NoteName.D, Alteration.Natural),
        ("dis", NoteName.D, Alteration.Sharp), ("disis", NoteName.D, Alteration.DoubleSharp),
        ("eses", NoteName.E, Alteration.DoubleFlat), ("es", NoteName.E, Alteration.Flat), ("e", NoteName.E, Alteration.Natural),
        ("eis", NoteName.E, Alteration.Sharp), ("eisis", NoteName.E, Alteration.DoubleSharp),
        ("feses", NoteName.F, Alteration.DoubleFlat), ("fes", NoteName.F, Alteration.Flat), ("f", NoteName.F, Alteration.Natural),
        ("fis", NoteName.F, Alteration.Sharp), ("fisis", NoteName.F, Alteration.DoubleSharp),
        ("geses", NoteName.G, Alteration.DoubleFlat), ("ges", NoteName.G, Alteration.Flat), ("g", NoteName.G, Alteration.Natural),
        ("gis", NoteName.G, Alteration.Sharp), ("gisis", NoteName.G, Alteration.DoubleSharp),
        ("asas", NoteName.A, Alteration.DoubleFlat), ("as", NoteName.A, Alteration.Flat), ("a", NoteName.A, Alteration.Natural),
        ("ais", NoteName.A, Alteration.Sharp), ("aisis", NoteName.A, Alteration.DoubleSharp),
        ("heses", NoteName.B, Alteration.DoubleFlat), ("hes", NoteName.B, Alteration.Flat), ("b", NoteName.B, Alteration.Flat),
        ("h", NoteName.B, Alteration.Natural), ("his", NoteName.B, Alteration.Sharp), ("hisis", NoteName.B, Alteration.DoubleSharp),
    }.OrderByDescending(n => n.Item1.Length).ToArray();

    private static bool TryParseGermanPitchClass(ReadOnlySpan<char> s, [NotNullWhen(true)] out Note? note, out int consumed)
    {
        foreach (var (name, letter, alteration) in German())
        {
            if (s.StartsWith(name, StringComparison.OrdinalIgnoreCase))
            {
                note = new Note(letter, alteration);
                consumed = name.Length;
                return true;
            }
        }
        note = null;
        consumed = 0;
        return false;
    }

    /// <summary>
    /// The note on diatonic <paramref name="step"/> (see <see cref="Step"/>) sounding <paramref name="semitones"/>
    /// above C0, or an exception naming <paramref name="what"/> when that needs more than two accidentals.
    /// </summary>
    internal static Note FromStep(int step, int semitones, Func<string> what)
    {
        int letter = MusicTheoryUtilities.Mod7(step);
        int octave = MusicTheoryUtilities.FloorDiv(step, 7);
        int alteration = semitones - (octave * 12 + MusicTheoryConstants.SemitonesFromC[letter]);
        if (Math.Abs(alteration) > 2)
            throw new InvalidOperationException($"{what()} needs {Math.Abs(alteration)} {(alteration > 0 ? "sharps" : "flats")} on {(NoteName)letter}; at most two are supported.");
        return new Note((NoteName)letter, (Alteration)alteration, octave);
    }
}
