using SmileDental.Models;

namespace SmileDental.Repositories.Interfaces
{
    public interface IEspecialidadesRepository
    {
        Task<IEnumerable<Especialidad>> GetEspecialidades();
    }
}
