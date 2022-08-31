using System;
using System.Collections.Generic;

public class OrdenTrabajoForRegister {

        public string emp { get; set; }
        public DateTime fecha_carga {get;set;}
        
        public string OC {get;set;}
        public decimal? peso_total {get;set;}
        public decimal? cantidad_total {get;set;}
        public decimal? volumen_total {get;set;}

        public List<OrdenTrabajoDetalleForRegister> documentos {get;set;}

 }