using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EducationCenter.Api.Middlware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Необработанное исключение при обработке запроса {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        var (statusCode, title) = exception switch
        {
            _ => (StatusCodes.Status500InternalServerError,
                "Внутренняя ошибка сервера")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // true — исключение обработано, дальнейшая обработка не требуется.
        return true;
    }
}
