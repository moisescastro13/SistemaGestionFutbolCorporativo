using SistemaGestionFutbolCorporativo.Domain.Common;
using SistemaGestionFutbolCorporativo.Domain.Events;

namespace SistemaGestionFutbolCorporativo.Domain.Entities;

public class Equipo : Entity
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

    public static Equipo Create(string nombre, string? logoUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            LogoUrl = logoUrl?.Trim(),
            FechaCreacion = DateTime.UtcNow
        };

        equipo.RaiseDomainEvent(new TeamCreatedDomainEvent(
            equipo.Id,
            equipo.Nombre,
            equipo.LogoUrl,
            equipo.FechaCreacion));

        return equipo;
    }

    public void Update(string nombre, string? logoUrl)
    {
        UpdateName(nombre);
        UpdateLogo(logoUrl);
    }

    public void UpdateName(string nombre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        Nombre = nombre.Trim();
    }

    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? null : logoUrl.Trim();
    }
}