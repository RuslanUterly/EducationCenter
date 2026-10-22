namespace EducationCenter.Api.Contracts.Cart;

public sealed record CalculateCartRequest(IReadOnlyList<Guid> CourseIds, Guid UserId);