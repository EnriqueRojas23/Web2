
using System;

public class ObtenerOrdenTransporteDto 
    {
        public long id	 {get;set;}
        public string numero_ot	 {get;set;}
        public string shipment	 {get;set;}
        public string delivery	 {get;set;}
        public string destinatario	 {get;set;}
        public string remitente	 {get;set;}
        public bool por_asignar	 {get;set;}
        public int remitente_id	 {get;set;}
        public int destinatario_id	 {get;set;}
        public string factura	 {get;set;}
        public string oc	 {get;set;}
        public int cantidad	 {get;set;}
        public decimal volumen	 {get;set;}
        public decimal peso	 {get;set;}
        public int tiposervicio_id	 {get;set;}
        public int distrito_carga_id	 {get;set;}
        public string distrito_carga	 {get;set;}
        public string direccion_carga	 {get;set;}
        public DateTime fecha_carga	 {get;set;}
        public string hora_carga	 {get;set;}
        public string distrito_servicio	 {get;set;}
        public string direccion_destino_servicio	 {get;set;}
        public DateTime fecha_salida	 {get;set;}
        public string hora_salida	 {get;set;}
        public string direccion_entrega	 {get;set;}
        public string provincia_entrega	 {get;set;}
        public string hora_entrega	 {get;set;}
        public DateTime? fecha_entrega	 {get;set;}
        public string numero_manifiesto	 {get;set;}
        public string Estado	 {get;set;}
        public string Tracto	 {get;set;}
        public string Carreta	 {get;set;}
        public string Chofer	 {get;set;}
        public string dni {get;set;}
        public DateTime fecha_registro	 {get;set;}
        public int manifiesto_id	 {get;set;}
        public string TipoServicio	 {get;set;}
        public int estado_id {get;set;}
        public string  notificacion{get;set;}
        public int nivel_satisfaccion {get;set;}
        public string observacion_satisfaccion {get;set;}

        public decimal lat_entrega {get;set;}
        public decimal lng_entrega {get;set;}
        public int tipo_entrega_id {get;set;}
        
    }