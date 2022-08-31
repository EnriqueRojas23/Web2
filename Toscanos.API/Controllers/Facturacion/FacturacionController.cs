using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;

using CargaClic.Data.Interface;

using CargaClic.Repository.Interface;
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using CargaClic.Domain.Despacho;
using CargaClic.ReadRepository.Interface.Facturacion;
using CargaClic.Domain.Facturacion;
using CargaClic.Data;

namespace CargaClic.API.Controllers.Facturacion
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FacturacionController : ControllerBase
    {

        private readonly IRepository<OrdenSalida> _repository;
        private readonly IRepository<OrdenSalidaDetalle> _repositoryDetalle;
        private readonly IRepository<Documento> _repository_Documento;
        private readonly IFacturacionReadRepository _repo_Read_Facturacion;
        private readonly IFacturacionRepository _repo_Facturacion;
        private readonly IMapper _mapper;

        public FacturacionController(
         IFacturacionReadRepository repo_read_Facturacion,
         IFacturacionRepository repo_Facturacion,
         IRepository<Documento> repo_Documento,
         IMapper mapper) {
            _repo_Read_Facturacion = repo_read_Facturacion;
            _repo_Facturacion = repo_Facturacion;
            _repository_Documento = repo_Documento;
            _mapper = mapper;
        }
        [HttpGet("GetPendientesLiquidacion")]
        public async Task<IActionResult> GetPendientesLiquidacion(int Id, string fechainicio, string fechafin)
        { 
            var resp  =  await _repo_Read_Facturacion.GetPendientesLiquidacion(Id,
            fechainicio, fechafin);
            return Ok (resp);
        }
        
        [HttpGet("GetPreLiquidaciones")]
        public async Task<IActionResult> GetPreLiquidaciones(int Id)
        { 
            var resp  =  await _repo_Read_Facturacion.GetPreLiquidaciones(Id);
            return Ok (resp);
        }
         [HttpGet("GetPreLiquidacion")]
        public async Task<IActionResult> GetPreLiquidacion(int Id)
        { 
            var resp  =  await _repo_Read_Facturacion.GetPreLiquidacion(Id);
            return Ok (resp);
        }
       

        [HttpGet("GetAllSeries")]
        public async  Task<IActionResult> GetAllSeries()
        { 
            var resp  = await _repository_Documento.GetAll();
            return Ok (resp);
        }
    }
}