using System.Text.Json.Serialization;

namespace RollCallBackend.Models;

public class Trait
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
}
