using Microsoft.AspNetCore.Mvc;
using GrowsmartAPI.Services;

namespace GrowsmartAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IGeminiService _geminiService;
    private readonly ILogger<DebugController> _logger;

    public DebugController(IConfiguration configuration, IGeminiService geminiService, ILogger<DebugController> logger)
    {
        _configuration = configuration;
        _geminiService = geminiService;
        _logger = logger;
    }

    [HttpGet("verify-ai")]
    public async Task<IActionResult> VerifyAi()
    {
        var apiKey = _configuration["GoogleGemini:ApiKey"];
        var isPlaceholder = string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE";

        var status = new {
            ApiKeyConfigured = !isPlaceholder,
            ApiKeyPreview = isPlaceholder ? "Not Set" : (apiKey != null && apiKey.Length > 10 ? apiKey.Substring(0, 5) + "..." + apiKey.Substring(apiKey.Length - 4) : "Set"),
            ServiceConnectivityTest = "Pending"
        };

        if (isPlaceholder)
        {
            return BadRequest(new { 
                Error = "Google Gemini API Key is missing or using placeholder.",
                Recommendation = "Please get a key from https://aistudio.google.com/ and update appsettings.json",
                Status = status
            });
        }

        try {
            _logger.LogInformation("Testing Gemini API with real model call (gemini-2.0-flash)...");
            
            // Try identifying a simple word to verify model availability
            var testPrompt = "Identify this: { 'item': 'Test' }";
            var result = await _geminiService.IdentifyPlantAsync("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg=="); // 1x1 transparent pixel

            return Ok(new {
                Message = "Backend successfully communicated with Gemini AI.",
                ApiKeyStatus = "CONFIGURED",
                LastAiStatus = result.Status,
                TestResult = result.CommonName,
                Status = status
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                Error = "Failed to communicate with Gemini AI.",
                Details = ex.Message,
                Recommendation = "Check if the models (gemini-2.0-flash, gemini-1.5-flash) are available for your key.",
                Status = status
            });
        }
    }
}
