using SmileDental.DTOs.Dentista;
using SmileDental.Models;

namespace SmileDental.Repositories.Interfaces
{
    public interface ICitaRepository
    {
        Task<IEnumerable<Cita>> GetCitasAsync();
        Task<Cita> GetCitaByIdAsync(int id);
        Task AddCitaAsync(Cita cita);
        Task UpdateCitaAsync(Cita cita);
        Task DeleteCitaAsync(int id);
        Task<IEnumerable<Cita>> GetCitasByPacienteIdAsync(int pacienteId);
        Task<IEnumerable<Cita>> GetCitasByOdontologoIdAsync(int odontologoId, int pagina);
        Task<IEnumerable<Cita>> GetCitasByFechaAsync(DateTime fecha);

        Task<IEnumerable<Cita>> GetCitasByOdontologoIdAndFechaAsync(int odontologoId, DateTime fecha);

        Task<IEnumerable<CitaPacienteDTO>> GetCitasByPacienteDNI(string dniPaciente);
        Task<IEnumerable<int>> GetHorariosDisponiblesPorFecha(DateTime fecha);

        Task<bool> GetCitaByPacienteId(int pacienteId);

        Task<IEnumerable<Cita>> GetCitasByFechaAndDentistaId(DateTime fecha, int dentistaId);
    }
}
