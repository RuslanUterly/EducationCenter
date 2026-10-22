using EducationCenter.Api.Contracts.Cart;
using EducationCenter.Api.Domain.Context;
using EducationCenter.Api.Services.Discounts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartController(
    EducationDbContext context,
    IValidator<CalculateCartRequest> validator,
    CartPriceCalculator priceCalculator) : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] CalculateCartRequest request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var user = context.Users.FirstOrDefault(u => u.Id == request.UserId);

        if (user is null)
            return NotFound($"Пользователь {request.UserId} не найден.");

        var courses = context.Courses
            .Where(c => request.CourseIds.Contains(c.Id))
            .ToList();

        if (courses.Count == 0)
            return NotFound("Ни один из указанных курсов не найден.");

        var response = priceCalculator.Calculate(courses, user);

        return Ok(response);
    }
}
