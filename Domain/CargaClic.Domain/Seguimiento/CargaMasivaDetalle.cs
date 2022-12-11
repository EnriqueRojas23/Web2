using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class CargaMasivaDetalle : Entity
    {
            [Key]
            public long id { get; set; }
            [MaxLength(30)]
            public string shipment { get; set; }
            [MaxLength(30)]
            public string delivery { get; set; }
            public bool asignado { get; set; }
            [MaxLength(50)]
            public string remitente { get; set; }
            [MaxLength(50)]
            public string destinatario { get; set; }
            [MaxLength(100)]
            public string factura { get; set; }
            [MaxLength(100)]
           
            public string guias {get;set;}
            public int? cantidad { get; set; }
            public decimal volumen { get; set; }
            public decimal peso { get; set; }
            [MaxLength(30)]
            public string tiposervicio { get; set; }
            [MaxLength(30)]
            public string distrito_carga { get; set; }
            [MaxLength(100)]
            public string direccion_carga { get; set; }
            public DateTime fecha_carga { get; set; }
            public string hora_carga { get; set; }
            [MaxLength(100)]
            public string direccion_destino_servicio { get; set; }
            [MaxLength(30)]
            public string distrito_destino_servicio { get; set; }
            public DateTime fecha_salida { get; set; }
            [MaxLength(30)]
            public string hora_salida { get; set; }
            public string direccion_entrega { get; set; }
            public string provincia { get; set; }
            public DateTime fecha_entrega { get; set; }
            public string hora_entrega { get; set; }
            public string tracto { get; set; }
            public string carreta { get; set; }
            public string conductor { get; set; }
            public int? carga_id { get; set; }
            public bool? recojo {get;set;}
            public string notificacion {get;set;}

            public decimal? costo {get;set;}
            public decimal? valorizado {get;set;}
            public decimal? estiba {get;set;}


            public string incidencia {get;set;}
            public string contacto {get;set;}
            public string tipo_cedible {get;set;}
            public DateTime? fecha_cedible {get;set;}

            public string boleto {get;set;}

             public string oc { get; set; }
             public string owner {get;set;}
             public bool error {get;set;}
             public string detalleerror {get;set;}

    }
}