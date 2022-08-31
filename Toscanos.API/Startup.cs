
using System.Net;
using System.Text;
using AutoMapper;
using CargaClic.Data;
using CargaClic.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.IdentityModel.Tokens;
using CargaClic.API.Data;

using CargaClic.Handlers.Seguridad;
using Common.QueryHandlers;
using CargaClic.Data.Contracts.Parameters.Seguridad;


using CargaClic.Domain.Mantenimiento;

using CargaClic.Contracts.Parameters.Mantenimiento;
using CargaClic.Handlers.Mantenimiento;



using CargaClic.Domain.Inventario;

using CargaClic.ReadRepository.Interface.Mantenimiento;
using CargaClic.ReadRepository.Interface.Inventario;
using CargaClic.ReadRepository.Repository.Inventario;
using CargaClic.ReadRepository.Interface.Despacho;
using CargaClic.ReadRepository.Repository.Despacho;
using CargaClic.Domain.Despacho;
using CargaClic.ReadRepository.Interface.Facturacion;
using CargaClic.Domain.Facturacion;

using CargaClic.Domain.Seguimiento;
using Toscanos.API.Data;
using CargaClic.ReadRepository.Interface.Seguimiento;
using CargaClic.Data.Interface;
using Common;
using CargaClic.Repository.Interface.Mantenimiento;
using CargaClic.Repository.Repository.Mantenimiento;
using CargaClic.Repository.Interface;
using CargaClic.Repository.Interface.Seguimiento;
using CargaClic.Repository;
using CargaClic.Repository.Seguimiento;
using CargaClic.Domain.Seguridad;
using CargaClic.ReadRepository.Repository.Seguimiento;

namespace CargaClic.API
{
    public class Startup
    {
     
  
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddMvc(options => options.EnableEndpointRouting = false);

             services.AddDbContext<DataContext>(x=>x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
             services.AddSingleton(_ => Configuration);
             //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
             services.AddCors();
             services.AddAutoMapper();
             
             services.AddTransient<Seed>();
             services.AddTransient<Seguimiento>();
             services.AddTransient<Reporte>();
             //services.AddTransient<Seed>();

             services.AddScoped<IRepository<User>,Repository<User>>();
             services.AddScoped<IRepository<Rol>,Repository<Rol>>();
             services.AddScoped<IRepository<RolPagina>,Repository<RolPagina>>();
             services.AddScoped<IRepository<Pagina>,Repository<Pagina>>();
             services.AddScoped<IRepository<RolUser>,Repository<RolUser>>();
             services.AddScoped<IRepository<Estado>,Repository<Estado>>();
             services.AddScoped<IRepository<Incidencia>,Repository<Incidencia>>();
             services.AddScoped<IRepository<Archivo>,Repository<Archivo>>();
             services.AddScoped<IRepository<Proveedor>,Repository<Proveedor>>();
             services.AddScoped<IRepository<Manifiesto>,Repository<Manifiesto>>();

             

             services.AddScoped<IMantenimientoReadRepository,MantenimientoReadRepository>();
             services.AddScoped<IAuthRepository,AuthRepository>();
             services.AddScoped<ISeguimientoReadRepository,SeguimientoReadRepository>();
             services.AddScoped<IQueryHandler<ListarUsuariosParameters>,ListarUsuariosQuery>();
             services.AddScoped<IQueryHandler<ListarMenusxRolParameter>,ListarMenusxRolQuery>();
             services.AddScoped<IQueryHandler<ListarTreeViewParameter>,ListarTreeViewQuery>();
             services.AddScoped<IQueryHandler<ListarRolesPorUsuarioParameter>,ListarRolesPorUsuarioQuery>();


            services.AddScoped<IRepository<Cliente>, Repository<Cliente>>();
            services.AddScoped<IRepository<Proveedor>, Repository<Proveedor>>();
            services.AddScoped<IRepository<Vehiculo>, Repository<Vehiculo>>();
            services.AddScoped<IRepository<Huella>, Repository<Huella>>();
            services.AddScoped<IRepository<Chofer>, Repository<Chofer>>();
            services.AddScoped<IRepository<ValorTabla>, Repository<ValorTabla>>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();


       
            services.AddScoped<IQueryHandler<ListarPlacasParameter>,ListarPlacasQuery>();
            services.AddScoped<IQueryHandler<ListarProveedorParameter>,ListarProveedorQuery>();
            services.AddScoped<IQueryHandler<ObtenerEquipoTransporteParameter>,ObtenerEquipoTransporteQuery>();




            

            services.AddScoped<IRepository<OrdenSalida>,Repository<OrdenSalida>>();
            services.AddScoped<IRepository<OrdenSalidaDetalle>,Repository<OrdenSalidaDetalle>>();
            services.AddScoped<IOrdenRepository,OrdenRepository>();
            services.AddScoped<IOrdenSalidaRepository,OrdenSalidaRepository>();

            services.AddScoped<IManifiestoReadRepository,ManifiestoReadRepository>();


            services.AddScoped<IDespachoReadRepository,DespachoReadRepository>();

            services.AddScoped<IFacturacionReadRepository,FacturacionReadRepository>();
            services.AddScoped<IFacturacionRepository,FacturacionRepository>();
            services.AddScoped<IRepository<Documento>,Repository<Documento>>();
            services.AddScoped<IRepository<CentroCosto>,Repository<CentroCosto>>();
            
            
            services.AddScoped<IRepository<Propietario>,Repository<Propietario>>();
            services.AddScoped<IRepository<OrdenTransporte>,Repository<OrdenTransporte>>();
            services.AddScoped<IRepository<CargaMasivaDetalle>,Repository<CargaMasivaDetalle>>();
            
            

            
            
    
            services.AddScoped<IRepository<Area>,Repository<Area>>();
            services.AddScoped<IRepository<InventarioGeneral>,Repository<InventarioGeneral>>();
                 

             
             services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options => {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(
                                   Encoding.ASCII.GetBytes(Configuration
                                .GetSection("AppSettings:Token").Value)),
                                ValidateIssuer = false,
                                ValidateAudience = false                            
                            };
                        });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder,ILoggerFactory loggerFactory)
        {
            // loggerFactory.AddConsole(Configuration.GetSection("Loggin"));
            // loggerFactory.AddDebug();
            loggerFactory.AddFile("Logs/toscanos-{Date}.txt");
            

            if (env.IsDevelopment())
            {
                  app.UseExceptionHandler(builder=> { 
                    builder.Run(async context => {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message); 
                            await context.Response.WriteAsync(error.Error.Message); 
                        }
                    });
                });
            }
            else
            {
                app.UseExceptionHandler(builder=> { 
                    builder.Run(async context => {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message); 
                            await context.Response.WriteAsync(error.Error.Message); 
                        }
                    });
                });
               // app.UseHsts();
            }
            // app.UseHttpsRedirection();
            
            //seeder.SeedEstados();
           // seeder.SeedUsers();
            //seeder.SeedPaginas();
            //seeder.SeedRoles();
           // seeder.SeedRolPaginas();
            
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseMvc();
          
           // app.UseMvc();
        }
    }
}   

