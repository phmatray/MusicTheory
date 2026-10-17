using System.Diagnostics.CodeAnalysis;

namespace MusicTheory;

/// <summary>
/// Represents the seven natural note names in music.
/// </summary>
public enum NoteName
{
    /// <summary>C natural</summary>
    C,
    /// <summary>D natural</summary>
    D,
    /// <summary>E natural</summary>
    E,
    /// <summary>F natural</summary>
    F,
    /// <summary>G natural</summary>
    G,
    /// <summary>A natural</summary>
    A,
    /// <summary>B natural</summary>
    B
}

/// <summary>
/// Represents alterations that can be applied to a note.
/// </summary>
public enum Alteration
{
    /// <summary>Double flat (♭♭) - lowers the note by two semitones</summary>
    DoubleFlat = -2,
    /// <summary>Flat (♭) - lowers the note by one semitone</summary>
    Flat = -1,
    /// <summary>Natural (♮) - no alteration</summary>
    Natural = 0,
    /// <summary>Sharp (♯) - raises the note by one semitone</summary>
    Sharp = 1,
    /// <summary>Double sharp (♯♯) - raises the note by two semitones</summary>
    DoubleSharp = 2
}

/// <summary>
/// Represents a musical note with a specific name, alteration, and octave.
/// </summary>
public class Note
{
    /// <summary>
    /// Gets the name of the note.
    /// </summary>
    public NoteName Name { get; }

    /// <summary>
    /// Gets the alteration applied to the note.
    /// </summary>
    public Alteration Alteration { get; }

    /// <summary>
    /// Gets the octave of the note. Middle C is in octave 4.
    /// </summary>
    public int Octave { get; }

    /// <summary>
    /// Gets the frequency of the note in Hz, calculated using equal temperament tuning with A4 = 440 Hz.
    /// </summary>
    public double Frequency => CalculateFrequency();

    /// <summary>
    /// Gets the MIDI note number (0-127).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the note is outside the valid MIDI range.</exception>
    public int MidiNumber
    {
        get
        {
            // MIDI note 60 is Middle C (C4)
            // Calculate semitones from C-1 (MIDI 0)
            int midiNumber = (Octave + 1) * MusicTheoryConstants.SemitonesPerOctave + 
                           MusicTheoryConstants.SemitonesFromC[(int)Name] + 
                           (int)Alteration;
            
            if (midiNumber < 0 || midiNumber > 127)
            {
                throw new InvalidOperationException($"Note {this} is outside the valid MIDI range (0-127).");
            }
            
            return midiNumber;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Note"/> class with natural alteration and octave 4.
    /// </summary>
    /// <param name="name">The name of the note.</param>
    public Note(NoteName name) : this(name, Alteration.Natural, 4)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Note"/> class with a specific alteration and octave 4.
    /// </summary>
    /// <param name="name">The name of the note.</param>
    /// <param name="alteration">The alteration to apply to the note.</param>
    public Note(NoteName name, Alteration alteration) : this(name, alteration, 4)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Note"/> class with a specific alteration and octave.
    /// </summary>
    /// <param name="name">The name of the note.</param>
    /// <param name="alteration">The alteration to apply to the note.</param>
    /// <param name="octave">The octave of the note.</param>
    public Note(NoteName name, Alteration alteration, int octave)
    {
        Name = name;
        Alteration = alteration;
        Octave = octave;
    }

    /// <summary>
    /// Calculates the frequency of the note using equal temperament tuning with A4 = 440 Hz.
    /// </summary>
    /// <returns>The frequency in Hz.</returns>
    private double CalculateFrequency()
    {
        // A4 is the reference note at 440 Hz
        // Calculate semitones from A4
        int semitonesFromA4 = GetSemitonesFromA4();
        
        // Apply equal temperament formula: f = 440 * 2^(n/12)
        // where n is the number of semitones from A4
        return MusicTheoryConstants.A4Frequency * Math.Pow(2.0, semitonesFromA4 / 12.0);
    }

    /// <summary>
    /// Gets the number of semitones from A4 to this note.
    /// </summary>
    /// <returns>The number of semitones (positive for higher notes, negative for lower).</returns>
    private int GetSemitonesFromA4()
    {
        // Calculate semitones from C0 to this note
        int semitonesFromC0 = MusicTheoryUtilities.GetTotalSemitones(this);
        
        // A4 is 57 semitones from C0 (4 * 12 + 9 = 57)
        const int a4SemitonesFromC0 = 57;
        
        return semitonesFromC0 - a4SemitonesFromC0;
    }

    /// <summary>
    /// Transposes the note by the specified interval.
    /// </summary>
    /// <param name="interval">The interval to transpose by.</param>
    /// <param name="direction">The direction to transpose (default is Up).</param>
    /// <returns>A new note transposed by the interval.</returns>
    public Note Transpose(Interval interval, Direction direction = Direction.Up)
    {
        int semitones = interval.Semitones;
        if (direction == Direction.Down)
            semitones = -semitones;

        // Calculate the new note name based on the interval number
        int noteOffset = interval.Number - 1;
        if (direction == Direction.Down)
            noteOffset = -noteOffset;

        int newNoteIndex = ((int)Name + noteOffset) % 7;
        if (newNoteIndex < 0)
            newNoteIndex += 7;

        var targetNoteName = (NoteName)newNoteIndex;

        // Calculate the actual semitones from C0
        int currentSemitones = GetTotalSemitones();
        int targetSemitones = currentSemitones + semitones;

        // Calculate octave
        int baseOctave = Octave;
        if (direction == Direction.Up && targetNoteName < Name)
            baseOctave++;
        else if (direction == Direction.Down && targetNoteName > Name)
            baseOctave--;

        // Adjust octave based on total semitone change
        int octaveAdjustment = 0;
        if (direction == Direction.Up)
        {
            octaveAdjustment = noteOffset / 7;
        }
        else
        {
            octaveAdjustment = -((-noteOffset + 6) / 7);
        }

        int targetOctave = baseOctave + octaveAdjustment;

        // Calculate the required alteration
        int expectedSemitones = targetOctave * MusicTheoryConstants.SemitonesPerOctave + 
                              MusicTheoryConstants.SemitonesFromC[(int)targetNoteName];
        int alterationValue = targetSemitones - expectedSemitones;

        // Clamp alteration to valid range
        if (alterationValue < -2)
        {
            alterationValue += MusicTheoryConstants.SemitonesPerOctave;
            targetOctave--;
        }
        else if (alterationValue > 2)
        {
            alterationValue -= MusicTheoryConstants.SemitonesPerOctave;
            targetOctave++;
        }

        return new Note(targetNoteName, (Alteration)alterationValue, targetOctave);
    }

    /// <summary>
    /// Transposes the note by a specific number of semitones.
    /// </summary>
    /// <param name="semitones">The number of semitones to transpose (positive for up, negative for down).</param>
    /// <returns>A new note transposed by the specified semitones.</returns>
    public Note TransposeBySemitones(int semitones)
    {
        // Calculate target semitones from C0
        int currentSemitones = GetTotalSemitones();
        int targetSemitones = currentSemitones + semitones;

        // Calculate octave and note
        int targetOctave = targetSemitones / MusicTheoryConstants.SemitonesPerOctave;
        int semitonesInOctave = targetSemitones % MusicTheoryConstants.SemitonesPerOctave;

        // Handle negative modulo
        if (semitonesInOctave < 0)
        {
            semitonesInOctave += MusicTheoryConstants.SemitonesPerOctave;
            targetOctave--;
        }

        // Find the closest natural note
        int bestNote = 0;
        int bestDifference = int.MaxValue;

        for (int i = 0; i < 7; i++)
        {
            int difference = Math.Abs(semitonesInOctave - MusicTheoryConstants.SemitonesFromC[i]);
            if (difference < bestDifference)
            {
                bestDifference = difference;
                bestNote = i;
            }
        }

        // Calculate alteration
        int alterationValue = semitonesInOctave - MusicTheoryConstants.SemitonesFromC[bestNote];

        return new Note((NoteName)bestNote, (Alteration)alterationValue, targetOctave);
    }

    /// <summary>
    /// Gets the total semitones from C0 for this note.
    /// </summary>
    private int GetTotalSemitones()
    {
        return MusicTheoryUtilities.GetTotalSemitones(this);
    }

    /// <summary>
    /// Gets the semitones from C in the current octave.
    /// </summary>
    private int GetSemitonesFromC()
    {
        return GetTotalSemitones();
    }

    /// <summary>
    /// Creates a note from a MIDI note number.
    /// </summary>
    /// <param name="midiNumber">The MIDI note number (0-127).</param>
    /// <param name="preferFlats">Whether to prefer flats over sharps for accidentals.</param>
    /// <returns>A new Note instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the MIDI number is outside the valid range.</exception>
    public static Note FromMidiNumber(int midiNumber, bool preferFlats = false)
    {
        if (midiNumber < 0 || midiNumber > 127)
        {
            throw new ArgumentOutOfRangeException(nameof(midiNumber), 
                "MIDI note number must be between 0 and 127.");
        }

        // Calculate octave (C-1 = 0, C0 = 12, C1 = 24, etc.)
        int octave = (midiNumber / MusicTheoryConstants.SemitonesPerOctave) - 1;
        int noteInOctave = midiNumber % MusicTheoryConstants.SemitonesPerOctave;

        // Map semitones to note names and alterations
        if (preferFlats)
        {
            // Prefer flats for black keys
            return noteInOctave switch
            {
                0 => new Note(NoteName.C, Alteration.Natural, octave),
                1 => new Note(NoteName.D, Alteration.Flat, octave),
                2 => new Note(NoteName.D, Alteration.Natural, octave),
                3 => new Note(NoteName.E, Alteration.Flat, octave),
                4 => new Note(NoteName.E, Alteration.Natural, octave),
                5 => new Note(NoteName.F, Alteration.Natural, octave),
                6 => new Note(NoteName.G, Alteration.Flat, octave),
                7 => new Note(NoteName.G, Alteration.Natural, octave),
                8 => new Note(NoteName.A, Alteration.Flat, octave),
                9 => new Note(NoteName.A, Alteration.Natural, octave),
                10 => new Note(NoteName.B, Alteration.Flat, octave),
                11 => new Note(NoteName.B, Alteration.Natural, octave),
                _ => throw new InvalidOperationException($"Invalid note in octave: {noteInOctave}")
            };
        }
        else
        {
            // Prefer sharps for black keys (default)
            return noteInOctave switch
            {
                0 => new Note(NoteName.C, Alteration.Natural, octave),
                1 => new Note(NoteName.C, Alteration.Sharp, octave),
                2 => new Note(NoteName.D, Alteration.Natural, octave),
                3 => new Note(NoteName.E, Alteration.Flat, octave),  // Eb is more common than D#
                4 => new Note(NoteName.E, Alteration.Natural, octave),
                5 => new Note(NoteName.F, Alteration.Natural, octave),
                6 => new Note(NoteName.F, Alteration.Sharp, octave),
                7 => new Note(NoteName.G, Alteration.Natural, octave),
                8 => new Note(NoteName.A, Alteration.Flat, octave),  // Ab is more common than G#
                9 => new Note(NoteName.A, Alteration.Natural, octave),
                10 => new Note(NoteName.B, Alteration.Flat, octave), // Bb is more common than A#
                11 => new Note(NoteName.B, Alteration.Natural, octave),
                _ => throw new InvalidOperationException($"Invalid note in octave: {noteInOctave}")
            };
        }
    }

    /// <summary>
    /// Returns a string representation of the note.
    /// </summary>
    /// <returns>A string in the format "NoteName[Alteration]Octave".</returns>
    public override string ToString()
    {
        var alterationSymbol = Alteration switch
        {
            Alteration.DoubleFlat => "bb",
            Alteration.Flat => "b",
            Alteration.Natural => "",
            Alteration.Sharp => "#",
            Alteration.DoubleSharp => "##",
            _ => ""
        };
        
        return $"{Name}{alterationSymbol}{Octave}";
    }

    /// <summary>
    /// Determines if this note is enharmonically equivalent to another note.
    /// Two notes are enharmonic if they represent the same pitch with different names.
    /// </summary>
    /// <param name="other">The other note to compare.</param>
    /// <returns>True if the notes are enharmonic; otherwise, false.</returns>
    public bool IsEnharmonicWith(Note? other)
    {
        if (other == null) return false;
        
        // Two notes are enharmonic if they have the same semitone value
        // but different note names or alterations
        var thisSemitones = GetSemitonesFromC();
        var otherSemitones = other.GetSemitonesFromC();
        
        // Normalize to same octave for comparison
        var thisNormalized = thisSemitones % MusicTheoryConstants.SemitonesPerOctave;
        var otherNormalized = otherSemitones % MusicTheoryConstants.SemitonesPerOctave;
        
        return thisNormalized == otherNormalized;
    }

    /// <summary>
    /// Gets the most common enharmonic equivalent of this note.
    /// </summary>
    /// <returns>The enharmonic equivalent note, or null if no simple equivalent exists.</returns>
    public Note? GetEnharmonicEquivalent()
    {
        // Common enharmonic equivalents
        return (Name, Alteration) switch
        {
            // Sharp to flat conversions
            (NoteName.C, Alteration.Sharp) => new Note(NoteName.D, Alteration.Flat, Octave),
            (NoteName.D, Alteration.Sharp) => new Note(NoteName.E, Alteration.Flat, Octave),
            (NoteName.F, Alteration.Sharp) => new Note(NoteName.G, Alteration.Flat, Octave),
            (NoteName.G, Alteration.Sharp) => new Note(NoteName.A, Alteration.Flat, Octave),
            (NoteName.A, Alteration.Sharp) => new Note(NoteName.B, Alteration.Flat, Octave),
            
            // Flat to sharp conversions
            (NoteName.D, Alteration.Flat) => new Note(NoteName.C, Alteration.Sharp, Octave),
            (NoteName.E, Alteration.Flat) => new Note(NoteName.D, Alteration.Sharp, Octave),
            (NoteName.G, Alteration.Flat) => new Note(NoteName.F, Alteration.Sharp, Octave),
            (NoteName.A, Alteration.Flat) => new Note(NoteName.G, Alteration.Sharp, Octave),
            (NoteName.B, Alteration.Flat) => new Note(NoteName.A, Alteration.Sharp, Octave),
            
            // Natural to sharp/flat conversions
            (NoteName.E, Alteration.Natural) => new Note(NoteName.F, Alteration.Flat, Octave),
            (NoteName.F, Alteration.Natural) => new Note(NoteName.E, Alteration.Sharp, Octave),
            (NoteName.B, Alteration.Natural) => new Note(NoteName.C, Alteration.Flat, Octave + 1),
            (NoteName.C, Alteration.Natural) => new Note(NoteName.B, Alteration.Sharp, Octave - 1),
            
            // Sharp/flat to natural conversions
            (NoteName.E, Alteration.Sharp) => new Note(NoteName.F, Alteration.Natural, Octave),
            (NoteName.F, Alteration.Flat) => new Note(NoteName.E, Alteration.Natural, Octave),
            (NoteName.B, Alteration.Sharp) => new Note(NoteName.C, Alteration.Natural, Octave + 1),
            (NoteName.C, Alteration.Flat) => new Note(NoteName.B, Alteration.Natural, Octave - 1),
            
            // Double alterations to simpler equivalents
            (NoteName.C, Alteration.DoubleSharp) => new Note(NoteName.D, Alteration.Natural, Octave),
            (NoteName.D, Alteration.DoubleFlat) => new Note(NoteName.C, Alteration.Natural, Octave),
            (NoteName.D, Alteration.DoubleSharp) => new Note(NoteName.E, Alteration.Natural, Octave),
            (NoteName.E, Alteration.DoubleFlat) => new Note(NoteName.D, Alteration.Natural, Octave),
            (NoteName.E, Alteration.DoubleSharp) => new Note(NoteName.F, Alteration.Sharp, Octave),
            (NoteName.F, Alteration.DoubleFlat) => new Note(NoteName.E, Alteration.Flat, Octave),
            (NoteName.F, Alteration.DoubleSharp) => new Note(NoteName.G, Alteration.Natural, Octave),
            (NoteName.G, Alteration.DoubleFlat) => new Note(NoteName.F, Alteration.Natural, Octave),
            (NoteName.G, Alteration.DoubleSharp) => new Note(NoteName.A, Alteration.Natural, Octave),
            (NoteName.A, Alteration.DoubleFlat) => new Note(NoteName.G, Alteration.Natural, Octave),
            (NoteName.A, Alteration.DoubleSharp) => new Note(NoteName.B, Alteration.Natural, Octave),
            (NoteName.B, Alteration.DoubleFlat) => new Note(NoteName.A, Alteration.Natural, Octave),
            (NoteName.B, Alteration.DoubleSharp) => new Note(NoteName.C, Alteration.Sharp, Octave + 1),
            
            // No simple equivalent for natural D, G, A
            _ => null
        };
    }

    /// <summary>
    /// Gets all possible enharmonic equivalents of this note.
    /// </summary>
    /// <returns>A collection of all enharmonically equivalent notes.</returns>
    public IEnumerable<Note> GetAllEnharmonicEquivalents()
    {
        var equivalents = new List<Note>();
        var targetSemitones = GetSemitonesFromC() % MusicTheoryConstants.SemitonesPerOctave;
        
        // Check all possible note name and alteration combinations
        foreach (NoteName noteName in Enum.GetValues<NoteName>())
        {
            foreach (Alteration alteration in Enum.GetValues<Alteration>())
            {
                // Skip the current note itself
                if (noteName == Name && alteration == Alteration)
                    continue;
                
                var candidate = new Note(noteName, alteration, Octave);
                if (candidate.GetSemitonesFromC() % MusicTheoryConstants.SemitonesPerOctave == targetSemitones)
                {
                    equivalents.Add(candidate);
                }
            }
        }
        
        return equivalents;
    }

    /// <summary>
    /// Returns a simplified version of this note, preferring natural notes and single alterations.
    /// </summary>
    /// <returns>A simplified enharmonic equivalent.</returns>
    public Note SimplifyEnharmonic()
    {
        // Only simplify specific cases where there's a simpler equivalent
        // E#/Fb and B#/Cb should be simplified to their natural equivalents
        // Double alterations should also be simplified
        
        var equivalent = GetEnharmonicEquivalent();
        
        // Check if the equivalent is simpler (natural or single alteration vs double)
        if (equivalent != null)
        {
            // Always simplify double alterations
            if (Alteration == Alteration.DoubleSharp || Alteration == Alteration.DoubleFlat)
                return equivalent;
            
            // Simplify E#/Fb and B#/Cb to their natural equivalents
            if ((Name == NoteName.E && Alteration == Alteration.Sharp) ||
                (Name == NoteName.F && Alteration == Alteration.Flat) ||
                (Name == NoteName.B && Alteration == Alteration.Sharp) ||
                (Name == NoteName.C && Alteration == Alteration.Flat))
            {
                return equivalent;
            }
        }
        
        // Otherwise, keep the note as-is (including C#, Db, etc.)
        return this;
    }

    /// <summary>
    /// Parses a string representation of a note.
    /// </summary>
    /// <param name="noteString">The string to parse (e.g., "C#4", "Bb3", "F##5", "B♭", "bb"). The letter may be
    /// lower-case, accidentals may be ASCII (<c>#</c>, <c>b</c>, <c>##</c>, <c>bb</c>) or Unicode (♯ ♭ 𝄪 𝄫),
    /// and the octave is optional (default 4).</param>
    /// <returns>A new Note instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the string cannot be parsed.</exception>
    public static Note Parse(string noteString)
    {
        if (string.IsNullOrWhiteSpace(noteString))
            throw new ArgumentException("Note string cannot be empty.", nameof(noteString));

        if (!TryParsePitchClass(noteString, out var pitch, out var consumed))
            throw new ArgumentException($"Invalid note name: {noteString[0]}", nameof(noteString));

        if (consumed == noteString.Length)
            return pitch;

        var rest = noteString[consumed..];
        if (!int.TryParse(rest, out var octave))
            throw new ArgumentException($"Invalid octave: {rest}", nameof(noteString));

        return new Note(pitch.Name, pitch.Alteration, octave);
    }

    /// <summary>
    /// Tries to parse a note; see <see cref="Parse"/> for the accepted syntax.
    /// </summary>
    /// <param name="text">The string to parse.</param>
    /// <param name="note">The parsed note, or null.</param>
    /// <returns>True when <paramref name="text"/> is a valid note.</returns>
    public static bool TryParse(string? text, [NotNullWhen(true)] out Note? note)
    {
        note = null;
        if (string.IsNullOrWhiteSpace(text) || !TryParsePitchClass(text, out var pitch, out var consumed))
            return false;
        if (consumed == text.Length)
        {
            note = pitch;
            return true;
        }
        if (!int.TryParse(text.AsSpan(consumed), out var octave))
            return false;
        note = new Note(pitch.Name, pitch.Alteration, octave);
        return true;
    }

    /// <summary>
    /// Reads a note letter (any case) and up to two accidentals at the start of <paramref name="s"/>.
    /// </summary>
    /// <param name="s">The text to read from; anything after the note is ignored.</param>
    /// <param name="note">The note read, in octave 4.</param>
    /// <param name="consumed">How many characters the note used.</param>
    /// <returns>True when <paramref name="s"/> starts with a note letter.</returns>
    public static bool TryParsePitchClass(ReadOnlySpan<char> s, out Note note, out int consumed)
    {
        note = null!;
        consumed = 0;
        if (s.IsEmpty)
            return false;

        int letter = "CDEFGAB".IndexOf(char.ToUpperInvariant(s[0]));
        if (letter < 0)
            return false;

        int i = 1, alteration = 0, count = 0;
        while (i < s.Length && count < 2)
        {
            int step, width = 1;
            if (s[i] is '#' or '♯') step = 1;
            else if (s[i] is 'b' or '♭') step = -1;
            else if (s[i..].StartsWith("𝄪")) { step = 2; width = 2; }
            else if (s[i..].StartsWith("𝄫")) { step = -2; width = 2; }
            else break;

            if (alteration != 0 && Math.Sign(step) != Math.Sign(alteration) || Math.Abs(alteration + step) > 2)
                break;
            alteration += step;
            i += width;
            count++;
        }

        note = new Note((NoteName)letter, (Alteration)alteration);
        consumed = i;
        return true;
    }
}
