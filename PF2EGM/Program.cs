using System.Text.Json;
using PF2EGM.Components;
using PF2EGM.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

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

app.Run();