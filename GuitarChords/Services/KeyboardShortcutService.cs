using Microsoft.JSInterop;

namespace GuitarChords.Services;

public class KeyboardShortcutService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<KeyboardShortcutService>? _objRef;
    
    public event Action<string>? OnKeyPressed;
    
    public KeyboardShortcutService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync()
    {
        _objRef = DotNetObjectReference.Create(this);
        await _jsRuntime.InvokeVoidAsync("registerKeyboardShortcuts", _objRef);
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
            await _jsRuntime.InvokeVoidAsync("unregisterKeyboardShortcuts");
            _objRef.Dispose();
        }
    }
}