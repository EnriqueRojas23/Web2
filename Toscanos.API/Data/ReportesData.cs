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

namespace Toscanos.API.Data
{
    public class Reporte
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

        public Reporte(DataContext context,
        IMapper mapper,
        IRepository<Vehiculo> repo_Vehiculo,
        IRepository<Proveedor> repo_Proveedor,
        IRepository<Chofer> repo_Chofer,
        IRepository<Incidencia> repo_Incidencia,
        ISeguimientoReadRepository repo_seguimiento,
        IOrdenRepository repository,
        IQueryHandler<ObtenerEquipoTransporteParameter> handlerEqTransporte) {
                _context = context;
                _repo_Proveedor = repo_Proveedor;
                _mapper = mapper;
                _repo_Chofer = repo_Chofer;
                _repo_Incidencia = repo_Incidencia;
                _repo_Seguimiento = repo_seguimiento;
                _repository = repository;
                _repo_Vehiculo = repo_Vehiculo;
                _handlerEqTransporte = handlerEqTransporte;
        }
        public EquipoTransporte GetEquipoTransporte(long id) => _repo_Seguimiento.GetEquipoTransporte(id);


      
        public async Task<GetTotalDespachos> GetTotalDespachos(int? remitente_id, string fec_ini,string fec_fin) 
        {
            return await _repo_Seguimiento.GetTotalDespachos(remitente_id,fec_ini, fec_fin);
        }
        public async Task<GetDespachosATiempo> GetDespachosATiempo(int? remitente_id, string fec_ini,string fec_fin) 
        {
            return await _repo_Seguimiento.GetDespachosATiempo(remitente_id,fec_ini, fec_fin);
        }
        public async Task<IEnumerable<ReporteEncuestaResult>> GetReporteEncuesta(int? remitente_id, int? usuario_id, string fec_ini,string fec_fin) 
        {
            return await _repo_Seguimiento.GetReporteEncuesta(remitente_id, usuario_id,fec_ini, fec_fin);
        }
        public async Task<IEnumerable<GetDespachosTiempoEntrega>> GetDespachosTiempoEntrega(int? remitente_id, string fec_ini,string fec_fin) 
        {
            return await _repo_Seguimiento.GetDespachosTiempoEntrega(remitente_id,fec_ini, fec_fin);
        }
          public async Task<IEnumerable<GetDaysOfWeek>> GetDaysOfWeek(int? remitente_id, string fec_ini,string fec_fin) 
        {
            return await _repo_Seguimiento.GetDaysOfWeek(remitente_id,fec_ini, fec_fin);
        }
         public async Task<IEnumerable<GetCantidadxManifiesto>> GetCantidadxManifiesto(int? remitente_id, string fec_ini,string fec_fin) 
        {
            return await _repo_Seguimiento.GetCantidadxManifiesto(remitente_id,fec_ini, fec_fin);
        }
        public async Task<IEnumerable<GetDespachosPuntualidad>> GetDespachosPuntualidad(int? remitente_id, string fec_ini,string fec_fin) 
        {
            return await _repo_Seguimiento.GetDespachosPuntualidad(remitente_id,fec_ini, fec_fin);
        }
        public async Task<IEnumerable<GetAsignacionUnidades>> GetAsignacionUnidadesVehiculo() 
        {
            return await _repo_Seguimiento.GetAsignacionUnidadesVehiculo();
        }
        public async Task<IEnumerable<GetAsignacionUnidades>> GetAsignacionUnidadesVehiculoTerceros() 
        {
            return await _repo_Seguimiento.GetAsignacionUnidadesVehiculoTerceros();
        }
        public async Task<IEnumerable<GetActivityPropios>> GetVehiculoPropios() 
        {
            return await _repo_Seguimiento.GetVehiculoPropios();
        }
        
        
        
        
    }
}