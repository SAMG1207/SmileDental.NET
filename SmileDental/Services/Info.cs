using Microsoft.EntityFrameworkCore;
using SmileDental.DTOs.Administrador;
using SmileDental.Models;
using SmileDental.Repositories.Repository;
using SmileDental.Services.Interfaces;

namespace SmileDental.Services
{
    public class Info(DentistaRepository dentistaRepository, EspecialidadesRepository especialidadesRepository) : IInfo
    {
        private readonly DentistaRepository _dentistaRepository = dentistaRepository;
        private readonly EspecialidadesRepository _especialidadesRepository = especialidadesRepository;

        public async Task<IEnumerable<string>> VerEspecialidades()
        {
           return (IEnumerable<string>)await _especialidadesRepository.GetEspecialidades();
        }

        public async Task<IEnumerable<PresentacionDentistaDTO>> VerUrlFotosDentistasEspecialidad()
        {
           return await _dentistaRepository.GetPresentacionDentistasNoGenerales();
        }
    }
}
