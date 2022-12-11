using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CargaClic.Data;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;
using CargaClic.ReadRepository.Interface.Seguimiento;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CargaClic.ReadRepository.Repository.Seguimiento
{
    public class ManifiestoReadRepository : IManifiestoReadRepository
    {
         private readonly DataContext _context;
            private readonly IConfiguration _config;

            public ManifiestoReadRepository(DataContext context,IConfiguration config)
            {
                _context = context;
                _config = config;
            }
            public IDbConnection Connection
            {   
                get
                {
                    return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                }
            }
            
        public async Task<IEnumerable<GetManifiestoResult>> GetAllManifiestoCliente(int ClienteId)
        {
             var parametros = new DynamicParameters();
            parametros.Add("clienteid", dbType: DbType.Int32, direction: ParameterDirection.Input, value: ClienteId);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[seguimiento].[pa_listar_manifiesto_clientes]";
                conn.Open();
                var result = await conn.QueryAsync<GetManifiestoResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                ); 
                return result;
            }
        }

        public async Task<IEnumerable<GetManifiestoResult>> GetAllManifiestos(int ChoferId)
        {
            var parametros = new DynamicParameters();
            parametros.Add("choferid", dbType: DbType.Int32, direction: ParameterDirection.Input, value: ChoferId);

            using (IDbConnection conn = Connection)
            {
                string sQuery = "[seguimiento].[pa_listar_manifiesto]";
                conn.Open();
                var result = await conn.QueryAsync<GetManifiestoResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                ); 
                return result;
            }
        }

        public async Task<IEnumerable<GetManifiestoResult>> GetAllManifiestos(string ids, int idusuario, string inicio, string fin, int? tiposervicioid)
        {
             var parametros = new DynamicParameters();
             parametros.Add("remitente_id", dbType: DbType.String, direction: ParameterDirection.Input, value: ids);
             parametros.Add("usuario_id", dbType: DbType.Int32, direction: ParameterDirection.Input, value: idusuario);
             parametros.Add("fecini", dbType: DbType.String, direction: ParameterDirection.Input, value: inicio);
             parametros.Add("fecfin", dbType: DbType.String, direction: ParameterDirection.Input, value: fin);
             parametros.Add("tiposervicioid", dbType: DbType.Int32, direction: ParameterDirection.Input, value: tiposervicioid);


            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Seguimiento].[pa_listarmanifiestosxmes]";
                conn.Open();
                var result = await conn.QueryAsync<GetManifiestoResult>(sQuery,
                                                                    parametros
                                                                    ,commandType:CommandType.StoredProcedure
                  ); 
                return result;
            }
        }
    }
}