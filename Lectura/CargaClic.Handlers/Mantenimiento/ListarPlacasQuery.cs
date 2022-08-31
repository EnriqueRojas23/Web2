

using System.Data;
using CargaClic.Contracts.Parameters.Mantenimiento;
using CargaClic.Contracts.Results.Mantenimiento;
using Common.QueryContracts;
using Common.QueryHandlers;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CargaClic.Handlers.Mantenimiento
{
    public class ListarPlacasQuery : IQueryHandler<ListarPlacasParameter>
    {
        private readonly IConfiguration _config;
        public ListarPlacasQuery(IConfiguration config)
        {
            _config = config;   
        }
        public QueryResult Execute(ListarPlacasParameter parameters)
        {
            using (var conn = new ConnectionFactory(_config).GetOpenConnection())
            {
                 var parametros = new DynamicParameters();
                 parametros.Add("Criterio", dbType: DbType.String, direction: ParameterDirection.Input, value: parameters.Criterio);
                 parametros.Add("IdProveedor", dbType: DbType.Int16, direction: ParameterDirection.Input, value: parameters.idproveedor);
                 
                 var result = new ListarPlacasResult();
                 result.Hits =  conn.Query<ListarPlacasDto>("Mantenimiento.pa_buscarplaca"
                                                                        ,parametros
                                                                        ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }
    }
}