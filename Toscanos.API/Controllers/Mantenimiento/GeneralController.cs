using System.Threading.Tasks;
using AutoMapper;
using CargaClic.API.Dtos.Matenimiento;
using CargaClic.Contracts.Parameters.Mantenimiento;
using CargaClic.Contracts.Results.Mantenimiento;
using CargaClic.Domain.Mantenimiento;
using Common.QueryHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CargaClic.API.Dtos.Recepcion;
using CargaClic.Common;
using CargaClic.Repository.Interface.Seguimiento;
using System;
using CargaClic.ReadRepository.Interface.Mantenimiento;
using Toscanos.API.Data;
using System.Linq;
using System.Collections.Generic;
using CargaClic.Data;

namespace CargaClic.API.Controllers.Mantenimiento
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        
        private readonly IMantenimientoReadRepository _repoMantenimientoRead;
        private readonly IRepository<Estado> _repo;
        private readonly IRepository<Vehiculo> _repoVehiculo;
        private readonly IRepository<Chofer> _repoChofer;
        private readonly IRepository<Area> _repoArea;

        private readonly IRepository<Proveedor> _repoProveedor;

        private readonly IRepository<ValorTabla> _repoValorTabla;
        private readonly IQueryHandler<ListarPlacasParameter> _handlerVehiculo;
        private readonly IQueryHandler<ListarProveedorParameter> _handlerProveedor;
        private readonly IQueryHandler<ObtenerEquipoTransporteParameter> _handlerEqTransporte;
        private readonly IOrdenRepository _repoOrdenRecibo;
        private readonly Seguimiento _seguimiento;
        private readonly IMapper _mapper;

        public GeneralController(
         IMantenimientoReadRepository repoMantenimientoRead
        ,IRepository<Estado> repo
        ,IRepository<Vehiculo> repoVehiculo
        ,IRepository<Chofer> repoChofer
        ,IRepository<Area> repoArea
        ,IRepository<Proveedor> repoProveedor
        ,IRepository<ValorTabla> repoValorTabla
        ,IQueryHandler<ListarPlacasParameter> handlerVehiculo
        ,IQueryHandler<ListarProveedorParameter> handlerProveedor
        ,IQueryHandler<ObtenerEquipoTransporteParameter> handlerEqTransporte
        ,IOrdenRepository repoOrdenRecibo
        ,Seguimiento seguimiento
        
        ,IMapper mapper
        )
        {
            
            _repoMantenimientoRead = repoMantenimientoRead;
            _repo = repo;
            _repoVehiculo = repoVehiculo;
            _repoChofer = repoChofer;
            _repoArea = repoArea;
            _repoProveedor = repoProveedor;
            _repoValorTabla = repoValorTabla;
            _handlerVehiculo = handlerVehiculo;
            _handlerProveedor = handlerProveedor;
            _handlerEqTransporte = handlerEqTransporte;
            _repoOrdenRecibo = repoOrdenRecibo;
            _seguimiento = seguimiento;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int TablaId)
        {
           var result = await _repo.GetAll(x=>x.TablaId == TablaId);
           
           return Ok(result);
        }
        [HttpGet("GetAllValorTabla")]
        public async Task<IActionResult> GetAllValorTabla(int TablaId)
        {
           var result = await _repoValorTabla.GetAll(x=>x.TablaId == TablaId);
           
           return Ok(result);
        }

#region _repoVehiculo
 [HttpGet("GetVehiculos")]
        public IActionResult GetVehiculos(string placa, int? idproveedor)
        {
            if(placa=="undefined") placa = null;
            var param = new ListarPlacasParameter
            {
                Criterio = placa ,
                idproveedor = idproveedor
            };
            var result = (ListarPlacasResult)  _handlerVehiculo.Execute(param);
            
            return Ok(result.Hits);
        }
        [HttpGet("GetVehiculo")]
        public async Task<IActionResult> GetVehiculo(int id)
        {
           var placa = await _repoVehiculo.Get(x=>x.Id == id);
           return Ok(placa);
        }
        [HttpPost("RegisterVehiculo")]
        public async Task<IActionResult> RegisterVehiculo(VehiculoForRegisterDto vehiculoForRegisterDto)
        {
            vehiculoForRegisterDto.Placa = vehiculoForRegisterDto.Placa.ToUpper();

            
            try
            {
                var param = _mapper.Map<VehiculoForRegisterDto, Vehiculo>(vehiculoForRegisterDto);    
                var createdVehiculo = await _repoVehiculo.AddAsync(param);
                return Ok(createdVehiculo);
            }
            catch (System.Exception ex)
            {
                //var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                var sqlException = ( Microsoft.Data.SqlClient.SqlException) ex.InnerException ;
                        if (sqlException.Number == 2601 || sqlException.Number == 2627)
                            throw new ArgumentException("La placa ya existe");
                        else
                            throw new ArgumentException("Error al insertar");
                
            }
            
            
        }
        [HttpPost("UpdateVehiculo")]
        public async Task<IActionResult> UpdateVehiculo(VehiculoForRegisterDto vehiculoForRegisterDto)
        {
            vehiculoForRegisterDto.Placa = vehiculoForRegisterDto.Placa.ToUpper();
            var vehiculo = await _repoVehiculo.Get(x=>x.Id == vehiculoForRegisterDto.Id);
            vehiculo.MarcaId = vehiculoForRegisterDto.MarcaId;
            vehiculo.Placa = vehiculoForRegisterDto.Placa;
            vehiculo.TipoId = vehiculoForRegisterDto.TipoId;
            vehiculo.ProveedorId = vehiculoForRegisterDto.ProveedorId;
            vehiculo.Volumen = vehiculoForRegisterDto.Volumen;
            vehiculo.PesoBruto = vehiculoForRegisterDto.PesoBruto;
            vehiculo.Confveh = vehiculoForRegisterDto.Confveh;
        

            
            var createdVehiculo = await _repoVehiculo.SaveAll();
            return Ok(createdVehiculo);
        }

#endregion


#region _repoProveedor

        [HttpGet("GetProveedores")]
        public async Task<IActionResult> GetProveedores(string criterio)
        {
            var proveedores = await _repoMantenimientoRead.GetAllProvedores();
            return Ok(proveedores);
         
        }
         [HttpGet("GetProveedor")]
        public async Task<IActionResult> GetProveedor(int id)
        {
            var proveedor = await _repoProveedor.Get(x=>x.Id == id);
            return Ok(proveedor);
         
        }

        [HttpPost("RegisterProveedor")]
        public async Task<IActionResult> RegisterProveedor(ProveedorForRegisterDto proveedor)
        {

             proveedor.RazonSocial = proveedor.RazonSocial.ToUpper();

             var param = _mapper.Map<ProveedorForRegisterDto, Proveedor>(proveedor);
            var createdProveedor = await _repoProveedor.AddAsync(param);
            return Ok(createdProveedor);
        }
        [HttpPost("EditProveedor")]
        public async Task<IActionResult> EditProveedor(ProveedorForRegisterDto proveedor)
        {
             var dbproveedor = _repoProveedor.Get(x=>x.Id == proveedor.Id).Result;
             dbproveedor.RazonSocial = proveedor.RazonSocial.ToUpper();
             dbproveedor.Ruc = proveedor.Ruc;
             await _repoProveedor.SaveAll();
            return Ok(dbproveedor);
        }

#endregion

#region _repoChofer

        [HttpGet("GetChofer")]
        public async Task<IActionResult> GetChofer(string criterio)
        {
            var result = await _repoChofer.GetAll(x=>x.Dni.Contains(criterio)
            || x.NombreCompleto.Contains(criterio) );
            
       
            return Ok(result);
        }
        [HttpGet("GetChoferxId")]
        public async Task<IActionResult> GetChofer(int id)
        {
            var result = await _repoChofer.Get(x=>x.Id == id);
            return Ok(result);
        }
        [HttpPost("RegisterChofer")]
        public async Task<IActionResult> RegisterChofer(Chofer chofer)
        {

            try
            {
                var createdChofer = await _repoChofer.AddAsync(chofer);
                return Ok(createdChofer);    
            }
            catch (System.Exception)
            {
                throw new ArgumentException("El conductor ya existe");
            }
            
        }
        [HttpPost("UpdateChofer")]
        public async Task<IActionResult> UpdateChofer(Chofer chofer)
        {
            var choferDb = await _repoChofer.Get(x=>x.Id == chofer.Id);
            choferDb.Brevete =  chofer.Brevete;
            choferDb.Dni = chofer.Dni;
            choferDb.NombreCompleto = chofer.NombreCompleto;
            choferDb.Telefono = chofer.Telefono;

            try
            {
                await _repoChofer.SaveAll();
                return Ok();    
            }
            catch (System.Exception)
            {
                throw new ArgumentException("El conductor ya existe");
            }
        }

        [HttpGet("GetChoferes")]
        public async Task<IActionResult> GetChoferes()
        {
            var result = await _repoChofer.GetAll();
            return Ok(result);
        }

#endregion          
#region _repoUbicion/Area
        [HttpGet("GetAreas")]
        public async Task<IActionResult> GetAreas()
        {
            var result = await _repoArea.GetAll();
            return Ok(result);
        }
   
  
         [HttpDelete("DeleteProveedor")]
        public async Task<IActionResult> DeleteProveedor(int id)
        { 
            return Ok(await _seguimiento.DeleteProveedor(id));
        }
        
#endregion
   [HttpGet("GetEquipoTransporte")]
        public  IActionResult GetEquipoTransporte(string placa)
        {
             var vehiculo =  _repoVehiculo.Get(x=>x.Placa == placa).Result;

             if(vehiculo == null)
                return Ok();
             

            var param = new ObtenerEquipoTransporteParameter
            {
                VehiculoId = vehiculo.Id 
            };
            var result = (ObtenerEquipoTransporteResult)   _handlerEqTransporte.Execute(param);
            return Ok(result);

        }
    

        [HttpPost("RegisterEquipoTransporte")]
        public async Task<IActionResult> RegisterEquipoTransporte(EquipoTransporteForRegisterDto equipotrans)
        {
              List<long> ids  = new List<long>();
              String[] prm = equipotrans.OrdenTransporteId.Split(',');

              foreach (var item in prm)
              {
                  ids.Add(long.Parse(item));
              }


              var param = new EquipoTransporte();
               
              var vehiculo = await _repoVehiculo.Get(x=>x.Placa ==  equipotrans.Placa);
              if(vehiculo == null)  new ArgumentException("No existe el vehículo");

              var carreta = await _repoVehiculo.Get(x=>x.Placa ==  equipotrans.Carreta);

              var proveedor = await _repoProveedor.Get(x=>x.Ruc == equipotrans.Ruc);
              if(proveedor == null) new ArgumentException("No existe el proveedor");
               
              
              var chofer = await _repoChofer.Get(x=>x.Dni == equipotrans.Dni);
              if(proveedor == null) new ArgumentException("No existe el conductor");


              param.ProveedorId = proveedor.Id;
              param.VehiculoId = vehiculo.Id;
              param.ChoferId = chofer.Id.Value;
              param.EstadoId = (int) Constantes.EstadoEquipoTransporte.EnProceso;
              //param.PropietarioId = equipotrans.PropietarioId;
              if(carreta != null)
                param.CarretaId = carreta.Id;
                try
                {
                    var createdEquipoTransporte = await _repoOrdenRecibo.RegisterEquipoTransporte(param, ids);
                    return Ok(createdEquipoTransporte);
                }
                catch (System.Exception)
                {
                    
                    throw;
                }

             

             
        }

    }
}