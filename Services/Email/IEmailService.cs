namespace caportal.Services.Email;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? toName = null);
}
