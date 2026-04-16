using System.Text.Json.Serialization;

namespace PF2EGM.Models
{
    public class Item
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("item_category")]
        public required string ItemCategory { get; set; }

        [JsonPropertyName("item_subcategory")]
        public required string ItemSubcategory { get; set; }

        [JsonPropertyName("level")]
        public required string Level { get; set; }

        [JsonPropertyName("price")]
        public required string Price { get; set; }

        [JsonPropertyName("bulk")]
        public required string Bulk { get; set; }

        [JsonPropertyName("trait")]
        public required string Trait { get; set; }

        [JsonPropertyName("rarity")]
        public required string Rarity { get; set; }

        [JsonPropertyName("pfs")]
        public required string Pfs { get; set; }
    }
}