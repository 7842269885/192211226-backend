using Microsoft.AspNetCore.Mvc;

namespace GrowsmartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InsightsController : ControllerBase
{
    [HttpGet("weather")]
    public IActionResult GetWeather()
    {
        return Ok(new
        {
            temperature = "32°C",
            condition = "Sunny",
            suggestion = "Water early today"
        });
    }

    [HttpGet("garden")]
    public IActionResult GetGardenInsights()
    {
        return Ok(new List<string>
        {
            "Your plants are 80% healthy 🌿",
            "Next watering in 6 hours 💧",
            "2 plants growing fast 🚀"
        });
    }

    [HttpGet("tips")]
    public IActionResult GetTips()
    {
        return Ok(new List<object>
        {
            new { title = "Soil Aeration", description = "Remember to loosen the top soil once every week for better root breathing." },
            new { title = "Mulching", description = "Add a layer of organic mulch to retain moisture during hot summer days." }
        });
    }
}
