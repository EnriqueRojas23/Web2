using System;
using Microsoft.Extensions.Configuration;
using CargaClic.Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;
using Toscanos.API.Data;
using CargaClic.Repository.Interface.Seguimiento;
using System.Threading.Tasks;
using CargaClic.Domain.Seguimiento;
using CargaClic.Repository.Contracts.Seguimiento;
using CargaClic.Domain.Mantenimiento;
using System.Linq;
using CargaClic.ReadRepository.Interface.Mantenimiento;
using CargaClic.Contracts.Parameters.Mantenimiento;
using Common.QueryHandlers;
using CargaClic.API.Dtos.Matenimiento;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using CargaClic.Common;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;
using System.Collections.Generic;
using System.IO.Compression;
using CargaClic.Data;
using System.Globalization;

namespace CargaClic.API.Controllers.Despacho
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenController : ControllerBase
    {

     
        private readonly IConfiguration _config;
        private readonly IOrdenRepository _repo;
        private readonly IMantenimientoReadRepository repoMantenimiento;
        private readonly IRepository<Vehiculo> _repo_Vehiculo;
        private readonly IRepository<Chofer> _repo_Chofer;
        private readonly IRepository<Incidencia> _repo_Incidencia;
        private readonly IRepository<Archivo> _repo_Archivo;
        private readonly IRepository<Manifiesto> _repo_Manifiesto;
        private readonly IRepository<OrdenTransporte> _repo_OrdenTransporte;
        private readonly IRepository<CargaMasivaDetalle> _repo_CargaMasiva;
        private readonly IQueryHandler<ObtenerEquipoTransporteParameter> _handlerEqTransporte;
        private readonly IRepository<Cliente> _repo_cliente;
        private readonly Seguimiento _seguimiento;
        private readonly Reporte _reporte;

        public OrdenController(IConfiguration config, 
          IOrdenRepository repo,
          IMantenimientoReadRepository repoMantenimiento,
          IRepository<Vehiculo> repo_Vehiculo,
          IRepository<Chofer> repo_Chofer,
          IRepository<OrdenTransporte> repo_OrdenTransporte,
          IRepository<CargaMasivaDetalle> repo_CargaMasiva,
          IRepository<Incidencia> repo_Incidencia,
          IRepository<Archivo> repo_Archivo,
          IRepository<Manifiesto> repo_Manifiesto,
          IQueryHandler<ObtenerEquipoTransporteParameter> handlerEqTransporte,
          IRepository<Cliente> repo_cliente,
          Reporte reporte,
          Seguimiento seguimiento) {
        
            _config = config;
            _repo = repo;
            this.repoMantenimiento = repoMantenimiento;
            _handlerEqTransporte = handlerEqTransporte;
            _repo_cliente = repo_cliente;
            _repo_Vehiculo = repo_Vehiculo;
            _repo_Chofer = repo_Chofer;
            _repo_OrdenTransporte = repo_OrdenTransporte;
            _repo_CargaMasiva = repo_CargaMasiva;
            _seguimiento = seguimiento;
            _repo_Incidencia = repo_Incidencia;
            _repo_Archivo = repo_Archivo;
            _repo_Manifiesto = repo_Manifiesto;
            _reporte  = reporte;
        }
        [HttpPost("UploadFile")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(int usrid)
        {
          // var seguimiento = new Seguimiento();
            try
            {
                // Grabar Excel en disco
                string fullPath = await SaveFile(0);
                // Leer datos de excel
                var celdas = _seguimiento.GetExcel(fullPath);
                // Generar entidades
                var entidades = _seguimiento.ObtenerEntidades_CargaMasiva(celdas);
                // Grabar entidades en base de datos
                
                var carga = new CargaMasivaForRegister();
                carga.estado_id = 1;
                carga.fecha_registro = DateTime.Now;
            

            
                var resp =  await _repo.RegisterCargaMasiva(carga, entidades);

                //Generar Ordenes de trabajo y Manifiestos      
                var detalles_cargados =  _repo_CargaMasiva.GetAll(x=>x.carga_id == resp).Result;


                var lista = detalles_cargados.ToList();
                var manifiestos = _seguimiento.ObtenerEntidades_Manifiesto(lista);
                //Registrar manifiestos 
                await _repo.RegisterOrdenes(manifiestos,usrid);

                

            }
            catch (System.Exception ex)
            {
                return Ok(ex.Message);
                throw ex;
              
            }
            return Ok();
         }
        private async Task<string> SaveFile(long usuario_id)
        {
            
            var fullPath = string.Empty;
          
            
            var ruta =  _config.GetSection("AppSettings:Uploads").Value;

            var file = Request.Form.Files[0];
            var idOrden = usuario_id;

            string folderName = idOrden.ToString();
            string webRootPath = ruta ;
            string newPath = Path.Combine(webRootPath, folderName);

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
            {
                fileData = binaryReader.ReadBytes(Request.Form.Files[0].ContentDisposition.Length);
            }

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fullPath = Path.Combine(newPath, fileName);

                var checkextension = Path.GetExtension(fileName).ToLower();
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {

                    file.CopyTo(stream);
                    await _repo_OrdenTransporte.SaveAll();
                    
                }

            }
            return fullPath;

        }
        
     

        [HttpGet("getPendientesPorDia")] 
        public async Task<IActionResult> getPendientesPorDia(string fecha)
        {
            var result = await _seguimiento.getPendientesPorDia(fecha);
            return Ok(result);
        }


        [HttpGet("GetSustentoManifiestoCerrados")] 
        public async Task<IActionResult> GetSustentoManifiestoCerrados(string manifiesto)
        {
            var result = await _seguimiento.GetSustentoManifiestoCerrado(manifiesto);
            return Ok(result);
        }
        [HttpGet("getAllFolders")]
        public async Task<IActionResult> getAllFolders(string carpeta)
        {
               List<DocumentoDto> temp = new List<DocumentoDto>();
               DocumentoDto ar = new DocumentoDto();             
               DirectoryInfo di = new DirectoryInfo("D:\\APPS\\Toscanos\\upload");
               DirectoryInfo[] diArr = di.GetDirectories();

                if(carpeta == "null")
                {
                    foreach (DirectoryInfo dri in diArr)
                    {
                        ar = new DocumentoDto();  
                        ar.nombrearchivo = dri.Name;
                        ar.hasChildren = true;
                        temp.Add(ar );
                    }
                }
                else
                {
                    DirectoryInfo diFinal = new DirectoryInfo("D:\\APPS\\Toscanos\\upload\\" +  carpeta);
                    foreach (var fi in diFinal.GetFiles())
                    {
                        ar = new DocumentoDto();  
                        ar.nombrearchivo = fi.Name;
                        ar.hasChildren = false;
                        temp.Add(ar );
                       
                    }

                }




              // var result = await _seguimiento.GetAllManifiestos();
               return Ok(temp);
        }


        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> GetAllOrder(string remitente_id, int? estado_id, int usuario_id, string fec_ini, string fec_fin, int? tiposervicio_id= null)
        {
             var result  = await _seguimiento.Listar_OrdensTransporte(remitente_id,  estado_id, usuario_id, fec_ini, fec_fin, tiposervicio_id);
             return Ok(result);
        }
        [HttpGet("GetChofer")]
        public async Task<IActionResult> GetChofer(int usrid)
        {
             var result  = await _repo_Chofer.Get(x=>x.UsuarioId == usrid);
             return Ok(result.Id);
        }
        [HttpGet("GetAllOrderByManifiesto")]
        public async Task<IActionResult> GetAllOrderByManifiesto(long manifiestoId)
        {
             var result  = await _seguimiento.Listar_OrdensTransporte(manifiestoId);
             return Ok(result);
        }

         [HttpGet("SearchOrden")]
        public async Task<IActionResult> SearchOrden(string criterio, int dias)
        {
             var result  = await _seguimiento.BuscarOrdensTransporte(criterio,dias);
             return Ok(result);
        }

        [HttpGet("GetAllOrderByManifiestoCliente")]
        public async Task<IActionResult> GetAllOrderByManifiestoCliente(int manifiestoId, int UsuarioId)
        {
             var result  = await _seguimiento.Listar_OrdensTransportCliente(manifiestoId, UsuarioId);
             return Ok(result);
        }
        
        [HttpGet("GetAllIncidencias")]
        public async Task<IActionResult> GetAllIncidencias(long OrdenTransporteId)
        {
             var result  = await _seguimiento.Listar_Incidencias(OrdenTransporteId);
             return Ok(result);
        }

        [HttpGet("GetMaestroIncidencias")]
        public async Task<IActionResult> GetMaestroIncidencias()
        {
             var result  = await _seguimiento.GetMaestroIncidencias();
             return Ok(result);
        }

        [HttpGet("GetDatosIncidencia")]
        public async Task<IActionResult> GetDatosIncidencia(long incidencia)
        {
             var result  = await _seguimiento.GetDatosIncidencia(incidencia);
             return Ok(result);
        }
        
        // [HttpGet("GetAllManifiesto")]
        // public async Task<IActionResult> GetAllManifiesto(int ChoferId)
        // {
        //      var result  = await _seguimiento.Listar_Manifiesto(ChoferId);
        //      return Ok(result);
        // }

        [HttpGet("GetEstadisticasMobile")]
        public async Task<IActionResult> GetEstadisticasMobile(int cliente_id)
        {
             var result  = await _seguimiento.GetEstadisticas(cliente_id);
             return Ok(result);
        }
        

        // [HttpGet("GetAllManifiestoCliente")]
        // public async Task<IActionResult> GetAllManifiestoCliente(int UsuarioId)
        // {
        //      var result  = await _seguimiento.Listar_Manifiesto_Cliente(UsuarioId);
        //      return Ok(result);
        // }
        [HttpDelete("DeleteFile")]
        public async Task<IActionResult> DeleteFile(int documentoId)
        {
             var result  = await _repo_Archivo.Get(x=>x.idarchivo ==documentoId );
             _repo_Archivo.Delete(result);
             await _repo_Archivo.SaveAll();
             return Ok(result);
        }



        [HttpPost("RegisterIncidencia")]
        public async Task<IActionResult> RegisterIncidencia(IncidenciaForRegister incidencia)
        {
            var createdProveedor = await _seguimiento.RegisterIncidencia(incidencia);
            return Ok(createdProveedor);
        }

        
        [HttpPost("IniciarTransitoFluvial")]
        public async Task<IActionResult> IniciarTransitoFluvial(ConfirmarEntregaDto model)
        {
             EquipoTransporte equipotransporte = null;
            var incidencia = new Incidencia();
            var ordentrabajo = await  _repo_OrdenTransporte.Get(x=>x.id == model.idordentrabajo); 
             Chofer chofer = null;

            ordentrabajo.estado_id = (int) Constantes.EstadoOrdenTransporte.EnTransitoFluvial;
            incidencia.maestro_incidencia_id = 11;
            ordentrabajo.reconocimento_embarque = model.reconocimiento_embarque;
            ordentrabajo.numero_lancha = model.numero_lancha;
            incidencia.observacion = "En Tránsito fluvial " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();

            incidencia.activo = true;
            incidencia.descripcion = "";
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.orden_trabajo_id = model.idordentrabajo;


            if(ordentrabajo.equipo_transporte_id.HasValue)
            {
                equipotransporte =   _seguimiento.GetEquipoTransporte(ordentrabajo.equipo_transporte_id.Value);
                chofer = await _repo_Chofer.Get(x=>x.Id == equipotransporte.ChoferId );
            }

                if(chofer != null)
                     incidencia.usuario_id = chofer.UsuarioId;
                else incidencia.usuario_id = 1;


                await _repo_Incidencia.AddAsync(incidencia);
                await _repo_OrdenTransporte.SaveAll();
          
            return Ok("ordentrabajo");
        } 
        
        [HttpPost("CambiarEstadoOrdenGeoLocalizacion")]
        public async Task<IActionResult> CambiarEstadoOrdenGeoLocalizacion(long idOrden, decimal lat, decimal lng, int usr_id)
        {
            var incidencia = new Incidencia();
            EquipoTransporte equipotransporte = null;
            GeoEquipoTransporteForRegister geoEquipoTransporte ;
            string SMTP_SERVER =  _config.GetSection("AppSettings:SMTPSERVER").Value;
            string SMTP_MAIL =  _config.GetSection("AppSettings:MAIL_SMTP").Value;
            string SMTP_PASSWORD =  _config.GetSection("AppSettings:SMTP_PASSWORD").Value;
            string CORREO_PRUEBA =  _config.GetSection("AppSettings:PRUEBA_CORREO").Value;
            Chofer chofer = null;
            

            var ordenTransporte =  await _repo_OrdenTransporte.Get(x=>x.id == idOrden);

             geoEquipoTransporte = new GeoEquipoTransporteForRegister();
             geoEquipoTransporte.usuario_id = usr_id;
             geoEquipoTransporte.lat = lat;
             geoEquipoTransporte.lng  = lng;


             var result =  await  _repo.RegisterGeoLocalizacion(geoEquipoTransporte);
            // return Ok(result);

       
            if(ordenTransporte.equipo_transporte_id.HasValue)
            {
                 equipotransporte =   _seguimiento.GetEquipoTransporte(ordenTransporte.equipo_transporte_id.Value);
                 chofer = await _repo_Chofer.Get(x=>x.Id == equipotransporte.ChoferId);
            }
            
            if(ordenTransporte.estado_id == 11)
            {
                    incidencia.activo = true;
                    incidencia.maestro_incidencia_id = (Int32) Constantes.EstadoOrdenTransporte.EnRuta;
                    incidencia.observacion = "Esta incidencia es de prueba";
                    incidencia.descripcion = "";
                    incidencia.fecha_incidencia = DateTime.Now;
                    incidencia.fecha_registro = DateTime.Now;
                    incidencia.orden_trabajo_id = idOrden;
                    if(chofer != null)
                        incidencia.usuario_id = chofer.UsuarioId;
                    else incidencia.usuario_id = 1;
                    
                    await _repo_Incidencia.AddAsync(incidencia);

                    return Ok(ordenTransporte);
            }

            ordenTransporte.estado_id = ordenTransporte.estado_id + 1  ;
            if(ordenTransporte.estado_id == 7)
            {
                  ordenTransporte.estado_id = (Int32) Constantes.EstadoOrdenTransporte.EnRuta;
            }
            else if(ordenTransporte.estado_id == 8)
            {
                    ordenTransporte.estado_id =(Int32) Constantes.EstadoOrdenTransporte.EnRuta;
                   if(ordenTransporte.recojo == true)
                   {
                      ordenTransporte.estado_id =(Int32) Constantes.EstadoOrdenTransporte.FinRecojo;
                   } 
            }
            else if(ordenTransporte.estado_id == 37)
            {
                ordenTransporte.estado_id =(Int32) Constantes.EstadoOrdenTransporte.EnRuta;

            }
            await _repo_OrdenTransporte.SaveAll();

     

            if(ordenTransporte.estado_id  == 6)
            {
                incidencia.maestro_incidencia_id = 3;
                incidencia.observacion = "Llegada al punto de recojo " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }
            else if(ordenTransporte.estado_id  == 8)
            {
                incidencia.maestro_incidencia_id = 5;
                incidencia.observacion = "Inicio de ruta " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();

                // Envío de mail....
                // var cliente = await _repo_cliente.Get(x=>x.id == ordenTransporte.destinatario_id);
                // if(cliente.mail_notificacion != null || cliente.mail_notificacion != string.Empty)
                //         CORREO_PRUEBA = CORREO_PRUEBA + ";"  + cliente.mail_notificacion;

                // var provincia = _seguimiento.ObtenerProvincia(ordenTransporte.provincia_entrega.Value);

                // var vehiculo = await _repo_Vehiculo.Get(x => x.Id == equipotransporte.VehiculoId);
             
                
                // string htmlString = @"<html>
                //         <body>
                //         <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-8533450555590531398title'><tbody><tr style='height:18.75pt'><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal' align='center' style='text-align:center'><strong><span style='font-size:16.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414;letter-spacing:-.4pt'>Tu pedido está en camino </span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:21.0pt'><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                //         <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-8533450555590531398content'><tbody><tr><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><div align='center'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse' id='m_-8533450555590531398inner-content'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>

                //         <span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //             Estimados: "+ cliente.razon_social + @" 
                //         </span>
                        
                //         <span style='font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr style='height:14.25pt'><td style='padding:0cm 0cm 0cm 0cm;height:14.25pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal' style='text-align:justify'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         <br>La unidad a cargo de la entrega de tu pedido, estará aproximadamente a las "+ ordenTransporte.hora_entrega  + @" en la dirección de despacho que nos indicaste. <u></u><u></u></span></p></td></tr><tr style='height:14.25pt'><td style='padding:0cm 0cm 0cm 0cm;height:14.25pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr></tbody></table></td></tr></tbody></table></td><td width='20' style='width:15.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;border-radius:3px'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'><td style='background:#db0414;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'> <a href='http://104.36.166.65/toscanos/#/seguimiento/verorden/"+ ordenTransporte.id + @"'><span style='font-size:11.5pt;color:white;text-decoration:none'>Rastrear mi pedido</span></a> </span><span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:10.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:10.5pt'></td></tr></tbody></table></td></tr></tbody></table></div></td><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                        
                //         <table border='0' cellspacing='0' cellpadding='0' width='100%' 
                //         style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'>
                //         <td style='background:#db0414;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' 
                //         align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'>
                       
                       
                //          <span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table>
                //         <p>   
                //         </body>
                //         <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-1387954621597574012order-details'><tbody><tr style='height:15.0pt'><td width='25' style='width:18.75pt;border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><div align='center'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse' id='m_-1387954621597574012inner-content'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414'>Datos del envío:</span></strong><span style='font-family:&quot;Helvetica&quot;,sans-serif'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong>
                //         <span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Fecha de Pedido:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>
                //         <tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'>
                //         <span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>

                //         " +  ordenTransporte.fecha_registro.ToShortDateString() + @"
                        
                //         <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Fecha de Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+ ordenTransporte.fecha_entrega.Value.ToShortDateString() +@"
                //         <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Domicilio de Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+ ordenTransporte.direccion_entrega + @"
                        
                //         <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Número de Teléfono:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>
                //         <tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Distrito:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+ provincia + @"
                        
                //         <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr><tr style='height:12.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:12.0pt;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         # Orden de Transporte </span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'><u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+  ordenTransporte.numero_ot  + @" 
                //         <u></u><u></u></span></p></td></tr><tr style='height:30.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:30.0pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Nro. Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'><u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>

                //         <tr style='height:30.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:30.0pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td></tr></tbody></table></td></tr></tbody></table></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414'>Información de Chofer:</span></strong><span style='font-family:&quot;Helvetica&quot;,sans-serif'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-size:1.0pt'><img border='0' height='95' style='height:.9895in' id='m_-1387954621597574012_x0000_i1026' src='https://ci3.googleusercontent.com/proxy/Md5He3hPe9rJeJALU5ArtgdTYUhgRbRT82SpKo3pAwsTu13eV1QAzy4TPKwlF0qXMShaSEpjxx_5YBxs_B1wWxOVIUhyGRMq4U6hSKtL4-dw5Rx6nUxvWJv5YjjtlzNB-Rc-y2JjHA=s0-d-e1-ft#http://toscargo.e-strategit.com/Public/Imagenes/Chofer/fotochofer_06082019050819.png' alt='Foto conductor' class='CToWUd'><u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Nombre:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:8.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+   chofer.NombreCompleto   +@"
                //          <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>DNI:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //          "+ chofer.Dni +@"
                //          <u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Placa:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //          " + vehiculo.Placa +@"
                //           <u></u><u></u></span></p></td></tr></tbody></table></td></tr>
                //         <tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td></tr></tbody></table></div></td><td width='25' style='width:18.75pt;border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:15.0pt'><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                //         </html>";   

                //string body =  "Se ha registrado una entrega (" +  model.descripcion + " ) para la OT " + ordentrabajo.numero_ot;
                // try
                // {
                           
                //         // MailHelper.EnviarMail(true,SMTP_SERVER,SMTP_MAIL,SMTP_PASSWORD,CORREO_PRUEBA,
                //         //     "",   ordenTransporte.numero_ot , htmlString,true );
                // }
                // catch(Exception ex)
                // {

                // }
                
            }
                
            else if(ordenTransporte.estado_id  == 9)
            {
                if(ordenTransporte.tiposervicio_id == 166)
                {
                        ordenTransporte.estado_id = 11;
                }
           
                   incidencia.maestro_incidencia_id = 6;
                   incidencia.observacion = "Llegada a destino " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
               
            }
            
            else if(ordenTransporte.estado_id  == 10)
            {
                incidencia.maestro_incidencia_id = 7;
                   incidencia.observacion = "Descarga de la mercadería " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }

            else if(ordenTransporte.estado_id  == 11)
            {
                incidencia.maestro_incidencia_id = 8;
                   incidencia.observacion = "Término de descarga " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }
            else if (ordenTransporte.estado_id == 36)
            {
                    incidencia.maestro_incidencia_id = 19;
                   incidencia.observacion = "Fin de Recojo " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }
            else if (ordenTransporte.estado_id == 41)
            {
                   
                   incidencia.maestro_incidencia_id = 13;
                   incidencia.observacion = "Fin del desembarque " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }
            // else if(ordenTransporte.estado_id  == 12)
            // {
            //     incidencia.maestro_incidencia_id = 9;
            //        incidencia.observacion = "Término de descarga " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            // }
            //  ordenTransporte =  await _repo_OrdenTransporte.Get(x=>x.id == idOrden);
            
            incidencia.activo = true;
            incidencia.descripcion = "";
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.orden_trabajo_id = idOrden;
            if(chofer != null)
                incidencia.usuario_id = chofer.UsuarioId;
            else incidencia.usuario_id = 1;
            
           

            await _repo_Incidencia.AddAsync(incidencia);

            return Ok(ordenTransporte);



        }
        [HttpGet("GetListarReporteMargen")]
        public async Task<IActionResult> GetListarReporteMargen(int anio, int mes)
        { 
            var resp  = await _seguimiento.GetListarReporteMargen(anio, mes);
            return Ok (resp);
        }



        [HttpPost("CambiarEstadoOrden")]
        public async Task<IActionResult> CambiarEstadoOrden(long idOrden)
        {
            var incidencia = new Incidencia();
            EquipoTransporte equipotransporte = null;
            string SMTP_SERVER =  _config.GetSection("AppSettings:SMTPSERVER").Value;
            string SMTP_MAIL =  _config.GetSection("AppSettings:MAIL_SMTP").Value;
            string SMTP_PASSWORD =  _config.GetSection("AppSettings:SMTP_PASSWORD").Value;
            string CORREO_PRUEBA =  _config.GetSection("AppSettings:PRUEBA_CORREO").Value;
            Chofer chofer = null;
            

            var ordenTransporte =  await _repo_OrdenTransporte.Get(x=>x.id == idOrden);

       
            if(ordenTransporte.equipo_transporte_id.HasValue)
            {
                equipotransporte =   _seguimiento.GetEquipoTransporte(ordenTransporte.equipo_transporte_id.Value);
                 chofer = await _repo_Chofer.Get(x=>x.Id == equipotransporte.ChoferId);
            }
            
            if(ordenTransporte.estado_id == 11)
            {
                    incidencia.activo = true;
                    incidencia.maestro_incidencia_id = (Int32) Constantes.EstadoOrdenTransporte.EnRuta;
                    incidencia.observacion = "Esta incidencia es de prueba";
                    incidencia.descripcion = "";
                    incidencia.fecha_incidencia = DateTime.Now;
                    incidencia.fecha_registro = DateTime.Now;
                    incidencia.orden_trabajo_id = idOrden;
                    if(chofer != null)
                        incidencia.usuario_id = chofer.UsuarioId;
                    else incidencia.usuario_id = 1;
                    
                    await _repo_Incidencia.AddAsync(incidencia);

                    return Ok(ordenTransporte);
            }

            ordenTransporte.estado_id = ordenTransporte.estado_id + 1  ;
            if(ordenTransporte.estado_id == 7)
            {
                  ordenTransporte.estado_id = (Int32) Constantes.EstadoOrdenTransporte.EnRuta;
            }
            else if(ordenTransporte.estado_id == 8)
            {
                    ordenTransporte.estado_id =(Int32) Constantes.EstadoOrdenTransporte.EnRuta;
                   if(ordenTransporte.recojo == true)
                   {
                      ordenTransporte.estado_id =(Int32) Constantes.EstadoOrdenTransporte.FinRecojo;
                   } 
            }
            else if(ordenTransporte.estado_id == 37)
            {
                ordenTransporte.estado_id =(Int32) Constantes.EstadoOrdenTransporte.EnRuta;

            }
            await _repo_OrdenTransporte.SaveAll();

     

            if(ordenTransporte.estado_id  == 6)
            {
                incidencia.maestro_incidencia_id = 3;
                incidencia.observacion = "Llegada al punto de recojo " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }
            else if(ordenTransporte.estado_id  == 8)
            {
                incidencia.maestro_incidencia_id = 5;
                incidencia.observacion = "Inicio de ruta " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();

                // Envío de mail....
                // var cliente = await _repo_cliente.Get(x=>x.id == ordenTransporte.destinatario_id);
                // if(cliente.mail_notificacion != null || cliente.mail_notificacion != string.Empty)
                //         CORREO_PRUEBA = CORREO_PRUEBA + ";"  + cliente.mail_notificacion;

                // var provincia = _seguimiento.ObtenerProvincia(ordenTransporte.provincia_entrega.Value);

                // var vehiculo = await _repo_Vehiculo.Get(x => x.Id == equipotransporte.VehiculoId);
             
                
                // string htmlString = @"<html>
                //         <body>
                //         <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-8533450555590531398title'><tbody><tr style='height:18.75pt'><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal' align='center' style='text-align:center'><strong><span style='font-size:16.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414;letter-spacing:-.4pt'>Tu pedido está en camino </span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:21.0pt'><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                //         <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-8533450555590531398content'><tbody><tr><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><div align='center'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse' id='m_-8533450555590531398inner-content'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>

                //         <span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //             Estimados: "+ cliente.razon_social + @" 
                //         </span>
                        
                //         <span style='font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr style='height:14.25pt'><td style='padding:0cm 0cm 0cm 0cm;height:14.25pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal' style='text-align:justify'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         <br>La unidad a cargo de la entrega de tu pedido, estará aproximadamente a las "+ ordenTransporte.hora_entrega  + @" en la dirección de despacho que nos indicaste. <u></u><u></u></span></p></td></tr><tr style='height:14.25pt'><td style='padding:0cm 0cm 0cm 0cm;height:14.25pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr></tbody></table></td></tr></tbody></table></td><td width='20' style='width:15.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;border-radius:3px'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'><td style='background:#db0414;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'> <a href='http://104.36.166.65/toscanos/#/seguimiento/verorden/"+ ordenTransporte.id + @"'><span style='font-size:11.5pt;color:white;text-decoration:none'>Rastrear mi pedido</span></a> </span><span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:10.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:10.5pt'></td></tr></tbody></table></td></tr></tbody></table></div></td><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                        
                //         <table border='0' cellspacing='0' cellpadding='0' width='100%' 
                //         style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'>
                //         <td style='background:#db0414;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' 
                //         align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'>
                       
                       
                //          <span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table>
                //         <p>   
                //         </body>
                //         <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-1387954621597574012order-details'><tbody><tr style='height:15.0pt'><td width='25' style='width:18.75pt;border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><div align='center'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse' id='m_-1387954621597574012inner-content'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414'>Datos del envío:</span></strong><span style='font-family:&quot;Helvetica&quot;,sans-serif'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong>
                //         <span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Fecha de Pedido:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>
                //         <tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'>
                //         <span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>

                //         " +  ordenTransporte.fecha_registro.ToShortDateString() + @"
                        
                //         <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Fecha de Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+ ordenTransporte.fecha_entrega.Value.ToShortDateString() +@"
                //         <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Domicilio de Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+ ordenTransporte.direccion_entrega + @"
                        
                //         <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Número de Teléfono:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>
                //         <tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Distrito:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+ provincia + @"
                        
                //         <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr><tr style='height:12.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:12.0pt;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         # Orden de Transporte </span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'><u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+  ordenTransporte.numero_ot  + @" 
                //         <u></u><u></u></span></p></td></tr><tr style='height:30.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:30.0pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Nro. Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'><u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>

                //         <tr style='height:30.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:30.0pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td></tr></tbody></table></td></tr></tbody></table></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414'>Información de Chofer:</span></strong><span style='font-family:&quot;Helvetica&quot;,sans-serif'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-size:1.0pt'><img border='0' height='95' style='height:.9895in' id='m_-1387954621597574012_x0000_i1026' src='https://ci3.googleusercontent.com/proxy/Md5He3hPe9rJeJALU5ArtgdTYUhgRbRT82SpKo3pAwsTu13eV1QAzy4TPKwlF0qXMShaSEpjxx_5YBxs_B1wWxOVIUhyGRMq4U6hSKtL4-dw5Rx6nUxvWJv5YjjtlzNB-Rc-y2JjHA=s0-d-e1-ft#http://toscargo.e-strategit.com/Public/Imagenes/Chofer/fotochofer_06082019050819.png' alt='Foto conductor' class='CToWUd'><u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Nombre:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:8.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //         "+   chofer.NombreCompleto   +@"
                //          <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>DNI:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //          "+ chofer.Dni +@"
                //          <u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Placa:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                //          " + vehiculo.Placa +@"
                //           <u></u><u></u></span></p></td></tr></tbody></table></td></tr>
                //         <tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td></tr></tbody></table></div></td><td width='25' style='width:18.75pt;border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:15.0pt'><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                //         </html>";   

                //string body =  "Se ha registrado una entrega (" +  model.descripcion + " ) para la OT " + ordentrabajo.numero_ot;
                // try
                // {
                           
                //         // MailHelper.EnviarMail(true,SMTP_SERVER,SMTP_MAIL,SMTP_PASSWORD,CORREO_PRUEBA,
                //         //     "",   ordenTransporte.numero_ot , htmlString,true );
                // }
                // catch(Exception ex)
                // {

                // }
                
            }
                
            else if(ordenTransporte.estado_id  == 9)
            {
                if(ordenTransporte.tiposervicio_id == 166)
                {
                        ordenTransporte.estado_id = 11;
                }
           
                   incidencia.maestro_incidencia_id = 6;
                   incidencia.observacion = "Llegada a destino " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
               
            }
            
            else if(ordenTransporte.estado_id  == 10)
            {
                incidencia.maestro_incidencia_id = 7;
                   incidencia.observacion = "Descarga de la mercadería " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }

            else if(ordenTransporte.estado_id  == 11)
            {
                incidencia.maestro_incidencia_id = 8;
                   incidencia.observacion = "Término de descarga " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }
            else if (ordenTransporte.estado_id == 36)
            {
                    incidencia.maestro_incidencia_id = 19;
                   incidencia.observacion = "Fin de Recojo " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }
            else if (ordenTransporte.estado_id == 41)
            {
                   
                   incidencia.maestro_incidencia_id = 13;
                   incidencia.observacion = "Fin del desembarque " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            }
            // else if(ordenTransporte.estado_id  == 12)
            // {
            //     incidencia.maestro_incidencia_id = 9;
            //        incidencia.observacion = "Término de descarga " + DateTime.Now.ToShortDateString() + "  -  "+ DateTime.Now.ToShortTimeString();
            // }
            //  ordenTransporte =  await _repo_OrdenTransporte.Get(x=>x.id == idOrden);
            
            incidencia.activo = true;
            incidencia.descripcion = "";
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.orden_trabajo_id = idOrden;
            if(chofer != null)
                incidencia.usuario_id = chofer.UsuarioId;
            else incidencia.usuario_id = 1;
            
           

            await _repo_Incidencia.AddAsync(incidencia);

            return Ok(ordenTransporte);



        }
        [HttpPost("UploadFileConfirm")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFileConfirm()
        {

            var archivo = new Archivo() ;
            string tipo = "1" , factura = "",  folderName = "", webRootPath ="", newPath ="";
         
            var ruta =  _config.GetSection("AppSettings:Uploads").Value;
            var ruta2 =  _config.GetSection("AppSettings:Uploads2").Value;

            var file = Request.Form.Files[0];
            var idOrden = Request.Form["idOrden"];

            tipo = Request.Form["tipo"];
            factura = Request.Form["factura"];

            var objOrden = await _repo_OrdenTransporte.Get(x=>x.id == long.Parse(idOrden.ToString()));
                
            if(objOrden.cantidad_fotos == null)
                objOrden.cantidad_fotos  = 1;
            else
                objOrden.cantidad_fotos = objOrden.cantidad_fotos + 1;


            if(tipo  == "2")
            {
                    folderName = DateTime.Now.ToString("ddMMyyyy");
                    webRootPath  = ruta2 ;
                    newPath  = Path.Combine(webRootPath, folderName);
            }
            else 
            {
                    folderName = idOrden;
                    webRootPath  = ruta ;
                    newPath  = Path.Combine(webRootPath, folderName);

            }


            try
            {

               // obtiene File cargado
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
                {
                    fileData = binaryReader.ReadBytes(Request.Form.Files[0].ContentDisposition.Length);
                }

                //crea carpeta nueva
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                   
                    if(tipo  == "2")
                    {
                        if(objOrden.factura == "")
                        {
                             fileName = factura + ".jpg";
                        }
                        else {
                             fileName = objOrden.factura  + ".jpg";
                        }
                            string fullPath = Path.Combine(newPath, fileName);
                            var checkextension = Path.GetExtension(fileName).ToLower();
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {

                                file.CopyTo(stream);
                                
                                archivo.extension = ".pdf";
                                archivo.nombrearchivo = fileName;
                                archivo.rutaacceso = newPath;
                                archivo.idordentrabajo = long.Parse(idOrden);

                                await this._repo_Archivo.AddAsync(archivo);
                                await this._repo_OrdenTransporte.SaveAll();


                            }
                    }
                    else {
                            string fullPath = Path.Combine(newPath, fileName);
                            var checkextension = Path.GetExtension(fileName).ToLower();
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {

                                file.CopyTo(stream);
                                
                                archivo.extension = checkextension;
                                archivo.nombrearchivo = fileName;
                                archivo.rutaacceso = newPath;
                                archivo.idordentrabajo = long.Parse(idOrden);

                                await this._repo_Archivo.AddAsync(archivo);
                                await this._repo_OrdenTransporte.SaveAll();


                            }

                    }

                  
                }
                return Ok();
            }
            catch (System.Exception ex)
            {
                 return Ok();
            }
        }
        [HttpPost("UploadFileConfirm2")]
        [DisableRequestSizeLimit]
        public IActionResult UploadFileConfirm2(long id)
        {

            var archivo = new Archivo() ;

            try
            {
                
                var ruta =  _config.GetSection("AppSettings:Uploads").Value;

                var file = Request.Form.Files[0];
                var idOrden = id.ToString(); //Request.Form["idOrden"];
                
             
 

                string folderName = idOrden;
                string webRootPath = ruta ;
                string newPath = Path.Combine(webRootPath, folderName);
                

                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
                {
                    fileData = binaryReader.ReadBytes(Request.Form.Files[0].ContentDisposition.Length);
                }

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);

                    var checkextension = Path.GetExtension(fileName).ToLower();

                  
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {

                        file.CopyTo(stream);
                        
                        archivo.extension = checkextension;
                        archivo.nombrearchivo = fileName;
                        archivo.rutaacceso = newPath;
                        archivo.idordentrabajo = long.Parse(idOrden);

                        this._repo_Archivo.AddAsync(archivo);


                    }
                }
                return Ok();
            }
            catch (System.Exception ex)
            {
                 return Ok();
            }
        }
        [HttpPost("ConfirmarEntrega")]
        public async Task<IActionResult> ConfirmarEntrega(ConfirmarEntregaDto model)
        {
           
            string SMTP_SERVER =  _config.GetSection("AppSettings:SMTPSERVER").Value;
            string SMTP_MAIL =  _config.GetSection("AppSettings:MAIL_SMTP").Value;
            string SMTP_PASSWORD =  _config.GetSection("AppSettings:SMTP_PASSWORD").Value;
            string CORREO_PRUEBA =  _config.GetSection("AppSettings:PRUEBA_CORREO").Value;

            var incidencia = new IncidenciaForRegister();
            model.visible = true;

         

            var ordentrabajo = await _seguimiento.Obtener_OrdenTransporte(model.idordentrabajo);
            
           var _ordentrabajo = await  _repo_OrdenTransporte.Get(x=>x.id == model.idordentrabajo);

            // if(ordentrabajo.remitente_id == 134 || _ordentrabajo.remitente_id == 225  || _ordentrabajo.remitente_id == 239) {
            //     //var documentos = await _seguimiento.GetAllDocuments(model.idordentrabajo);
            //     if(_ordentrabajo.cantidad_fotos == 0 || _ordentrabajo.cantidad_fotos == null) {
            //                 return Ok("0");
            //     }
            // }
         
            var cliente_remitente = await _repo_cliente.Get(x=>x.id == ordentrabajo.remitente_id);

            if(ordentrabajo.notificacion != null)
               CORREO_PRUEBA = CORREO_PRUEBA + ";"  + ordentrabajo.notificacion;
            
            if (cliente_remitente.mail_notificacion != string.Empty)
                CORREO_PRUEBA = CORREO_PRUEBA + ";"  + cliente_remitente.mail_notificacion;



            
         //   var _ordentrabajo = await  _repo_OrdenTransporte.Get(x=>x.id == model.idordentrabajo);

            if(model.idtipoentrega == 15) { // OK
                        _ordentrabajo.estado_id = (int)Constantes.EstadoOrdenTransporte.Finalizado;
                        _ordentrabajo.fecha_entrega = DateTime.Now;
                        _ordentrabajo.lat_entrega =  model.lat;
                        _ordentrabajo.lng_entrega = model.lng;
                        _ordentrabajo.tipo_entrega_id = 15;
                        
            }
            else if( model.idtipoentrega == 16 ){ // PARCIAL
                         _ordentrabajo.estado_id = (int)Constantes.EstadoOrdenTransporte.Finalizado;
                        _ordentrabajo.fecha_entrega = DateTime.Now;
                        _ordentrabajo.lat_entrega =  model.lat;
                        _ordentrabajo.lng_entrega = model.lng;
                        _ordentrabajo.tipo_entrega_id = 16;
            }
            else if( model.idtipoentrega == 17 ){ // RECHAZO
                         _ordentrabajo.estado_id = (int)Constantes.EstadoOrdenTransporte.Finalizado;
                        _ordentrabajo.fecha_entrega = DateTime.Now;
                        _ordentrabajo.lat_entrega =  model.lat;
                        _ordentrabajo.lng_entrega = model.lng;
                        _ordentrabajo.tipo_entrega_id = 17;
            }
            
            incidencia.activo = true;
            incidencia.descripcion = model.descripcion ;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.orden_trabajo_id = model.idordentrabajo;
            incidencia.usuario_id =  model.usuario_id;
            
            incidencia.maestro_incidencia_id = model.idmaestroetapa;
            await _seguimiento.RegisterIncidencia(incidencia);

             

            // await _repo_Incidencia.AddAsync(incidencia);
             await _repo_OrdenTransporte.SaveAll();


            string htmlString =  _seguimiento.GetBodyMail(ordentrabajo.id.ToString(), ordentrabajo.destinatario,
            ordentrabajo.fecha_carga.ToShortDateString() , ordentrabajo.direccion_entrega , ordentrabajo.provincia_entrega
            ,ordentrabajo.numero_ot, ordentrabajo.Chofer, ordentrabajo.dni, ordentrabajo.Tracto,
            model.lat.ToString(), model.lng.ToString()) ;
                  

       

            string body =  "Se ha registrado una entrega (" +  model.descripcion + " ) para la OT " + _ordentrabajo.numero_ot;
                try
                {
                     MailHelper.EnviarMail(true,SMTP_SERVER,SMTP_MAIL,SMTP_PASSWORD,CORREO_PRUEBA,
                            "",   ordentrabajo.numero_ot , htmlString,true );

                             return Ok("ordentrabajo");
                }
                catch (System.Exception ex)
                {
                    
                  return Ok(ex)  ;
                }
            
        }
         [HttpPost("ConfirmarEntrega2")]
        public async Task<IActionResult> ConfirmarEntrega2(ConfirmarEntregaDto model)
        {
           
            string SMTP_SERVER =  _config.GetSection("AppSettings:SMTPSERVER").Value;
            string SMTP_MAIL =  _config.GetSection("AppSettings:MAIL_SMTP").Value;
            string SMTP_PASSWORD =  _config.GetSection("AppSettings:SMTP_PASSWORD").Value;
            string CORREO_PRUEBA =  _config.GetSection("AppSettings:PRUEBA_CORREO").Value;

            var incidencia = new IncidenciaForRegister();
            model.visible = true;

         

            var ordentrabajo = await _seguimiento.Obtener_OrdenTransporte(model.idordentrabajo);
            
           var _ordentrabajo = await  _repo_OrdenTransporte.Get(x=>x.id == model.idordentrabajo);

            if(ordentrabajo.remitente_id == 134 || _ordentrabajo.remitente_id == 225  || _ordentrabajo.remitente_id == 239) {
                //var documentos = await _seguimiento.GetAllDocuments(model.idordentrabajo);
                if(_ordentrabajo.cantidad_fotos == 0 || _ordentrabajo.cantidad_fotos == null) {
                            return Ok("0");
                }
            }
         
            var cliente_remitente = await _repo_cliente.Get(x=>x.id == ordentrabajo.remitente_id);

            if(ordentrabajo.notificacion != null)
               CORREO_PRUEBA = CORREO_PRUEBA + ";"  + ordentrabajo.notificacion;
            
            if (cliente_remitente.mail_notificacion != string.Empty)
                CORREO_PRUEBA = CORREO_PRUEBA + ";"  + cliente_remitente.mail_notificacion;



            
         //   var _ordentrabajo = await  _repo_OrdenTransporte.Get(x=>x.id == model.idordentrabajo);

            if(model.idtipoentrega == 15) { // OK
                        _ordentrabajo.estado_id = (int)Constantes.EstadoOrdenTransporte.Finalizado;
                        _ordentrabajo.fecha_entrega = DateTime.Now;
                        _ordentrabajo.lat_entrega =  model.lat;
                        _ordentrabajo.lng_entrega = model.lng;
                        _ordentrabajo.tipo_entrega_id = 15;
                        
            }
            else if( model.idtipoentrega == 16 ){ // PARCIAL
                         _ordentrabajo.estado_id = (int)Constantes.EstadoOrdenTransporte.Finalizado;
                        _ordentrabajo.fecha_entrega = DateTime.Now;
                        _ordentrabajo.lat_entrega =  model.lat;
                        _ordentrabajo.lng_entrega = model.lng;
                        _ordentrabajo.tipo_entrega_id = 16;
            }
            else if( model.idtipoentrega == 17 ){ // RECHAZO
                         _ordentrabajo.estado_id = (int)Constantes.EstadoOrdenTransporte.Finalizado;
                        _ordentrabajo.fecha_entrega = DateTime.Now;
                        _ordentrabajo.lat_entrega =  model.lat;
                        _ordentrabajo.lng_entrega = model.lng;
                        _ordentrabajo.tipo_entrega_id = 17;
            }
               else if( model.idtipoentrega == 18 ){ // RECHAZO
                         _ordentrabajo.estado_id = (int)Constantes.EstadoOrdenTransporte.Finalizado;
                        _ordentrabajo.fecha_entrega = DateTime.Now;
                        _ordentrabajo.lat_entrega =  model.lat;
                        _ordentrabajo.lng_entrega = model.lng;
                        _ordentrabajo.tipo_entrega_id = 18;
            }
            
            incidencia.activo = true;
            incidencia.descripcion = model.descripcion ;
            incidencia.fecha_incidencia = DateTime.Now;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.orden_trabajo_id = model.idordentrabajo;
            incidencia.usuario_id =  model.usuario_id;
       

            
            incidencia.maestro_incidencia_id = model.idmaestroetapa;
            await _seguimiento.RegisterIncidencia(incidencia);

             

            // await _repo_Incidencia.AddAsync(incidencia);
             await _repo_OrdenTransporte.SaveAll();

            // #region Cambiar de estado al manifiesto 
            // var ordenes_eval = await _repo_OrdenTransporte.GetAll(x=>x.manifiesto_id == manifiesto.id);

            // foreach (var item in ordenes_eval)
            // { 
                
            //     if(item.estado_id ==  (int) Constantes.EstadoOrdenTransporte.Finalizado
            //     || item.estado_id == (int) Constantes.EstadoOrdenTransporte.Cerrado)
            //     {
            //            manifiesto.estado_id =  (int) Constantes.EstadoManifiesto.Finalizado;
            //     }
               
            //     else {
            //             manifiesto.estado_id =  (int) Constantes.EstadoManifiesto.Registrado;
            //             break;
            //     }
            // }
            // /////////////////////////////////////
            // #endregion
           // await _repo_OrdenTransporte.SaveAll();


            string htmlString =  _seguimiento.GetBodyMail(ordentrabajo.id.ToString(), ordentrabajo.destinatario,
            ordentrabajo.fecha_carga.ToShortDateString() , ordentrabajo.direccion_entrega , ordentrabajo.provincia_entrega
            ,ordentrabajo.numero_ot, ordentrabajo.Chofer, ordentrabajo.dni, ordentrabajo.Tracto,
            model.lat.ToString(), model.lng.ToString()) ;
                  

       

            string body =  "Se ha registrado una entrega (" +  model.descripcion + " ) para la OT " + _ordentrabajo.numero_ot;
                try
                {
                     MailHelper.EnviarMail(true,SMTP_SERVER,SMTP_MAIL,SMTP_PASSWORD,CORREO_PRUEBA,
                            "",   ordentrabajo.numero_ot , htmlString,true );

                             return Ok("ordentrabajo");
                }
                catch (System.Exception ex)
                {
                    
                  return Ok(ex)  ;
                }
            
        }
        [HttpPost("ConfirmarEntregaBajoDemanda")]
        public async Task<IActionResult> ConfirmarEntregaBajoDemanda(ConfirmarEntregaDto model)
        {

            var incidencia = new IncidenciaForRegister();
            
            var ordentrabajo = await _seguimiento.Obtener_OrdenTransporte(model.idordentrabajo);
            var _ordentrabajo = await  _repo_OrdenTransporte.Get(x=>x.id == model.idordentrabajo);


            _ordentrabajo.estado_id = (int)Constantes.EstadoOrdenTransporte.Finalizado;
            _ordentrabajo.fecha_entrega = model.fechaetapa;
            _ordentrabajo.tipo_entrega_id = model.idmaestroetapa;
             await _repo_OrdenTransporte.SaveAll();
           
            

            incidencia.activo = true;
            incidencia.descripcion = model.descripcion ;
            incidencia.fecha_incidencia = model.fechaetapa;
            incidencia.fecha_registro = DateTime.Now;
            incidencia.orden_trabajo_id = model.idordentrabajo;
            incidencia.usuario_id =     1;
            incidencia.maestro_incidencia_id = model.idmaestroetapa;

            await _seguimiento.RegisterIncidencia(incidencia);
        


            return Ok("ordentrabajo");

       

           
            
        }
        [HttpGet("getAllDocumentos")]
        public async Task<IActionResult> getAllDocumentos(int id)
        { 
            var resp  = await _seguimiento.GetAllDocuments(id);
            return Ok (resp);
        }

        [HttpGet("DownloadPlantilla")]
        public FileResult DownloadPlantilla()
        {
            string filePath =   _config.GetSection("AppSettings:UploadsDocuments").Value;
            
            
            // var documento = _repo_Archivo.Get(x=>x.idarchivo == documentoId).Result;

            IFileProvider provider = new PhysicalFileProvider(filePath );
            IFileInfo fileInfo = provider.GetFileInfo("plantilla_toscanos.xlsx");
            var readStream = fileInfo.CreateReadStream();
            //var mimeType = "application/vnd.ms-excel";
            return File(readStream,GetContentType(filePath + "//" + "plantilla_toscanos.xlsx") , "plantilla_toscanos.xlsx");
            
        }

        [HttpGet("DownloadFactura")]
        public FileResult DownloadFactura(string carpeta)
        {
            string filePath =   _config.GetSection("AppSettings:UploadsDocuments").Value;


            //  FileStream originalFileStream = System.IO.File.Open(OriginalFileName, FileMode.Open);
            //  FileStream compressedFileStream = System.IO.File.Create(CompressedFileName);
            //  var compressor = new GZipStream(compressedFileStream, CompressionMode.Compress);
            //   originalFileStream.CopyTo(compressor);
       


         //     List<DocumentoDto> temp = new List<DocumentoDto>();
         //      DocumentoDto ar = new DocumentoDto();             
               DirectoryInfo di = new DirectoryInfo("D:\\APPS\\Toscanos\\upload");
               DirectoryInfo[] diArr = di.GetDirectories();


                string startPath = di.FullName + "\\" + carpeta;
                string zipPath =di.FullName + "\\"  + carpeta + ".zip";
                
                System.IO.File.Delete(zipPath);


                ZipFile.CreateFromDirectory(startPath, zipPath);

                
               
              
               
                //     using (FileStream zipFile = System.IO.File.Open("compressed_files.zip", FileMode.Create))
                //     {
                //         foreach (FileInfo fileToCompress in diFinal.GetFiles())
                //         {

                //         // File to be added to archive
                //         using (FileStream source1 = System.IO.File.Open(fileToCompress.FullName, FileMode.Open, FileAccess.Read))
                //         {
                //             // File to be added to archive
                        
                                
                //                 using (var archive = new Archive())
                //                 {
                //                     // Add files to the archive
                //                     archive.CreateEntry("alice29.txt", source1);
                //                     archive.CreateEntry("asyoulik3.txt", source2);
                //                     // ZIP the files
                //                     archive.Save(zipFile, new ArchiveSaveOptions() { Encoding = Encoding.ASCII, ArchiveComment = "two files are compressed in this archive" });
                //                 }
                            
                //         }
                //         }

                //     }



                //     using (FileStream originalFileStream = fileToCompress.OpenRead())
                //     {
                //         if (( System.IO.File.GetAttributes(fileToCompress.FullName) &
                //         FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".zip")
                //         {
                //             using (FileStream compressedFileStream =  System.IO.File.Create(fileToCompress.FullName + ".zip"))
                //             {
                //                 using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                //                 CompressionMode.Compress))
                //                 {
                //                     originalFileStream.CopyTo(compressionStream);
                //                 }
                //             }
                //             // FileInfo info = new FileInfo(directoryPath + Path.DirectorySeparatorChar + fileToCompress.Name + ".gz");
                //             // Console.WriteLine($"Compressed {fileToCompress.Name} from {fileToCompress.Length.ToString()} to {info.Length.ToString()} bytes.");
                //         }
                //     }
                // }


           
            
            
         //    var documento = _repo_Archivo.Get(x=>x.idarchivo == documentoId).Result;

            // IFileProvider provider = new PhysicalFileProvider("D:\\APPS\\Toscanos\\upload\\" +  carpeta);
            // IFileInfo fileInfo = provider.GetFileInfo("D:\\APPS\\Toscanos\\upload\\" + carpeta +  ".zip");
            // var readStream = fileInfo.CreateReadStream();

             const string contentType ="application/zip";
            HttpContext.Response.ContentType = contentType;
            var result = new FileContentResult(System.IO.File.ReadAllBytes("D:\\APPS\\Toscanos\\upload\\" + carpeta +  ".zip"), contentType)
            {
                // It can be zip file or pdf but we need to change the extension respectively
                FileDownloadName = $"{"D:\\APPS\\Toscanos\\upload\\" + carpeta }.zip"
            };
              return result;
            
            //return File("D:\\APPS\\Toscanos\\upload\\" + carpeta +  ".zip",  carpeta,GetContentType("D:\\APPS\\Toscanos\\upload\\" + carpeta +  ".zip") , false);
            
        }

        [HttpGet("DownloadArchivo")]
        public FileResult DownloadArchivo(long documentoId)
        {
            string filePath =   _config.GetSection("AppSettings:UploadsDocuments").Value;
             var documento = _repo_Archivo.Get(x=>x.idarchivo == documentoId).Result;

            IFileProvider provider = new PhysicalFileProvider(documento.rutaacceso );
            IFileInfo fileInfo = provider.GetFileInfo(documento.nombrearchivo);
            var readStream = fileInfo.CreateReadStream();
            //var mimeType = "application/vnd.ms-excel";
            return File(readStream,GetContentType(documento.rutaacceso + "//" + documento.nombrearchivo) , documento.nombrearchivo);
            
        }
        private string GetContentType(string path)
        {
           var provider = new FileExtensionContentTypeProvider();
           string contentType;
           if(!provider.TryGetContentType(path, out contentType))
           {
               contentType = "application/octet-stream";
           }
           return contentType;
        }
        [HttpPost("UpdateGeolocalizacion")]
        public async Task<IActionResult> UpdateGeolocalizacion(GeoEquipoTransporteForRegister geoEquipoTransporteForRegister)
        {
             var result =  await  _repo.RegisterGeoLocalizacion(geoEquipoTransporteForRegister);
             return Ok(result);
        }
        [HttpGet("GetLocalizacion")]
        public async Task<IActionResult> GetLocalizacion(int id)
        { 
            var resp  = await  _seguimiento.GetLocalizacion(id);
            return Ok (resp);
        }
        [HttpGet("GetLiveView")]
        public async Task<IActionResult> GetLiveView()
        { 
            var resp  = await  _seguimiento.GetLiveView();
            return Ok (resp);
        }




        [AllowAnonymous]
        [HttpGet("GetOrden")]
        public async Task<IActionResult> GetOrden(long id)
        { 
            var resp  = await  _repo_OrdenTransporte.Get(x=>x.id == id);
            return Ok (resp);
        }
        [AllowAnonymous]
        [HttpPost("UpdateEncuesta")]
        public async Task<IActionResult> UpdateEncuesta(OrdenTransporte ordenTransporte)
        {
            var resp  = await  _repo_OrdenTransporte.Get(x=>x.id == ordenTransporte.id);
            resp.nivel_satisfaccion = ordenTransporte.nivel_satisfaccion;
            resp.observacion_satisfaccion = ordenTransporte.observacion_satisfaccion;
             
             
             
             var updatedOrden = _repo_OrdenTransporte.SaveAll();

            return Ok(resp);
        }

        [HttpPost("UpdateOrden")]
        public async Task<IActionResult> UpdateOrden(OrdenTransporte ordenTransporte)
        { 
            var resp  = await  _repo_OrdenTransporte.Get(x=>x.id == ordenTransporte.id);

            resp.cantidad = ordenTransporte.cantidad;
            resp.factura = ordenTransporte.factura;
            resp.oc = ordenTransporte.oc;
            resp.peso = ordenTransporte.peso;
            resp.volumen = ordenTransporte.volumen;
            resp.delivery = ordenTransporte.delivery;
            resp.destinatario_id = ordenTransporte.destinatario_id;
            resp.fecha_carga = ordenTransporte.fecha_carga;
            resp.fecha_entrega = ordenTransporte.fecha_entrega;
            resp.fecha_salida = ordenTransporte.fecha_salida;
            resp.destinatario = ordenTransporte.destinatario;
          

            var updatedOrden = _repo_OrdenTransporte.SaveAll();

            return Ok (resp);
        }

        [HttpPost("UpdateOrdenEliminar")]
        public async Task<IActionResult> UpdateOrdenEliminar(OrdenTransporteDto ordenTransporte)
        { 
              var resp  = await  _repo_OrdenTransporte.Get(x=>x.id == ordenTransporte.id);
              resp.activo = false;
              var updatedOrden = _repo_OrdenTransporte.SaveAll();

              return Ok (resp);
        }


        [HttpGet("GetCantidadDespacho")]
        public async Task<IActionResult> GetCantidadDespacho(int? remitente_id, string fec_ini, string fec_fin)
        {
            var result = await _reporte.GetTotalDespachos(remitente_id, fec_ini,fec_fin);
            return Ok (result);
        }
        [HttpGet("GetTotalActivity")]
        public async Task<IActionResult> GetTotalActivity()
        {
            var result = await _seguimiento.GetActivityTotal();
            return Ok (result);
        }
        [HttpGet("GetTotalActivityRecojo")]
        public async Task<IActionResult> GetTotalActivityRecojo()
        {
            var result = await _seguimiento.GetActivityTotalRecojo();
            return Ok (result);
        }
        [HttpGet("GetTotalActivityClientes")]
        public async Task<IActionResult> GetTotalActivityClientes()
        {
            var result = await _seguimiento.GetActivityTotalClientes();
            return Ok (result);
        }
         [HttpGet("GetActivityVehiculosRuta")]
        public async Task<IActionResult> GetActivityVehiculosRuta()
        {
            var result = await _seguimiento.GetActivityVehiculosRuta();
            return Ok (result);
        }
        [HttpGet("GetActivityOTTotalesYEntregadas")]
        public async Task<IActionResult> GetActivityOTTotalesYEntregadas()
        {
            var result = await _seguimiento.GetActivityOTTotalesYEntregadas();
            return Ok (result);
        }


        [HttpGet("GetReporteServicio")]
        public async Task<IActionResult> GetReporteServicio()
        {
            var result = await _seguimiento.GetReporteServicio();
            return Ok (result);
        }
        [HttpGet("GetReporteEncuesta")]
        public async Task<IActionResult> GetReporteEncuesta(int? remitente_id, int? usuario_id, string fec_ini, string fec_fin)
        {
            var result = await _reporte.GetReporteEncuesta(remitente_id, usuario_id, fec_ini,fec_fin);
            return Ok (result);
        }

        [HttpGet("GetDespachosATiempo")]
        public async Task<IActionResult> GetDespachosATiempo(int? remitente_id, string fec_ini, string fec_fin)
        {
            var result = await _reporte.GetDespachosATiempo(remitente_id, fec_ini,fec_fin);
            return Ok (result);
        }
       
        [HttpGet("GetDespachosTiempoEntrega")]
        public async Task<IActionResult> GetDespachosTiempoEntrega(int? remitente_id, string fec_ini, string fec_fin)
        {
            var result = await _reporte.GetDespachosTiempoEntrega(remitente_id, fec_ini,fec_fin);
            return Ok (result);
        }
       [HttpGet("GetDaysOfWeek")]
        public async Task<IActionResult> GetDaysOfWeek(int? remitente_id, string fec_ini, string fec_fin)
        {
            var result = await _reporte.GetDaysOfWeek(remitente_id, fec_ini,fec_fin);
            return Ok (result);
        }
        [HttpGet("GetCantidadxManifiesto")]
        public async Task<IActionResult> GetCantidadxManifiesto(int? remitente_id, string fec_ini, string fec_fin)
        {
            var result = await _reporte.GetCantidadxManifiesto(remitente_id, fec_ini,fec_fin);
            return Ok (result);
        }
        [HttpGet("GetDespachosPuntualidad")]
        public async Task<IActionResult> GetDespachosPuntualidad(int? remitente_id, string fec_ini, string fec_fin)
        {
            var result = await _reporte.GetDespachosPuntualidad(remitente_id, fec_ini,fec_fin);
            return Ok (result);
        }
        [HttpGet("GetAsignacionUnidadesVehiculo")]
        public async Task<IActionResult> GetAsignacionUnidadesVehiculo()
        {
            var result = await _reporte.GetAsignacionUnidadesVehiculo();
            return Ok (result);
        }
        [HttpGet("GetAsignacionUnidadesVehiculoTerceros")]
        public async Task<IActionResult> GetAsignacionUnidadesVehiculoTerceros()
        {
            var result = await _reporte.GetAsignacionUnidadesVehiculoTerceros();
            return Ok (result);
        }
        [HttpGet("GetVehiculoPropios")]
        public async Task<IActionResult> GetVehiculoPropios()
        {
            var result = await _reporte.GetVehiculoPropios();
            return Ok (result);
        }

        [HttpPost("ActualizarIncidencia")]
        public async Task<IActionResult> UpdateEncuesta(IncidenciaForUpdate incidencia)
        {
            var resp = await _repo.ActualizarIncidencia(incidencia);
            return Ok(resp);
        }

         [HttpPost("ActualizarKMxVehiculo")]
        public async Task<IActionResult> ActualizarKMxVehiculo(ManifiestoForUpdate manifiestoForUpdate)
        {
            var manifiesto = await _repo_Manifiesto.Get(x=> x.id == manifiestoForUpdate.id);
            manifiesto.kmrecorridos = manifiestoForUpdate.kmrecorridos;

            var resp = await _repo_Manifiesto.SaveAll();
            
            return Ok(resp);
        }

        [HttpGet("GetManifiestoPendienteSustentar")] 
        public async Task<IActionResult> GetManifiestoPendienteSustentar(string documento)
        {
            var result = await _seguimiento.GetManifiestoPendienteSustentar(documento);
            return Ok(result);
        }
        // [HttpGet("GetOrdersForManifest")] 
        // public async Task<IActionResult> GetOrdersForManifest(string numero_manifiesto)
        // {
        //     var result = await _repo_OrdenTransporte.Get(x)
        //     return Ok(result);
        // }
        

        [HttpGet("GetTipoDocumentoSustento")] 
        public async Task<IActionResult> GetTipoDocumentoSustento()
        {
            var result = await _seguimiento.GetTipoDocumentoSustento();
            return Ok(result);
        }

        [HttpGet("GetTipoSustento")] 
        public async Task<IActionResult> GetTipoSustento()
        {
            var result = await _seguimiento.GetTipoSustento();
            return Ok(result);
        }

        [HttpGet("GetSustentoManifiesto")] 
        public async Task<IActionResult> GetSustentoManifiesto(long manifiesto)
        {
            var result = await _seguimiento.GetSustentoManifiesto(manifiesto);
            return Ok(result);
        }

        [HttpGet("GetDocumentosSustento")] 
        public async Task<IActionResult> GetDocumentosSustento(long? sustento,int? tipo)
        {
            var result = await _seguimiento.GetDocumentosSustento(sustento,tipo);
            return Ok(result);
        }

        [HttpGet("GetDocumentosSustentoPendientesAprobacion")] 
        public async Task<IActionResult> GetDocumentosSustentoPendientesAprobacion(string docConductor)
        {
            var result = await _seguimiento.GetDocumentosSustentoPendientesAprobacion(docConductor);
            return Ok(result);
        }

        [HttpGet("GetUsuarioAprobacionSustento")] 
        public async Task<IActionResult> GetUsuarioAprobacionSustento()
        {
            var result = await _seguimiento.GetUsuarioAprobacionSustento();
            return Ok(result);
        }

        [HttpGet("GetRazonSocialSustento")] 
        public async Task<IActionResult> GetRazonSocialSustento(string ruc)
        {
            var result = await _seguimiento.GetRazonSocialSustento(ruc);
            return Ok(result);
        }

        [HttpGet("GetTiposDocumentoEmisor")] 
        public async Task<IActionResult> GetTiposDocumentoEmisor()
        {
            var result = await _seguimiento.GetTiposDocumentoEmisor();
            return Ok(result);
        }

        [HttpPost("RegisterSustento")] 
        public async Task<IActionResult> RegisterSustento(SustentoForRegister model)
        {
            var result = await _repo.RegisterSustento(model);
            return Ok(result);
        }

        [HttpPost("ActualizarSustento")]
        public async Task<IActionResult> ActualizarSustento(SustentoForRegister model)
        {
            var result = await _repo.ActualizarSustento(model);
            return Ok(result);
        }

        [HttpPost("FinalizarSustento")]
        public async Task<IActionResult> FinalizarSustento(FinalizarSustentoForUpdate model)
        {
            var result = await _repo.FinalizarSustento(model);
            return Ok(result);
        }

        [HttpPost("RegisterSustentoDetalle")]
        public async Task<IActionResult> RegisterSustentoDetalle(SustentoDetalleForRegister model)
        {
            var result = await _repo.RegisterSustentoDetalle(model);
            return Ok(result);
        }

        [HttpPost("AprobarDocumentoSustento")]
        public async Task<IActionResult> AprobarDocumentoSustento(long documento)
        {
            var result = await _repo.AprobarDocumentoSustento(documento,1);
            return Ok(result);
        }

        [HttpPost("RechazarDocumentoSustento")]
        public async Task<IActionResult> RechazarDocumentoSustento(long documento)
        {
            var result = await _repo.RechazarDocumentoSustento(documento,1);
            return Ok(result);
        }


        [HttpGet("GetAllSustento")]
        public async Task<IActionResult> GetAllSustento(GetSustentoResult model)
        {
           var result = await _seguimiento.GetAllSustento(model);
           
           return Ok(result);
        }
        [HttpDelete("DeleteSustentoDetalle")]
        public async Task<IActionResult> DeleteSustentoDetalle(long id)
        {
            var result = await _repo.DeleteSustentoDetalle(id) ;
            return Ok(result);
        }


        ////////////////////////////////////////////////
        ////////////////////////////////////////////////
        ///////////////////////////////////////////////
        [HttpGet("GetReporteServicio2")]
        public async Task<IActionResult> ReporteServicio2(string idclientes)
        {
           var result = await _seguimiento.GetReporteServicio_unidadesasignadas(idclientes);
           return Ok(result);
        }
        [HttpGet("GetReporteServicio2")]
        public async Task<IActionResult> ReporteServicio2_valorizacion(string idclientes)
        {
           var result = await _seguimiento.GetReporteServicio_valorizacion(idclientes);
           return Ok(result);
        }
       
        [HttpGet("GetManifiesto")]
        public async Task<IActionResult> GetManifiesto(long id)
        {
               var result = await _repo_Manifiesto.Get(x=>x.id == id);
               return Ok(result);
        }
          [HttpPost("UpdateManifiesto")]
        public async Task<IActionResult> UpdateManifiesto(ManifiestoForUpdate manifiestoForUpdate)
        {

             var   ordenes = await _repo_OrdenTransporte.GetAll(x=> x.manifiesto_id ==  manifiestoForUpdate.id
              && x.activo == true  );
                
                // Actualiza a cero los valorizados de todas las ordenes.

                foreach (var item in ordenes)
                {
                    item.valorizado = 0;
                }
                await _repo_OrdenTransporte.SaveAll();

                
                // Actualiza la primera OT al valor total del manifiesto

                ordenes.ToList()[0].valorizado = manifiestoForUpdate.valorizado;

            
             var _manifiesto = await  _repo_Manifiesto.Get(x => x.id == manifiestoForUpdate.id);
            
            _manifiesto.valorizado = manifiestoForUpdate.valorizado;
          //  _manifiesto.valorizadoFluvial = manifiestoForUpdate.valorizadoFluvial;
            //_manifiesto.bejaranoiquitos = manifiestoForUpdate.bejaranoiquitos;
            //_manifiesto.bejaranopucallpa = manifiestoForUpdate.bejaranopucallpa;
            _manifiesto.costotercero = manifiestoForUpdate.costotercero;
            _manifiesto.deestiba  = manifiestoForUpdate.deestiba;
           // _manifiesto.estiba = manifiestoForUpdate.estiba;
            //_manifiesto.estiba_adicional = manifiestoForUpdate.estiba_adicional;
            //_manifiesto.fluvial = manifiestoForUpdate.fluvial;
            //_manifiesto.oriental = manifiestoForUpdate.oriental;
            _manifiesto.otrosgastos = manifiestoForUpdate.otrosgastos;
            _manifiesto.retorno_tarifa = manifiestoForUpdate.retorno_tarifa;
            _manifiesto.sobreestadia_tarifa = manifiestoForUpdate.sobreestadia_tarifa;
            _manifiesto.adicionales_tarifa = manifiestoForUpdate.adicionales_tarifa;
          
            var result = await _repo_Manifiesto.SaveAll();


            return Ok(result);
        }
    
        
        [HttpPost("registrardespachos")]
        public async Task<IActionResult> registrardespachos(OrdenTrabajoForRegister ordenTrabajoForRegister)
        {
            var carga = new CargaMasivaForRegister();
            var entidades = new List<CargaMasivaDetalleForRegister>();
            CargaMasivaDetalleForRegister entidad ;


            carga.estado_id = 1;
            carga.fecha_registro = DateTime.Now;
            carga.cantidad_total = Convert.ToInt32(ordenTrabajoForRegister.cantidad_total);
            carga.peso_total =  ordenTrabajoForRegister.peso_total;
            carga.usuario_id = 296; // autorex
            carga.oc = ordenTrabajoForRegister.OC;
            

            ordenTrabajoForRegister.documentos.ForEach (x => {
                entidad = new CargaMasivaDetalleForRegister();
                entidad.asignado = true;
                entidad.cantidad = Convert.ToInt32(x.cantidad);
                entidad.carreta =  "";
                entidad.conductor = x.conductor.Trim();
                entidad.owner = x.owner.Trim();
                entidad.delivery = x.id_documento_salida.Trim();
                entidad.destinatario = x.nombre_recepcion.Trim();
                entidad.direccion_destino_servicio = x.direccion_entrega.Trim();
                entidad.direccion_carga = x.agencia_direccion.Trim();
                entidad.distrito_destino_servicio = x.distrito;
                entidad.distrito_carga = "LIMA";
                entidad.factura = x.factura;
                entidad.fecha_carga = DateTime.Now;
                entidad.fecha_entrega = DateTime.Now;
                entidad.fecha_salida = DateTime.Now;
                entidad.hora_carga = DateTime.Now.ToShortTimeString();
                entidad.hora_entrega = "";
                entidad.hora_salida = "";
                entidad.oc =  ordenTrabajoForRegister.OC.Trim();
                entidad.peso = x.peso.Value;
                entidad.provincia  =  "LIMA";//
                entidad.tracto = x.tracto.Split('-')[0] +  x.tracto.Split('-')[1] ; 
                entidad.volumen = x.volumen.Value;
                entidad.tiposervicio = "ULTIMA MILLA";
                entidad.direccion_entrega  = x.direccion_entrega.Trim();
                entidad.shipment = x.numero_remesa;
                entidades.Add(entidad);

            });




            var result = true;
            var resp =  await _repo.RegisterCargaMasiva(carga, entidades);

            //Generar Ordenes de trabajo y Manifiestos      
           

            var detalles_cargados =  _repo_CargaMasiva.GetAll(x=>x.carga_id == resp).Result;

            var detalles_separados =  detalles_cargados.GroupBy(x=>x.owner);

            foreach (var item in detalles_separados)
            {
                var lista = item.ToList();
                
                item.ToList().ForEach(x=> {

                    if(x.owner == "9")
                      {
                          x.remitente = "AUTOREX PERUANA S.A.";
                      }
                      else
                      {
                          x.remitente = "ROBERT BOSCH S.A.C.";
                      }
                      x.provincia  = "Lima";
                    
                });

                var manifiestos = await _seguimiento.ObtenerEntidades_Manifiesto_autorex(lista);
                //Registrar manifiestos 
                await _repo.RegisterOrdenes_Interface(manifiestos,296);

            }
            return Ok(result);
        }
        [HttpGet("getdespacho")]
        public async Task<IActionResult> getdespacho(string Oc)
        {
            
              var entidades = new List<OrdenTrabajoDetalleForRegister>();
            OrdenTrabajoDetalleForRegister entidad ;

            var detalles_cargados =  await _repo_CargaMasiva.GetAll(x=>x.oc == Oc);

            var ordenes = await _repo_OrdenTransporte.GetAll(x=>  x.oc.Trim() == Oc.Trim() );



            foreach (var item in ordenes)
            {

                 //var unique  = await 
     

                //  if(ordenes == null || ordenes.Count() > 0)
                //  {

                     
              //   var orden = ordenes.ToList()[0];
                 var ord = await _seguimiento.Obtener_OrdenTransporte(item.id.Value);
                 

                 entidad = new OrdenTrabajoDetalleForRegister();
                 entidad.cantidad = item.cantidad;
                 entidad.carreta = ord.Carreta;
                 entidad.conductor = ord.Chofer;
                 entidad.direccion_entrega = item.direccion_entrega;
                 entidad.distrito = ord.distrito_carga;
                 entidad.factura = item.factura;
                 entidad.guia = item.guias;
                 entidad.id_documento_salida = item.delivery;
                 entidad.nombre_recepcion = item.destinatario;
                 entidad.factura = item.factura;

                if(item.estado_id == 12 )
                {
                    if(item.tipo_entrega_id == 15)
                    {
                        entidad.tipo_cedible = "CED";
                    }
                    else if (item.tipo_entrega_id  == 16)
                    {
                        entidad.tipo_cedible = "DEV";
                    }
                    else if (item.tipo_entrega_id == 17 )
                    {
                        entidad.tipo_cedible = "REE";  
                    }
                }
                else 
                {
                     entidad.tipo_cedible ="";
                }


                 if(ord.fecha_entrega != null)
                    entidad.fecha_cedible = ord.fecha_entrega.Value.ToShortDateString() +  " " + ord.fecha_entrega.Value.ToString("HH:mm"); ; // Convert.ToDateTime().ToShortDateString() + " " + ord.hora_entrega;   //item.fecha_cedible.HasValue ?   item.fecha_cedible.Value.ToShortDateString() : "";

                //  entidad.contacto = item.contacto;
                //  entidad.incidencia = item.incidencia;
                //  entidad.boleto = item.boleto;

                 entidades.Add(entidad);
                 

            }
            
            return Ok(entidades);
        }

        // [HttpGet("SearchOrden")]
        // public async Task<IActionResult> SearchOrden(string criterio, int dias)
        // {
        //      var result  = await _seguimiento.BuscarOrdensTransporte(criterio,dias);
        //      return Ok(result);
        // }


        [HttpDelete("DeleteSustento")]
        public async Task<IActionResult> DeleteSustento(long id)
        {
            var result = await _repo.DeleteSustento(id) ;
            return Ok(result);
        }
        [HttpGet("GetOrdenByWayPoint")]
        public async Task<IActionResult> GetOrdenByWayPoint(long manifiesto_id, string lat , string lng, int tiempo, string ordenentrega)
        { 
            int contador = 7;
            string lat_final = lat.Substring(0,contador);
            string lng_final = lng.Substring(0,contador);

            var resp  = await  _seguimiento.GetOrdenByWayPoint(manifiesto_id,lat_final,lng_final);

    
            if(resp != null)
            {
                var orden = await _repo_OrdenTransporte.Get(x => x.id == resp.id);
                orden.orden_entrega = ordenentrega;
                orden.fecha_eta = DateTime.Now.AddSeconds(tiempo);
                await _repo_OrdenTransporte.SaveAll();
                

            }
                
            

            return Ok (resp);
        }
        
      
    }             
}
