using Microsoft.EntityFrameworkCore;
using SmileDental.Models;
using SmileDental.Repositories.Interfaces;

namespace SmileDental.Repositories.Repository
{
    /*
     * ESTE ES EL REPOSITORIO DE PACIENTES
     * NO TIENE SOLO QUE VER CON PACIENTES
     * SINO MAS BIEN CON LAS QUERYS QUE USARA EL PACIENTE EN SU CONTROLADOR
     */
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
        public async Task<Paciente?> GetPacienteById(int id)
        {
            return await _context.Pacientes
                .Include(p => p.Citas)
                .FirstOrDefaultAsync(p => p.Id == id) ?? throw new Exception("Paciente no registrado");
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


        public async Task<bool> UpdatePaciente(Paciente paciente)
        {

            if(await _context.Pacientes.AnyAsync(p => p.Id == paciente.Id))
            {
                _context.Pacientes.Update(paciente);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<string> GetPacienteName(int id)
        {
           return await _context.Pacientes
                .Where(p => p.Id == id)
                .Select(p => p.Nombre + " " + p.Apellido)
                .FirstOrDefaultAsync() ?? throw new Exception("Paciente no registrado");
        }

        public async Task<Paciente> GetPacienteByEmail(string email)
        {
            return await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Email == email) ?? throw new Exception("Paciente no registrado");
        }

        public async Task<Paciente> GetPacienteByDNI(string dni)
        {
             return await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Dni == dni) ?? throw new Exception("Paciente no registrado");
        }
    }
}
