using EducationCenter.Api.Domain;
using EducationCenter.Api.Contracts.Cart;

namespace EducationCenter.Api.Services.Discounts;

public sealed class CartPriceCalculator(IEnumerable<IDiscountPolicy> discountPolicies)
{
    public CartCalculationResponse Calculate(IReadOnlyList<Course> courses, User user)
    {
        var items = new List<CoursePriceItem>(courses.Count);
        decimal totalOriginal = 0m;
        decimal totalFinal = 0m;

        foreach (var course in courses)
        {
            var bestDiscount = discountPolicies
                .Select(policy => policy.TryCalculate(course, user))
                .OfType<DiscountResult>()
                .MaxBy(discount => discount.Percentage);

            var discount = bestDiscount?.Percentage ?? 0m;
            var finalPrice = Math.Round(course.Price * (1 - discount), 2, MidpointRounding.AwayFromZero);

            items.Add(new CoursePriceItem(course.Id, course.Name, course.Price, discount, finalPrice));
        }

        return new CartCalculationResponse(items, totalOriginal, totalFinal);
    }
}
