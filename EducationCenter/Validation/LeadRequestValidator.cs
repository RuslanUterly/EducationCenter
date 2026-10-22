using System.Text.RegularExpressions;
using EducationCenter.Api.Contracts.Leads;
using FluentValidation;

namespace EducationCenter.Api.Validation;

/// <summary>Валидация заявки «Обратный звонок»: имя, телефон, email, тип клиента.</summary>
public sealed partial class LeadRequestValidator : AbstractValidator<LeadRequest>
{
    public LeadRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно для заполнения");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Телефон обязателен для заполнения");

        RuleFor(x => x.Phone)
            .Must(phone => phone.Count(char.IsDigit) >= 7)
                .WithMessage("Телефон должен содержать не менее 7 цифр")
            .Matches(PhonePattern())
                .WithMessage("Телефон указан в неверном формате. Допускаются цифры, +, скобки, пробелы и дефисы")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен для заполнения")
            .EmailAddress().WithMessage("Email указан в неверном формате");

        RuleFor(x => x.ClientType)
            .IsInEnum().WithMessage("Указан неизвестный тип клиента");
    }

    [GeneratedRegex(@"^\+?[\d\s\-\(\)]{7,20}$")]
    private static partial Regex PhonePattern();
}