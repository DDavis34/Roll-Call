namespace PF2EGM.Models;

public class Character
{
    private string Name { get; set; }
    private int Level { get; set; }
    private int ArmorClass { get; set; }
    private Ancestry Ancestry { get; set; }
    private Armor Armor { get; set; }
    private Background Background { get; set; }
    private Class Class { get; set; }
    private List<Attribute> Attributes { get; set; } 
    private List<Item> Items { get; set; }
    private List<Ritual> Rituals { get; set; }
    private List<Spell> Spells { get; set; }
    private List<Skill> Skills{ get; set; }
}