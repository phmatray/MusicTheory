namespace MusicTheory.Guitar;

/// <summary>Diagram options. Spacings are in your drawing units.</summary>
/// <param name="Frets">Frets shown in the window.</param>
/// <param name="StringSpacing">Distance between strings.</param>
/// <param name="FretSpacing">Distance between frets.</param>
/// <param name="LeftHanded">Mirror the strings (lowest string on the right).</param>
public sealed record DiagramOptions(int Frets = 4, double StringSpacing = 1, double FretSpacing = 1, bool LeftHanded = false);

/// <summary>A line segment.</summary>
public sealed record Line(double X1, double Y1, double X2, double Y2);

/// <summary>A point.</summary>
public sealed record Point(double X, double Y);

/// <summary>Open or muted marker kind.</summary>
public enum MarkerKind
{
    /// <summary>String not played (×).</summary>
    Muted,
    /// <summary>Open string (○).</summary>
    Open
}

/// <summary>An open/muted marker above the nut.</summary>
public sealed record StringMarker(int String, MarkerKind Kind, double X, double Y);

/// <summary>A fretted note.</summary>
/// <param name="String">String index, 0 = lowest.</param>
/// <param name="Fret">Fret (relative to the capo).</param>
/// <param name="X">Centre x.</param>
/// <param name="Y">Centre y (middle of the fret space).</param>
/// <param name="Finger">Finger 1–4, if known.</param>
/// <param name="NoteName">Sounding note name, e.g. <c>F#</c>.</param>
public sealed record FingerDot(int String, int Fret, double X, double Y, int? Finger, string NoteName);

/// <summary>A barre bar from <see cref="X1"/> to <see cref="X2"/> at height <see cref="Y"/>.</summary>
public sealed record BarreShape(double X1, double X2, double Y, int Fret);

/// <summary>
/// Framework-agnostic geometry for a chord diagram: strings are vertical lines, frets horizontal; (0, 0) is the
/// top-left of the grid, the nut (or top fret line) is at y = 0, and markers sit half a fret above it.
/// </summary>
public sealed record ChordDiagramLayout(
    double Width, double Height,
    IReadOnlyList<Line> Strings, IReadOnlyList<Line> FretLines,
    bool ShowNut, int? PositionLabel,
    IReadOnlyList<StringMarker> Markers,
    IReadOnlyList<FingerDot> Dots,
    BarreShape? Barre,
    IReadOnlyList<Point> InlayDots)
{
    private static readonly int[] Inlays = [3, 5, 7, 9, 12, 15];

    /// <summary>Lays out <paramref name="voicing"/> in a window starting at its <see cref="GuitarVoicing.BaseFret"/>.</summary>
    public static ChordDiagramLayout For(GuitarVoicing voicing, DiagramOptions? options = null, Tuning? tuning = null)
    {
        options ??= new DiagramOptions();
        int strings = voicing.Frets.Count;
        double width = (strings - 1) * options.StringSpacing;
        double height = options.Frets * options.FretSpacing;
        double X(int s) => (options.LeftHanded ? strings - 1 - s : s) * options.StringSpacing;
        double Y(int fret) => (fret - voicing.BaseFret + 0.5) * options.FretSpacing;

        var sound = voicing.Sound(tuning ?? Tuning.Standard);
        var barre = voicing.Barre;
        var markers = new List<StringMarker>();
        var dots = new List<FingerDot>();
        for (int s = 0; s < strings; s++)
        {
            var fret = voicing.Frets[s];
            if (fret is null or 0)
                markers.Add(new StringMarker(s, fret is null ? MarkerKind.Muted : MarkerKind.Open, X(s), -options.FretSpacing / 2));
            else if (barre is null || fret != barre.Fret || s < barre.LowString || s > barre.HighString)
                dots.Add(new FingerDot(s, fret.Value, X(s), Y(fret.Value), voicing.Fingers[s], sound[s]!.GetName()));
        }

        return new ChordDiagramLayout(
            width, height,
            Enumerable.Range(0, strings).Select(s => new Line(X(s), 0, X(s), height)).ToList(),
            Enumerable.Range(0, options.Frets + 1).Select(i => new Line(0, i * options.FretSpacing, width, i * options.FretSpacing)).ToList(),
            voicing.BaseFret == 1,
            voicing.BaseFret == 1 ? null : voicing.BaseFret,
            markers,
            dots,
            barre is null ? null : new BarreShape(Math.Min(X(barre.LowString), X(barre.HighString)),
                                                  Math.Max(X(barre.LowString), X(barre.HighString)), Y(barre.Fret), barre.Fret),
            Inlays.Where(f => f >= voicing.BaseFret && f < voicing.BaseFret + options.Frets)
                  .SelectMany(f => f % 12 == 0
                      ? new[] { new Point(width / 4, Y(f)), new Point(width * 3 / 4, Y(f)) }
                      : new[] { new Point(width / 2, Y(f)) })
                  .ToList());
    }
}
