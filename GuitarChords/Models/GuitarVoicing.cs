using MusicTheory;

namespace GuitarChords.Models;

/// <summary>
/// Represents a specific way to play a chord on the guitar (voicing).
/// Contains more detailed information than GuitarChord for algorithmic generation.
/// </summary>
public class GuitarVoicing
{
    /// <summary>
    /// The underlying music theory chord.
    /// </summary>
    public Chord Chord { get; }

    /// <summary>
    /// Fret positions for each string (0 = open, -1 = muted/not played).
    /// Index 0 = Low E (6th string), Index 5 = High E (1st string).
    /// </summary>
    public int[] FretPositions { get; }

    /// <summary>
    /// The actual notes produced at each string position.
    /// Null for muted strings.
    /// </summary>
    public Note?[] Notes { get; }

    /// <summary>
    /// Suggested finger assignments (1-4 for fingers, 0 for open/thumb, -1 for not used).
    /// </summary>
    public int[] Fingerings { get; }

    /// <summary>
    /// The lowest non-open fret used in this voicing.
    /// </summary>
    public int LowestFret { get; }

    /// <summary>
    /// The highest fret used in this voicing.
    /// </summary>
    public int HighestFret { get; }

    /// <summary>
    /// The span between lowest and highest frets (excluding open strings).
    /// </summary>
    public int FretSpan => HighestFret > 0 && LowestFret > 0 ? HighestFret - LowestFret : 0;

    /// <summary>
    /// The base fret for diagram positioning.
    /// </summary>
    public int BaseFret => LowestFret > 0 ? LowestFret : 0;

    /// <summary>
    /// Indicates if this is a barre chord.
    /// </summary>
    public bool IsBarreChord { get; private set; }

    /// <summary>
    /// If it's a barre chord, which fret is barred.
    /// </summary>
    public int? BarreFret { get; private set; }

    /// <summary>
    /// Which strings are included in the barre (if any).
    /// </summary>
    public List<int> BarredStrings { get; }

    /// <summary>
    /// The number of fingers required (excluding open strings and thumb).
    /// </summary>
    public int FingersRequired { get; private set; }

    /// <summary>
    /// The number of strings actually played (not muted).
    /// </summary>
    public int StringsPlayed { get; }

    /// <summary>
    /// The number of open strings in this voicing.
    /// </summary>
    public int OpenStrings { get; }

    /// <summary>
    /// The number of muted strings in this voicing.
    /// </summary>
    public int MutedStrings { get; }

    /// <summary>
    /// Gets the chord name/symbol.
    /// </summary>
    public string Name => Chord.GetSymbol();

    /// <summary>
    /// Position label for this voicing (e.g., "Open", "Barre 3", "Position 5").
    /// </summary>
    public string PositionLabel { get; private set; } = "Open";

    /// <summary>
    /// Creates a new guitar voicing.
    /// </summary>
    /// <param name="chord">The underlying chord.</param>
    /// <param name="fretPositions">Fret position for each string.</param>
    /// <param name="fingerings">Optional finger assignments.</param>
    public GuitarVoicing(Chord chord, int[] fretPositions, int[]? fingerings = null)
    {
        if (fretPositions.Length != 6)
            throw new ArgumentException("Fret positions must have exactly 6 elements.", nameof(fretPositions));

        Chord = chord;
        FretPositions = fretPositions.ToArray();
        Fingerings = fingerings?.ToArray() ?? new int[6];
        BarredStrings = new List<int>();
        Notes = new Note?[6];

        // Calculate derived properties
        var playedFrets = fretPositions.Where(f => f > 0).ToList();
        LowestFret = playedFrets.Any() ? playedFrets.Min() : 0;
        HighestFret = playedFrets.Any() ? playedFrets.Max() : 0;

        OpenStrings = fretPositions.Count(f => f == 0);
        MutedStrings = fretPositions.Count(f => f == -1);
        StringsPlayed = 6 - MutedStrings;

        // Generate notes for each string
        for (int i = 0; i < 6; i++)
        {
            if (fretPositions[i] >= 0)
            {
                Notes[i] = GuitarTuning.GetNoteAtFret(i, fretPositions[i]);
            }
        }

        // Detect barre chords and calculate fingerings
        DetectBarreChord();
        if (fingerings == null || fingerings.All(f => f == 0))
        {
            CalculateFingerings();
        }
        CalculateFingersRequired();
        GeneratePositionLabel();
    }

    /// <summary>
    /// Converts this voicing to a GuitarChord for compatibility with existing code.
    /// </summary>
    public GuitarChord ToGuitarChord()
    {
        return new GuitarChord(Chord)
        {
            FretPositions = FretPositions.ToArray(),
            Fingerings = Fingerings.ToArray(),
            BaseFret = BaseFret,
            IsBarreChord = IsBarreChord,
            BarreFret = BarreFret,
            BarredStrings = BarredStrings.ToList()
        };
    }

    private void DetectBarreChord()
    {
        // Check for barre by looking for the same fret across multiple consecutive strings
        var fretGroups = FretPositions
            .Select((fret, index) => new { Fret = fret, Index = index })
            .Where(x => x.Fret > 0)
            .GroupBy(x => x.Fret)
            .Where(g => g.Count() >= 2);

        foreach (var group in fretGroups)
        {
            var indices = group.Select(x => x.Index).OrderBy(x => x).ToList();

            // Check if strings are consecutive or near-consecutive (allowing for one gap)
            if (indices.Last() - indices.First() <= indices.Count)
            {
                // This looks like a barre
                IsBarreChord = true;
                BarreFret = group.Key;
                BarredStrings.Clear();
                BarredStrings.AddRange(indices);
                break;
            }
        }
    }

    private void CalculateFingerings()
    {
        // Simple fingering algorithm
        var usedFingers = new bool[5]; // 0=thumb, 1-4=fingers
        var frettedPositions = FretPositions
            .Select((fret, index) => new { Fret = fret, Index = index })
            .Where(x => x.Fret > 0)
            .OrderBy(x => x.Fret)
            .ThenBy(x => x.Index)
            .ToList();

        if (IsBarreChord && BarreFret.HasValue)
        {
            // Assign index finger (1) to all barred strings
            foreach (var barredString in BarredStrings)
            {
                Fingerings[barredString] = 1;
            }
            usedFingers[1] = true;

            // Assign remaining fingers to other positions
            int nextFinger = 2;
            foreach (var pos in frettedPositions.Where(p => !BarredStrings.Contains(p.Index)))
            {
                if (nextFinger <= 4)
                {
                    Fingerings[pos.Index] = nextFinger++;
                }
            }
        }
        else
        {
            // Non-barre chord: assign fingers in order of fret position
            int finger = 1;
            foreach (var pos in frettedPositions)
            {
                if (finger <= 4)
                {
                    Fingerings[pos.Index] = finger++;
                }
            }
        }
    }

    private void CalculateFingersRequired()
    {
        if (IsBarreChord)
        {
            // Barre counts as 1 finger + other positions
            var otherPositions = FretPositions
                .Select((fret, index) => new { Fret = fret, Index = index })
                .Where(x => x.Fret > 0 && !BarredStrings.Contains(x.Index))
                .Select(x => x.Fret)
                .Distinct()
                .Count();

            FingersRequired = 1 + otherPositions;
        }
        else
        {
            // Count distinct fretted positions
            FingersRequired = FretPositions
                .Where(f => f > 0)
                .Distinct()
                .Count();

            // Adjust for positions that might need multiple fingers
            var positionsByFret = FretPositions
                .Select((fret, index) => new { Fret = fret, Index = index })
                .Where(x => x.Fret > 0)
                .GroupBy(x => x.Fret);

            int totalFingers = 0;
            foreach (var group in positionsByFret)
            {
                totalFingers += group.Count();
            }
            FingersRequired = Math.Min(totalFingers, 4);
        }
    }

    private void GeneratePositionLabel()
    {
        if (OpenStrings > 0 && LowestFret <= 3)
        {
            PositionLabel = "Open";
        }
        else if (IsBarreChord && BarreFret.HasValue)
        {
            PositionLabel = $"Barre {BarreFret}";
        }
        else if (LowestFret > 0)
        {
            PositionLabel = $"Position {LowestFret}";
        }
        else
        {
            PositionLabel = "Open";
        }
    }
}
