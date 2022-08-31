using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetTotalDespachos
    {
        public decimal ok { get; set; }
        public int rechazado { get; set; }
        public int total { get; set; }

        public decimal entregado { get; set; }
        public decimal pendiente { get; set; }
        public decimal noentregado { get; set; }


        public int demorado { get; set; }
        
        public int entregaparcial { get; set; }
        public int edireccion { get; set; }
        public int pendientes { get; set; }
        public int ok_cantidad {get;set;}
        public int pendientes_cantidad {get;set;}
        public int entregados_cantidad {get;set;}
        public int entregaparcial_cantidad {get;set;}
        public int noentregado_cantidad {get;set;}


    }
    public class GetDespachosATiempo
    {
        public int atiempo {get;set;}
        public int notiempo {get;set;}
    }
    public class ReporteEncuestaResult
    {
        public int nivel_satisfaccion {get;set;}
        
    }
    public class GetDespachosTiempoEntrega
    {
        public long id {get;set;}
        public int tiempo {get;set;}
        public DateTime llegada {get;set;}
        public DateTime entrega {get;set;}
    }
    public class GetDaysOfWeek
    {
        public int cantidad {get;set;}
        public string dayofw {get;set;}
    }
    public class GetCantidadxManifiesto
    {
        public long id {get;set;}
        public string numero_manifiesto {get;set;}
        public int cantidad {get;set;}
    }
    public class GetDespachosPuntualidad
    {
        public long id {get;set;}
        public string numero_ot {get;set;}
        public DateTime fecha_hora_programada {get;set;}
        public DateTime fecha_hora_llegada {get;set;}
        public int diferencia {get;set;}
    }

    public class GetAsignacionUnidades
    {
        public DateTime fecha_carga {get;set;}
        public int cantidad {get;set;}
        public int disponibilidad {get;set;}
    }
    public class GetActivityPropios
    {
        public string placa {get;set;}
        public string NombreCompleto {get;set;}
        public string razon_social {get;set;}
        public string nombreEstado {get;set;}
        public int proveedor_id {get;set;}
    }
    public class GetActivityTotal
    {   
        public string razon_social {get;set;}
        public int total {get;set;}
        public string tipo {get;set;}
    }
    public class GetActivityVehiculoRuta
    {   
        public string placa {get;set;}
        public int estado_id {get;set;}
    }
    public class GetActivityOTsTotalesYEntregadas
    {
        public int EnTransito {get;set;}
    }
    public class ReporteServicioResult
    {
        public string razon_social {get;set;}
        public int ots {get;set;}
        public decimal costo {get;set;}
        public decimal valorizado {get;set;}
        public string fecha_carga {get;set;}
    }
     public class GetReporteServicioResult
    {
        
        public int vehiculos {get;set;}
        public string mes2 {get;set;}
        
    }
}