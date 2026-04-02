namespace PF2EGM.Models;

public class Ancestry
{
    public string Name { get; set; }
    public string Source { get; set; }
    public string Rarity { get; set; }
    public string Pfs { get; set; }
    public string Size { get; set; }
    public string Speed { get; set; }
    public int HitPointsBase { get; set; }
    public string Vision { get; set; }
    public List<string> Language { get; set; }
    public List<Attribute> AttributeFlaw { get; set; }
    public List<Attribute> AttributeBoost { get; set; }
    public List<Feat> AncestryFeats { get; set; }
    public List<Heritage> Heritages { get; set; }
    public List<Ability> Abilities { get; set; }
}