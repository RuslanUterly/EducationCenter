using EducationCenter.Api.Contracts.Cart;
using FluentValidation;

namespace EducationCenter.Api.Validation;

public sealed partial class CalculateCartValidator : AbstractValidator<CalculateCartRequest>
{
    public CalculateCartValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Id пользователя обязателен для заполнения");

        RuleFor(x => x.CourseIds)
            .NotNull().WithMessage("Список курсов обязателен для заполнения")
            .NotEmpty().WithMessage("Список курсов не должен быть пустым");

        RuleForEach(x => x.CourseIds)
            .NotEmpty().WithMessage("Id курса не должен быть пустым (Guid.Empty)");
    }
}