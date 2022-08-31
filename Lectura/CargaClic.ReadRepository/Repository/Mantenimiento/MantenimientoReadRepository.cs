using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CargaClic.Common;
using CargaClic.Data;

using CargaClic.Domain.Mantenimiento;
using CargaClic.ReadRepository.Contracts.Mantenimiento.Results;
using CargaClic.ReadRepository.Interface.Mantenimiento;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CargaClic.Handlers.Mantenimiento
{
    public class MantenimientoReadRepository : IMantenimientoReadRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public MantenimientoReadRepository(DataContext context,IConfiguration config)
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

  
        public async Task<IEnumerable<GetAllClientesResult>> GetAllClientes(string Criterio, int UsuarioId)
        {
            var parametros = new DynamicParameters();
            parametros.Add("usuarioid", dbType: DbType.Int32, direction: ParameterDirection.Input, value: UsuarioId);
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Mantenimiento].[pa_listarclientes]";
                conn.Open();
                var result = await conn.QueryAsync<GetAllClientesResult>(sQuery, parametros ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }

     

        public async Task<GetAllHuellaResult> GetHuella(int HuellaId)
        {
            var parametros = new DynamicParameters();
            parametros.Add("HuellaId", dbType: DbType.Int32, direction: ParameterDirection.Input, value: HuellaId);
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Mantenimiento].[pa_obtenerhuella]";
                conn.Open();
                var result = await conn.QueryAsync<GetAllHuellaResult>(sQuery, parametros ,commandType:CommandType.StoredProcedure);
                return result.SingleOrDefault();
            }
        }

       

       
        public async Task<IEnumerable<GetAllDireccionesResult>> GetAllDirecciones(int ClienteId)
        {
            var parametros = new DynamicParameters();
            parametros.Add("idcliente", dbType: DbType.Int32, direction: ParameterDirection.Input, value: ClienteId);
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Mantenimiento].[pa_listar_direccionesxcliente]";
                conn.Open();
                var result = await conn.QueryAsync<GetAllDireccionesResult>(sQuery, parametros ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<GetAllDepartamentos>> GetAllDepartamentos()
        {
            var parametros = new DynamicParameters();
            
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[seguimiento].[pa_listardepartamento]";
                conn.Open();
                var result = await conn.QueryAsync<GetAllDepartamentos>(sQuery, parametros ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<GetAllProvincias>> GetAllProvincias(int DepartamentoId)
        {
            var parametros = new DynamicParameters();
            parametros.Add("iddepartamento", dbType: DbType.Int32, direction: ParameterDirection.Input, value: DepartamentoId);
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[seguimiento].[pa_listarprovincia]";
                conn.Open();
                var result = await conn.QueryAsync<GetAllProvincias>(sQuery, parametros ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<GetAllDistritos>> GetAllDistritos(int ProvinciaId)
        {
            var parametros = new DynamicParameters();
            parametros.Add("idprovincia", dbType: DbType.Int32, direction: ParameterDirection.Input, value: ProvinciaId);
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[seguimiento].[pa_listardistritos]";
                conn.Open();
                var result = await conn.QueryAsync<GetAllDistritos>(sQuery, parametros ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<Proveedor>> GetAllProvedores()
        {
            var parametros = new DynamicParameters();
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Mantenimiento].[pa_listarproveedores]";
                conn.Open();
                var result = await conn.QueryAsync<Proveedor>(sQuery, parametros ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<GetAllTarifas[]>> GetAllTarifas(int ClienteId)
        {
            var parametros = new DynamicParameters();
            using (IDbConnection conn = Connection)
            {
                string sQuery = "[Mantenimiento].[pa_listartarifario]";
                conn.Open();
                var result = await conn.QueryAsync<GetAllTarifas[]>(sQuery, parametros ,commandType:CommandType.StoredProcedure);
                return result;
            }
        }
    }
}