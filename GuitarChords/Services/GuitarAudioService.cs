using Microsoft.JSInterop;
using GuitarChords.Models;

namespace GuitarChords.Services;

public class GuitarAudioService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isInitialized;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public GuitarAudioService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized)
                return;

            try
            {
                await _jsRuntime.InvokeVoidAsync("GuitarAudio.initialize");
                _isInitialized = true;
            }
            catch (InvalidOperationException)
            {
                // During prerendering, JavaScript is not available
                _isInitialized = false;
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task PlayChordAsync(GuitarChord chord)
    {
        await EnsureInitializedAsync();
        await _jsRuntime.InvokeVoidAsync("GuitarAudio.playChord", chord.FretPositions);
    }

    public async Task PlayChordAsync(int[] fretPositions)
    {
        await EnsureInitializedAsync();
        await _jsRuntime.InvokeVoidAsync("GuitarAudio.playChord", fretPositions);
    }

    public async Task PlayNoteAsync(int stringIndex, int fret)
    {
        await EnsureInitializedAsync();
        await _jsRuntime.InvokeVoidAsync("GuitarAudio.playNote", stringIndex, fret);
    }

    public async Task StopAllAsync()
    {
        if (!_isInitialized)
            return;

        await _jsRuntime.InvokeVoidAsync("GuitarAudio.stopAll");
    }

    public async Task SetPluckStrengthAsync(float strength)
    {
        if (!_isInitialized)
            return;

        await _jsRuntime.InvokeVoidAsync("GuitarAudio.setPluckStrength", strength);
    }

    public async Task SetStringDampingAsync(float damping)
    {
        if (!_isInitialized)
            return;

        await _jsRuntime.InvokeVoidAsync("GuitarAudio.setStringDamping", damping);
    }

    /// <summary>
    /// Plays a metronome click sound.
    /// </summary>
    /// <param name="isDownbeat">True for the first beat of a measure (higher pitch), false for other beats.</param>
    public async Task PlayMetronomeClickAsync(bool isDownbeat = false)
    {
        await EnsureInitializedAsync();
        try
        {
            await _jsRuntime.InvokeVoidAsync("GuitarAudio.playMetronomeClick", isDownbeat);
        }
        catch (InvalidOperationException)
        {
            // During prerendering, JavaScript is not available
        }
    }

    /// <summary>
    /// Plays a chord with strumming effect (strings played in sequence).
    /// </summary>
    /// <param name="chord">The chord to play.</param>
    /// <param name="direction">"down" for low to high, "up" for high to low.</param>
    /// <param name="speed">Milliseconds between each string (default 30ms).</param>
    public async Task PlayChordStrummedAsync(GuitarChord chord, string direction = "down", int speed = 30)
    {
        await EnsureInitializedAsync();
        await _jsRuntime.InvokeVoidAsync("GuitarAudio.playChordStrummed", chord.FretPositions, direction, speed);
    }

    /// <summary>
    /// Plays a chord with strumming effect using fret positions array.
    /// </summary>
    public async Task PlayChordStrummedAsync(int[] fretPositions, string direction = "down", int speed = 30)
    {
        await EnsureInitializedAsync();
        await _jsRuntime.InvokeVoidAsync("GuitarAudio.playChordStrummed", fretPositions, direction, speed);
    }

    public async Task<AudioState> GetStateAsync()
    {
        try
        {
            var state = await _jsRuntime.InvokeAsync<AudioStateDto>("GuitarAudio.getState");
            return new AudioState
            {
                IsInitialized = state.initialized,
                ContextState = state.contextState
            };
        }
        catch
        {
            return new AudioState
            {
                IsInitialized = false,
                ContextState = "error"
            };
        }
    }

    /// <summary>
    /// Sets the master volume (0.0 to 1.0).
    /// </summary>
    public async Task SetMasterVolumeAsync(double volume)
    {
        await EnsureInitializedAsync();
        await _jsRuntime.InvokeVoidAsync("GuitarAudio.setMasterVolume", Math.Clamp(volume, 0.0, 1.0));
    }

    /// <summary>
    /// Gets the current master volume.
    /// </summary>
    public async Task<double> GetMasterVolumeAsync()
    {
        await EnsureInitializedAsync();
        return await _jsRuntime.InvokeAsync<double>("GuitarAudio.getMasterVolume");
    }

    /// <summary>
    /// Gets available strumming patterns.
    /// </summary>
    public async Task<List<StrummingPattern>> GetStrummingPatternsAsync()
    {
        await EnsureInitializedAsync();
        return await _jsRuntime.InvokeAsync<List<StrummingPattern>>("GuitarAudio.getStrummingPatterns");
    }

    /// <summary>
    /// Plays a chord with a strumming pattern.
    /// </summary>
    /// <param name="chord">The chord to play.</param>
    /// <param name="patternId">The strumming pattern ID.</param>
    /// <param name="bpm">Beats per minute.</param>
    /// <param name="measures">Number of measures to play.</param>
    public async Task PlayStrummingPatternAsync(GuitarChord chord, string patternId, int bpm = 120, int measures = 1)
    {
        await EnsureInitializedAsync();
        await _jsRuntime.InvokeVoidAsync("GuitarAudio.playStrummingPattern", chord.FretPositions, patternId, bpm, measures);
    }

    /// <summary>
    /// Plays a chord with a strumming pattern using fret positions.
    /// </summary>
    public async Task PlayStrummingPatternAsync(int[] fretPositions, string patternId, int bpm = 120, int measures = 1)
    {
        await EnsureInitializedAsync();
        await _jsRuntime.InvokeVoidAsync("GuitarAudio.playStrummingPattern", fretPositions, patternId, bpm, measures);
    }

    private async Task EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isInitialized)
        {
            await StopAllAsync();
        }
        _initLock?.Dispose();
    }

    private class AudioStateDto
    {
        public bool initialized { get; set; }
        public string contextState { get; set; } = string.Empty;
    }
}

public class AudioState
{
    public bool IsInitialized { get; set; }
    public string ContextState { get; set; } = string.Empty;
}

public class StrummingPattern
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Pattern { get; set; } = string.Empty;
    public int BeatsPerMeasure { get; set; }
}