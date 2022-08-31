using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CargaClic.Common;
using CargaClic.Data;
using CargaClic.Domain.Mantenimiento;
using CargaClic.ReadRepository.Contracts.Mantenimiento.Results;


namespace CargaClic.ReadRepository.Interface.Mantenimiento
{
    public interface IMantenimientoReadRepository
    {

         Task<IEnumerable<GetAllClientesResult>> GetAllClientes(String Criterio, int UsuarioId);
         Task<IEnumerable<GetAllDireccionesResult>> GetAllDirecciones(int ClienteId);

         Task<IEnumerable<GetAllDepartamentos>> GetAllDepartamentos();
         Task<IEnumerable<GetAllProvincias>> GetAllProvincias(int DepartamentoId);
         Task<IEnumerable<GetAllDistritos>> GetAllDistritos(int ProvinciaId);

         Task<IEnumerable<GetAllTarifas[]>> GetAllTarifas(int ClienteId);
         Task<IEnumerable<Proveedor>> GetAllProvedores();





    }
}