using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetMaestroIncidencia
    {
        public int id {get;set;}
        public string descripcion {get;set;}
        public bool activo {get;set;}        
    }
}