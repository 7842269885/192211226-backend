using GrowsmartAPI.Models;

namespace GrowsmartAPI.Services;

public interface IPlantIdentityService
{
    Task<PlantResponse> IdentifyAsync(string base64Image, string language = "en");
    Task<PlantHealthResponse> AnalyzeHealthAsync(string base64Image, string language = "en");
    Task<SpaceAnalysisResponse> AnalyzeSpaceAsync(SpaceAnalysisRequest request, string language = "en");
}
