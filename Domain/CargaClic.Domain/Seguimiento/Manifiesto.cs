using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
   public class Manifiesto : Entity
    {
        [Key]
        public long id { get; set; }
        public string numero_manifiesto { get; set; }
        public DateTime? fecha_registro { get; set; }
        public int usuario_id { get; set; }
        public int estado_id {get;set;}

        public decimal? valorizado {get;set;}
        public decimal? valorizadoFluvial {get;set;}

        public decimal? estiba {get;set;}
        public decimal? estiba_adicional {get;set;}
        public decimal? bejaranopucallpa {get;set;}
        public decimal? bejaranoiquitos {get;set;}
        public decimal? oriental {get;set;}
        public decimal? fluvial {get;set;}
        public decimal? otrosgastos {get;set;}
        public decimal? costotercero {get;set;}
        public decimal? deestiba {get;set;}
        public decimal? adicionales_tarifa {get;set;}
        public decimal? retorno_tarifa {get;set;}
        public decimal? sobreestadia_tarifa {get;set;}
        public ICollection<OrdenTransporte> Ordenes { get; set;}
        public decimal? kmrecorridos {get;set;}
        public bool? facturado {get;set;}
        public bool? adicional_facturado {get;set;}
        public bool? sobreestadia_facturado {get;set;}
        public bool? retorno_facturado {get;set;}

        public DateTime? fecha_facturado {get;set;}
        public DateTime? fecha_adicional_facturado {get;set;}
        public DateTime? fecha_sobreestadia_facturado {get;set;}
        public DateTime? fecha_retorno_facturado {get;set;}


        public string numServicio {get;set;}
        public string numAdicional {get;set;}
        public string numSobreestadia {get;set;}
        public string numRetorno {get;set;}

    }
}