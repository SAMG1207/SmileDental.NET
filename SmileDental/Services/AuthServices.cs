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

            var paciente = new Paciente
            {
                Dni = crearUser.Dni,
                FechaNacimiento = crearUser.FechaDeNacimiento,
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
            var userQueries = new Dictionary<string, Func<Task<dynamic>>>
    {
        { "paciente", async () => await _context.Pacientes.FirstOrDefaultAsync(p => p.Email == loginDTO.Email) },
        { "dentista", async () => await _context.Dentistas.FirstOrDefaultAsync(d => d.Email == loginDTO.Email) },
        { "administrador", async () => await _context.Dentistas.FirstOrDefaultAsync(d => d.Email == loginDTO.Email && d.EsAdmin) }
    };

            if (!userQueries.ContainsKey(loginDTO.tipoUsuario.ToLower()))
                return null;

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

        private async Task<Paciente> GetPacienteByDni(string dni) => await _context.Pacientes.FirstOrDefaultAsync(p => p.Dni == dni);


    }

}
