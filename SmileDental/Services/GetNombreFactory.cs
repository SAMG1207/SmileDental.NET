using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Services
{
    public class GetNombreFactory : IGetNombreFactory
    {
        private readonly AdminService _adminService;
        private readonly PacienteService _pacienteService;
        private readonly DentistaService _dentistaService;

        public GetNombreFactory(AdminService adminService, PacienteService pacienteService, DentistaService dentistaService)
        {
            _adminService = adminService;
            _pacienteService = pacienteService;
            _dentistaService = dentistaService;
        }
        public IGetNombre GetServicio(EnumeradorUsuarios usuarios)
        {
            return usuarios switch
            {
                EnumeradorUsuarios.Administrador => _adminService,
                EnumeradorUsuarios.Dentista => _dentistaService,
                EnumeradorUsuarios.Paciente => _pacienteService,
                _ => throw new ArgumentException("Tipo de usuario no válido")
            };
        }
    }

}
