namespace RollCallBackend.Models;

public class Heritage
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Ancestry? Ancestry { get; set; }
    public List<Feat> FeatsRequiringHeritage { get; set; } = [];
}
