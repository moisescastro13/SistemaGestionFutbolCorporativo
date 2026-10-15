using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiSolucion.Infrastructure.Queries;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Equipos.Queries;
using SistemaGestionFutbolCorporativo.Infrastructure.Persistence;

namespace SistemaGestionFutbolCorporativo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        // EF Core

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Dapper Connection Factory 
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IEquipoQueries, EquipoQueries>();

        return services;
    }
}