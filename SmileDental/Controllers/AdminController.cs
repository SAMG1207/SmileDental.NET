using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmileDental.DTOs.Administrador;
using SmileDental.Models;
using SmileDental.Services;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Controllers
{
    //SOLO SE PUEDE ENTRAR SI HAY UN TOKEN VALIDO PARA ADMIN
    // TODOS LOS ENDOPOINTS DEBEN ESTAR PROTEGIDOS EXCEPTO EL DE LOGIN

     [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        //private readonly JWTHandler _jwtHandler;
        private readonly IAdminInterface _adminInterface;
        public AdminController(IAdminInterface adminInterface)
        {
           // _jwtHandler = jwtHandler;
            _adminInterface = adminInterface;
        }


        [HttpGet("check-token")]
        public IActionResult CheckToken()
        {
            // Funcon para proteger la vista de la página
            return Ok(true); // Si llega aquí, significa que el usuario está autenticado y autorizado
        }

        [HttpPost("FotoDentista")]
        public async Task<IActionResult> SubirFotoDentista([FromForm] SubirFotoDTO subirFotoDTO)
        {
            try
            {
                if (!Request.Form.Files.Any())
                {
                    Log.Warning("No se recibió ningún archivo.");
                    return BadRequest("No se recibió ningún archivo.");
                }

                //IFormFile file = Request.Form.Files[0];

                // Llamada a tu lógica para subir el archivo y asociarlo con el dentista.
                var result = await _adminInterface.SubirFotoEspecialista( subirFotoDTO);
               

                if (result)
                {
                    return Ok(new { message = "Foto subida exitosamente" });
                }
                else
                {
                    return BadRequest("Error al subir la foto");
                }
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);

            }
        }



        [HttpPost("register-dentista")]
        public async Task<IActionResult> RegistrarDentista([FromBody] RegistrarDentistaDTO registrarDentistaDTO)
        {
            try
            {
                await _adminInterface.RegistrarDentista(registrarDentistaDTO);
                return Ok(new { message = "Dentista registrado exitosamente" });
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("dentistas")]
        public async Task<IActionResult> VerDentistas()
        {
            var dentistas = await _adminInterface.VerDentistas();
            return Ok(dentistas);
        }

        [HttpGet("pacientes")]
        public async Task<IActionResult> VerPacientes()
        {
            var pacientes = await _adminInterface.VerPacientes();
            return Ok(pacientes);
        }

        [HttpGet("citas-pacientes/{dniPaciente}")]
        public async Task<IActionResult> VerCitasPacientes(string dniPaciente)
        {
            var citas = await _adminInterface.VerCitasPacientes(dniPaciente);
            return Ok(citas);
        }

        [HttpGet("citas-dentistas/{idDentista}")]
        public async Task<IActionResult> VerCitasDentistas(int idDentista)
        {
            var citas = await _adminInterface.VerCitasDentistas(idDentista);
            return Ok(citas);
        }

        [HttpGet("citas-fecha")]
        public async Task<IActionResult> VerCitaPorFecha([FromQuery] DateTime fechaInicial, [FromQuery] DateTime? fechaFinal = null)
        {
            var citas = await _adminInterface.VerCitaPorFecha(fechaInicial, fechaFinal ?? DateTime.UtcNow);
            return Ok(citas);
        }

        [HttpGet("citas-fecha/{fechaInicial}")]
        public async Task<IActionResult> VerCitaPorFecha([FromQuery] DateTime fechaInicial)
        {
            var citas = await _adminInterface.VerCitaPorFecha(fechaInicial);
            return Ok(citas);
        }
        
        [HttpPost("agendar-cita")]
        public async Task<IActionResult> AgendarCitaEspecialidad([FromBody] Cita cita)
        {
            try
            {
                await _adminInterface.AgendarCitaEspecialidad(cita);
                return Ok(new { message = "Cita agendada exitosamente" });
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("bajaDentista/{idDentista}")]
        public async Task<IActionResult> DarDeBajaDentista(int idDentista)
        {
            try
            {
                await _adminInterface.DarDeBajaDentista(idDentista);
                return Ok(new { message = "Dentista dado de baja exitosamente" });
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }



    }

  
}
