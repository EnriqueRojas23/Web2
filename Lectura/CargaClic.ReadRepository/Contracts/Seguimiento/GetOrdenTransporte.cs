using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetOrdenTransporte
    {
        public long id	 {get;set;}
        public string numero_ot {get;set;}
        public string shipment {get;set;}
        public string delivery {get;set;}
        public string destinatario {get;set;}
        public string remitente {get;set;}
        public bool por_asignar {get;set;}
        public int remitente_id {get;set;}
        public string destinatario_id {get;set;}
        public string factura {get;set;}
        public string oc {get;set;}
        public int cantidad {get;set;}
        public decimal volumen {get;set;}
        public decimal peso {get;set;}
        public int tiposervicio_id {get;set;}
        public int distrito_carga_id {get;set;}
        public string distrito_carga {get;set;}
        public string direccion_carga {get;set;}
        public DateTime fecha_carga {get;set;}
        public string hora_carga {get;set;}
        public string distrito_servicio {get;set;}
        public string direccion_destino_servicio {get;set;}
        public DateTime fecha_salida {get;set;}
        public DateTime fecha_registro {get;set;}
        public DateTime fecha_entrega {get;set;}
        public string hora_salida {get;set;}
        public string direccion_entrega {get;set;}
        public string provincia_entrega	 {get;set;}
        public string hora_entrega	 {get;set;}
        public string numero_manifiesto	 {get;set;}
        public long manifiesto_id {get;set;}
        public string Tracto	 {get;set;}
        public string Carreta	 {get;set;}
        public string Chofer {get;set;}
        public string Estado {get;set;}
        public string TipoServicio {get;set;}
        public int estado_id {get;set;}
        public string button {get;set;}
        public string usuario_registro {get;set;    }
        public string guias {get;set;}
        public string TipoEntrega {get;set;}
        public int cantidadFiles {get;set;}
        public bool recojo {get;set;}
        public int nivel_satisfaccion {get;set;}
        public decimal?  lat_waypoint {get;set;}
        public decimal? lng_waypoint {get;set;}
        public string orden_entrega {get;set;}
        public DateTime? fecha_eta {get;set;}

        public decimal? lat_entrega {get;set;}
        public decimal? lng_entrega {get;set;}

        public decimal? valorizado {get;set;}

        public decimal?  retorno_tarifa       {get;set;}
        public decimal? sobreestadia_tarifa {get;set;}
        public decimal? adicionales_tarifa   {get;set;}


        public string placa {get;set;}
        
        public int pendientes {get;set;}
        public int entregados {get;set;}
        public int total {get;set;}

        public decimal avance {get;set;}
        public int EntregadosOK {get;set;}
        public int EntregadosParcial {get;set;}
        public int EntregadosRechazo {get;set;}


        
    }
}