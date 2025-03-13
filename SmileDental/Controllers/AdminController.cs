using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileDental.DTOs.Dentista;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Controllers
{
    //SOLO SE PUEDE ENTRAR SI HAY UN TOKEN VALIDO PARA ADMIN
    // TODOS LOS ENDOPOINTS DEBEN ESTAR PROTEGIDOS EXCEPTO EL DE LOGIN
    public class AdminController : ControllerBase
    {
        private readonly JWTHandler _jwtHandler;
        private readonly IAdminInterface _adminInterface;
        public AdminController(JWTHandler jwtHandler, IAdminInterface adminInterface)
        {
            _jwtHandler = jwtHandler;
            _adminInterface = adminInterface;
        }

        /*
        [HttpPost("register-dentista")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RegistrarDentista([FromBody] CrearDentistaDTO crearDentistaDTO)
        {
            try
            {
                await _authInterface.RegisterDentista(crearDentistaDTO);
                return Ok(new { message = "Dentista registrado exitosamente" });
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        */
    }
}
