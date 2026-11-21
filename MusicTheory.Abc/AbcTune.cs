using System.Diagnostics.CodeAnalysis;

namespace MusicTheory.Abc;

/// <summary>A single-voice ABC tune: header fields and measures.</summary>
/// <param name="ReferenceNumber">The <c>X:</c> reference number.</param>
/// <param name="Title">The first <c>T:</c> title, or null.</param>
/// <param name="TimeSignature">The <c>M:</c> meter, 4/4 by default (<c>C</c> is 4/4, <c>C|</c> is 2/2).</param>
/// <param name="UnitLength">The <c>L:</c> default note length as a fraction of a whole note.</param>
/// <param name="Key">The <c>K:</c> key. A modal key (<c>K:Edor</c>) is stored as its signature-equivalent
/// major key (D major).</param>
/// <param name="Tempo">The <c>Q:</c> tempo, or null.</param>
/// <param name="Measures">The bars. A first bar shorter than the meter is a pickup measure.</param>
public sealed record AbcTune(int ReferenceNumber, string? Title, TimeSignature TimeSignature,
    Rational UnitLength, KeySignature Key, TempoMap? Tempo, IReadOnlyList<Measure> Measures)
{
    /// <summary>Parses the first tune of <paramref name="text"/>.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
    /// <exception cref="FormatException">A missing <c>X:</c> or <c>K:</c>, or text that is not ABC.</exception>
    /// <exception cref="NotSupportedException">A <c>V:</c> voice or a <c>w:</c> lyric line.</exception>
    public static AbcTune Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        try
        {
            return AbcParser.Parse(text);
        }
        catch (ArgumentException e)
        {
            throw new FormatException(e.Message, e);
        }
    }

    /// <summary>Tries to parse a tune; see <see cref="Parse"/>. Never throws.</summary>
    public static bool TryParse(string? text, [NotNullWhen(true)] out AbcTune? tune)
    {
        tune = null;
        if (text is null)
            return false;
        try
        {
            tune = Parse(text);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }
}
