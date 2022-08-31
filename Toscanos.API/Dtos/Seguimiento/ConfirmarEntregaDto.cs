using System;
using System.ComponentModel.DataAnnotations;

namespace CargaClic.API.Dtos.Matenimiento
{
    public class ConfirmarEntregaDto
    {
        public long? idetapa { get; set; }
        public int idordentrabajo { get; set; }
        public string descripcion { get; set; }
        public string horaetapa { get; set; }
        public string recurso { get; set; }
        public string documento { get; set; }
        public int idmaestroetapa {get;set;}
        public int idtipoentrega {get;set;}
     
        
        [DateTimeModelBinder(DateFormat = "dd/MM/yyyy")]
        public DateTime fechaetapa { get; set; }

        [DateTimeModelBinder(DateFormat = "dd/MM/yyyy")]
        public DateTime? fecharegistro { get; set; }
        public int? idusuarioregistro { get; set; }
        public bool visible { get; set; }
        public int idestado { get; set; }
        public int idusuarioentrega { get; set; }
        public decimal lat {get;set;}
        public decimal lng {get;set;}
        public int usuario_id {get;set;}

        public string reconocimiento_embarque {get;set;}
        public string numero_lancha {get;set;}


    }
}