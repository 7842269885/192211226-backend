using System.ComponentModel.DataAnnotations;

namespace GrowsmartAPI.Models;

public class RawIdentificationLog
{
    [Key]
    public int Id { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? Status { get; set; }
    
    public string? PlantNameCaptured { get; set; }
    
    public double Confidence { get; set; }
    
    public string? RawApiResponse { get; set; } // JSON
    
    // Optional: store the image hash or partial base64 for debugging
    public string? ImageReference { get; set; } 
}
