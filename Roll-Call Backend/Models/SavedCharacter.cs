namespace RollCallBackend.Models;

public class SavedCharacter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string? Ancestry { get; set; }
    public string? CharacterClass { get; set; }
    public int Level { get; set; } = 1;
    public string Data { get; set; } = "{}";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
