using GuitarChords.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add application services
builder.Services.AddSingleton<GuitarChords.Services.GuitarChordService>();
builder.Services.AddSingleton<GuitarChords.Services.ChordProgressionService>();
builder.Services.AddSingleton<GuitarChords.Services.ChordTransitionService>();
builder.Services.AddScoped<GuitarChords.Services.ChordPlayerService>();
builder.Services.AddScoped<GuitarChords.Services.KeyboardShortcutService>();
builder.Services.AddScoped<GuitarChords.Services.UserPreferencesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();