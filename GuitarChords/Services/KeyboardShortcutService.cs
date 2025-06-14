using Microsoft.JSInterop;

namespace GuitarChords.Services;

public class KeyboardShortcutService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<KeyboardShortcutService>? _objRef;
    private bool _isInitialized;
    
    public event Action<string>? OnKeyPressed;
    
    public KeyboardShortcutService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync()
    {
        try
        {
            _objRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("registerKeyboardShortcuts", _objRef);
            _isInitialized = true;
        }
        catch (InvalidOperationException)
        {
            // Ignore if we're in prerendering
            _isInitialized = false;
        }
    }
    
    [JSInvokable]
    public void HandleKeyPress(string key)
    {
        OnKeyPressed?.Invoke(key);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_objRef != null)
        {
            // Only try to unregister if we successfully initialized
            if (_isInitialized)
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync("unregisterKeyboardShortcuts");
                }
                catch (InvalidOperationException)
                {
                    // Ignore if we're in prerendering or disposal
                }
            }
            
            _objRef.Dispose();
            _objRef = null;
        }
    }
}