using SmileDental.Models;

namespace SmileDental.Repositories.Interfaces
{
    public interface IPacienteRepository
    {
        Task<List<Paciente>> GetPacientes();
        Task<Paciente> GetPacienteById(int id);
        Task<bool> UpdatePacientePassword(int pacienteid, string password);
        Task<bool> UpdatePaciente(Paciente paciente);
        Task<bool> DeletePaciente(int id);
        Task<bool> CreatePaciente(Paciente paciente);
    }
}
