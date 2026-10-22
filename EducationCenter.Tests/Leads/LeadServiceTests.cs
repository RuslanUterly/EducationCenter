using EducationCenter.Api.Contracts.Leads;
using EducationCenter.Api.Domain;
using EducationCenter.Api.Infrastructure.Email;
using EducationCenter.Api.Services.Leads;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace EducationCenter.Tests.Leads;

public sealed class LeadServiceTests
{
    private const string FallbackForIndividual = "specialist.ufl@yandex.ru";
    private const string FallbackForLegalEntity = "specialist.corp@yandex.ru";

    private readonly Mock<IEmailSender> _primaryEmailSenderMock = new();
    private readonly Mock<IEmailSender> _backupEmailSenderMock = new();
    private readonly LeadService _leadService;

    public LeadServiceTests()
    {
        var options = Options.Create(new FallbackEmailOptions
        {
            PrimaryRecipient = "sales@studycenter.ru",
            FallbackRecipientForLegalEntity = FallbackForLegalEntity,
            FallbackRecipientForIndividual = FallbackForIndividual
        });

        _leadService = new LeadService(
            _primaryEmailSenderMock.Object,
            _backupEmailSenderMock.Object,
            options,
            NullLogger<LeadService>.Instance);
    }

    [Theory]
    [InlineData(ClientType.Individual, FallbackForIndividual)]
    [InlineData(ClientType.LegalEntity, FallbackForLegalEntity)]
    public async Task SubmitAsync_WhenPrimaryMailFails_SendsLeadToCorrectFallbackAddress(
        ClientType clientType, string expectedFallbackRecipient)
    {
        _primaryEmailSenderMock
       .Setup(sender => sender.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
       .ThrowsAsync(new Exception("Основной почтовый сервис недоступен"));

        var response = await _leadService.SubmitAsync(CreateLead(clientType));

        response.Success.Should().BeTrue();

        _backupEmailSenderMock.Verify(
            sender => sender.SendAsync(
                It.Is<EmailMessage>(message => message.To == expectedFallbackRecipient),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WhenPrimaryMailFails_UserStillReceivesSuccessResponse()
    {
        var response = await _leadService.SubmitAsync(CreateLead(ClientType.LegalEntity));

        response.Success.Should().BeTrue();
        response.Message.Should().NotBeNullOrEmpty();
    }

    private static LeadRequest CreateLead(ClientType clientType) =>
        new("Иван Иванов", "+7 (999) 123-45-67", "ivan@example.com", clientType);
}
