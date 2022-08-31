
using CargaClic.Domain.Mantenimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Mantenimiento
{
    public class DistritoConfiguration : IEntityTypeConfiguration<Distrito>
    {
        public void Configure(EntityTypeBuilder<Distrito> builder)
        {
            builder.ToTable("Distrito","Mantenimiento");
            builder.HasKey(x=>x.iddistrito);
            builder.Property(x=>x.distrito).HasMaxLength(100).IsRequired();
            
        }
    }
}