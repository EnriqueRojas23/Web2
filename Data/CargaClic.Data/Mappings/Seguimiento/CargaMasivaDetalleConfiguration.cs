

using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Prerecibo
{
    public class CargaMasivaDetalleConfiguration: IEntityTypeConfiguration<CargaMasivaDetalle>
    {
        public void Configure(EntityTypeBuilder<CargaMasivaDetalle> builder)
        {
            builder.ToTable("CargaMasivaDetalle","Seguimiento");
            builder.HasKey(x=>x.id);

        }
    }
}