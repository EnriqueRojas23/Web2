using System;

namespace CargaClic.ReadRepository.Contracts.Seguimiento.Results
{
    public class GetDocumentoSustento
    {
        public long id {get;set;}
        public long sustento {get;set;}
        public string fecha {get;set;}
        public int tipo {get;set;}
        public string descTipo {get;set;}
        public int tipoSustento {get;set;}
        public string descTipoSustento {get;set;}
        public string serieDocumento {get;set;}
        public string numeroDocumento {get;set;}
        public string documento {get;set;}
        public string tipoDocumentoEmisor {get;set;}
        public string documentoEmisor {get;set;}
        public string razonSocialEmisor {get;set;}
        public decimal montoBase {get;set;}
        public decimal montoImpuesto {get;set;}
        public decimal montoTotal {get;set;}
        public int? usuarioAprobador {get;set;}
        public string nombreUsuarioAprobador {get;set;}
        public string fechaAprobacion {get;set;}
        public int? usuarioAprobacion {get;set;}
        public string nombreUsuarioAprobacion {get;set;}
        public int estado {get;set;}
        public string NombreEstado {get;set;}
        public int usuarioRegistro {get;set;}
        public string fechaRegistro {get;set;}

        public string fechacarga {get;set;}
        public decimal valorBase {get;set;}
        public decimal costoD2 {get;set;}
        
    }
}