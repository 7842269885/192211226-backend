using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GrowsmartAPI.Models;

public class User
{
    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [JsonIgnore]
    public Profile? Profile { get; set; }
    
    [JsonIgnore]
    public ICollection<Plant> Plants { get; set; } = new List<Plant>();
    
    [JsonIgnore]
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
