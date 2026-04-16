using System.Text.Json.Serialization;

namespace PF2EGM.Models
{
    public class Ritual
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("rank")]
        public required string Rank { get; set; }

        [JsonPropertyName("heighten")]
        public required string Heighten { get; set; }

        [JsonPropertyName("school")]
        public required string School { get; set; }

        [JsonPropertyName("trait")]
        public required string Trait { get; set; }

        [JsonPropertyName("primary_check")]
        public required string PrimaryCheck { get; set; }

        [JsonPropertyName("secondary_casters")]
        public required string SecondaryCasters { get; set; }

        [JsonPropertyName("secondary_check")]
        public required string SecondaryCheck { get; set; }

        [JsonPropertyName("cost")]
        public required string Cost { get; set; }

        [JsonPropertyName("actions")]
        public required string Actions { get; set; }

        [JsonPropertyName("target")]
        public required string Target { get; set; }

        [JsonPropertyName("range")]
        public required string Range { get; set; }

        [JsonPropertyName("area")]
        public required string Area { get; set; }

        [JsonPropertyName("duration")]
        public required string Duration { get; set; }

        [JsonPropertyName("rarity")]
        public required string Rarity { get; set; }

        [JsonPropertyName("pfs")]
        public required string Pfs { get; set; }
    }
}