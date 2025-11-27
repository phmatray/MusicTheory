namespace MusicTheory.Analysis;

/// <summary>
/// Represents a set of pitch classes (0-11) for pitch class set theory analysis.
/// </summary>
public class PitchClassSet : IEquatable<PitchClassSet>
{
    private readonly SortedSet<int> _pitchClasses;

    /// <summary>
    /// Gets the pitch classes in the set (0-11).
    /// </summary>
    public IReadOnlyList<int> PitchClasses => _pitchClasses.ToList();

    /// <summary>
    /// Gets the cardinality (number of unique pitch classes) of the set.
    /// </summary>
    public int Cardinality => _pitchClasses.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="PitchClassSet"/> class.
    /// </summary>
    private PitchClassSet(IEnumerable<int> pitchClasses)
    {
        _pitchClasses = new SortedSet<int>(pitchClasses.Select(pc => ((pc % 12) + 12) % 12));
    }

    /// <summary>
    /// Creates a pitch class set from a collection of notes.
    /// </summary>
    /// <param name="notes">The notes to convert to pitch classes.</param>
    /// <returns>A new pitch class set.</returns>
    public static PitchClassSet FromNotes(IEnumerable<Note> notes)
    {
        var pitchClasses = notes.Select(GetPitchClass);
        return new PitchClassSet(pitchClasses);
    }

    /// <summary>
    /// Creates a pitch class set from interval values (semitones from 0).
    /// </summary>
    /// <param name="intervals">The intervals (pitch class integers 0-11).</param>
    /// <returns>A new pitch class set.</returns>
    public static PitchClassSet FromIntervals(IEnumerable<int> intervals)
    {
        return new PitchClassSet(intervals);
    }

    /// <summary>
    /// Transposes the pitch class set by a given number of semitones.
    /// </summary>
    /// <param name="semitones">The number of semitones to transpose.</param>
    /// <returns>A new transposed pitch class set.</returns>
    public PitchClassSet Transpose(int semitones)
    {
        var transposed = _pitchClasses.Select(pc => (pc + semitones) % 12);
        return new PitchClassSet(transposed);
    }

    /// <summary>
    /// Inverts the pitch class set around pitch class 0.
    /// </summary>
    /// <returns>A new inverted pitch class set.</returns>
    public PitchClassSet Invert()
    {
        var inverted = _pitchClasses.Select(pc => (12 - pc) % 12);
        return new PitchClassSet(inverted);
    }

    /// <summary>
    /// Gets the normal form of the pitch class set (most compact arrangement).
    /// </summary>
    /// <returns>The pitch class set in normal form.</returns>
    public PitchClassSet GetNormalForm()
    {
        if (_pitchClasses.Count <= 1)
            return this;

        var orderedPCs = _pitchClasses.ToList();
        var rotations = new List<List<int>>();

        // Generate all rotations
        for (int i = 0; i < orderedPCs.Count; i++)
        {
            var rotation = new List<int>();
            for (int j = 0; j < orderedPCs.Count; j++)
            {
                var pc = orderedPCs[(i + j) % orderedPCs.Count];
                // Normalize so first element is 0
                rotation.Add(((pc - orderedPCs[i]) % 12 + 12) % 12);
            }
            rotations.Add(rotation);
        }

        // Find the most compact form (smallest span, then lexicographically smallest)
        var normalForm = rotations
            .OrderBy(r => r.Last())  // Smallest span
            .ThenBy(r => r, Comparer<List<int>>.Create((a, b) =>
            {
                for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
                {
                    var cmp = a[i].CompareTo(b[i]);
                    if (cmp != 0) return cmp;
                }
                return a.Count.CompareTo(b.Count);
            }))
            .First();

        return new PitchClassSet(normalForm);
    }

    /// <summary>
    /// Gets the prime form of the pitch class set (normal form of the most compact transposition/inversion).
    /// </summary>
    /// <returns>The pitch class set in prime form.</returns>
    public PitchClassSet GetPrimeForm()
    {
        var normalForm = GetNormalForm();
        var invertedNormalForm = Invert().GetNormalForm();

        // Compare and return the lexicographically smaller one
        var normalPCs = normalForm.PitchClasses.ToList();
        var invertedPCs = invertedNormalForm.PitchClasses.ToList();

        for (int i = 0; i < Math.Min(normalPCs.Count, invertedPCs.Count); i++)
        {
            if (normalPCs[i] < invertedPCs[i])
                return normalForm;
            if (invertedPCs[i] < normalPCs[i])
                return invertedNormalForm;
        }

        return normalForm;
    }

    /// <summary>
    /// Returns the union of this set with another set.
    /// </summary>
    /// <param name="other">The other pitch class set.</param>
    /// <returns>A new set containing all pitch classes from both sets.</returns>
    public PitchClassSet Union(PitchClassSet other)
    {
        return new PitchClassSet(_pitchClasses.Union(other._pitchClasses));
    }

    /// <summary>
    /// Returns the intersection of this set with another set.
    /// </summary>
    /// <param name="other">The other pitch class set.</param>
    /// <returns>A new set containing only common pitch classes.</returns>
    public PitchClassSet Intersection(PitchClassSet other)
    {
        return new PitchClassSet(_pitchClasses.Intersect(other._pitchClasses));
    }

    /// <summary>
    /// Determines if this set is a subset of another set.
    /// </summary>
    /// <param name="other">The other pitch class set.</param>
    /// <returns>True if all pitch classes in this set are in the other set.</returns>
    public bool IsSubsetOf(PitchClassSet other)
    {
        return _pitchClasses.IsSubsetOf(other._pitchClasses);
    }

    /// <inheritdoc />
    public bool Equals(PitchClassSet? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _pitchClasses.SetEquals(other._pitchClasses);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as PitchClassSet);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var pc in _pitchClasses)
        {
            hash.Add(pc);
        }
        return hash.ToHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{string.Join(", ", _pitchClasses)}]";
    }

    private static int GetPitchClass(Note note)
    {
        var semitones = MusicTheoryConstants.SemitonesFromC[(int)note.Name] + (int)note.Alteration;
        return ((semitones % 12) + 12) % 12;
    }
}
