using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GrowsmartAPI.Models;

public class PlantSpecies
{
    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("commonName")]
    public string CommonName { get; set; } = string.Empty;

    [JsonPropertyName("scientificName")]
    public string ScientificName { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("careTips")]
    public string CareTips { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("horticultureSuggestions")]
    public string HorticultureSuggestions { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = "Horticulture"; // "Agricultural" or "Horticulture"

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
