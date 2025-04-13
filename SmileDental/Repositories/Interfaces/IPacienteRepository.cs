using SmileDental.Models;

namespace SmileDental.Repositories.Interfaces
{
    public interface IPacienteRepository
    {
        Task<List<Paciente>> GetPacientes();
        Task<List<Cita>> GetCitasByPacienteId(int pacienteId, Paciente paciente);
        Task<Paciente> GetPacienteById(int id);
        Task<bool> UpdatePacientePassword(int pacienteid, string password);
        Task<bool> UpdatePaciente(int pacienteId, Paciente paciente);
        Task<bool> DeletePaciente(int id);
        Task<bool> CreatePaciente(Paciente paciente);
    }
}
