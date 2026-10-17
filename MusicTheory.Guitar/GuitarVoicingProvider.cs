namespace MusicTheory.Guitar;

/// <summary>Supplies guitar voicings for a chord.</summary>
public interface IGuitarVoicingProvider
{
    /// <summary>Voicings for <paramref name="chord"/>, easiest first.</summary>
    IReadOnlyList<GuitarVoicing> GetVoicings(ChordSymbol chord, VoicingOptions? options = null);
}

/// <summary>
/// Voicings for every <see cref="ChordType"/>: curated open shapes, movable E/A/D-form shapes and a
/// fretboard search, merged, de-duplicated and ranked by <see cref="GuitarVoicing.Difficulty"/>. The best voicing of
/// each named shape is always kept, so the list spans the neck.
/// </summary>
/// <remarks>
/// Every voicing is checked the same way whatever its source: sounding pitch classes are chord tones (plus a
/// slash bass), the lowest note is the root or the slash bass, all required tones sound (root, 3rd or sus
/// tones, 7th, alterations and the top extension; the perfect 5th, and the 9th/11th of 11th/13th chords, may be
/// left out), at most one muted string sits between sounding strings, and a fingering with fingers 1–4 (index
/// barre allowed) exists. Curated and movable shapes are only used in standard tuning.
/// </remarks>
public sealed class GuitarVoicingProvider : IGuitarVoicingProvider
{
    // Common open / first-position shapes (standard tuning). Any that sound the requested chord are offered.
    private static readonly string[] Curated =
    [
        "x32010", "x02220", "320003", "320033", "022100", "xx0232", "x02210", "022000", "xx0231", "133211",
        "x02020", "x21202", "x32310", "xx0212", "020100", "320001", "x02120", "x32000", "xx0222", "xx3210",
        "x02010", "xx0211", "022030", "020000", "x02200", "x02230", "xx0230", "xx0233", "022200", "x32030",
        "2x0232", "022xxx", "x022xx", "xx023x", "x02212", "xx0201", "x00232", "x32003", "3x0003",
    ];

    // Movable shapes as fret offsets from the root's fret on the root string (null = muted).
    private static readonly (string Name, int RootString, int?[] Offsets)[] Movable =
    [
        ("E-form", 0, [0, 2, 2, 1, 0, 0]), ("E-form", 0, [0, 2, 2, 0, 0, 0]), ("E-form", 0, [0, 2, 0, 1, 0, 0]),
        ("E-form", 0, [0, 2, 1, 1, 0, 0]), ("E-form", 0, [0, 2, 0, 0, 0, 0]), ("E-form", 0, [0, 2, 2, 2, 0, 0]),
        ("E-form", 0, [0, 2, 0, 2, 0, 0]), ("E-form", 0, [0, null, 0, 0, -1, null]), ("E-form", 0, [0, null, -1, 0, -1, null]),
        ("E-form", 0, [0, 2, null, null, null, null]),
        ("A-form", 1, [null, 0, 2, 2, 2, 0]), ("A-form", 1, [null, 0, 2, 2, 1, 0]), ("A-form", 1, [null, 0, 2, 0, 2, 0]),
        ("A-form", 1, [null, 0, 2, 1, 2, 0]), ("A-form", 1, [null, 0, 2, 0, 1, 0]), ("A-form", 1, [null, 0, 2, 2, 3, 0]),
        ("A-form", 1, [null, 0, 2, 2, 0, 0]), ("A-form", 1, [null, 0, 2, 0, 3, 0]), ("A-form", 1, [null, 0, 1, 0, 1, null]),
        ("A-form", 1, [null, 0, 1, 2, 1, 2]), ("A-form", 1, [null, 0, 2, 2, 2, 2]), ("A-form", 1, [null, 0, 2, 2, 1, 2]),
        ("A-form", 1, [null, 0, -1, 0, 0, 0]), ("A-form", 1, [null, 0, -1, 0, 1, null]), ("A-form", 1, [null, 0, 2, null, null, null]),
        ("D-form", 2, [null, null, 0, 2, 3, 2]), ("D-form", 2, [null, null, 0, 2, 3, 1]), ("D-form", 2, [null, null, 0, 2, 1, 2]),
        ("D-form", 2, [null, null, 0, 2, 2, 2]), ("D-form", 2, [null, null, 0, 2, 1, 1]), ("D-form", 2, [null, null, 0, 2, 3, 3]),
        ("D-form", 2, [null, null, 0, 2, 3, 0]),
    ];

    /// <inheritdoc />
    public IReadOnlyList<GuitarVoicing> GetVoicings(ChordSymbol chord, VoicingOptions? options = null)
    {
        options ??= new VoicingOptions();
        var tuning = options.Tuning ?? Tuning.Standard;
        if (!options.IncludeSlashBass)
            chord = chord with { Bass = null };
        var spec = new Spec(chord, tuning, options);

        var found = new Dictionary<string, GuitarVoicing>();
        void Offer(int?[] frets, VoicingSource source, string? shape)
        {
            var voicing = spec.TryBuild(frets, source, shape);
            if (voicing is null)
                return;
            var key = voicing.ToTab();
            if (!found.TryGetValue(key, out var existing) || voicing.Difficulty < existing.Difficulty)
                found[key] = voicing;
        }

        if (tuning.Strings.Count == 6 && tuning.Strings.Select(n => n.MidiNumber).SequenceEqual(Tuning.Standard.Strings.Select(n => n.MidiNumber)))
        {
            foreach (var tab in Curated)
                Offer(tab.Select(c => c == 'x' ? (int?)null : c - '0').ToArray(), VoicingSource.Curated, "open");

            foreach (var (name, rootString, offsets) in Movable)
            {
                int first = Mod12(MusicTheoryUtilities.GetPitchClass(chord.Root) - tuning.Strings[rootString].MidiNumber);
                for (int fret = first; fret <= spec.MaxFret; fret += 12)
                    Offer(offsets.Select(o => o + fret).ToArray(), VoicingSource.MovableShape, name);
            }
        }

        foreach (var frets in spec.Search())
            Offer(frets, VoicingSource.Generated, null);

        var ranked = found.Values
            .OrderBy(v => v.Difficulty)
            .ThenBy(v => v.BaseFret)
            .ThenBy(v => v.ToTab(), StringComparer.Ordinal)
            .ToList();
        // Keep the best of each named shape (open, E/A/D-form) so next/previous moves across the neck,
        // then fill the remaining slots by rank.
        var picked = ranked.Where(v => v.ShapeName is not null).DistinctBy(v => v.ShapeName).Take(options.Max).ToHashSet();
        foreach (var v in ranked)
            if (picked.Count < options.Max)
                picked.Add(v);
        return ranked.Where(picked.Contains).ToList();
    }

    private static int Mod12(int n) => ((n % 12) + 12) % 12;

    /// <summary>The chord's constraints and the checks every candidate goes through.</summary>
    private sealed class Spec
    {
        private readonly ChordSymbol _chord;
        private readonly int[] _open;
        private readonly int _stretch;
        private readonly HashSet<int> _allowed;
        private readonly HashSet<int> _required;
        private readonly int _bass;
        public readonly int MaxFret;

        public Spec(ChordSymbol chord, Tuning tuning, VoicingOptions options)
        {
            _chord = chord;
            _open = tuning.Strings.Select(n => n.MidiNumber).ToArray();
            _stretch = options.MaxStretch;
            MaxFret = options.MaxFret - options.Capo;

            int root = MusicTheoryUtilities.GetPitchClass(chord.Root);
            var intervals = new Chord(chord.Root, chord.Type).GetNotes()
                .Select(n => Mod12(MusicTheoryUtilities.GetPitchClass(n) - root)).Distinct().ToList();
            _allowed = intervals.Select(i => (i + root) % 12).ToHashSet();
            _required = intervals.Where(i => !Optional(chord.Type, i)).Select(i => (i + root) % 12).ToHashSet();
            _bass = chord.Bass is null ? root : MusicTheoryUtilities.GetPitchClass(chord.Bass);
            _allowed.Add(_bass);
        }

        // The perfect 5th may go (not in a power chord); so may the 9th/11th of 11th/13th chords and the b5 of 7alt.
        private static bool Optional(ChordType type, int interval) => interval switch
        {
            7 => type != ChordType.Power5,
            2 => type is ChordType.Major11 or ChordType.Minor11 or ChordType.Dominant11
                or ChordType.Major13 or ChordType.Minor13 or ChordType.Dominant13,
            5 => type is ChordType.Major13 or ChordType.Minor13 or ChordType.Dominant13,
            6 => type == ChordType.Dominant7Alt,
            _ => false,
        };

        private int Pc(int s, int fret) => (_open[s] + fret) % 12;

        /// <summary>Depth-first search, one 4-fret window at a time, lowest string first; open strings only in windows up to fret 3.</summary>
        public IEnumerable<int?[]> Search()
        {
            var seen = new HashSet<string>();
            var frets = new int?[_open.Length];
            for (int window = 1; window + _stretch - 1 <= MaxFret; window++)
                foreach (var f in Walk(0, window, bassPlaced: false))
                    if (seen.Add(string.Join(",", f)))
                        yield return f;

            IEnumerable<int?[]> Walk(int s, int window, bool bassPlaced)
            {
                if (s == frets.Length)
                {
                    yield return (int?[])frets.Clone();
                    yield break;
                }
                var options = new List<int?> { null };
                // ponytail: open strings only next to low positions; a smarter reach model if mixed voicings matter.
                if (window <= 3 && (!bassPlaced ? Pc(s, 0) == _bass : _allowed.Contains(Pc(s, 0))))
                    options.Add(0);
                for (int f = window; f < window + _stretch && f <= MaxFret; f++)
                    if (!bassPlaced ? Pc(s, f) == _bass : _allowed.Contains(Pc(s, f)))
                        options.Add(f);
                foreach (var option in options)
                {
                    frets[s] = option;
                    foreach (var result in Walk(s + 1, window, bassPlaced || option is not null))
                        yield return result;
                }
                frets[s] = null;
            }
        }

        public GuitarVoicing? TryBuild(int?[] frets, VoicingSource source, string? shape)
        {
            if (frets.Length != _open.Length || frets.Any(f => f < 0 || f > MaxFret))
                return null;
            var sounding = Enumerable.Range(0, frets.Length).Where(s => frets[s] is not null).ToList();
            if (sounding.Count < 3)
                return null;

            var pcs = sounding.Select(s => Pc(s, frets[s]!.Value)).ToList();
            if (pcs[0] != _bass || !pcs.All(_allowed.Contains) || !_required.IsSubsetOf(pcs))
                return null;

            int innerMutes = sounding[^1] - sounding[0] + 1 - sounding.Count;
            if (innerMutes > 1)
                return null;

            var fretted = sounding.Where(s => frets[s] > 0).ToList();
            int low = fretted.Count == 0 ? 0 : fretted.Min(s => frets[s]!.Value);
            int high = fretted.Count == 0 ? 0 : fretted.Max(s => frets[s]!.Value);
            if (high - low >= _stretch)
                return null;

            var fingering = Fingering.Assign(frets);
            if (fingering is null)
                return null;
            var (fingers, barre) = fingering.Value;

            double difficulty = fingers.Where(f => f is not null).Distinct().Count()
                + (barre is null ? 0 : 1.5)
                + (high - low) * 0.5
                + Math.Max(low - 1, 0) * 0.3
                + innerMutes * 1.5
                + (frets.Length - sounding.Count) * 0.5
                + source switch { VoicingSource.Curated => -3, VoicingSource.MovableShape => -1, _ => 2 };

            return new GuitarVoicing(_chord, frets, fingers, barre, high <= 4 ? 1 : low, source, shape, Math.Round(difficulty, 2));
        }
    }
}

/// <summary>Assigns fingers 1–4 to a fret pattern, with an index barre when more than four notes are fretted.</summary>
internal static class Fingering
{
    /// <summary>
    /// Fretted notes in (fret, string) order get consecutive fingers, so a higher fret never gets a lower finger.
    /// With more than four fretted notes, finger 1 barres the lowest fret across the strings it covers; every
    /// string under the barre must be fretted at or above it. Returns null when four fingers are not enough.
    /// </summary>
    public static (int?[] Fingers, Barre? Barre)? Assign(IReadOnlyList<int?> frets)
    {
        var fingers = new int?[frets.Count];
        var notes = Enumerable.Range(0, frets.Count).Where(s => frets[s] > 0)
            .OrderBy(s => frets[s]).ThenBy(s => s).ToList();
        Barre? barre = null;
        int next = 1;

        if (notes.Count > 4)
        {
            int fret = frets[notes[0]]!.Value;
            var atFret = notes.Where(s => frets[s] == fret).ToList();
            int lo = atFret.Min(), hi = atFret.Max();
            if (atFret.Count < 2 || Enumerable.Range(lo, hi - lo + 1).Any(s => frets[s] is null || frets[s] < fret))
                return null;
            barre = new Barre(fret, lo, hi);
            foreach (var s in atFret)
                fingers[s] = 1;
            notes = notes.Except(atFret).ToList();
            next = 2;
        }

        if (next + notes.Count - 1 > 4)
            return null;
        foreach (var s in notes)
            fingers[s] = next++;
        return (fingers, barre);
    }
}
