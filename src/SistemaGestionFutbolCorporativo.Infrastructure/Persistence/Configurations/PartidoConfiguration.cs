using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace MiSolucion.Infrastructure.Persistence.Configurations;

public class PartidoConfiguration : IEntityTypeConfiguration<Partido>
{
    public void Configure(EntityTypeBuilder<Partido> builder)
    {
        builder.ToTable("partidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id");

        builder.Property(p => p.EquipoLocalId)
            .HasColumnName("equipo_local_id")
            .IsRequired();

        builder.Property(p => p.EquipoVisitanteId)
            .HasColumnName("equipo_visitante_id")
            .IsRequired();

        builder.Property(p => p.Jornada)
            .HasColumnName("jornada")
            .IsRequired();

        builder.Property(p => p.FechaHora)
            .HasColumnName("fecha_hora")
            .IsRequired();

        builder.Property(p => p.GolesLocal)
            .HasColumnName("goles_local")
            .IsRequired(false);

        builder.Property(p => p.GolesVisitante)
            .HasColumnName("goles_visitante")
            .IsRequired(false);

        builder.Property(p => p.Estado)
            .HasColumnName("estado")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(p => p.EquipoLocal)
            .WithMany(e => e.PartidosLocales)
            .HasForeignKey(p => p.EquipoLocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.EquipoVisitante)
            .WithMany(e => e.PartidosVisitantes)
            .HasForeignKey(p => p.EquipoVisitanteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}