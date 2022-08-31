namespace CargaClic.Common
{
    public sealed class Constantes
    {
        public enum EstadoOrdenTransporte : int
        {
            PendienteProgramacion = 4,
            Programado = 5,
            EnEsperaCarga = 6,
            DespachoValidado = 7,
            EnRuta = 8,
            LlegadaDestino = 9,
            Descargando = 10,
            TerminoDescarga = 11,
            Finalizado = 12,
            Cerrado = 13,
            FinRecojo = 36,
            EnTransitoFluvial = 40,
            Desembarcando = 41
        }
        public enum EstadoManifiesto : int
        {
            Registrado = 37,
            Finalizado = 38
        }

    
   
        public enum EstadoEquipoTransporte : int
        {
            EnProceso = 13,
            Asignado = 14,
            EnDescarga = 15,
            Cerrado = 16,
        }
       

      
    }
}