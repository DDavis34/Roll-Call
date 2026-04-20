using System.Text.Json;
using RollCallBackend.Models;

namespace RollCallBackend.Services;

public interface IDataTableService
{
    IReadOnlyList<Ancestry> Ancestries { get; }
    IReadOnlyList<Armor> Armors { get; }
    IReadOnlyList<Background> Backgrounds { get; }
    IReadOnlyList<Class> Classes { get; }
    IReadOnlyList<Creature> Creatures { get; }
    IReadOnlyList<Feat> Feats { get; }
    IReadOnlyList<Item> Items { get; }
    IReadOnlyList<Ritual> Rituals { get; }
    IReadOnlyList<Spell> Spells { get; }
    IReadOnlyList<Trait> Traits { get; }

    Task LoadAllAsync(CancellationToken ct = default);
}

public class DataTableService : IDataTableService
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly List<Ancestry> _ancestries = [];
    private readonly List<Armor> _armors = [];
    private readonly List<Background> _backgrounds = [];
    private readonly List<Class> _classes = [];
    private readonly List<Creature> _creatures = [];
    private readonly List<Feat> _feats = [];
    private readonly List<Item> _items = [];
    private readonly List<Ritual> _rituals = [];
    private readonly List<Spell> _spells = [];
    private readonly List<Trait> _traits = [];

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DataTableService> _logger;

    public DataTableService(IWebHostEnvironment env, ILogger<DataTableService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public IReadOnlyList<Ancestry> Ancestries => _ancestries;
    public IReadOnlyList<Armor> Armors => _armors;
    public IReadOnlyList<Background> Backgrounds => _backgrounds;
    public IReadOnlyList<Class> Classes => _classes;
    public IReadOnlyList<Creature> Creatures => _creatures;
    public IReadOnlyList<Feat> Feats => _feats;
    public IReadOnlyList<Item> Items => _items;
    public IReadOnlyList<Ritual> Rituals => _rituals;
    public IReadOnlyList<Spell> Spells => _spells;
    public IReadOnlyList<Trait> Traits => _traits;

    public async Task LoadAllAsync(CancellationToken ct = default)
    {
        var dataDir = Path.Combine(_env.ContentRootPath, "DataTables");

        _ancestries.AddRange(await LoadAsync<Ancestry>(dataDir, "AncestriesDataTable.json", ct));
        _armors.AddRange(await LoadAsync<Armor>(dataDir, "ArmorDataTable.json", ct));
        _backgrounds.AddRange(await LoadAsync<Background>(dataDir, "BackgroundsDataTable.json", ct));
        _classes.AddRange(await LoadAsync<Class>(dataDir, "ClassesDataTable.json", ct));
        _creatures.AddRange(await LoadAsync<Creature>(dataDir, "CreaturesDataTable.json", ct));
        _feats.AddRange(await LoadAsync<Feat>(dataDir, "FeatsDataTable.json", ct));
        _items.AddRange(await LoadAsync<Item>(dataDir, "ItemsDataTable.json", ct));
        _rituals.AddRange(await LoadAsync<Ritual>(dataDir, "RitualsDataTable.json", ct));
        _spells.AddRange(await LoadAsync<Spell>(dataDir, "SpellsDataTable.json", ct));
        _traits.AddRange(await LoadAsync<Trait>(dataDir, "TraitsDataTable.json", ct));

        _logger.LogInformation(
            "DataTableService loaded: {A} ancestries, {Ar} armors, {B} backgrounds, " +
            "{C} classes, {Cr} creatures, {F} feats, {I} items, {R} rituals, {S} spells.",
            _ancestries.Count, _armors.Count, _backgrounds.Count,
            _classes.Count, _creatures.Count, _feats.Count,
            _items.Count, _rituals.Count, _spells.Count);
    }

    private async Task<List<T>> LoadAsync<T>(string dir, string filename, CancellationToken ct)
    {
        var path = Path.Combine(dir, filename);
        if (!File.Exists(path))
        {
            _logger.LogWarning("Data file not found, skipping: {Path}", path);
            return [];
        }

        try
        {
            await using var stream = File.OpenRead(path);
            var result = await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOpts, ct);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load data table: {Filename}", filename);
            return [];
        }
    }
}
