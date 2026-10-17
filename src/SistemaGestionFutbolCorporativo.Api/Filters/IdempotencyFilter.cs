using System.Text.Json;
using SistemaGestionFutbolCorporativo.Application.Common;


namespace SistemaGestionFutbolCorporativo.Api.Filters;
public class IdempotencyFilter(IIdempotencyStore store) : IEndpointFilter
{
    public const string HeaderName = "Idempotency-Key";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;

        if (!httpContext.Request.Headers.TryGetValue(HeaderName, out var rawKey) || string.IsNullOrWhiteSpace(rawKey))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Header Missing",
                detail: $"El encabezado '{HeaderName}' es obligatorio para esta operación.");
        }

        var key = rawKey.ToString().Trim();
        var path = httpContext.Request.Path.Value ?? string.Empty;

        var existingRecord = await store.GetAsync(key, cancellationToken);

        if (existingRecord is not null)
        {
            if (!existingRecord.IsCompleted)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict",
                    detail: "Una operación con esta clave de idempotencia ya está en progreso.");
            }

            httpContext.Response.Headers[HeaderName] = key;
            return Results.Content(
                content: existingRecord.ResponseBody ?? string.Empty,
                contentType: "application/json",
                statusCode: existingRecord.StatusCode);
        }

        var created = await store.TryCreateAsync(key, path, cancellationToken);
        if (!created)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: "Conflicto de concurrencia al registrar la clave de idempotencia.");
        }

        var result = await next(context);

        if (result is IStatusCodeHttpResult statusResult)
        {
            var statusCode = statusResult.StatusCode ?? StatusCodes.Status200OK;
            string? bodyJson = null;

            if (result is IValueHttpResult valueResult && valueResult.Value is not null)
            {
                bodyJson = JsonSerializer.Serialize(valueResult.Value);
            }

            await store.MarkCompletedAsync(key, statusCode, bodyJson, cancellationToken);
        }

        return result;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class RequireIdempotencyAttribute : Attribute;

public static class IdempotencyExtensions
{
    public static RouteHandlerBuilder RequireIdempotency(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<IdempotencyFilter>()
            .WithMetadata(new RequireIdempotencyAttribute());
    }
}