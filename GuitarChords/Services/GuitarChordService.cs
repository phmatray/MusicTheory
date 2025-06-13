using GuitarChords.Models;
using MusicTheory;

namespace GuitarChords.Services;

public class GuitarChordService
{
    // Common open chord shapes
    private static readonly Dictionary<string, GuitarChord> CommonChords = new();

    static GuitarChordService()
    {
        InitializeCommonChords();
    }

    private static void InitializeCommonChords()
    {
        // C Major
        var cMajor = new GuitarChord(new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major))
        {
            FretPositions = new[] { -1, 3, 2, 0, 1, 0 },
            Fingerings = new[] { -1, 3, 2, 0, 1, 0 },
            BaseFret = 0
        };
        CommonChords["C"] = cMajor;

        // G Major
        var gMajor = new GuitarChord(new Chord(new Note(NoteName.G, Alteration.Natural, 3), ChordType.Major))
        {
            FretPositions = new[] { 3, 2, 0, 0, 3, 3 },
            Fingerings = new[] { 2, 1, 0, 0, 3, 4 },
            BaseFret = 0
        };
        CommonChords["G"] = gMajor;

        // D Major
        var dMajor = new GuitarChord(new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.Major))
        {
            FretPositions = new[] { -1, -1, 0, 2, 3, 2 },
            Fingerings = new[] { -1, -1, 0, 1, 3, 2 },
            BaseFret = 0
        };
        CommonChords["D"] = dMajor;

        // A Major
        var aMajor = new GuitarChord(new Chord(new Note(NoteName.A, Alteration.Natural, 3), ChordType.Major))
        {
            FretPositions = new[] { -1, 0, 2, 2, 2, 0 },
            Fingerings = new[] { -1, 0, 1, 2, 3, 0 },
            BaseFret = 0
        };
        CommonChords["A"] = aMajor;

        // E Major
        var eMajor = new GuitarChord(new Chord(new Note(NoteName.E, Alteration.Natural, 3), ChordType.Major))
        {
            FretPositions = new[] { 0, 2, 2, 1, 0, 0 },
            Fingerings = new[] { 0, 2, 3, 1, 0, 0 },
            BaseFret = 0
        };
        CommonChords["E"] = eMajor;

        // F Major (barre chord)
        var fMajor = new GuitarChord(new Chord(new Note(NoteName.F, Alteration.Natural, 3), ChordType.Major))
        {
            FretPositions = new[] { 1, 3, 3, 2, 1, 1 },
            Fingerings = new[] { 1, 3, 4, 2, 1, 1 },
            BaseFret = 1,
            IsBarreChord = true,
            BarreFret = 1,
            BarredStrings = new List<int> { 0, 4, 5 }
        };
        CommonChords["F"] = fMajor;

        // Am (A minor)
        var aMinor = new GuitarChord(new Chord(new Note(NoteName.A, Alteration.Natural, 3), ChordType.Minor))
        {
            FretPositions = new[] { -1, 0, 2, 2, 1, 0 },
            Fingerings = new[] { -1, 0, 2, 3, 1, 0 },
            BaseFret = 0
        };
        CommonChords["Am"] = aMinor;

        // Em (E minor)
        var eMinor = new GuitarChord(new Chord(new Note(NoteName.E, Alteration.Natural, 3), ChordType.Minor))
        {
            FretPositions = new[] { 0, 2, 2, 0, 0, 0 },
            Fingerings = new[] { 0, 2, 3, 0, 0, 0 },
            BaseFret = 0
        };
        CommonChords["Em"] = eMinor;

        // Dm (D minor)
        var dMinor = new GuitarChord(new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.Minor))
        {
            FretPositions = new[] { -1, -1, 0, 2, 3, 1 },
            Fingerings = new[] { -1, -1, 0, 2, 3, 1 },
            BaseFret = 0
        };
        CommonChords["Dm"] = dMinor;
    }

    public GuitarChord? GetChord(string chordSymbol)
    {
        return CommonChords.TryGetValue(chordSymbol, out var chord) ? chord : null;
    }

    public IEnumerable<GuitarChord> GetAllChords()
    {
        return CommonChords.Values;
    }

    public IEnumerable<GuitarChord> GetChordsForRoot(Note root)
    {
        return CommonChords.Values.Where(c => c.Chord.Root.Name == root.Name && 
                                             c.Chord.Root.Alteration == root.Alteration);
    }

    public IEnumerable<GuitarChord> GetChordsOfType(ChordType type)
    {
        return CommonChords.Values.Where(c => c.Chord.Type == type);
    }

    /// <summary>
    /// Calculates possible fingerings for a given chord.
    /// This is a simplified version - a full implementation would be much more complex.
    /// </summary>
    public List<GuitarChord> CalculateChordFingerings(Chord chord, int maxFret = 5)
    {
        var fingerings = new List<GuitarChord>();
        
        // Get the notes in the chord
        var chordNotes = chord.GetNotes().ToList();
        
        // For now, just return the pre-defined chord if it exists
        var symbol = chord.GetSymbol();
        if (CommonChords.TryGetValue(symbol, out var commonChord))
        {
            fingerings.Add(commonChord);
        }
        
        // TODO: Implement algorithm to calculate possible fingerings
        // This would involve:
        // 1. Finding all positions of each chord note on the fretboard
        // 2. Finding combinations that are physically playable
        // 3. Ranking them by difficulty/commonality
        
        return fingerings;
    }
}