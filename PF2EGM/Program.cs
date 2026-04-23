using System.Text.Json;
using PF2EGM.Components;
using Supabase;
using PF2EGM.Services;
using LumexUI.Extensions;
using PF2EGM.Models;

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
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddSingleton<IDataTableService, DataTableService>();

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

try 
{
    // Read the actual file
    string jsonString = File.ReadAllText("DataTables/ItemsDataTable.json");
    
    // Attempt to convert the whole file into a list of Items
    var allItems = JsonSerializer.Deserialize<List<Item>>(jsonString);
    
    // ADDED SAFETY CHECK: This fixes the CS8602 warning!
    if (allItems != null && allItems.Count > 0)
    {
        Console.WriteLine($"\n--- SUCCESS! Loaded {allItems.Count} items.");
        Console.WriteLine($"--- The first item is: {allItems[0].Name}\n");
    }
    else 
    {
        Console.WriteLine("\n--- ERROR: The list was empty or null.\n");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n--- ERROR reading models: {ex.Message}\n");
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
await app.Services.GetRequiredService<IDataTableService>().LoadAllAsync();

app.Run();

