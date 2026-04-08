using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Data;

using System.Text.Json.Serialization;

namespace GrowsmartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GardenController : ControllerBase
{
    private readonly AppDbContext _context;

    public GardenController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary/{userId}")]
    public async Task<ActionResult<GardenSummaryDto>> GetSummary(int userId)
    {
        var plants = await _context.Plants.Where(p => p.UserId == userId).ToListAsync();
        var today = DateTime.UtcNow.Date;
        
        // Mocking completed tasks based on plants watered/fertilized today
        var completedWatering = plants.Count(p => p.LastWateredAt != null && p.LastWateredAt.Value.Date == today);
        var completedFertilizing = plants.Count(p => p.LastFertilizedAt != null && p.LastFertilizedAt.Value.Date == today);

        var summary = new GardenSummaryDto
        {
            TotalPlants = plants.Count,
            HealthyPlants = plants.Count(p => p.HealthStatus == "Healthy"),
            NeedsAttention = plants.Count(p => p.HealthStatus != "Healthy"),
            CompletedTasksToday = completedWatering + completedFertilizing
        };

        return Ok(summary);
    }
}

public class GardenSummaryDto
{
    [JsonPropertyName("totalPlants")]
    public int TotalPlants { get; set; }

    [JsonPropertyName("healthyPlants")]
    public int HealthyPlants { get; set; }

    [JsonPropertyName("needsAttention")]
    public int NeedsAttention { get; set; }

    [JsonPropertyName("completedTasksToday")]
    public int CompletedTasksToday { get; set; }
}
