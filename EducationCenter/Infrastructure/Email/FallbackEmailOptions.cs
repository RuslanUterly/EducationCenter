namespace EducationCenter.Api.Infrastructure.Email;

public sealed class FallbackEmailOptions
{
    /// <summary>Штатный адрес отдела продаж — сюда уходят заявки при работающем основном канале.</summary>
    public required string PrimaryRecipient { get; init; }

    /// <summary>Резервный адрес для юридических лиц.</summary>
    public required string FallbackRecipientForLegalEntity { get; init; }

    /// <summary>Резервный адрес для физических лиц.</summary>
    public required string FallbackRecipientForIndividual { get; init; }
}