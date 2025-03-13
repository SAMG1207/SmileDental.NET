using Microsoft.AspNetCore.Mvc;
using SmileDental.DTOs;
using SmileDental.DTOs.Dentista;

namespace SmileDental.Services.Interfaces
{
    public interface IAuthInterface
    {
        Task<UserDTO> ValidarUsuario([FromBody] LoginDTO loginDTO);
        Task<bool> RegisterPaciente(CrearPacienteDTO crearUser);

    }
}
