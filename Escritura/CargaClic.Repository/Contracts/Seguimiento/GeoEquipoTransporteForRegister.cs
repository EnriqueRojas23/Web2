using System;

namespace CargaClic.Repository.Contracts.Seguimiento
{
    public class GeoEquipoTransporteForRegister
    {
        public int id { get; set; }
        public int usuario_id { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        
    }
}