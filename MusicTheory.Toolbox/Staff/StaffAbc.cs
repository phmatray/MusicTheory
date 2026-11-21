using MusicTheory.Abc;

namespace MusicTheory.Toolbox.Rendering;

/// <summary>Builds the ABC text that the browser draws on a staff.</summary>
public static class StaffAbc
{
    /// <summary>One voice of quarter notes in 4/4 under <paramref name="key"/>; <see cref="AbcTune.Parse"/> reads it back.</summary>
    public static string FromNotes(KeySignature key, IReadOnlyList<Note> notes)
    {
        var time = new TimeSignature(4, 4);
        var tune = new AbcTune(1, null, time, Rational.Of(1, 4), key, null, MelodyMeasures.FromNotes(notes));
        return tune.ToString();
    }
}
