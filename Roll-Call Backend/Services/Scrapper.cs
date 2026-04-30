using System.Net.Http.Json;
using System.Text.Json;

namespace RollCallBackend.Services;

public class Scrapper
{
    private const string SearchUrl = "https://elasticsearch.aonprd.com/aon/_search";
    private const int PageSize = 1000;

    private readonly IHttpClientFactory _httpFactory;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<Scrapper> _logger;

    public Scrapper(IHttpClientFactory httpFactory, IWebHostEnvironment env, ILogger<Scrapper> logger)
    {
        _httpFactory = httpFactory;
        _env = env;
        _logger = logger;
    }

    // ── Public API ──────────────────────────────────────────────────────────────

    public async Task ScrapeAllAsync(IProgress<string>? progress = null, CancellationToken ct = default)
    {
        var dir = Path.Combine(_env.ContentRootPath, "DataTables");

        await Task.WhenAll(
            Save(dir, "AncestriesDataTable.json",  ScrapeAncestriesAsync(ct),  progress, "Ancestries",  ct),
            Save(dir, "ClassesDataTable.json",      ScrapeClassesAsync(ct),     progress, "Classes",     ct),
            Save(dir, "FeatsDataTable.json",        ScrapeFeatsAsync(ct),       progress, "Feats",       ct),
            Save(dir, "SpellsDataTable.json",       ScrapeSpellsAsync(ct),      progress, "Spells",      ct),
            Save(dir, "BackgroundsDataTable.json",  ScrapeBackgroundsAsync(ct), progress, "Backgrounds", ct),
            Save(dir, "ArmorDataTable.json",        ScrapeArmorAsync(ct),       progress, "Armor",       ct),
            Save(dir, "ItemsDataTable.json",        ScrapeItemsAsync(ct),       progress, "Items",       ct),
            Save(dir, "CreaturesDataTable.json",    ScrapeCreaturesAsync(ct),   progress, "Creatures",   ct),
            Save(dir, "RitualsDataTable.json",      ScrapeRitualsAsync(ct),     progress, "Rituals",     ct),
            Save(dir, "SkillsDataTable.json",       ScrapeSkillFeatsAsync(ct),  progress, "Skills",      ct),
            Save(dir, "TraitsDataTable.json",       ScrapeTraitsAsync(ct),      progress, "Traits",      ct)
        );
    }

    public Task ScrapeAsync(string category, IProgress<string>? progress = null, CancellationToken ct = default)
    {
        var dir = Path.Combine(_env.ContentRootPath, "DataTables");
        return category.ToLowerInvariant() switch
        {
            "ancestries"  => Save(dir, "AncestriesDataTable.json",  ScrapeAncestriesAsync(ct),  progress, "Ancestries",  ct),
            "classes"     => Save(dir, "ClassesDataTable.json",      ScrapeClassesAsync(ct),     progress, "Classes",     ct),
            "feats"       => Save(dir, "FeatsDataTable.json",        ScrapeFeatsAsync(ct),       progress, "Feats",       ct),
            "spells"      => Save(dir, "SpellsDataTable.json",       ScrapeSpellsAsync(ct),      progress, "Spells",      ct),
            "backgrounds" => Save(dir, "BackgroundsDataTable.json",  ScrapeBackgroundsAsync(ct), progress, "Backgrounds", ct),
            "armor"       => Save(dir, "ArmorDataTable.json",        ScrapeArmorAsync(ct),       progress, "Armor",       ct),
            "items"       => Save(dir, "ItemsDataTable.json",        ScrapeItemsAsync(ct),       progress, "Items",       ct),
            "creatures"   => Save(dir, "CreaturesDataTable.json",    ScrapeCreaturesAsync(ct),   progress, "Creatures",   ct),
            "rituals"     => Save(dir, "RitualsDataTable.json",      ScrapeRitualsAsync(ct),     progress, "Rituals",     ct),
            "skills"      => Save(dir, "SkillsDataTable.json",       ScrapeSkillFeatsAsync(ct),  progress, "Skills",      ct),
            "traits"      => Save(dir, "TraitsDataTable.json",       ScrapeTraitsAsync(ct),      progress, "Traits",      ct),
            _             => Task.FromException(new ArgumentException($"Unknown category: {category}"))
        };
    }

    // ── Per-category scrape methods ─────────────────────────────────────────────

    private async Task<List<object>> ScrapeAncestriesAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("ancestry", ct: ct);
        return hits.Select(el => (object)new
        {
            name          = Str(el, "name"),
            hp            = Str(el, "hp_raw") is { Length: > 0 } s ? s : Str(el, "hp"),
            size          = JoinArray(el, "size"),
            speed         = Str(el, "speed_raw"),
            ability_boost = JoinArray(el, "attribute"),
            ability_flaw  = JoinArray(el, "attribute_flaw"),
            language      = JoinArray(el, "language"),
            vision        = Str(el, "vision"),
            rarity        = Cap(Str(el, "rarity")),
            pfs           = Str(el, "pfs")
        }).ToList();
    }

    private async Task<List<object>> ScrapeClassesAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("class", ct: ct);
        return hits.Select(el => (object)new
        {
            name                  = Str(el, "name"),
            ability               = JoinArray(el, "attribute"),
            hp                    = Str(el, "hp_raw") is { Length: > 0 } s ? s : Str(el, "hp"),
            attack_proficiency    = JoinArray(el, "attack_proficiency", "\n"),
            defense_proficiency   = JoinArray(el, "defense_proficiency", "\n"),
            fortitude_proficiency = Str(el, "fortitude_proficiency"),
            reflex_proficiency    = Str(el, "reflex_proficiency"),
            will_proficiency      = Str(el, "will_proficiency"),
            perception_proficiency = Str(el, "perception_proficiency"),
            skill_proficiency     = JoinArray(el, "skill_proficiency", "\n"),
            rarity                = Cap(Str(el, "rarity")),
            pfs                   = Str(el, "pfs")
        }).ToList();
    }

    private async Task<List<object>> ScrapeFeatsAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("feat", ct: ct);
        return hits.Select(el => (object)new
        {
            name         = Str(el, "name"),
            level        = Str(el, "level"),
            trait        = JoinArray(el, "trait_raw"),
            prerequisite = Str(el, "prerequisite"),
            summary      = Str(el, "summary"),
            rarity       = Cap(Str(el, "rarity")),
            pfs          = Str(el, "pfs"),
            source       = FirstArr(el, "source_raw") ?? Str(el, "primary_source_raw")
        }).ToList();
    }

    private async Task<List<object>> ScrapeSpellsAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("spell", ct: ct);
        return hits.Select(el => (object)new
        {
            name       = Str(el, "name"),
            spell_type = Cap(Str(el, "type")),
            rank       = OrdinalRank(el),
            heighten   = JoinArray(el, "heighten"),
            tradition  = JoinArray(el, "tradition"),
            school     = Cap(Str(el, "school")),
            trait      = JoinArray(el, "trait_raw"),
            actions    = Str(el, "actions"),
            component  = JoinArray(el, "component", ", ", cap: true),
            trigger    = Str(el, "trigger"),
            target     = Str(el, "target"),
            range      = Str(el, "range_raw"),
            area       = Str(el, "area_raw"),
            duration   = Str(el, "duration"),
            defense    = Str(el, "saving_throw"),
            rarity     = Cap(Str(el, "rarity")),
            pfs        = Str(el, "pfs")
        }).ToList();
    }

    private async Task<List<object>> ScrapeBackgroundsAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("background", ct: ct);
        return hits.Select(el => (object)new
        {
            name   = Str(el, "name"),
            pfs    = Str(el, "pfs"),
            ability = JoinArray(el, "attribute"),
            skill  = JoinArray(el, "skill"),
            feat   = JoinArray(el, "feat"),
            rarity = Cap(Str(el, "rarity")),
            source = FirstArr(el, "source_raw") ?? Str(el, "primary_source_raw")
        }).ToList();
    }

    private async Task<List<object>> ScrapeArmorAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("armor", ct: ct);
        return hits.Select(el => (object)new
        {
            name           = Str(el, "name"),
            armor_category = Str(el, "armor_category"),
            ac             = Str(el, "ac_raw") is { Length: > 0 } s ? s : Str(el, "ac"),
            dex_cap        = Str(el, "dex_cap"),
            check_penalty  = Str(el, "check_penalty"),
            speed_penalty  = Str(el, "speed_penalty"),
            strength       = Str(el, "strength_raw") is { Length: > 0 } sr ? sr : Str(el, "strength"),
            bulk           = Str(el, "bulk_raw") is { Length: > 0 } br ? br : Str(el, "bulk"),
            armor_group    = Str(el, "armor_group"),
            trait          = JoinArray(el, "trait_raw")
        }).ToList();
    }

    private async Task<List<object>> ScrapeItemsAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("equipment", ct: ct);
        return hits.Select(el => (object)new
        {
            name             = Str(el, "name"),
            item_category    = Str(el, "item_category"),
            item_subcategory = Str(el, "item_subcategory"),
            level            = Str(el, "level"),
            price            = Str(el, "price_raw") is { Length: > 0 } p ? p : FormatPrice(el),
            bulk             = Str(el, "bulk_raw") is { Length: > 0 } b ? b : Str(el, "bulk"),
            trait            = JoinArray(el, "trait_raw"),
            rarity           = Cap(Str(el, "rarity")),
            pfs              = Str(el, "pfs")
        }).ToList();
    }

    private async Task<List<object>> ScrapeCreaturesAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("creature", ct: ct);
        return hits.Select(el => (object)new
        {
            name                  = Str(el, "name"),
            level                 = Str(el, "level"),
            creature_family       = Str(el, "creature_family"),
            source                = FirstArr(el, "source_raw") ?? Str(el, "primary_source_raw"),
            rarity                = Cap(Str(el, "rarity")),
            size                  = JoinArray(el, "size"),
            trait                 = JoinArray(el, "trait_raw"),
            hp                    = Str(el, "hp_raw") is { Length: > 0 } h ? h : Str(el, "hp"),
            hp_scale              = Str(el, "hp_scale"),
            ac                    = Str(el, "ac"),
            ac_scale              = Str(el, "ac_scale"),
            fortitude             = Mod(el, "fortitude_save"),
            fortitude_scale       = Str(el, "fortitude_save_scale"),
            reflex                = Mod(el, "reflex_save"),
            reflex_scale          = Str(el, "reflex_save_scale"),
            will                  = Mod(el, "will_save"),
            will_scale            = Str(el, "will_save_scale"),
            immunity              = JoinArray(el, "immunity"),
            resistance            = FormatResistanceObj(el, "resistance"),
            weakness              = FormatResistanceObj(el, "weakness"),
            creature_ability      = JoinArray(el, "creature_ability"),
            perception            = Mod(el, "perception"),
            perception_scale      = Str(el, "perception_scale"),
            sense                 = Str(el, "sense").Trim(),
            speed                 = Str(el, "speed_raw"),
            attack_bonus          = FirstIntArr(el, "attack_bonus"),
            attack_bonus_scale    = Str(el, "attack_bonus_scale"),
            strike_damage_average = FirstIntArr(el, "strike_damage_average"),
            strike_damage_scale   = Str(el, "strike_damage_scale"),
            spell_attack          = Mod(el, "spell_attack"),
            spell_attack_scale    = Str(el, "spell_attack_scale"),
            spell_dc              = Str(el, "spell_dc"),
            spell_dc_scale        = Str(el, "spell_dc_scale"),
            spell                 = JoinArray(el, "spell"),
            language              = JoinArray(el, "language"),
            strength              = Mod(el, "strength"),
            strength_scale        = Str(el, "strength_scale"),
            dexterity             = Mod(el, "dexterity"),
            dexterity_scale       = Str(el, "dexterity_scale"),
            constitution          = Mod(el, "constitution"),
            constitution_scale    = Str(el, "constitution_scale"),
            intelligence          = Mod(el, "intelligence"),
            intelligence_scale    = Str(el, "intelligence_scale"),
            wisdom                = Mod(el, "wisdom"),
            wisdom_scale          = Str(el, "wisdom_scale"),
            charisma              = Mod(el, "charisma"),
            charisma_scale        = Str(el, "charisma_scale"),
            skill                 = FormatSkillMod(el)
        }).ToList();
    }

    private async Task<List<object>> ScrapeRitualsAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("ritual", ct: ct);
        return hits.Select(el => (object)new
        {
            name              = Str(el, "name"),
            rank              = OrdinalRank(el),
            heighten          = JoinArray(el, "heighten"),
            school            = Cap(Str(el, "school")),
            trait             = JoinArray(el, "trait_raw"),
            primary_check     = Str(el, "primary_check"),
            secondary_casters = Str(el, "secondary_casters"),
            secondary_check   = JoinArray(el, "secondary_check"),
            cost              = Str(el, "cost_raw") is { Length: > 0 } c ? c : Str(el, "cost"),
            actions           = Str(el, "actions"),
            target            = Str(el, "target"),
            range             = Str(el, "range_raw"),
            area              = Str(el, "area_raw"),
            duration          = Str(el, "duration"),
            rarity            = Cap(Str(el, "rarity")),
            pfs               = Str(el, "pfs")
        }).ToList();
    }

    private async Task<List<object>> ScrapeSkillFeatsAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("feat", extraFilter: "trait_raw:Skill", ct: ct);
        return hits.Select(el => (object)new
        {
            name         = Str(el, "name"),
            pfs          = Str(el, "pfs"),
            source       = FirstArr(el, "source_raw") ?? Str(el, "primary_source_raw"),
            rarity       = Cap(Str(el, "rarity")),
            trait        = JoinArray(el, "trait_raw"),
            level        = Str(el, "level"),
            prerequisite = Str(el, "prerequisite"),
            summary      = Str(el, "summary"),
            spoilers     = ""
        }).ToList();
    }

    private async Task<List<object>> ScrapeTraitsAsync(CancellationToken ct)
    {
        var hits = await FetchAllAsync("trait", ct: ct);
        return hits.Select(el => (object)new
        {
            category    = FirstArr(el, "trait_group") ?? "",
            name        = Str(el, "name"),
            description = Str(el, "summary")
        }).ToList();
    }

    // ── Elasticsearch helpers ───────────────────────────────────────────────────

    private async Task<List<JsonElement>> FetchAllAsync(
        string category,
        string? extraFilter = null,
        CancellationToken ct = default)
    {
        using var client = _httpFactory.CreateClient("aon");
        var all = new List<JsonElement>();
        int from = 0;
        int? total = null;

        while (true)
        {
            var body = BuildQuery(category, extraFilter, from, PageSize);
            using var resp = await client.PostAsJsonAsync(SearchUrl, body, ct);
            resp.EnsureSuccessStatusCode();

            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            var hitsNode = doc.GetProperty("hits");

            total ??= hitsNode.GetProperty("total").GetProperty("value").GetInt32();

            var page = hitsNode.GetProperty("hits")
                .EnumerateArray()
                .Select(h => h.GetProperty("_source"))
                .ToList();

            all.AddRange(page);
            _logger.LogDebug("Fetched {Count}/{Total} {Category} records.", all.Count, total, category);

            if (page.Count < PageSize || all.Count >= total) break;
            from += PageSize;
        }

        return all;
    }

    private static object BuildQuery(string category, string? extraFilter, int from, int size)
    {
        var filters = new List<object>
        {
            new { term = new Dictionary<string, string> { ["category"] = category } }
        };

        if (!string.IsNullOrWhiteSpace(extraFilter))
            filters.Add(new { query_string = new { query = extraFilter } });

        return new
        {
            query = new { @bool = new { filter = filters } },
            from,
            size
        };
    }

    // ── Field extraction helpers ────────────────────────────────────────────────

    private static string Str(JsonElement el, string field)
    {
        if (!el.TryGetProperty(field, out var p)) return "";
        return p.ValueKind switch
        {
            JsonValueKind.String => p.GetString() ?? "",
            JsonValueKind.Number => p.ToString(),
            JsonValueKind.True   => "true",
            JsonValueKind.False  => "false",
            JsonValueKind.Null   => "",
            _                    => ""
        };
    }

    private static string JoinArray(JsonElement el, string field, string sep = ", ", bool cap = false)
    {
        if (!el.TryGetProperty(field, out var p) || p.ValueKind != JsonValueKind.Array)
            return "";
        var items = p.EnumerateArray()
            .Where(e => e.ValueKind == JsonValueKind.String)
            .Select(e => e.GetString() ?? "")
            .Where(s => s.Length > 0);
        if (cap) items = items.Select(Cap);
        return string.Join(sep, items);
    }

    private static string? FirstArr(JsonElement el, string field)
    {
        if (!el.TryGetProperty(field, out var p) || p.ValueKind != JsonValueKind.Array)
            return null;
        var first = p.EnumerateArray().FirstOrDefault();
        return first.ValueKind == JsonValueKind.String ? first.GetString() : null;
    }

    // Format an integer field as a signed modifier string: 5 → "+5", -1 → "-1", 0 → "+0"
    private static string Mod(JsonElement el, string field)
    {
        if (!el.TryGetProperty(field, out var p)) return "";
        if (p.ValueKind == JsonValueKind.String)
        {
            var s = p.GetString() ?? "";
            // Already formatted (e.g. "+5")
            if (s.StartsWith('+') || s.StartsWith('-') || s == "") return s;
            if (int.TryParse(s, out var n)) return n >= 0 ? $"+{n}" : $"{n}";
            return s;
        }
        if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var v))
            return v >= 0 ? $"+{v}" : $"{v}";
        return "";
    }

    // Format the first element of an integer array as a signed modifier
    private static string FirstIntArr(JsonElement el, string field)
    {
        if (!el.TryGetProperty(field, out var p) || p.ValueKind != JsonValueKind.Array)
            return "";
        var first = p.EnumerateArray().FirstOrDefault();
        if (first.ValueKind == JsonValueKind.Number && first.TryGetInt32(out var v))
            return v >= 0 ? $"+{v}" : $"{v}";
        if (first.ValueKind == JsonValueKind.String)
            return first.GetString() ?? "";
        return "";
    }

    // Convert resistance/weakness object {fire:5, cold:10} → "fire 5, cold 10"
    private static string FormatResistanceObj(JsonElement el, string field)
    {
        if (!el.TryGetProperty(field, out var p)) return "";
        if (p.ValueKind == JsonValueKind.String) return p.GetString() ?? "";
        if (p.ValueKind != JsonValueKind.Object) return "";
        var parts = new List<string>();
        foreach (var prop in p.EnumerateObject())
        {
            var val = prop.Value.ValueKind == JsonValueKind.Number
                ? prop.Value.ToString()
                : (prop.Value.GetString() ?? "");
            parts.Add(val.Length > 0 ? $"{prop.Name} {val}" : prop.Name);
        }
        return string.Join(", ", parts);
    }

    // Convert skill_mod object {acrobatics:5, athletics:2} → "Acrobatics +5, Athletics +2"
    private static string FormatSkillMod(JsonElement el)
    {
        if (!el.TryGetProperty("skill_mod", out var p) || p.ValueKind != JsonValueKind.Object)
            return JoinArray(el, "skill"); // fallback to skill array if present

        var parts = new List<string>();
        foreach (var prop in p.EnumerateObject())
        {
            var name = Cap(prop.Name);
            if (prop.Value.ValueKind == JsonValueKind.Number && prop.Value.TryGetInt32(out var v))
                parts.Add($"{name} {(v >= 0 ? $"+{v}" : $"{v}")}");
        }
        return string.Join(", ", parts);
    }

    // Format item price from numeric copper value or raw string
    private static string FormatPrice(JsonElement el)
    {
        if (el.TryGetProperty("price_raw", out var raw) && raw.ValueKind == JsonValueKind.String)
            return raw.GetString() ?? "";
        if (!el.TryGetProperty("price", out var p)) return "";
        if (p.ValueKind == JsonValueKind.String) return p.GetString() ?? "";
        if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var cp))
        {
            if (cp == 0) return "0 gp";
            if (cp % 1000 == 0) return $"{cp / 1000} pp";
            if (cp % 100 == 0) return $"{cp / 100} gp";
            if (cp % 10 == 0) return $"{cp / 10} sp";
            return $"{cp} cp";
        }
        return "";
    }

    // Convert level integer to ordinal rank string: 1→"1st", 2→"2nd", 3→"3rd", N→"Nth"
    private static string OrdinalRank(JsonElement el)
    {
        if (!el.TryGetProperty("level", out var p)) return "";
        int level;
        if (p.ValueKind == JsonValueKind.Number) level = p.GetInt32();
        else if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var n)) level = n;
        else return p.ValueKind == JsonValueKind.String ? p.GetString() ?? "" : "";

        return level switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => $"{level}th"
        };
    }

    private static string Cap(string s) =>
        s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s[1..];

    // ── Persistence ─────────────────────────────────────────────────────────────

    private async Task Save<T>(
        string dir, string file, Task<List<T>> scrapeTask,
        IProgress<string>? progress, string label, CancellationToken ct)
    {
        try
        {
            var items = await scrapeTask;
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(Path.Combine(dir, file), json, ct);
            var msg = $"{label}: {items.Count} records written to {file}.";
            progress?.Report(msg);
            _logger.LogInformation("{Message}", msg);
        }
        catch (Exception ex)
        {
            var msg = $"{label}: FAILED – {ex.Message}";
            progress?.Report(msg);
            _logger.LogError(ex, "Scrape failed for {Label}.", label);
        }
    }
}
