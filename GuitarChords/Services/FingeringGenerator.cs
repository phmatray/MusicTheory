using GuitarChords.Models;
using MusicTheory;

namespace GuitarChords.Services;

/// <summary>
/// Generates guitar chord fingerings algorithmically.
/// </summary>
public class FingeringGenerator : IFingeringGenerator
{
    private readonly Fretboard _fretboard;
    private readonly IPlayabilityScorer _scorer;

    public FingeringGenerator(IPlayabilityScorer? scorer = null)
    {
        _fretboard = new Fretboard();
        _scorer = scorer ?? new PlayabilityScorer();
    }

    /// <inheritdoc/>
    public IEnumerable<GuitarVoicing> GenerateVoicings(Chord chord, FingeringOptions? options = null)
    {
        options ??= new FingeringOptions();

        var chordNotes = chord.GetNotes().ToList();
        var rootNote = chord.Root;

        // Find all positions for each chord tone
        var positionsPerNote = new Dictionary<int, List<FretboardPosition>>();

        foreach (var note in chordNotes)
        {
            int semitone = GetSemitone(note.Name, note.Alteration);
            if (!positionsPerNote.ContainsKey(semitone))
            {
                var positions = _fretboard.FindPositionsForPitchClass(
                    note.Name,
                    note.Alteration,
                    options.FretRange?.Max ?? options.MaxFret
                ).ToList();

                if (options.FretRange.HasValue)
                {
                    positions = positions.Where(p => p.Fret >= options.FretRange.Value.Min).ToList();
                }

                positionsPerNote[semitone] = positions;
            }
        }

        // Generate candidate voicings
        var candidates = GenerateCandidateVoicings(chord, chordNotes, positionsPerNote, options);

        // Score and filter
        var scoredVoicings = candidates
            .Select(v => (Voicing: v, Score: _scorer.Score(v)))
            .Where(x => x.Score.TotalScore >= options.MinPlayabilityScore)
            .Where(x => x.Score.Difficulty <= options.MaxDifficulty)
            .OrderByDescending(x => x.Score.TotalScore)
            .Take(options.MaxResults)
            .Select(x => x.Voicing);

        return scoredVoicings;
    }

    /// <inheritdoc/>
    public GuitarVoicing? GenerateBestVoicing(Chord chord, FingeringOptions? options = null)
    {
        return GenerateVoicings(chord, options).FirstOrDefault();
    }

    /// <inheritdoc/>
    public IEnumerable<GuitarVoicing> GenerateVoicingsInRange(Chord chord, int minFret, int maxFret)
    {
        var options = new FingeringOptions
        {
            FretRange = (minFret, maxFret),
            MaxResults = 20
        };

        return GenerateVoicings(chord, options);
    }

    private IEnumerable<GuitarVoicing> GenerateCandidateVoicings(
        Chord chord,
        List<Note> chordNotes,
        Dictionary<int, List<FretboardPosition>> positionsPerNote,
        FingeringOptions options)
    {
        var rootSemitone = GetSemitone(chord.Root.Name, chord.Root.Alteration);
        var results = new List<GuitarVoicing>();

        // We need to assign one position per string, ensuring we cover all chord tones
        // Start with common patterns and then explore variations

        // Strategy 1: Open position voicings (frets 0-3)
        if (options.PreferOpenStrings || options.MaxFret >= 3)
        {
            var openVoicings = GenerateOpenPositionVoicings(chord, chordNotes, positionsPerNote, options, rootSemitone);
            results.AddRange(openVoicings);
        }

        // Strategy 2: Barre chord voicings based on moveable shapes
        if (options.AllowBarreChords)
        {
            var barreVoicings = GenerateBarreVoicings(chord, chordNotes, positionsPerNote, options, rootSemitone);
            results.AddRange(barreVoicings);
        }

        // Strategy 3: Explore other positions systematically
        var exploredVoicings = ExploreSystematically(chord, chordNotes, positionsPerNote, options, rootSemitone);
        results.AddRange(exploredVoicings);

        // Remove duplicates
        return results.DistinctBy(v => string.Join(",", v.FretPositions));
    }

    private IEnumerable<GuitarVoicing> GenerateOpenPositionVoicings(
        Chord chord,
        List<Note> chordNotes,
        Dictionary<int, List<FretboardPosition>> positionsPerNote,
        FingeringOptions options,
        int rootSemitone)
    {
        var results = new List<GuitarVoicing>();

        // Try to build voicings using open strings and first 3 frets
        var openPositions = positionsPerNote.Values
            .SelectMany(p => p)
            .Where(p => p.Fret <= 3)
            .ToList();

        // Group by string
        var positionsByString = openPositions
            .GroupBy(p => p.StringIndex)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Try to build a valid voicing
        var voicing = TryBuildVoicing(chord, chordNotes, positionsByString, options, rootSemitone, maxFret: 3);
        if (voicing != null)
        {
            results.Add(voicing);
        }

        return results;
    }

    private IEnumerable<GuitarVoicing> GenerateBarreVoicings(
        Chord chord,
        List<Note> chordNotes,
        Dictionary<int, List<FretboardPosition>> positionsPerNote,
        FingeringOptions options,
        int rootSemitone)
    {
        var results = new List<GuitarVoicing>();

        // Find root note positions on bass strings (strings 0-2)
        var rootPositions = positionsPerNote.TryGetValue(rootSemitone, out var pos)
            ? pos.Where(p => p.StringIndex <= 2 && p.Fret > 0 && p.Fret <= options.MaxFret)
            : Enumerable.Empty<FretboardPosition>();

        foreach (var rootPos in rootPositions.Take(5)) // Limit to avoid explosion
        {
            int baseFret = rootPos.Fret;

            // Try E-shape barre (root on string 0)
            if (rootPos.StringIndex == 0)
            {
                var eShapeVoicing = TryEShapeBarre(chord, chordNotes, baseFret, options);
                if (eShapeVoicing != null)
                    results.Add(eShapeVoicing);
            }

            // Try A-shape barre (root on string 1)
            if (rootPos.StringIndex == 1)
            {
                var aShapeVoicing = TryAShapeBarre(chord, chordNotes, baseFret, options);
                if (aShapeVoicing != null)
                    results.Add(aShapeVoicing);
            }
        }

        return results;
    }

    private GuitarVoicing? TryEShapeBarre(Chord chord, List<Note> chordNotes, int baseFret, FingeringOptions options)
    {
        // E-shape major barre: Root on 6th string
        // Pattern: [0, 0, 2, 2, 2, 0] relative to barre position
        var chordType = chord.Type;
        int[] relativePattern;

        if (chordType == ChordType.Major)
            relativePattern = new[] { 0, 0, 2, 2, 2, 0 };
        else if (chordType == ChordType.Minor)
            relativePattern = new[] { 0, 0, 2, 2, 1, 0 };
        else if (chordType == ChordType.Dominant7)
            relativePattern = new[] { 0, 0, 2, 0, 2, 0 };
        else if (chordType == ChordType.Minor7)
            relativePattern = new[] { 0, 0, 2, 0, 1, 0 };
        else if (chordType == ChordType.Major7)
            relativePattern = new[] { 0, 0, 2, 1, 2, 0 };
        else
            return null; // Shape not defined for this chord type

        var fretPositions = relativePattern.Select(f => f + baseFret).ToArray();

        // Validate that the voicing produces the correct notes
        if (!ValidateVoicing(chord, fretPositions))
            return null;

        return new GuitarVoicing(chord, fretPositions);
    }

    private GuitarVoicing? TryAShapeBarre(Chord chord, List<Note> chordNotes, int baseFret, FingeringOptions options)
    {
        // A-shape major barre: Root on 5th string
        // Pattern: [-1, 0, 2, 2, 2, 0] relative to barre position
        var chordType = chord.Type;
        int[] relativePattern;

        if (chordType == ChordType.Major)
            relativePattern = new[] { -1, 0, 2, 2, 2, 0 };
        else if (chordType == ChordType.Minor)
            relativePattern = new[] { -1, 0, 2, 2, 1, 0 };
        else if (chordType == ChordType.Dominant7)
            relativePattern = new[] { -1, 0, 2, 0, 2, 0 };
        else if (chordType == ChordType.Minor7)
            relativePattern = new[] { -1, 0, 2, 0, 1, 0 };
        else
            return null;

        var fretPositions = relativePattern.Select(f => f == -1 ? -1 : f + baseFret).ToArray();

        if (!ValidateVoicing(chord, fretPositions))
            return null;

        return new GuitarVoicing(chord, fretPositions);
    }

    private IEnumerable<GuitarVoicing> ExploreSystematically(
        Chord chord,
        List<Note> chordNotes,
        Dictionary<int, List<FretboardPosition>> positionsPerNote,
        FingeringOptions options,
        int rootSemitone)
    {
        var results = new List<GuitarVoicing>();
        var chordSemitones = chordNotes.Select(n => GetSemitone(n.Name, n.Alteration)).ToHashSet();

        // Try building voicings starting from different fret positions
        for (int startFret = 0; startFret <= options.MaxFret - 3; startFret += 2)
        {
            int endFret = Math.Min(startFret + options.MaxFretSpan, options.MaxFret);

            var positionsInRange = positionsPerNote.Values
                .SelectMany(p => p)
                .Where(p => p.Fret >= startFret && p.Fret <= endFret)
                .GroupBy(p => p.StringIndex)
                .ToDictionary(g => g.Key, g => g.ToList());

            var voicing = TryBuildVoicing(chord, chordNotes, positionsInRange, options, rootSemitone, endFret);
            if (voicing != null)
            {
                results.Add(voicing);
            }
        }

        return results;
    }

    private GuitarVoicing? TryBuildVoicing(
        Chord chord,
        List<Note> chordNotes,
        Dictionary<int, List<FretboardPosition>> positionsByString,
        FingeringOptions options,
        int rootSemitone,
        int maxFret)
    {
        var chordSemitones = chordNotes.Select(n => GetSemitone(n.Name, n.Alteration)).ToHashSet();
        var fretPositions = new int[6];
        var coveredSemitones = new HashSet<int>();
        bool hasRoot = false;

        // Start from bass strings
        for (int stringIndex = 0; stringIndex < 6; stringIndex++)
        {
            if (!positionsByString.TryGetValue(stringIndex, out var positions) || !positions.Any())
            {
                // Check if we can use open string
                var openNote = GuitarTuning.GetNoteAtFret(stringIndex, 0);
                var openSemitone = GetSemitone(openNote.Name, openNote.Alteration);

                if (chordSemitones.Contains(openSemitone))
                {
                    fretPositions[stringIndex] = 0;
                    coveredSemitones.Add(openSemitone);
                    if (openSemitone == rootSemitone && stringIndex <= 2)
                        hasRoot = true;
                }
                else
                {
                    fretPositions[stringIndex] = -1; // Muted
                }
                continue;
            }

            // Choose the best position for this string
            FretboardPosition? bestPosition = null;

            // Prioritize: root on bass strings, then other chord tones
            if (stringIndex <= 2 && !hasRoot)
            {
                bestPosition = positions.FirstOrDefault(p => p.Semitone == rootSemitone);
                if (bestPosition != null)
                    hasRoot = true;
            }

            if (bestPosition == null)
            {
                // Choose a chord tone we haven't covered yet, or reinforce the root
                bestPosition = positions
                    .Where(p => chordSemitones.Contains(p.Semitone))
                    .OrderBy(p => coveredSemitones.Contains(p.Semitone) ? 1 : 0) // Prefer uncovered
                    .ThenBy(p => p.Fret) // Prefer lower frets
                    .FirstOrDefault();
            }

            if (bestPosition != null)
            {
                fretPositions[stringIndex] = bestPosition.Fret;
                coveredSemitones.Add(bestPosition.Semitone);
            }
            else
            {
                fretPositions[stringIndex] = -1;
            }
        }

        // Validate
        int mutedStrings = fretPositions.Count(f => f == -1);
        if (mutedStrings > options.MaxMutedStrings)
            return null;

        if (options.RequireRootInBass && !hasRoot)
            return null;

        // Check that we have enough chord tones
        if (coveredSemitones.Count < Math.Min(3, chordSemitones.Count))
            return null;

        // Check fret span
        var playedFrets = fretPositions.Where(f => f > 0).ToList();
        if (playedFrets.Any())
        {
            int span = playedFrets.Max() - playedFrets.Min();
            if (span > options.MaxFretSpan)
                return null;
        }

        return new GuitarVoicing(chord, fretPositions);
    }

    private bool ValidateVoicing(Chord chord, int[] fretPositions)
    {
        var chordNotes = chord.GetNotes().ToList();
        var chordSemitones = chordNotes.Select(n => GetSemitone(n.Name, n.Alteration)).ToHashSet();
        var voicingSemitones = new HashSet<int>();

        for (int i = 0; i < 6; i++)
        {
            if (fretPositions[i] >= 0)
            {
                var note = GuitarTuning.GetNoteAtFret(i, fretPositions[i]);
                var semitone = GetSemitone(note.Name, note.Alteration);
                voicingSemitones.Add(semitone);
            }
        }

        // Check that all played notes are chord tones
        if (!voicingSemitones.All(s => chordSemitones.Contains(s)))
            return false;

        // Check that we have at least root and one other chord tone
        var rootSemitone = GetSemitone(chord.Root.Name, chord.Root.Alteration);
        if (!voicingSemitones.Contains(rootSemitone))
            return false;

        return voicingSemitones.Count >= 2;
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
