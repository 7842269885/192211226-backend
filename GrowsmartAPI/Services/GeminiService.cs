using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GrowsmartAPI.Models;

namespace GrowsmartAPI.Services;

public interface IGeminiService
{
    Task<PlantResponse> IdentifyPlantAsync(string base64Image, string language = "en");
    Task<PlantHealthResponse> AnalyzeHealthAsync(string base64Image, string language = "en");
    Task<SpaceAnalysisResponse> AnalyzeSpaceAsync(SpaceAnalysisRequest request, string language = "en");
}

public class GeminiService : IGeminiService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(IConfiguration configuration, HttpClient httpClient, ILogger<GeminiService> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PlantResponse> IdentifyPlantAsync(string base64Image, string language = "en")
    {
        var apiKey = _configuration["GoogleGemini:ApiKey"]?.Trim();
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
        {
            _logger.LogError("Gemini API Key is missing or default in appsettings.json.");
            return new PlantResponse 
            { 
                Status = "Error: AI Disabled", 
                CommonName = "AI Discovery Offline", 
                Description = "Google Gemini AI is currently disabled. You can still browse our local 'Plant Library' for information on common crops and flowers."
            };
        }

        var prompt = @$"Act as a professional botanist and agricultural expert. Analyze this image to identify the plant, crop, or flower.
        
        STRICT VALIDATION RULES:
        1. If the image does NOT contain a plant, leaf, seed, or flower (e.g., if it is a human face, a hand, an indoor office, or an inanimate object like a car), you MUST set ""status"" to ""Error: Not a Plant"".
        2. Do not attempt to guess or provide botanical info for non-plant subjects.
        
        GOAL: Provide a highly detailed and accurate identification for botanical subjects.
        
        REQUIRED JSON DATA FIELDS:
        1. ""status"": ""Success"" ONLY if a plant/flower/crop is the primary subject. Otherwise ""Error: Not a Plant"".
        2. ""commonName"": The specific name of the plant (No dummy data).
        3. ""scientificName"": The Latin name.
        4. ""confidence"": Numerical confidence (0.0 to 1.0).
        5. ""description"": A professional summary of the identified species.
        6. ""recommendations"": 4-5 expert care tips.
        7. ""isPlant"": true only for botanical subjects.
        8. ""isAgricultural"": true for food or farm crops.
        9. ""horticultureSuggestions"": A comma-separated string of 3-4 suggestions for similar horticulture plants.
        
        LOCALIZATION: Provide all text (names, descriptions, recommendations) in the '{language}' language. Use proper scripts (e.g. Hindi/Telugu).
        
        Return ONLY a raw JSON object:";

        try
        {
            var rawJson = await CallGeminiApiAsync(base64Image, prompt);
            _logger.LogInformation($"Gemini Raw Response: {rawJson}");
            
            // Extract JSON if AI wrapped it in markdown
            var cleanJson = ExtractJson(rawJson);
            
            var result = JsonSerializer.Deserialize<PlantResponse>(cleanJson);
            if (result != null && result.Recommendations != null && result.Recommendations.Count > 0)
            {
                result.CareTips = string.Join("\n", result.Recommendations.Select(r => "• " + r));
            }
            return result ?? new PlantResponse { Status = "Failure: Null Parse" };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Gemini Identification Error: {ex.Message}");
            throw;
        }
    }

    public async Task<PlantHealthResponse> AnalyzeHealthAsync(string base64Image, string language = "en")
    {
        var apiKey = _configuration["GoogleGemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
        {
            _logger.LogError("Gemini API Key is missing in appsettings.json.");
            return new PlantHealthResponse 
            { 
                Status = "Error: AI Disabled", 
                DiseaseName = "Health Analysis Offline", 
                Description = "Google Gemini AI is currently disabled. Please refer to our local 'Disease Library' for common plant health symptoms and treatments."
            };
        }

        var prompt = @$"Act as a world-class plant pathologist. Analyze this leaf/plant photo for diseases, pests, or nutrient deficiencies.
        
        STRICT VALIDATION RULES:
        1. If the image does NOT contain a plant or leaf (e.g., if it is a face, hand, or random object), you MUST set ""status"" to ""Error: Not a Plant"".
        2. Do not attempt to diagnose non-plant subjects.
 
        GOAL: Detect even early signs of stress for agricultural and horticulture plants.
        
        REQUIRED JSON DATA FIELDS:
        1. ""status"": ""Success"" if the image is a plant/leaf. Otherwise ""Error: Not a Plant"".
        2. ""isHealthy"": boolean.
        3. ""healthScore"": 0.0 to 1.0.
        4. ""diseaseName"": SPECIFIC common name and scientific name of the disease(s) found (No dummy data).
        5. ""description"": Detailed explanation of the condition.
        6. ""symptoms"": Exhaustive list of visual indicators found.
        7. ""treatments"": Multi-step actionable treatment plan as a single string.
        8. ""isPlant"": true for botanical matter.
        9. ""isAgricultural"": true for farm crops.
        
        LOCALIZATION: Provide all text (names, descriptions, treatments, symptoms) in the '{language}' language. Use proper scripts (e.g. Hindi/Telugu).
        
        Return ONLY a raw JSON object:";

        try
        {
            var rawJson = await CallGeminiApiAsync(base64Image, prompt);
            
            var cleanJson = ExtractJson(rawJson);
            
            var result = JsonSerializer.Deserialize<PlantHealthResponse>(cleanJson);
            return result ?? new PlantHealthResponse { Status = "Failure: Null Parse" };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Gemini Health Analysis Error: {ex.Message}");
            throw;
        }
    }

    private async Task<string> CallGeminiApiAsync(string base64Image, string prompt)
    {
        var apiKey = _configuration["GoogleGemini:ApiKey"]?.Trim();
        var models = new[] { "gemini-2.5-flash", "gemini-2.0-flash", "gemini-1.5-flash", "gemini-1.5-pro" };
        string lastError = string.Empty;

        foreach (var model in models)
        {
            // Use v1beta for latest models like gemini-1.5-flash
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt },
                            new { inline_data = new { mime_type = "image/jpeg", data = base64Image } }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.2, // Low temperature for consistent JSON
                    topP = 0.8,
                    topK = 40
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Success!
                    using var doc = JsonDocument.Parse(responseString);
                    if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                    {
                        var firstCandidate = candidates[0];
                        if (firstCandidate.TryGetProperty("content", out var contentEl))
                        {
                            var parts = contentEl.GetProperty("parts");
                            if (parts.GetArrayLength() > 0)
                            {
                                return parts[0].GetProperty("text").GetString() ?? string.Empty;
                            }
                        }
                    }
                    throw new Exception("Google AI returned an empty response. This usually happens if the image was blocked by safety filters.");
                }

                // If 404, try the next model
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning($"Model {model} not found. Attempting fallback...");
                    lastError = "NotFound";
                    continue;
                }

                // Other errors
                string detail = responseString;
                try
                {
                    using var errDoc = JsonDocument.Parse(responseString);
                    if (errDoc.RootElement.TryGetProperty("error", out var errorEl))
                    {
                        detail = errorEl.GetProperty("message").GetString() ?? responseString;
                    }
                }
                catch { }

                throw new Exception($"Google AI Error ({response.StatusCode}): {detail}");
            }
            catch (Exception) when (lastError == "NotFound")
            {
                // Continue to next model
                continue;
            }
        }

        throw new Exception($"All models failed or are unavailable in your region. Last error: {lastError}. Check your Google AI Studio configuration.");
    }

    public async Task<SpaceAnalysisResponse> AnalyzeSpaceAsync(SpaceAnalysisRequest request, string language = "en")
    {
        var apiKey = _configuration["GoogleGemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
        {
            throw new Exception("Google Gemini API Key is missing.");
        }

        var prompt = @$"Analyze this image of land/soil for agricultural space analysis.
        User-provided details: Soil Type: {request.SoilType}, Land Size: {request.LandSize}.
        
        CRITICAL INSTRUCTIONS:
        1. If the image is NOT agricultural land (e.g., office, indoors, human face, electronics), set ""verdict"": ""Rejected"".
        2. If the land is concrete/unfit for farming, set ""verdict"": ""Alert"".
        3. Otherwise, set ""verdict"": ""Success"".
        
        CROP SELECTION LOGIC based on Land Size & Context:
        1. Parse the 'LandSize' string carefully:
           - If it contains 'sqft', 'sq m', 'marla', 'perch', or values < 2000 sq ft: Classify as 'Home Garden/Small Scale'. Suggest high-intensity crops like Tomatoes, Peppers, Herbs, Spinach, or Chilies.
           - If it contains 'acres', 'hectares', 'ha', 'bigha', or values > 0.5 Acre: Classify as 'Commercial/Industrial Farm'. Suggest broad-acre crops like Wheat, Rice, Corn, Sugarcane, or Soybeans.
        2. Visual Reconciliation: Use the image to identify soil color (e.g., Red, Clay-brown, Sandy) and texture to further refine the 4-5 best crops.
        3. Priority: If the image looks like a vast field but the user says '500 sqft', prioritize 'Home Garden' recommendations in the 'crops' list but acknowledge the soil texture.
        
        LOCALIZATION: Provide all text (soilDynamics, recommendations, horticultureSuggestions) in the '{language}' language. Use proper scripts (e.g. Hindi/Telugu).
        
        Return ONLY a JSON object:
        {{
            ""status"": ""Success"",
            ""verdict"": ""Success/Rejected/Alert"",
            ""scale"": ""string (e.g. Commercial Farm, Home Garden)"",
            ""soilDynamics"": ""string summary of soil health and properties (acknowledge the '{request.SoilType}' input)"",
            ""recommendations"": ""string detailed expert advice strictly for {request.LandSize} area"",
            ""horticultureSuggestions"": ""string comma separated list of 4-5 specific horticulture crops suitable for {request.LandSize} and the local climate/soil"",
            ""confidence"": 0.95,
            ""isAgricultural"": true/false
        }}";

        try
        {
            if (string.IsNullOrWhiteSpace(request.Image))
            {
                return new SpaceAnalysisResponse 
                { 
                    Status = "Error", 
                    Verdict = "Rejected", 
                    Recommendations = "No image found. Please capture or upload a photo of the land to begin analysis." 
                };
            }

            var rawJson = await CallGeminiApiAsync(request.Image, prompt);
            var cleanJson = ExtractJson(rawJson);
            var result = JsonSerializer.Deserialize<SpaceAnalysisResponse>(cleanJson);
            return result ?? new SpaceAnalysisResponse { Status = "Failure: Null Parse" };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Gemini Space Analysis Error: {ex.Message}");
            throw;
        }
    }

    private string ExtractJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "{}";
        
        // Remove markdown code blocks if present
        if (text.Contains("```json"))
        {
            text = text.Split("```json")[1].Split("```")[0];
        }
        else if (text.Contains("```"))
        {
            text = text.Split("```")[1].Split("```")[0];
        }
        
        return text.Trim();
    }
}
