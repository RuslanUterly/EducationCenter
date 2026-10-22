using EducationCenter.Api.Domain.Context;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(EducationDbContext context) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(context.Users.ToList());
}
