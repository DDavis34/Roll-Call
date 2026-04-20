using PF2EGM.Components;
using PF2EGM.Services;
using LumexUI.Extensions;

var builder = WebApplication.CreateBuilder(args);

var backendUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:7001";

builder.Services.AddHttpClient<IApiAuthService, ApiAuthService>(client =>
{
    client.BaseAddress = new Uri(backendUrl);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddLumexServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
