using GrowsmartAPI.Models;
using GrowsmartAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace GrowsmartAPI.Services;

public class PlantIdService : IPlantIdentityService
{
    private readonly IGeminiService _geminiService;
    private readonly AppDbContext _context;
    private readonly ILogger<PlantIdService> _logger;

    public PlantIdService(IGeminiService geminiService, AppDbContext context, ILogger<PlantIdService> logger)
    {
        _geminiService = geminiService;
        _context = context;
        _logger = logger;
    }

    public async Task<PlantResponse> IdentifyAsync(string base64Image, string language = "en")
    {
        try
        {
            _logger.LogInformation($"Routing request to Gemini AI for identification (Lang: {language})...");
            var result = await _geminiService.IdentifyPlantAsync(base64Image, language);

            if (result != null && result.Status != null && result.Status.Contains("Error: Not a Plant"))
            {
                _logger.LogWarning("AI rejected image: Not a plant subject (Face/Hand/Object).");
                return new PlantResponse 
                { 
                    Status = "Error: Invalid Subject", 
                    CommonName = "No Plant Detected",
                    Description = "The AI detected a face, hand, or inanimate object. Please take a clear photo focusing only on the plant, leaf, or flower."
                };
            }

            // --- ORIGINAL DATA ENHANCEMENT ---
            try 
            {
                if (result != null && result.Status != null && result.Status.Contains("Success") && !string.IsNullOrEmpty(result.CommonName))
                {
                    // Look up this plant in our "Original" Seeded Database
                    var original = await _context.Species.FirstOrDefaultAsync(s => 
                        s.CommonName.ToLower() == result.CommonName.ToLower() || 
                        result.CommonName.ToLower().Contains(s.CommonName.ToLower()));

                    if (original != null)
                    {
                        _logger.LogInformation($"[ORIGINAL DATA] Merging AI result for {result.CommonName} with professional database data.");
                        result.Description = original.Description;
                        result.CareTips = original.CareTips;
                        result.HorticultureSuggestions = original.HorticultureSuggestions;
                        // Use original categories if applicable
                    }
                }
            } catch (Exception dbEx) { _logger.LogWarning($"DB Enhancement Error: {dbEx.Message}"); }
            
            return result ?? new PlantResponse { Status = "Error", CommonName = "Unknown" };
        }
        catch (Exception ex)
        {
            _logger.LogError($"AI Identification Error: {ex.Message}");
            return new PlantResponse 
            { 
                Status = "Error", 
                CommonName = "Recognition Failed",
                Description = "Our AI models are currently overwhelmed or unavailable in your region. Please try again in a few minutes."
            };
        }
    }

    public async Task<PlantHealthResponse> AnalyzeHealthAsync(string base64Image, string language = "en")
    {
        try
        {
            _logger.LogInformation($"Routing request to Gemini AI for health assessment (Lang: {language})...");
            var result = await _geminiService.AnalyzeHealthAsync(base64Image, language);

            if (result != null && result.Status != null && result.Status.Contains("Error: Not a Plant"))
            {
                _logger.LogWarning("AI health check rejected image: Not a plant subject.");
                return new PlantHealthResponse 
                { 
                    Status = "Error: Invalid Subject", 
                    DiseaseName = "No Plant Detected",
                    Description = "To analyze health, please take a clear photo of the affected plant part (leaf or stem). Avoid including faces, hands, or other objects."
                };
            }

            // --- ORIGINAL DISEASE ENHANCEMENT ---
            try 
            {
                if (result != null && result.Status != null && result.Status.Contains("Success") && !string.IsNullOrEmpty(result.DiseaseName))
                {
                    var original = await _context.CommonDiseases.FirstOrDefaultAsync(d => 
                        d.Name.ToLower() == result.DiseaseName.ToLower() || 
                        result.DiseaseName.ToLower().Contains(d.Name.ToLower()));

                    if (original != null)
                    {
                        _logger.LogInformation($"[ORIGINAL DATA] Merging AI diagnosis for {result.DiseaseName} with professional disease database.");
                        result.Description = original.Description;
                        result.Symptoms = original.Symptoms;
                        result.Treatments = original.TreatmentPlan;
                    }
                }
            } catch (Exception dbEx) { _logger.LogWarning($"DB Disease Enhancement Error: {dbEx.Message}"); }

            return result ?? new PlantHealthResponse { Status = "Error", DiseaseName = "Unknown" };
        }
        catch (Exception ex)
        {
            _logger.LogError($"AI Health Analysis Error: {ex.Message}");
            return new PlantHealthResponse 
            { 
                Status = "Error", 
                DiseaseName = "Analysis Unavailable",
                Description = "Our health analysis AI is currently unavailable. Please check back later."
            };
        }
    }

    public async Task<SpaceAnalysisResponse> AnalyzeSpaceAsync(SpaceAnalysisRequest request, string language = "en")
    {
        try
        {
            _logger.LogInformation($"Routing request to Gemini AI for space analysis (Lang: {language})...");
            return await _geminiService.AnalyzeSpaceAsync(request, language);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Migration Error (Space): {ex.Message}");
            return new SpaceAnalysisResponse 
            { 
                Status = "Error", 
                Verdict = "Service Unavailable"
            };
        }
    }
}
