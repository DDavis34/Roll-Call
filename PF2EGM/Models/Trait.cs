using System.Text.Json.Serialization;

namespace PF2EGM.Models;

public class Trait
{
    [JsonPropertyName("category")]
    public string Category { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    public string Description { get; set; }
}