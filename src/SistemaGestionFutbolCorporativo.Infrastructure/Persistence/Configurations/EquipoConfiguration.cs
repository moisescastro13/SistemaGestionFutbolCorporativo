using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace MiSolucion.Infrastructure.Persistence.Configurations;

public class EquipoConfiguration : IEntityTypeConfiguration<Equipo>
{
    public void Configure(EntityTypeBuilder<Equipo> builder)
    {
        builder.ToTable("equipos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => e.Nombre)
            .IsUnique();

        builder.Property(e => e.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .IsRequired();
    }
}