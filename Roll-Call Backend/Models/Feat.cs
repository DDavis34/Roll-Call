using System.Text.Json.Serialization;

namespace RollCallBackend.Models;

public class Feat
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("level")]
    public required string Level { get; set; }

    [JsonPropertyName("trait")]
    public required string Trait { get; set; }

    [JsonPropertyName("prerequisite")]
    public required string Prerequisite { get; set; }

    [JsonPropertyName("summary")]
    public required string Summary { get; set; }

    [JsonPropertyName("rarity")]
    public required string Rarity { get; set; }

    [JsonPropertyName("pfs")]
    public required string Pfs { get; set; }

    [JsonPropertyName("source")]
    public required string Source { get; set; }

    [JsonPropertyName("actions")]
    public string Actions { get; set; } = "";

    [JsonPropertyName("trigger")]
    public string Trigger { get; set; } = "";

    [JsonPropertyName("requirements")]
    public string Requirements { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
}
