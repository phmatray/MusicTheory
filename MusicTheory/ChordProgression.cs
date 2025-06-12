namespace MusicTheory;

/// <summary>
/// Represents a chord progression in a specific key.
/// </summary>
public class ChordProgression
{
    /// <summary>
    /// Gets the key signature of the progression.
    /// </summary>
    public KeySignature Key { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChordProgression"/> class.
    /// </summary>
    /// <param name="key">The key signature for the progression.</param>
    public ChordProgression(KeySignature key)
    {
        Key = key;
    }

    /// <summary>
    /// Gets the diatonic chords for the key.
    /// </summary>
    /// <returns>An enumerable of the seven diatonic chords.</returns>
    public IEnumerable<Chord> GetDiatonicChords()
    {
        var scale = new Scale(Key.Tonic, Key.Mode == KeyMode.Major ? ScaleType.Major : ScaleType.NaturalMinor);
        var scaleNotes = scale.GetNotes().Take(7).ToList();

        // Chord qualities for major scale: I ii iii IV V vi vii°
        var majorQualities = new[] 
        { 
            ChordQuality.Major, ChordQuality.Minor, ChordQuality.Minor, 
            ChordQuality.Major, ChordQuality.Major, ChordQuality.Minor, 
            ChordQuality.Diminished 
        };

        // Chord qualities for natural minor scale: i ii° III iv v VI VII
        var minorQualities = new[] 
        { 
            ChordQuality.Minor, ChordQuality.Diminished, ChordQuality.Major, 
            ChordQuality.Minor, ChordQuality.Minor, ChordQuality.Major, 
            ChordQuality.Major 
        };

        var qualities = Key.Mode == KeyMode.Major ? majorQualities : minorQualities;

        for (int i = 0; i < 7; i++)
        {
            yield return new Chord(scaleNotes[i], qualities[i]);
        }
    }

    /// <summary>
    /// Gets the Roman numeral representation for a scale degree.
    /// </summary>
    /// <param name="degree">The scale degree (1-7).</param>
    /// <returns>The Roman numeral representation.</returns>
    public string GetRomanNumeral(int degree)
    {
        if (degree < 1 || degree > 7)
            throw new ArgumentOutOfRangeException(nameof(degree), "Degree must be between 1 and 7.");

        var romanNumerals = new[] { "I", "II", "III", "IV", "V", "VI", "VII" };
        var diatonicChords = GetDiatonicChords().ToList();
        var chord = diatonicChords[degree - 1];

        var numeral = romanNumerals[degree - 1];
        
        // Lowercase for minor chords
        if (chord.Quality == ChordQuality.Minor)
        {
            numeral = numeral.ToLower();
        }
        
        // Add ° for diminished
        if (chord.Quality == ChordQuality.Diminished)
        {
            numeral = numeral.ToLower() + "°";
        }

        return numeral;
    }

    /// <summary>
    /// Gets a chord by its scale degree.
    /// </summary>
    /// <param name="degree">The scale degree (1-7).</param>
    /// <returns>The chord at that degree.</returns>
    public Chord GetChordByDegree(int degree)
    {
        if (degree < 1 || degree > 7)
            throw new ArgumentOutOfRangeException(nameof(degree), "Degree must be between 1 and 7.");

        var diatonicChords = GetDiatonicChords().ToList();
        return diatonicChords[degree - 1];
    }

    /// <summary>
    /// Parses a chord progression string and returns the chords.
    /// </summary>
    /// <param name="progressionString">The progression string (e.g., "I - IV - V - I").</param>
    /// <returns>An enumerable of chords.</returns>
    public IEnumerable<Chord> ParseProgression(string progressionString)
    {
        var parts = progressionString.Split(new[] { '-', ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var part in parts)
        {
            yield return ParseChordSymbol(part);
        }
    }

    /// <summary>
    /// Parses a single chord symbol.
    /// </summary>
    private Chord ParseChordSymbol(string symbol)
    {
        // Remove whitespace
        symbol = symbol.Trim();

        // Check for seventh chord indicators
        bool isSeventh = symbol.Contains("7");
        bool isMajor7 = symbol.Contains("Maj7");
        
        // Extract the basic Roman numeral
        string baseSymbol = symbol.Replace("Maj7", "").Replace("7", "").Trim();
        
        // Determine the degree
        int degree = GetDegreeFromRomanNumeral(baseSymbol);
        
        // Get the basic chord
        var chord = GetChordByDegree(degree);
        
        // Add seventh if needed
        if (isSeventh)
        {
            if (isMajor7)
            {
                chord.AddExtension(7, IntervalQuality.Major);
            }
            else
            {
                // For dominant 7th on V, use minor 7th; for others, use appropriate 7th
                if (degree == 5 && Key.Mode == KeyMode.Major)
                {
                    chord.AddExtension(7, IntervalQuality.Minor);
                }
                else if (chord.Quality == ChordQuality.Major)
                {
                    chord.AddExtension(7, IntervalQuality.Major);
                }
                else if (chord.Quality == ChordQuality.Minor)
                {
                    chord.AddExtension(7, IntervalQuality.Minor);
                }
                else if (chord.Quality == ChordQuality.Diminished)
                {
                    chord.AddExtension(7, IntervalQuality.Diminished);
                }
            }
        }
        
        return chord;
    }

    /// <summary>
    /// Gets the scale degree from a Roman numeral.
    /// </summary>
    private int GetDegreeFromRomanNumeral(string romanNumeral)
    {
        var upperNumeral = romanNumeral.ToUpper().Replace("°", "");
        
        return upperNumeral switch
        {
            "I" => 1,
            "II" => 2,
            "III" => 3,
            "IV" => 4,
            "V" => 5,
            "VI" => 6,
            "VII" => 7,
            _ => throw new ArgumentException($"Invalid Roman numeral: {romanNumeral}")
        };
    }

    /// <summary>
    /// Gets the secondary dominant of a given degree.
    /// </summary>
    /// <param name="targetDegree">The degree to get the secondary dominant for.</param>
    /// <returns>The secondary dominant chord.</returns>
    public Chord GetSecondaryDominant(int targetDegree)
    {
        // Get the chord at the target degree
        var targetChord = GetChordByDegree(targetDegree);
        
        // The secondary dominant is a major chord built on the fifth above the target
        var dominantRoot = targetChord.Root.Transpose(new Interval(IntervalQuality.Perfect, 5));
        
        return new Chord(dominantRoot, ChordQuality.Major);
    }
}