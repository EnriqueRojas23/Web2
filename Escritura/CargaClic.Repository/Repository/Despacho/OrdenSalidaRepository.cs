
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CargaClic.API.Dtos.Despacho;
using CargaClic.API.Dtos.Recepcion;
using CargaClic.Common;
using CargaClic.Data;
using CargaClic.Domain.Despacho;
using CargaClic.Domain.Inventario;
using CargaClic.Repository.Contracts.Despacho;
using CargaClic.Repository.Contracts.Inventario;
using CargaClic.Repository.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CargaClic.Repository
{
    public class OrdenSalidaRepository : IOrdenSalidaRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public OrdenSalidaRepository(DataContext context,IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public IDbConnection Connection
        {   
            get
            {
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                try
                {
                     connection.Open();
                     return connection;
                }
                catch (System.Exception)
                {
                    connection.Close();
                    connection.Dispose();
                    throw;
                }
            }
        }

     
        


      
        

       
   
       
        
    }
}