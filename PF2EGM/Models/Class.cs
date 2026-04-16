using System.Text.Json.Serialization;

namespace PF2EGM.Models
{
    public class Class
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("ability")]
        public required string Ability { get; set; }

        [JsonPropertyName("hp")]
        public required string Hp { get; set; }

        [JsonPropertyName("attack_proficiency")]
        public required string AttackProficiency { get; set; }

        [JsonPropertyName("defense_proficiency")]
        public required string DefenseProficiency { get; set; }

        [JsonPropertyName("fortitude_proficiency")]
        public required string FortitudeProficiency { get; set; }

        [JsonPropertyName("reflex_proficiency")]
        public required string ReflexProficiency { get; set; }

        [JsonPropertyName("will_proficiency")]
        public required string WillProficiency { get; set; }

        [JsonPropertyName("perception_proficiency")]
        public required string PerceptionProficiency { get; set; }

        [JsonPropertyName("skill_proficiency")]
        public required string SkillProficiency { get; set; }

        [JsonPropertyName("rarity")]
        public required string Rarity { get; set; }

        [JsonPropertyName("pfs")]
        public required string Pfs { get; set; }
    }
}