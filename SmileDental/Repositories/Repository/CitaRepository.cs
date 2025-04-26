using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SmileDental.DTOs.Dentista;
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
            return Task.FromResult<IEnumerable<Cita>>(_context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .Where(c => c.PacienteId == pacienteId)
                .ToList());
        }
        public async Task<IEnumerable<Cita>> GetCitasByOdontologoIdAsync(int odontologoId, int pagina)
        {
            int citasPorPagina = 10;
            List<Cita> citas = await _context.Citas
                .Where(c => c.DentistaId == odontologoId)
                .OrderByDescending(c => c.Fecha)
                .Skip((pagina - 1) * citasPorPagina)
                .Take(citasPorPagina)
                .ToListAsync();

            return citas;
        }

        public Task<IEnumerable<Cita>> GetCitasByFechaAsync(DateTime fecha)
        {
            var citas = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .Where(c => c.Fecha.Date == fecha.Date)
                .ToList();

            return Task.FromResult<IEnumerable<Cita>>(citas);
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

        public async Task<IEnumerable<Cita>> GetCitasByFechaAndDentistaId(DateTime fecha, int dentistaId)
        {
            var citas = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .Where(c => c.Fecha.Date == fecha.Date && c.DentistaId == dentistaId)
                .ToListAsync();
            return citas;
        }

        public async Task<IEnumerable<CitaPacienteDTO>> GetCitasByPacienteDNI(string dniPaciente)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Dni == dniPaciente) ?? throw new Exception("Paciente no encontrado");
            int idPaciente = paciente.Id;
            var citas =  await GetCitasByPacienteIdAsync(idPaciente);
            List<CitaPacienteDTO> citasDTO = new ();
            foreach (var cita in citas)
            {
                citasDTO.Add(new CitaPacienteDTO
                {
                    citaId = cita.Id,
                    fecha = cita.Fecha,
                    hora = cita.Hora,
                    nombreInteresado = await _context.Dentistas.Where(d => d.Id == cita.DentistaId).Select(d => d.Nombre).FirstOrDefaultAsync(),
                    apellidoInteresado = await _context.Dentistas.Where(d => d.Id == cita.DentistaId).Select(d => d.Apellido).FirstOrDefaultAsync(),
                    urlCita = cita.URLCita
                });          
            }
            return citasDTO;
        }

        public async Task<IEnumerable<Cita>> GetCitasByOdontologoIdAndFechaAsync(int odontologoId, DateTime fecha)
        {
            var citas = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .Where(c => c.DentistaId == odontologoId && c.Fecha.Date == fecha.Date)
                .ToListAsync();
            return citas;
        }
    }
}
