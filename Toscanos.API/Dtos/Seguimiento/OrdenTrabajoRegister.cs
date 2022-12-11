using System;
using System.Collections.Generic;

public class OrdenTrabajoDtoMasivo {


        public long? id { get; set; }
        public string numero_ot { get; set; }
        public string shipment { get; set; }
        public string delivery { get; set; }
        public bool por_asignar { get; set; }
        public int estado_id {get;set;}
        public int? remitente_id { get; set; }
        public int? destinatario_id { get; set; }
        public string destinatario {get;set;}
        public string factura { get; set; }
        public string oc { get; set; }
        public string guias {get;set;}
        public int? cantidad { get; set; }
        public decimal? volumen { get; set; }
        public decimal? peso { get; set; }
        public int? tiposervicio_id { get; set; }
        public int? distrito_carga_id { get; set; }
        public string direccion_carga { get; set; }
        public DateTime? fecha_carga { get; set; }
        public string hora_carga { get; set; }
        public int? distrito_destino_servicio_id { get; set; }
        public string direccion_destino_servicio { get; set; }
        public DateTime? fecha_salida { get; set; }
        public string hora_salida { get; set; }
        public string direccion_entrega { get; set; }
        public int? provincia_entrega { get; set; }
        public DateTime? fecha_entrega { get; set; }
        public string hora_entrega { get; set; }
        public long? equipo_transporte_id { get; set; }
        public long manifiesto_id { get; set; }
        public DateTime fecha_registro {get;set;}
        public decimal lat_entrega {get;set;}
        public decimal lng_entrega {get;set;}

        public decimal? lat_carga {get;set;}
        public decimal? lng_carga {get;set;}


        public int usuario_registro_id {get;set;}
        public bool activo {get ;set;}
        public string reconocimento_embarque {get;set;}
        public string  numero_lancha {get;set;}
        public int? tipo_entrega_id {get;set;}
        public bool? recojo {get;set;}
        public string notificacion {get;set;}
        public int? nivel_satisfaccion {get;set;}
        public string observacion_satisfaccion {get;set;}

        public decimal? costo {get;set;}
        public decimal? valorizado {get;set;}
        public int? cantidad_fotos {get;set;}


        public decimal? lat_waypoint {get;set;}
        public decimal? lng_waypoint {get;set;}
        public string orden_entrega {get;set;}
        public DateTime? fecha_eta {get;set;}
        public string errores {get;set;}
}

public class OrdenTrabajoForRegister {

        public string emp { get; set; }
        public DateTime fecha_carga {get;set;}
        
        public string OC {get;set;}
        public decimal? peso_total {get;set;}
        public decimal? cantidad_total {get;set;}
        public decimal? volumen_total {get;set;}

        public List<OrdenTrabajoDetalleForRegister> documentos {get;set;}

 }
   public class CargaMasivaDto {
            public long id {get;set;}
            public int idcliente {get;set;}
            public int idusuario {get;set;}
        }