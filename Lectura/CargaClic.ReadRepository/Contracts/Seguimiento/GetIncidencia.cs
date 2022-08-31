using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetIncidencia
    {
        public long id {get;set;}
        public string incidencia {get;set;}
        public DateTime fecha_incidencia {get;set;}
        public string observacion {get;set;}
        public string usuario_registro {get;set;}
        
    }
}