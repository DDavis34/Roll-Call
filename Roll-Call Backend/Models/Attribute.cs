namespace RollCallBackend.Models;

public class Attribute
{
    public string Name { get; set; } = "";
    public string Abbreviation { get; set; } = "";
    public int Score { get; set; }
    public List<Skill> Skills { get; set; } = [];
}
