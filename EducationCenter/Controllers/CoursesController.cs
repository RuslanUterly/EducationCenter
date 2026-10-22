using EducationCenter.Api.Domain.Context;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/courses")]
public sealed class CoursesController(EducationDbContext context) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(context.Courses.ToList());
}
