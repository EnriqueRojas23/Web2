using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class GeoEquipoTransporte : Entity
    {
        [Key]
        public long id { get; set; }
        public long equipo_transporte_id { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }

    }
}