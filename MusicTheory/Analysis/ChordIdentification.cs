namespace MusicTheory.Analysis;

/// <summary>
/// Represents the result of identifying a chord from a collection of notes.
/// </summary>
public class ChordIdentification
{
    /// <summary>
    /// Gets the identified chord.
    /// </summary>
    public Chord IdentifiedChord { get; }

    /// <summary>
    /// Gets the inversion of the identified chord.
    /// </summary>
    public ChordInversion Inversion { get; }

    /// <summary>
    /// Gets the confidence level of the identification (0.0 to 1.0).
    /// </summary>
    public double Confidence { get; }

    /// <summary>
    /// Gets the notes that could not be matched to the identified chord.
    /// </summary>
    public IReadOnlyList<Note> UnmatchedNotes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChordIdentification"/> class.
    /// </summary>
    /// <param name="identifiedChord">The identified chord.</param>
    /// <param name="inversion">The inversion of the chord.</param>
    /// <param name="confidence">The confidence level (0.0 to 1.0).</param>
    /// <param name="unmatchedNotes">Optional list of notes that could not be matched.</param>
    public ChordIdentification(
        Chord identifiedChord,
        ChordInversion inversion,
        double confidence,
        IEnumerable<Note>? unmatchedNotes = null)
    {
        IdentifiedChord = identifiedChord;
        Inversion = inversion;
        Confidence = confidence;
        UnmatchedNotes = unmatchedNotes?.ToList().AsReadOnly() ?? new List<Note>().AsReadOnly();
    }
}
