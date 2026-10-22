using EducationCenter.Api.Domain;
using EducationCenter.Api.Services.Discounts;
using FluentAssertions;

namespace EducationCenter.Tests.Discounts;

public sealed class CartPriceCalculatorTests
{
    private static readonly DateTime Today = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private readonly CartPriceCalculator _calculator = new(
    [
        new HotGroupDiscountPolicy(new FixedTimeProvider(new DateTimeOffset(Today))),
        new GraduateDiscountPolicy()
    ]);

    private static User RegularUser { get; } =
        new() { Id = Guid.NewGuid(), Name = "Игорь", IsGraduate = false };

    private static User GraduateUser { get; } =
        new() { Id = Guid.NewGuid(), Name = "Роман", IsGraduate = true };

    private static Course CourseStartingIn(int days) => new()
    {
        Id = Guid.NewGuid(),
        Name = "C#: основы программирования",
        Price = 10_000m,
        StartDate = Today.AddDays(days)
    };

    [Theory]
    [InlineData(3, 0.20)]   // до старта 3 дня — «горящая группа», скидка 20%
    [InlineData(-1, 0.20)]  // курс уже начался — также считается горящим
    public void Calculate_WhenCourseStartsInLessThan5Days_AppliesHotGroupDiscount(
        int daysUntilStart, double expectedDiscount)
    {
        var result = _calculator.Calculate([CourseStartingIn(daysUntilStart)], RegularUser);

        var item = result.Items.Should().ContainSingle().Which;
        item.AppliedDiscountPercent.Should().Be((decimal)expectedDiscount);
        item.FinalPrice.Should().Be(8_000m);
    }

    [Fact]
    public void Calculate_WhenCourseStartsInExactly5Days_NoDiscount()
    {
        var result = _calculator.Calculate([CourseStartingIn(5)], RegularUser);

        var item = result.Items.Should().ContainSingle().Which;
        item.AppliedDiscountPercent.Should().Be(0m);
        item.FinalPrice.Should().Be(10_000m);
    }

    [Fact]
    public void Calculate_WhenUserIsGraduate_AppliesGraduateDiscount()
    {
        var result = _calculator.Calculate([CourseStartingIn(10)], GraduateUser);

        var item = result.Items.Should().ContainSingle().Which;
        item.AppliedDiscountPercent.Should().Be(0.30m);
        item.FinalPrice.Should().Be(7_000m);
    }

    [Fact]
    public void Calculate_WhenGraduateAndHotGroup_OnlyMaxDiscountApplies()
    {
        var result = _calculator.Calculate([CourseStartingIn(2)], GraduateUser);

        var item = result.Items.Should().ContainSingle().Which;
        item.AppliedDiscountPercent.Should().Be(0.30m);
        item.FinalPrice.Should().Be(7_000m);
        result.TotalOriginal.Should().Be(10_000m);
        result.TotalFinal.Should().Be(7_000m);
    }
}
