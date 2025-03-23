using SmileDental.DTOs.Cita;
using SmileDental.DTOs.Dentista;
using SmileDental.Models;

namespace SmileDental.Services.Interfaces
{
    public interface IDentistInterface
    {
        Task<bool> PacienteEstaRegistrado(string dniPaciente);
        Task<List<Cita>> VerCitas(int dentistaId);
        Task<bool> SubirInformeCita(SubirCitaDTO subirCitaDTO);
        Task<List<CitaPacienteDTO>> verCitasPaciente(string dniPaciente);
    }
}
