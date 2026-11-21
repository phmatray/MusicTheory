namespace MusicTheory.Toolbox.I18n;

/// <summary>Note names for display: letters in English, fixed-do syllables in French.</summary>
public static class NoteNames
{
    /// <summary>The name without octave: <c>F#</c> or <c>Fa dièse</c>.</summary>
    public static string Format(Note note, Lang lang) =>
        lang == Lang.Fr ? Solfege.FixedDo(note) : note.GetName();
}
