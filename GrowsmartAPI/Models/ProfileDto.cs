using System.Text.Json.Serialization;

namespace GrowsmartAPI.Models
{
    public class ProfileDto
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("userId")] public int UserId { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }
        [JsonPropertyName("userType")] public string? UserType { get; set; }
        [JsonPropertyName("location")] public string? Location { get; set; }
        [JsonPropertyName("gardenSize")] public string? GardenSize { get; set; }
        [JsonPropertyName("phone")] public string? Phone { get; set; }
        [JsonPropertyName("primaryInterest")] public string? PrimaryInterest { get; set; }
        [JsonPropertyName("receiveReminders")] public bool ReceiveReminders { get; set; }
        [JsonPropertyName("receiveWeatherAlerts")] public bool ReceiveWeatherAlerts { get; set; }
        [JsonPropertyName("profileImage")] public string? ProfileImage { get; set; }
    }
}
