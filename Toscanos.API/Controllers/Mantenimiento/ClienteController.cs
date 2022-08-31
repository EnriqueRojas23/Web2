using System.Threading.Tasks;
using AutoMapper;
using CargaClic.Data;
using CargaClic.Domain.Mantenimiento;
using CargaClic.ReadRepository.Interface.Mantenimiento;
using CargaClic.Repository.Contracts.Mantenimiento;
using CargaClic.Repository.Interface.Mantenimiento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargaClic.API.Controllers.Mantenimiento
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IRepository<Cliente> _repo;
        private readonly IRepository<Propietario> _repo_propietario;
        private readonly IMantenimientoReadRepository _repository;
        private readonly IClienteRepository _repository_Cliente;
        private readonly IMapper _mapper;

        public ClienteController(IRepository<Cliente> repo,
         IMantenimientoReadRepository repository ,
         IRepository<Propietario> repo_propietario,
         IClienteRepository repository_cliente
         ,IMapper mapper)
        {
            _repo = repo;
            _repository = repository;
            _repository_Cliente = repository_cliente;
            _mapper = mapper;
            _repo_propietario = repo_propietario;
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var usuario = await  _repo.Get(x=> x.id == id);
            return Ok(usuario);
        }
        [HttpGet("GetPropietario")]
        public async Task<IActionResult> GetPropietario(int id)
        {
            var usuario = await  _repo_propietario.Get(x=> x.Id == id);
            return Ok(usuario);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
           var usuarios = await  _repo.GetAll();
            return Ok(usuarios);
        }
      
        [HttpGet("GetAllClientes")]
        public async Task<IActionResult> GetAllClientes(string criterio, int UsuarioId)
        {
            var result = await _repository.GetAllClientes(criterio, UsuarioId);
            return Ok(result);
        }
       
        [HttpPost("ClientRegister")]
        public async Task<IActionResult> ClientRegister(ClienteForRegister model)
        {
            var result = await _repository_Cliente.ClientRegister(model);
            return Ok(result);
        }
        [HttpDelete("ClientDelete")]
        public async Task<IActionResult> ClientDelete(int id)
        {
            var cliente = await _repo.Get(x=>x.id == id);
             _repo.Delete(cliente);
            return Ok();
        }
        [HttpPost("ClientUpdate")]
        public async Task<IActionResult> ClientUpdate(ClienteForUpdate model)
        {
             var client = await _repo.Get(x=>x.id == model.id);
             client.razon_social = model.razon_social;
             client.ruc = model.ruc;
             client.mail_notificacion = model.mail_notificacion;
             await _repo.SaveAll();
             return Ok();
        }
        
        
        [HttpGet("GetAllDirecciones")]
        public async Task<IActionResult> GetAllDirecciones(int Id)
        {
            var resp = await  _repository.GetAllDirecciones(Id);
            return Ok(resp);
        }
        [HttpPost("AddressRegister")]
        public async Task<IActionResult> AddressRegister(AddressForRegister model)
        {
            var result = await _repository_Cliente.AddressRegister(model);
            return Ok(result);
        }
        [HttpGet("GetAllDepartamentos")]
        public async Task<IActionResult> GetAllDepartamentos()
        {
            var resp = await  _repository.GetAllDepartamentos();
            return Ok(resp);
        }
        [HttpGet("GetAllProvincias")]
        public async Task<IActionResult> GetAllProvincias(int DepartamentoId)
        {
            var resp = await  _repository.GetAllProvincias(DepartamentoId);
            return Ok(resp);
        }
        [HttpGet("GetAllDistritos")]
        public async Task<IActionResult> GetAllDistritos(int ProvinciaId)
        {
            var resp = await  _repository.GetAllDistritos(ProvinciaId);
            return Ok(resp);
        }

        
        [HttpGet("GetAllTarifas")]
        public async Task<IActionResult> GetAllTarifas(int ClienteId)
        {
            var resp = await  _repository.GetAllTarifas(ClienteId);
            return Ok(resp);
        }
    }
}