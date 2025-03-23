using Microsoft.AspNetCore.Mvc;
using SmileDental.DTOs;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JWTHandler _jwtHandler;
        private readonly IAuthInterface _authInterface;

        public AuthController(ILogger<AuthController> logger, JWTHandler jwtHandler, IAuthInterface authInterface)
        {
            _logger = logger;
            _jwtHandler = jwtHandler;
            _authInterface = authInterface;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            _logger.LogInformation("Login de usuario: " + loginDTO.Email);
            var user = await _authInterface.ValidarUsuario(loginDTO);

            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales equivocados" });
            }

            string token = _jwtHandler.GenerarToken(user.Id, user.Email, user.Role);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registro([FromBody] CrearPacienteDTO crearPacienteDTO)
        {
            try
            {
                var paciente = await _authInterface.RegisterPaciente(crearPacienteDTO);
                return Ok(paciente);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }


    }

}
