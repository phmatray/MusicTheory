namespace MusicTheory;

/// <summary>A piano key in white-key units: white keys are 1 wide, black keys 0.6 and sit on the boundary.</summary>
/// <param name="Midi">MIDI note number (C4 = 60).</param>
/// <param name="IsBlack">True for a black key.</param>
/// <param name="X">Left edge; the first white key of the range starts at 0.</param>
/// <param name="Width">1 for white keys, 0.6 for black keys.</param>
public sealed record PianoKey(int Midi, bool IsBlack, double X, double Width);

/// <summary>Keyboard geometry for drawing a piano view.</summary>
public static class PianoLayout
{
    private const double BlackWidth = 0.6;

    /// <summary>The keys from <paramref name="lowMidi"/> to <paramref name="highMidi"/> inclusive.</summary>
    public static IReadOnlyList<PianoKey> Keys(int lowMidi, int highMidi)
    {
        var keys = new List<PianoKey>();
        int whites = 0;
        for (int midi = lowMidi; midi <= highMidi; midi++)
        {
            bool black = (midi % 12) is 1 or 3 or 6 or 8 or 10;
            keys.Add(black
                ? new PianoKey(midi, true, whites - BlackWidth / 2, BlackWidth)
                : new PianoKey(midi, false, whites++, 1));
        }
        return keys;
    }
}
