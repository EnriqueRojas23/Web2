using System;
using System.ComponentModel.DataAnnotations;

namespace CargaClic.API.Dtos.Matenimiento
{
    public class IncidenciaForRegister
    {
        
        [Required]
        public long id { get; set; }
        [Required]
        public int maestro_incidencia_id { get; set; }
        [Required]
        public long? orden_trabajo_id { get; set; }
        public string descripcion { get; set; }
        public string observacion { get; set; }
        public DateTime? fecha_incidencia { get; set; }
        public DateTime? fecha_registro { get; set; }
        public int? usuario_id { get; set; }
        public bool? activo { get; set; }
        public string documento { get; set; }
        public string recurso { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public int? usuario_modificacion { get; set; }
        
    }
   
}