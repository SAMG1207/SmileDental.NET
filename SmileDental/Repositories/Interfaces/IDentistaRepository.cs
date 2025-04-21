using SmileDental.Models;

namespace SmileDental.Repositories.Interfaces
{
    public interface IDentistaRepository
    {
        Task<IEnumerable<Dentista>> GetAllAsync();
        Task<Dentista> GetByIdAsync(int id);
        Task AddAsync(Dentista dentista);
        Task UpdateAsync(Dentista dentista);
        Task DeleteAsync(int id);

        Task <bool> DesactivarDentista(int id);

        Task<bool> ActivarDentista(int id);

        Task<IEnumerable<int>> GetDentistasGeneralesIds();

        Task<IEnumerable<int>> GetDentistasEspecialistasIds();

        Task<IEnumerable<int>> GetDisponibilidadDeDentistasGeneralesPorFechaYHora( DateTime fecha, int hora);

        Task<IEnumerable<int>> GetHorasDisponiblesPorFecha(DateTime fecha);


    }
}
