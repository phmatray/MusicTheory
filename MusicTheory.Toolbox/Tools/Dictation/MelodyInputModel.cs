namespace MusicTheory.Toolbox.Tools.Dictation;

/// <summary>The notes a student has entered so far for a melody of fixed length. Component state only.</summary>
public sealed class MelodyInputModel(int length)
{
    private readonly List<Note> _notes = [];

    public int Length { get; } = length;
    public bool IsComplete => _notes.Count >= Length;
    public IReadOnlyList<Note> Value => _notes;

    /// <summary>Appends a note; throws when the melody is already complete.</summary>
    public void Add(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        if (IsComplete) throw new InvalidOperationException("The melody is complete.");
        _notes.Add(note);
    }

    /// <summary>Removes the last note; nothing to remove is not an error.</summary>
    public void Undo()
    {
        if (_notes.Count > 0) _notes.RemoveAt(_notes.Count - 1);
    }

    /// <summary>The keyboard range of a question: a little below the first note, one and a half octaves wide.</summary>
    public static (int Low, int High) KeyboardRange(IReadOnlyList<Note> answer)
    {
        var low = answer[0].MidiNumber - 2;
        return (low, low + 18);
    }

    /// <summary>The note a key stands for, spelled for the question's key.</summary>
    public static Note Spell(int midi, KeySignature? key)
    {
        if (key is null) return Note.FromMidiNumber(midi);
        var n = key.Spell(midi % 12); // octave 4; moved to the key's own octave by whole octaves
        return new Note(n.Name, n.Alteration, n.Octave + (midi - n.MidiNumber) / 12);
    }
}
