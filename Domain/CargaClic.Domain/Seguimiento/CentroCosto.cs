using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class CentroCosto : Entity
    {
        [Key]
        public long manifiesto_id {get;set;}
        public bool? estiba_facturado {get;set;}
        public string estiba_numerodoc {get;set;}
        public DateTime? estiba_fecha {get;set;}

        public bool? estibaadicional_facturado {get;set;}
        public string estibaadicional_numerodoc {get;set;}
        public DateTime? estibaadicional_fecha {get;set;}


        public bool? bejarano_pucallpa_facturado {get;set;}
        public DateTime? bejarano_pucallpa_fecha {get;set;}
        public string bejarano_pucallpa_numerodoc {get;set;}


        public bool? bejarano_iquitos_facturado {get;set;}
        public DateTime? bejarano_iquitos_fecha {get;set;}
        public string bejarano_iquitos_numerodoc {get;set;}

        public bool? oriental_facturado {get;set;}
        public DateTime? oriental_fecha {get;set;}
        public string oriental_numerodoc {get;set;}


        public bool? fluvial_facturado {get;set;}
        public DateTime? fluvial_fecha {get;set;}
        public string fluvial_numerodoc {get;set;}




    }
}