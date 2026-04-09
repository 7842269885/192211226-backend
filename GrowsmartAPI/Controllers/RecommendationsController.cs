using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Data;

using System.Text.Json.Serialization;

namespace GrowsmartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecommendationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RecommendationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<RecommendationDto>>> GetRecommendations(int userId)
    {
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        var recommendations = new List<RecommendationDto>();

        if (profile?.UserType == "Terrace")
        {
            recommendations.Add(new RecommendationDto { Name = "Spinach", Description = "Perfect for your terrace space.", Tags = new List<string> { "Day 30 Harvest", "Low Maintenance" } });
            recommendations.Add(new RecommendationDto { Name = "Mint", Description = "Grows rapidly in semi-shade areas.", Tags = new List<string> { "Fast Growth", "Aromatic" } });
        }
        else
        {
            recommendations.Add(new RecommendationDto { Name = "Aloe Vera", Description = "Great for indoor beginners.", Tags = new List<string> { "Low Water", "Air Purifier" } });
        }

        return Ok(recommendations);
    }
}

public class RecommendationDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();
}
