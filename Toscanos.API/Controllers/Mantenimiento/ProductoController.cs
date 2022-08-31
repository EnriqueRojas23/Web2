using System;
using System.Threading.Tasks;
using CargaClic.Contracts.Parameters.Mantenimiento;
using CargaClic.Contracts.Parameters.Prerecibo;
using CargaClic.Contracts.Results.Mantenimiento;
using CargaClic.Data;
using CargaClic.Data.Interface;
using CargaClic.Domain.Mantenimiento;
using CargaClic.ReadRepository.Interface.Mantenimiento;
using CargaClic.Repository.Contracts.Mantenimiento;
using CargaClic.Repository.Interface.Mantenimiento;
using Common.QueryHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargaClic.API.Controllers.Mantenimiento
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IQueryHandler<ListarProductosParameter> _handler;
        private readonly IRepository<Huella> _repositoryHuella;
        private readonly IMantenimientoReadRepository _repoMantenimiento;
        private readonly IProductoRepository _repoProducto;
        

        public ProductoController(IQueryHandler<ListarProductosParameter> handler,
        IMantenimientoReadRepository repoMantenimiento,
        IProductoRepository repoProducto,
        IRepository<Huella> repositoryHuella)
        {
            
            _handler = handler;
            _repositoryHuella = repositoryHuella;
            _repoMantenimiento = repoMantenimiento;
            _repoProducto = repoProducto;
        }
       
        [HttpGet]
        public IActionResult GetAll(string criterio, int ClienteId)
        {
            if(criterio == "undefined")
            criterio = null;
            var param = new ListarProductosParameter 
            {
                Criterio = criterio,
                ClienteId = ClienteId
            };
           var result = (ListarProductosResult) _handler.Execute(param);
           return Ok(result.Hits);
        }       
       
        [HttpPost("ProductRegister")]
        public async Task<IActionResult> ProductRegister(ProductoForRegister productoForRegister)
        {
            var result = await _repoProducto.ProductRegister(productoForRegister) ;
            return Ok(result);
        }
        [HttpPost("HuellaDetalleRegister")]
        public async Task<IActionResult> HuellaDetalleRegister(HuellaDetalleForRegister huellaDetalleForRegister)
        {
            var result = await _repoProducto.HuellaDetalleRegister(huellaDetalleForRegister) ;
            return Ok(result);
        }
        [HttpPost("HuellaRegister")]
        public async Task<IActionResult> HuellaRegister(HuellaForRegister huellaForRegister)
        {
            var result = await _repoProducto.HuellaRegister(huellaForRegister) ;
            return Ok(result);
        }
        [HttpDelete("HuellaDetalleDelete")]
        public IActionResult HuellaDetalleDelete(int id)
        {
            var result =  _repoProducto.HuellaDetalleDelete(id) ;
            return Ok(result);
        }
    }
}