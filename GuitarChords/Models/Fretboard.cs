using MusicTheory;

namespace GuitarChords.Models;

/// <summary>
/// Represents a position on the guitar fretboard.
/// </summary>
/// <param name="StringIndex">The string index (0 = Low E, 5 = High E).</param>
/// <param name="Fret">The fret number (0 = open string).</param>
/// <param name="Note">The note produced at this position.</param>
public record FretboardPosition(int StringIndex, int Fret, Note Note)
{
    /// <summary>
    /// Gets the pitch class (note name + alteration) without octave.
    /// </summary>
    public (NoteName Name, Alteration Alteration) PitchClass => (Note.Name, Note.Alteration);

    /// <summary>
    /// Gets the semitone value (0-11) for this position.
    /// </summary>
    public int Semitone => GetSemitone(Note.Name, Note.Alteration);

    private static int GetSemitone(NoteName name, Alteration alteration)
    {
        int baseSemitone = name switch
        {
            NoteName.C => 0,
            NoteName.D => 2,
            NoteName.E => 4,
            NoteName.F => 5,
            NoteName.G => 7,
            NoteName.A => 9,
            NoteName.B => 11,
            _ => 0
        };
        return (baseSemitone + (int)alteration + 12) % 12;
    }
}

/// <summary>
/// Mathematical model of a guitar fretboard that can find note positions.
/// </summary>
public class Fretboard
{
    private readonly Note[] _tuning;
    private readonly int _numberOfFrets;
    private readonly List<FretboardPosition> _allPositions;

    /// <summary>
    /// Creates a new fretboard with the specified tuning.
    /// </summary>
    /// <param name="tuning">The tuning for each string (6 notes for standard guitar).</param>
    /// <param name="numberOfFrets">The number of frets on the guitar (default 22).</param>
    public Fretboard(Note[]? tuning = null, int numberOfFrets = 22)
    {
        _tuning = tuning ?? GuitarTuning.StandardTuning;
        _numberOfFrets = numberOfFrets;
        _allPositions = GenerateAllPositions();
    }

    /// <summary>
    /// Gets the tuning for this fretboard.
    /// </summary>
    public IReadOnlyList<Note> Tuning => _tuning;

    /// <summary>
    /// Gets all positions on the fretboard.
    /// </summary>
    public IReadOnlyList<FretboardPosition> AllPositions => _allPositions;

    /// <summary>
    /// Finds all positions on the fretboard that produce the given pitch class.
    /// </summary>
    /// <param name="name">The note name.</param>
    /// <param name="alteration">The alteration.</param>
    /// <param name="maxFret">Maximum fret to search (default 15).</param>
    /// <returns>All positions that produce this pitch class.</returns>
    public IEnumerable<FretboardPosition> FindPositionsForPitchClass(
        NoteName name,
        Alteration alteration,
        int maxFret = 15)
    {
        int targetSemitone = GetSemitone(name, alteration);

        return _allPositions
            .Where(p => p.Fret <= maxFret && p.Semitone == targetSemitone)
            .OrderBy(p => p.Fret)
            .ThenBy(p => p.StringIndex);
    }

    /// <summary>
    /// Finds all positions on the fretboard that match the given note (including octave).
    /// </summary>
    /// <param name="note">The note to find.</param>
    /// <param name="maxFret">Maximum fret to search (default 15).</param>
    /// <returns>All positions that produce this exact note.</returns>
    public IEnumerable<FretboardPosition> FindPositionsForNote(Note note, int maxFret = 15)
    {
        return _allPositions
            .Where(p => p.Fret <= maxFret &&
                       p.Note.Name == note.Name &&
                       p.Note.Alteration == note.Alteration &&
                       p.Note.Octave == note.Octave)
            .OrderBy(p => p.Fret)
            .ThenBy(p => p.StringIndex);
    }

    /// <summary>
    /// Gets the note at a specific position on the fretboard.
    /// </summary>
    /// <param name="stringIndex">The string index (0-5).</param>
    /// <param name="fret">The fret number (0 = open).</param>
    /// <returns>The note at this position.</returns>
    public Note GetNoteAt(int stringIndex, int fret)
    {
        return GuitarTuning.GetNoteAtFret(stringIndex, fret);
    }

    /// <summary>
    /// Gets all positions within a fret range.
    /// </summary>
    /// <param name="minFret">Minimum fret (inclusive).</param>
    /// <param name="maxFret">Maximum fret (inclusive).</param>
    /// <returns>All positions in the range.</returns>
    public IEnumerable<FretboardPosition> GetPositionsInRange(int minFret, int maxFret)
    {
        return _allPositions.Where(p => p.Fret >= minFret && p.Fret <= maxFret);
    }

    private List<FretboardPosition> GenerateAllPositions()
    {
        var positions = new List<FretboardPosition>();

        for (int stringIndex = 0; stringIndex < _tuning.Length; stringIndex++)
        {
            for (int fret = 0; fret <= _numberOfFrets; fret++)
            {
                var note = GuitarTuning.GetNoteAtFret(stringIndex, fret);
                positions.Add(new FretboardPosition(stringIndex, fret, note));
            }
        }

        return positions;
    }

    private static int GetSemitone(NoteName name, Alteration alteration)
    {
        int baseSemitone = name switch
        {
            NoteName.C => 0,
            NoteName.D => 2,
            NoteName.E => 4,
            NoteName.F => 5,
            NoteName.G => 7,
            NoteName.A => 9,
            NoteName.B => 11,
            _ => 0
        };
        return (baseSemitone + (int)alteration + 12) % 12;
    }
}
