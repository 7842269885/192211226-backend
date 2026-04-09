using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GrowsmartAPI.Models;

public class Notification
{
    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "Reminder"; // "Reminder", "Alert", "System"

    [JsonPropertyName("isRead")]
    public bool IsRead { get; set; } = false;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; } // For reminders that have a specific timeframe

    [JsonIgnore]
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
