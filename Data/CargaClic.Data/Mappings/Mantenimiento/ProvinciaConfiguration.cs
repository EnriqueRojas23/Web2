
using CargaClic.Domain.Mantenimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Mantenimiento
{
    public class ProvinciaConfiguration : IEntityTypeConfiguration<Provincia>
    {
        public void Configure(EntityTypeBuilder<Provincia> builder)
        {
            builder.ToTable("Provincia","Mantenimiento");
            builder.HasKey(x=>x.idprovincia);
            builder.Property(x=>x.provincia).HasMaxLength(100).IsRequired();
            
        }
    }
}