using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
    public class Sustento : Entity
    {
        [Key]
        public long id { get; set; }
        public long manifiesto { get; set; }
        public DateTime fecha   { get; set; }        
        public decimal? monto { get; set; }
        public decimal? kilometrajeInicio { get; set; }
        public decimal? kilometrajefinal {get;set;}
        public int usuarioRegistro { get; set; }
        public DateTime fechaRegistro   { get; set; }

    }
}