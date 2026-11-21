using System.Globalization;

namespace MusicTheory;

/// <summary>A note plus a deviation in cents from its 12-TET pitch: a tuner reading or a microtonal pitch.</summary>
public sealed record TunedPitch
{
    /// <summary>Creates a pitch <paramref name="cents"/> above (or below, when negative) <paramref name="note"/>.</summary>
    /// <param name="note">The nearest 12-TET note.</param>
    /// <param name="cents">The deviation in cents, strictly between -100 and 100.</param>
    /// <exception cref="ArgumentNullException"><paramref name="note"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="cents"/> is not finite or not in (-100, 100).</exception>
    public TunedPitch(Note note, double cents)
    {
        ArgumentNullException.ThrowIfNull(note);
        if (!double.IsFinite(cents) || cents <= -100 || cents >= 100)
            throw new ArgumentOutOfRangeException(nameof(cents), cents, "Cents must be finite and strictly between -100 and 100.");
        Note = note;
        Cents = cents;
    }

    /// <summary>The nearest 12-TET note.</summary>
    public Note Note { get; }

    /// <summary>The deviation from <see cref="Note"/> in cents.</summary>
    public double Cents { get; }

    /// <summary>The frequency in Hz, tuned to <paramref name="a4"/>.</summary>
    /// <param name="a4">The frequency of A4 in Hz.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="a4"/> is not a positive, finite number.</exception>
    public double GetFrequency(double a4 = Note.StandardA4) => Note.GetFrequency(a4) * Math.Pow(2, Cents / 1200);

    /// <summary>The distance in cents from <paramref name="fromHz"/> to <paramref name="toHz"/>: 1200 per octave.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Either frequency is not a positive, finite number.</exception>
    public static double CentsBetween(double fromHz, double toHz)
    {
        if (!double.IsFinite(fromHz) || fromHz <= 0)
            throw new ArgumentOutOfRangeException(nameof(fromHz), fromHz, "Frequency must be positive and finite.");
        if (!double.IsFinite(toHz) || toHz <= 0)
            throw new ArgumentOutOfRangeException(nameof(toHz), toHz, "Frequency must be positive and finite.");
        return 1200 * Math.Log2(toHz / fromHz);
    }

    /// <summary>The note and its signed offset to one decimal, such as <c>A4 +23.4¢</c>.</summary>
    // Rounding first, then adding +0.0, turns a tiny negative offset into +0.0 rather than "-0.0".
    public override string ToString() => $"{Note} {(Math.Round(Cents, 1) + 0.0).ToString("+0.0;-0.0", CultureInfo.InvariantCulture)}¢";
}
