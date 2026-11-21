using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MusicTheory;

/// <summary>
/// An unordered set of pitch classes (0 to 11), the object of pitch-class set theory. Duplicates and order are
/// ignored, and equality is set equality. <see cref="NormalForm"/>, <see cref="PrimeForm"/> and
/// <see cref="IntervalClassVector"/> are pinned against music21 for all 4,095 non-empty sets.
/// </summary>
public sealed record PitchClassSet
{
    private readonly ushort _mask;

    private PitchClassSet(int mask) => _mask = (ushort)(mask & 0xFFF);

    /// <summary>The empty set.</summary>
    public static readonly PitchClassSet Empty = new(0);

    /// <summary>All twelve pitch classes.</summary>
    public static readonly PitchClassSet Chromatic = new(0xFFF);

    /// <summary>Builds a set from pitch classes, each reduced modulo 12 (negatives allowed).</summary>
    public static PitchClassSet Of(params ReadOnlySpan<int> pitchClasses)
    {
        var mask = 0;
        foreach (var pc in pitchClasses)
            mask |= 1 << MusicTheoryUtilities.Mod12(pc);
        return new PitchClassSet(mask);
    }

    /// <summary>Builds a set from the <see cref="Note.PitchClass"/> of each note.</summary>
    public static PitchClassSet FromNotes(IEnumerable<Note> notes) => Of([.. notes.Select(n => n.PitchClass)]);

    /// <summary>The number of distinct pitch classes.</summary>
    public int Cardinality => BitOperations.PopCount(_mask);

    /// <summary>The pitch classes in ascending order, 0 to 11.</summary>
    public IReadOnlyList<int> PitchClasses => Members(_mask);

    /// <summary>Whether the set holds the pitch class, reduced modulo 12.</summary>
    public bool Contains(int pitchClass) => (_mask >> MusicTheoryUtilities.Mod12(pitchClass) & 1) == 1;

    /// <summary>Tn: moves every pitch class up by <paramref name="n"/> semitones, modulo 12.</summary>
    public PitchClassSet Transpose(int n)
    {
        var k = MusicTheoryUtilities.Mod12(n);
        return new PitchClassSet(_mask << k | _mask >> (12 - k));
    }

    /// <summary>TnI: maps every pitch class x to <paramref name="n"/> minus x, modulo 12.</summary>
    public PitchClassSet Invert(int n = 0) => new(InvertMask(_mask, n));

    /// <summary>The pitch classes this set lacks.</summary>
    public PitchClassSet Complement => new(~_mask);

    /// <summary>
    /// The most compact ascending ordering, as a list of pitch classes. Rotations are ranked by smallest span,
    /// then by the intervals from the first element outward (Forte's packing from the left), then by the lowest
    /// starting pitch class. Empty for <see cref="Empty"/>.
    /// </summary>
    public IReadOnlyList<int> NormalForm
    {
        get
        {
            var (start, zero) = Pack(Members(_mask));
            return [.. zero.Select(x => (x + start) % 12)];
        }
    }

    /// <summary>
    /// The normal form transposed to start on 0, or that of the inversion if it is smaller. Shared by every
    /// set of the same set class. Empty for <see cref="Empty"/>.
    /// </summary>
    public IReadOnlyList<int> PrimeForm =>
        _mask == 0 ? [] : Smaller(Pack(Members(_mask)).Zero, Pack(Members(InvertMask(_mask, 0))).Zero);

    /// <summary>How often each interval class (1 to 6) occurs between pairs of members, as six counts.</summary>
    public IReadOnlyList<int> IntervalClassVector
    {
        get
        {
            var pcs = Members(_mask);
            var vector = new int[6];
            for (var i = 0; i < pcs.Length; i++)
                for (var j = i + 1; j < pcs.Length; j++)
                    vector[Math.Min(pcs[j] - pcs[i], 12 - pcs[j] + pcs[i]) - 1]++;
            return vector;
        }
    }

    /// <summary>Whether <paramref name="other"/> is this set moved by some Tn.</summary>
    public bool IsTranspositionOf(PitchClassSet other) =>
        Pack(Members(_mask)).Zero.AsSpan().SequenceEqual(Pack(Members(other._mask)).Zero);

    /// <summary>Whether <paramref name="other"/> is this set moved by some Tn or TnI (the same prime form).</summary>
    public bool IsTranspositionOrInversionOf(PitchClassSet other) => PrimeForm.SequenceEqual(other.PrimeForm);

    /// <summary>
    /// Forte's name for the set class (TnI, no A/B suffix): <c>3-11</c> for major and minor triads, <c>4-Z15</c>,
    /// <c>0-1</c> for <see cref="Empty"/>, <c>12-1</c> for <see cref="Chromatic"/>. Generated from music21 in
    /// <c>ForteCatalog</c>.
    /// </summary>
    public string ForteName => ForteCatalog.NameOf(PrimeMask)!;

    /// <summary>Whether the set class shares its interval-class vector with another, not related by Tn or TnI.</summary>
    public bool IsZRelated => ForteName.Contains('Z');

    /// <summary>The prime form of the Z-partner set class (<c>4-Z15</c> and <c>4-Z29</c>), or null when not Z-related.</summary>
    public PitchClassSet? ZPartner => !IsZRelated ? null : ForteCatalog.All()
        .Select(row => new PitchClassSet(row.PrimeMask))
        .Single(p => p.Cardinality == Cardinality && p.PrimeMask != PrimeMask && p.IntervalClassVector.SequenceEqual(IntervalClassVector));

    /// <summary>
    /// The prime form of the named set class, for example <c>4-Z15</c> gives <c>{0,1,4,6}</c>. The <c>Z</c> may be
    /// lower case.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
    /// <exception cref="FormatException">The name is not in the catalogue; the message suggests a missing <c>Z</c>.</exception>
    public static PitchClassSet FromForteName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (TryFromForteName(name, out var set))
            return set;
        var dash = name.IndexOf('-');
        var hint = dash >= 0 && ForteCatalog.PrimeMaskOf(name.Insert(dash + 1, "Z")) is not null
            ? $" Did you mean \"{name.Insert(dash + 1, "Z")}\"?"
            : "";
        throw new FormatException($"\"{name}\" is not a Forte set class name.{hint}");
    }

    /// <summary>Like <see cref="FromForteName"/> but returns false instead of throwing.</summary>
    public static bool TryFromForteName(string? name, [NotNullWhen(true)] out PitchClassSet? set)
    {
        var mask = name is null ? null : ForteCatalog.PrimeMaskOf(name.ToUpperInvariant());
        set = mask is { } m ? new PitchClassSet(m) : null;
        return set is not null;
    }

    private ushort PrimeMask => (ushort)PrimeForm.Sum(pc => 1 << pc);

    /// <summary>The set as <c>{0,4,7}</c>.</summary>
    public override string ToString() => $"{{{string.Join(",", Members(_mask))}}}";

    private static int[] Members(int mask) => [.. Enumerable.Range(0, 12).Where(pc => (mask >> pc & 1) == 1)];

    private static int InvertMask(int mask, int n)
    {
        var inverted = 0;
        foreach (var pc in Members(mask))
            inverted |= 1 << MusicTheoryUtilities.Mod12(n - pc);
        return inverted;
    }

    // The rotation of the ascending pitch classes that packs most to the left, as offsets from its first
    // element, plus that element. The empty set packs to nothing.
    private static (int Start, int[] Zero) Pack(int[] pcs)
    {
        if (pcs.Length == 0)
            return (0, []);
        var best = Rotation(pcs, 0);
        var start = pcs[0];
        for (var i = 1; i < pcs.Length; i++)
        {
            var candidate = Rotation(pcs, i);
            if (Compare(candidate, best) < 0)
            {
                best = candidate;
                start = pcs[i];
            }
        }
        return (start, best);
    }

    private static int[] Rotation(int[] pcs, int from) =>
        [.. Enumerable.Range(0, pcs.Length).Select(j => MusicTheoryUtilities.Mod12(pcs[(from + j) % pcs.Length] - pcs[from]))];

    // Smallest span (the last offset) wins, then the first differing offset from the left.
    private static int Compare(int[] a, int[] b)
    {
        var bySpan = a[^1].CompareTo(b[^1]);
        return bySpan != 0 ? bySpan : a.AsSpan().SequenceCompareTo(b);
    }

    // Stryker disable once Equality: on a tie both arguments are equal arrays
    private static int[] Smaller(int[] a, int[] b) => Compare(a, b) <= 0 ? a : b;
}
