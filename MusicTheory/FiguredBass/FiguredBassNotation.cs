namespace MusicTheory.FiguredBass;

/// <summary>
/// Represents a figured bass notation (e.g., "6", "6/4", "7", "6/5").
/// Figured bass is a Baroque-era notation system for indicating harmonies above a bass line.
/// </summary>
public class FiguredBassNotation
{
    private readonly Dictionary<int, Alteration> _alterations = new();
    private readonly List<int> _figures = new();

    /// <summary>
    /// Gets the chord inversion indicated by the figured bass.
    /// </summary>
    public ChordInversion Inversion { get; private set; }

    /// <summary>
    /// Gets whether this figured bass indicates a seventh chord.
    /// </summary>
    public bool IsSeventhChord { get; private set; }

    /// <summary>
    /// Gets the figures (intervals above the bass).
    /// </summary>
    public IReadOnlyList<int> Figures => _figures;

    /// <summary>
    /// Private constructor - use Parse factory method.
    /// </summary>
    private FiguredBassNotation()
    {
    }

    /// <summary>
    /// Parses a figured bass string into a FiguredBassNotation object.
    /// </summary>
    /// <param name="notation">The figured bass string (e.g., "6", "6/4", "7", "#6").</param>
    /// <returns>A FiguredBassNotation object.</returns>
    public static FiguredBassNotation Parse(string notation)
    {
        var result = new FiguredBassNotation();

        if (string.IsNullOrWhiteSpace(notation))
        {
            // Empty = root position triad (5/3)
            result._figures.AddRange([5, 3]);
            result.Inversion = ChordInversion.Root;
            result.IsSeventhChord = false;
            return result;
        }

        // Normalize notation
        notation = notation.Trim();

        // Parse alterations and figures
        var parts = notation.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var figureNumbers = new List<int>();

        foreach (var part in parts)
        {
            var (figure, alteration) = ParseFigurePart(part);
            figureNumbers.Add(figure);

            if (alteration.HasValue)
            {
                result._alterations[figure] = alteration.Value;
            }
        }

        result._figures.AddRange(figureNumbers);

        // Determine inversion and chord type based on figures
        DetermineChordType(result, figureNumbers);

        return result;
    }

    /// <summary>
    /// Checks if a specific figure has an alteration.
    /// </summary>
    /// <param name="figure">The figure number (e.g., 3, 5, 6, 7).</param>
    /// <returns>True if the figure has an alteration.</returns>
    public bool HasAlteration(int figure)
    {
        return _alterations.ContainsKey(figure);
    }

    /// <summary>
    /// Gets the alteration for a specific figure.
    /// </summary>
    /// <param name="figure">The figure number.</param>
    /// <returns>The alteration, or Natural if none specified.</returns>
    public Alteration GetAlteration(int figure)
    {
        return _alterations.GetValueOrDefault(figure, Alteration.Natural);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (_figures.Count == 0 || (_figures.SequenceEqual([5, 3]) && _alterations.Count == 0))
        {
            return "";
        }

        var parts = new List<string>();

        foreach (var figure in _figures)
        {
            var prefix = "";
            if (_alterations.TryGetValue(figure, out var alteration))
            {
                prefix = alteration switch
                {
                    Alteration.Sharp => "#",
                    Alteration.Flat => "b",
                    Alteration.Natural => "n",
                    _ => ""
                };
            }

            parts.Add(prefix + figure);
        }

        // Simplify common notations
        var result = string.Join("/", parts);

        // Simplify 6/3 to 6
        if (result == "6/3" && _alterations.Count == 0)
            return "6";

        // Simplify 7/5/3 to 7
        if (result == "7/5/3" && _alterations.Count == 0)
            return "7";

        return result;
    }

    private static (int figure, Alteration? alteration) ParseFigurePart(string part)
    {
        Alteration? alteration = null;
        var startIndex = 0;

        // Check for alteration prefix
        if (part.Length > 0)
        {
            switch (part[0])
            {
                case '#':
                    alteration = Alteration.Sharp;
                    startIndex = 1;
                    break;
                case 'b':
                    alteration = Alteration.Flat;
                    startIndex = 1;
                    break;
                case 'n':
                case 'N':
                    alteration = Alteration.Natural;
                    startIndex = 1;
                    break;
            }
        }

        // Parse the figure number
        var figureStr = part[startIndex..];
        if (int.TryParse(figureStr, out var figure))
        {
            return (figure, alteration);
        }

        // Default to 3 if parsing fails
        return (3, alteration);
    }

    private static void DetermineChordType(FiguredBassNotation result, List<int> figures)
    {
        // Single figures
        if (figures.Count == 1)
        {
            var fig = figures[0];

            switch (fig)
            {
                case 6:
                    // 6 = first inversion triad
                    result.Inversion = ChordInversion.First;
                    result.IsSeventhChord = false;
                    break;
                case 7:
                    // 7 = root position seventh
                    result.Inversion = ChordInversion.Root;
                    result.IsSeventhChord = true;
                    break;
                case 2:
                    // 2 = abbreviated 4/2 = third inversion seventh
                    result.Inversion = ChordInversion.Third;
                    result.IsSeventhChord = true;
                    break;
                default:
                    result.Inversion = ChordInversion.Root;
                    result.IsSeventhChord = false;
                    break;
            }

            return;
        }

        // Two figures
        if (figures.Count == 2)
        {
            var (top, bottom) = (figures[0], figures[1]);

            // 6/4 = second inversion triad
            if (top == 6 && bottom == 4)
            {
                result.Inversion = ChordInversion.Second;
                result.IsSeventhChord = false;
                return;
            }

            // 6/5 = first inversion seventh
            if (top == 6 && bottom == 5)
            {
                result.Inversion = ChordInversion.First;
                result.IsSeventhChord = true;
                return;
            }

            // 4/3 = second inversion seventh
            if (top == 4 && bottom == 3)
            {
                result.Inversion = ChordInversion.Second;
                result.IsSeventhChord = true;
                return;
            }

            // 4/2 = third inversion seventh
            if (top == 4 && bottom == 2)
            {
                result.Inversion = ChordInversion.Third;
                result.IsSeventhChord = true;
                return;
            }

            // 6/3 = first inversion triad (explicit)
            if (top == 6 && bottom == 3)
            {
                result.Inversion = ChordInversion.First;
                result.IsSeventhChord = false;
                return;
            }

            // 5/3 = root position triad (explicit)
            if (top == 5 && bottom == 3)
            {
                result.Inversion = ChordInversion.Root;
                result.IsSeventhChord = false;
                return;
            }
        }

        // Three figures - usually seventh chords
        if (figures.Count == 3)
        {
            // 7/5/3 = root position seventh
            if (figures[0] == 7)
            {
                result.Inversion = ChordInversion.Root;
                result.IsSeventhChord = true;
                return;
            }

            // 6/5/3 = first inversion seventh
            if (figures[0] == 6 && figures[1] == 5)
            {
                result.Inversion = ChordInversion.First;
                result.IsSeventhChord = true;
                return;
            }

            // 6/4/3 = second inversion seventh
            if (figures[0] == 6 && figures[1] == 4)
            {
                result.Inversion = ChordInversion.Second;
                result.IsSeventhChord = true;
                return;
            }

            // 6/4/2 = third inversion seventh
            if (figures[0] == 6 && figures[1] == 4 && figures[2] == 2)
            {
                result.Inversion = ChordInversion.Third;
                result.IsSeventhChord = true;
                return;
            }
        }

        // Default
        result.Inversion = ChordInversion.Root;
        result.IsSeventhChord = figures.Contains(7);
    }
}
