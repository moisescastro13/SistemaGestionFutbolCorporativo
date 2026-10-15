using Dapper;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

namespace MiSolucion.Infrastructure.Queries;

public sealed class EquipoQueries(ISqlConnectionFactory connectionFactory) : IEquipoQueries
{
    public async Task<EquipoDetalleResponse?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
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

        var equipo = await multi.ReadFirstOrDefaultAsync<EquipoDetalleResponse>();

        if (equipo is null)
        {
            return null;
        }

        var jugadores = (await multi.ReadAsync<JugadorDto>()).AsList();

        return equipo with { Jugadores = jugadores };
    }
}