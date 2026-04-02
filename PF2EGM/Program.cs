using PF2EGM.Components;
using Supabase;
using PF2EGM.Services;
using LumexUI.Extensions;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = builder.Configuration["Supabase:Url"]!;
var supabaseKey = builder.Configuration["Supabase:AnonKey"]!;

builder.Services.AddSingleton(_ =>
{
    var options = new SupabaseOptions
    {
        AutoRefreshToken    = true,
        AutoConnectRealtime = false   // enable if you need Realtime
    };
    return new Supabase.Client(supabaseUrl, supabaseKey, options);
});


builder.Services.AddScoped<ISupabaseAuthService, SupabaseAuthService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddLumexServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var supabase = scope.ServiceProvider.GetRequiredService<Supabase.Client>();
    await supabase.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

