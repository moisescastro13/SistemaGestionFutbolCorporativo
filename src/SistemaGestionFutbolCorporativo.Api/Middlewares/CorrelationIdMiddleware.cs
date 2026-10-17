using System.Diagnostics;

namespace SistemaGestionFutbolCorporativo.Api.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var headerValue) && 
                            !string.IsNullOrWhiteSpace(headerValue)
            ? headerValue.ToString()
            : Guid.NewGuid().ToString("D");

        context.TraceIdentifier = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TraceId"] = correlationId
        }))
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;
                var method = context.Request.Method;
                var path = context.Request.Path;
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                if (statusCode >= StatusCodes.Status500InternalServerError)
                {
                    logger.LogError("HTTP {Method} {Path} falló con estado {StatusCode} en {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
                        method, path, statusCode, elapsedMs, correlationId);
                }
                else if (statusCode >= StatusCodes.Status400BadRequest)
                {
                    logger.LogWarning("HTTP {Method} {Path} respondió con advertencia {StatusCode} en {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
                        method, path, statusCode, elapsedMs, correlationId);
                }
                else
                {
                    logger.LogInformation("HTTP {Method} {Path} completado con éxito {StatusCode} en {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
                        method, path, statusCode, elapsedMs, correlationId);
                }
            }
        }
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
