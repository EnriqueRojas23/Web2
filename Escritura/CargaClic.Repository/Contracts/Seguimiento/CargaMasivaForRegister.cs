using System;

namespace CargaClic.Repository.Contracts.Seguimiento
{
    public class CargaMasivaForRegister
    {
        public int id { get; set; }
        public DateTime? fecha_registro { get; set; }
        public int? usuario_id { get; set; }
        public int? estado_id { get; set; }
        public string owner {get;set;}
      
        public string oc {get;set;}
        public decimal? peso_total {get;set;}
        public int cantidad_total {get;set;}
        public decimal volumen_total {get;set;}



    }
}