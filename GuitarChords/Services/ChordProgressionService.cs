using System.Text.Json;

namespace GuitarChords.Services;

public class ChordProgressionService
{
    private readonly List<ChordProgression> _savedProgressions = new();
    private readonly Dictionary<string, List<string>> _commonProgressions = new()
    {
        { "I-V-vi-IV (Pop)", new List<string> { "C", "G", "Am", "F" } },
        { "I-IV-V (Blues)", new List<string> { "C", "F", "G" } },
        { "I-vi-IV-V (50s)", new List<string> { "C", "Am", "F", "G" } },
        { "ii-V-I (Jazz)", new List<string> { "Dm", "G7", "C" } },
        { "I-V-vi-iii-IV (Canon)", new List<string> { "C", "G", "Am", "Em", "F" } },
        { "vi-IV-I-V (Pop Alt)", new List<string> { "Am", "F", "C", "G" } },
        { "I-IV-vi-V (Country)", new List<string> { "G", "C", "Em", "D" } },
        { "I-bVII-IV-I (Rock)", new List<string> { "C", "Bb", "F", "C" } }
    };

    public ChordProgressionService()
    {
        // Load saved progressions from local storage or initialize with defaults
        InitializeDefaultProgressions();
    }

    private void InitializeDefaultProgressions()
    {
        _savedProgressions.Add(new ChordProgression
        {
            Id = Guid.NewGuid(),
            Name = "Basic Pop Progression",
            ChordNames = new List<string> { "C", "G", "Am", "F" },
            Tempo = 120,
            CreatedDate = DateTime.Now
        });

        _savedProgressions.Add(new ChordProgression
        {
            Id = Guid.NewGuid(),
            Name = "Blues in G",
            ChordNames = new List<string> { "G", "G7", "C", "C7", "G", "D7" },
            Tempo = 100,
            CreatedDate = DateTime.Now
        });
    }

    public IEnumerable<ChordProgression> GetSavedProgressions() => _savedProgressions;

    public Dictionary<string, List<string>> GetCommonProgressions() => _commonProgressions;

    public ChordProgression SaveProgression(string name, List<string> chordNames, int tempo = 120)
    {
        var progression = new ChordProgression
        {
            Id = Guid.NewGuid(),
            Name = name,
            ChordNames = chordNames,
            Tempo = tempo,
            CreatedDate = DateTime.Now
        };

        _savedProgressions.Add(progression);
        return progression;
    }

    public void DeleteProgression(Guid id)
    {
        _savedProgressions.RemoveAll(p => p.Id == id);
    }

    public string ExportProgression(ChordProgression progression)
    {
        return JsonSerializer.Serialize(progression, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }

    public ChordProgression? ImportProgression(string json)
    {
        try
        {
            var progression = JsonSerializer.Deserialize<ChordProgression>(json);
            if (progression != null)
            {
                progression.Id = Guid.NewGuid(); // Generate new ID to avoid conflicts
                _savedProgressions.Add(progression);
                return progression;
            }
        }
        catch
        {
            // Invalid JSON
        }
        return null;
    }
}

public class ChordProgression
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> ChordNames { get; set; } = new();
    public int Tempo { get; set; } = 120;
    public DateTime CreatedDate { get; set; }
    public string? Description { get; set; }
}