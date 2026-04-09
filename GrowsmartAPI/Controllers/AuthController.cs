using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Data;
using GrowsmartAPI.Models;
using System.Text.Json.Serialization;
using GrowsmartAPI.Services;
using BCrypt.Net;

namespace GrowsmartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public AuthController(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register([FromBody] RegisterDto dto)
    {
        try 
        {
            if (dto == null) return BadRequest("Invalid request payload");

            var normalizedEmail = dto.Email.Trim().ToLower();
            if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
                return BadRequest("Email already exists");

            // Securely hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash.Trim());

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = normalizedEmail,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[INFO] New user registered and hashed: {normalizedEmail}");

            // Ensure user has a profile
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile == null)
            {
                profile = new Profile { 
                    UserId = user.Id,
                    UserType = "Gardening Enthusiast" // Provide a default
                };
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            return Ok(new {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                createdAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Registration failed for {dto?.Email}: {ex.Message}");
            return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login([FromBody] LoginDto loginDto)
    {
        try 
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email)) 
            {
                Console.WriteLine("[DEBUG] Login failed: Empty LoginDto or Email");
                return BadRequest("Invalid login credentials");
            }

            var loginEmail = loginDto.Email.Trim().ToLower();
            var loginPwd = loginDto.Password.Trim();
            
            Console.WriteLine($"[DEBUG-LOGIN] Attempt received for: [{loginEmail}]");

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == loginEmail);

            if (user == null) {
                Console.WriteLine($"[DEBUG-LOGIN] User NOT FOUND in database for: [{loginEmail}]");
                return Unauthorized("Invalid email or password");
            }

            Console.WriteLine($"[DEBUG-LOGIN] User found with ID: {user.Id}. Comparing hashes...");

            bool isPasswordCorrect = false;

            // 1. Try BCrypt Check First (Standard way)
            if (!string.IsNullOrEmpty(user.PasswordHash) && 
               (user.PasswordHash.StartsWith("$2a$") || user.PasswordHash.StartsWith("$2b$") || user.PasswordHash.Length > 50))
            {
                try {
                    isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginPwd, user.PasswordHash);
                    Console.WriteLine($"[DEBUG-LOGIN] BCrypt mapping check result: {isPasswordCorrect}");
                } catch (Exception bcEx) {
                    Console.WriteLine($"[WARN-LOGIN] BCrypt verify threw exception: {bcEx.Message}. Falling back to text check.");
                    isPasswordCorrect = user.PasswordHash.Trim() == loginPwd;
                }
            }
            else 
            {
                // 2. Legacy Fallback: Plain text comparison
                Console.WriteLine("[DEBUG-LOGIN] PasswordHash doesn't look like BCrypt. Using legacy plain-text comparison.");
                isPasswordCorrect = (user.PasswordHash ?? "").Trim() == loginPwd;
            }
            
            // Auto-Migrate to hash on successful login if it's not already hashed correctly
            if (isPasswordCorrect && (!user.PasswordHash.StartsWith("$2") || user.PasswordHash.Length < 50)) 
            {
                Console.WriteLine($"[INFO-LOGIN] Migrating user {loginEmail} to secure hash format.");
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginPwd);
                await _context.SaveChangesAsync();
                Console.WriteLine("[INFO-LOGIN] Password migrated and saved.");
            }

            if (!isPasswordCorrect) {
                Console.WriteLine($"[DEBUG-LOGIN] Password MISMATCH for user: [{loginEmail}].");
                return Unauthorized("Invalid email or password");
            }

            Console.WriteLine($"[SUCCESS-LOGIN] User logged in: {loginEmail}. Sending response...");
            
            return Ok(new {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                createdAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL-LOGIN-FAIL] Crash for {loginDto?.Email} at {DateTime.Now}");
            Console.WriteLine($"[ERROR-MSG] {ex.Message}");
            Console.WriteLine($"[STACK-TRACE] {ex.StackTrace}");
            if (ex.InnerException != null) {
                Console.WriteLine($"[INNER-ERROR] {ex.InnerException.Message}");
                Console.WriteLine($"[INNER-STACK] {ex.InnerException.StackTrace}");
            }
            return StatusCode(500, $"Internal server error: {ex.Message}. Check backend console for stack trace.");
        }
    }

    [HttpDelete("delete-account/{userId}")]
    public async Task<IActionResult> DeleteAccount(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Plants)
            .Include(u => u.Notifications)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Remove related data manually for permanent delete guarantee
        if (user.Profile != null) _context.Profiles.Remove(user.Profile);
        _context.Plants.RemoveRange(user.Plants);
        _context.Notifications.RemoveRange(user.Notifications);

        // Remove the user itself
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        Console.WriteLine($"[INFO] Account PERMANENTLY deleted for user ID: {userId}");
        return Ok(new { message = "Account successfully deleted" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim());
        if (user == null) return NotFound(new { message = "User with this email does not exist" });

        // Generate 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();
        var expiry = DateTime.UtcNow.AddMinutes(15);

        // Remove old unused OTPs for this email
        var oldOtps = _context.PasswordResetOtps.Where(o => o.Email == request.Email.ToLower().Trim() && !o.IsUsed);
        _context.PasswordResetOtps.RemoveRange(oldOtps);

        var otpEntry = new ForgotPasswordOtp
        {
            Email = request.Email.ToLower().Trim(),
            OtpCode = otp,
            ExpiryTime = expiry
        };

        _context.PasswordResetOtps.Add(otpEntry);
        await _context.SaveChangesAsync();

        // Send Email
        var subject = "GrowSmart - Password Reset OTP";
        var body = $@"Hi {user.Name},

You recently requested to reset your account password. Please use the OTP code below to proceed with setting up a new password. 
This code is valid for 15 minutes.

Your OTP Code: {otp}

This is an automated message from the GrowSmart Security Team for your forgot password request.
Have a great day!

If you didn't request this change, please ignore this email to keep your account safe.";

        // Fire and forget the email task so the user doesn't have to wait 50 seconds for SMTP to connect
        _ = Task.Run(() => _emailService.SendEmailAsync(user.Email, subject, body));

        return Ok(new { message = "OTP sent to your email" });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        Console.WriteLine($"[DEBUG] VerifyOtp called with Email=[{request.Email}], OtpCode=[{request.OtpCode}]");
        
        var otpEntries = await _context.PasswordResetOtps
            .Where(o => o.Email == request.Email.ToLower().Trim() && !o.IsUsed)
            .ToListAsync();
            
        Console.WriteLine($"[DEBUG] Found {otpEntries.Count} unused OTPs for this email in database.");
        foreach(var o in otpEntries) {
            Console.WriteLine($"[DEBUG] DB OTP: id={o.Id}, code=[{o.OtpCode}], expiry={o.ExpiryTime}, utcNow={DateTime.UtcNow}");
        }

        var otpEntry = otpEntries
            .Where(o => o.OtpCode == request.OtpCode)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefault();

        if (otpEntry == null)
        {
            Console.WriteLine("[DEBUG] VerifyOtp Error: otpEntry is null (mismatch)");
            return BadRequest(new { message = "Invalid or expired OTP" });
        }
        if (otpEntry.ExpiryTime < DateTime.UtcNow)
        {
            Console.WriteLine("[DEBUG] VerifyOtp Error: OTP is expired.");
            return BadRequest(new { message = "Invalid or expired OTP" });
        }

        return Ok(new { message = "OTP verified successfully" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var otpEntry = await _context.PasswordResetOtps
            .Where(o => o.Email == request.Email.ToLower().Trim() && o.OtpCode == request.OtpCode && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpEntry == null || otpEntry.ExpiryTime < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Access denied: Invalid or expired OTP" });
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim());
        if (user == null) return NotFound(new { message = "User not found" });

        // Hash and Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword.Trim());
        otpEntry.IsUsed = true; // Mark OTP as consumed

        await _context.SaveChangesAsync();

        return Ok(new { message = "Password updated successfully" });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        Console.WriteLine($"[DEBUG-PWD] Change attempt for UserId: {request.UserId}");
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null) {
            Console.WriteLine($"[DEBUG-PWD] User {request.UserId} not found in database.");
            return NotFound(new { message = "User not found" });
        }

        bool isOldPasswordCorrect = false;
        
        // 1. Try BCrypt Check First
        if (!string.IsNullOrEmpty(user.PasswordHash) && 
           (user.PasswordHash.StartsWith("$2a$") || user.PasswordHash.StartsWith("$2b$") || user.PasswordHash.Length > 50))
        {
            try {
                Console.WriteLine("[DEBUG-PWD] Using BCrypt verification...");
                isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.OldPassword.Trim(), user.PasswordHash);
                Console.WriteLine($"[DEBUG-PWD] BCrypt result: {isOldPasswordCorrect}");
            } catch (Exception ex) {
                Console.WriteLine($"[DEBUG-PWD] BCrypt verify failed with error: {ex.Message}. Falling back to text.");
                isOldPasswordCorrect = user.PasswordHash.Trim() == request.OldPassword.Trim();
            }
        }
        else 
        {
            // 2. Legacy Fallback: Plain text comparison
            Console.WriteLine("[DEBUG-PWD] No BCrypt hash detected. Using legacy plain-text comparison...");
            isOldPasswordCorrect = (user.PasswordHash ?? "").Trim() == request.OldPassword.Trim();
            Console.WriteLine($"[DEBUG-PWD] Legacy result: {isOldPasswordCorrect}");
        }

        if (!isOldPasswordCorrect)
        {
            return BadRequest(new { message = "Invalid current password" });
        }

        // Update with new hashed password (always secure now)
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword.Trim());
        await _context.SaveChangesAsync();
        Console.WriteLine("[DEBUG-PWD] Password updated successfully in database.");

        return Ok(new { message = "Password updated successfully" });
    }

    [HttpGet("debug-otps")]
    public async Task<IActionResult> DebugOtps()
    {
        var otps = await _context.PasswordResetOtps.ToListAsync();
        return Ok(otps);
    }
}

public class ChangePasswordRequest
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("oldPassword")]
    public string OldPassword { get; set; } = string.Empty;

    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}

public class RegisterDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;
}

public class LoginDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public class VerifyOtpRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("otpCode")]
    public string OtpCode { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("otpCode")]
    public string OtpCode { get; set; } = string.Empty;

    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}

