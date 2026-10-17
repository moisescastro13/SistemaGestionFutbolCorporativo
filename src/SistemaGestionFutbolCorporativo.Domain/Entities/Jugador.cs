using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Domain.Entities;

public class Jugador : Entity
{
    private static readonly HashSet<string> ValidPositions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Portero",
        "Defensa",
        "Medio",
        "Mediocampista",
        "Delantero"
    };

    public Guid Id { get; private set; }
    public Guid EquipoId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public int Dorsal { get; private set; }
    public string Posicion { get; private set; } = string.Empty;

    public Equipo Equipo { get; private set; } = null!;
    public ICollection<Gol> Goles { get; private set; } = [];

    private Jugador() { }

    public static Jugador Create(Guid equipoId, string nombre, int dorsal, string posicion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ValidateDorsal(dorsal);
        var normalizedPosition = ValidatePosition(posicion);

        return new Jugador
        {
            Id = Guid.NewGuid(),
            EquipoId = equipoId,
            Nombre = nombre.Trim(),
            Dorsal = dorsal,
            Posicion = normalizedPosition
        };
    }

    public void Update(string nombre, int dorsal, string posicion)
    {
        UpdateName(nombre);
        UpdateDorsal(dorsal);
        UpdatePosition(posicion);
    }

    public void UpdateName(string nombre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        Nombre = nombre.Trim();
    }

    public void UpdateDorsal(int dorsal)
    {
        ValidateDorsal(dorsal);
        Dorsal = dorsal;
    }

    public void UpdatePosition(string posicion)
    {
        Posicion = ValidatePosition(posicion);
    }

    public static bool IsValidPosition(string posicion) =>
        !string.IsNullOrWhiteSpace(posicion) && ValidPositions.Contains(posicion.Trim());

    private static void ValidateDorsal(int dorsal)
    {
        if (dorsal is < 1 or > 99)
        {
            throw new ArgumentOutOfRangeException(nameof(dorsal), "El dorsal debe estar comprendido entre 1 y 99.");
        }
    }

    private static string ValidatePosition(string posicion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(posicion);
        var trimmed = posicion.Trim();
        if (!ValidPositions.Contains(trimmed))
        {
            throw new ArgumentException($"La posición '{posicion}' no es válida. Posiciones permitidas: Portero, Defensa, Medio/Mediocampista, Delantero.");
        }

        return ValidPositions.First(p => string.Equals(p, trimmed, StringComparison.OrdinalIgnoreCase));
    }
}