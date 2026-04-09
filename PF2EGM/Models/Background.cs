using System.Text.Json.Serialization;

namespace PF2EGM.Models
{
    public class Background
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("pfs")]
        public required string Pfs { get; set; }

        [JsonPropertyName("ability")]
        public required string Ability { get; set; }

        [JsonPropertyName("skill")]
        public required string Skill { get; set; }

        [JsonPropertyName("feat")]
        public required string Feat { get; set; }

        [JsonPropertyName("rarity")]
        public required string Rarity { get; set; }

        [JsonPropertyName("source")]
        public required string Source { get; set; }
    }
}