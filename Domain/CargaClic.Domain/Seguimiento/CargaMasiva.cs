using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
   public class CargaMasiva
    {
        [Key]
        public int id { get; set; }
        public DateTime? fecha_registro { get; set; }
        public int? usuario_id { get; set; }
        public int? estado_id { get; set; }
        public int? cantidad_total {get;set;}
        public decimal? peso_total {get;set;}
        public string oc {get;set;}
        public string owner {get;set;}

    }
}