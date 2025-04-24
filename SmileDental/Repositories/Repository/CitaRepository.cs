using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SmileDental.Models;
using SmileDental.Repositories.Interfaces;

namespace SmileDental.Repositories.Repository
{
    public class CitaRepository(ApiDbContext context) : ICitaRepository
    {
        private readonly ApiDbContext _context = context;

        public async Task<IEnumerable<Cita>> GetCitasAsync()
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .ToListAsync();
        }
        public async Task<Cita> GetCitaByIdAsync(int id)
        {
            var cita = await _context.Citas
            .Include(c => c.Paciente)
             .Include(c => c.Dentista)
             .FirstOrDefaultAsync(c => c.Id == id);

            return cita ?? throw new Exception("No existe esta cita");
        }
        public async Task AddCitaAsync(Cita cita)
        {
             await _context.Citas.AddAsync(cita);
             await _context.SaveChangesAsync(); 
        }
        public async Task UpdateCitaAsync(Cita cita)
        {
            if(await _context.Citas.AnyAsync(c => c.Id == cita.Id))
            {
                _context.Citas.Update(cita);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Cita no registrada");
            }
        }
        public async Task DeleteCitaAsync(int id)
        {
            var cita = await _context.Citas.FirstOrDefaultAsync(c => c.Id == id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Cita no registrada");
            }
        }
        public Task<IEnumerable<Cita>> GetCitasByPacienteIdAsync(int pacienteId)
        {
            var citas = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .Where(c => c.PacienteId == pacienteId)
                .ToListAsync();

            return citas.ContinueWith(task => (IEnumerable<Cita>)task.Result);
        }
        public Task<IEnumerable<Cita>> GetCitasByOdontologoIdAsync(int odontologoId)
        {
           var citas = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .Where(c => c.DentistaId == odontologoId)
                .ToListAsync();
            
            return citas.ContinueWith(task => (IEnumerable<Cita>)task.Result);
        }
        public Task<IEnumerable<Cita>> GetCitasByFechaAsync(DateTime fecha)
        {
            var citas = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .Where(c => c.Fecha.Date == fecha.Date)
                .ToListAsync();
            return citas.ContinueWith(task => (IEnumerable<Cita>)task.Result) ?? throw new Exception("No existen citas en esta fecha");
        }

        public async Task<IEnumerable<int>> GetHorariosDisponiblesPorFecha(DateTime fecha)
        {
           return await _context.GetHorasLibresPorFecha(fecha);
        }

        public async Task<bool> GetCitaByPacienteId(int pacienteId)
        {
            // no se hace doble comprobacion del pacienteid
            return await _context.Citas
                .AnyAsync(c => c.PacienteId == pacienteId && c.Fecha > DateTime.Now);
        }
    }
}
