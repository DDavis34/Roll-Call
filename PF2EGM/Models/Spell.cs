using System.Text.Json.Serialization;

namespace PF2EGM.Models
{
    public class Spell
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("spell_type")]
        public required string SpellType { get; set; }

        [JsonPropertyName("rank")]
        public required string Rank { get; set; }

        [JsonPropertyName("heighten")]
        public required string Heighten { get; set; }

        [JsonPropertyName("tradition")]
        public required string Tradition { get; set; }

        [JsonPropertyName("school")]
        public required string School { get; set; }

        [JsonPropertyName("trait")]
        public required string Trait { get; set; }

        [JsonPropertyName("actions")]
        public required string Actions { get; set; }

        [JsonPropertyName("component")]
        public required string Component { get; set; }

        [JsonPropertyName("trigger")]
        public required string Trigger { get; set; }

        [JsonPropertyName("target")]
        public required string Target { get; set; }

        [JsonPropertyName("range")]
        public required string Range { get; set; }

        [JsonPropertyName("area")]
        public required string Area { get; set; }

        [JsonPropertyName("duration")]
        public required string Duration { get; set; }

        [JsonPropertyName("defense")]
        public required string Defense { get; set; }

        [JsonPropertyName("rarity")]
        public required string Rarity { get; set; }

        [JsonPropertyName("pfs")]
        public required string Pfs { get; set; }
    }
}