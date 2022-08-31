using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Mantenimiento
{
    public class Provincia : Entity
    {
        [Key]
        public int idprovincia {get;set;}
        public string provincia {get;set;}
        public int iddepartamento {get;set;}
    }
}