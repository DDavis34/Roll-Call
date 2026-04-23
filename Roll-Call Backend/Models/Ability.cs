namespace RollCallBackend.Models;

public class Ability
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<Trait> Traits { get; set; } = [];
}
