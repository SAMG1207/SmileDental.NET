using SmileDental.Models;
using SmileDental.Services.Interfaces;

namespace SmileDental.Services
{
    public class Info : IInfo
    {
        public Task<List<Especialidad>> verEspecialidades()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> verUrlFotosDentistasEspecialidad()
        {
            throw new NotImplementedException();
        }
    }
}
