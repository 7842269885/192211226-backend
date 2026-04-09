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

    [HttpGet("{userId}", Name = "GetProfileByUserId")]
    public async Task<ActionResult<ProfileDto>> GetProfile(int userId)
    {
        var profile = await _context.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new Profile { UserId = userId };
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
            profile = await _context.Profiles.Include(p => p.User).FirstAsync(p => p.UserId == userId);
        }

        return Ok(MapToDto(profile));
    }

    [HttpPost(Name = "CreateOrUpdateProfileBase")]
    public async Task<ActionResult<ProfileDto>> CreateOrUpdateProfile([FromBody] ProfileDto dto)
    {
        if (dto == null) return BadRequest("Invalid profile data");

        var profile = await _context.Profiles.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == dto.UserId);
        if (profile == null)
        {
            profile = new Profile { UserId = dto.UserId };
            _context.Profiles.Add(profile);
        }

        UpdateProfileFromDto(profile, dto);
        await _context.SaveChangesAsync();
        
        return Ok(MapToDto(profile));
    }

    [HttpPut("{userId}", Name = "UpdateProfileDetails")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] ProfileDto dto)
    {
        if (dto == null) return BadRequest("Invalid profile data");
        if (userId != dto.UserId) return BadRequest("User ID mismatch");

        var profile = await _context.Profiles.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return NotFound();

        UpdateProfileFromDto(profile, dto);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPost("upload-image/{userId}", Name = "UploadProfilePicture")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfileImage([FromRoute] int userId, IFormFile file)
    {
        // Debug Log
        Console.WriteLine($"[DEBUG] UploadProfileImage hit. UserID from route: {userId}");

        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return NotFound($"Profile for User ID {userId} not found");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "ProfileImages");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"{Request.Scheme}://{Request.Host.Value}/Uploads/ProfileImages/{uniqueFileName}";
        profile.ProfileImage = imageUrl;
        await _context.SaveChangesAsync();

        return Ok(new { imageUrl });
    }

    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
    private ProfileDto MapToDto(Profile p)
    {
        return new ProfileDto
        {
            Id = p.Id,
            UserId = p.UserId,
            Name = p.User?.Name ?? "",
            Email = p.User?.Email ?? "",
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

    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
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

        if (p.User != null)
        {
            if (!string.IsNullOrEmpty(dto.Name)) p.User.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Email)) p.User.Email = dto.Email.ToLower().Trim();
        }
    }
}
