using SmileDental.Models;

namespace SmileDental.Services.Interfaces
{
    public interface IInfo
    {
        Task<List<Especialidad>> verEspecialidades();
        Task<List<string>> verUrlFotosDentistasEspecialidad();
    }
}
