using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SistemaGestionFutbolCorporativo.Api.Middlewares;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            ArgumentOutOfRangeException outOfRange => (StatusCodes.Status400BadRequest, "Parámetro fuera de rango", outOfRange.Message),
            ArgumentException argEx => (StatusCodes.Status400BadRequest, "Solicitud inválida", argEx.Message),
            InvalidOperationException invOpEx => (StatusCodes.Status400BadRequest, "Operación no permitida", invOpEx.Message),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor", "Ha ocurrido un error inesperado al procesar la solicitud.")
        };

        httpContext.Response.StatusCode = statusCode;

        var handled = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Extensions = { ["traceId"] = httpContext.TraceIdentifier }
            },
            Exception = exception
        });

        if (!handled)
        {
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Extensions = { ["traceId"] = httpContext.TraceIdentifier }
            }, cancellationToken);
        }

        return true;
    }
}
