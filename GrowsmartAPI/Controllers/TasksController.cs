using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Data;
using GrowsmartAPI.Models;

using System.Text.Json.Serialization;

namespace GrowsmartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("urgent/{userId}")]
    public async Task<ActionResult<IEnumerable<GardenTaskDto>>> GetUrgentTasks(int userId)
    {
        var plants = await _context.Plants.Where(p => p.UserId == userId).ToListAsync();
        var urgentTasks = new List<GardenTaskDto>();
        var today = DateTime.UtcNow.Date;

        foreach (var plant in plants)
        {
            // Urgent if Health is "Needs Attention" or overdue watering
            if (plant.HealthStatus == "Needs Attention" || plant.LastWateredAt == null || plant.LastWateredAt.Value.Date < today)
            {
                urgentTasks.Add(new GardenTaskDto
                {
                    Id = plant.Id,
                    Title = $"Urgent: Water {plant.Name}",
                    Type = "Watering",
                    Time = "Now",
                    Status = "Urgent"
                });
            }
        }

        return Ok(urgentTasks);
    }

    [HttpGet("today/{userId}")]
    public async Task<ActionResult<IEnumerable<GardenTaskDto>>> GetTodayTasks(int userId)
    {
        var plants = await _context.Plants.Where(p => p.UserId == userId).ToListAsync();
        var tasks = new List<GardenTaskDto>();
        var today = DateTime.UtcNow.Date;

        foreach (var plant in plants)
        {
            // Watering Task: If never watered or last watered before today
            if (plant.LastWateredAt == null || plant.LastWateredAt.Value.Date < today)
            {
                tasks.Add(new GardenTaskDto
                {
                    Id = plant.Id,
                    Title = $"Water {plant.Name}",
                    Type = "Watering",
                    Time = "Morning",
                    Status = "Pending"
                });
            }

            // Fertilizing Task: Every 14 days
            if (plant.LastFertilizedAt == null || (DateTime.UtcNow - plant.LastFertilizedAt.Value).TotalDays >= 14)
            {
                tasks.Add(new GardenTaskDto
                {
                    Id = plant.Id,
                    Title = $"Fertilize {plant.Name}",
                    Type = "Fertilizing",
                    Time = "Anytime",
                    Status = "Pending"
                });
            }
        }

        return Ok(tasks);
    }

    [HttpPost("complete/{plantId}/{taskType}")]
    public async Task<IActionResult> CompleteTask(int plantId, string taskType)
    {
        var plant = await _context.Plants.FindAsync(plantId);
        if (plant == null) return NotFound();

        if (taskType.Equals("Watering", StringComparison.OrdinalIgnoreCase))
            plant.LastWateredAt = DateTime.UtcNow;
        else if (taskType.Equals("Fertilizing", StringComparison.OrdinalIgnoreCase))
            plant.LastFertilizedAt = DateTime.UtcNow;
        
        plant.HealthStatus = "Healthy"; // Completing a task restores health in this simplified logic

        await _context.SaveChangesAsync();
        return Ok();
    }
}

public class GardenTaskDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
