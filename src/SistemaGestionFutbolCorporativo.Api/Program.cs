
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(SistemaGestionFutbolCorporativo.Application.Common.IUnitOfWork).Assembly));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => "OK");

app.MapEndpoints();

app.Run();
