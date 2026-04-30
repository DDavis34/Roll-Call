using Microsoft.AspNetCore.Mvc;
using RollCallBackend.Services;

namespace RollCallBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScraperController(Scrapper scrapper, ILogger<ScraperController> logger) : ControllerBase
{
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private static bool _running;

    /// <summary>
    /// Triggers a full scrape of all Archives of Nethys data tables.
    /// Returns 202 Accepted immediately; scraping runs in the background.
    /// </summary>
    [HttpPost("run")]
    public IActionResult RunAll()
    {
        if (!_lock.Wait(0))
            return Conflict("A scrape is already running.");

        _running = true;
        var messages = new List<string>();
        var progress = new Progress<string>(msg =>
        {
            messages.Add(msg);
            logger.LogInformation("[Scraper] {Message}", msg);
        });

        _ = Task.Run(async () =>
        {
            try
            {
                await scrapper.ScrapeAllAsync(progress);
            }
            finally
            {
                _running = false;
                _lock.Release();
            }
        });

        return Accepted(new { message = "Full scrape started.", status = "running" });
    }

    /// <summary>
    /// Triggers a scrape for a single category.
    /// Valid values: ancestries, classes, feats, spells, backgrounds,
    ///               armor, items, creatures, rituals, skills, traits
    /// </summary>
    [HttpPost("run/{category}")]
    public IActionResult RunCategory(string category)
    {
        if (!_lock.Wait(0))
            return Conflict("A scrape is already running.");

        _running = true;
        var progress = new Progress<string>(msg => logger.LogInformation("[Scraper] {Message}", msg));

        _ = Task.Run(async () =>
        {
            try
            {
                await scrapper.ScrapeAsync(category, progress);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning("Unknown scrape category requested: {Category}. {Error}", category, ex.Message);
            }
            finally
            {
                _running = false;
                _lock.Release();
            }
        });

        return Accepted(new { message = $"Scrape started for category: {category}.", status = "running" });
    }

    /// <summary>
    /// Returns whether a scrape is currently running.
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus() =>
        Ok(new { running = _running });
}
