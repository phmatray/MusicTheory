using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MusicTheory.Toolbox;
using MusicTheory.Toolbox.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<ToolboxSession>();

var host = builder.Build();
// Read once: only used when the link carries no (valid) lang.
host.Services.GetRequiredService<ToolboxSession>().BrowserLang =
    await host.Services.GetRequiredService<IJSRuntime>().InvokeAsync<string>("tbBrowserLang");
await host.RunAsync();
