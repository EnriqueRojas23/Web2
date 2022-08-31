using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetDatosIncidencia
    {
        public long id {get;set;}
        public int maestro_incidencia_id {get;set;}
        public string incidencia {get;set;}
        public string descripcion {get;set;}
        public string observacion {get;set;}
        public DateTime fecha_incidencia {get;set;}
        public DateTime fecha_registro {get;set;}
        public int usuario_id {get;set;}
        public bool? activo {get;set;}
        public string documento {get;set;}
        public string recurso {get;set;}
        
    }
}