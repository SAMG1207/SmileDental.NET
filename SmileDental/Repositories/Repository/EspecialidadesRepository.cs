using Microsoft.EntityFrameworkCore;
using SmileDental.Models;
using SmileDental.Repositories.Interfaces;

namespace SmileDental.Repositories.Repository
{
    public class EspecialidadesRepository : IEspecialidadesRepository
    {
        private readonly ApiDbContext _context;
        public EspecialidadesRepository(ApiDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Especialidad>> GetEspecialidades()
        {
            return await _context.Especialidades.ToListAsync();
        }
    }
  
}
