using System;
using System.ComponentModel.DataAnnotations;

namespace CargaClic.Repository.Contracts.Mantenimiento
{
   
    public class ClienteForRegister
    {
        [Required]
        public string razon_social {get;set;}
        [Required]
        public string ruc {get;set;}
        public string mail_notificacion {get;set;}
        
    }
     public class ClienteForUpdate
    {
        [Required]
        public int id {get;set;}
        [Required]
        public string razon_social {get;set;}
        [Required]
        public string ruc {get;set;}
        
        public string mail_notificacion {get;set;}
        
    }
   
}