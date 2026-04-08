using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowsmartAPI.Models;

public class CommonDisease
{
    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("scientificName")]
    public string ScientificName { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("symptoms")]
    public string Symptoms { get; set; } = string.Empty;

    [JsonPropertyName("treatmentPlan")]
    public string TreatmentPlan { get; set; } = string.Empty;

    [JsonPropertyName("affectedPlants")]
    public string AffectedPlants { get; set; } = string.Empty; // e.g. "Rice, Wheat" or "Tomato, Chili"
}
