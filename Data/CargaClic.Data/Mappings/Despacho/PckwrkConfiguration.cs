
using CargaClic.Domain.Despacho;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Prerecibo
{
    public class PckwrkConfiguration: IEntityTypeConfiguration<Pckwrk>
    {
        public void Configure(EntityTypeBuilder<Pckwrk> builder)
        {
            builder.ToTable("Pckwrk","Despacho");
            builder.HasKey(x=>x.Id);
        }
    }
}