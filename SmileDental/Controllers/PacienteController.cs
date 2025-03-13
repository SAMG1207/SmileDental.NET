using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileDental.DTOs;
using SmileDental.DTOs.Cita;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;
using System.Security.Claims;

namespace SmileDental.Controllers
{
    //SOLO PUEDE ACCEDER EL PACIENTE
    [Authorize(Roles = "Paciente")]
    [ApiController]
    [Route("api/[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteInterface _userEventsServices;
        private readonly JWTHandler _jwtHandler;
        public PacienteController(IPacienteInterface userEventsServices, JWTHandler jWTHandler)  // Inyecta la interfaz, no la clase directamente
        {
            _userEventsServices = userEventsServices;
            _jwtHandler = jWTHandler;
        }


        [HttpGet("check-token")]
        public IActionResult CheckToken()
        {
            return Ok(true); // Si llega aquí, significa que el usuario está autenticado y autorizado
        }


        [HttpPost("CrearCita")]
        public async Task<IActionResult> CrearCita([FromBody] CitaGeneralDTO citaGeneralDTO)
        {
            var pacienteId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            citaGeneralDTO.PacienteId = pacienteId;
            var result = await _userEventsServices.CrearCita(citaGeneralDTO);
            if (result)
            {
                return Ok(new { message = "Cita creada correctamente" });

            }
            return BadRequest("Error al crear la cita");
        }

        [HttpDelete("CancelarCita/{citaId}")]
        public async Task<IActionResult> CancelarCita(int citaId)
        {
            var pacienteId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            var result = await _userEventsServices.CancelarCita(citaId);
            if (result)
            {
                return Ok("Cita cancelada correctamente");
            }
            return BadRequest("Error al cancelar la cita");
        }

        [HttpGet("VerCitas")]
        public async Task<IActionResult> VerCitas()
        {
            int pacienteId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            var result = await _userEventsServices.VerCitas(pacienteId);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Error al obtener las citas");
        }

        [HttpGet("GetHorariosDisponibles/{fecha}")]
        public async Task<IActionResult> GetHorasDisponibles(DateTime fecha)
        {
            var result = await _userEventsServices.HorasDisponibles(fecha);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Error al obtener los horarios disponibles");
        }

        [HttpPut("CambiarEmail")]
        public async Task<IActionResult> CambiarEmail([FromBody] string nuevoEmail)
        {
            var pacienteId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            var emailDto = new EmailDTO { PacienteId = pacienteId, Email = nuevoEmail };
            var result = await _userEventsServices.CambiarEmail(emailDto);
            if (result)
            {
                return Ok("Email cambiado correctamente");
            }
            return BadRequest("Error al cambiar el email");

        }

        [HttpPost("CambiarPassword")]
        public async Task<IActionResult> CambiarPassword([FromBody] string nuevoPassword)
        {
            var pacienteId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            var passwordDto = new PasswordDTO { Id = pacienteId, Password = nuevoPassword };
            var result = await _userEventsServices.CambiarPassword(passwordDto);
            if (result)
            {
                return Ok("Password cambiado correctamente");
            }
            return BadRequest("Error al cambiar el password");
        }

        [HttpPut("CambiarTelefono")]
        public async Task<IActionResult> CambiarTelefono([FromBody] string nuevoTelefono)
        {
            var pacienteId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            var TelefonoDto = new TelefonoDTO { PacienteId = pacienteId, Telefono = nuevoTelefono };
            var result = await _userEventsServices.CambiarTelefono(TelefonoDto);
            if (result)
            {
                return Ok("Password cambiado correctamente");
            }
            return BadRequest("Error al cambiar el password");
        }

        [HttpGet("CitaFutura")]
        public async Task<IActionResult> CitaFutura()
        {
            var pacienteId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            var result = await _userEventsServices.CitaFutura(pacienteId);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Error al obtener la cita futura");
        }
    }
}
