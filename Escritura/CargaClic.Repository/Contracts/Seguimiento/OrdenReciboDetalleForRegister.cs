using System;


namespace  CargaClic.Repository.Contracts.Seguimiento
{
    public class OrdenTransporteDto
    {
         public long? id { get; set; }
         
    }
    public class OrdenTransporteForRegister
    {
        public long? id { get; set; }
        
        public string shipment { get; set; }
        public string delivery { get; set; }
        public bool? por_asignar { get; set; }
        public int? remitente_id { get; set; }
        public int? destinatario_id { get; set; }
        public string factura { get; set; }
        public string oc { get; set; }
        public int? cantidad { get; set; }
        public decimal? volumen { get; set; }
        public decimal? peso { get; set; }
        public int? tiposervicio_id { get; set; }
        public int? distrito_carga_id { get; set; }
        public string direccion_carga { get; set; }
        public DateTime? fecha_carga { get; set; }
        public string hora_carga { get; set; }
        public int? distrito_destino_id { get; set; }
        public string direccion_destino { get; set; }
        public DateTime? fecha_salida { get; set; }
        public string hora_salida { get; set; }
        public string direccion_entrega { get; set; }
        public int? provincia_entrega { get; set; }
        public DateTime? fecha_entrega { get; set; }
        public string hora_entrega { get; set; }
        public int? equipo_transporte_id { get; set; }
        public long? manifiesto_id { get; set; }

    }
}