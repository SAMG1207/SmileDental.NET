using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmileDental.DTOs.Dentista;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;
using System.Security.Claims;

namespace SmileDental.Controllers
{
    //SOLO SE PUEDE ENTRAR SI HAY UN TOKEN VALIDO PARA DENTISTA
    // TODOS LOS ENDOPOINTS DEBEN ESTAR PROTEGIDOS EXCEPTO EL DE LOGIN



    [Authorize(Roles = "Dentista")]
    [ApiController]
    [Route("api/[controller]")]
    public class DentistaController : ControllerBase
    {
        //private readonly JWTHandler _jwtHandler;
        private readonly IDentistInterface _dentistInterface;
        private readonly IGetNombre _getNombre;

        public DentistaController(
            //JWTHandler jwtHandler,
            IDentistInterface dentistInterface,
            IGetNombre getNombre)
        {
           // _jwtHandler = jwtHandler;
            _dentistInterface = dentistInterface;
            _getNombre = getNombre;
        }


        [HttpGet("check-token")]
        public IActionResult CheckToken()
        {
            // Funcon para proteger la vista de la página
            return Ok(true); // Si llega aquí, significa que el usuario está autenticado y autorizado
        }

        [HttpGet("getName")]
        public async Task<IActionResult> GetNombre()
        {
            int dentistaId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            string nombre = await _getNombre.GetNombre(dentistaId);
            return Ok(new { nombreUsuario = nombre });
        }

        [HttpPost("subirInformeCita")]
        [DisableRequestSizeLimit] // NO LO SE RICK
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> SubirInformeCita(SubirCitaDTO subirCitaDTO)
        {
            /*if (!Request.Form.Files.Any())
                return Ok();

            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);

            foreach (IFormFile file in Request.Form.Files)
            {
                string fullPath = Path.Combine(pathToSave, file.FileName);
                using FileStream stream = new(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            return Ok();
            */

            if (!Request.Form.Files.Any())
            {
                Log.Warning("No se recibió ningún archivo.");
                return BadRequest("No se recibió ningún archivo.");
            }

            //IFormFile file = Request.Form.Files[0]; NO SE UTILIZA, SUGERENCIA DE COPILOT, FALTA PROBAR 

            var result = await _dentistInterface.SubirInformeCita(subirCitaDTO);

            if (result)
            {
                return Ok(new { message = "Informe subido exitosamente" });
            }
            else
            {
                return BadRequest("Error al subir el informe");
            }

        }

        
        [HttpGet("VerHistoria/{urlHistoria}")]
        public async Task<IActionResult> DescargarCita(string urlHistoria)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Historias", urlHistoria);
            if (!System.IO.File.Exists(path))
            {
                Log.Warning($"El archivo {urlHistoria} no existe.");
                return NotFound();
            }
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(path);
            return File(fileBytes, "application/pdf", urlHistoria);
        }

        [HttpGet("VerCitas")]
        public async Task<IActionResult> VerCitas()
        {
            try
            {
                var dentistaId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
                var result = await _dentistInterface.VerCitas(dentistaId);
                if(result.Count == 0)
                {
                    return NotFound(new { message = "No hay citas disponibles" });
                }
                return Ok(result);

            }
            catch (Exception e)
            {
                Log.Error($"Error al ver las citas: {e.Message}");
                return BadRequest("Error al ver las citas");
            }

        }

        [HttpGet("VerCitasFecha/{fecha}")]
        public async Task<IActionResult> VerCitasPorFecha(DateTime fecha)
        {
            int dentistaId = int.Parse(User.FindAll(ClaimTypes.NameIdentifier).First().Value);
            var results = await _dentistInterface.VerCitasPorFecha(dentistaId, fecha);
            if(results.Count == 0)
            {
                return NotFound(new { message = "No hay citas en esta fecha" });
            }
            return Ok(results);
        }

        [HttpGet("CitasPaciente/{pacienteDNI}")]
        public async Task<IActionResult> CitasPaciente(string pacienteDNI)
        {
            try
            {
                var result = await _dentistInterface.verCitasPaciente(pacienteDNI);
                return Ok(result);
            }
            catch(Exception e)
            {
                Log.Error($"Error al ver las citas del paciente: {e.Message}");
                return BadRequest("Error al ver las citas del paciente");
            }
        }

    }
}
