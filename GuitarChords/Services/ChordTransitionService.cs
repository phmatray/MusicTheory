using GuitarChords.Models;

namespace GuitarChords.Services;

public class ChordTransitionService
{
    private readonly GuitarChordService _chordService;
    
    // Common chord progressions in different keys
    private readonly Dictionary<string, List<string[]>> _commonProgressions = new()
    {
        { "Major", new List<string[]>
            {
                new[] { "I", "IV", "V" },        // Classic I-IV-V
                new[] { "I", "V", "vi", "IV" },  // Pop progression
                new[] { "I", "vi", "IV", "V" },  // 50s progression
                new[] { "I", "IV", "I", "V" },   // 12-bar blues variation
                new[] { "ii", "V", "I" },        // Jazz ii-V-I
            }
        },
        { "Minor", new List<string[]>
            {
                new[] { "i", "iv", "v" },        // Minor i-iv-v
                new[] { "i", "VI", "III", "VII" }, // Natural minor progression
                new[] { "i", "iv", "VII", "i" }, // Andalusian cadence
                new[] { "i", "v", "i", "iv" },   // Minor blues
            }
        }
    };
    
    // Chord function mappings
    private readonly Dictionary<string, Dictionary<string, string>> _chordFunctions = new()
    {
        { "C", new Dictionary<string, string> 
            { 
                { "I", "C" }, { "ii", "Dm" }, { "iii", "Em" }, { "IV", "F" }, 
                { "V", "G" }, { "vi", "Am" }, { "vii°", "Bdim" },
                { "i", "Cm" }, { "iv", "Fm" }, { "v", "Gm" }, { "VI", "Ab" }, 
                { "III", "Eb" }, { "VII", "Bb" }
            } 
        },
        { "G", new Dictionary<string, string> 
            { 
                { "I", "G" }, { "ii", "Am" }, { "iii", "Bm" }, { "IV", "C" }, 
                { "V", "D" }, { "vi", "Em" }, { "vii°", "F#dim" },
                { "i", "Gm" }, { "iv", "Cm" }, { "v", "Dm" }, { "VI", "Eb" }, 
                { "III", "Bb" }, { "VII", "F" }
            } 
        },
        { "D", new Dictionary<string, string> 
            { 
                { "I", "D" }, { "ii", "Em" }, { "iii", "F#m" }, { "IV", "G" }, 
                { "V", "A" }, { "vi", "Bm" }, { "vii°", "C#dim" },
                { "i", "Dm" }, { "iv", "Gm" }, { "v", "Am" }, { "VI", "Bb" }, 
                { "III", "F" }, { "VII", "C" }
            } 
        },
        { "A", new Dictionary<string, string> 
            { 
                { "I", "A" }, { "ii", "Bm" }, { "iii", "C#m" }, { "IV", "D" }, 
                { "V", "E" }, { "vi", "F#m" }, { "vii°", "G#dim" },
                { "i", "Am" }, { "iv", "Dm" }, { "v", "Em" }, { "VI", "F" }, 
                { "III", "C" }, { "VII", "G" }
            } 
        },
        { "E", new Dictionary<string, string> 
            { 
                { "I", "E" }, { "ii", "F#m" }, { "iii", "G#m" }, { "IV", "A" }, 
                { "V", "B" }, { "vi", "C#m" }, { "vii°", "D#dim" },
                { "i", "Em" }, { "iv", "Am" }, { "v", "Bm" }, { "VI", "C" }, 
                { "III", "G" }, { "VII", "D" }
            } 
        }
    };
    
    public ChordTransitionService(GuitarChordService chordService)
    {
        _chordService = chordService;
    }
    
    public List<ChordSuggestion> GetNextChordSuggestions(GuitarChord currentChord, List<GuitarChord> recentChords)
    {
        var suggestions = new List<ChordSuggestion>();
        
        // Get the key context from recent chords
        var key = DetectKey(recentChords);
        
        // Add suggestions based on chord function
        AddFunctionalSuggestions(suggestions, currentChord, key);
        
        // Add suggestions based on voice leading
        AddVoiceLeadingSuggestions(suggestions, currentChord);
        
        // Add suggestions based on common progressions
        AddProgressionSuggestions(suggestions, currentChord, recentChords);
        
        // Sort by confidence and remove duplicates
        return suggestions
            .GroupBy(s => s.Chord.Name)
            .Select(g => g.OrderByDescending(s => s.Confidence).First())
            .OrderByDescending(s => s.Confidence)
            .Take(6)
            .ToList();
    }
    
    private string DetectKey(List<GuitarChord> recentChords)
    {
        // Simple key detection based on chord occurrences
        // In a real implementation, this would be more sophisticated
        if (!recentChords.Any()) return "C";
        
        var chordCounts = recentChords
            .GroupBy(c => c.Chord.Root.Name.ToString())
            .OrderByDescending(g => g.Count())
            .ToList();
        
        return chordCounts.First().Key;
    }
    
    private void AddFunctionalSuggestions(List<ChordSuggestion> suggestions, GuitarChord currentChord, string key)
    {
        if (!_chordFunctions.ContainsKey(key)) return;
        
        var functions = _chordFunctions[key];
        var currentFunction = functions.FirstOrDefault(f => f.Value == currentChord.Name).Key;
        
        if (currentFunction != null)
        {
            // Suggest common next chords based on function
            switch (currentFunction)
            {
                case "I":
                    AddSuggestionIfExists(suggestions, key, "IV", "Subdominant - Creates movement", 0.9f);
                    AddSuggestionIfExists(suggestions, key, "V", "Dominant - Strong resolution", 0.9f);
                    AddSuggestionIfExists(suggestions, key, "vi", "Relative minor - Emotional shift", 0.8f);
                    break;
                case "IV":
                    AddSuggestionIfExists(suggestions, key, "V", "Dominant - Natural progression", 0.95f);
                    AddSuggestionIfExists(suggestions, key, "I", "Tonic - Return home", 0.85f);
                    AddSuggestionIfExists(suggestions, key, "ii", "Supertonic - Jazz flavor", 0.7f);
                    break;
                case "V":
                    AddSuggestionIfExists(suggestions, key, "I", "Tonic - Strong resolution", 0.95f);
                    AddSuggestionIfExists(suggestions, key, "vi", "Deceptive cadence", 0.8f);
                    break;
                case "vi":
                    AddSuggestionIfExists(suggestions, key, "IV", "Subdominant - Smooth transition", 0.85f);
                    AddSuggestionIfExists(suggestions, key, "ii", "Supertonic - Minor feel", 0.8f);
                    AddSuggestionIfExists(suggestions, key, "V", "Dominant - Build tension", 0.75f);
                    break;
            }
        }
    }
    
    private void AddVoiceLeadingSuggestions(List<ChordSuggestion> suggestions, GuitarChord currentChord)
    {
        var allChords = _chordService.GetAllChords();
        
        foreach (var chord in allChords)
        {
            if (chord.Name == currentChord.Name) continue;
            
            var commonTones = CountCommonTones(currentChord, chord);
            var fingerDistance = CalculateFingerDistance(currentChord, chord);
            
            if (commonTones >= 2 || fingerDistance <= 2)
            {
                var confidence = (commonTones * 0.3f) + ((5 - fingerDistance) * 0.1f);
                var reason = commonTones >= 2 
                    ? $"{commonTones} common tones - Smooth voice leading" 
                    : "Minimal finger movement required";
                
                suggestions.Add(new ChordSuggestion
                {
                    Chord = chord,
                    Reason = reason,
                    Confidence = Math.Min(confidence, 0.7f),
                    TransitionDifficulty = fingerDistance <= 2 ? "Easy" : "Medium"
                });
            }
        }
    }
    
    private void AddProgressionSuggestions(List<ChordSuggestion> suggestions, GuitarChord currentChord, List<GuitarChord> recentChords)
    {
        // Look for patterns in recent chords
        if (recentChords.Count >= 2)
        {
            var lastTwo = recentChords.TakeLast(2).Select(c => c.Name).ToList();
            
            // Check common two-chord patterns
            var patterns = new Dictionary<(string, string), List<(string, string)>>
            {
                { ("C", "Am"), new List<(string, string)> { ("F", "Continue the progression"), ("G", "Complete the cycle") } },
                { ("G", "Em"), new List<(string, string)> { ("C", "Resolution to tonic"), ("D", "Secondary dominant") } },
                { ("D", "A"), new List<(string, string)> { ("G", "IV chord in D"), ("Bm", "Relative minor") } },
                { ("Am", "F"), new List<(string, string)> { ("C", "Back to tonic"), ("G", "Dominant preparation") } },
            };
            
            var key = (lastTwo[0], lastTwo[1]);
            if (patterns.ContainsKey(key))
            {
                foreach (var (chordName, reason) in patterns[key])
                {
                    var chord = _chordService.GetChord(chordName);
                    if (chord != null)
                    {
                        suggestions.Add(new ChordSuggestion
                        {
                            Chord = chord,
                            Reason = $"Common progression: {reason}",
                            Confidence = 0.8f,
                            TransitionDifficulty = "Medium"
                        });
                    }
                }
            }
        }
    }
    
    private void AddSuggestionIfExists(List<ChordSuggestion> suggestions, string key, string function, string reason, float confidence)
    {
        if (_chordFunctions.ContainsKey(key) && _chordFunctions[key].ContainsKey(function))
        {
            var chordName = _chordFunctions[key][function];
            var chord = _chordService.GetChord(chordName);
            
            if (chord != null)
            {
                suggestions.Add(new ChordSuggestion
                {
                    Chord = chord,
                    Reason = reason,
                    Confidence = confidence,
                    TransitionDifficulty = DetermineTransitionDifficulty(chord)
                });
            }
        }
    }
    
    private int CountCommonTones(GuitarChord chord1, GuitarChord chord2)
    {
        var notes1 = chord1.Chord.GetNotes().Select(n => n.Name).ToHashSet();
        var notes2 = chord2.Chord.GetNotes().Select(n => n.Name).ToHashSet();
        return notes1.Intersect(notes2).Count();
    }
    
    private int CalculateFingerDistance(GuitarChord chord1, GuitarChord chord2)
    {
        var distance = 0;
        
        for (int i = 0; i < 6; i++)
        {
            if (chord1.FretPositions[i] >= 0 && chord2.FretPositions[i] >= 0)
            {
                distance += Math.Abs(chord1.FretPositions[i] - chord2.FretPositions[i]);
            }
        }
        
        return distance;
    }
    
    private string DetermineTransitionDifficulty(GuitarChord chord)
    {
        if (chord.IsBarreChord) return "Hard";
        
        var fingersUsed = chord.Fingerings.Count(f => f > 0);
        if (fingersUsed <= 2) return "Easy";
        else if (fingersUsed <= 3) return "Medium";
        else return "Hard";
    }
}

public class ChordSuggestion
{
    public GuitarChord Chord { get; set; } = null!;
    public string Reason { get; set; } = "";
    public float Confidence { get; set; }
    public string TransitionDifficulty { get; set; } = "Medium";
}