using Dapper;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Torneo.Queries;

namespace SistemaGestionFutbolCorporativo.Infrastructure.Queries;

public sealed class TournamentQueries(ISqlConnectionFactory connectionFactory) : ITournamentQueries
{
    public async Task<IReadOnlyList<TournamentStandingDto>> GetStandingsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            WITH stats_partidos AS (
                SELECT 
                    p.equipo_local_id AS equipo_id,
                    1 AS pj,
                    CASE WHEN p.goles_local > p.goles_visitante THEN 1 ELSE 0 END AS pg,
                    CASE WHEN p.goles_local = p.goles_visitante THEN 1 ELSE 0 END AS pe,
                    CASE WHEN p.goles_local < p.goles_visitante THEN 1 ELSE 0 END AS pp,
                    COALESCE(p.goles_local, 0) AS gf,
                    COALESCE(p.goles_visitante, 0) AS gc
                FROM partidos p
                WHERE p.estado ILIKE 'Finalizado'

                UNION ALL

                SELECT 
                    p.equipo_visitante_id AS equipo_id,
                    1 AS pj,
                    CASE WHEN p.goles_visitante > p.goles_local THEN 1 ELSE 0 END AS pg,
                    CASE WHEN p.goles_visitante = p.goles_local THEN 1 ELSE 0 END AS pe,
                    CASE WHEN p.goles_visitante < p.goles_local THEN 1 ELSE 0 END AS pp,
                    COALESCE(p.goles_visitante, 0) AS gf,
                    COALESCE(p.goles_local, 0) AS gc
                FROM partidos p
                WHERE p.estado ILIKE 'Finalizado'
            )
            SELECT 
                ROW_NUMBER() OVER (
                    ORDER BY 
                        ((COALESCE(SUM(s.pg), 0) * 3) + (COALESCE(SUM(s.pe), 0) * 1)) DESC,
                        (COALESCE(SUM(s.gf), 0) - COALESCE(SUM(s.gc), 0)) DESC,
                        COALESCE(SUM(s.gf), 0) DESC,
                        e.nombre ASC
                )::int AS Posicion,
                e.id AS EquipoId,
                e.nombre AS EquipoNombre,
                e.logo_url AS LogoUrl,
                COALESCE(SUM(s.pj), 0)::int AS PJ,
                COALESCE(SUM(s.pg), 0)::int AS PG,
                COALESCE(SUM(s.pe), 0)::int AS PE,
                COALESCE(SUM(s.pp), 0)::int AS PP,
                COALESCE(SUM(s.gf), 0)::int AS GF,
                COALESCE(SUM(s.gc), 0)::int AS GC,
                (COALESCE(SUM(s.gf), 0) - COALESCE(SUM(s.gc), 0))::int AS DG,
                ((COALESCE(SUM(s.pg), 0) * 3) + (COALESCE(SUM(s.pe), 0) * 1))::int AS Puntos
            FROM equipos e
            LEFT JOIN stats_partidos s ON s.equipo_id = e.id
            GROUP BY e.id, e.nombre, e.logo_url
            ORDER BY Posicion ASC;
            """;

        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<TournamentStandingDto>(command);

        return result.AsList();
    }

    public async Task<IReadOnlyList<TopScorerDto>> GetTopScorersAsync(
        int? limit = null, 
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                ROW_NUMBER() OVER (ORDER BY COUNT(g.id) DESC, j.nombre ASC)::int AS Posicion,
                j.id AS JugadorId,
                j.nombre AS JugadorNombre,
                e.id AS EquipoId,
                e.nombre AS EquipoNombre,
                j.posicion AS PosicionJugador,
                COUNT(g.id)::int AS Goles
            FROM goles g
            INNER JOIN jugadores j ON j.id = g.jugador_id
            INNER JOIN equipos e ON e.id = j.equipo_id
            INNER JOIN partidos p ON p.id = g.partido_id
            WHERE g.es_autogol = FALSE 
              AND p.estado ILIKE 'Finalizado'
            GROUP BY j.id, j.nombre, e.id, e.nombre, j.posicion
            HAVING COUNT(g.id) > 0
            ORDER BY Goles DESC, j.nombre ASC
            LIMIT @Limit;
            """;

        var command = new CommandDefinition(sql, new { Limit = limit }, cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<TopScorerDto>(command);

        return result.AsList();
    }
}
