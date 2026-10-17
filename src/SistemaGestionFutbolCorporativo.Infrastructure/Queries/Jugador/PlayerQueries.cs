using Dapper;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Jugadores.Queries;

namespace SistemaGestionFutbolCorporativo.Infrastructure.Queries;

public sealed class PlayerQueries(ISqlConnectionFactory connectionFactory) : IPlayerQueries
{
    public async Task<PlayerDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                j.id AS Id,
                j.equipo_id AS EquipoId,
                e.nombre AS EquipoNombre,
                j.nombre AS Nombre,
                j.dorsal AS Dorsal,
                j.posicion AS Posicion,
                COUNT(g.id)::int AS TotalGoles
            FROM jugadores j
            INNER JOIN equipos e ON e.id = j.equipo_id
            LEFT JOIN goles g ON g.jugador_id = j.id AND g.es_autogol = FALSE
            WHERE j.id = @Id
            GROUP BY j.id, j.equipo_id, e.nombre, j.nombre, j.dorsal, j.posicion;
            """;

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<PlayerDetailResponse>(command);
    }

    public async Task<PagedResponse<PlayerSummaryDto>> GetAllAsync(
        Guid? equipoId, 
        string? posicion, 
        PaginationParams pagination, 
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        var posicionFiltro = string.IsNullOrWhiteSpace(posicion) ? null : $"%{posicion.Trim()}%";

        var sortBy = pagination.SortBy?.ToLowerInvariant() switch
        {
            "nombre" => "j.nombre",
            "dorsal" => "j.dorsal",
            "posicion" => "j.posicion",
            "equiponombre" or "equipo_nombre" => "e.nombre",
            "totalgoles" or "total_goles" => "TotalGoles",
            _ => "j.nombre"
        };
        var sortDirection = pagination.SortDirection == "desc" ? "DESC" : "ASC";

        const string countSql = """
            SELECT COUNT(*) 
            FROM jugadores j
            WHERE (@EquipoId IS NULL OR j.equipo_id = @EquipoId)
              AND (@PosicionFiltro IS NULL OR j.posicion ILIKE @PosicionFiltro);
            """;

        var totalRecords = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                countSql, 
                new { EquipoId = equipoId, PosicionFiltro = posicionFiltro }, 
                cancellationToken: cancellationToken));

        var dataSql = $"""
            SELECT 
                j.id AS Id,
                j.equipo_id AS EquipoId,
                e.nombre AS EquipoNombre,
                j.nombre AS Nombre,
                j.dorsal AS Dorsal,
                j.posicion AS Posicion,
                COUNT(g.id)::int AS TotalGoles
            FROM jugadores j
            INNER JOIN equipos e ON e.id = j.equipo_id
            LEFT JOIN goles g ON g.jugador_id = j.id AND g.es_autogol = FALSE
            WHERE (@EquipoId IS NULL OR j.equipo_id = @EquipoId)
              AND (@PosicionFiltro IS NULL OR j.posicion ILIKE @PosicionFiltro)
            GROUP BY j.id, j.equipo_id, e.nombre, j.nombre, j.dorsal, j.posicion
            ORDER BY {sortBy} {sortDirection}
            LIMIT @PageSize OFFSET @Offset;
            """;

        var items = (await connection.QueryAsync<PlayerSummaryDto>(
            new CommandDefinition(
                dataSql, 
                new 
                { 
                    EquipoId = equipoId, 
                    PosicionFiltro = posicionFiltro, 
                    pagination.PageSize, 
                    pagination.Offset 
                }, 
                cancellationToken: cancellationToken))).AsList();

        return PagedResponse<PlayerSummaryDto>.Create(items, pagination.PageNumber, pagination.PageSize, totalRecords);
    }
}
