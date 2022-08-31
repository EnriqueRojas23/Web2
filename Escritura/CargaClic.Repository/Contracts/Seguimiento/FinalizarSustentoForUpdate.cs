using System;

namespace CargaClic.Repository.Contracts.Seguimiento
{
    public class FinalizarSustentoForUpdate
    {
        public long sustento { get; set; }
        public long manifiesto { get; set; }
        public decimal? monto { get; set; }
        public decimal? kilometrajeInicio { get; set; }
        public decimal? kilometrajefinal {get;set;}
        public int usuario { get; set; }

    }
}
