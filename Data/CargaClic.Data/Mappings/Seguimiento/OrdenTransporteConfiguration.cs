
using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Seguimiento
{
    public class OrdenTransporteConfiguration: IEntityTypeConfiguration<OrdenTransporte>
    {
        public void Configure(EntityTypeBuilder<OrdenTransporte> builder)
        {
            builder.ToTable("OrdenTransporte","Seguimiento");
            builder.HasKey(x=>x.id);
            
            

         
            

            
        }
    }
}