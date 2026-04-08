using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace GrowsmartAPI.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _config.GetSection("EmailSettings");
        var host = emailSettings["SmtpServer"];
        var port = int.Parse(emailSettings["SmtpPort"] ?? "587");
        var user = emailSettings["SmtpUsername"];
        var pass = emailSettings["SmtpPassword"];
        var senderName = emailSettings["SenderName"] ?? "GrowSmart App";

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || user.Contains("YOUR_EMAIL") || pass.Contains("YOUR_GMAIL_APP_PASSWORD"))
        {
            Console.WriteLine("[ERROR] EmailSettings have placeholder values! Falling back to Console Log mock email.");
            Console.WriteLine($"\n--- EMAIL MOCK ---\nTo: {toEmail}\nSubject: {subject}\nBody:\n{body}\n------------------\n");
            return;
        }

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(user, senderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        mailMessage.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(mailMessage);
            Console.WriteLine($"[INFO] Real email sent successfully to {toEmail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to send email to {toEmail}: {ex.Message}");
            throw; // Re-throw to inform caller
        }
    }
}
