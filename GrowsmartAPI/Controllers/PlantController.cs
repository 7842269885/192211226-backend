using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Services;
using GrowsmartAPI.Data;
using GrowsmartAPI.Models;

namespace GrowsmartAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlantController : ControllerBase
{
    private readonly IPlantIdentityService _identityService;
    private readonly AppDbContext _context;
    private readonly ILogger<PlantController> _logger;

    public PlantController(IPlantIdentityService identityService, AppDbContext context, ILogger<PlantController> logger)
    {
        _identityService = identityService;
        _context = context;
        _logger = logger;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        Console.WriteLine("[DEBUG] Ping received!");
        return Ok(new { message = "API working", timestamp = DateTime.UtcNow });
    }

    [HttpPost("identify")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<PlantResponse>> Identify([FromBody] PlantRequest request)
    {
        Console.WriteLine($"[DEBUG] Identify Request received. Base64 length: {request.Image?.Length ?? 0}");
        if (string.IsNullOrEmpty(request.Image)) {
            Console.WriteLine("[DEBUG] Error: Image data is required");
            return BadRequest(new { error = "Image data is required" });
        }

        try
        {
            var language = Request.Headers["Accept-Language"].ToString() ?? "en";
            var result = await _identityService.IdentifyAsync(request.Image, language);
            Console.WriteLine($"[DEBUG] Service returned status: {result.Status} (Lang: {language})");
            
            // Collect/Cache the result for "faster recognition and understanding"
            try 
            {
                if (result.Status.Contains("Success") && !string.IsNullOrEmpty(result.CommonName))
                {
                    var existing = await _context.Species.FirstOrDefaultAsync(s => s.CommonName == result.CommonName);
                    if (existing == null)
                    {
                        _context.Species.Add(new PlantSpecies
                        {
                            CommonName = result.CommonName,
                            ScientificName = result.ScientificName,
                            Description = result.Description,
                            CareTips = string.Join("\n", result.Recommendations)
                        });
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"[DEBUG] Cached new species: {result.CommonName}");
                    }
                }

            }
            catch (Exception dbEx)
            {
                Console.WriteLine($"[DEBUG] Non-blocking DB Cache Error: {dbEx.Message}");
            }

            // Convert IdentificationResult to PlantResponse
            return Ok(new PlantResponse 
            {
                Status = result.Status,
                CommonName = result.CommonName,
                ScientificName = result.ScientificName,
                Confidence = result.Confidence,
                Description = result.Description,
                Recommendations = result.Recommendations
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Controller Exception: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("health")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<PlantHealthResponse>> AnalyzeHealth([FromBody] PlantRequest request)
    {
        Console.WriteLine($"[DEBUG] Health Request received. Base64 length: {request.Image?.Length ?? 0}");
        if (string.IsNullOrEmpty(request.Image)) {
            Console.WriteLine("[DEBUG] Error: Image data is required");
            return BadRequest(new { error = "Image data is required" });
        }

        try
        {
            var language = Request.Headers["Accept-Language"].ToString() ?? "en";
            var result = await _identityService.AnalyzeHealthAsync(request.Image, language);
            Console.WriteLine($"[DEBUG] Health Service returned status: {result.Status} (Lang: {language})");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Health Controller Exception: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("space-analyze")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<SpaceAnalysisResponse>> AnalyzeSpace([FromBody] SpaceAnalysisRequest request)
    {
        _logger.LogInformation($"Space Analysis Request received. Base64 length: {request.Image?.Length ?? 0}");
        if (string.IsNullOrEmpty(request.Image)) {
            return BadRequest(new { error = "Image data is required" });
        }

        try
        {
            var language = Request.Headers["Accept-Language"].ToString() ?? "en";
            var result = await _identityService.AnalyzeSpaceAsync(request, language);
            _logger.LogInformation($"Space Service returned verdict: {result.Verdict} (Lang: {language})");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Space Controller Exception: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("library")]
    public async Task<ActionResult<IEnumerable<PlantSpecies>>> GetLibrary()
    {
        try 
        {
            var species = await _context.Species.ToListAsync();
            return Ok(species);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Library Error: {ex.Message}");
            return StatusCode(500, new { error = "Unable to fetch plant library" });
        }
    }

    [HttpGet("diseases")]
    public async Task<ActionResult<IEnumerable<CommonDisease>>> GetDiseases()
    {
        try 
        {
            var diseases = await _context.CommonDiseases.ToListAsync();
            return Ok(diseases);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Disease Library Error: {ex.Message}");
            return StatusCode(500, new { error = "Unable to fetch disease library" });
        }
    }
}
