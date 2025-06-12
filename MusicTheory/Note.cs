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
        const double a4Frequency = 440.0;
        
        // Calculate semitones from A4
        int semitonesFromA4 = GetSemitonesFromA4();
        
        // Apply equal temperament formula: f = 440 * 2^(n/12)
        // where n is the number of semitones from A4
        return a4Frequency * Math.Pow(2.0, semitonesFromA4 / 12.0);
    }

    /// <summary>
    /// Gets the number of semitones from A4 to this note.
    /// </summary>
    /// <returns>The number of semitones (positive for higher notes, negative for lower).</returns>
    private int GetSemitonesFromA4()
    {
        // Semitones from C to each note
        int[] semitonesFromC = { 0, 2, 4, 5, 7, 9, 11 }; // C, D, E, F, G, A, B
        
        // Calculate semitones from C0 to this note
        int semitonesFromC0 = Octave * 12 + semitonesFromC[(int)Name] + (int)Alteration;
        
        // A4 is 57 semitones from C0 (4 * 12 + 9 = 57)
        const int a4SemitonesFromC0 = 57;
        
        return semitonesFromC0 - a4SemitonesFromC0;
    }
}