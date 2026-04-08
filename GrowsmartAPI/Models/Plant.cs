using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GrowsmartAPI.Models;

public class Plant
{
    [Key]
    [JsonPropertyName("id")]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("userId")]
    [Column("UserId")]
    public int UserId { get; set; }

    [Required]
    [JsonPropertyName("name")]
    [Column("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("species")]
    [Column("Species")]
    public string Species { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    [Column("Category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("waterFrequencyDays")]
    [Column("WaterFrequencyDays")]
    public int WaterFrequencyDays { get; set; } = 1;

    [JsonPropertyName("sunlightRequirement")]
    [Column("SunlightRequirement")]
    public string SunlightRequirement { get; set; } = "Full Sun";

    [JsonPropertyName("isIndoor")]
    [Column("IsIndoor")]
    public bool IsIndoor { get; set; } = false;

    [JsonPropertyName("datePlanted")]
    [Column("DatePlanted")]
    public DateTime DatePlanted { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("lastWateredAt")]
    [Column("LastWateredAt")]
    public DateTime? LastWateredAt { get; set; }

    [JsonPropertyName("lastFertilizedAt")]
    [Column("LastFertilizedAt")]
    public DateTime? LastFertilizedAt { get; set; }

    [JsonPropertyName("healthStatus")]
    [Column("HealthStatus")]
    public string HealthStatus { get; set; } = "Healthy";
 // Healthy, Needs Water, Sick

    [JsonIgnore]
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
