using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using RollCallBackend.Models;

namespace RollCallBackend.Services;

public class Scrapper
{
    private const string AonBase      = "https://2e.aonprd.com";
    private const string SearchUrl    = "https://elasticsearch.aonprd.com/aon/_search";
    private const int    EsPageSize   = 1000;
    private static readonly TimeSpan  RequestDelay = TimeSpan.FromMilliseconds(350);

    private readonly IHttpClientFactory    _httpFactory;
    private readonly IWebHostEnvironment   _env;
    private readonly ILogger<Scrapper>     _logger;

    public Scrapper(IHttpClientFactory httpFactory, IWebHostEnvironment env, ILogger<Scrapper> logger)
    {
        _httpFactory = httpFactory;
        _env         = env;
        _logger      = logger;
    }

    // ── Public API ───────────────────────────────────────────────────────────────

    public async Task ScrapeAllAsync(IProgress<string>? progress = null, CancellationToken ct = default)
    {
        var dir = Path.Combine(_env.ContentRootPath, "DataTables");

        // Sequential to be respectful to AoN's servers
        await Save(dir, "AncestriesDataTable.json",  ScrapeAncestriesAsync(progress, ct),  progress, "Ancestries",  ct);
        await Save(dir, "ClassesDataTable.json",      ScrapeClassesAsync(progress, ct),     progress, "Classes",     ct);
        await Save(dir, "FeatsDataTable.json",        ScrapeFeatsAsync(progress, ct),       progress, "Feats",       ct);
        await Save(dir, "SpellsDataTable.json",       ScrapeSpellsAsync(progress, ct),      progress, "Spells",      ct);
        await Save(dir, "BackgroundsDataTable.json",  ScrapeBackgroundsAsync(progress, ct), progress, "Backgrounds", ct);
        await Save(dir, "ArmorDataTable.json",        ScrapeArmorAsync(progress, ct),       progress, "Armor",       ct);
        await Save(dir, "ItemsDataTable.json",        ScrapeItemsAsync(progress, ct),       progress, "Items",       ct);
        await Save(dir, "CreaturesDataTable.json",    ScrapeCreaturesAsync(progress, ct),   progress, "Creatures",   ct);
        await Save(dir, "RitualsDataTable.json",      ScrapeRitualsAsync(progress, ct),     progress, "Rituals",     ct);
        await Save(dir, "SkillsDataTable.json",       ScrapeSkillFeatsAsync(progress, ct),  progress, "Skills",      ct);
        await Save(dir, "TraitsDataTable.json",       ScrapeTraitsAsync(progress, ct),      progress, "Traits",      ct);
    }

    public Task ScrapeAsync(string category, IProgress<string>? progress = null, CancellationToken ct = default)
    {
        var dir = Path.Combine(_env.ContentRootPath, "DataTables");
        return category.ToLowerInvariant() switch
        {
            "ancestries"  => Save(dir, "AncestriesDataTable.json",  ScrapeAncestriesAsync(progress, ct),  progress, "Ancestries",  ct),
            "classes"     => Save(dir, "ClassesDataTable.json",      ScrapeClassesAsync(progress, ct),     progress, "Classes",     ct),
            "feats"       => Save(dir, "FeatsDataTable.json",        ScrapeFeatsAsync(progress, ct),       progress, "Feats",       ct),
            "spells"      => Save(dir, "SpellsDataTable.json",       ScrapeSpellsAsync(progress, ct),      progress, "Spells",      ct),
            "backgrounds" => Save(dir, "BackgroundsDataTable.json",  ScrapeBackgroundsAsync(progress, ct), progress, "Backgrounds", ct),
            "armor"       => Save(dir, "ArmorDataTable.json",        ScrapeArmorAsync(progress, ct),       progress, "Armor",       ct),
            "items"       => Save(dir, "ItemsDataTable.json",        ScrapeItemsAsync(progress, ct),       progress, "Items",       ct),
            "creatures"   => Save(dir, "CreaturesDataTable.json",    ScrapeCreaturesAsync(progress, ct),   progress, "Creatures",   ct),
            "rituals"     => Save(dir, "RitualsDataTable.json",      ScrapeRitualsAsync(progress, ct),     progress, "Rituals",     ct),
            "skills"      => Save(dir, "SkillsDataTable.json",       ScrapeSkillFeatsAsync(progress, ct),  progress, "Skills",      ct),
            "traits"      => Save(dir, "TraitsDataTable.json",       ScrapeTraitsAsync(progress, ct),      progress, "Traits",      ct),
            _             => Task.FromException(new ArgumentException($"Unknown category: {category}"))
        };
    }

    // ── Persistence ──────────────────────────────────────────────────────────────

    private async Task Save<T>(
        string dir, string file, Task<List<T>> scrapeTask,
        IProgress<string>? progress, string label, CancellationToken ct)
    {
        try
        {
            var items = await scrapeTask;
            Directory.CreateDirectory(dir);
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

    // ── HTTP helpers ─────────────────────────────────────────────────────────────

    /// <summary>Fetch an AoN page by relative path (e.g. "Ancestries.aspx?ID=1").</summary>
    private async Task<HtmlDocument?> FetchHtmlAsync(string relativePath, CancellationToken ct)
    {
        try
        {
            // Use an unnamed client so we're not bound to the Elasticsearch base address
            using var client = _httpFactory.CreateClient();
            var fullUrl = $"{AonBase}/{relativePath.TrimStart('/')}";

            using var req = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            req.Headers.Add("User-Agent", "RollCallBot/1.0 (PF2e character creator research)");

            using var resp = await client.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("HTTP {Status} fetching {Url}", (int)resp.StatusCode, fullUrl);
                return null;
            }

            var html = await resp.Content.ReadAsStringAsync(ct);
            var doc  = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning("Failed to fetch {Path}: {Error}", relativePath, ex.Message);
            return null;
        }
    }

    // ── URL / ID discovery ───────────────────────────────────────────────────────

    /// <summary>
    /// Parses a static HTML list page (e.g. Ancestries.aspx) for ?ID= links.
    /// Works for any category whose list page renders links server-side.
    /// </summary>
    private async Task<List<int>> DiscoverIdsFromListPageAsync(
        string listPath, string categoryAspx, CancellationToken ct)
    {
        var doc = await FetchHtmlAsync(listPath, ct);
        if (doc is null) return [];

        var pattern = categoryAspx.Replace(".aspx", "", StringComparison.OrdinalIgnoreCase);
        var links   = doc.DocumentNode.SelectNodes($"//a[contains(@href,'{pattern}.aspx?ID=')]");
        if (links is null) return [];

        var ids = new HashSet<int>();
        foreach (var a in links)
        {
            var href  = a.GetAttributeValue("href", "");
            var match = Regex.Match(href, @"\?ID=(\d+)", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out var id))
                ids.Add(id);
        }

        return [.. ids.Order()];
    }

    /// <summary>
    /// Uses the AoN Elasticsearch API to discover page URLs for categories whose
    /// list pages are JS-rendered. Only the "url" field is requested — no game data.
    /// </summary>
    private async Task<List<string>> DiscoverUrlsFromApiAsync(
        string category, CancellationToken ct, string? extraFilter = null)
    {
        using var client = _httpFactory.CreateClient("aon");
        var all  = new List<string>();
        int from = 0;
        int? total = null;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var filters = new List<object>
            {
                new { term = new Dictionary<string, string> { ["category"] = category } }
            };
            if (!string.IsNullOrWhiteSpace(extraFilter))
                filters.Add(new { query_string = new { query = extraFilter } });

            var body = new
            {
                query   = new { @bool = new { filter = filters } },
                _source = new[] { "url" },
                from,
                size    = EsPageSize
            };

            using var resp = await client.PostAsJsonAsync(SearchUrl, body, ct);
            resp.EnsureSuccessStatusCode();

            var json     = await resp.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            var hitsNode = json.GetProperty("hits");
            total ??= hitsNode.GetProperty("total").GetProperty("value").GetInt32();

            var page = hitsNode.GetProperty("hits").EnumerateArray().ToList();
            foreach (var hit in page)
            {
                if (hit.GetProperty("_source").TryGetProperty("url", out var urlProp)
                    && urlProp.ValueKind == JsonValueKind.String)
                {
                    var url = urlProp.GetString() ?? "";
                    if (url.Length > 0) all.Add(url);
                }
            }

            _logger.LogDebug("Discovered {Count}/{Total} {Category} URLs.", all.Count, total, category);
            if (page.Count < EsPageSize || all.Count >= total) break;
            from += EsPageSize;
        }

        return all.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    // ── HTML extraction helpers ──────────────────────────────────────────────────

    /// <summary>Returns the #main div, which contains all page content.</summary>
    private static HtmlNode? GetContentNode(HtmlDocument doc) =>
        doc.DocumentNode.SelectSingleNode("//div[@id='main']");

    /// <summary>Name from the first h1.title link (or plain text if no link).</summary>
    private static string GetH1Name(HtmlNode content)
    {
        var h1 = content.SelectSingleNode(".//h1[@class='title']");
        if (h1 is null) return "";

        var link = h1.SelectSingleNode(".//a");
        if (link is not null) return CleanText(link.InnerText);

        // Strip the level/rank span ("Feat 1", "Spell 4") from the end
        var levelSpan = h1.SelectSingleNode(".//span[@style]");
        if (levelSpan is not null)
        {
            var full  = CleanText(h1.InnerText);
            var level = CleanText(levelSpan.InnerText);
            return full.Replace(level, "").Trim();
        }

        return CleanText(h1.InnerText);
    }

    /// <summary>
    /// Finds &lt;h2 class="title"&gt;sectionName&lt;/h2&gt; and collects all sibling
    /// text until the next h1/h2/hr, treating &lt;br&gt; as newlines.
    /// </summary>
    private static string GetH2SectionText(HtmlNode content, string sectionName)
    {
        var h2Nodes = content.SelectNodes(".//h2[@class='title']");
        if (h2Nodes is null) return "";

        foreach (var h2 in h2Nodes)
        {
            if (!CleanText(h2.InnerText).Equals(sectionName, StringComparison.OrdinalIgnoreCase))
                continue;
            return CollectSiblingText(h2);
        }

        return "";
    }

    /// <summary>
    /// Collects text from siblings of startNode until the next h1, h2, or hr.
    /// &lt;br&gt; nodes become newlines.
    /// </summary>
    private static string CollectSiblingText(HtmlNode startNode)
    {
        var sb   = new StringBuilder();
        var node = startNode.NextSibling;

        while (node is not null)
        {
            if (node.Name is "h1" or "h2" or "hr") break;

            if (node.Name == "br")
            {
                if (sb.Length > 0) sb.Append('\n');
            }
            else if (node.NodeType == HtmlNodeType.Text)
            {
                var t = HtmlEntity.DeEntitize(node.InnerText);
                if (!string.IsNullOrWhiteSpace(t)) sb.Append(t.Trim());
            }
            else
            {
                var t = CleanText(node.InnerText);
                if (t.Length > 0)
                {
                    if (sb.Length > 0 && sb[^1] != '\n') sb.Append(' ');
                    sb.Append(t);
                }
            }

            node = node.NextSibling;
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Finds &lt;b&gt;label&lt;/b&gt; and collects following siblings until the next
    /// &lt;b&gt;, h1, h2, or hr.  Action icon spans are converted to their title text.
    /// </summary>
    private static string GetBoldFieldText(HtmlNode content, string label)
    {
        var boldNodes = content.SelectNodes(".//b");
        if (boldNodes is null) return "";

        foreach (var b in boldNodes)
        {
            if (!CleanText(b.InnerText).Equals(label, StringComparison.OrdinalIgnoreCase))
                continue;

            var sb   = new StringBuilder();
            var node = b.NextSibling;

            while (node is not null)
            {
                if (node.Name is "b" or "hr" or "h1" or "h2") break;

                if (node.Name == "br")
                {
                    if (sb.Length > 0) sb.Append('\n');
                }
                else if (node.Name == "span" &&
                         node.GetAttributeValue("class", "").Contains("action"))
                {
                    var title = node.GetAttributeValue("title", "");
                    if (title.Length > 0) sb.Append(title);
                }
                else if (node.NodeType == HtmlNodeType.Text)
                {
                    var t = HtmlEntity.DeEntitize(node.InnerText).Trim();
                    if (t.Length > 0) sb.Append(t);
                }
                else
                {
                    var t = CleanText(node.InnerText);
                    if (t.Length > 0)
                    {
                        if (sb.Length > 0 && sb[^1] != ' ' && sb[^1] != '\n') sb.Append(' ');
                        sb.Append(t);
                    }
                }

                node = node.NextSibling;
            }

            return sb.ToString().Trim().TrimStart(':').Trim();
        }

        return "";
    }

    /// <summary>All trait link texts inside span.trait elements.</summary>
    private static List<string> GetTraits(HtmlNode content)
    {
        var nodes = content.SelectNodes(".//span[contains(@class,'trait')]//a");
        if (nodes is null) return [];
        return nodes.Select(a => CleanText(a.InnerText)).Where(s => s.Length > 0).ToList();
    }

    /// <summary>Text of the first source book link after &lt;b&gt;Source&lt;/b&gt;.</summary>
    private static string GetSource(HtmlNode content)
    {
        var boldNodes = content.SelectNodes(".//b");
        if (boldNodes is null) return "";

        foreach (var b in boldNodes)
        {
            if (!CleanText(b.InnerText).StartsWith("Source", StringComparison.OrdinalIgnoreCase))
                continue;

            var parts = new List<string>();
            var node  = b.NextSibling;

            while (node is not null)
            {
                if (node.Name is "b" or "hr" or "h1" or "h2" or "br") break;

                if (node.Name is "a" or "i")
                {
                    var t = CleanText(node.InnerText);
                    if (t.Length > 0) parts.Add(t);
                }

                node = node.NextSibling;
            }

            return string.Join(", ", parts);
        }

        return "";
    }

    /// <summary>
    /// Determines rarity from trait spans (rare/uncommon/unique CSS classes) or
    /// falls back to checking trait text.  Defaults to "Common".
    /// </summary>
    private static string GetRarity(HtmlNode content)
    {
        // AoN marks rarity with CSS classes on the trait span
        var rare = content.SelectSingleNode(
            ".//span[contains(@class,'trait-rare') or contains(@class,'trait-uncommon') or contains(@class,'trait-unique')]");
        if (rare is not null) return CleanText(rare.InnerText);

        var rarities = new[] { "Rare", "Uncommon", "Unique" };
        var found    = GetTraits(content).FirstOrDefault(t =>
            rarities.Contains(t, StringComparer.OrdinalIgnoreCase));

        return found ?? "Common";
    }

    /// <summary>All text after the first &lt;hr&gt; up to the next h1/h2/hr.</summary>
    private static string GetDescriptionAfterHr(HtmlNode content)
    {
        var hr = content.SelectSingleNode(".//hr");
        if (hr is null) return "";

        var sb   = new StringBuilder();
        var node = hr.NextSibling;

        while (node is not null)
        {
            if (node.Name is "h1" or "h2" or "hr") break;

            if (node.Name == "br")
            {
                sb.Append('\n');
            }
            else if (node.Name == "p")
            {
                if (sb.Length > 0) sb.Append('\n');
                sb.Append(CleanText(node.InnerText));
            }
            else if (node.NodeType == HtmlNodeType.Text)
            {
                var t = HtmlEntity.DeEntitize(node.InnerText);
                if (!string.IsNullOrWhiteSpace(t)) sb.Append(t);
            }
            else
            {
                var t = CleanText(node.InnerText);
                if (t.Length > 0) sb.Append(t);
            }

            node = node.NextSibling;
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Action cost from the h1 title's action span (e.g. "Two Actions", "Reaction").
    /// </summary>
    private static string GetActionType(HtmlNode content)
    {
        var h1         = content.SelectSingleNode(".//h1[@class='title']");
        var actionSpan = h1?.SelectSingleNode(".//span[contains(@class,'action')]");
        return actionSpan?.GetAttributeValue("title", "") ?? "";
    }

    /// <summary>
    /// Extracts the numeric level/rank from the h1 trailing span ("Feat 1" → "1").
    /// typeLabel is the word before the number (Feat, Spell, Item, Ritual, Creature).
    /// </summary>
    private static string GetLevelFromH1(HtmlNode content, string typeLabel)
    {
        var h1        = content.SelectSingleNode(".//h1[@class='title']");
        var levelSpan = h1?.SelectSingleNode(".//span[@style]");
        if (levelSpan is null) return "";

        var text  = CleanText(levelSpan.InnerText);
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 && int.TryParse(parts[^1], out var n) ? n.ToString() : text;
    }

    /// <summary>Spell type from the h1 trailing span ("Focus Spell 2" → "Focus").</summary>
    private static string GetSpellType(HtmlNode content)
    {
        var h1        = content.SelectSingleNode(".//h1[@class='title']");
        var levelSpan = h1?.SelectSingleNode(".//span[@style]");
        if (levelSpan is null) return "Spell";

        var text = CleanText(levelSpan.InnerText);
        if (text.Contains("Focus",   StringComparison.OrdinalIgnoreCase)) return "Focus";
        if (text.Contains("Cantrip", StringComparison.OrdinalIgnoreCase)) return "Cantrip";
        return "Spell";
    }

    /// <summary>Primary sense from known vision h2 sections, defaults to "Normal Vision".</summary>
    private static string GetVision(HtmlNode content)
    {
        foreach (var v in new[] { "Darkvision", "Low-Light Vision", "Greater Darkvision" })
            if (GetH2SectionText(content, v).Length > 0 ||
                content.SelectSingleNode($".//h2[@class='title' and normalize-space()='{v}']") is not null)
                return v;
        return "Normal Vision";
    }

    /// <summary>
    /// Gathers named h2 sections that appear after the first &lt;hr&gt; and are not
    /// standard ancestry mechanics headings — these are the special racial abilities.
    /// </summary>
    private static List<NamedAbility> GetSpecialAbilities(HtmlNode content)
    {
        var hr = content.SelectSingleNode(".//hr");
        if (hr is null) return [];

        var mechanicSections = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Hit Points", "Size", "Speed", "Attribute Boosts", "Attribute Flaws",
            "Languages", "Additional Languages"
        };

        var results = new List<NamedAbility>();
        var h2Nodes = content.SelectNodes(".//h2[@class='title']");
        if (h2Nodes is null) return results;

        foreach (var h2 in h2Nodes)
        {
            var sectionName = CleanText(h2.InnerText);
            if (mechanicSections.Contains(sectionName)) continue;
            if (h2.Line <= hr.Line) continue;  // only h2s after the <hr>

            var text = CollectSiblingText(h2);
            if (text.Length > 0)
                results.Add(new NamedAbility { Name = sectionName, Description = text });
        }

        return results;
    }

    /// <summary>
    /// Finds the "Initial Proficiencies" h1, then locates the requested h2 beneath it.
    /// </summary>
    private static string GetInitialProficiencySection(HtmlNode content, string sectionName)
    {
        var h1Nodes = content.SelectNodes(".//h1[@class='title']");
        if (h1Nodes is null) return "";

        HtmlNode? profH1 = h1Nodes.FirstOrDefault(h =>
            CleanText(h.InnerText).Contains("Initial Proficiencie", StringComparison.OrdinalIgnoreCase));
        if (profH1 is null) return "";

        var node = profH1.NextSibling;
        while (node is not null)
        {
            if (node.Name == "h1") break;
            if (node.Name == "h2" &&
                node.GetAttributeValue("class", "") == "title" &&
                CleanText(node.InnerText).Equals(sectionName, StringComparison.OrdinalIgnoreCase))
                return CollectSiblingText(node);
            node = node.NextSibling;
        }

        return "";
    }

    /// <summary>Extracts a specific save from the "Saving Throws" proficiency block.</summary>
    private static string GetSavingThrowProficiency(HtmlNode content, string saveName)
    {
        var block = GetInitialProficiencySection(content, "Saving Throws");
        return block.Split('\n')
                    .Select(l => l.Trim())
                    .FirstOrDefault(l => l.Contains(saveName, StringComparison.OrdinalIgnoreCase))
               ?? "";
    }

    /// <summary>Parses the class features table (Level | Features columns).</summary>
    private static List<ClassFeatureEntry> GetClassFeatures(HtmlNode content)
    {
        var features = new List<ClassFeatureEntry>();
        var tables   = content.SelectNodes(".//table[@class='inner']");
        if (tables is null) return features;

        foreach (var table in tables)
        {
            var headerCells = table.SelectNodes(".//tr[1]/th | .//tr[1]/td");
            if (headerCells is null || headerCells.Count < 2) continue;
            if (!CleanText(headerCells[0].InnerText).Equals("Level", StringComparison.OrdinalIgnoreCase))
                continue;

            var rows = table.SelectNodes(".//tr");
            if (rows is null) break;

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes(".//td");
                if (cells is null || cells.Count < 2) continue;
                var lvl = CleanText(cells[0].InnerText);
                var fts = CleanText(cells[1].InnerText);
                if (lvl.Length > 0)
                    features.Add(new ClassFeatureEntry { Level = lvl, Features = fts });
            }

            break;
        }

        return features;
    }

    /// <summary>
    /// Collects all "Heightened (Xth)" bold-field entries into a single string.
    /// </summary>
    private static string GetHeightenedText(HtmlNode content)
    {
        var boldNodes = content.SelectNodes(".//b");
        if (boldNodes is null) return "";

        var sb = new StringBuilder();
        foreach (var b in boldNodes)
        {
            var label = CleanText(b.InnerText);
            if (!label.StartsWith("Heightened", StringComparison.OrdinalIgnoreCase)) continue;

            var text = GetBoldFieldText(content, label);
            if (sb.Length > 0) sb.Append('\n');
            sb.Append($"{label} {text}");
        }

        return sb.ToString().Trim();
    }

    /// <summary>First sentence of the post-hr description, used as feat summary.</summary>
    private static string GetFirstSentenceOfDescription(HtmlNode content)
    {
        var desc = GetDescriptionAfterHr(content);
        if (desc.Length == 0) return "";
        var dot = desc.IndexOf('.');
        return dot >= 0 ? desc[..(dot + 1)] : desc[..Math.Min(desc.Length, 200)];
    }

    /// <summary>Decode HTML entities, collapse whitespace, and trim.</summary>
    private static string CleanText(string raw)
    {
        var text = HtmlEntity.DeEntitize(raw);
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    // ── Per-category scrape methods ──────────────────────────────────────────────

    private async Task<List<Ancestry>> ScrapeAncestriesAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var ids = await DiscoverIdsFromListPageAsync("Ancestries.aspx", "Ancestries.aspx", ct);
        progress?.Report($"Ancestries: found {ids.Count} IDs.");
        var results = new List<Ancestry>();

        foreach (var id in ids)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync($"Ancestries.aspx?ID={id}", ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try
            {
                var a = ParseAncestry(content);
                results.Add(a);
                progress?.Report($"Ancestries: scraped '{a.Name}'.");
            }
            catch (Exception ex) { _logger.LogWarning("Ancestry ID={Id}: {Error}", id, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        return results;
    }

    private static Ancestry ParseAncestry(HtmlNode content) => new()
    {
        Name              = GetH1Name(content),
        HitPointsBase     = GetH2SectionText(content, "Hit Points"),
        Size              = GetH2SectionText(content, "Size"),
        Speed             = GetH2SectionText(content, "Speed"),
        AbilityBoost      = GetH2SectionText(content, "Attribute Boosts"),
        AbilityFlaw       = GetH2SectionText(content, "Attribute Flaws"),
        Language          = GetH2SectionText(content, "Languages"),
        AdditionalLanguages = GetH2SectionText(content, "Additional Languages"),
        Vision            = GetVision(content),
        Rarity            = GetRarity(content),
        Pfs               = "",
        Source            = GetSource(content),
        Traits            = GetTraits(content),
        Description       = GetDescriptionAfterHr(content),
        SpecialAbilities  = GetSpecialAbilities(content),
    };

    // ─────────────────────────────────────────────────────────────────────────────

    private async Task<List<Class>> ScrapeClassesAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var ids = await DiscoverIdsFromListPageAsync("Classes.aspx", "Classes.aspx", ct);
        progress?.Report($"Classes: found {ids.Count} IDs.");
        var results = new List<Class>();

        foreach (var id in ids)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync($"Classes.aspx?ID={id}", ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try
            {
                var c = ParseClass(content);
                results.Add(c);
                progress?.Report($"Classes: scraped '{c.Name}'.");
            }
            catch (Exception ex) { _logger.LogWarning("Class ID={Id}: {Error}", id, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        return results;
    }

    private static Class ParseClass(HtmlNode content)
    {
        var classDc = GetH2SectionText(content, "Class DC");
        if (classDc.Length == 0) classDc = GetBoldFieldText(content, "Class DC");

        return new Class
        {
            Name                 = GetH1Name(content),
            Ability              = GetH2SectionText(content, "Key Attribute"),
            Hp                   = GetH2SectionText(content, "Hit Points"),
            AttackProficiency    = GetInitialProficiencySection(content, "Attacks"),
            DefenseProficiency   = GetInitialProficiencySection(content, "Defenses"),
            FortitudeProficiency = GetSavingThrowProficiency(content, "Fortitude"),
            ReflexProficiency    = GetSavingThrowProficiency(content, "Reflex"),
            WillProficiency      = GetSavingThrowProficiency(content, "Will"),
            PerceptionProficiency= GetInitialProficiencySection(content, "Perception"),
            SkillProficiency     = GetInitialProficiencySection(content, "Skills"),
            Rarity               = GetRarity(content),
            Pfs                  = "",
            Source               = GetSource(content),
            Description          = GetDescriptionAfterHr(content),
            ClassDc              = classDc,
            ClassFeatures        = GetClassFeatures(content),
        };
    }

    // ─────────────────────────────────────────────────────────────────────────────

    private async Task<List<Feat>> ScrapeFeatsAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = await DiscoverUrlsFromApiAsync("feat", ct);
        progress?.Report($"Feats: found {urls.Count} URLs.");
        var results = new List<Feat>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try { results.Add(ParseFeat(content)); }
            catch (Exception ex) { _logger.LogWarning("Feat {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Feats: scraped {results.Count} feats.");
        return results;
    }

    private static Feat ParseFeat(HtmlNode content)
    {
        var traits = GetTraits(content);
        return new Feat
        {
            Name         = GetH1Name(content),
            Level        = GetLevelFromH1(content, "Feat"),
            Trait        = string.Join(", ", traits),
            Prerequisite = GetBoldFieldText(content, "Prerequisites"),
            Summary      = GetFirstSentenceOfDescription(content),
            Rarity       = GetRarity(content),
            Pfs          = "",
            Source       = GetSource(content),
            Actions      = GetActionType(content),
            Trigger      = GetBoldFieldText(content, "Trigger"),
            Requirements = GetBoldFieldText(content, "Requirements"),
            Description  = GetDescriptionAfterHr(content),
        };
    }

    // ─────────────────────────────────────────────────────────────────────────────

    private async Task<List<Spell>> ScrapeSpellsAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var cat in new[] { "spell", "focus" })
            foreach (var u in await DiscoverUrlsFromApiAsync(cat, ct))
                urls.Add(u);

        progress?.Report($"Spells: found {urls.Count} URLs.");
        var results = new List<Spell>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try { results.Add(ParseSpell(content)); }
            catch (Exception ex) { _logger.LogWarning("Spell {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Spells: scraped {results.Count} spells.");
        return results;
    }

    private static Spell ParseSpell(HtmlNode content) => new()
    {
        Name          = GetH1Name(content),
        SpellType     = GetSpellType(content),
        Rank          = GetLevelFromH1(content, "Spell"),
        Heighten      = GetBoldFieldText(content, "Heightened"),
        Tradition     = GetBoldFieldText(content, "Traditions"),
        School        = GetBoldFieldText(content, "School"),
        Trait         = string.Join(", ", GetTraits(content)),
        Actions       = GetActionType(content),
        Component     = GetBoldFieldText(content, "Cast"),
        Trigger       = GetBoldFieldText(content, "Trigger"),
        Target        = GetBoldFieldText(content, "Targets"),
        Range         = GetBoldFieldText(content, "Range"),
        Area          = GetBoldFieldText(content, "Area"),
        Duration      = GetBoldFieldText(content, "Duration"),
        Defense       = GetBoldFieldText(content, "Defense"),
        Rarity        = GetRarity(content),
        Pfs           = "",
        Source        = GetSource(content),
        Description   = GetDescriptionAfterHr(content),
        HeightenedText= GetHeightenedText(content),
    };

    // ─────────────────────────────────────────────────────────────────────────────

    private async Task<List<Background>> ScrapeBackgroundsAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = await DiscoverUrlsFromApiAsync("background", ct);
        progress?.Report($"Backgrounds: found {urls.Count} URLs.");
        var results = new List<Background>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try { results.Add(ParseBackground(content)); }
            catch (Exception ex) { _logger.LogWarning("Background {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Backgrounds: scraped {results.Count} backgrounds.");
        return results;
    }

    private static Background ParseBackground(HtmlNode content) => new()
    {
        Name        = GetH1Name(content),
        Pfs         = "",
        Ability     = GetBoldFieldText(content, "Attribute Boosts"),
        Skill       = GetBoldFieldText(content, "Skills") is { Length: > 0 } s ? s
                        : GetBoldFieldText(content, "Skill"),
        Feat        = GetBoldFieldText(content, "Feat"),
        Rarity      = GetRarity(content),
        Source      = GetSource(content),
        Description = GetDescriptionAfterHr(content),
    };

    // ── Armor ────────────────────────────────────────────────────────────────────

    private async Task<List<object>> ScrapeArmorAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = await DiscoverUrlsFromApiAsync("armor", ct);
        progress?.Report($"Armor: found {urls.Count} URLs.");
        var results = new List<object>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try { results.Add(ParseArmor(content)); }
            catch (Exception ex) { _logger.LogWarning("Armor {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Armor: scraped {results.Count} items.");
        return results;
    }

    private static object ParseArmor(HtmlNode content) => new
    {
        name           = GetH1Name(content),
        armor_category = GetBoldFieldText(content, "Category"),
        ac             = GetBoldFieldText(content, "AC Bonus"),
        dex_cap        = GetBoldFieldText(content, "Dex Cap"),
        check_penalty  = GetBoldFieldText(content, "Check Penalty"),
        speed_penalty  = GetBoldFieldText(content, "Speed Penalty"),
        strength       = GetBoldFieldText(content, "Strength"),
        bulk           = GetBoldFieldText(content, "Bulk"),
        armor_group    = GetBoldFieldText(content, "Armor Group"),
        trait          = string.Join(", ", GetTraits(content)),
        source         = GetSource(content),
        description    = GetDescriptionAfterHr(content),
    };

    // ── Items ─────────────────────────────────────────────────────────────────────

    private async Task<List<object>> ScrapeItemsAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = await DiscoverUrlsFromApiAsync("equipment", ct);
        progress?.Report($"Items: found {urls.Count} URLs.");
        var results = new List<object>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try { results.Add(ParseItem(content)); }
            catch (Exception ex) { _logger.LogWarning("Item {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Items: scraped {results.Count} items.");
        return results;
    }

    private static object ParseItem(HtmlNode content) => new
    {
        name             = GetH1Name(content),
        item_category    = GetBoldFieldText(content, "Category"),
        item_subcategory = GetBoldFieldText(content, "Subcategory"),
        level            = GetLevelFromH1(content, "Item"),
        price            = GetBoldFieldText(content, "Price"),
        bulk             = GetBoldFieldText(content, "Bulk"),
        trait            = string.Join(", ", GetTraits(content)),
        rarity           = GetRarity(content),
        source           = GetSource(content),
        description      = GetDescriptionAfterHr(content),
    };

    // ── Creatures ─────────────────────────────────────────────────────────────────

    private async Task<List<object>> ScrapeCreaturesAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = await DiscoverUrlsFromApiAsync("creature", ct);
        progress?.Report($"Creatures: found {urls.Count} URLs.");
        var results = new List<object>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try { results.Add(ParseCreature(content)); }
            catch (Exception ex) { _logger.LogWarning("Creature {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Creatures: scraped {results.Count} creatures.");
        return results;
    }

    private static object ParseCreature(HtmlNode content) => new
    {
        name            = GetH1Name(content),
        level           = GetLevelFromH1(content, "Creature"),
        creature_family = GetBoldFieldText(content, "Family"),
        source          = GetSource(content),
        rarity          = GetRarity(content),
        size            = GetBoldFieldText(content, "Size"),
        trait           = string.Join(", ", GetTraits(content)),
        hp              = GetBoldFieldText(content, "HP"),
        ac              = GetBoldFieldText(content, "AC"),
        fortitude       = GetBoldFieldText(content, "Fort"),
        reflex          = GetBoldFieldText(content, "Ref"),
        will            = GetBoldFieldText(content, "Will"),
        perception      = GetBoldFieldText(content, "Perception"),
        speed           = GetBoldFieldText(content, "Speed"),
        immunity        = GetBoldFieldText(content, "Immunities"),
        resistance      = GetBoldFieldText(content, "Resistances"),
        weakness        = GetBoldFieldText(content, "Weaknesses"),
        language        = GetBoldFieldText(content, "Languages"),
        description     = GetDescriptionAfterHr(content),
    };

    // ── Rituals ───────────────────────────────────────────────────────────────────

    private async Task<List<object>> ScrapeRitualsAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = await DiscoverUrlsFromApiAsync("ritual", ct);
        progress?.Report($"Rituals: found {urls.Count} URLs.");
        var results = new List<object>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try { results.Add(ParseRitual(content)); }
            catch (Exception ex) { _logger.LogWarning("Ritual {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Rituals: scraped {results.Count} rituals.");
        return results;
    }

    private static object ParseRitual(HtmlNode content) => new
    {
        name               = GetH1Name(content),
        rank               = GetLevelFromH1(content, "Ritual"),
        heighten           = GetBoldFieldText(content, "Heightened"),
        school             = GetBoldFieldText(content, "School"),
        trait              = string.Join(", ", GetTraits(content)),
        primary_check      = GetBoldFieldText(content, "Primary Check"),
        secondary_casters  = GetBoldFieldText(content, "Secondary Casters"),
        secondary_check    = GetBoldFieldText(content, "Secondary Checks"),
        cost               = GetBoldFieldText(content, "Cost"),
        actions            = GetBoldFieldText(content, "Cast"),
        target             = GetBoldFieldText(content, "Targets"),
        range              = GetBoldFieldText(content, "Range"),
        area               = GetBoldFieldText(content, "Area"),
        duration           = GetBoldFieldText(content, "Duration"),
        rarity             = GetRarity(content),
        source             = GetSource(content),
        description        = GetDescriptionAfterHr(content),
    };

    // ── Skill feats ───────────────────────────────────────────────────────────────

    private async Task<List<object>> ScrapeSkillFeatsAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = await DiscoverUrlsFromApiAsync("feat", ct, extraFilter: "trait_raw:Skill");
        progress?.Report($"Skills: found {urls.Count} URLs.");
        var results = new List<object>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try
            {
                var f = ParseFeat(content);
                results.Add(new
                {
                    name         = f.Name,
                    pfs          = f.Pfs,
                    source       = f.Source,
                    rarity       = f.Rarity,
                    trait        = f.Trait,
                    level        = f.Level,
                    prerequisite = f.Prerequisite,
                    summary      = f.Summary,
                    description  = f.Description,
                    spoilers     = "",
                });
            }
            catch (Exception ex) { _logger.LogWarning("Skill feat {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Skills: scraped {results.Count} skill feats.");
        return results;
    }

    // ── Traits ────────────────────────────────────────────────────────────────────

    private async Task<List<object>> ScrapeTraitsAsync(IProgress<string>? progress, CancellationToken ct)
    {
        var urls = await DiscoverUrlsFromApiAsync("trait", ct);
        progress?.Report($"Traits: found {urls.Count} URLs.");
        var results = new List<object>();

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();
            var doc = await FetchHtmlAsync(url, ct);
            var content = doc is not null ? GetContentNode(doc) : null;
            if (content is null) { await Task.Delay(RequestDelay, ct); continue; }

            try { results.Add(ParseTrait(content)); }
            catch (Exception ex) { _logger.LogWarning("Trait {Url}: {Error}", url, ex.Message); }

            await Task.Delay(RequestDelay, ct);
        }

        progress?.Report($"Traits: scraped {results.Count} traits.");
        return results;
    }

    private static object ParseTrait(HtmlNode content) => new
    {
        category    = GetBoldFieldText(content, "Category"),
        name        = GetH1Name(content),
        description = GetDescriptionAfterHr(content),
    };
}
