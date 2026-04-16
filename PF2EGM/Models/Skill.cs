using System.Text.Json.Serialization;

namespace PF2EGM.Models
{
    public class Skill
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("pfs")]
        public required string Pfs { get; set; }

        [JsonPropertyName("source")]
        public required string Source { get; set; }

        [JsonPropertyName("rarity")]
        public required string Rarity { get; set; }

        [JsonPropertyName("trait")]
        public required string Trait { get; set; }

        [JsonPropertyName("level")]
        public required string Level { get; set; }

        [JsonPropertyName("prerequisite")]
        public required string Prerequisite { get; set; }

        [JsonPropertyName("summary")]
        public required string Summary { get; set; }

        [JsonPropertyName("spoilers")]
        public required string Spoilers { get; set; }
    }
}