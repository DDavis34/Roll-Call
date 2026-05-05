using Microsoft.EntityFrameworkCore;
using RollCallBackend.Data;
using RollCallBackend.Services;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = builder.Configuration["Supabase:Url"]!;
var supabaseKey = builder.Configuration["Supabase:AnonKey"]!;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=data.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddSingleton(_ =>
{
    var options = new SupabaseOptions
    {
        AutoRefreshToken    = true,
        AutoConnectRealtime = false
    };
    return new Supabase.Client(supabaseUrl, supabaseKey, options);
});

builder.Services.AddScoped<ISupabaseAuthService, SupabaseAuthService>();
builder.Services.AddSingleton<IDataTableService, DataTableService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? ["https://localhost:7040", "http://localhost:5144"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    var supabase = scope.ServiceProvider.GetRequiredService<Supabase.Client>();
    await supabase.InitializeAsync();
}

await app.Services.GetRequiredService<IDataTableService>().LoadAllAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
