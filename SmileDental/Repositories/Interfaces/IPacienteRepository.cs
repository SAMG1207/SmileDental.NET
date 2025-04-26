using SmileDental.Models;

namespace SmileDental.Repositories.Interfaces
{
    public interface IPacienteRepository
    {
        Task<List<Paciente>> GetPacientes();
        Task<Paciente?> GetPacienteById(int id);
        Task<bool> UpdatePaciente(Paciente paciente);
        Task<bool> DeletePaciente(int id);
        Task<bool> CreatePaciente(Paciente paciente);

        Task<Paciente> GetPacienteByDNI(string dni);
        Task<Paciente> GetPacienteByEmail(string email);
        Task<string> GetPacienteName(int id);
    }
}
