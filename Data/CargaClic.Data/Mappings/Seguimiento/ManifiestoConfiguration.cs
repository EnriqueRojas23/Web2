
using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Seguimiento
{
    public class ManifiestoConfiguration: IEntityTypeConfiguration<Manifiesto>
    {
        public void Configure(EntityTypeBuilder<Manifiesto> builder)
        {
            builder.ToTable("Manifiesto","Seguimiento");
            builder.HasKey(x=>x.id);
            builder.Property(x=>x.fecha_registro).IsRequired();
            builder.Property(x=>x.usuario_id).IsRequired();
            
        }
    }
}