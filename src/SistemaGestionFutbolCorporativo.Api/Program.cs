
using Microsoft.OpenApi.Models;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Api.Middlewares;
using SistemaGestionFutbolCorporativo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Sistema de Gestión de Fútbol Corporativo API",
            Version = "v1",
            Description = "API RESTful con Clean Architecture, CQRS (EF Core + Dapper), Eventos de Dominio y soporte de Idempotencia."
        };
        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        if (context.Description.ActionDescriptor.EndpointMetadata.OfType<RequireIdempotencyAttribute>().Any())
        {
            operation.Parameters ??= [];
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = IdempotencyFilter.HeaderName,
                In = ParameterLocation.Header,
                Required = true,
                Description = "Clave única para garantizar la idempotencia de la operación",
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
        return Task.CompletedTask;
    });
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(SistemaGestionFutbolCorporativo.Application.Common.IUnitOfWork).Assembly));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("Idempotency-Key", "X-Correlation-Id");
    });
});

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();
app.UseCorrelationId();

await app.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Sistema de Gestión de Fútbol Corporativo v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Fútbol Corporativo API - Swagger UI";
    });
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapGet("/health", () => "OK");

app.MapEndpoints();

app.Run();
