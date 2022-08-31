using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Seguimiento
{
   public class MaestroIncidencia : Entity
    {
        [Key]
        public long id { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }

    }
}