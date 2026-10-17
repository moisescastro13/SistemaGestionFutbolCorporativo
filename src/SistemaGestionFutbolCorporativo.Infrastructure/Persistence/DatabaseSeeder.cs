using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace SistemaGestionFutbolCorporativo.Infrastructure.Persistence;


public sealed class DatabaseInitializer(
    ApplicationDbContext dbContext,
    IDatabaseSeeder seeder,
    ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (dbContext.Database.IsRelational())
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            await seeder.SeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error durante la inicialización o sembrado de la base de datos.");
            //throw;
        }
    }
}
public sealed class DatabaseSeeder(ApplicationDbContext context) : IDatabaseSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await context.Equipos.AnyAsync(cancellationToken))
        {
            return;
        }

        var alianza = Equipo.Create("Alianza FC", "https://img.freepik.com/vector-premium/alianza.png");
        var fas = Equipo.Create("CD FAS", "https://img.freepik.com/vector-premium/fas.png");
        var aguila = Equipo.Create("CD Águila", "https://img.freepik.com/vector-premium/aguila.png");
        var firpo = Equipo.Create("CD Luis Ángel Firpo", "https://img.freepik.com/vector-premium/firpo.png");

        await context.Equipos.AddRangeAsync([alianza, fas, aguila, firpo], cancellationToken);

        var jAlianza = new List<Jugador>
        {
            Jugador.Create(alianza.Id, "Mario González", 1, "Portero"),
            Jugador.Create(alianza.Id, "Henry Romero", 4, "Defensa"),
            Jugador.Create(alianza.Id, "Narciso Orellana", 6, "Medio"),
            Jugador.Create(alianza.Id, "Marvin Monterroza", 10, "Medio"),
            Jugador.Create(alianza.Id, "Rodolfo Zelaya", 22, "Delantero")
        };

        var jFas = new List<Jugador>
        {
            Jugador.Create(fas.Id, "Kevin Carabantes", 1, "Portero"),
            Jugador.Create(fas.Id, "Rudy Clavel", 3, "Defensa"),
            Jugador.Create(fas.Id, "Jairo Martínez", 8, "Medio"),
            Jugador.Create(fas.Id, "Dustin Corea", 11, "Delantero"),
            Jugador.Create(fas.Id, "Rafael Tejada", 9, "Delantero")
        };

        var jAguila = new List<Jugador>
        {
            Jugador.Create(aguila.Id, "Rafael García", 1, "Portero"),
            Jugador.Create(aguila.Id, "Ronald Rodríguez", 2, "Defensa"),
            Jugador.Create(aguila.Id, "Darwin Cerén", 7, "Medio"),
            Jugador.Create(aguila.Id, "Santos Ortiz", 17, "Medio"),
            Jugador.Create(aguila.Id, "Dixon Rivas", 19, "Delantero")
        };

        var jFirpo = new List<Jugador>
        {
            Jugador.Create(firpo.Id, "Wilberth Hernández", 1, "Portero"),
            Jugador.Create(firpo.Id, "Tardelis Peña", 5, "Defensa"),
            Jugador.Create(firpo.Id, "Diego Flores", 14, "Medio"),
            Jugador.Create(firpo.Id, "Styven Vásquez", 9, "Delantero"),
            Jugador.Create(firpo.Id, "Mateo Ramírez", 20, "Delantero")
        };

        var todosLosJugadores = jAlianza.Concat(jFas).Concat(jAguila).Concat(jFirpo).ToList();
        await context.Jugadores.AddRangeAsync(todosLosJugadores, cancellationToken);

        // Jornada 1
        var p1 = Partido.Schedule(alianza.Id, fas.Id, 1, DateTime.UtcNow.AddDays(-6));
        var p2 = Partido.Schedule(aguila.Id, firpo.Id, 1, DateTime.UtcNow.AddDays(-6));

        // Jornada 2
        var p3 = Partido.Schedule(alianza.Id, aguila.Id, 2, DateTime.UtcNow.AddDays(-3));
        var p4 = Partido.Schedule(fas.Id, firpo.Id, 2, DateTime.UtcNow.AddDays(2)); // Pendiente

        // Jornada 3
        var p5 = Partido.Schedule(firpo.Id, alianza.Id, 3, DateTime.UtcNow.AddDays(5)); // Pendiente
        var p6 = Partido.Schedule(fas.Id, aguila.Id, 3, DateTime.UtcNow.AddDays(6));   // Pendiente

        p1.RegisterResult(2, 1);
        var golesP1 = new List<Gol>
        {
            Gol.Register(p1.Id, jAlianza[4].Id, alianza.Id, 23), 
            Gol.Register(p1.Id, jFas[4].Id, fas.Id, 55),          
            Gol.Register(p1.Id, jAlianza[3].Id, alianza.Id, 88)  
        };

        p2.RegisterResult(0, 0);

        p3.RegisterResult(1, 2);
        var golesP3 = new List<Gol>
        {
            Gol.Register(p3.Id, jAguila[4].Id, aguila.Id, 14),   
            Gol.Register(p3.Id, jAlianza[4].Id, alianza.Id, 40), 
            Gol.Register(p3.Id, jAguila[2].Id, aguila.Id, 79)    
        };

        await context.Partidos.AddRangeAsync([p1, p2, p3, p4, p5, p6], cancellationToken);
        await context.Goles.AddRangeAsync(golesP1.Concat(golesP3), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}