using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileDental.DTOs;
using SmileDental.DTOs.Dentista;
using SmileDental.Models;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Services
{
    public class AuthServices : IAuthInterface
    {
        private readonly ApiDbContext _context;
        private readonly PasswordManager _passwordService;
        private readonly JWTHandler _jwtHandler;


        public AuthServices(ApiDbContext context, PasswordManager passwordService, JWTHandler jwtHandler)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtHandler = jwtHandler;
        }

        public async Task<bool> RegisterDentista(CrearDentistaDTO crearDentistaDTO)
        {
            try
            {
                if (crearDentistaDTO.Nombre == null ||
                !StringManager.validaNombre(crearDentistaDTO.Nombre) ||
                crearDentistaDTO.Apellido == null ||
                !StringManager.validaNombre(crearDentistaDTO.Apellido))
                {
                    throw new ArgumentException("El nombre ingresado no es válido.");
                }

                if (crearDentistaDTO.Password == null || !StringManager.validaPassword(crearDentistaDTO.Password))
                {
                    throw new ArgumentException("La contraseña ingresada no es válida.");
                }

                if (crearDentistaDTO.Dni == null || !StringManager.validaDni(crearDentistaDTO.Dni))
                {
                    throw new ArgumentException("El DNI ingresado no es válido.");
                }

                if (crearDentistaDTO.Telefono == null || !StringManager.validaTelefono(crearDentistaDTO.Telefono))
                {
                    throw new ArgumentException("El teléfono ingresado no es válido.");
                }

                if (await _context.Dentistas.FirstOrDefaultAsync(d => d.Email == crearDentistaDTO.Email) != null)
                {
                    throw new ArgumentException("El correo electrónico ingresado ya está registrado.");
                }

                if (await _context.Dentistas.FirstOrDefaultAsync(d => d.Dni == crearDentistaDTO.Dni) != null)
                {
                    throw new ArgumentException("El DNI ingresado ya está registrado.");
                }

                if (await _context.Dentistas.FirstOrDefaultAsync(d => d.EspecialidadId == crearDentistaDTO.EspecialidadId) == null)
                {
                    throw new ArgumentException("La especialidad ingresada no es válida.");
                }

                var dentista = new Dentista
                {
                    Dni = crearDentistaDTO.Dni,
                    FechaNacimiento = crearDentistaDTO.FechaNacimiento,
                    Nombre = crearDentistaDTO.Nombre,
                    Apellido = crearDentistaDTO.Apellido,
                    Email = crearDentistaDTO.Email,
                    Password = _passwordService.HashPassword(crearDentistaDTO.Password),
                    Telefono = crearDentistaDTO.Telefono,
                    EspecialidadId = crearDentistaDTO.EspecialidadId,
                    EsAdmin = crearDentistaDTO.EsAdmin,
                    FotoUrl = crearDentistaDTO.FotoUrl,
                    HoraEntrada = crearDentistaDTO.HoraEntrada,
                    HoraSalida = crearDentistaDTO.HoraSalida
                };

                return true;
            } catch(Exception ex)
            {
                return false;
                // MEJORAR ESTO 
            }

        }


        public async Task<bool> RegisterPaciente(CrearPacienteDTO crearUser)
        {
            if (!StringManager.validaNombre(crearUser.Nombre) || !StringManager.validaNombre(crearUser.Apellido))
            {
                throw new ArgumentException("El nombre ingresado no es válido.");
            }

            if (!StringManager.validaPassword(crearUser.Password))
            {
                throw new ArgumentException("La contraseña ingresada no es válida.");
            }

            if (!StringManager.validaDni(crearUser.Dni))
            {
                throw new ArgumentException("El DNI ingresado no es válido.");
            }

            if (!StringManager.validaTelefono(crearUser.Telefono))
            {
                throw new ArgumentException("El teléfono ingresado no es válido.");
            }

            if (await _context.Pacientes.FirstOrDefaultAsync(p => p.Email == crearUser.Email) != null)
            {
                throw new ArgumentException("El correo electrónico ingresado ya está registrado.");
            }

            if (await GetPacienteByDni(crearUser.Dni) != null)
            {
                throw new ArgumentException("El DNI ingresado ya está registrado.");
            }

            var paciente = new Paciente
            {
                Dni = crearUser.Dni,
                FechaNacimiento = crearUser.FechaNacimiento,
                Nombre = crearUser.Nombre,
                Apellido = crearUser.Apellido,
                Email = crearUser.Email,
                Password = _passwordService.HashPassword(crearUser.Password),
                Telefono = crearUser.Telefono
            };

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDTO> ValidarUsuario([FromBody] LoginDTO loginDTO)
        {
           switch (loginDTO.tipoUsuario.ToLower())
            {
                case "paciente":
                    // Utilizamos FirstOrDefaultAsync para operaciones asincrónicas.
                    var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Email == loginDTO.Email);

                    if (paciente != null && _passwordService.VerifyHashedPassword(paciente.Password, loginDTO.Password))
                    {
                        return new UserDTO
                        {
                            Id = paciente.Id,
                            Email = paciente.Email,
                            Role = "Paciente" 
                        };
                    }
                    break;

                case "dentista":

                    var dentista = await _context.Dentistas.FirstOrDefaultAsync(d=>d.Email == loginDTO.Email);
                    
                    if(dentista != null && _passwordService.VerifyHashedPassword(dentista.Password, loginDTO.Password))
                    {
                        return new UserDTO
                        {
                            Id = dentista.Id,
                            Email = dentista.Email,
                            Role = "Dentista"
                        };
                    }
                    break;

                case "administrador":

                    var administrador = await _context.Dentistas.FirstOrDefaultAsync(d=> d.Email == loginDTO.Email && d.EsAdmin == true);

                    if(administrador != null && _passwordService.VerifyHashedPassword(administrador.Password, loginDTO.Password))
                    {
                        return new UserDTO
                        {
                            Id = administrador.Id,
                            Email = administrador.Email,
                            Role = "Administrador"
                        };
                    }

                    break;
            }
            return null;
        }

        private async Task<Paciente> GetPacienteByDni(string dni) => await _context.Pacientes.FirstOrDefaultAsync(p => p.Dni == dni);

   
    }
    
}
