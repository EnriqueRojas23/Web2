using System.Collections.Generic;
using System.Threading.Tasks;
using CargaClic.Domain.Mantenimiento;
using CargaClic.ReadRepository.Contracts.Seguimiento.Results;

namespace CargaClic.ReadRepository.Interface.Seguimiento
{
    public interface ISeguimientoReadRepository
    {
         Task<IEnumerable<GetOrdenTransporte>> GetAllOrdenTransporte(string remitente_id, int? estado_id, int usuario_id
         , string fec_ini, string fec_fin, string pedido);

         Task<IEnumerable<GetOrdenTransporte>> GetAllOrdenTransporte(long manifiestoId);
         Task<IEnumerable<GetOrdenTransporte>> SearchOrdenTransporte(string criterio, int dias);

         Task<IEnumerable<GetOrdenTransporte>> GetAllOrdenTransporteCliente(int manifiestoId, int UsuarioId);
         Task<ObtenerOrdenTransporteDto> ObtenerOrdenTrasporte(long orden_id);
         Task<IEnumerable<GetReporteMargenDto>> GetListarReporteMargen(int anio, int mes);

        
         Task<IEnumerable<GetEstadisticas>> GetEstadisticas(int cliente_id);
         
         Task<IEnumerable<GetDocumentoResult>> GetAllDocumentos(int site_id);
         Task<IEnumerable<GetIncidencia>> GetAllOrdenIncidencias(long OrdenTransporteId);
         Task<GetLocalizacionResult> GetLocalizacion(int usuarioId);
         Task<IEnumerable<GetLocalizacionResult>> GetLiveView();
         EquipoTransporte GetEquipoTransporte(long id);

         Task<GetTotalDespachos> GetTotalDespachos(int? remitente_id, string fec_ini, int? tiposervicioid);
         Task<GetDespachosATiempo> GetDespachosATiempo(int? remitente_id,string fec_ini, string fec_fin);

         Task<IEnumerable<ReporteEncuestaResult>> GetReporteEncuesta(int? remitente_id, int? usuario_id,string fec_ini, string fec_fin);
         Task<IEnumerable<GetSustentoManifiesto>> GetSustentoManifiestoCerrado(string manifiesto);
         Task<IEnumerable<GetOrdenTransporte>> getPendientesPorDia(int? remitente_id, string fec_ini, int? tiposervicioid);


         Task<IEnumerable<GetDespachosTiempoEntrega>> GetDespachosTiempoEntrega(int? remitente_id,string fec_ini, string fec_fin);
         Task<IEnumerable<GetDaysOfWeek>> GetDaysOfWeek(int? remitente_id,string fec_ini, string fec_fin);
         Task<IEnumerable<GetCantidadxManifiesto>> GetCantidadxManifiesto(int? remitente_id,string fec_ini, string fec_fin);
         Task<IEnumerable<GetDespachosPuntualidad>> GetDespachosPuntualidad(int? remitente_id,string fec_ini, string fec_fin);

         Task<IEnumerable<GetActivityTotal>> GetActivityTotal();
         Task<IEnumerable<GetActivityTotal>> GetActivityTotalRecojo();
         Task<IEnumerable<GetActivityTotal>> GetActivityTotalClientes();
         Task<IEnumerable<GetActivityVehiculoRuta>> GetActivityVehiculosRuta();
         Task<IEnumerable<ReporteServicioResult>> GetReporteServicio();
         
         Task<IEnumerable<GetActivityOTsTotalesYEntregadas>> GetActivityOTTotalesYEntregadas();

         Task<IEnumerable<GetAsignacionUnidades>> GetAsignacionUnidadesVehiculo();
         Task<IEnumerable<GetAsignacionUnidades>> GetAsignacionUnidadesVehiculoTerceros();
         Task<IEnumerable<GetActivityPropios>> GetVehiculoPropios();

         Task<IEnumerable<GetMaestroIncidencia>> GetMaestroIncidencias();

         Task<GetDatosIncidencia> GetDatosIncidencia(long incidencia);
         Task<IEnumerable<GetSustentoResult>> GetAllSustento(GetSustentoResult sustento);

         Task<IEnumerable<GetReporteServicioResult>> GetReporteServicio_unidadesasignadas(string clients);
         Task<IEnumerable<GetReporteServicioResult>> GetReporteServicio_valorizacion(string clients);


         
         Task<IEnumerable<GetManifiestoPendienteSustentar>> GetManifiestoPendienteSustentar(string documento);
         Task<IEnumerable<GetTipoDocumentoSustento>> GetTipoDocumentoSustento();
         Task<IEnumerable<GetTipoSustento>> GetTipoSustento();
         Task<GetSustentoManifiesto> GetSustentoManifiesto(long sustento);

         Task<IEnumerable<GetDocumentoSustento>> GetDocumentosSustento(long? sustento,int? tipo);
         Task<IEnumerable<GetDocumentoSustentoPendiente>> GetDocumentosSustentoPendientesAprobacion(string docConductor);
         Task<IEnumerable<GetUsuarioAprobacionSustento>> GetUsuarioAprobacionSustento();
         Task<GetRazonSocialSustento> GetRazonSocialSustento(string ruc);
         Task<IEnumerable<GetTipoDocumentoEmisor>> GetTiposDocumentoEmisor();
         
         Task<GetOrdenTransporte> GetOrdenByWayPoint(long manifiesto_id, string lat, string lng);
    }
}
