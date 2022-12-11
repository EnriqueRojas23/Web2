using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CargaClic.API.Dtos.Matenimiento;
using CargaClic.Data;
using CargaClic.Domain.Seguimiento;
using CargaClic.ReadRepository.Interface.Seguimiento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Toscanos.API.Controllers.Seguimiento
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ManifiestoController : ControllerBase
    {
        private const bool V = false;
        private readonly IConfiguration _config;
        private IRepository<Manifiesto> _repoManifiesto;
        private IRepository<CentroCosto> _repoCentroCosto;  
        private IRepository<OrdenTransporte> _repoOrdenTransporte;
        private IManifiestoReadRepository _repo;

        public ManifiestoController(IConfiguration config
        , IRepository<Manifiesto> repoManifiesto
        , IRepository<OrdenTransporte> repoOrdenTransporte
        , IManifiestoReadRepository repo,   
          IRepository<CentroCosto> repoCentroCosto)
        {
            _repoManifiesto = repoManifiesto;
            _config = config;
            _repoOrdenTransporte = repoOrdenTransporte;
            _repo = repo;
            _repoCentroCosto = repoCentroCosto;
        }


        [HttpGet("GetAllManifiestos")]
        public async Task<IActionResult> GetAllManifiestos(string ids, int idusuario, string inicio, string fin
        , int? idtiposervicio = 0)
        {
            if(ids != null)
              ids = ids.Substring(1, ids.Length -1);
            
            var result = await _repo.GetAllManifiestos(ids, idusuario, inicio, fin, idtiposervicio);
            return Ok(result);
        }
        [HttpGet("GetManifiesto")]
        public async Task<IActionResult> GetManifiesto(long id)
        {
               var result = await _repoManifiesto.Get(x=>x.id == id);
               return Ok(result);
        }

        [HttpGet("GetCentroCosto")]
        public async Task<IActionResult> GetCentroCosto(long id)
        {
              CentroCostoDto centroCostoDto = new CentroCostoDto();
               

               var centroCosto = await _repoCentroCosto.Get(x=>x.manifiesto_id == id);
               var manifiesto = await _repoManifiesto.Get(x=>x.id == id);
    
                if(centroCosto == (null))
                {
                    var nuevo = new CentroCosto();
                    nuevo.manifiesto_id = id;
                    nuevo.estiba_facturado = false;
                    nuevo.estiba_fecha = null;
                    nuevo.estiba_numerodoc = "";

                    await _repoCentroCosto.AddAsync(nuevo);

                    await _repoCentroCosto.SaveAll(); 

                    nuevo.estiba_fecha =  DateTime.Now;

                    return Ok(nuevo);
                   
                } 
                else 
                {

                        centroCostoDto.manifiesto_id = manifiesto.id;

                  
                        centroCostoDto.estiba_fecha = centroCosto.estiba_facturado==true? centroCosto.estiba_fecha: DateTime.Now;
                        centroCostoDto.estibaadicional_fecha =centroCosto.estibaadicional_facturado==true? centroCosto.estibaadicional_fecha: DateTime.Now;
                        centroCostoDto.bejarano_pucallpa_fecha = centroCosto.bejarano_pucallpa_facturado==true? centroCosto.bejarano_pucallpa_fecha: DateTime.Now;
                        centroCostoDto.bejarano_iquitos_fecha = centroCosto.bejarano_iquitos_facturado==true? centroCosto.bejarano_iquitos_fecha: DateTime.Now;
                        centroCostoDto.oriental_fecha = centroCosto.oriental_facturado==true? centroCosto.oriental_fecha: DateTime.Now;
                        centroCostoDto.fluvial_fecha = centroCosto.fluvial_facturado==true? centroCosto.fluvial_fecha: DateTime.Now;

                        centroCostoDto.costotercero_fecha = centroCosto.costotercero_facturado==true? centroCosto.costotercero_fecha: DateTime.Now;
                        centroCostoDto.otrosgastos_fecha = centroCosto.otrosgastos_facturado==true? centroCosto.otrosgastos_fecha: DateTime.Now;


                  
                        centroCostoDto.estiba = manifiesto.estiba;
                        centroCostoDto.estiba_adicional = manifiesto.estiba_adicional;
                        centroCostoDto.bejaranopucallpa  = manifiesto.bejaranopucallpa;
                        centroCostoDto.bejaranoiquitos =  manifiesto.bejaranoiquitos;
                        centroCostoDto.oriental = manifiesto.oriental;
                        centroCostoDto.fluvial = manifiesto.fluvial;
                        centroCostoDto.costotercero = manifiesto.costotercero;
                        centroCostoDto.otrosgastos = manifiesto.otrosgastos;


                        centroCostoDto.estiba_facturado = centroCosto.estiba_facturado;
                        centroCostoDto.estibaadicional_facturado = centroCosto.estibaadicional_facturado;
                        centroCostoDto.bejarano_pucallpa_facturado = centroCosto.bejarano_pucallpa_facturado;
                        centroCostoDto.bejarano_iquitos_facturado = centroCosto.bejarano_iquitos_facturado;
                        centroCostoDto.oriental_facturado = centroCosto.oriental_facturado;
                        centroCostoDto.fluvial_facturado = centroCosto.fluvial_facturado;
                        centroCostoDto.costotercero_facturado = centroCosto.costotercero_facturado;
                        centroCostoDto.otrosgastos_facturado = centroCosto.otrosgastos_facturado;
                        
                        

                        centroCostoDto.estiba_numerodoc = centroCosto.estiba_numerodoc;
                        centroCostoDto.estibaadicional_numerodoc = centroCosto.estibaadicional_numerodoc;
                        centroCostoDto.bejarano_pucallpa_numerodoc = centroCosto.bejarano_pucallpa_numerodoc;
                        centroCostoDto.bejarano_iquitos_numerodoc = centroCosto.bejarano_iquitos_numerodoc;
                        centroCostoDto.oriental_numerodoc = centroCosto.oriental_numerodoc;
                        centroCostoDto.fluvial_numerodoc = centroCosto.fluvial_numerodoc;
                        centroCostoDto.costotercero_numerodoc = centroCosto.costotercero_numerodoc;
                        centroCostoDto.otrosgastos_numerodoc = centroCosto.otrosgastos_numerodoc;


                        
                  
                    return Ok(centroCostoDto);

                }
               
        }


        [HttpPost("UpdateManifiesto")]
        public async Task<IActionResult> UpdateManifiesto(ManifiestoForUpdate manifiestoForUpdate)
        {

             var   ordenes = await _repoOrdenTransporte.GetAll(x=> x.manifiesto_id ==  manifiestoForUpdate.id
              && x.activo == true  );
                
                // Actualiza a cero los valorizados de todas las ordenes.

                foreach (var item in ordenes)
                {
                    item.valorizado = 0;
                }
                await _repoOrdenTransporte.SaveAll();

                
                // Actualiza la primera OT al valor total del manifiesto

                ordenes.ToList()[0].valorizado = manifiestoForUpdate.valorizado;

            
             var _manifiesto = await  _repoManifiesto.Get(x => x.id == manifiestoForUpdate.id);
            
            _manifiesto.valorizado = manifiestoForUpdate.valorizado;
            _manifiesto.valorizadoFluvial = manifiestoForUpdate.valorizadoFluvial;
            _manifiesto.bejaranoiquitos = manifiestoForUpdate.bejaranoiquitos;
            _manifiesto.bejaranopucallpa = manifiestoForUpdate.bejaranopucallpa;
            _manifiesto.costotercero = manifiestoForUpdate.costotercero;
            _manifiesto.deestiba  = manifiestoForUpdate.deestiba;
            _manifiesto.estiba = manifiestoForUpdate.estiba;
            _manifiesto.estiba_adicional = manifiestoForUpdate.estiba_adicional;
            _manifiesto.fluvial = manifiestoForUpdate.fluvial;
            _manifiesto.oriental = manifiestoForUpdate.oriental;
            _manifiesto.otrosgastos = manifiestoForUpdate.otrosgastos;
            _manifiesto.retorno_tarifa = manifiestoForUpdate.retorno_tarifa;
            _manifiesto.sobreestadia_tarifa = manifiestoForUpdate.sobreestadia_tarifa;
            _manifiesto.adicionales_tarifa = manifiestoForUpdate.adicionales_tarifa;
          
            var result = await _repoManifiesto.SaveAll();


            return Ok(result);
        }

         [HttpPost("updateInvoiceManifiesto")]
        public async Task<IActionResult> updateInvoiceManifiesto(ManifiestoForUpdate manifiestoForUpdate)
        {
            var cultureInfo = new CultureInfo("es-PE");
            var manifiesto = await _repoManifiesto.Get(x => x.id == manifiestoForUpdate.id);

            manifiesto.facturado = manifiestoForUpdate.facturado;
            if(manifiestoForUpdate.fecha_facturado == null) { 
                manifiesto.fecha_facturado = null;
                manifiesto.numServicio = null;
              } 
            else {
                manifiesto.fecha_facturado = DateTime.ParseExact(manifiestoForUpdate.fecha_facturado,"dd/MM/yyyy", null) ;
                manifiesto.numServicio = manifiestoForUpdate.numServicio;

            }
       

            manifiesto.adicional_facturado = manifiestoForUpdate.adicional_facturado;
             if(manifiestoForUpdate.fecha_adicional_facturado == null) { 
                manifiesto.fecha_adicional_facturado = null;
                manifiesto.numAdicional = null;
              }
            else {
                manifiesto.fecha_adicional_facturado =DateTime.ParseExact(manifiestoForUpdate.fecha_adicional_facturado.ToString(),"dd/MM/yyyy", cultureInfo) ;
                manifiesto.numAdicional = manifiestoForUpdate.numAdicional;
            }

            manifiesto.retorno_facturado = manifiestoForUpdate.retorno_facturado;
            if(manifiestoForUpdate.fecha_retorno_facturado == null) { 
                manifiesto.fecha_retorno_facturado = null;
                manifiesto.numRetorno = null;
              }
            else {
                
            manifiesto.fecha_retorno_facturado = DateTime.ParseExact(manifiestoForUpdate.fecha_retorno_facturado,"dd/MM/yyyy",  cultureInfo) ;
            manifiesto.numRetorno = manifiestoForUpdate.numRetorno;

            }

            manifiesto.sobreestadia_facturado = manifiestoForUpdate.sobreestadia_facturado;
             if(manifiestoForUpdate.fecha_sobreestadia_facturado == null) { 
                manifiesto.fecha_sobreestadia_facturado = null;
                 manifiesto.numSobreestadia = null;
              }
            else {
               manifiesto.fecha_sobreestadia_facturado =DateTime.ParseExact(manifiestoForUpdate.fecha_sobreestadia_facturado,"dd/MM/yyyy", null)  ;
               manifiesto.numSobreestadia = manifiestoForUpdate.numSobreestadia;
            }


            await _repoManifiesto.SaveAll();
            return Ok();
        }


        [HttpPost("UpdateCentroCosto")]
        public async Task<IActionResult> UpdateCentroCosto(CentroCostoForUpdate manifiestoForUpdate)
        {
             var cultureInfo = new CultureInfo("es-PE");
            
             var centroCosto = await  _repoCentroCosto.Get(x => x.manifiesto_id == manifiestoForUpdate.manifiesto_id);
             var manifiesto = await  _repoManifiesto.Get(x => x.id == manifiestoForUpdate.manifiesto_id);

            if(manifiestoForUpdate.estiba_fecha == null) { 
                
                centroCosto.estiba_facturado = false;
                centroCosto.estiba_numerodoc = null;
                manifiesto.estiba = null;
                centroCosto.estiba_fecha = null ;

              } 
            else {

                centroCosto.estiba_fecha = DateTime.ParseExact(manifiestoForUpdate.estiba_fecha,"dd/MM/yyyy", cultureInfo) ;
                centroCosto.estiba_numerodoc = manifiestoForUpdate.estiba_numerodoc;
                manifiesto.estiba = manifiestoForUpdate.estiba;
                centroCosto.estiba_facturado = true;
                
            }

            //   Estiba adicional 

             if(manifiestoForUpdate.estibaadicional_fecha == null) { 
                
                centroCosto.estibaadicional_facturado = false;
                centroCosto.estibaadicional_numerodoc = null;
                centroCosto.estibaadicional_fecha = null ;
                manifiesto.estiba_adicional = null;

              } 
            else {

                centroCosto.estibaadicional_fecha = DateTime.ParseExact(manifiestoForUpdate.estibaadicional_fecha,"dd/MM/yyyy", cultureInfo) ;
                centroCosto.estibaadicional_numerodoc = manifiestoForUpdate.estibaadicional_numerodoc;
                manifiesto.estiba_adicional = manifiestoForUpdate.estiba_adicional;
                centroCosto.estibaadicional_facturado = true;
                
            }



              //  Bejarano Pucallpa

             if(manifiestoForUpdate.bejarano_pucallpa_fecha == null) { 
                
                centroCosto.bejarano_pucallpa_facturado= false;
                centroCosto.bejarano_pucallpa_numerodoc = null;
                centroCosto.bejarano_pucallpa_fecha = null ;
                manifiesto.bejaranopucallpa = null;

              } 
            else {

                centroCosto.bejarano_pucallpa_fecha = DateTime.ParseExact(manifiestoForUpdate.bejarano_pucallpa_fecha,"dd/MM/yyyy", cultureInfo) ;
                centroCosto.bejarano_pucallpa_numerodoc = manifiestoForUpdate.bejarano_pucallpa_numerodoc;
                manifiesto.bejaranopucallpa = manifiestoForUpdate.bejaranopucallpa;
                centroCosto.bejarano_pucallpa_facturado = true;
                
            }


            
              //  Bejarano Iquitos

             if(manifiestoForUpdate.bejarano_iquitos_fecha == null) { 
                
                centroCosto.bejarano_iquitos_facturado= false;
                centroCosto.bejarano_iquitos_numerodoc = null;
                centroCosto.bejarano_iquitos_fecha = null ;
                manifiesto.bejaranoiquitos = null;

              } 
            else {

                centroCosto.bejarano_iquitos_fecha = DateTime.ParseExact(manifiestoForUpdate.bejarano_iquitos_fecha,"dd/MM/yyyy", cultureInfo) ;
                centroCosto.bejarano_iquitos_numerodoc = manifiestoForUpdate.bejarano_iquitos_numerodoc;
                manifiesto.bejaranoiquitos = manifiestoForUpdate.bejaranoiquitos;
                centroCosto.bejarano_iquitos_facturado = true;
                
            }


                //  Fluvial

             if(manifiestoForUpdate.fluvial_fecha == null) { 
                
                centroCosto.fluvial_facturado= false;
                centroCosto.fluvial_numerodoc = null;
                centroCosto.fluvial_fecha = null ;
                manifiesto.fluvial = null;

              } 
            else {

                centroCosto.fluvial_fecha = DateTime.ParseExact(manifiestoForUpdate.fluvial_fecha,"dd/MM/yyyy", cultureInfo) ;
                centroCosto.fluvial_numerodoc = manifiestoForUpdate.fluvial_numerodoc;
                manifiesto.fluvial = manifiestoForUpdate.fluvial;
                centroCosto.fluvial_facturado = true;
                
            }



                //  Oriental
             if(manifiestoForUpdate.oriental_fecha == null) { 
                
                centroCosto.oriental_facturado= false;
                centroCosto.oriental_numerodoc = null;
                centroCosto.oriental_fecha = null ;
                manifiesto.oriental = null;

              } 
            else {

                centroCosto.oriental_fecha = DateTime.ParseExact(manifiestoForUpdate.oriental_fecha,"dd/MM/yyyy", cultureInfo) ;
                centroCosto.oriental_numerodoc = manifiestoForUpdate.oriental_numerodoc;
                manifiesto.oriental = manifiestoForUpdate.oriental;
                centroCosto.oriental_facturado = true;
            }

            
             //  Costo tercero
             if(manifiestoForUpdate.costotercero == null || manifiestoForUpdate.costotercero == 0) { 
                centroCosto.costotercero_facturado= false;
                centroCosto.costotercero_numerodoc = null;
                centroCosto.costotercero_fecha = null ;
                manifiesto.costotercero = null;
              } 
            else {
                centroCosto.costotercero_fecha = DateTime.ParseExact(manifiestoForUpdate.costotercero_fecha,"dd/MM/yyyy", cultureInfo) ;
                centroCosto.costotercero_numerodoc = manifiestoForUpdate.costotercero_numerodoc;
                manifiesto.costotercero = manifiestoForUpdate.costotercero;
                centroCosto.costotercero_facturado = true;
              }


             //  Otros gastos
             if(manifiestoForUpdate.otrosgastos  == null  || manifiestoForUpdate.otrosgastos  == 0) { 
                centroCosto.otrosgastos_facturado= false;
                centroCosto.otrosgastos_numerodoc = null;
                centroCosto.otrosgastos_fecha = null ;
                manifiesto.otrosgastos = null;
              } 
            else {
                centroCosto.otrosgastos_fecha = DateTime.ParseExact(manifiestoForUpdate.otrosgastos_fecha,"dd/MM/yyyy", cultureInfo) ;
                centroCosto.otrosgastos_numerodoc = manifiestoForUpdate.otrosgastos_numerodoc;
                manifiesto.otrosgastos = manifiestoForUpdate.otrosgastos;
                centroCosto.otrosgastos_facturado = true;
            }


          
            var result = await _repoCentroCosto.SaveAll();
            return Ok(result);
        }



    }
}