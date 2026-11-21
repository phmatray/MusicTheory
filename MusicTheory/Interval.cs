using System.Diagnostics.CodeAnalysis;

namespace MusicTheory;

/// <summary>
/// The quality of an interval, ordered from narrowest to widest. Perfect applies to unisons, 4ths, 5ths and
/// octaves; minor and major to 2nds, 3rds, 6ths and 7ths; diminished and augmented to both, up to quintuple,
/// which is as far as two spelled notes with at most two accidentals each can reach (F𝄫 to B𝄪).
/// </summary>
public enum IntervalQuality
{
    /// <summary>Quintuply diminished (<c>ddddd</c>).</summary>
    QuintuplyDiminished,
    /// <summary>Quadruply diminished (<c>dddd</c>).</summary>
    QuadruplyDiminished,
    /// <summary>Triply diminished (<c>ddd</c>).</summary>
    TriplyDiminished,
    /// <summary>Doubly diminished (<c>dd</c>).</summary>
    DoublyDiminished,
    /// <summary>Diminished (<c>d</c>).</summary>
    Diminished,
    /// <summary>Minor (<c>m</c>).</summary>
    Minor,
    /// <summary>Perfect (<c>P</c>).</summary>
    Perfect,
    /// <summary>Major (<c>M</c>).</summary>
    Major,
    /// <summary>Augmented (<c>A</c>).</summary>
    Augmented,
    /// <summary>Doubly augmented (<c>AA</c>).</summary>
    DoublyAugmented,
    /// <summary>Triply augmented (<c>AAA</c>).</summary>
    TriplyAugmented,
    /// <summary>Quadruply augmented (<c>AAAA</c>).</summary>
    QuadruplyAugmented,
    /// <summary>Quintuply augmented (<c>AAAAA</c>).</summary>
    QuintuplyAugmented
}

/// <summary>
/// A spelled, directed interval: a quality, a number (1 = unison, 8 = octave, 9 and up are compound) and a
/// direction on the staff. A major 3rd and a diminished 4th both span 4 semitones but are different intervals.
/// </summary>
/// <remarks>
/// Names follow music21: <c>M3</c>, <c>P5</c>, <c>dd2</c>, <c>AAAA1</c>, and a descending interval writes its
/// direction before the number, <c>M-3</c>. A unison has no direction on the staff: <c>d1</c> lowers a note,
/// <c>A1</c> raises it.
/// </remarks>
public sealed record Interval
{
    /// <summary>The quality.</summary>
    public IntervalQuality Quality { get; }

    /// <summary>The number: 1 for a unison, 8 for an octave, 9 for a ninth (compound 2nd), and so on.</summary>
    public int Number { get; }

    /// <summary>Up or down the staff. Always <see cref="Direction.Up"/> for a unison.</summary>
    public Direction Direction { get; }

    /// <summary>
    /// Creates an interval.
    /// </summary>
    /// <param name="quality">The quality; perfect for 1, 4, 5 and their compounds, minor or major for the others.</param>
    /// <param name="number">The number, 1 or more.</param>
    /// <param name="direction">Up (default) or down; a unison is always up.</param>
    /// <exception cref="ArgumentOutOfRangeException">A number below 1, an undefined quality or direction, a
    /// quality that does not exist for that number (a perfect 3rd, a major 5th), or a descending unison.</exception>
    public Interval(IntervalQuality quality, int number, Direction direction = Direction.Up)
    {
        if (!Enum.IsDefined(quality))
            throw new ArgumentOutOfRangeException(nameof(quality), quality, "Undefined interval quality.");
        if (!Enum.IsDefined(direction))
            throw new ArgumentOutOfRangeException(nameof(direction), direction, "Undefined direction.");
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        bool perfect = IsPerfectNumber(number);
        if (perfect && quality is IntervalQuality.Minor or IntervalQuality.Major)
            throw new ArgumentOutOfRangeException(nameof(quality), quality, $"A {Ordinal(number)} is perfect, diminished or augmented, never {quality.ToString().ToLowerInvariant()}.");
        if (!perfect && quality == IntervalQuality.Perfect)
            throw new ArgumentOutOfRangeException(nameof(quality), quality, $"A {Ordinal(number)} is major, minor, diminished or augmented, never perfect.");
        if (number == 1 && direction == Direction.Down)
            throw new ArgumentOutOfRangeException(nameof(direction), direction, "A unison has no direction; use d1 to lower a note and A1 to raise it.");
        Quality = quality;
        Number = number;
        Direction = direction;
    }

    /// <summary>
    /// The size in semitones, ignoring direction: 4 for a major 3rd, 6 for both A4 and d5, 14 for a major 9th.
    /// Negative only for intervals narrower than nothing: a diminished unison is −1, a doubly diminished 2nd −1.
    /// </summary>
    public int Semitones
    {
        get
        {
            int simple = (Number - 1) % 7;
            int semitones = MusicTheoryConstants.SemitonesFromC[simple] + (Number - 1) / 7 * 12;
            int offset = (int)Quality - (int)IntervalQuality.Perfect;
            if (IsPerfectNumber(Number))
                return semitones + offset + (offset < 0 ? 1 : 0) - (offset > 0 ? 1 : 0);
            return semitones + offset - (offset > 0 ? 1 : 0);
        }
    }

    /// <summary>True for intervals wider than an octave (9ths and up).</summary>
    public bool IsCompound => Number > 8;

    /// <summary>True for any degree of augmented.</summary>
    public bool IsAugmented => Quality > IntervalQuality.Major;

    /// <summary>True for any degree of diminished.</summary>
    public bool IsDiminished => Quality < IntervalQuality.Minor;

    /// <summary>The name without direction: <c>M3</c>, <c>P8</c>, <c>dd2</c>.</summary>
    public string Name => Abbreviation(Quality) + Number;

    /// <summary>The directed name: <c>M3</c> up, <c>M-3</c> down.</summary>
    public override string ToString() =>
        Direction == Direction.Down ? $"{Abbreviation(Quality)}-{Number}" : Name;

    /// <summary>
    /// The interval from <paramref name="from"/> to <paramref name="to"/>. The number counts letters and the
    /// direction follows the staff, so C4→F##4 is a doubly augmented 4th (AA4), E4→C4 is a descending major
    /// 3rd (M-3) and C4→B#3 a descending diminished 2nd (d-2) of 0 semitones. Never throws for two notes.
    /// </summary>
    /// <exception cref="ArgumentNullException">A note is null.</exception>
    public static Interval Between(Note from, Note to)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        int steps = to.Step - from.Step;
        int semitones = to.Semitones - from.Semitones;
        var direction = Direction.Up;
        if (steps < 0)
        {
            (steps, semitones, direction) = (-steps, -semitones, Direction.Down);
        }
        int number = steps + 1;
        return new Interval(QualityOf(number, semitones), number, direction);
    }

    /// <summary>The same interval in the other direction; a unison mirrors instead (A1 ↔ d1, P1 stays).</summary>
    public Interval Reverse() => Number == 1
        ? new Interval(Mirror(Quality), 1)
        : new Interval(Quality, Number, Direction == Direction.Up ? Direction.Down : Direction.Up);

    /// <summary>
    /// The interval reduced to an octave or less, keeping quality and direction: M9 → M2, P15 → P8, P8 stays.
    /// </summary>
    public Interval ToSimple() => new(Quality, (Number - 2) % 7 + 2, Direction);

    /// <summary>
    /// The inversion, keeping direction: the numbers add up to 9 and the quality mirrors (M ↔ m, A ↔ d,
    /// AA ↔ dd, P stays). Compound intervals are reduced first (M10 inverts to m6).
    /// </summary>
    public Interval Invert()
    {
        int number = 9 - ToSimple().Number;
        return new Interval(Mirror(Quality), number, number == 1 ? Direction.Up : Direction);
    }

    /// <summary>True when both intervals move by the same number of semitones in the same direction (A4 and d5).</summary>
    public bool IsEnharmonicWith(Interval? other) => other is not null && DirectedSemitones == other.DirectedSemitones;

    /// <summary>
    /// Parses a music21-style name: quality letters (<c>P</c>, <c>M</c>, <c>m</c>, <c>d</c>…<c>ddddd</c>,
    /// <c>A</c>…<c>AAAAA</c>), an optional <c>-</c> for a descending interval, and the number: <c>M3</c>,
    /// <c>P-5</c>, <c>dd2</c>, <c>m9</c>.
    /// </summary>
    /// <exception cref="FormatException">The text is not an interval.</exception>
    public static Interval Parse(string text) =>
        TryParse(text, out var interval) ? interval : throw new FormatException($"Not an interval: '{text}'.");

    /// <summary>Tries to parse an interval; see <see cref="Parse"/>. Never throws.</summary>
    public static bool TryParse(string? text, [NotNullWhen(true)] out Interval? interval)
    {
        interval = null;
        var s = text.AsSpan().Trim();
        int letters = 0;
        while (letters < s.Length && s[letters] is 'P' or 'M' or 'm' or 'd' or 'A')
            letters++;
        var abbreviation = s[..letters].ToString();
        int quality = Array.FindIndex(Enum.GetValues<IntervalQuality>(), q => Abbreviation(q) == abbreviation);
        if (quality < 0)
            return false;

        var rest = s[letters..];
        var direction = Direction.Up;
        if (rest.StartsWith("-"))
        {
            direction = Direction.Down;
            rest = rest[1..];
        }
        if (!int.TryParse(rest, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out int number)
            || number < 1)
            return false;

        var q = (IntervalQuality)quality;
        bool perfect = IsPerfectNumber(number);
        if ((perfect && q is IntervalQuality.Minor or IntervalQuality.Major) || (!perfect && q == IntervalQuality.Perfect)
            || (number == 1 && direction == Direction.Down))
            return false;

        interval = new Interval(q, number, direction);
        return true;
    }

    /// <summary>Semitones with the direction's sign: 4 for M3, −4 for M-3, −1 for d1.</summary>
    public int DirectedSemitones => Direction == Direction.Down ? -Semitones : Semitones;

    // d, m, P, M, A with the d or A repeated for each further degree.
    private static string Abbreviation(IntervalQuality quality) => quality switch
    {
        IntervalQuality.Minor => "m",
        IntervalQuality.Perfect => "P",
        IntervalQuality.Major => "M",
        < IntervalQuality.Minor => new string('d', IntervalQuality.Minor - quality),
        _ => new string('A', quality - IntervalQuality.Major),
    };

    internal static bool IsPerfectNumber(int number) => ((number - 1) % 7) is 0 or 3 or 4;

    // The enum is symmetric around Perfect (and Minor/Major around it), so mirroring is one subtraction:
    // M <-> m, A <-> d, AA <-> dd, P stays.
    private static IntervalQuality Mirror(IntervalQuality quality) =>
        (IntervalQuality)(2 * (int)IntervalQuality.Perfect - (int)quality);

    /// <summary>The quality of a number spanning <paramref name="semitones"/>.</summary>
    private static IntervalQuality QualityOf(int number, int semitones)
    {
        int simple = (number - 1) % 7;
        int offset = semitones - MusicTheoryConstants.SemitonesFromC[simple] - (number - 1) / 7 * 12;
        int perfect = (int)IntervalQuality.Perfect;
        // Perfect numbers: 0 is P, each semitone away is one more degree of A or d.
        // Others: 0 is M and −1 is m; further out, A starts at +1 and d at −2.
        int quality = IsPerfectNumber(number)
            ? perfect + offset + Math.Sign(offset)
            : offset >= 0 ? perfect + 1 + offset : perfect + offset;
        return (IntervalQuality)quality;
    }

    private static string Ordinal(int number) => number switch
    {
        1 => "unison",
        8 => "octave",
        _ when number % 10 == 1 && number % 100 != 11 => $"{number}st",
        _ when number % 10 == 2 && number % 100 != 12 => $"{number}nd",
        _ when number % 10 == 3 && number % 100 != 13 => $"{number}rd",
        _ => $"{number}th",
    };
}
