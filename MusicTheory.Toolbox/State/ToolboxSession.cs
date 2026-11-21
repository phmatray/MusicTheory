using Microsoft.AspNetCore.Components;

namespace MusicTheory.Toolbox.State;

/// <summary>The current state, always re-read from the address bar (the URL is the only store).</summary>
public sealed class ToolboxSession(NavigationManager nav)
{
    /// <summary>Set once at startup from <c>navigator.language</c>.</summary>
    public string BrowserLang { get; set; } = "en";

    public ToolState Current => ToolboxUrl.Parse(new Uri(nav.Uri), BrowserLang);
}
