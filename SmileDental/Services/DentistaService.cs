using SmileDental.DTOs;
using SmileDental.Services.Interfaces;

namespace SmileDental.Services
{
    public class DentistaService : IDentistInterface
    {
        public Task<bool> Login(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Register(CrearPacienteDTO crearUser)
        {
            throw new NotImplementedException();
        }
    }
}
