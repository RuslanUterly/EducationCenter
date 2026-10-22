using EducationCenter.Api.Domain;

namespace EducationCenter.Api.Services.Discounts;

/// <summary>
/// «Горящие группы»: если до старта курса осталось менее 5 дней, применяется скидка 20%.
/// Курс, который уже начался, также считается горящим (цель — добрать в группу).
/// </summary>
public sealed class HotGroupDiscountPolicy(TimeProvider clock) : IDiscountPolicy
{
    public const decimal DiscountPercentage = 0.20m;
    public const int RemainingDaysThreshold = 5;

    public string Name => "Горящие группы";

    public DiscountResult? TryCalculate(Course course, User user)
    {
        var today = clock.GetUtcNow().UtcDateTime.Date;
        var daysLeft = (course.StartDate.Date - today).TotalDays;

        return daysLeft < RemainingDaysThreshold
            ? new DiscountResult(DiscountPercentage, Name)
            : null;
    }
}
