using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetSustentoResult
    {
        
        public long idsustento {get;set;}
        public DateTime fecharegistro {get;set;}
        public long idmanifiesto {get;set;}
        public decimal monto {get;set;}
        public decimal kminicio {get;set;}
        public decimal kmfin {get;set;}

        public long idsustentodetalle {get; set;}
        public long idtiposustento {get; set;}

        public long idtipogasto {get;set;}
        public string ruc  {get;set;}
        public string razonsocial  {get;set;}
        public string serienumero  {get;set;}
        public decimal montobase  {get;set;}
        public decimal montoigv  {get;set;}
        public decimal montototal  {get;set;}
        
    }
}