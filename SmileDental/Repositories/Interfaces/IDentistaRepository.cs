using SmileDental.DTOs.Administrador;
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

        Task<IEnumerable<int>> GetDisponibilidadDeDentistasGeneralesPorFechaYHora(DateTime fecha, int hora);

        Task<string> GetNombreDentistaPorId(int id);

        Task<Dentista> GetDentistaByEmail(string email);

        Task<Dentista> GetAdministradorByEmail(string email);

        Task<IEnumerable<PresentacionDentistaDTO>> GetPresentacionDentistasNoGenerales();
        /*

        Task<IEnumerable<int>> GetDentistasGeneralesIds();

        Task<IEnumerable<int>> GetDentistasEspecialistasIds();

        

        Task<IEnumerable<int>> GetHorasDisponiblesPorFecha(DateTime fecha);
        */

    }
}
