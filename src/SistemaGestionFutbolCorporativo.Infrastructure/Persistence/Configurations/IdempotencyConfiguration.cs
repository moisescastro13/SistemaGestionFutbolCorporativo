using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionFutbolCorporativo.Application.Common;

namespace MiSolucion.Infrastructure.Persistence.Configurations;
public class IdempotencyConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("idempotency_records");
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).HasMaxLength(128);
        builder.Property(x => x.Path).HasMaxLength(256).IsRequired();
        builder.Property(x => x.ResponseBody).HasColumnType("text");
    }

}