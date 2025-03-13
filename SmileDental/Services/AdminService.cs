using SmileDental.DTOs;
using SmileDental.Services.Interfaces;

namespace SmileDental.Services
{
    public class AdminService : IAdminInterface
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
