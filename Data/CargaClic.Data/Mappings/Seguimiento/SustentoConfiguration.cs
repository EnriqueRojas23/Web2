
using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Seguimiento
{
    public class SustentoConfiguration: IEntityTypeConfiguration<Sustento>
    {
        public void Configure(EntityTypeBuilder<Sustento> builder)
        {
            builder.ToTable("Sustento","Seguimiento");
            builder.HasKey(x=>x.id);
            
        }
    }
}