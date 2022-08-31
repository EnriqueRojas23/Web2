using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetManifiestoResult
    {
        public long id	 {get;set;}
        public string numero_manifiesto	 {get;set;}
        public DateTime fecha_registro	 {get;set;}
        public string provincias	 {get;set;}
        public decimal peso_total {get;set;}
        public string cliente {get;set;}
        public string estado { get;set;}
        public string destino {get;set;}
        public DateTime fecha_salida {get;set;}
        public string placas {get;set;}
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
        public decimal? kmrecorridos {get;set;}
        public string facturado {get;set;}
        public bool? adicional_facturado {get;set;}
        public bool? retorno_facturado {get;set;}
        public bool? sobreestadia_facturado {get;set;}



        
        
        
    }
}
