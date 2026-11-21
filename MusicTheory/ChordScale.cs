namespace MusicTheory;

/// <summary>Chord-scale matching: which scales on a chord's root contain every chord tone.</summary>
public static class ChordScale
{
    /// <summary>Scales on the chord's root containing every chord tone (tensions included).</summary>
    /// <param name="chord">The chord symbol. A slash bass is ignored.</param>
    /// <returns>
    /// The matching scales, fewest avoid notes first (an avoid note is a scale tone a minor second above a
    /// chord tone), then in <see cref="ScaleType"/> declaration order; empty when none fit.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="chord"/> is null.</exception>
    public static IReadOnlyList<Scale> ScalesFor(ChordSymbol chord)
    {
        ArgumentNullException.ThrowIfNull(chord);
        var tones = chord.ToChord().GetNotes().Select(n => n.PitchClass).ToHashSet();
        var result = new List<Scale>();
        foreach (var type in Enum.GetValues<ScaleType>())
        {
            if (type == ScaleType.Chromatic)
                continue;
            var scale = new Scale(chord.Root, type);
            HashSet<int> pcs;
            try
            {
                pcs = scale.GetNotes().Select(n => n.PitchClass).ToHashSet();
            }
            catch (InvalidOperationException)
            {
                continue; // a degree needing more than two accidentals cannot be spelled
            }
            if (tones.IsSubsetOf(pcs))
                result.Add(scale);
        }
        return result.OrderBy(s => AvoidNotes(s, tones)).ThenBy(s => (int)s.Type).ToList();
    }

    // An avoid note is a scale tone a minor second above a chord tone.
    private static int AvoidNotes(Scale scale, HashSet<int> tones) =>
        scale.GetNotes().Select(n => n.PitchClass).Distinct()
            .Count(p => !tones.Contains(p) && tones.Contains((p + 11) % 12));
}
