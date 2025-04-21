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
        Task<IEnumerable<Cita>> GetCitasByOdontologoIdAsync(int odontologoId);
        Task<IEnumerable<Cita>> GetCitasByFechaAsync(DateTime fecha);

        
    }
}
