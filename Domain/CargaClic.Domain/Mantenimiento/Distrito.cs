using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

namespace CargaClic.Domain.Mantenimiento
{
    public class Distrito : Entity
    {
        [Key]
        public int iddistrito {get;set;}
        public string distrito {get;set;}
        public int idprovincia {get;set;}
    }
}