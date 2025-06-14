using Microsoft.JSInterop;
using System.Text.Json;

namespace GuitarChords.Services;

public class UserPreferencesService
{
    private readonly IJSRuntime _jsRuntime;
    private const string PreferencesKey = "guitarChords.preferences";
    private UserPreferences? _cachedPreferences;
    
    public event Action? PreferencesChanged;
    
    public UserPreferencesService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<UserPreferences> GetPreferencesAsync()
    {
        if (_cachedPreferences != null)
            return _cachedPreferences;
        
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", PreferencesKey);
            if (!string.IsNullOrEmpty(json))
            {
                _cachedPreferences = JsonSerializer.Deserialize<UserPreferences>(json) ?? new UserPreferences();
            }
            else
            {
                _cachedPreferences = new UserPreferences();
            }
        }
        catch
        {
            _cachedPreferences = new UserPreferences();
        }
        
        return _cachedPreferences;
    }
    
    public async Task SavePreferencesAsync(UserPreferences preferences)
    {
        _cachedPreferences = preferences;
        var json = JsonSerializer.Serialize(preferences);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", PreferencesKey, json);
        PreferencesChanged?.Invoke();
    }
    
    public async Task UpdatePreferenceAsync<T>(string propertyName, T value)
    {
        var preferences = await GetPreferencesAsync();
        var property = typeof(UserPreferences).GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(preferences, value);
            await SavePreferencesAsync(preferences);
        }
    }
    
    public async Task ResetPreferencesAsync()
    {
        _cachedPreferences = new UserPreferences();
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", PreferencesKey);
        PreferencesChanged?.Invoke();
    }
}

public class UserPreferences
{
    // Display preferences
    public string DefaultView { get; set; } = "grid";
    public bool ShowChordDiagrams { get; set; } = true;
    public bool AutoShowFilters { get; set; } = false;
    public bool AutoShowProgression { get; set; } = false;
    
    // Practice preferences
    public int DefaultBpm { get; set; } = 120;
    public int DefaultBeatsPerChord { get; set; } = 4;
    public bool PlayMetronomeSound { get; set; } = true;
    public bool ShowDiagramInPractice { get; set; } = true;
    public string DefaultPracticeMode { get; set; } = "single";
    
    // Chord preferences
    public List<string> FavoriteChords { get; set; } = new();
    public List<string> RecentChords { get; set; } = new();
    public int MaxRecentChords { get; set; } = 12;
    public bool ShowBeginnerChords { get; set; } = true;
    public bool ShowIntermediateChords { get; set; } = true;
    public bool ShowAdvancedChords { get; set; } = true;
    
    // Progression preferences
    public bool AutoSuggestNextChord { get; set; } = true;
    public int SuggestionConfidenceThreshold { get; set; } = 60;
    public bool ShowTransitionDifficulty { get; set; } = true;
    
    // Export preferences
    public string DefaultExportFormat { get; set; } = "text";
    public bool IncludeChordDiagramsInPdf { get; set; } = true;
    
    // Accessibility
    public bool HighContrastMode { get; set; } = false;
    public bool ReducedMotion { get; set; } = false;
    public float ChordDiagramScale { get; set; } = 1.0f;
    
    // Theme
    public bool DarkMode { get; set; } = false;
    public string AccentColor { get; set; } = "blue";
}