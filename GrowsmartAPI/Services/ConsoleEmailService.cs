using Microsoft.Extensions.Logging;

namespace GrowsmartAPI.Services;

public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        var message = $@"
============================================================
📧 EMAIL SENT (CONSOLE MODE)
To: {to}
Subject: {subject}
------------------------------------------------------------
{body}
============================================================
";
        _logger.LogInformation(message);
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}
