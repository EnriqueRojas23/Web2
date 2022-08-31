using System;
using System.Collections.Generic;
using System.Linq;
using CargaClic.Repository.Contracts.Seguimiento;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;
using CargaClic.Data;
using CargaClic.Data.Interface;
using CargaClic.Domain.Mantenimiento;
using CargaClic.Contracts.Parameters.Mantenimiento;
using CargaClic.Contracts.Results.Mantenimiento;
using Common.QueryHandlers;
using CargaClic.ReadRepository.Interface.Seguimiento;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;
using System.Threading.Tasks;
using CargaClic.Domain.Seguimiento;
using CargaClic.API.Dtos.Matenimiento;
using AutoMapper;
using CargaClic.Common;
using CargaClic.Repository.Interface.Seguimiento;
using System.Text.RegularExpressions;
using CargaClic.Domain.Seguridad;
using CargaClic.Data.Contracts.Parameters.Seguridad;
using CargaClic.Data.Contracts.Results.Seguridad;

namespace Toscanos.API.Data
{
    public class Seguimiento
    {
         private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IRepository<Vehiculo> _repo_Vehiculo;
        private readonly IRepository<Chofer> _repo_Chofer;
        private readonly IRepository<Proveedor> _repo_Proveedor;
        private readonly IRepository<Incidencia> _repo_Incidencia;
        private readonly ISeguimientoReadRepository _repo_Seguimiento;
        private readonly IOrdenRepository _repository;
        private readonly IQueryHandler<ObtenerEquipoTransporteParameter> _handlerEqTransporte;
        private readonly IRepository<User> _repo_User;
        private readonly IQueryHandler<ListarRolesPorUsuarioParameter> _hanlder_RolUser;

        public Seguimiento(DataContext context,
        IMapper mapper,
        IRepository<Vehiculo> repo_Vehiculo,
        IRepository<Proveedor> repo_Proveedor,
        IRepository<Chofer> repo_Chofer,
        IRepository<Incidencia> repo_Incidencia,
        ISeguimientoReadRepository repo_seguimiento,
        IOrdenRepository repository,
        IQueryHandler<ObtenerEquipoTransporteParameter> handlerEqTransporte,
        IRepository<User> repo_User, 
        IQueryHandler<ListarRolesPorUsuarioParameter> hanlder_RolUser)
        {
            _context = context;
            _repo_Proveedor = repo_Proveedor;
            _mapper = mapper;
            _repo_Chofer = repo_Chofer;
            _repo_Incidencia = repo_Incidencia;
            _repo_Seguimiento = repo_seguimiento;
            _repository = repository;
            _repo_Vehiculo = repo_Vehiculo;
            _handlerEqTransporte = handlerEqTransporte;
            _repo_User = repo_User;
            _hanlder_RolUser = hanlder_RolUser;
        }
        public EquipoTransporte GetEquipoTransporte(long id) => _repo_Seguimiento.GetEquipoTransporte(id);


        public async Task<IEnumerable<GetSustentoManifiesto>> GetSustentoManifiestoCerrado(string manifiesto) 
        {
                return await _repo_Seguimiento.GetSustentoManifiestoCerrado(manifiesto);
        }
        public async Task<IEnumerable<GetOrdenTransporte>> getPendientesPorDia(string fecha) 
        {
                return await _repo_Seguimiento.getPendientesPorDia(fecha);
        }



        //
        public async Task<long?> GetEquipoTransporte(string placa_carreta, string placa_tracto, string dni_chofer)
        {
                Vehiculo objCarreta ;
                if(placa_carreta != string.Empty){
                    objCarreta =  _repo_Vehiculo.Get(x=>x.Placa == placa_carreta).Result;
                     if(objCarreta == null)
                      throw new ArgumentException( $"No Existe la placa de la carreta {placa_carreta}.");
                }
                 else objCarreta = new Vehiculo();

                Vehiculo objTracto ;
                if(placa_tracto != string.Empty){
                    objTracto =  _repo_Vehiculo.Get(x=>x.Placa == placa_tracto).Result;
                    if(objTracto == null)
                      throw new ArgumentException($"No Existe la placa del tracto {placa_tracto}.");
                      else{}
                } else throw new ArgumentException("No Existe la placa del tracto");

                Chofer objChofer ;
                 if(dni_chofer != string.Empty){
                    objChofer =  _repo_Chofer.Get(x=>x.Dni == dni_chofer).Result;
                    if(objChofer == null)
                      throw new ArgumentException($"No Existe el DNI {dni_chofer}.");
                    else {}
                } else throw new ArgumentException("No Existe el DNI");



                var param = new ObtenerEquipoTransporteParameter
                {
                    VehiculoId = objTracto.Id 
                    
                };
                //Jala al ultimo chofe y eso no deberia ser asi
                var result = (ObtenerEquipoTransporteResult)   _handlerEqTransporte.Execute(param);
                if(result == null)
                {
                   return await Registrar_EquipoTransporte(objCarreta.Placa, objTracto.Placa, objChofer.Dni );
                }
                else
                {
                    return await Registrar_EquipoTransporte(objCarreta.Placa, objTracto.Placa, objChofer.Dni );
                }
              
               // return result.Id;

        }
      
        public int? ObtenerClienteId(string razon_social)
        {
            var Cliente =   _context.Cliente.Where(x=>x.razon_social == razon_social).SingleOrDefault();
            if(Cliente != null) 
                return Cliente.id;
            else  throw new ArgumentException( $" No existe el cliente {razon_social}.");
        }
        
        

        public int? ObtenerDistritoId(string distrito)
        {
            if(distrito == string.Empty) return null;
            
            var Distrito =  _context.Distritos.Where(x=>x.distrito == distrito).FirstOrDefault();
            if(Distrito != null) 
                return Distrito.iddistrito;
              else  throw new ArgumentException( $" No existe el distrito {distrito}.");
        }

        public int? ObtenerProvinciaId(string provincia)
        {
            var Provincia =  _context.Provincias.Where(x=>x.provincia == provincia).FirstOrDefault();
            if(Provincia != null) 
                return Provincia.idprovincia;
           else  throw new ArgumentException( $" No existe la provincia {provincia}.");
            
        }
        public string ObtenerProvincia(int ProvinciaId)
        {
            var Provincia =  _context.Provincias.Where(x=>x.idprovincia == ProvinciaId).FirstOrDefault();
            return Provincia.provincia;
           
            
        }
        public async Task<IEnumerable<GetOrdenTransporte>> Listar_OrdensTransporte(string remitente_id, int? estado_id
        , int usuario_id,string fec_ini,string fec_fin, int? tiposervicio_id) 
        {
            var filtrado = new List<GetOrdenTransporte>();

            var result = await _repo_Seguimiento.GetAllOrdenTransporte( remitente_id,  estado_id, usuario_id, fec_ini, fec_fin, tiposervicio_id);
            
            var Param = new ListarRolesPorUsuarioParameter 
            {
               UserId  = usuario_id
            };

            var roles =  (ListarRolesPorUsuarioResult)  _hanlder_RolUser.Execute(Param);

            if(roles.Hits.Where(x => x.RolID == 10).SingleOrDefault()   != null)
            {
                  filtrado.AddRange( result.Where(x => x.tiposervicio_id == 164).ToList());
            }
            else if(roles.Hits.Where(x => x.RolID == 11).SingleOrDefault()   != null)
            {
                filtrado.AddRange( result.Where(x => x.tiposervicio_id == 165).ToList());
            }
            else if(roles.Hits.Where(x => x.RolID == 13).SingleOrDefault()   != null)
            {
                filtrado.AddRange( result.Where(x => x.tiposervicio_id == 166).ToList());
            }
            else if(roles.Hits.Where(x => x.RolID == 14).SingleOrDefault()   != null)
            {
                filtrado.AddRange( result.Where(x => x.tiposervicio_id == 169).ToList());
            }
            else if(roles.Hits.Where(x => x.RolID == 15).SingleOrDefault()   != null)
            {
                filtrado.AddRange( result.Where(x => x.tiposervicio_id == 170).ToList());
            }
            else 
            {
                return result;
            }

            return filtrado;
            
            
        }

        // internal async Task<IEnumerable<GetManifiestoResult>> GetAllManifiestos(string ids, int idusuario, string inicio, string fin)
        // {
        //     return await _repo_Seguimiento.GetAllManifiesto(ids, idusuario, inicio, fin);
        // }

        public async Task<IEnumerable<GetOrdenTransporte>> BuscarOrdensTransporte(string criterio, int dias) 
        {
            return await _repo_Seguimiento.SearchOrdenTransporte( criterio, dias);
        }

        public async Task<ObtenerOrdenTransporteDto> Obtener_OrdenTransporte(long orden_id){

            return await _repo_Seguimiento.ObtenerOrdenTrasporte(orden_id);

        }
          public async Task<IEnumerable<GetReporteMargenDto>> GetListarReporteMargen(int anio , int mes){

            return await _repo_Seguimiento.GetListarReporteMargen(anio, mes);

        }

        public async Task<IEnumerable<GetOrdenTransporte>> Listar_OrdensTransporte(long manifiestoId) 
        {
            var  ordenes =  await _repo_Seguimiento.GetAllOrdenTransporte(manifiestoId);
            foreach (var item in ordenes)
            {
               switch (item.estado_id)
               {
                    case 5:
                         item.button = "En espera de carga";
                         break;
                    // case 6:
                    //      item.button = "Validar despacho";
                    //      break;
                    case 6:
                       if(item.recojo == true)
                       {
                            item.button = "Fin de Recojo";
                       }
                       else 
                        { 
                             item.button = "Iniciar ruta";
                        }

                    break;            
                     case 8:
                         item.button = "Llegada a destino";

                    break;                            
                     case 9:
                         item.button = "Inicio de descarga";

                    break;       
                      case 10:
                         item.button = "Término de descarga";

                    break;  
                     case 11:
                         item.button = "Entregar";
                     

                    break;                         
                    case 36:
                        item.button = "Iniciar Ruta";
                    break;
                    case 40:
                        item.button = "Desembarcar";
                    break;

                   default:
                        break;
               }
            }
            return ordenes;
        }
        public async Task<IEnumerable<GetOrdenTransporte>> Listar_OrdensTransportCliente(int manifiestoId, int usuario_id) 
        {
            var  ordenes =  await _repo_Seguimiento.GetAllOrdenTransporteCliente(manifiestoId, usuario_id);
            foreach (var item in ordenes)
            {
               switch (item.estado_id)
               {
                   case 5:
                         item.button = "En espera de carga";

                    break;

                    case 6:
                         item.button = "Validar despacho";
                         break;
                    case 7:
                       if(item.tiposervicio_id == 163)
                       {
                            item.button = "Fin de Recojo";
                    }
                       else 
                        { 
                             item.button = "Iniciar ruta";
                        }

                    break;            
                     case 8:
                         item.button = "Llegada a destino";

                    break;                            
                     case 9:
                         item.button = "Inicio de descarga";

                    break;       
                      case 10:
                         item.button = "Término de descarga";

                    break;  
                     case 11:
                         item.button = "Entregar";
                     

                    break;                         
                    case 36:
                        item.button = "Iniciar Ruta";
                    break;
                    case 40:
                        item.button = "Desembarcar";
                    break;

                   default:
                        break;
               }
            }
            return ordenes;
        }
        public async Task<IEnumerable<GetIncidencia>> Listar_Incidencias(long OrdenTransporteId) 
        {
                return await _repo_Seguimiento.GetAllOrdenIncidencias(OrdenTransporteId);
        }

        public async Task<IEnumerable<GetMaestroIncidencia>> GetMaestroIncidencias() 
        {
                return await _repo_Seguimiento.GetMaestroIncidencias();
        }
        public async Task<GetDatosIncidencia> GetDatosIncidencia(long incidencia)
        {
                return await _repo_Seguimiento.GetDatosIncidencia(incidencia);
        }
        // public async Task<IEnumerable<GetManifiesto>> Listar_Manifiesto(int ChoferId) 
        // {
        //         return await _repo_Seguimiento.GetAllManifiesto(ChoferId);
        // }
        public async Task<IEnumerable<GetEstadisticas>> GetEstadisticas(int cliente_id) 
        {
                return await _repo_Seguimiento.GetEstadisticas(cliente_id);
        }
        // public async Task<IEnumerable<GetManifiesto>> Listar_Manifiesto_Cliente(int UsuarioId) 
        // {
                
        //         var usuario = await _repo_User.Get(x=>x.Id == UsuarioId);
        //         if (usuario.ClientesIds.Length == 0) new ArgumentException("No tiene clientes asignados");
        //         string[] prm = usuario.ClientesIds.Split(',');

        //         int ClienteId =  Convert.ToInt32(prm[0]);
        //         return await _repo_Seguimiento.GetAllManifiestoCliente( ClienteId);
        // }
        public async Task<long> Registrar_EquipoTransporte(string placa_carreta, string placa_tracto, string dni)
        {
             var param = new EquipoTransporte();
               
             var carreta = await _repo_Vehiculo.Get(x=>x.Placa ==  placa_carreta);
             var tracto = await _repo_Vehiculo.Get(x=>x.Placa ==  placa_tracto);

               
              
              var chofer = await _repo_Chofer.Get(x=>x.Dni == dni);
    
              param.ProveedorId = 1;
              param.VehiculoId = tracto.Id;
              param.ChoferId = chofer.Id.Value;
              param.EstadoId = (int) Constantes.EstadoEquipoTransporte.EnProceso;
              if(carreta != null)
              param.CarretaId = carreta.Id;
              

             var createdEquipoTransporte = await _repository.RegisterEquipoTransporte(param,null);
             return createdEquipoTransporte.Id;
        }

        public List<Manifiesto> ObtenerEntidades_Manifiesto(List<CargaMasivaDetalle> detalles_cargados )
        {
                Manifiesto manifiesto ;
                List<Manifiesto>  manifiestos = new List<Manifiesto>();
                var primer_caso = detalles_cargados.GroupBy(x=>x.conductor).ToList();

                foreach (var vehiculo_cargado in primer_caso)
                {
                    manifiesto = new Manifiesto();
                    manifiesto.fecha_registro = DateTime.Now;
                    manifiesto.numero_manifiesto = "";
                    manifiesto.usuario_id = 1;
                    manifiesto.estiba = detalles_cargados[0].estiba;
                    manifiesto.Ordenes = new  List<OrdenTransporte>();

                    var preots = vehiculo_cargado.GroupBy(x=>x.direccion_entrega.Trim()).ToList();
                    var cargados = new List<OrdenTransporte>();
                    // Manifiesto manifiesto 
                    OrdenTransporte orden;

                     var ordenes = new List<OrdenTransporte>();
                     foreach (var shipments in preots)
                     {
                         var ots = shipments.GroupBy(x=>x.shipment).ToList();
                         foreach (var ot in ots)
                         {
                                var distrito_carga_id =   ObtenerDistritoId(ot.First().distrito_carga.Trim());
                                var distrito_destino_servicio_id =  ObtenerDistritoId(ot.First().distrito_destino_servicio.Trim());
                                var provincia_id =   ObtenerProvinciaId(ot.First().provincia.Trim());

                                var remitente_id =   ObtenerClienteId(ot.First().remitente.Trim());

                                orden = new OrdenTransporte();
                                orden.shipment = ot.First().shipment;
                                orden.cantidad = ot.Sum(x=>x.cantidad);
                                
                                orden.delivery = string.Empty;
                                orden.guias = string.Empty;

                                foreach (var item in ot)
                                {
                                   if(orden.delivery.Contains(item.delivery)) continue;
                                   orden.delivery = orden.delivery +", " +  item.delivery;
                                }

                                foreach (var item in ot)
                                {
                                    if(orden.guias != ""){
                                        if(orden.guias.Contains(item.guias)) continue;
                                        orden.guias = orden.guias +", " +  item.guias;
                                     }
                                }

                                if(orden.delivery.Length>0)
                                orden.delivery =  orden.delivery.Substring(1, orden.delivery.Length -1 ).Trim();

                                if(orden.guias.Length>0)
                                orden.guias =  orden.guias.Substring(1, orden.guias.Length -1 ).Trim();

                                orden.remitente_id = remitente_id;
                                orden.destinatario_id = null; // destinatario_id;

                                orden.destinatario = ot.First().destinatario.Trim();

                                orden.direccion_carga = ot.First().direccion_carga;
                                orden.direccion_destino_servicio = ot.First().direccion_destino_servicio;
                                orden.direccion_entrega = ot.First().direccion_entrega;


                                orden.distrito_carga_id = distrito_carga_id;
                                orden.distrito_destino_servicio_id = distrito_destino_servicio_id;

                        

                                orden.equipo_transporte_id = GetEquipoTransporte(ot.First().carreta, ot.First().tracto, ot.First().conductor).Result;


                                orden.factura = ot.First().factura;

                                orden.fecha_carga = ot.First().fecha_carga;
                                orden.hora_carga = ot.First().hora_carga;


                                orden.fecha_entrega = ot.First().fecha_entrega;
                                orden.hora_entrega = ot.First().hora_entrega;


                                orden.fecha_salida = ot.First().fecha_salida;
                                orden.hora_salida = ot.First().hora_salida;

                                 orden.recojo =  ot.First().recojo;
                                                                


                                orden.oc = ot.First().oc;
                                orden.peso = ot.Sum(x=>x.peso)  ;

                                if( orden.equipo_transporte_id == null){
                                        orden.por_asignar = true;
                                        orden.estado_id = (int) Constantes.EstadoOrdenTransporte.PendienteProgramacion;
                                }
                                else {
                                    orden.por_asignar = false;
                                    orden.estado_id = (int) Constantes.EstadoOrdenTransporte.Programado;
                                }
                                
                                orden.provincia_entrega = provincia_id;
                                orden.numero_lancha  =  ot.First().tiposervicio.ToString();
                               // String compare = "DISTRIBUCION LOCAL";
                                
                                // orden.tiposervicio_id = 
                                // (ot.First().tiposervicio.ToUpper() == ("ENTREGA DIRECTA").ToString().ToUpper() ? 162: 163);
                                

                                if(orden.numero_lancha.Length == ("DISTRIBUCION LOCAL").Length)
                                {
                                     orden.tiposervicio_id = 164;
                                }
                                else if ( orden.numero_lancha.Length == "PROVINCIA".ToString().ToUpper().Length)
                                {
                                    orden.tiposervicio_id = 165;
                                }
                                else if ( orden.numero_lancha.Length == "ULTIMA MILLA".ToString().ToUpper().Length)
                                {
                                     orden.tiposervicio_id = 166;
                                }
                                else if ( orden.numero_lancha.Length == "AASS".ToString().ToUpper().Length)
                                {
                                    orden.tiposervicio_id = 169;
                                }
                                else if ( orden.numero_lancha.Length == "VET".ToString().ToUpper().Length)
                                {
                                    orden.tiposervicio_id = 170;
                                }
                                else
                                { 
                                    orden.tiposervicio_id = 164;
                                }
                                orden.numero_lancha = null;
                                //  orden.tiposervicio_id = 
                                // (ot.First().tiposervicio.ToUpper() == ("ENTREGA DIRECTA").ToString().ToUpper() ? 162: 163);
                                
                                orden.volumen = ot.Sum(x=>x.volumen);
                                orden.notificacion = ot.First().notificacion;
                                orden.costo = ot.Sum(x=>x.costo);
                                orden.valorizado =  ot.Sum(x=>x.valorizado);

                                manifiesto.Ordenes.Add(orden);
                                cargados.Add(orden);   
                            }
                        }
                    
                        manifiestos.Add(manifiesto);            
                }
                 return manifiestos;
            
            
               
        }
         public async Task<List<Manifiesto>> ObtenerEntidades_Manifiesto_autorex(List<CargaMasivaDetalle> detalles_cargados )
        {
                Manifiesto manifiesto ;
                List<Manifiesto>  manifiestos = new List<Manifiesto>();
                var primer_caso = detalles_cargados.GroupBy(x=>x.tracto).ToList();
                OrdenTransporte orden ;

                foreach (var vehiculo_cargado in primer_caso)
                {
                    manifiesto = new Manifiesto();
                    manifiesto.fecha_registro = DateTime.Now;
                    manifiesto.numero_manifiesto = "";
                    manifiesto.usuario_id = 1;
                    manifiesto.estiba = detalles_cargados[0].estiba;
                    manifiesto.Ordenes = new  List<OrdenTransporte>();

                    var cargados = new List<OrdenTransporte>();
                    var getdato = vehiculo_cargado.ToList()[0];

                    var equipoid  = await GetEquipoTransporte(getdato.carreta, getdato.tracto, getdato.conductor);
                   
                         //var ots = shipments.GroupBy(x=>x.shipment).ToList();
                         foreach (var ot in vehiculo_cargado)
                         {
                                var distrito_carga_id =   ObtenerDistritoId("Lima");
                                var distrito_destino_servicio_id =  ObtenerDistritoId("Lima");
                                var provincia_id =   ObtenerProvinciaId(ot.provincia.Trim());

                                var remitente_id =   ObtenerClienteId(ot.remitente.Trim());

                                orden = new OrdenTransporte();
                                orden.shipment = ot.shipment;
                                orden.cantidad = ot.cantidad;
                                
                                orden.delivery = string.Empty;
                                orden.guias = string.Empty;

                              
                                orden.delivery = ot.delivery ;
                                orden.guias = ot.guias;

                                orden.delivery =  orden.delivery;

                                if(orden.guias != null)
                                if(orden.guias.Length>0 )
                                orden.guias =  orden.guias;

                                orden.remitente_id = remitente_id;
                                orden.destinatario_id = null; // destinatario_id;

                                orden.destinatario = ot.destinatario.Trim();

                                orden.direccion_carga = ot.direccion_carga;
                                orden.direccion_destino_servicio = ot.direccion_destino_servicio;
                                orden.direccion_entrega = ot.direccion_entrega;


                                orden.distrito_carga_id = distrito_carga_id;
                                orden.distrito_destino_servicio_id = distrito_destino_servicio_id;

                        

                                
                                orden.equipo_transporte_id = equipoid;

                                orden.factura = ot.factura;

                                orden.fecha_carga = ot.fecha_carga;
                                orden.hora_carga = ot.hora_carga;


                                orden.fecha_entrega = ot.fecha_entrega;
                                orden.hora_entrega = ot.hora_entrega;


                                orden.fecha_salida = ot.fecha_salida;
                                orden.hora_salida = ot.hora_salida;

                                 orden.recojo =  ot.recojo;
                                                                


                                orden.oc = ot.oc;
                                orden.peso = ot.peso  ;

                                if( orden.equipo_transporte_id == null){
                                        orden.por_asignar = true;
                                        orden.estado_id = (int) Constantes.EstadoOrdenTransporte.PendienteProgramacion;
                                }
                                else {
                                    orden.por_asignar = false;
                                    orden.estado_id = (int) Constantes.EstadoOrdenTransporte.Programado;
                                }
                                
                                orden.provincia_entrega = provincia_id;
                                orden.numero_lancha  =  ot.tiposervicio.ToString();
                               // String compare = "DISTRIBUCION LOCAL";
                                
                                // orden.tiposervicio_id = 
                                // (ot.First().tiposervicio.ToUpper() == ("ENTREGA DIRECTA").ToString().ToUpper() ? 162: 163);
                                

                                if(orden.numero_lancha.Length == ("DISTRIBUCION LOCAL").Length)
                                {
                                     orden.tiposervicio_id = 164;
                                }
                                else if ( orden.numero_lancha.Length == "PROVINCIA".ToString().ToUpper().Length)
                                {
                                    orden.tiposervicio_id = 165;
                                }
                                else if ( orden.numero_lancha.Length == "ULTIMA MILLA".ToString().ToUpper().Length)
                                {
                                     orden.tiposervicio_id = 166;
                                }
                                else if ( orden.numero_lancha.Length == "AASS".ToString().ToUpper().Length)
                                {
                                    orden.tiposervicio_id = 169;
                                }
                                else if ( orden.numero_lancha.Length == "VET".ToString().ToUpper().Length)
                                {
                                    orden.tiposervicio_id = 170;
                                }
                                else
                                { 
                                    orden.tiposervicio_id = 164;
                                }
                               
                                orden.numero_lancha = null;
                                orden.volumen = ot.volumen;
                                orden.notificacion = ot.notificacion;
                                orden.costo = ot.costo;
                                orden.valorizado =  ot.valorizado;

                                manifiesto.Ordenes.Add(orden);
                                cargados.Add(orden);   
                            }
                        
                    
                        manifiestos.Add(manifiesto);            
                }
                 return manifiestos;
            
            
               
        }
        
        public List<CargaMasivaDetalleForRegister> ObtenerEntidades_CargaMasiva(List<List<String>> data) 
        {
                data = validar_fin(data);
                var totales = new List<CargaMasivaDetalleForRegister>();
                CargaMasivaDetalleForRegister linea ;

                foreach (var item in data.Skip(1))
                {
                    linea =  new CargaMasivaDetalleForRegister();
                    linea.shipment = ValidarRequerido(item[0] , "Shipment" );
                    linea.delivery = item[1];
                    linea.remitente = ValidarRequerido(item[2], "Remitente");
                    linea.destinatario =ValidarRequerido( item[3] , "Destinatario") ;
                    linea.factura = item[4];
                    linea.oc = item[5];
                    linea.guias = item[6];

                    if(item[7] != "")
                    linea.cantidad = int.Parse(item[7]);

                    if(item[8] != "")
                    linea.volumen = Decimal.Parse(ValidarRequerido(item[8].Trim(),"Volumen"), System.Globalization.NumberStyles.Float);

                    if(item[9] != null)
                    linea.peso  = Decimal.Parse(ValidarRequerido(item[9].Trim(), "Peso"), System.Globalization.NumberStyles.Float);


                    linea.tiposervicio = ValidarRequerido(item[10],"Tipo de Servicio");

                    linea.distrito_carga = ValidarRequerido(item[11],"Distrito de carga");
                    linea.direccion_carga = ValidarRequerido(item[12], "Dirección de carga");
                    linea.fecha_carga =DateTime.FromOADate( double.Parse(ValidarRequerido(item[13],"Fecha de carga")));
                    linea.hora_carga = DateTime.FromOADate( double.Parse(ValidarRequerido(item[14],"Hora de carga"))).TimeOfDay.ToString(@"hh\:mm\:ss");

                    linea.direccion_destino_servicio = item[15];
                    linea.distrito_destino_servicio = item[16];
                    linea.fecha_salida =  DateTime.FromOADate( double.Parse(ValidarRequerido(item[17],"Fecha salida")));
                    linea.hora_salida = DateTime.FromOADate( double.Parse(ValidarRequerido(item[18], "Hora salida"))).TimeOfDay.ToString(@"hh\:mm\:ss");
                    linea.direccion_entrega = ValidarRequerido(item[19], "Dirección de entrega");
                    linea.provincia = ValidarRequerido(item[20], "Provincia de entrega");
                    linea.fecha_entrega =  DateTime.FromOADate( double.Parse(ValidarRequerido(item[21],"Fecha de entrega")));
                    linea.hora_entrega = DateTime.FromOADate( double.Parse(ValidarRequerido(item[22],"Hora de entrega"))).TimeOfDay.ToString(@"hh\:mm\:ss");
                    linea.tracto = ValidarRequerido(item[23],"Tracto");
                    linea.carreta = (item[24]=="Sin Data"?"":item[24]);
                    linea.conductor = ValidarRequerido(item[25],"Conductor");
                    linea.recojo = ValidarRequerido(item[26],"Recojo");
                    if(item.Count > 27)
                        linea.notificacion = item[27];
                    if(item.Count > 28)
                        linea.costo = decimal.Parse(item[28]);
                    if(item.Count > 29)
                        linea.valorizado = decimal.Parse(item[29]);
                    if(item.Count > 30)
                        linea.estiba = decimal.Parse(item[30]);
                
                    totales.Add(linea);
                    
                }
                return totales;
        }

        private List<List<string>> validar_fin(List<List<string>> data)
        {
            List<List<string>> new_data = new List<List<string>>();
            foreach (var item in data)
            {
                if(item[0] == "" && item[2] == ""){
                    break;
                }
                else 
                new_data.Add(item);
                
            }
            return new_data;
        }

        private string ValidarRequerido(string v, string field)
        {
            if(String.IsNullOrEmpty(v))
            {
              throw new ArgumentException( $" {field} no puede estar en blanco .");
            }
            return v;
        }

        public List<List<string>> GetExcel(string fullPath)
        {     
             List<List<string>> valores = new List<List<string>>();
            try
            {
                
                using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fullPath, false))
                {
                    WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                    IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    string relationshipId = sheets.First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                    Worksheet workSheet = worksheetPart.Worksheet;
                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                    IEnumerable<Row> rows = sheetData.Descendants<Row>();
                    // foreach (Cell cell in rows.ElementAt(0))
                    // {
                    //     dt.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                    // }
                    foreach (Row row in rows) //this will also include your header row...
                    {
                         List<String> linea = new List<string>();
                        int columnIndex = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            // Gets the column index of the cell with data
                            int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(cell.CellReference));
                            cellColumnIndex--; //zero based index
                            if (columnIndex < cellColumnIndex)
                            {
                                do
                                {
                                    linea.Add(""); //Insert blank data here;
                                    columnIndex++;
                                }
                                while (columnIndex < cellColumnIndex);
                            }
                             linea.Add(GetCellValue(spreadSheetDocument, cell));
                          
                            columnIndex++;
                        }
                        valores.Add(linea);
                    }
                }
               // dt.Rows.RemoveAt(0); //...so i'm taking it out here.
                     return valores;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }
      
        public static int? GetColumnIndexFromName(string columnName)
        {
                       
            //return columnIndex;
            string name = columnName;
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }
        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            if (cell.CellValue ==null)
            {
            return "";
            }
            string value = cell.CellValue.InnerXml;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }
        // public List<List<string>> GetExcel(string fullPath)
        // {
        


        //     List<List<string>> valores = new List<List<string>>();
        //     using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fullPath, false))
        //     {
        //             WorkbookPart workbookPart = doc.WorkbookPart;
        //             Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();
                    
        //             StringBuilder excelResult = new StringBuilder();
        //                 //using for each loop to get the sheet from the sheetcollection  
        //             foreach (Sheet thesheet in thesheetcollection)
        //             {
        //                 WorksheetPart theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id));

        //                   Cell theCell = theWorksheet.Worksheet.Descendants<Cell>().
        //                       Where(c => c.CellReference == "O2").FirstOrDefault();
                        
        //                 SheetData thesheetdata = (SheetData)theWorksheet.Worksheet.GetFirstChild<SheetData>();
        //                 foreach (Row thecurrentrow in thesheetdata)
        //                 {
        //                     List<String> linea = new List<string>();
        //                     int inicio = 0;
        //                     foreach (Cell thecurrentcell in thecurrentrow)
        //                     {
                                
        //                         //statement to take the integer value  
        //                         string currentcellvalue = string.Empty;
        //                         if (thecurrentcell.DataType != null)
        //                         {
        //                             if (thecurrentcell.DataType == CellValues.SharedString)
        //                             {
        //                                 int id;
        //                                 if (Int32.TryParse(thecurrentcell.InnerText, out id))
        //                                 {
        //                                     var item = 
        //                                     workbookPart.SharedStringTablePart.SharedStringTable
        //                                     .Elements<SharedStringItem>().ElementAt(id);

        //                                     if (item.Text != null)
        //                                     {
                                                
        //                                         excelResult.Append(item.Text.Text + " ");
        //                                         linea.Add(item.Text.Text);
        //                                     }
        //                                     else if (item.InnerText != null)
        //                                     {
        //                                         currentcellvalue = item.InnerText;
        //                                     }
        //                                     else if (item.InnerXml != null)
        //                                     {
        //                                         currentcellvalue = item.InnerXml;
        //                                     }
                                            
        //                                 }
                                        
        //                             }
        //                         }
        //                         else
        //                         {   
        //                             excelResult.Append(thecurrentcell.InnerText + " ");
        //                             linea.Add(thecurrentcell.InnerText);
        //                         }
        //                         inicio ++;
        //                     }
        //                     valores.Add(linea);
        //                 }
        //             }
        //             return valores;
        //         }
        // }
        public async Task<long> RegisterIncidencia(IncidenciaForRegister incidencia)
        {
            var param = _mapper.Map<IncidenciaForRegister, Incidencia>(incidencia);
            var createdIncidencia = await _repo_Incidencia.AddAsync(param);
            await _repo_Incidencia.SaveAll();
            return createdIncidencia.id;
        }
        public async Task<IEnumerable<GetActivityTotal>> GetActivityTotal()
        {
              return  await _repo_Seguimiento.GetActivityTotal();
        }
        public async Task<IEnumerable<GetActivityTotal>> GetActivityTotalRecojo()
        {
              return  await _repo_Seguimiento.GetActivityTotalRecojo();
        }
        public async Task<IEnumerable<GetActivityTotal>> GetActivityTotalClientes()
        {
              return  await _repo_Seguimiento.GetActivityTotalClientes();
        }
        
        public async Task<IEnumerable<GetActivityVehiculoRuta>> GetActivityVehiculosRuta()
        {
              return  await _repo_Seguimiento.GetActivityVehiculosRuta();
        }
         public async Task<IEnumerable<GetActivityOTsTotalesYEntregadas>> GetActivityOTTotalesYEntregadas()
        {
              return  await _repo_Seguimiento.GetActivityOTTotalesYEntregadas();
        }
        public async Task<IEnumerable<ReporteServicioResult>> GetReporteServicio()
        {
              return  await _repo_Seguimiento.GetReporteServicio();
        }
        
        public async Task<IEnumerable<GetDocumentoResult>> GetAllDocuments(int id)
        {
                return  await _repo_Seguimiento.GetAllDocumentos(id);
        }
        public async Task<GetLocalizacionResult> GetLocalizacion(int id)
        {
               return  await _repo_Seguimiento.GetLocalizacion(id);
        }  
        public async Task<IEnumerable<GetLocalizacionResult>> GetLiveView()
        {
               return  await _repo_Seguimiento.GetLiveView();
        }  
        public async Task<bool> DeleteProveedor(int id)
        {
              var proveedor = await _repo_Proveedor.Get(x=>x.Id == id);
             try
             {

                _repo_Proveedor.Delete(proveedor);   

             }
             catch (System.Exception)
             {
                 
                 throw new ArgumentException("El proveedor esta siendo usado.");
             } 
              

            return true; 
        }

        public async Task<bool> ActualizarIncidencia(IncidenciaForUpdate incidencia)
        {
            try
            {
              await  _repository.ActualizarIncidencia(incidencia);
            }
            catch (System.Exception)
            {
                 throw new ArgumentException("Error al actualizar la incidencia.");
            } 
            return true; 
        }

        public string GetBodyMail(string id,string remitente,string fecha_registro, string direccion_entrega,
        string provincia, string numero_ot, string chofer, string dni, string placa ,string lat_entrega , string lng_entrega )   {


           return  @"<html>
                        <head>
               
                </head>
                <body>
                <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-8533450555590531398title'><tbody><tr style='height:18.75pt'><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:18.75pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal' align='center' style='text-align:center'><strong><span style='font-size:16.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414;letter-spacing:-.4pt'>Tu pedido ha llegado a su destino </span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:21.0pt'><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;padding:0cm 0cm 0cm 0cm;height:21.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-8533450555590531398content'><tbody><tr><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><div align='center'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse' id='m_-8533450555590531398inner-content'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>

                <span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                    Estimados: "+ remitente + @" 
                </span>
                
                <span style='font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr style='height:14.25pt'><td style='padding:0cm 0cm 0cm 0cm;height:14.25pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal' style='text-align:justify'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                <br>La unidad a cargo de la entrega de tu pedido ha llegado a su destino en la dirección de despacho. <u></u><u></u></span></p></td></tr><tr style='height:14.25pt'><td style='padding:0cm 0cm 0cm 0cm;height:14.25pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr></tbody></table></td></tr></tbody></table></td><td width='20' style='width:15.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;border-radius:3px'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'><td style='background:#db0414;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'> <a href='http://104.36.166.65/toscanos/#/seguimiento/verorden/"+ id + @"'><span style='font-size:11.5pt;color:white;text-decoration:none'>Ver detalle de mi pedido</span></a> </span><span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:10.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:10.5pt'></td></tr></tbody></table></td></tr></tbody></table></div></td><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>

                <table border='0' cellspacing='0' cellpadding='0' width='100%' 
                style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'>
                <td style='background:#db0414;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' 
                align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'>
                
                
                    <span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table>
                <p>   
                </body>
                <table border='0' cellspacing='0' cellpadding='0' width='600' style='width:450.0pt;border-collapse:collapse' id='m_-1387954621597574012order-details'><tbody><tr style='height:15.0pt'><td width='25' style='width:18.75pt;border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><div align='center'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse' id='m_-1387954621597574012inner-content'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414'>Datos del envío:</span></strong><span style='font-family:&quot;Helvetica&quot;,sans-serif'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong>
                <span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Fecha de Pedido:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>
                <tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'>
                <span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>

                " +  fecha_registro + @"
                
                <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Fecha de Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                "+ DateTime.Now.ToShortDateString() +@"
                <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Domicilio de Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                "+ direccion_entrega + @"
                
                <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Número de Teléfono:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>
                <tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td width='170' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Distrito:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                "+ provincia + @"
                
                <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr><tr style='height:12.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:12.0pt;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                # Orden de Transporte </span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'><u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                "+  numero_ot  + @" 
                <u></u><u></u></span></p></td></tr><tr style='height:30.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:30.0pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Nro. Entrega:</span></strong><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'><u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' style='word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td></tr>

                <tr style='height:30.0pt'><td style='padding:0cm 0cm 0cm 0cm;height:30.0pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td><td width='9' style='width:6.75pt;border:none;border-right:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'>&nbsp;<u></u><u></u></p></td></tr></tbody></table></td></tr></tbody></table></td><td width='10' style='width:7.5pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:11.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#db0414'>Información de Chofer:</span></strong><span style='font-family:&quot;Helvetica&quot;,sans-serif'> <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-size:1.0pt'><img border='0' height='95' style='height:.9895in' id='m_-1387954621597574012_x0000_i1026' src='https://ci3.googleusercontent.com/proxy/Md5He3hPe9rJeJALU5ArtgdTYUhgRbRT82SpKo3pAwsTu13eV1QAzy4TPKwlF0qXMShaSEpjxx_5YBxs_B1wWxOVIUhyGRMq4U6hSKtL4-dw5Rx6nUxvWJv5YjjtlzNB-Rc-y2JjHA=s0-d-e1-ft#http://toscargo.e-strategit.com/Public/Imagenes/Chofer/fotochofer_06082019050819.png' alt='Foto conductor' class='CToWUd'><u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Nombre:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:8.5pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                "+   chofer   +@"
                    <u></u><u></u></span></p></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>DNI:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                    "+ dni +@"
                    <u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>Placa:</span></strong><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'> <u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>
                    " + placa +@"
                    <u></u><u></u></span></p></td></tr></tbody></table></td></tr>
                <tr><td style='padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td valign='top' style='padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td><td width='105' style='width:78.75pt;padding:0cm 0cm 0cm 0cm;word-wrap:break-word'><p class='MsoNormal' align='right' style='text-align:right;word-break:break-all'><span style='font-size:10.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#666666'>&nbsp;<u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:7.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:7.5pt'><p class='MsoNormal'><span style='font-size:1.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table></td></tr></tbody></table></div></td><td width='25' style='width:18.75pt;border:none;border-top:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr><tr style='height:15.0pt'><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td style='border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm;height:15.0pt'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                       <img src='http://maps.googleapis.com/maps/api/staticmap?center=  
                       " + lat_entrega + "," +  lng_entrega  + @"&size=1000x800&maptype=roadmap&scale=2&format=png&markers=color:blue%7Clabel:S%7C
                        " + lat_entrega  + "," + lng_entrega + @"&zoom=17&key=AIzaSyDnh35oUHQYGDPcVs6rfKOY057Xo7ujDsQ' width='500' height='500'/>
                       <p>  

                       
                        <h2>Encuesta Post-Venta</h2>
                        <br>Califique nuestro servicio:  <u></u><u></u></span></p></td></tr><tr style='height:14.25pt'><td style='padding:0cm 0cm 0cm 0cm;height:14.25pt'><p class='MsoNormal'>&nbsp; <u></u><u></u></p></td></tr></tbody></table></td></tr></tbody></table></td><td width='20' style='width:15.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;border-radius:3px'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'><td style='background:#13ba37;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'> <a href='http://104.36.166.65/toscanos/#/seguimiento/encuestasatisfaccion/"+ id +  "/" + 1 + @"'><span style='font-size:11.5pt;color:white;text-decoration:none'>Muy bueno</span></a> </span><span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:10.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:10.5pt'></td></tr></tbody></table></td>
                        <td width='20' style='width:15.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;border-radius:3px'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'><td style='background:#e1e85f;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'> <a href='http://104.36.166.65/toscanos/#/seguimiento/encuestasatisfaccion/"+ id + "/" + 2 +  @" '><span style='font-size:11.5pt;color:white;text-decoration:none'>Bueno </span></a> </span><span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:10.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:10.5pt'></td></tr></tbody></table></td>
                         <td width='20' style='width:15.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;border-radius:3px'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'><td style='background:#eaf5cb;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'> <a href='http://104.36.166.65/toscanos/#/seguimiento/encuestasatisfaccion/"+ id +  "/" + 3 + @"'><span style='font-size:11.5pt;color:black;text-decoration:none'>Regular </span></a> </span><span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:10.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:10.5pt'></td></tr></tbody></table></td>
                         <td width='20' style='width:15.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;border-radius:3px'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'><td style='background:#eaf5cb;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'> <a href='http://104.36.166.65/toscanos/#/seguimiento/encuestasatisfaccion/"+ id +  "/" + 4 + @"'><span style='font-size:11.5pt;color:black;text-decoration:none'>Malo </span></a> </span><span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:10.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:10.5pt'></td></tr></tbody></table></td>
                        <td width='20' style='width:15.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td><td width='170' valign='top' style='width:127.5pt;padding:0cm 0cm 0cm 0cm'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr><td style='padding:0cm 0cm 0cm 0cm'></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm;border-radius:3px'><table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100.0%;border-collapse:collapse'><tbody><tr style='height:22.5pt'><td style='background:#db0414;padding:0cm 0cm 0cm 0cm;height:22.5pt'><p class='MsoNormal' align='center' style='text-align:center'><span style='font-family:&quot;Helvetica&quot;,sans-serif;color:black'> <a href='http://104.36.166.65/toscanos/#/seguimiento/encuestasatisfaccion/"+ id +  "/" + 5 +@"'><span style='font-size:11.5pt;color:white;text-decoration:none'>Muy malo </span></a> </span><span style='font-family:&quot;Helvetica&quot;,sans-serif'><u></u><u></u></span></p></td></tr></tbody></table></td></tr><tr style='height:10.5pt'><td style='padding:0cm 0cm 0cm 0cm;height:10.5pt'></td></tr></tbody></table></td></tr></tbody></table></div></td><td width='25' style='width:18.75pt;border:none;border-bottom:solid #f2f2f2 1.0pt;padding:0cm 0cm 0cm 0cm'><p class='MsoNormal'><span style='font-size:10.0pt'>&nbsp; <u></u><u></u></span></p></td></tr></tbody></table>
                              
                      </body>
                </html>";   
        } 
        public async Task<IEnumerable<GetSustentoResult>> GetAllSustento(GetSustentoResult sustento) 
        {
                return await _repo_Seguimiento.GetAllSustento(sustento);
        }
         public async Task<IEnumerable<GetReporteServicioResult>> GetReporteServicio_unidadesasignadas(string idclientes) 
        {
                return await _repo_Seguimiento.GetReporteServicio_unidadesasignadas(idclientes);
        }
         public async Task<IEnumerable<GetReporteServicioResult>> GetReporteServicio_valorizacion(string idclientes) 
        {
                return await _repo_Seguimiento.GetReporteServicio_valorizacion(idclientes);
        }

        public async Task<IEnumerable<GetManifiestoPendienteSustentar>> GetManifiestoPendienteSustentar(string documento) 
        {
                return await _repo_Seguimiento.GetManifiestoPendienteSustentar(documento);
        }

        public async Task<IEnumerable<GetTipoDocumentoSustento>> GetTipoDocumentoSustento() 
        {
                return await _repo_Seguimiento.GetTipoDocumentoSustento();
        }

        public async Task<IEnumerable<GetTipoSustento>> GetTipoSustento() 
        {
                return await _repo_Seguimiento.GetTipoSustento();
        }

        public async Task<GetSustentoManifiesto> GetSustentoManifiesto(long manifiesto) 
        {
                return await _repo_Seguimiento.GetSustentoManifiesto(manifiesto);
        }
        public async Task<IEnumerable<GetDocumentoSustento>> GetDocumentosSustento(long? sustento,int? tipo) 
        {
                return await _repo_Seguimiento.GetDocumentosSustento(sustento,tipo);
        }

        public async Task<IEnumerable<GetDocumentoSustentoPendiente>> GetDocumentosSustentoPendientesAprobacion(string docConductor)
        {
                return await _repo_Seguimiento.GetDocumentosSustentoPendientesAprobacion(docConductor);
        }

         public async Task<IEnumerable<GetUsuarioAprobacionSustento>> GetUsuarioAprobacionSustento() 
        {
                return await _repo_Seguimiento.GetUsuarioAprobacionSustento();
        }

         public async Task<GetRazonSocialSustento> GetRazonSocialSustento(string ruc) 
        {
                return await _repo_Seguimiento.GetRazonSocialSustento(ruc);
        }
        public async Task<IEnumerable<GetTipoDocumentoEmisor>> GetTiposDocumentoEmisor() 
        {
                return await _repo_Seguimiento.GetTiposDocumentoEmisor();
        }

        public async Task<GetOrdenTransporte>  GetOrdenByWayPoint(long manifiesto_id, string lat, string lng)
        {
            return await _repo_Seguimiento.GetOrdenByWayPoint(manifiesto_id, lat, lng);
        }
    }
}
