using Microsoft.EntityFrameworkCore;
using SmileDental.Models;
using SmileDental.Repositories.Interfaces;

namespace SmileDental.Repositories.Repository
{
    public class PacienteRepository(ApiDbContext context) : IPacienteRepository
    {
        private readonly ApiDbContext _context = context;

        public async Task<List<Paciente>> GetPacientes()
        {
            return await _context.Pacientes.ToListAsync();
        }
        public Task<List<Cita>> GetCitasByPacienteId(int pacienteId)
        {
            return _context.Citas
                .Where(c => c.PacienteId == pacienteId)
                .ToListAsync();
        }
        public async Task<Paciente> GetPacienteById(int id)
        {
            return await _context.Pacientes
                .Include(p => p.Citas)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public Task<bool> UpdatePaciente(int pacienteId, Paciente paciente)
        {
            // EL PASSWORD NO SE ACTUALIZA
            Paciente pacienteObtenido = _context.Pacientes.Find(pacienteId);
            if (pacienteObtenido == null)
            {
                return Task.FromResult(false);
            }
            pacienteObtenido.SetNombre(paciente.Nombre);
            pacienteObtenido.SetApellido(paciente.Apellido);
            pacienteObtenido.SetEmail(paciente.Email);
            pacienteObtenido.SetTelefono(paciente.Telefono);
            pacienteObtenido.SetFechaNacimiento(paciente.FechaNacimiento);
            _context.Pacientes.Update(pacienteObtenido);
            _context.SaveChanges();
            return Task.FromResult(true);
        }
        public async Task<bool> DeletePaciente(int id)
        {
            return await _context.Pacientes
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync() > 0;
        }
        public async Task<bool> CreatePaciente(Paciente paciente)
        {
            await _context.Pacientes.AddAsync(paciente);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<List<Cita>> GetCitasByPacienteId(int pacienteId, Paciente paciente)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePacientePassword(int pacienteid, string password)
        {
            var paciente = await _context.Pacientes.FindAsync(pacienteid);
            if (paciente == null)
            {
                return false;
            }
            paciente.SetPassword(password);
            _context.Pacientes.Update(paciente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
