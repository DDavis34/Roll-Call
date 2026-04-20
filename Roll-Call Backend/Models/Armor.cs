using System.Text.Json.Serialization;

namespace RollCallBackend.Models;

public class Armor
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("armor_category")]
    public required string ArmorCategory { get; set; }

    [JsonPropertyName("ac")]
    public required string Ac { get; set; }

    [JsonPropertyName("dex_cap")]
    public required string DexCap { get; set; }

    [JsonPropertyName("check_penalty")]
    public required string CheckPenalty { get; set; }

    [JsonPropertyName("speed_penalty")]
    public required string SpeedPenalty { get; set; }

    [JsonPropertyName("strength")]
    public required string Strength { get; set; }

    [JsonPropertyName("bulk")]
    public required string Bulk { get; set; }

    [JsonPropertyName("armor_group")]
    public required string ArmorGroup { get; set; }

    [JsonPropertyName("trait")]
    public required string Trait { get; set; }
}
