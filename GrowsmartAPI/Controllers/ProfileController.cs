using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Data;
using GrowsmartAPI.Models;
using System.Text.Json.Serialization;

namespace GrowsmartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(int userId)
    {
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            // Auto-create if not exists
            profile = new Profile { UserId = userId };
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        return Ok(MapToDto(profile));
    }

    [HttpPost]
    public async Task<ActionResult<ProfileDto>> CreateOrUpdateProfile([FromBody] ProfileDto dto)
    {
        if (dto == null) return BadRequest("Invalid profile data");

        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == dto.UserId);

        if (profile == null)
        {
            profile = new Profile { UserId = dto.UserId };
            _context.Profiles.Add(profile);
        }

        UpdateProfileFromDto(profile, dto);

        await _context.SaveChangesAsync();
        return Ok(MapToDto(profile));
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] ProfileDto dto)
    {
        if (dto == null) return BadRequest("Invalid profile data");
        if (userId != dto.UserId)
            return BadRequest("User ID mismatch");

        var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (existingProfile == null)
            return NotFound();

        UpdateProfileFromDto(existingProfile, dto);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private ProfileDto MapToDto(Profile p)
    {
        return new ProfileDto
        {
            Id = p.Id,
            UserId = p.UserId,
            Name = p.User?.Name ?? "", // Include name from User table
            Email = p.User?.Email ?? "", // Include email from User table
            UserType = p.UserType,
            Location = p.Location,
            GardenSize = p.GardenSize,
            Phone = p.Phone,
            PrimaryInterest = p.CropInterests,
            ReceiveReminders = p.NotifCultivation,
            ReceiveWeatherAlerts = p.NotifWeather,
            ProfileImage = p.ProfileImage
        };
    }

    private void UpdateProfileFromDto(Profile p, ProfileDto dto)
    {
        p.UserType = dto.UserType ?? p.UserType;
        p.Location = dto.Location ?? p.Location;
        p.GardenSize = dto.GardenSize ?? p.GardenSize;
        p.Phone = dto.Phone ?? p.Phone;
        p.CropInterests = dto.PrimaryInterest ?? p.CropInterests;
        p.NotifCultivation = dto.ReceiveReminders;
        p.NotifWeather = dto.ReceiveWeatherAlerts;
        p.ProfileImage = dto.ProfileImage ?? p.ProfileImage;

        // NEW: Update associated User details if provided
        if (p.User != null)
        {
            if (!string.IsNullOrEmpty(dto.Name)) p.User.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Email)) p.User.Email = dto.Email.ToLower().Trim();
        }
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] ProfileDto dto)
    {
        if (dto == null) return BadRequest("Invalid profile data");
        if (userId != dto.UserId)
            return BadRequest("User ID mismatch");

        // Use Include(p => p.User) to ensure we can update user details
        var existingProfile = await _context.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (existingProfile == null)
            return NotFound();

        UpdateProfileFromDto(existingProfile, dto);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("upload-image/{userId}")]
    public async Task<IActionResult> UploadProfileImage(int userId, [FromForm] IFormFile file)
    {
        Console.WriteLine($"[DEBUG] UploadProfileImage hit for UserID: {userId}");
        if (file == null || file.Length == 0)
        {
            Console.WriteLine("[DEBUG] UploadProfileImage failed: File is null or empty");
            return BadRequest("No file uploaded");
        }

        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
            return NotFound("Profile not found");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "ProfileImages");
        Directory.CreateDirectory(uploadsFolder); // Create directory if it doesn't exist

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host.Value}";
        var imageUrl = $"{baseUrl}/Uploads/ProfileImages/{uniqueFileName}";

        profile.ProfileImage = imageUrl;
        await _context.SaveChangesAsync();

        return Ok(new { imageUrl });
    }
}

public class ProfileDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("userType")]
    public string? UserType { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("gardenSize")]
    public string? GardenSize { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("primaryInterest")]
    public string? PrimaryInterest { get; set; }

    [JsonPropertyName("receiveReminders")]
    public bool ReceiveReminders { get; set; }

    [JsonPropertyName("receiveWeatherAlerts")]
    public bool ReceiveWeatherAlerts { get; set; }

    [JsonPropertyName("profileImage")]
    public string? ProfileImage { get; set; }
}

