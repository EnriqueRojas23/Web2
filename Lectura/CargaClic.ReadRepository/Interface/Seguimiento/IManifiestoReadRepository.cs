using System.Collections.Generic;
using System.Threading.Tasks;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;

namespace CargaClic.ReadRepository.Interface.Seguimiento
{
    public interface IManifiestoReadRepository
    {
         Task<IEnumerable<GetManifiestoResult>> GetAllManifiestos(int ChoferId);
         Task<IEnumerable<GetManifiestoResult>> GetAllManifiestoCliente(int ClienteId);
         Task<IEnumerable<GetManifiestoResult>> GetAllManifiestos(string ids, int idusuario , string inicio, string fin);

         

    }
}