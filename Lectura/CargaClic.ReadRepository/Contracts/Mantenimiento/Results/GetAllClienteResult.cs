namespace CargaClic.ReadRepository.Contracts.Mantenimiento.Results
{
    public class GetAllClientesResult
    {
        public int id	{get;set;}
        public string razon_social	{get;set;}
        public string ruc	{get;set;}
        

    }
    public class GetAllTarifas 
    {
        public int id {get;set;}
        public int idcliente {get;set;}
        public int? iddistrito_origen {get;set;}
        public int? iddistrito_destino {get;set;}
        public int? idprovincia_origen {get;set;}
        public int? idprovincia_destino {get;set;}
        public int? iddepartamento_origen {get;set;}
        public int? iddepartamento_destino {get;set;}
        public int? idtipounidad {get;set;}
        public decimal? tarifa {get;set;}
 
    }
}