using GuitarChords.Models;
using MusicTheory;

namespace GuitarChords.Services;

/// <summary>
/// Service for managing guitar chords, combining pre-defined common chords
/// with algorithmically generated voicings.
/// </summary>
public class GuitarChordService
{
    private readonly IFingeringGenerator _fingeringGenerator;
    private readonly IPlayabilityScorer _scorer;

    // Common open chord shapes (verified, hand-crafted fingerings)
    private static readonly Dictionary<string, GuitarChord> CommonChords = new();

    // Cache for algorithmically generated voicings
    private readonly Dictionary<string, List<GuitarVoicing>> _voicingCache = new();

    static GuitarChordService()
    {
        InitializeCommonChords();
    }

    public GuitarChordService(IFingeringGenerator? fingeringGenerator = null, IPlayabilityScorer? scorer = null)
    {
        _fingeringGenerator = fingeringGenerator ?? new FingeringGenerator();
        _scorer = scorer ?? new PlayabilityScorer();
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

        // Cm (C minor) - barre chord
        var cMinor = new GuitarChord(new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Minor))
        {
            FretPositions = new[] { -1, 3, 5, 5, 4, 3 },
            Fingerings = new[] { -1, 1, 3, 4, 2, 1 },
            BaseFret = 3,
            IsBarreChord = true,
            BarreFret = 3,
            BarredStrings = new List<int> { 1, 5 }
        };
        CommonChords["Cm"] = cMinor;

        // Gm (G minor) - barre chord
        var gMinor = new GuitarChord(new Chord(new Note(NoteName.G, Alteration.Natural, 3), ChordType.Minor))
        {
            FretPositions = new[] { 3, 5, 5, 3, 3, 3 },
            Fingerings = new[] { 1, 3, 4, 1, 1, 1 },
            BaseFret = 3,
            IsBarreChord = true,
            BarreFret = 3,
            BarredStrings = new List<int> { 0, 3, 4, 5 }
        };
        CommonChords["Gm"] = gMinor;

        // Fm (F minor) - barre chord
        var fMinor = new GuitarChord(new Chord(new Note(NoteName.F, Alteration.Natural, 3), ChordType.Minor))
        {
            FretPositions = new[] { 1, 3, 3, 1, 1, 1 },
            Fingerings = new[] { 1, 3, 4, 1, 1, 1 },
            BaseFret = 1,
            IsBarreChord = true,
            BarreFret = 1,
            BarredStrings = new List<int> { 0, 3, 4, 5 }
        };
        CommonChords["Fm"] = fMinor;

        // Bm (B minor) - simplified version
        var bMinor = new GuitarChord(new Chord(new Note(NoteName.B, Alteration.Natural, 3), ChordType.Minor))
        {
            FretPositions = new[] { -1, 2, 4, 4, 3, 2 },
            Fingerings = new[] { -1, 1, 3, 4, 2, 1 },
            BaseFret = 2
        };
        CommonChords["Bm"] = bMinor;

        // G7 (G dominant 7th)
        var g7 = new GuitarChord(new Chord(new Note(NoteName.G, Alteration.Natural, 3), ChordType.Major))
        {
            FretPositions = new[] { 3, 2, 0, 0, 0, 1 },
            Fingerings = new[] { 3, 2, 0, 0, 0, 1 },
            BaseFret = 0
        };
        CommonChords["G7"] = g7;

        // C7 (C dominant 7th)
        var c7 = new GuitarChord(new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major))
        {
            FretPositions = new[] { -1, 3, 2, 3, 1, 0 },
            Fingerings = new[] { -1, 3, 2, 4, 1, 0 },
            BaseFret = 0
        };
        CommonChords["C7"] = c7;

        // D7 (D dominant 7th)
        var d7 = new GuitarChord(new Chord(new Note(NoteName.D, Alteration.Natural, 4), ChordType.Major))
        {
            FretPositions = new[] { -1, -1, 0, 2, 1, 2 },
            Fingerings = new[] { -1, -1, 0, 2, 1, 3 },
            BaseFret = 0
        };
        CommonChords["D7"] = d7;

        // A7 (A dominant 7th)
        var a7 = new GuitarChord(new Chord(new Note(NoteName.A, Alteration.Natural, 3), ChordType.Major))
        {
            FretPositions = new[] { -1, 0, 2, 0, 2, 0 },
            Fingerings = new[] { -1, 0, 2, 0, 3, 0 },
            BaseFret = 0
        };
        CommonChords["A7"] = a7;

        // E7 (E dominant 7th)
        var e7 = new GuitarChord(new Chord(new Note(NoteName.E, Alteration.Natural, 3), ChordType.Major))
        {
            FretPositions = new[] { 0, 2, 0, 1, 0, 0 },
            Fingerings = new[] { 0, 2, 0, 1, 0, 0 },
            BaseFret = 0
        };
        CommonChords["E7"] = e7;

        // B7 (B dominant 7th)
        var b7 = new GuitarChord(new Chord(new Note(NoteName.B, Alteration.Natural, 3), ChordType.Major))
        {
            FretPositions = new[] { -1, 2, 1, 2, 0, 2 },
            Fingerings = new[] { -1, 2, 1, 3, 0, 4 },
            BaseFret = 0
        };
        CommonChords["B7"] = b7;
    }

    /// <summary>
    /// Gets a pre-defined common chord by its symbol.
    /// </summary>
    public GuitarChord? GetChord(string chordSymbol)
    {
        return CommonChords.TryGetValue(chordSymbol, out var chord) ? chord : null;
    }

    /// <summary>
    /// Gets all pre-defined common chords.
    /// </summary>
    public IEnumerable<GuitarChord> GetAllChords()
    {
        return CommonChords.Values;
    }

    /// <summary>
    /// Gets all pre-defined chords for a specific root note.
    /// </summary>
    public IEnumerable<GuitarChord> GetChordsForRoot(Note root)
    {
        return CommonChords.Values.Where(c => c.Chord.Root.Name == root.Name &&
                                             c.Chord.Root.Alteration == root.Alteration);
    }

    /// <summary>
    /// Gets all pre-defined chords of a specific type.
    /// </summary>
    public IEnumerable<GuitarChord> GetChordsOfType(ChordType type)
    {
        return CommonChords.Values.Where(c => c.Chord.Type == type);
    }

    /// <summary>
    /// Gets multiple voicings for a chord, using both pre-defined and generated fingerings.
    /// </summary>
    /// <param name="chord">The chord to get voicings for.</param>
    /// <param name="options">Options controlling voicing generation.</param>
    /// <returns>A collection of voicings, sorted by playability.</returns>
    public IEnumerable<GuitarVoicing> GetVoicings(Chord chord, FingeringOptions? options = null)
    {
        var cacheKey = GetCacheKey(chord, options);

        if (!_voicingCache.TryGetValue(cacheKey, out var voicings))
        {
            voicings = new List<GuitarVoicing>();

            // First, check if we have a pre-defined chord
            var symbol = chord.GetSymbol();
            if (CommonChords.TryGetValue(symbol, out var commonChord))
            {
                // Convert to GuitarVoicing for consistent API
                var commonVoicing = new GuitarVoicing(commonChord.Chord, commonChord.FretPositions, commonChord.Fingerings);
                voicings.Add(commonVoicing);
            }

            // Then generate additional voicings algorithmically
            var generatedVoicings = _fingeringGenerator.GenerateVoicings(chord, options);
            foreach (var voicing in generatedVoicings)
            {
                // Avoid duplicates
                if (!voicings.Any(v => string.Join(",", v.FretPositions) == string.Join(",", voicing.FretPositions)))
                {
                    voicings.Add(voicing);
                }
            }

            // Sort by playability
            voicings = voicings
                .Select(v => (Voicing: v, Score: _scorer.Score(v)))
                .OrderByDescending(x => x.Score.TotalScore)
                .Select(x => x.Voicing)
                .Take(options?.MaxResults ?? 10)
                .ToList();

            _voicingCache[cacheKey] = voicings;
        }

        return voicings;
    }

    /// <summary>
    /// Gets the best (easiest to play) voicing for a chord.
    /// </summary>
    public GuitarVoicing? GetBestVoicing(Chord chord, FingeringOptions? options = null)
    {
        return GetVoicings(chord, options).FirstOrDefault();
    }

    /// <summary>
    /// Gets a GuitarChord (for compatibility with existing code) from a Chord.
    /// </summary>
    public GuitarChord? GetGuitarChord(Chord chord, FingeringOptions? options = null)
    {
        var voicing = GetBestVoicing(chord, options);
        return voicing?.ToGuitarChord();
    }

    /// <summary>
    /// Generates voicings for a chord (uses algorithmic generation).
    /// This replaces the old CalculateChordFingerings method.
    /// </summary>
    public List<GuitarChord> CalculateChordFingerings(Chord chord, int maxFret = 5)
    {
        var options = new FingeringOptions { MaxFret = maxFret };
        return GetVoicings(chord, options)
            .Select(v => v.ToGuitarChord())
            .ToList();
    }

    /// <summary>
    /// Gets all voicings for a chord within a specific fret range.
    /// </summary>
    public IEnumerable<GuitarVoicing> GetVoicingsInRange(Chord chord, int minFret, int maxFret)
    {
        return _fingeringGenerator.GenerateVoicingsInRange(chord, minFret, maxFret);
    }

    /// <summary>
    /// Gets the playability score for a voicing.
    /// </summary>
    public PlayabilityScore GetPlayabilityScore(GuitarVoicing voicing)
    {
        return _scorer.Score(voicing);
    }

    /// <summary>
    /// Gets the playability score for a GuitarChord.
    /// </summary>
    public PlayabilityScore GetPlayabilityScore(GuitarChord guitarChord)
    {
        var voicing = new GuitarVoicing(guitarChord.Chord, guitarChord.FretPositions, guitarChord.Fingerings);
        return _scorer.Score(voicing);
    }

    /// <summary>
    /// Clears the voicing cache (useful if options change).
    /// </summary>
    public void ClearCache()
    {
        _voicingCache.Clear();
    }

    private static string GetCacheKey(Chord chord, FingeringOptions? options)
    {
        var symbol = chord.GetSymbol();
        var optionsKey = options == null ? "default" :
            $"{options.MaxFret}_{options.AllowBarreChords}_{options.MaxDifficulty}_{options.RequireRootInBass}";
        return $"{symbol}_{optionsKey}";
    }
}