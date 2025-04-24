using Microsoft.EntityFrameworkCore;
using SmileDental.Builders;
using SmileDental.DTOs;
using SmileDental.DTOs.Cita;
using SmileDental.Models;
using SmileDental.Repositories.Interfaces;
using SmileDental.Repositories.Repository;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Services
{
    //ESTE CONTROLADOR DEBE ESTAR RESTRINGIDO POR EL TOKEN Y SOLO A LOS PACIENTES
    public class PacienteService : IPacienteInterface, IGetNombre
    {
        private readonly PacienteRepository _pacienteRepository;

        private readonly PasswordManager _passwordService;

        private readonly CitaRepository _citaRepository;

        private readonly DentistaRepository _dentistaRepository;

        public PacienteService(PacienteRepository pacienteRepository, PasswordManager passwordService, CitaRepository citaRepository, DentistaRepository dentistaRepository)
        {
            _pacienteRepository = pacienteRepository;
            _passwordService = passwordService;
            _citaRepository = citaRepository;
            _dentistaRepository = dentistaRepository;
        }

        public async Task<List<int>> HorasDisponibles(DateTime fecha)
        {
                          
            return (List<int>) await _citaRepository.GetHorariosDisponiblesPorFecha(fecha);

        }

        private async Task<bool> PacienteEstaRegistrado(int pacienteId)
        {
            var paciente = await _pacienteRepository.GetPacienteById(pacienteId);
            return paciente != null;
        }


        public async Task<bool> CrearCita(CitaGeneralDTO citaGeneralDTO)
        {
            int pacienteId = citaGeneralDTO.PacienteId;
            DateTime fecha = citaGeneralDTO.Fecha;
            int hora = citaGeneralDTO.Hora;

                if (await CitaFutura(pacienteId))
                {
                    throw new ArgumentException("El paciente ya tiene una cita futura.");
                }
                int? dentistId = 
                    await GetDentistIdAvailable(fecha, hora) ?? 
                    throw new ArgumentException("No hay dentistas disponibles en la fecha y hora seleccionadas.");

                var cita = new CitaBuilder()
                    .WithPacienteId(pacienteId)
                    .WithDentistaId((int)dentistId)
                    .WithFecha(fecha)
                    .WithHora(hora)
                    .Build();

                await _context.Citas.AddAsync(cita);
                await _context.SaveChangesAsync();
                return true;

        }

        public async Task<bool> CancelarCita(int citaId)
        {

                var cita = _context.Citas.Find(citaId);
                if (cita == null)
                {
                    throw new ArgumentException("La cita no existe.");
                }
                _context.Remove(cita);
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<List<Cita>> VerCitas(int pacienteId)
        {
                bool pacienteExiste = await PacienteEstaRegistrado(pacienteId);
                if (!pacienteExiste)
                {
                    throw new ArgumentException("El paciente no existe.");
                }
                var citas = 
                    await _context.Citas.Where(c => c.PacienteId == pacienteId).ToListAsync() ??
                    throw new ArgumentException("No hay citas para el paciente."); 

                return citas;

        }

        public async Task<bool> CitaFutura(int pacienteId)
        {
            var paciente = await _pacienteRepository.GetPacienteById(pacienteId);
            return await _citaRepository.GetCitaByPacienteId(pacienteId);
        }

        private async Task<bool> ActualizarCampoPacienteAsync(int pacienteId, Action<Paciente> modificar)
        {
            var paciente = await _pacienteRepository.GetPacienteById(pacienteId);

            modificar(paciente); // Ejecuta la acción (cambiar email, dni, etc.)

            return await _pacienteRepository.UpdatePaciente(paciente);
        }


        public async Task<bool> CambiarEmail(EmailDTO emailDto)
        {
            return await ActualizarCampoPacienteAsync(emailDto.PacienteId, p => p.SetEmail(emailDto.Email));
        }

        public async Task<bool> CambiarPassword(PasswordDTO passwordDto )
        {
            return await ActualizarCampoPacienteAsync(passwordDto.Id, p => p.SetPassword(_passwordService.HashPassword(passwordDto.Password)));  
  
        }

        public async Task<bool> CambiarTelefono(TelefonoDTO telefonoDto)
        {
            return await ActualizarCampoPacienteAsync(telefonoDto.PacienteId, p => p.SetTelefono(telefonoDto.Telefono));
        }

        public async Task<bool> CambiarDni(DniDTO dniDto)
        {
            return await ActualizarCampoPacienteAsync(dniDto.Id, p => p.SetDni(dniDto.Dni));
        }

        public async Task<string> GetNombre(int id)
        {
            return await _pacienteRepository.GetPacienteName(id);
        }
    }
}
