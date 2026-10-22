using EducationCenter.Api.Domain;

namespace EducationCenter.Api.Services.Discounts;

/// <summary>«Выпускник»: пользователям с флагом IsGraduate применяется скидка 30%.</summary>
public sealed class GraduateDiscountPolicy : IDiscountPolicy
{
    public const decimal DiscountPercentage = 0.30m;

    public string Name => "Выпускник";

    public DiscountResult? TryCalculate(Course course, User user) =>
        user.IsGraduate
            ? new DiscountResult(DiscountPercentage, Name)
            : null;
}
