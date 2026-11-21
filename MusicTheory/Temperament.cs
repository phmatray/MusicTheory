namespace MusicTheory;

/// <summary>
/// A tuning system: per-degree offsets (in cents) from 12-TET, applied relative to a tonic.
/// </summary>
/// <remarks>
/// The model is a 12-key keyboard instrument, so enharmonic notes share a frequency. In quarter-comma
/// meantone the wolf fifth falls between G# and Eb (relative to the tonic).
/// </remarks>
public sealed class Temperament
{
    private readonly double[] _offsets;

    private Temperament(string name, double[] offsets)
    {
        Name = name;
        _offsets = offsets;
    }

    /// <summary>The temperament's display name.</summary>
    public string Name { get; }

    /// <summary>Twelve-tone equal temperament: every offset is zero.</summary>
    public static Temperament Equal => new("Equal", new double[12]);

    /// <summary>Pythagorean tuning: a chain of pure fifths.</summary>
    public static Temperament Pythagorean => FromRatios(
        "Pythagorean",
        1, 256.0 / 243, 9.0 / 8, 32.0 / 27, 81.0 / 64, 4.0 / 3, 729.0 / 512, 3.0 / 2, 128.0 / 81, 27.0 / 16, 16.0 / 9, 243.0 / 128);

    /// <summary>5-limit just intonation (9/5 minor seventh, 45/32 tritone).</summary>
    public static Temperament JustIntonation => FromRatios(
        "Just intonation",
        1, 16.0 / 15, 9.0 / 8, 6.0 / 5, 5.0 / 4, 4.0 / 3, 45.0 / 32, 3.0 / 2, 8.0 / 5, 5.0 / 3, 9.0 / 5, 15.0 / 8);

    /// <summary>Quarter-comma meantone: pure major thirds, fifths narrowed by a quarter syntonic comma.</summary>
    public static Temperament QuarterCommaMeantone => MeantoneTable();

    /// <summary>Offset in cents from 12-TET of the degree <paramref name="semitonesAboveTonic"/> (0..11).</summary>
    /// <exception cref="ArgumentOutOfRangeException">The degree is outside 0..11.</exception>
    public double OffsetCents(int semitonesAboveTonic)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(semitonesAboveTonic);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(semitonesAboveTonic, 11);
        return _offsets[semitonesAboveTonic];
    }

    /// <summary>The frequency of <paramref name="note"/> in this temperament, keeping A sounding at <paramref name="a4"/>.</summary>
    /// <param name="note">The note to sound; only its pitch class and octave matter.</param>
    /// <param name="tonic">The key's tonic; its octave is ignored.</param>
    /// <param name="a4">The frequency of A4 in hertz.</param>
    /// <exception cref="ArgumentNullException"><paramref name="note"/> or <paramref name="tonic"/> is null.</exception>
    public double GetFrequency(Note note, Note tonic, double a4 = Note.StandardA4)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(tonic);
        var cents = _offsets[Degree(note.PitchClass, tonic)] - _offsets[Degree(9, tonic)];
        return note.GetFrequency(a4) * Math.Pow(2, cents / 1200);
    }

    /// <inheritdoc />
    public override string ToString() => Name;

    private static int Degree(int pitchClass, Note tonic) => ((pitchClass - tonic.PitchClass) % 12 + 12) % 12;

    private static Temperament FromRatios(string name, params double[] ratios)
    {
        var offsets = new double[12];
        for (var d = 0; d < 12; d++)
        {
            offsets[d] = 1200 * Math.Log2(ratios[d]) - 100 * d;
        }

        return new Temperament(name, offsets);
    }

    private static Temperament MeantoneTable()
    {
        // Fifths from the tonic that reach each degree 0..11.
        int[] fifths = [0, 7, 2, -3, 4, -1, 6, 1, 8, 3, -2, 5];
        var q = 1200 * Math.Log2(5) / 4;
        var offsets = new double[12];
        for (var d = 0; d < 12; d++)
        {
            offsets[d] = Mod(fifths[d] * q) - Mod(fifths[d] * 700.0);
        }

        return new Temperament("Quarter-comma meantone", offsets);
    }

    private static double Mod(double cents) => ((cents % 1200) + 1200) % 1200;
}
