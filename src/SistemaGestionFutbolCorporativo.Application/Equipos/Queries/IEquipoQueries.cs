namespace SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

public interface IEquipoQueries
{
    Task<EquipoDetalleResponse?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}