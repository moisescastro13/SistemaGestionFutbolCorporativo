namespace SistemaGestionFutbolCorporativo.Domain.Entities;

public enum EstadoPartido
{
    Pendiente = 0,
    Finalizado = 1
}

public class Partido
{
    public Guid Id { get; private set; }
    public Guid EquipoLocalId { get; private set; }
    public Guid EquipoVisitanteId { get; private set; }
    public int Jornada { get; private set; }
    public DateTime FechaHora { get; private set; }
    public int? GolesLocal { get; private set; }
    public int? GolesVisitante { get; private set; }
    public EstadoPartido Estado { get; private set; }

    public Equipo EquipoLocal { get; private set; } = null!;
    public Equipo EquipoVisitante { get; private set; } = null!;
    public ICollection<Gol> Goles { get; private set; } = [];

    private Partido() { }

    public static Partido Programar(Guid equipoLocalId, Guid equipoVisitanteId, int jornada, DateTime fechaHora)
    {
        if (equipoLocalId == equipoVisitanteId)
            throw new InvalidOperationException("Un equipo no puede jugar contra sí mismo.");

        return new Partido
        {
            Id = Guid.NewGuid(),
            EquipoLocalId = equipoLocalId,
            EquipoVisitanteId = equipoVisitanteId,
            Jornada = jornada,
            FechaHora = fechaHora,
            Estado = EstadoPartido.Pendiente,
            GolesLocal = null,
            GolesVisitante = null
        };
    }

    public void RegistrarResultado(int golesLocal, int golesVisitante)
    {
        if (golesLocal < 0 || golesVisitante < 0)
            throw new ArgumentException("Los goles no pueden ser negativos.");

        GolesLocal = golesLocal;
        GolesVisitante = golesVisitante;
        Estado = EstadoPartido.Finalizado;
    }
}