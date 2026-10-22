namespace EducationCenter.Api.Infrastructure.Email;

/// <summary>
/// Контракт отправки сообщения
/// </summary>
public sealed record EmailMessage
{
    public required string To { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
}
