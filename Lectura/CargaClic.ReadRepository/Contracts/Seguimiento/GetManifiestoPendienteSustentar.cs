using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetManifiestoPendienteSustentar
    {
        public int Id {get;set;}
        public string NombreCompleto {get;set;}
        public string Dni {get;set;}
        public string Brevete {get;set;}
        public string Telefono {get;set;}
        public int UsuarioId {get;set;}
        public long idManifiesto {get;set;}
        public DateTime fechaManifiesto {get;set;}
        public string numero_manifiesto {get;set;}
        public string provincia {get;set;}
        public string tracto {get;set;}
        public string carreta {get;set;}
        public int estadoManifiesto {get;set;}        
        public decimal valorizado {get;set;}
        public string cliente {get;set;}
    }
}
