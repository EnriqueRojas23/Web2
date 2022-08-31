using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetEstadisticas
    {
        public int Pendiente	 {get;set;}
        public int Entregado	 {get;set;}
        public int Cerrado	 {get;set;}
        
    }
}