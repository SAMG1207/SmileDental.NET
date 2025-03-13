using SmileDental.DTOs;
using SmileDental.DTOs.Cita;
using SmileDental.Models;

namespace SmileDental.Services.Interfaces
{
    public interface IPacienteInterface
    {
        Task<int?> GetDentistIdAvailable(DateTime fecha, int hora);
        Task<bool> CrearCita(CitaGeneralDTO citaGeneralDTO);
        Task<bool> CancelarCita(int citaId);
        Task<List<Cita>> VerCitas(int pacienteId);
        Task<List<int>> HorasDisponibles(DateTime fecha);
        Task<bool> CambiarEmail(EmailDTO emailDto);
        Task<bool> CambiarPassword(PasswordDTO passwordDto);
        Task<bool> CambiarTelefono(TelefonoDTO telefonoDto);
        Task<bool> CambiarDni(DniDTO dniDto);
        Task<bool> CitaFutura(int pacienteId);
    }
}
