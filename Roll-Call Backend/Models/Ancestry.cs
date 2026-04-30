using System.Text.Json.Serialization;

namespace RollCallBackend.Models;

public class Ancestry
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("hp")]
    public required string HitPointsBase { get; set; }

    [JsonPropertyName("size")]
    public required string Size { get; set; }

    [JsonPropertyName("speed")]
    public required string Speed { get; set; }

    [JsonPropertyName("ability_boost")]
    public required string AbilityBoost { get; set; }

    [JsonPropertyName("ability_flaw")]
    public required string AbilityFlaw { get; set; }

    [JsonPropertyName("language")]
    public required string Language { get; set; }

    [JsonPropertyName("additional_languages")]
    public string AdditionalLanguages { get; set; } = "";

    [JsonPropertyName("vision")]
    public required string Vision { get; set; }

    [JsonPropertyName("rarity")]
    public required string Rarity { get; set; }

    [JsonPropertyName("pfs")]
    public required string Pfs { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = "";

    [JsonPropertyName("traits")]
    public List<string> Traits { get; set; } = [];

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("special_abilities")]
    public List<NamedAbility> SpecialAbilities { get; set; } = [];

    public List<Feat> AncestryFeats { get; set; } = [];
    public List<Heritage> Heritages { get; set; } = [];
    public List<Ability> Abilities { get; set; } = [];
}

public class NamedAbility
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
}
