namespace GrowsmartAPI.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class PlantRequest
{
    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty; // Base64
}


public class PlantResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("commonName")]
    public string CommonName { get; set; } = string.Empty;

    [JsonPropertyName("scientificName")]
    public string ScientificName { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("recommendations")]
    public List<string> Recommendations { get; set; } = new List<string>();

    [JsonPropertyName("careTips")]
    public string CareTips { get; set; } = string.Empty;

    [JsonPropertyName("healthStatus")]
    public string HealthStatus { get; set; } = "Healthy";

    [JsonPropertyName("isPlant")]
    [JsonProperty("isPlant")]
    public bool IsPlant { get; set; } = true;

    [JsonPropertyName("isAgricultural")]
    public bool IsAgricultural { get; set; } = false;

    [JsonPropertyName("horticultureSuggestions")]
    public string HorticultureSuggestions { get; set; } = string.Empty;
}


public class PlantHealthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("isHealthy")]
    public bool IsHealthy { get; set; }

    [JsonPropertyName("healthScore")]
    public double HealthScore { get; set; }

    [JsonPropertyName("diseaseName")]
    public string DiseaseName { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("symptoms")]
    public string Symptoms { get; set; } = string.Empty;

    [JsonPropertyName("treatments")]
    public string Treatments { get; set; } = string.Empty;
    
    [JsonPropertyName("isPlant")]
    [JsonProperty("isPlant")]
    public bool IsPlant { get; set; } = true;

    [JsonPropertyName("isAgricultural")]
    public bool IsAgricultural { get; set; } = false;
}

public class SpaceAnalysisRequest
{
    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;

    [JsonPropertyName("soilType")]
    public string SoilType { get; set; } = string.Empty;

    [JsonPropertyName("landSize")]
    public string LandSize { get; set; } = string.Empty;
}

public class SpaceAnalysisResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("verdict")]
    public string Verdict { get; set; } = string.Empty; // "Success", "Rejected", "Alert"

    [JsonPropertyName("scale")]
    public string Scale { get; set; } = string.Empty;

    [JsonPropertyName("soilDynamics")]
    public string SoilDynamics { get; set; } = string.Empty;

    [JsonPropertyName("recommendations")]
    public string Recommendations { get; set; } = string.Empty;

    [JsonPropertyName("horticultureSuggestions")]
    public string HorticultureSuggestions { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("isAgricultural")]
    public bool IsAgricultural { get; set; }
}
