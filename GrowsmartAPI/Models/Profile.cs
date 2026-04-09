using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GrowsmartAPI.Models;

public class Profile
{
    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("userType")]
    public string UserType { get; set; } = string.Empty; // e.g., "Home Gardener", "Kitchen Gardener"

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty; // e.g., "New York, NY"

    [JsonPropertyName("gardenSize")]
    public string GardenSize { get; set; } = string.Empty; // e.g., "Balcony", "Small Backyard"

    [JsonPropertyName("cropInterests")]
    public string CropInterests { get; set; } = string.Empty; // e.g., "Vegetables,Flowers,Herbs"
    
    [JsonPropertyName("profileImage")]
    public string ProfileImage { get; set; } = string.Empty;
    
    // Notification Preferences
    [JsonPropertyName("notifWeather")]
    public bool NotifWeather { get; set; } = true;

    [JsonPropertyName("notifCultivation")]
    public bool NotifCultivation { get; set; } = true;

    [JsonPropertyName("notifMarket")]
    public bool NotifMarket { get; set; } = false;


    [JsonIgnore]
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
