using System;
using System.ComponentModel.DataAnnotations;

using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class SustentoDetalle : Entity
    {
         [Key]
        public long id { get; set; }
        public long sustento { get; set; }
        public DateTime fecha { get; set; }
        public int tipo { get; set; }
        public int tipoSustento { get; set; }
        public string serieDocumento { get; set; }
        public string numeroDocumento { get; set; }
        public string tipoDocumentoEmisor {get;set;}
        public string documentoEmisor {get;set;}
        public string razonSocialEmisor {get;set;}
        public decimal? montoBase {get;set;}
        public decimal? montoImpuesto {get;set;}
        public decimal? montoTotal {get;set;}
        public int? usuarioAprobador { get; set; }
        public bool aprobado { get; set; }
        public DateTime? fechaAprobacion { get; set; }
        public int? usuarioAprobacion { get; set; }
        public int estado { get; set; }
        public int usuarioRegistro { get; set; }
        public DateTime fechaRegistro { get; set; }
        public DateTime? fechacarga { get; set; }
        public decimal? valorBase {get;set;}

        public decimal? costoD2 {get;set;}

    }
}