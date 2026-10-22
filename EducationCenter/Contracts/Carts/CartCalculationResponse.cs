namespace EducationCenter.Api.Contracts.Cart;

public sealed record CartCalculationResponse(
    List<CoursePriceItem> Items,
    decimal TotalOriginal,
    decimal TotalFinal);

public sealed record CoursePriceItem(
    Guid CourseId,
    string Name,
    decimal BasePrice,
    decimal AppliedDiscountPercent,
    decimal FinalPrice);
