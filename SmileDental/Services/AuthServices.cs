using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SmileDental.Builders;
using SmileDental.DTOs;
using SmileDental.DTOs.Dentista;
using SmileDental.Models;
using SmileDental.Repositories.Repository;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Services
{
    public class AuthServices : IAuthInterface
    {
        private readonly ApiDbContext _context;
        private readonly DentistaRepository _dentistaRepository;
        private readonly PacienteRepository _pacienteRepository;
        private readonly CitaRepository _citaRepository;
        private readonly PasswordManager _passwordService;
        private readonly JWTHandler _jwtHandler;


        public AuthServices(ApiDbContext context, DentistaRepository dentistaRepository, PacienteRepository pacienteRepository, CitaRepository citaRepository, PasswordManager passwordService, JWTHandler jwtHandler)
        {
            _context = context;
            _pacienteRepository = pacienteRepository;
            _dentistaRepository = dentistaRepository;
            _citaRepository = citaRepository;
            _passwordService = passwordService;
            _jwtHandler = jwtHandler;
        }

     
        public async Task<bool> RegisterPaciente(DatosPersonalesDTO crearUser)
        {
             VerificaDatosPersonales.ValidarDatosPersonales(crearUser);

            if (await _context.Pacientes.FirstOrDefaultAsync(p => p.Email == crearUser.Email) != null)
            {
                throw new ArgumentException("El correo electrónico ingresado ya está registrado.");
            }

            if (await GetPacienteByDni(crearUser.Dni) != null)
            {
                throw new ArgumentException("El DNI ingresado ya está registrado.");
            }

            if (!StringManager.ValidaPassword(crearUser.Password))
            {
                throw new Exception("El password no es válido");
            }

            string hashedPassword = _passwordService.HashPassword(crearUser.Password);

            var paciente = new PacienteBuilder()
                .WithDni(crearUser.Dni)
                .WithNombre(crearUser.Nombre)
                .WithApellido(crearUser.Apellido)   
                .WithEmail(crearUser.Email)
                .WithPassword(hashedPassword)
                .WithTelefono(crearUser.Telefono)
                .WithFechaNacimiento(crearUser.FechaDeNacimiento)
                .Build();

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDTO?> ValidarUsuario([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
            {
                throw new ArgumentException("El correo electrónico y la contraseña son obligatorios.");
            }
            var userQueries = new Dictionary<string, Func<Task<dynamic>>>
            {
                { "paciente", async () => await _pacienteRepository.GetPacienteByEmail(loginDTO.Email)},
                { "dentista", async () => await _dentistaRepository.GetDentistaByEmail(loginDTO.Email) },
                { "administrador", async () => await _dentistaRepository.GetAdministradorByEmail(loginDTO.Email)}
            };

            if (!userQueries.ContainsKey(loginDTO.tipoUsuario.ToLower())) 
            { throw new ArgumentException("Tipo de usuario no válido."); }
               

            var usuario = await userQueries[loginDTO.tipoUsuario.ToLower()]();

            if (usuario != null && _passwordService.VerifyHashedPassword(usuario.Password, loginDTO.Password))
            {
                return new UserDTO
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Role = char.ToUpper(loginDTO.tipoUsuario[0]) + loginDTO.tipoUsuario.Substring(1).ToLower()
                };
            }

            return null;
        }

        private async Task<Paciente> GetPacienteByDni(string dni) => await _pacienteRepository.GetPacienteByDNI(dni);


    }

}
