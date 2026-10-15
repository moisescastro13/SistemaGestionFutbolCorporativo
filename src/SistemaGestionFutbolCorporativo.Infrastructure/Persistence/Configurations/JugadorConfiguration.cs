using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace MiSolucion.Infrastructure.Persistence.Configurations;

public class JugadorConfiguration : IEntityTypeConfiguration<Jugador>
{
    public void Configure(EntityTypeBuilder<Jugador> builder)
    {
        builder.ToTable("jugadores");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Id)
            .HasColumnName("id");

        builder.Property(j => j.EquipoId)
            .HasColumnName("equipo_id")
            .IsRequired();

        builder.Property(j => j.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(j => j.Dorsal)
            .HasColumnName("dorsal")
            .IsRequired();

        builder.Property(j => j.Posicion)
            .HasColumnName("posicion")
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(j => j.Equipo)
            .WithMany(e => e.Jugadores)
            .HasForeignKey(j => j.EquipoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(j => new { j.EquipoId, j.Dorsal })
            .IsUnique();
    }
}