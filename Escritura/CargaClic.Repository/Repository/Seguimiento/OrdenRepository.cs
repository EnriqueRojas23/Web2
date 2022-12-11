
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CargaClic.Common;
using CargaClic.Data;
using CargaClic.Domain.Mantenimiento;
using CargaClic.Domain.Seguimiento;
using CargaClic.Repository.Contracts.Seguimiento;
using CargaClic.Repository.Interface.Seguimiento;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CargaClic.Repository.Seguimiento
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public OrdenRepository(DataContext context,IConfiguration config)
        {
            _context = context;
            _config = config;
        }

       

        public IDbConnection Connection
        {   
            get
            {
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                try
                {
                     connection.Open();
                     return connection;
                }
                catch (System.Exception)
                {
                    connection.Close();
                    connection.Dispose();
                    throw;
                }
            }
        }
 

        public async Task<int> RegisterCargaMasiva(CargaMasivaForRegister command, IEnumerable<CargaMasivaDetalleForRegister> commandDetais )
        {
            CargaMasivaDetalle cargaMasivaDetalle ;
            CargaMasiva cargaMasiva = new CargaMasiva(); 
            cargaMasiva.estado_id = 1;
            cargaMasiva.fecha_registro = DateTime.Now;
            cargaMasiva.usuario_id  = 1; 
            cargaMasiva.cantidad_total = command.cantidad_total;
            cargaMasiva.peso_total =  command.peso_total;
            cargaMasiva.usuario_id = command.usuario_id; // autorex
            cargaMasiva.oc = command.oc;
            cargaMasiva.owner = command.owner;

            List<CargaMasivaDetalle> cargaMasivaDetalles = new List<CargaMasivaDetalle>();

            using(var transaction = _context.Database.BeginTransaction())
            {

                await _context.AddAsync<CargaMasiva>(cargaMasiva);
                await _context.SaveChangesAsync();

                foreach (var item in commandDetais)
                {
                    cargaMasivaDetalle = new CargaMasivaDetalle();
                    cargaMasivaDetalle.cantidad = item.cantidad;
                    cargaMasivaDetalle.carga_id = cargaMasiva.id;
                    
                    cargaMasivaDetalle.asignado = item.asignado;
                    cargaMasivaDetalle.carreta = item.carreta;
                    cargaMasivaDetalle.conductor = item.conductor.Trim();
                    cargaMasivaDetalle.delivery = item.delivery;
                    cargaMasivaDetalle.destinatario = item.destinatario.Trim();
                    cargaMasivaDetalle.direccion_carga = item.direccion_carga.Trim();
                    cargaMasivaDetalle.direccion_destino_servicio = item.direccion_destino_servicio.ToString().Trim();
                    cargaMasivaDetalle.direccion_entrega = item.direccion_entrega.ToString().Trim();

                    cargaMasivaDetalle.distrito_carga = item.distrito_carga;
                    cargaMasivaDetalle.distrito_destino_servicio = item.distrito_destino_servicio.Trim();
                    cargaMasivaDetalle.factura = item.factura;
                    cargaMasivaDetalle.fecha_carga = item.fecha_carga;
                    cargaMasivaDetalle.fecha_entrega = item.fecha_entrega;
                    cargaMasivaDetalle.fecha_salida = item.fecha_salida;
                    cargaMasivaDetalle.hora_carga = item.hora_carga;
                    cargaMasivaDetalle.hora_entrega = item.hora_entrega;
                    cargaMasivaDetalle.hora_salida = item.hora_salida;
                    cargaMasivaDetalle.oc = item.oc;
                    cargaMasivaDetalle.peso = item.peso;
                    cargaMasivaDetalle.guias = item.guias;

                    cargaMasivaDetalle.provincia = item.provincia;
                    cargaMasivaDetalle.remitente = item.remitente;
                    cargaMasivaDetalle.shipment = item.shipment;
                    cargaMasivaDetalle.tiposervicio = item.tiposervicio;

                    cargaMasivaDetalle.tracto = item.tracto;
                    cargaMasivaDetalle.volumen = item.volumen;
                    cargaMasivaDetalle.recojo  = item.recojo == "Si" ? true : false;
                    cargaMasivaDetalle.notificacion = item.notificacion;
                    cargaMasivaDetalle.costo = item.costo;
                    cargaMasivaDetalle.valorizado = item.valorizado;
                    cargaMasivaDetalle.estiba  = item.estiba;
                    cargaMasivaDetalle.owner = item.owner;
                    if(item.errores != null)
                    {
                        cargaMasivaDetalle.detalleerror = item.errores; 
                        cargaMasivaDetalle.error = true;
                    }

                    cargaMasivaDetalles.Add(cargaMasivaDetalle);
                }

                await _context.AddRangeAsync(cargaMasivaDetalles);
                await _context.SaveChangesAsync();



                transaction.Commit();
                

                return cargaMasiva.id;
            }
        }

       public async Task<EquipoTransporte> RegisterEquipoTransporte(EquipoTransporte eq, List<long> ids)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {

                var max = await _context.EquipoTransporte.MaxAsync(x=>x.Codigo);
                if(max==null) max = "EQ00000001";
                max  = "EQ" + (Convert.ToInt64(max.Substring(2,8)) + 1).ToString().PadLeft(8,'0');
                eq.Codigo = max;

                eq.FechaRegistro = DateTime.Now;
                eq.PropietarioId = eq.PropietarioId; 

                await _context.AddAsync<EquipoTransporte>(eq);
                await _context.SaveChangesAsync();

                if(ids != null){
                    foreach (var id in ids)
                    {
                        var ordentransporteDb = await _context.OrdenTransporte.Where(x=>x.id == id).SingleOrDefaultAsync();
                        ordentransporteDb.equipo_transporte_id = eq.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                transaction.Commit();
                return eq;


            }
        }

        public async Task<bool> RegisterGeoLocalizacion(GeoEquipoTransporteForRegister geoEquipoTransporte)
        {
            var geo = new GeoEquipoTransporte();
            geo.lat = geoEquipoTransporte.lat;
            geo.lng = geoEquipoTransporte.lng;
            

        
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    
                     var user =  _context.Chofer.Where(x=>x.UsuarioId == geoEquipoTransporte.usuario_id).SingleOrDefault();

                    //agregar estado cerrado y pendiente
                    var result = from  man in  _context.Manifiesto
                    join orden in _context.OrdenTransporte  on man.id equals orden.manifiesto_id
                    join equipo in _context.EquipoTransporte on orden.equipo_transporte_id equals equipo.Id
                    where equipo.ChoferId  == user.Id
                    select equipo;


                    

                    var equipoTransporte = result.OrderByDescending(x=>x.Id).First();


                    geo.equipo_transporte_id = equipoTransporte.Id;

                //    var manifiesto = _context.Manifiesto.Where(x=>x.usuario_id == geoEquipoTransporte.usuario_id).ToList();
                //    var ordenes = _context.OrdenTransporte.Where(x=>x.manifiesto_id == manifiesto[0].id).ToList();


                    // var equipoTransporte =  _context.EquipoTransporte
                    //                                 .Where(x=>x.Id == ordenes[0].equipo_transporte_id)
                    //                                 .FirstOrDefault();

                    //geo.equipo_transporte_id = equipoTransporte.Id;

                    await _context.AddAsync(geo);     
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (System.Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
               

            }
            return true;
        }

        public async Task<int> RegisterOrdenes(List<Manifiesto> manifiestosForRegisters, int usrid)
        {
            
            OrdenTransporte ordenTransporte ;
            Incidencia incidencia;
            List<Incidencia> incidencias;
            Manifiesto manifiesto; 
            List<OrdenTransporte> ordenTransportes;
            Direccion direccion = null;

            using(var transaction = _context.Database.BeginTransaction())
            {

                foreach (var man in manifiestosForRegisters)
                {
                        manifiesto = new Manifiesto();
                        ordenTransportes = new List<OrdenTransporte>();
                        incidencias = new List<Incidencia>();

                        manifiesto.fecha_registro = DateTime.Now;
                        manifiesto.usuario_id  = man.usuario_id; 
                        manifiesto.estado_id = (int) Constantes.EstadoManifiesto.Registrado;
                        manifiesto.estiba = man.estiba;
                        manifiesto.costotercero = 0;
                        manifiesto.valorizado = 0;
                        

                        await _context.AddAsync<Manifiesto>(manifiesto);
                        await _context.SaveChangesAsync();

                        manifiesto.numero_manifiesto  =  "MAN-" + manifiesto.id.ToString().PadLeft(6,'0');
                        
                        await _context.SaveChangesAsync();

                    foreach (var item in man.Ordenes) 
                    {
                        ordenTransporte = new OrdenTransporte();
                        ordenTransporte.cantidad = item.cantidad;
                        ordenTransporte.manifiesto_id = manifiesto.id;
                        ordenTransporte.por_asignar = item.por_asignar;
                        ordenTransporte.equipo_transporte_id = item.equipo_transporte_id;
                        ordenTransporte.delivery = item.delivery;
                        ordenTransporte.destinatario_id = item.destinatario_id;
                        ordenTransporte.destinatario = item.destinatario.Trim();


                        ordenTransporte.direccion_carga = item.direccion_carga;
                        direccion =  _context.Direccion.Where(x=> x.direccion == item.direccion_carga).FirstOrDefault();
                        if(direccion != null)
                        {
                            ordenTransporte.lng_carga = direccion.lng;
                            ordenTransporte.lat_carga = direccion.lat;
                        }




                        ordenTransporte.direccion_destino_servicio = item.direccion_destino_servicio;
                        ordenTransporte.direccion_entrega = item.direccion_entrega;

                        ordenTransporte.distrito_carga_id = item.distrito_carga_id;
                        ordenTransporte.distrito_destino_servicio_id = item.distrito_destino_servicio_id;
                        ordenTransporte.factura = item.factura;
                        ordenTransporte.fecha_carga = item.fecha_carga;
                        ordenTransporte.fecha_entrega = item.fecha_entrega;
                        ordenTransporte.fecha_salida = item.fecha_salida;
                        ordenTransporte.hora_carga = item.hora_carga;
                        ordenTransporte.hora_entrega = item.hora_entrega;
                        ordenTransporte.hora_salida = item.hora_salida;
                        ordenTransporte.oc = item.oc;
                        ordenTransporte.guias = item.guias;
                        ordenTransporte.peso = item.peso;

                        ordenTransporte.provincia_entrega = item.provincia_entrega;
                        ordenTransporte.remitente_id = item.remitente_id;
                        ordenTransporte.shipment = item.shipment;
                        ordenTransporte.tiposervicio_id = item.tiposervicio_id;
                        
                        ordenTransporte.volumen = item.volumen;
                        ordenTransporte.estado_id = item.estado_id;
                        ordenTransporte.fecha_registro = DateTime.Now;
                        ordenTransporte.usuario_registro_id = usrid;
                        ordenTransporte.activo = true;
                        ordenTransporte.recojo = item.recojo;
                        ordenTransporte.notificacion = item.notificacion;
                        ordenTransporte.costo = item.costo;
                        ordenTransporte.valorizado = item.valorizado;
                        
                        if(ordenTransporte.valorizado != null)
                        {
                          manifiesto.valorizado =  manifiesto.valorizado  + ordenTransporte.valorizado;
                        }
                        


                        ordenTransportes.Add(ordenTransporte);
                    }

                    

                    await _context.AddRangeAsync(ordenTransportes);
                    await _context.SaveChangesAsync();

                    ordenTransportes.ForEach(x=>    
                       {
                            x.numero_ot =  "100" + "-" + x.id.ToString().PadLeft(6,'0');

                    });

                    await _context.SaveChangesAsync();



                     ordenTransportes.ForEach(x=>    
                       {
                            
                            incidencia = new Incidencia ();
                            incidencia.activo = true;
                            incidencia.descripcion = "";
                            incidencia.documento = "";
                            incidencia.fecha_incidencia = DateTime.Now;
                            incidencia.fecha_registro = DateTime.Now;
                            incidencia.maestro_incidencia_id = 1; 
                            incidencia.observacion = "";
                            incidencia.orden_trabajo_id = x.id;
                            incidencia.usuario_id = usrid;
                         
                            incidencias.Add(incidencia);

                            if(!x.por_asignar){
                                    incidencia = new Incidencia ();
                                    incidencia.activo = true;
                                    incidencia.descripcion = "";
                                    incidencia.documento = "";
                                    incidencia.fecha_incidencia = DateTime.Now;
                                    incidencia.fecha_registro = DateTime.Now;
                                    incidencia.maestro_incidencia_id = 2; 
                                    incidencia.observacion = "Asignada a " + GetEquipoTransporte(x.equipo_transporte_id.Value);
                                    incidencia.orden_trabajo_id = x.id;
                                    incidencia.usuario_id = 1;
                                    
                                    incidencias.Add(incidencia);
                            }


                        });

                        try
                        {
                             await _context.AddRangeAsync(incidencias);
                            await _context.SaveChangesAsync();
                        }
                        catch (System.Exception ex)
                        {
                             transaction.Rollback(); 
                            
                            return 1;
                            throw;
                        }

                       
                }

            
               
                        transaction.Commit();

            }
            
            return 1;
        }
         public async Task<int> RegisterOrdenes_Interface(List<Manifiesto> manifiestosForRegisters, int usrid)
        {
            
            OrdenTransporte ordenTransporte ;
            Incidencia incidencia;
            List<Incidencia> incidencias;
            Manifiesto manifiesto; 
            List<OrdenTransporte> ordenTransportes;
            Direccion direccion = null;

            using(var transaction = _context.Database.BeginTransaction())
            {

                foreach (var man in manifiestosForRegisters)
                {
                        manifiesto = new Manifiesto();
                        ordenTransportes = new List<OrdenTransporte>();
                        incidencias = new List<Incidencia>();

                        manifiesto.fecha_registro = DateTime.Now;
                        manifiesto.usuario_id  = man.usuario_id; 
                        manifiesto.estado_id = (int) Constantes.EstadoManifiesto.Registrado;
                        manifiesto.estiba = man.estiba;
                        manifiesto.costotercero = 260;
                        manifiesto.valorizado = Convert.ToDecimal(380.80);
                        

                        await _context.AddAsync<Manifiesto>(manifiesto);
                        await _context.SaveChangesAsync();

                        manifiesto.numero_manifiesto  =  "MAN-" + manifiesto.id.ToString().PadLeft(6,'0');
                        
                        await _context.SaveChangesAsync();

                  

                    foreach (var item in man.Ordenes) 
                    {
                        ordenTransporte = new OrdenTransporte();
                        ordenTransporte.cantidad = item.cantidad;
                        ordenTransporte.manifiesto_id = manifiesto.id;

                        ordenTransporte.por_asignar = item.por_asignar;
                        ordenTransporte.equipo_transporte_id = item.equipo_transporte_id;
                        ordenTransporte.delivery = item.delivery;
                        ordenTransporte.destinatario_id = item.destinatario_id;
                        ordenTransporte.destinatario = item.destinatario.Trim();


                        ordenTransporte.direccion_carga = item.direccion_carga;
                        direccion =  _context.Direccion.Where(x=> x.direccion == item.direccion_carga).FirstOrDefault();
                        if(direccion != null)
                        {
                            ordenTransporte.lng_carga = direccion.lng;
                            ordenTransporte.lat_carga = direccion.lat;
                        }




                        ordenTransporte.direccion_destino_servicio = item.direccion_destino_servicio;
                        ordenTransporte.direccion_entrega = item.direccion_entrega;

                        ordenTransporte.distrito_carga_id = item.distrito_carga_id;
                        ordenTransporte.distrito_destino_servicio_id = item.distrito_destino_servicio_id;
                        ordenTransporte.factura = item.factura;
                        ordenTransporte.fecha_carga = item.fecha_carga;
                        ordenTransporte.fecha_entrega = item.fecha_entrega;
                        ordenTransporte.fecha_salida = item.fecha_salida;
                        ordenTransporte.hora_carga = item.hora_carga;
                        ordenTransporte.hora_entrega = item.hora_entrega;
                        ordenTransporte.hora_salida = item.hora_salida;
                        ordenTransporte.oc = item.oc;
                        ordenTransporte.guias = item.guias;
                        ordenTransporte.peso = item.peso;

                        ordenTransporte.provincia_entrega = item.provincia_entrega;
                        ordenTransporte.remitente_id = item.remitente_id;
                        ordenTransporte.shipment = item.shipment;
                        ordenTransporte.tiposervicio_id = item.tiposervicio_id;
                        
                        ordenTransporte.volumen = item.volumen;
                        ordenTransporte.estado_id = item.estado_id;
                        ordenTransporte.fecha_registro = DateTime.Now;
                        ordenTransporte.usuario_registro_id = usrid;
                        ordenTransporte.activo = true;
                        ordenTransporte.recojo = item.recojo;
                        ordenTransporte.notificacion = item.notificacion;
                        ordenTransporte.costo = item.costo;
                        ordenTransporte.valorizado = item.valorizado;
                        


                        ordenTransportes.Add(ordenTransporte);
                    }

                    await _context.AddRangeAsync(ordenTransportes);
                    await _context.SaveChangesAsync();

                    ordenTransportes.ForEach(x=>    
                       {
                            x.numero_ot =  "100" + "-" + x.id.ToString().PadLeft(6,'0');

                    });

                    await _context.SaveChangesAsync();



                     ordenTransportes.ForEach(x=>    
                       {
                            
                            incidencia = new Incidencia ();
                            incidencia.activo = true;
                            incidencia.descripcion = "";
                            incidencia.documento = "";
                            incidencia.fecha_incidencia = DateTime.Now;
                            incidencia.fecha_registro = DateTime.Now;
                            incidencia.maestro_incidencia_id = 1; 
                            incidencia.observacion = "";
                            incidencia.orden_trabajo_id = x.id;
                            incidencia.usuario_id = usrid;
                         
                            incidencias.Add(incidencia);

                            if(!x.por_asignar){
                                    incidencia = new Incidencia ();
                                    incidencia.activo = true;
                                    incidencia.descripcion = "";
                                    incidencia.documento = "";
                                    incidencia.fecha_incidencia = DateTime.Now;
                                    incidencia.fecha_registro = DateTime.Now;
                                    incidencia.maestro_incidencia_id = 2; 
                                    incidencia.observacion = "Asignada a " + GetEquipoTransporte(x.equipo_transporte_id.Value);
                                    incidencia.orden_trabajo_id = x.id;
                                    incidencia.usuario_id = 1;
                                    
                                    incidencias.Add(incidencia);
                            }


                        });

                        foreach (var item in ordenTransportes) 
                        {
                            item.valorizado =  Convert.ToDecimal(380.80);
                             await _context.SaveChangesAsync();
                            break;
                        }

                        try
                        {
                             await _context.AddRangeAsync(incidencias);
                            await _context.SaveChangesAsync();
                        }
                        catch (System.Exception ex)
                        {
                             transaction.Rollback(); 
                            
                            return 1;
                            throw;
                        }

                       
                }

            
               
                        transaction.Commit();

            }
            
            return 1;
        }
        
        public async Task<bool> DeleteSustento(long id)
        {
             try
            {
                var val = false;
                using(var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var sustento = _context.Sustento.Where(x=>x.id == id).SingleOrDefault();

                        _context.Sustento.Remove(sustento);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        val =  true;
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback(); 
                        throw;
                    }
                }
                return val;
            }
            catch (System.Exception ex)
            {
               throw new ArgumentException("Error al eliminar el detalle del sustento");
            }
        }
        private string GetEquipoTransporte(long equipotransporteid)
        {
             var equipo =  _context.EquipoTransporte.Where(x=>x.Id == equipotransporteid).FirstOrDefault();
             var vehiculo =  _context.Vehiculo.Where(x=>x.Id == equipo.VehiculoId).FirstOrDefault();
             var chofer = _context.Chofer.Where(x=>x.Id == equipo.ChoferId).FirstOrDefault();

             return vehiculo.Placa +  " - "  +  chofer.NombreCompleto;
        }

        public async Task<bool> ActualizarIncidencia (IncidenciaForUpdate incidencia )
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // var objIncidencia = await _context.Incidencias.Where(x=> x.id == incidencia.id).SingleOrDefaultAsync();
                    var objIncidencia = _context.Incidencias.Where(x=> x.id == incidencia.id).FirstOrDefault(); 
                    objIncidencia.fecha_incidencia      = incidencia.fecha;
                    objIncidencia.maestro_incidencia_id = incidencia.incidencia;
                    objIncidencia.descripcion = incidencia.observacion;
                    objIncidencia.fecha_modificacion= DateTime.Now;
                    objIncidencia.usuario_modificacion= incidencia.usuario;
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                    
                }
            }
            return true;
        }

     
        public async Task<long> RegisterSustento(SustentoForRegister sustentoregister)
        {
            Sustento sus;

            using(var transaction = _context.Database.BeginTransaction())
            {
                  try
                  {
                    sus = new Sustento();
                    sus.manifiesto = sustentoregister.manifiesto;
                    sus.fecha =  DateTime.Now;                    
                    sus.monto = sustentoregister.monto;
                    sus.kilometrajeInicio = sustentoregister.kilometrajeInicio;
                    sus.kilometrajefinal  = sustentoregister.kilometrajefinal;
                    sus.usuarioRegistro = sustentoregister.usuarioRegistro;
                    sus.fechaRegistro =  DateTime.Now;                    

                    await  _context.Sustento.AddAsync(sus);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                  }
                  catch (System.Exception ex)
                  {
                        transaction.Rollback(); 
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new ArgumentException("Error al insertar sustento");
                  }
                  return sus.id;
                 
            }
        }

        public async Task<bool> ActualizarSustento(SustentoForRegister sustentoupdate)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                  var val = false;
                  try
                  {
                    
                    var sustento =   _context.Sustento.Where(x=>x.id == sustentoupdate.id).SingleOrDefault();                  
                     sustento.monto = sustentoupdate.monto;
                    sustento.kilometrajeInicio = sustentoupdate.kilometrajeInicio;
                    sustento.kilometrajefinal  = sustentoupdate.kilometrajefinal;                   
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    val = true;
                    return val;
                  }
                  catch (System.Exception ex)
                  {
                    transaction.Rollback(); 
                    var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                    throw new ArgumentException("Error al actualizar el sustento");
                  }
            }
        }

        public async Task<bool> FinalizarSustento(FinalizarSustentoForUpdate sustentoupdate)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                  var val = false;
                  try
                  {
                    
                    var sustento  =   _context.Sustento.Where(x=>x.id == sustentoupdate.sustento).SingleOrDefault();  
                    var manifiesto =   _context.Manifiesto.Where(x=>x.id == sustento.manifiesto).SingleOrDefault();                
                    sustento.monto = sustentoupdate.monto;
                    sustento.kilometrajeInicio = sustentoupdate.kilometrajeInicio;
                    sustento.kilometrajefinal  = sustentoupdate.kilometrajefinal;   
                    manifiesto.estado_id = 46 ; // Liquidado                
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    val = true;
                    return val;
                  }
                  catch (System.Exception ex)
                  {
                    transaction.Rollback(); 
                    var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                    throw new ArgumentException("Error al finalizar el sustento");
                  }
            }
        }

        public async Task<long> RegisterSustentoDetalle(SustentoDetalleForRegister sustentodetalleregister)
        {
            SustentoDetalle det;
            using(var transaction = _context.Database.BeginTransaction())
            {
                  try
                  {
                    det = new SustentoDetalle();
                    det.sustento =  sustentodetalleregister.sustento;                    
                    det.fecha =  DateTime.Now;
                    det.tipo = sustentodetalleregister.tipo;
                    det.tipoSustento = sustentodetalleregister.tipoSustento;
                    det.serieDocumento = sustentodetalleregister.serieDocumento;  
                    det.numeroDocumento = sustentodetalleregister.numeroDocumento;    
                    det.tipoDocumentoEmisor = sustentodetalleregister.tipoDocumentoEmisor;    
                    det.documentoEmisor = sustentodetalleregister.documentoEmisor;    
                    det.razonSocialEmisor = sustentodetalleregister.razonSocialEmisor;
                    det.montoBase = sustentodetalleregister.montoBase;
                    det.montoImpuesto = sustentodetalleregister.montoImpuesto;
                    det.montoTotal = sustentodetalleregister.montoTotal;
                    det.usuarioAprobador = sustentodetalleregister.usuarioAprobador;
                    det.aprobado = sustentodetalleregister.aprobado;
                    det.fechaAprobacion = sustentodetalleregister.fechaAprobacion;   
                    det.usuarioAprobacion = sustentodetalleregister.usuarioAprobacion;  
                    det.estado = sustentodetalleregister.estado;  
                    det.usuarioRegistro = sustentodetalleregister.usuarioRegistro;  
                    det.fechaRegistro = DateTime.Now;   
                    det.fechacarga =  sustentodetalleregister.fechacarga;
                    det.valorBase = sustentodetalleregister.valorBase;    
                    det.costoD2 = sustentodetalleregister.costoD2;       

                    await  _context.SustentoDetalle.AddAsync(det);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                  }
                  catch (System.Exception ex)
                  {
                        transaction.Rollback(); 
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new ArgumentException("Error al insertar detalle del sustento");
                  }
                  return det.id;                 
            }
        }


        public async Task<bool>  DeleteSustentoDetalle(long idsustentodetalle)
        {
            try
            {
                var val = false;
                using(var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var deletedetalle = _context.SustentoDetalle.Where(x=>x.id == idsustentodetalle).SingleOrDefault();

                        _context.SustentoDetalle.Remove(deletedetalle);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        val =  true;
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback(); 
                        throw;
                    }
                }
                return val;
            }
            catch (System.Exception ex)
            {
               throw new ArgumentException("Error al eliminar el detalle del sustento");
            }
        }

        public async Task<bool>  AprobarDocumentoSustento(long documento, int usuario)
        {
            try
            {
                var val = false;
                using(var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var detalle =   _context.SustentoDetalle.Where(x=>x.id == documento).SingleOrDefault();
                        detalle.aprobado = true;
                        detalle.fechaAprobacion= DateTime.Now;
                        detalle.usuarioAprobacion= usuario;
                        detalle.estado = 44;
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        val =  true;
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback(); 
                        throw;
                    }
                }
                return val;
            }
            catch (System.Exception ex)
            {
               throw new ArgumentException("Error al aprobar el documento");
            }
        }

        public async Task<bool>  RechazarDocumentoSustento(long documento, int usuario)
        {
            try
            {
                var val = false;
                using(var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var detalle =   _context.SustentoDetalle.Where(x=>x.id == documento).SingleOrDefault();
                        detalle.aprobado = false;
                        detalle.usuarioAprobacion= usuario;
                        detalle.estado = 45;
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        val =  true;
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback(); 
                        throw;
                    }
                }
                return val;
            }
            catch (System.Exception ex)
            {
               throw new ArgumentException("Error al aprobar el documento");
            }
        }


    }
  
}
