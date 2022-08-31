using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetSustentoManifiesto
    {
        public long id {get;set;}
        public long manifiesto {get;set;}
        public DateTime fecha {get;set;}
        public decimal monto {get;set;}
        public decimal kilometrajeInicio {get;set;}
        public decimal kilometrajefinal {get;set;}
        public int usuarioRegistro { get;set;}
        public DateTime fechaRegistro {get;set;}
        public string numero_manifiesto {get;set;}
        public string chofer {get;set;}
        public string destino {get;set;}
        public decimal totalsustentado {get;set;}
        
    }
}
