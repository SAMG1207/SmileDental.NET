using Microsoft.AspNetCore.Mvc;
using SmileDental.DTOs;

namespace SmileDental.Services.Interfaces
{
    public interface IAuthInterface
    {


        Task<UserDTO?> ValidarUsuario([FromBody] LoginDTO loginDTO);
        Task<bool> RegisterPaciente(DatosPersonalesDTO datosPersonlesDto);

    }
}
