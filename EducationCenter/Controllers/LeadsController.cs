using EducationCenter.Api.Contracts.Leads;
using EducationCenter.Api.Services.Leads;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/leads")]
public sealed class LeadsController(
    IValidator<LeadRequest> validator,
    LeadService leadService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] LeadRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var response = await leadService.SubmitAsync(request, cancellationToken);
        return Ok(response);
    }
}
