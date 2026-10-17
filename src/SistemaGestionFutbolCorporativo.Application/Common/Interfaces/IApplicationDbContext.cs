using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace SistemaGestionFutbolCorporativo.Application.Common;

public interface IApplicationDbContext
{
    DbSet<Equipo> Equipos { get; }
    DbSet<Jugador> Jugadores { get; }
    DbSet<Partido> Partidos { get; }
    DbSet<Gol> Goles { get; }
}