namespace RollCallBackend.Models;

public class Character
{
    public string Name { get; set; } = "";
    public int Level { get; set; }
    public int ArmorClass { get; set; }
    public Ancestry? Ancestry { get; set; }
    public Armor? Armor { get; set; }
    public Background? Background { get; set; }
    public Class? Class { get; set; }
    public List<Attribute> Attributes { get; set; } = [];
    public List<Item> Items { get; set; } = [];
    public List<Ritual> Rituals { get; set; } = [];
    public List<Spell> Spells { get; set; } = [];
    public List<Skill> Skills { get; set; } = [];
}
