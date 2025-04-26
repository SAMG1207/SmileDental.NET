using SmileDental.DTOs.Cita;
using SmileDental.DTOs.Dentista;
using SmileDental.Models;

namespace SmileDental.Services.Interfaces
{
    public interface IDentistInterface
    {
        Task<bool> PacienteEstaRegistrado(string dniPaciente);
        Task<IEnumerable<Cita>> VerCitas(int dentistaId, int pagina);

        Task<int> NumeroDePaginas(int dentistaId);
        Task<bool> SubirInformeCita(SubirCitaDTO subirCitaDTO);
        Task<List<CitaPacienteDTO>> verCitasPaciente(string dniPaciente);

        Task<List<CitaPacienteDTO>> VerCitasPorFecha(int dentistaId, DateTime fecha);
    }
}
