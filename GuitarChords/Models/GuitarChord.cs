using MusicTheory;

namespace GuitarChords.Models;

/// <summary>
/// Represents a guitar chord with fret positions for each string.
/// </summary>
public class GuitarChord
{
    /// <summary>
    /// The chord being represented.
    /// </summary>
    public Chord Chord { get; set; }
    
    /// <summary>
    /// Fret positions for each string (0 = open, -1 = muted/not played).
    /// Index 0 = Low E (6th string), Index 5 = High E (1st string).
    /// </summary>
    public int[] FretPositions { get; set; }
    
    /// <summary>
    /// The lowest fret used in the chord (for positioning the diagram).
    /// </summary>
    public int BaseFret { get; set; }
    
    /// <summary>
    /// Optional fingering information (1-4 for fingers, 0 for open, -1 for not used).
    /// </summary>
    public int[] Fingerings { get; set; }
    
    /// <summary>
    /// The name/symbol of the chord.
    /// </summary>
    public string Name => Chord.GetSymbol();
    
    /// <summary>
    /// Indicates if this is a barre chord.
    /// </summary>
    public bool IsBarreChord { get; set; }
    
    /// <summary>
    /// If it's a barre chord, which fret is barred.
    /// </summary>
    public int? BarreFret { get; set; }
    
    /// <summary>
    /// Which strings are included in the barre (if any).
    /// </summary>
    public List<int> BarredStrings { get; set; }

    public GuitarChord(Chord chord)
    {
        Chord = chord;
        FretPositions = new int[6];
        Fingerings = new int[6];
        BarredStrings = new List<int>();
    }
}

/// <summary>
/// Represents the standard guitar tuning.
/// </summary>
public class GuitarTuning
{
    /// <summary>
    /// The notes of each string in standard tuning (from low to high).
    /// </summary>
    public static readonly Note[] StandardTuning = new[]
    {
        new Note(NoteName.E, Alteration.Natural, 2), // Low E
        new Note(NoteName.A, Alteration.Natural, 2), // A
        new Note(NoteName.D, Alteration.Natural, 3), // D
        new Note(NoteName.G, Alteration.Natural, 3), // G
        new Note(NoteName.B, Alteration.Natural, 3), // B
        new Note(NoteName.E, Alteration.Natural, 4)  // High E
    };
    
    /// <summary>
    /// Gets the note at a specific fret on a specific string.
    /// </summary>
    public static Note GetNoteAtFret(int stringIndex, int fret)
    {
        if (stringIndex < 0 || stringIndex >= 6)
            throw new ArgumentOutOfRangeException(nameof(stringIndex), "String index must be between 0 and 5.");
            
        var openStringNote = StandardTuning[stringIndex];
        // Calculate the new note by adding semitones
        var totalSemitones = MusicTheoryUtilities.GetTotalSemitones(openStringNote.Name, openStringNote.Alteration, openStringNote.Octave) + fret;
        var newOctave = totalSemitones / 12;
        var semitoneInOctave = totalSemitones % 12;
        
        // Find the note name for this semitone position
        // This is a simplified version - in reality, we'd need to handle enharmonic equivalents properly
        var noteMapping = new Dictionary<int, (NoteName, Alteration)>
        {
            { 0, (NoteName.C, Alteration.Natural) },
            { 1, (NoteName.C, Alteration.Sharp) },
            { 2, (NoteName.D, Alteration.Natural) },
            { 3, (NoteName.D, Alteration.Sharp) },
            { 4, (NoteName.E, Alteration.Natural) },
            { 5, (NoteName.F, Alteration.Natural) },
            { 6, (NoteName.F, Alteration.Sharp) },
            { 7, (NoteName.G, Alteration.Natural) },
            { 8, (NoteName.G, Alteration.Sharp) },
            { 9, (NoteName.A, Alteration.Natural) },
            { 10, (NoteName.A, Alteration.Sharp) },
            { 11, (NoteName.B, Alteration.Natural) }
        };
        
        var (noteName, alteration) = noteMapping[semitoneInOctave];
        return new Note(noteName, alteration, newOctave);
    }
}