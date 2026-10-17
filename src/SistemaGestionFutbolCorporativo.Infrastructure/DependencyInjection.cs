using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Equipos.Queries;
using SistemaGestionFutbolCorporativo.Application.Jugadores.Queries;
using SistemaGestionFutbolCorporativo.Application.Partidos.Queries;
using SistemaGestionFutbolCorporativo.Application.Torneo.Queries;
using SistemaGestionFutbolCorporativo.Infrastructure.Persistence;
using SistemaGestionFutbolCorporativo.Infrastructure.Queries;

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
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddScoped<IIdempotencyStore, IdempotencyStore>();

        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        // Dapper Connection Factory 
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<ITeamQueries, TeamQueries>();
        services.AddScoped<IPlayerQueries, PlayerQueries>();
        services.AddScoped<IMatchQueries, MatchQueries>();
        services.AddScoped<ITournamentQueries, TournamentQueries>();

        return services;
    }
}