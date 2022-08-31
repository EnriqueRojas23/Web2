using System;

namespace CargaClic.Repository.Contracts.Seguimiento
{
    public class IncidenciaForUpdate
    {
            public long id { get; set; }
            public DateTime fecha { get; set; }            
            public int incidencia { get; set; }
            public string observacion { get; set; }
            public int usuario { get; set; }

    }

    
}