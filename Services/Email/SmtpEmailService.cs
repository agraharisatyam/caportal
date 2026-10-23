using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace caportal.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? toName = null)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            return false;

        var host = _config["SmtpSettings:Host"];
        var portStr = _config["SmtpSettings:Port"];
        var username = _config["SmtpSettings:Username"];
        var password = _config["SmtpSettings:Password"];
        var enableSsl = bool.TryParse(_config["SmtpSettings:EnableSsl"], out var ssl) ? ssl : true;
        var senderEmail = _config["SmtpSettings:SenderEmail"] ?? "no-reply@cacharteredcampus.com";
        var senderName = _config["SmtpSettings:SenderName"] ?? "CA Chartered Campus";

        // If SMTP server is not yet configured, log to avoid failing the user workflow
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || host.Contains("example.com"))
        {
            _logger.LogInformation("Email simulation (SMTP not configured): To={ToEmail}, Subject={Subject}", toEmail, subject);
            return true;
        }

        int port = int.TryParse(portStr, out var p) ? p : 587;

        try
        {
            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl,
                Timeout = 10000
            };

            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail, toName ?? toEmail));

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            return false;
        }
    }
}
