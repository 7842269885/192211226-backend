using System.ComponentModel.DataAnnotations;

namespace GrowsmartAPI.Models;

public class ForgotPasswordOtp
{
    [Key]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string OtpCode { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiryTime { get; set; }

    public bool IsUsed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
