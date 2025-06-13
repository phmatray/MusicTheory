using GuitarChords.Models;

namespace GuitarChords.Services;

public class ChordPlayerService
{
    private CancellationTokenSource? _playbackCancellation;
    private int _currentChordIndex = -1;
    private bool _isPlaying = false;

    public event Action<int>? ChordChanged;
    public event Action? PlaybackStarted;
    public event Action? PlaybackStopped;

    public bool IsPlaying => _isPlaying;
    public int CurrentChordIndex => _currentChordIndex;

    public async Task PlayProgressionAsync(List<GuitarChord> chords, int tempo = 120)
    {
        if (_isPlaying)
        {
            StopPlayback();
            return;
        }

        _playbackCancellation = new CancellationTokenSource();
        _isPlaying = true;
        _currentChordIndex = 0;
        
        PlaybackStarted?.Invoke();

        try
        {
            // Calculate delay between chords based on tempo (assuming 4/4 time)
            var beatDuration = 60000 / tempo; // milliseconds per beat
            var chordDuration = beatDuration * 4; // 4 beats per chord

            foreach (var chord in chords)
            {
                if (_playbackCancellation.Token.IsCancellationRequested)
                    break;

                ChordChanged?.Invoke(_currentChordIndex);
                
                // In a real implementation, this would trigger audio playback
                // For now, we just wait for the duration
                await Task.Delay((int)chordDuration, _playbackCancellation.Token);

                _currentChordIndex++;
                if (_currentChordIndex >= chords.Count)
                    _currentChordIndex = 0; // Loop back to start
            }
        }
        catch (TaskCanceledException)
        {
            // Playback was cancelled
        }
        finally
        {
            _isPlaying = false;
            _currentChordIndex = -1;
            PlaybackStopped?.Invoke();
        }
    }

    public void StopPlayback()
    {
        _playbackCancellation?.Cancel();
        _playbackCancellation?.Dispose();
        _playbackCancellation = null;
        _isPlaying = false;
        _currentChordIndex = -1;
        PlaybackStopped?.Invoke();
    }

    public void PlaySingleChord(GuitarChord chord)
    {
        // In a real implementation, this would play the chord sound
        // For now, we'll just trigger the event
        Console.WriteLine($"Playing chord: {chord.Name}");
    }
}