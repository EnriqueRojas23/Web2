using System;

namespace CargaClic.Repository.Contracts.Seguimiento
{
    public class CargaMasivaDetalleForRegister
    {
            public long? id { get; set; }
            public string shipment { get; set; }
            public string delivery { get; set; }
            public bool asignado { get; set; }
            public string remitente { get; set; }
            public string destinatario { get; set; }
            public string factura { get; set; }
            public string oc { get; set; }
            public string guias {get;set;}
            public int? cantidad { get; set; }
            public decimal volumen { get; set; }
            public decimal peso { get; set; }
            public string tiposervicio { get; set; }
            public string distrito_carga { get; set; }
            public string direccion_carga { get; set; }
            public DateTime fecha_carga { get; set; }
            public string hora_carga { get; set; }
            public string direccion_destino_servicio { get; set; }
            public string distrito_destino_servicio { get; set; }
            public DateTime fecha_salida { get; set; }
            public string hora_salida { get; set; }
            public string direccion_entrega { get; set; }
            public string provincia { get; set; }
            public DateTime fecha_entrega { get; set; }
            public string hora_entrega { get; set; }
            public string tracto { get; set; }
            public string carreta { get; set; }
            public string conductor { get; set; }
            public int? carga_id { get; set; }  
            public string recojo {get;set;}
            public string notificacion {get;set;}
            public decimal? costo {get;set;}
            public decimal? valorizado {get;set;}
           public decimal? estiba {get;set;}
           public string owner {get;set;}

           public string numero_remesa {get;set;}
            
    }

    
}