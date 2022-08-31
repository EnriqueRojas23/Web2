
using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Seguimiento
{
    public class GeoEquipoTransporteConfiguration: IEntityTypeConfiguration<GeoEquipoTransporte>
    {
        public void Configure(EntityTypeBuilder<GeoEquipoTransporte> builder)
        {
            builder.ToTable("GeoEquipoTransporte","Seguimiento");
            builder.HasKey(x=>x.id);
            
        }
    }
}