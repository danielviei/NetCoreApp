using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserNamespace.Models;

public class EmailSettings
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }

    public required bool EnableSsl { get; set; } = true;

}

public interface IEmailSender<TUser> where TUser : class
{
    Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink);
    Task SendEmailAsync(string email, string subject, string message);
    Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode);
    Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink);
}

public class EmailSender : IEmailSender<CustomUser>
{
    private readonly SmtpClient _smtpClient;
    private readonly IConfiguration _configuration;

    public EmailSender(IOptions<EmailSettings> options, IConfiguration configuration)
    {
        if (options?.Value == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        _configuration = configuration;

        _smtpClient = new SmtpClient(options.Value.Host, options.Value.Port)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_configuration["EMAIL_USERNAME"], _configuration["EMAIL_PASSWORD"]),
            EnableSsl = options.Value.EnableSsl
        };
    }

    public Task SendConfirmationLinkAsync(CustomUser user, string email, string confirmationLink)
    {
        throw new NotImplementedException();
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["EMAIL_USERNAME"] ?? "noreply@localhost"),
            To = { email },
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        try
        {
            await _smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw; // Re-throw the exception for handling in the controller
        }
    }

    public Task SendPasswordResetCodeAsync(CustomUser user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(CustomUser user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }
}
