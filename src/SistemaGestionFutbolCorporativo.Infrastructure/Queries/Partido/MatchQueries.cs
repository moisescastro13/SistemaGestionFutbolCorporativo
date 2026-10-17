using Dapper;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Partidos.Queries;

namespace SistemaGestionFutbolCorporativo.Infrastructure.Queries;

public sealed class MatchQueries(ISqlConnectionFactory connectionFactory) : IMatchQueries
{
    public async Task<MatchDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                p.id AS Id,
                p.jornada AS Jornada,
                p.fecha_hora AS FechaHora,
                p.estado AS Estado,
                p.equipo_local_id AS EquipoLocalId,
                el.nombre AS EquipoLocalNombre,
                el.logo_url AS EquipoLocalLogoUrl,
                p.equipo_visitante_id AS EquipoVisitanteId,
                ev.nombre AS EquipoVisitanteNombre,
                ev.logo_url AS EquipoVisitanteLogoUrl,
                p.goles_local AS GolesLocal,
                p.goles_visitante AS GolesVisitante
            FROM partidos p
            INNER JOIN equipos el ON el.id = p.equipo_local_id
            INNER JOIN equipos ev ON ev.id = p.equipo_visitante_id
            WHERE p.id = @Id;

            SELECT 
                g.id AS Id,
                g.jugador_id AS JugadorId,
                j.nombre AS JugadorNombre,
                g.equipo_id AS EquipoId,
                e.nombre AS EquipoNombre,
                g.minuto AS Minuto,
                g.es_autogol AS EsAutogol
            FROM goles g
            INNER JOIN jugadores j ON j.id = g.jugador_id
            INNER JOIN equipos e ON e.id = g.equipo_id
            WHERE g.partido_id = @Id
            ORDER BY g.minuto ASC NULLS LAST;
            """;

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var multi = await connection.QueryMultipleAsync(command);

        var match = await multi.ReadFirstOrDefaultAsync<MatchDetailResponse>();
        if (match is null)
        {
            return null;
        }

        var goles = (await multi.ReadAsync<GoalDetailDto>()).AsList();
        return match with { Goles = goles };
    }

    public async Task<PagedResponse<MatchSummaryDto>> GetAllAsync(
        int? jornada, 
        Guid? equipoId, 
        string? estado, 
        PaginationParams pagination, 
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        var estadoFiltro = string.IsNullOrWhiteSpace(estado) ? null : estado.Trim();

        var sortBy = pagination.SortBy?.ToLowerInvariant() switch
        {
            "jornada" => "p.jornada",
            "fechahora" or "fecha_hora" => "p.fecha_hora",
            "estado" => "p.estado",
            _ => "p.jornada"
        };
        var sortDirection = pagination.SortDirection == "desc" ? "DESC" : "ASC";

        const string countSql = """
            SELECT COUNT(*) 
            FROM partidos p
            WHERE (@Jornada IS NULL OR p.jornada = @Jornada)
              AND (@EquipoId IS NULL OR p.equipo_local_id = @EquipoId OR p.equipo_visitante_id = @EquipoId)
              AND (@Estado IS NULL OR p.estado ILIKE @Estado);
            """;

        var totalRecords = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                countSql, 
                new { Jornada = jornada, EquipoId = equipoId, Estado = estadoFiltro }, 
                cancellationToken: cancellationToken));

        var dataSql = $"""
            SELECT 
                p.id AS Id,
                p.jornada AS Jornada,
                p.fecha_hora AS FechaHora,
                p.estado AS Estado,
                p.equipo_local_id AS EquipoLocalId,
                el.nombre AS EquipoLocalNombre,
                el.logo_url AS EquipoLocalLogoUrl,
                p.equipo_visitante_id AS EquipoVisitanteId,
                ev.nombre AS EquipoVisitanteNombre,
                ev.logo_url AS EquipoVisitanteLogoUrl,
                p.goles_local AS GolesLocal,
                p.goles_visitante AS GolesVisitante
            FROM partidos p
            INNER JOIN equipos el ON el.id = p.equipo_local_id
            INNER JOIN equipos ev ON ev.id = p.equipo_visitante_id
            WHERE (@Jornada IS NULL OR p.jornada = @Jornada)
              AND (@EquipoId IS NULL OR p.equipo_local_id = @EquipoId OR p.equipo_visitante_id = @EquipoId)
              AND (@Estado IS NULL OR p.estado ILIKE @Estado)
            ORDER BY {sortBy} {sortDirection}, p.fecha_hora ASC
            LIMIT @PageSize OFFSET @Offset;
            """;

        var items = (await connection.QueryAsync<MatchSummaryDto>(
            new CommandDefinition(
                dataSql, 
                new 
                { 
                    Jornada = jornada, 
                    EquipoId = equipoId, 
                    Estado = estadoFiltro, 
                    pagination.PageSize, 
                    pagination.Offset 
                }, 
                cancellationToken: cancellationToken))).AsList();

        return PagedResponse<MatchSummaryDto>.Create(items, pagination.PageNumber, pagination.PageSize, totalRecords);
    }
}
