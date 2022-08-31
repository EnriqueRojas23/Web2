using System;
using System.Threading.Tasks;
using CargaClic.Repository.Contracts.Mantenimiento;

namespace CargaClic.Repository.Interface.Mantenimiento
{
    public interface IClienteRepository
    {
        Task<int> ClientRegister(ClienteForRegister clienteForRegister);
        Task<int> AddressRegister(AddressForRegister ownerClientForAttach);
    } 
}
