namespace SistemaGestionFutbolCorporativo.Domain.Entities;

public class Equipo
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    public ICollection<Jugador> Jugadores { get; private set; } = [];
    public ICollection<Partido> PartidosLocales { get; private set; } = [];
    public ICollection<Partido> PartidosVisitantes { get; private set; } = [];
    public ICollection<Gol> Goles { get; private set; } = [];

    private Equipo() { }

    public static Equipo Crear(string nombre, string? logoUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        return new Equipo
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            LogoUrl = logoUrl?.Trim(),
            FechaCreacion = DateTime.UtcNow
        };
    }
}