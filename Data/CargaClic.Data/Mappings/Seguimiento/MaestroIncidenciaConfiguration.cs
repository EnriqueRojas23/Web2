
using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Seguimiento
{
    public class MaestroIncidenciaConfiguration: IEntityTypeConfiguration<MaestroIncidencia>
    {
        public void Configure(EntityTypeBuilder<MaestroIncidencia> builder)
        {
            builder.ToTable("MaestroIncidencia","Seguimiento");
            builder.HasKey(x=>x.id);
            
        }
    }
}