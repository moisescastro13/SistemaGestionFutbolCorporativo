using Dapper;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

namespace SistemaGestionFutbolCorporativo.Infrastructure.Queries;

public sealed class TeamQueries(ISqlConnectionFactory connectionFactory) : ITeamQueries
{
    public async Task<TeamDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                id AS Id,
                nombre AS Nombre,
                logo_url AS LogoUrl,
                fecha_creacion AS FechaCreacion
            FROM equipos
            WHERE id = @Id;

            SELECT 
                id AS Id,
                nombre AS Nombre,
                dorsal AS Dorsal,
                posicion AS Posicion
            FROM jugadores
            WHERE equipo_id = @Id
            ORDER BY dorsal ASC;
            """;

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var multi = await connection.QueryMultipleAsync(command);

        var team = await multi.ReadFirstOrDefaultAsync<TeamDetailResponse>();

        if (team is null)
        {
            return null;
        }

        var jugadores = (await multi.ReadAsync<PlayerDto>()).AsList();

        return team with { Jugadores = jugadores };
    }

    public async Task<PagedResponse<TeamSummaryDto>> GetAllAsync(
        string? nombre, 
        PaginationParams pagination, 
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        var filtroNombre = string.IsNullOrWhiteSpace(nombre) ? null : $"%{nombre.Trim()}%";

        var sortBy = pagination.SortBy?.ToLowerInvariant() switch
        {
            "nombre" => "e.nombre",
            "fechacreacion" or "fecha_creacion" => "e.fecha_creacion",
            "totaljugadores" or "total_jugadores" => "TotalJugadores",
            _ => "e.nombre"
        };
        var sortDirection = pagination.SortDirection == "desc" ? "DESC" : "ASC";

        const string countSql = """
            SELECT COUNT(*) 
            FROM equipos 
            WHERE (@FiltroNombre IS NULL OR nombre ILIKE @FiltroNombre);
            """;

        var totalRecords = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(countSql, new { FiltroNombre = filtroNombre }, cancellationToken: cancellationToken));

        var dataSql = $"""
            SELECT 
                e.id AS Id,
                e.nombre AS Nombre,
                e.logo_url AS LogoUrl,
                e.fecha_creacion AS FechaCreacion,
                COUNT(j.id)::int AS TotalJugadores
            FROM equipos e
            LEFT JOIN jugadores j ON j.equipo_id = e.id
            WHERE (@FiltroNombre IS NULL OR e.nombre ILIKE @FiltroNombre)
            GROUP BY e.id, e.nombre, e.logo_url, e.fecha_creacion
            ORDER BY {sortBy} {sortDirection}
            LIMIT @PageSize OFFSET @Offset;
            """;

        var items = (await connection.QueryAsync<TeamSummaryDto>(
            new CommandDefinition(
                dataSql, 
                new { FiltroNombre = filtroNombre, pagination.PageSize, pagination.Offset }, 
                cancellationToken: cancellationToken))).AsList();

        return PagedResponse<TeamSummaryDto>.Create(items, pagination.PageNumber, pagination.PageSize, totalRecords);
    }
}
