using System;
using System.Threading.Tasks;
using AutoMapper;
using CargaClic.Contracts.Parameters.Inventario;
using CargaClic.Data;
using CargaClic.Data.Interface;
using CargaClic.Domain.Inventario;
using CargaClic.ReadRepository.Contracts.Inventario.Parameters;
using CargaClic.ReadRepository.Interface.Inventario;
using CargaClic.Repository.Contracts.Inventario;
using CargaClic.Repository.Interface;
using Common.QueryHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargaClic.API.Controllers.Mantenimiento
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioRepository _repoInventario;
        private readonly IMapper _mapper;
        private readonly IRepository<InventarioGeneral> _repo;
        private readonly IInventarioReadRepository _repoReadInventario;
        private readonly IQueryHandler<ListarInventarioParameter> _handler;

        public InventarioController(IInventarioRepository repoInventario
        ,IMapper mapper 
        ,IRepository<InventarioGeneral> repo
        ,IInventarioReadRepository repoReadInventario
        ,IQueryHandler<ListarInventarioParameter> handler )
        {
            _repoInventario = repoInventario;
            _mapper = mapper;
            _repo = repo;
            _repoReadInventario = repoReadInventario;
            _handler = handler;
        }
       
       
        
        [HttpGet("Get")]
        public async Task<IActionResult> Get(long Id)
        {
            var result = await _repo.Get(x=>x.Id == Id);
            return Ok(result);
        }
  
        [HttpGet("GetAllInvetarioAjuste")]
        public async Task<IActionResult> GetAllInvetarioAjuste(Guid ProductoId , 
         int ClienteId, string FechaInicio, int EstadoId)
        {
            
            var param = new GetAllInventarioParameters {
                ClientId = ClienteId,
                ProductoId = ProductoId,
                EstadoId = EstadoId
            };
            
            var resp = await  _repoReadInventario.GetAllInventario(param);
            return Ok(resp);
        }
        [HttpGet("GetAllInvetarioAjusteDetalle")]
        public async Task<IActionResult> GetAllInvetarioAjusteDetalle(long Id)
        {
            var resp = await  _repoReadInventario.GetAllInventarioDetalle(Id);
            return Ok(resp);
        }

    
    }
}