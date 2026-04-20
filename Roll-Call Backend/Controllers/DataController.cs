using Microsoft.AspNetCore.Mvc;
using RollCallBackend.Services;

namespace RollCallBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IDataTableService _data;

    public DataController(IDataTableService data)
    {
        _data = data;
    }

    [HttpGet("ancestries")]
    public IActionResult GetAncestries() => Ok(_data.Ancestries);

    [HttpGet("armors")]
    public IActionResult GetArmors() => Ok(_data.Armors);

    [HttpGet("backgrounds")]
    public IActionResult GetBackgrounds() => Ok(_data.Backgrounds);

    [HttpGet("classes")]
    public IActionResult GetClasses() => Ok(_data.Classes);

    [HttpGet("creatures")]
    public IActionResult GetCreatures() => Ok(_data.Creatures);

    [HttpGet("feats")]
    public IActionResult GetFeats() => Ok(_data.Feats);

    [HttpGet("items")]
    public IActionResult GetItems() => Ok(_data.Items);

    [HttpGet("rituals")]
    public IActionResult GetRituals() => Ok(_data.Rituals);

    [HttpGet("spells")]
    public IActionResult GetSpells() => Ok(_data.Spells);

    [HttpGet("traits")]
    public IActionResult GetTraits() => Ok(_data.Traits);
}
