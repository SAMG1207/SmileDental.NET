using SmileDental.DTOs.Administrador;
using SmileDental.Models;

namespace SmileDental.Services.Interfaces
{
    public interface IInfo
    {
        Task<List<string>> VerEspecialidades();
        Task<List<PresentacionDentistaDTO>> VerUrlFotosDentistasEspecialidad();
    }
}
