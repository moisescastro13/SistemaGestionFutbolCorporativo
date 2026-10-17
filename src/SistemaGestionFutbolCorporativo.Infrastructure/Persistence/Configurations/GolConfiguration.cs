using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace SistemaGestionFutbolCorporativo.Infrastructure.Persistence.Configurations;

public class GolConfiguration : IEntityTypeConfiguration<Gol>
{
    public void Configure(EntityTypeBuilder<Gol> builder)
    {
        builder.ToTable("goles");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id");

        builder.Property(g => g.PartidoId)
            .HasColumnName("partido_id")
            .IsRequired();

        builder.Property(g => g.JugadorId)
            .HasColumnName("jugador_id")
            .IsRequired();

        builder.Property(g => g.EquipoId)
            .HasColumnName("equipo_id")
            .IsRequired();

        builder.Property(g => g.Minuto)
            .HasColumnName("minuto")
            .IsRequired(false);

        builder.Property(g => g.EsAutogol)
            .HasColumnName("es_autogol")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasOne(g => g.Partido)
            .WithMany(p => p.Goles)
            .HasForeignKey(g => g.PartidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Jugador)
            .WithMany(j => j.Goles)
            .HasForeignKey(g => g.JugadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Equipo)
            .WithMany(e => e.Goles)
            .HasForeignKey(g => g.EquipoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}