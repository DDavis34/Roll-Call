using System.Text.Json.Serialization;

namespace PF2EGM.Models
{
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

        [JsonPropertyName("vision")]
        public required string Vision { get; set; }

        [JsonPropertyName("rarity")]
        public required string Rarity { get; set; }

        [JsonPropertyName("pfs")]
        public required string Pfs { get; set; }
        public List<Feat> AncestryFeats { get; set; }
        public List<Heritage> Heritages { get; set; }
        public List<Ability> Abilities { get; set; }
    }
}