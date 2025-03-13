using Microsoft.AspNetCore.Mvc;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Controllers
{
    //SOLO SE PUEDE ENTRAR SI HAY UN TOKEN VALIDO PARA DENTISTA
    // TODOS LOS ENDOPOINTS DEBEN ESTAR PROTEGIDOS EXCEPTO EL DE LOGIN
    public class DentistaController : ControllerBase    
    {
        private readonly JWTHandler _jwtHandler;
        private readonly IDentistInterface _dentistInterface;

        public DentistaController(JWTHandler jwtHandler, IDentistInterface dentistInterface)
        {
            _jwtHandler = jwtHandler;
            _dentistInterface = dentistInterface;
        }

    }
}
