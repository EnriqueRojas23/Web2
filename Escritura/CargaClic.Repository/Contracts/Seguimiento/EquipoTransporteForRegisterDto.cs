using System;
using System.ComponentModel.DataAnnotations;

namespace CargaClic.API.Dtos.Recepcion
{
    public class EquipoTransporteForRegisterDto
    {
      

        public string Carreta {get;set;}


        [Required]
        public string Placa{ get;set; }
        [Required]
        public string Ruc{ get;set; }
        [Required]
        public string Dni{ get;set; }
   
    

        [Required]
        public string OrdenTransporteId {get;set;}

    }
}