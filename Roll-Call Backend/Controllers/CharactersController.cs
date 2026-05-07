using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RollCallBackend.Data;
using RollCallBackend.Models;
using RollCallBackend.Services;

namespace RollCallBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ISupabaseUserContextService _userContextService;

    public CharactersController(
        AppDbContext dbContext,
        ISupabaseUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SavedCharacterResponse>>> GetAll(CancellationToken ct)
    {
        var user = await _userContextService.GetCurrentUserAsync(ct);
        if (user is null)
        {
            return Unauthorized();
        }

        var characters = await _dbContext.SavedCharacters
            .AsNoTracking()
            .Where(character => character.UserId == user.UserId)
            .OrderByDescending(character => character.UpdatedUtc)
            .Select(character => new SavedCharacterResponse(
                character.Id,
                character.Name,
                character.Ancestry,
                character.CharacterClass,
                character.Level,
                character.Data,
                character.CreatedUtc,
                character.UpdatedUtc))
            .ToListAsync(ct);

        return Ok(characters);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SavedCharacterResponse>> GetById(Guid id, CancellationToken ct)
    {
        var user = await _userContextService.GetCurrentUserAsync(ct);
        if (user is null)
        {
            return Unauthorized();
        }

        var character = await _dbContext.SavedCharacters
            .AsNoTracking()
            .FirstOrDefaultAsync(saved => saved.Id == id && saved.UserId == user.UserId, ct);

        if (character is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(character));
    }

    [HttpPost]
    public async Task<ActionResult<SavedCharacterResponse>> Create(
        [FromBody] SaveCharacterRequest request,
        CancellationToken ct)
    {
        var user = await _userContextService.GetCurrentUserAsync(ct);
        if (user is null)
        {
            return Unauthorized();
        }

        var character = new SavedCharacter
        {
            UserId = user.UserId,
            Name = request.Name.Trim(),
            Ancestry = request.Ancestry?.Trim(),
            CharacterClass = request.CharacterClass?.Trim(),
            Level = request.Level,
            Data = request.Data,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.SavedCharacters.Add(character);
        await _dbContext.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = character.Id }, ToResponse(character));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SavedCharacterResponse>> Update(
        Guid id,
        [FromBody] SaveCharacterRequest request,
        CancellationToken ct)
    {
        var user = await _userContextService.GetCurrentUserAsync(ct);
        if (user is null)
        {
            return Unauthorized();
        }

        var character = await _dbContext.SavedCharacters
            .FirstOrDefaultAsync(saved => saved.Id == id && saved.UserId == user.UserId, ct);
        if (character is null)
        {
            return NotFound();
        }

        character.Name = request.Name.Trim();
        character.Ancestry = request.Ancestry?.Trim();
        character.CharacterClass = request.CharacterClass?.Trim();
        character.Level = request.Level;
        character.Data = request.Data;
        character.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);

        return Ok(ToResponse(character));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var user = await _userContextService.GetCurrentUserAsync(ct);
        if (user is null)
        {
            return Unauthorized();
        }

        var character = await _dbContext.SavedCharacters
            .FirstOrDefaultAsync(saved => saved.Id == id && saved.UserId == user.UserId, ct);
        if (character is null)
        {
            return NotFound();
        }

        _dbContext.SavedCharacters.Remove(character);
        await _dbContext.SaveChangesAsync(ct);

        return NoContent();
    }

    private static SavedCharacterResponse ToResponse(SavedCharacter character) =>
        new(
            character.Id,
            character.Name,
            character.Ancestry,
            character.CharacterClass,
            character.Level,
            character.Data,
            character.CreatedUtc,
            character.UpdatedUtc);
}

public record SaveCharacterRequest(
    string Name,
    string? Ancestry,
    string? CharacterClass,
    int Level,
    string Data);

public record SavedCharacterResponse(
    Guid Id,
    string Name,
    string? Ancestry,
    string? CharacterClass,
    int Level,
    string Data,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
