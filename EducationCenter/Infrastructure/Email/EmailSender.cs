using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EducationCenter.Api.Infrastructure.Email;

/// <summary>
/// Контракт отправки почты
/// </summary>
public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}

public class TestEmailSender : IEmailSender
{
    private readonly ILogger<TestEmailSender> logger;
    public TestEmailSender(ILogger<TestEmailSender> logger)
    {
        this.logger = logger;
    }
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Письмо отправлено на {ToEmail} с темой {Subject} и телом {HtmlBody}", message.To, message.Subject, message.Body);
        return Task.CompletedTask;
    }
}

public class EmailSender(
    IOptions<EmailOptions> options,
    ILogger<EmailSender> logger) : IEmailSender
{
    private readonly EmailOptions options = options.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        using var mailMessage = new MailMessage
        {
            From = new MailAddress(options.FromEmail, options.FromName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(message.To);

        using var client = new SmtpClient(options.Host, options.Port)
        {
            EnableSsl = options.EnableSsl,
        };

        if (!string.IsNullOrEmpty(options.UserName))
        {
            client.Credentials = new NetworkCredential(options.UserName, options.Password);
        }

        await client.SendMailAsync(mailMessage, cancellationToken);
        logger.LogInformation("Письмо отправлено на {ToEmail} с темой {Subject} и телом {HtmlBody}", mailMessage.To[0], mailMessage.Subject, mailMessage.Body);
    }
}
