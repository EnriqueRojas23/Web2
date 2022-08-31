using CargaClic.Common;

namespace CargaClic.Domain.Mantenimiento
{
    public class Cliente : Entity
    {
        public int id {get;set;}
        public string razon_social {get;set;}
        public string ruc {get;set;}
        public string mail_notificacion {get;set;}
   
    }
}