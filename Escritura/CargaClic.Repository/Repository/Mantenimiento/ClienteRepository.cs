using System;
using System.Threading.Tasks;
using CargaClic.Data;
using CargaClic.Domain.Mantenimiento;
using CargaClic.Repository.Contracts.Mantenimiento;
using CargaClic.Repository.Interface.Mantenimiento;
using Microsoft.Extensions.Configuration;

namespace CargaClic.Repository.Repository.Mantenimiento
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public ClienteRepository(DataContext context,IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<int> ClientRegister(ClienteForRegister clienteForRegister)
        {
          
            Cliente cliente;
            

          
            using(var transaction = _context.Database.BeginTransaction())
            {
                  try
                  {
                    cliente = new Cliente();
                    cliente.ruc = clienteForRegister.ruc;
                    cliente.razon_social = clienteForRegister.razon_social;
                    cliente.mail_notificacion = clienteForRegister.mail_notificacion;
                    

                    await  _context.Cliente.AddAsync(cliente);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                  }
                  catch (System.Exception ex)
                  {
                        transaction.Rollback(); 
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        if (sqlException.Number == 2601 || sqlException.Number == 2627)
                            throw new ArgumentException("El cliente ya existe");
                        else
                            throw new ArgumentException("Error al insertar");
                  }
                  return cliente.id;
                 
            }
        }


        public async Task<int> AddressRegister(AddressForRegister addressForRegister)
        {
             Direccion direccion ;
   
             using(var transaction = _context.Database.BeginTransaction())
             {
                  try
                  {
                        direccion = new Direccion();
                        direccion.Activo = true;
                        direccion.idcliente = addressForRegister.ClienteId;
                        direccion.codigo = addressForRegister.Codigo;
                        direccion.direccion = addressForRegister.Direccion;
                        direccion.iddistrito = addressForRegister.DistritoId;
                        
                        await  _context.Direccion.AddAsync(direccion);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        
                        return direccion.iddireccion;
                 }
                  catch (System.Exception)
                  {
                        transaction.Rollback(); 
                        throw;
                       
                  }
                 
             }
        }
    }
}