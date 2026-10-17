namespace SistemaGestionFutbolCorporativo.Domain.Entities;

public class Gol
{
    public Guid Id { get; private set; }
    public Guid PartidoId { get; private set; }
    public Guid JugadorId { get; private set; }
    public Guid EquipoId { get; private set; }
    public int? Minuto { get; private set; }
    public bool EsAutogol { get; private set; }

    public Partido Partido { get; private set; } = null!;
    public Jugador Jugador { get; private set; } = null!;
    public Equipo Equipo { get; private set; } = null!;

    private Gol() { }

    public static Gol Register(Guid partidoId, Guid jugadorId, Guid equipoId, int? minuto, bool esAutogol = false)
    {
        if (minuto is < 1 or > 130)
            throw new ArgumentOutOfRangeException(nameof(minuto), "El minuto debe ser válido.");

        return new Gol
        {
            Id = Guid.NewGuid(),
            PartidoId = partidoId,
            JugadorId = jugadorId,
            EquipoId = equipoId,
            Minuto = minuto,
            EsAutogol = esAutogol
        };
    }
}