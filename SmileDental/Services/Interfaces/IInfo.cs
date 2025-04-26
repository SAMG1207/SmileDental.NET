using SmileDental.DTOs.Administrador;
using SmileDental.Models;

namespace SmileDental.Services.Interfaces
{
    public interface IInfo
    {
        Task<IEnumerable<string>> VerEspecialidades();
        Task<IEnumerable<PresentacionDentistaDTO>> VerUrlFotosDentistasEspecialidad();
    }
}
