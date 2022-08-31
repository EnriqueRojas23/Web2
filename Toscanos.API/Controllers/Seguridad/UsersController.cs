using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using CargaClic.API.Dtos;
using CargaClic.Data;
using CargaClic.Data.Contracts.Parameters.Seguridad;
using CargaClic.Data.Contracts.Results.Seguridad;
using CargaClic.Data.Interface;
using CargaClic.Domain.Mantenimiento;
using CargaClic.Domain.Seguridad;
using Common.QueryHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CargaClic.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _repo;
        private readonly IRepository<Chofer> _repo_Chofer;
        private readonly IAuthRepository _auth;
        private readonly IMapper _mapper;
        private readonly IQueryHandler<ListarUsuariosParameters> _user;

        public IRepository<User> Repo => _repo;

        public UsersController(IRepository<User> repo
        ,IRepository<Chofer> repo_chofer
        ,IAuthRepository auth
        , IMapper mapper
        , IQueryHandler<ListarUsuariosParameters> user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _repo_Chofer = repo_chofer ?? throw new ArgumentNullException(nameof(repo_chofer));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            string clientes = string.Empty;

            if (await _auth.UserExists(userForRegisterDto.Username))
                return BadRequest("Username ya existe");

            if(userForRegisterDto.clientesids != null) 
            {
                foreach (var item in userForRegisterDto.clientesids)
                    clientes =  clientes +  "," + item.ToString();

                clientes = clientes.Substring(1,clientes.Length - 1 );
            }
                     if(clientes == null)
                clientes = null; 

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username,
                NombreCompleto = userForRegisterDto.NombreCompleto,
                Email = userForRegisterDto.Email,
                Created = DateTime.Now,
                LastActive = DateTime.Now,
                EstadoId = 1,
                Dni = userForRegisterDto.Dni    ,
                EsConductor = userForRegisterDto.EsConductor,
                ClientesIds = clientes
                
            };

            var createdUser = await _auth.Register(userToCreate, userForRegisterDto.Password);
            var choferDb = await _repo_Chofer.Get(x=>x.Dni== userForRegisterDto.Dni);
            
           
            if(userToCreate.EsConductor) {
                //crear conductor
                if(choferDb== null){

                var chofer = new Chofer();
                chofer.Brevete = userForRegisterDto.Dni;
                chofer.Dni = userForRegisterDto.Dni;
                chofer.NombreCompleto = userForRegisterDto.NombreCompleto;
                chofer.UsuarioId =  createdUser.Id ;
                    
                    try
                    {
                        await _repo_Chofer.AddAsync(chofer);
                    }
                    catch (DbUpdateException  sqlex)
                    {
                        throw new ArgumentException("No puede insertar datos duplicados");
                    }
                    catch (Exception  ex)
                    {
                            throw new ArgumentException("Ocurrió un error inesperado. Vuelva a intentarlo.");
                    }
                }
                else{
                    choferDb.UsuarioId = createdUser.Id;
                     await _repo_Chofer.SaveAll();
                }
              
                
            }



          
            return StatusCode(201);
        }
        [HttpPost("updateestado")]
        public async Task<IActionResult> UpdateEstado(UserForUpdateDto userForUpdateDto )
        {
          
            var userToUpdate = new User
            {
                Id = userForUpdateDto.Id,
                EstadoId = userForUpdateDto.EstadoId,
            };

            var createdUser = await _auth.UpdateEstadoId(userToUpdate);
            return StatusCode(200);
        }

        [HttpPost("updatePassword")]
        public async Task<IActionResult> updatePassword(UserPasswordForUpdateDto userPasswordForUpdateDto)
        {
            var userToCreate = new User
            {
                Id = userPasswordForUpdateDto.Id,
            };
            var createdUser = await _auth.UpdatePassword(userToCreate, userPasswordForUpdateDto.Password);
            return StatusCode(201);
        }


        [HttpPost("update")]
        public async Task<IActionResult> Update(UserForUpdateDto userForRegisterDto)
        {
            string clientes = string.Empty;

            if(userForRegisterDto.clientesids != null) 
            {
                foreach (var item in userForRegisterDto.clientesids)
                    clientes =  clientes +  "," + item.ToString();

                clientes = clientes.Substring(1,clientes.Length - 1 );
            }

            if(clientes == null)
                clientes = null;

                var userToCreate = new User
                {
                    Id = userForRegisterDto.Id,
                    NombreCompleto = userForRegisterDto.NombreCompleto,
                    Email = userForRegisterDto.Email,
                    Dni = userForRegisterDto.Dni,
                    EstadoId = userForRegisterDto.EstadoId,
                    EsConductor = userForRegisterDto.EsConductor,
                    ClientesIds = clientes,
                    
                };


            var createdUser = await _auth.Update(userToCreate);


                var choferDb = await _repo_Chofer.Get(x=>x.Dni== userForRegisterDto.Dni);
            
           
            if(userToCreate.EsConductor) {
                //crear conductor
                if(choferDb== null){

                var chofer = new Chofer();
                chofer.Brevete = userForRegisterDto.Dni;
                chofer.Dni = userForRegisterDto.Dni;
                chofer.NombreCompleto = userForRegisterDto.NombreCompleto;
                chofer.UsuarioId =  userToCreate.Id ;
                    
                    try
                    {
                        await _repo_Chofer.AddAsync(chofer);
                    }
                    catch (DbUpdateException  sqlex)
                    {
                        throw new ArgumentException("No puede insertar datos duplicados");
                    }
                    catch (Exception  ex)
                    {
                            throw new ArgumentException("Ocurrió un error inesperado. Vuelva a intentarlo.");
                    }
                }
                else{
                    choferDb.UsuarioId = createdUser.Id;
                     await _repo_Chofer.SaveAll();
                }
            }

            return StatusCode(201);
        }
        
        [HttpGet]
        public IActionResult GetUsers(string nombres)
        {
            ListarUsuariosParameters Param = new ListarUsuariosParameters();
            
            Param.Nombre = nombres;
            var users = (ListarUsuariosResult) _user.Execute(Param);
            return Ok(users.Hits);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id) 
        {
            var user = await _repo.Get(x => x.Id == id);
            var userToResult = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToResult);
        }


    }
}