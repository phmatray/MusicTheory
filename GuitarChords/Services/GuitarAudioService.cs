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