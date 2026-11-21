namespace MusicTheory.Toolbox.Drills;

/// <summary>The degrees a student has tapped so far, capped at the progression length.</summary>
public sealed class DegreeSequenceModel(int length)
{
    private readonly List<int> _value = [];

    public IReadOnlyList<int> Value => _value;
    public bool IsFull => _value.Count >= length;

    /// <summary>Appends a degree; ignored once the sequence is full.</summary>
    public void Add(int degree)
    {
        if (!IsFull) _value.Add(degree);
    }

    public void Undo()
    {
        if (_value.Count > 0) _value.RemoveAt(_value.Count - 1);
    }
}
