namespace SistemaGestionFutbolCorporativo.Domain.Entities;

public class Jugador
{
    public Guid Id { get; private set; }
    public Guid EquipoId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public int Dorsal { get; private set; }
    public string Posicion { get; private set; } = string.Empty;

    public Equipo Equipo { get; private set; } = null!;
    public ICollection<Gol> Goles { get; private set; } = [];

    private Jugador() { }

    public static Jugador Crear(Guid equipoId, string nombre, int dorsal, string posicion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ArgumentException.ThrowIfNullOrWhiteSpace(posicion);

        if (dorsal is < 1 or > 99)
            throw new ArgumentOutOfRangeException(nameof(dorsal), "El dorsal debe estar entre 1 y 99.");

        return new Jugador
        {
            Id = Guid.NewGuid(),
            EquipoId = equipoId,
            Nombre = nombre.Trim(),
            Dorsal = dorsal,
            Posicion = posicion.Trim()
        };
    }
}