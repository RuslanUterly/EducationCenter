using EducationCenter.Api.Contracts.Leads;
using EducationCenter.Api.Domain;
using EducationCenter.Api.Infrastructure.Email;
using Microsoft.Extensions.Options;

namespace EducationCenter.Api.Services.Leads;

public sealed class LeadService(
    IEmailSender emailSender,
    IOptions<FallbackEmailOptions> emailOptions,
    ILogger<LeadService> logger)
{
    public async Task<LeadResponse> SubmitAsync(LeadRequest request, CancellationToken cancellationToken = default)
    {
        var options = emailOptions.Value;

        try
        {
            throw new Exception("Имитируем ошибку отправки почты");
            await emailSender.SendAsync(CreateMessage(options.PrimaryRecipient, request), cancellationToken);
        }
        catch (Exception ex)
        {
            var fallbackRecipient = request.ClientType == ClientType.LegalEntity
                ? options.FallbackRecipientForLegalEntity
                : options.FallbackRecipientForIndividual;

            logger.LogWarning(
                ex,
                "Основной почтовый сервис недоступен. Заявка ({ClientType}) переправлена на резервный адрес {FallbackRecipient}",
                request.ClientType, fallbackRecipient);

            await emailSender.SendAsync(CreateMessage(fallbackRecipient, request), cancellationToken);
        }

        return new(
            Success: true, 
            Message: "Заявка принята. Мы свяжемся с вами в ближайшее время.");
    }

    private static EmailMessage CreateMessage(string recipient, LeadRequest request)
    {
        var clientTypeName = MapClientType(request.ClientType);

        return new EmailMessage
        {
            To = recipient,
            Subject = $"Заявка на обратный звонок — {clientTypeName}",
            Body = $"Имя: {request.Name}\nТелефон: {request.Phone}\nEmail: {request.Email}\nТип клиента: {clientTypeName}"
        };
    }

    private static string MapClientType(ClientType clientType) => clientType switch
    {
        ClientType.Individual => "Физическое лицо",
        ClientType.LegalEntity => "Юридическое лицо",
        _ => throw new ArgumentOutOfRangeException(nameof(clientType), clientType, "Неизвестный тип клиента")
    };
}
