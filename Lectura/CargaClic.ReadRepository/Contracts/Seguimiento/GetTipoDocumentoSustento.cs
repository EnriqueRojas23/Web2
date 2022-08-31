using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetTipoDocumentoSustento
    {
        public int id {get;set;}
        public string descripcion {get;set;}
        public string codigo {get;set;}
        public bool activo {get;set;}
        public bool requiereAprobacion {get;set;}
        
    }
}