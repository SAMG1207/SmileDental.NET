using Microsoft.EntityFrameworkCore;
using Serilog;
using SmileDental.DTOs;
using SmileDental.DTOs.Cita;
using SmileDental.DTOs.Dentista;
using SmileDental.Models;
using SmileDental.Repositories.Repository;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Services
{
    public class DentistaService
        (
          DentistaRepository dentistaRepository
        , CitaRepository citaRepository
        , PacienteRepository pacienteRepository
        , IActionLog actionLogger
        ) 
        : IDentistInterface, IGetNombre
     {
        
        private readonly IActionLog _actionLogger = actionLogger;
        private readonly DentistaRepository _dentistaRepository = dentistaRepository;
        private readonly CitaRepository _citaRepository = citaRepository;
        private readonly PacienteRepository _pacienteRepository = pacienteRepository;

        public async Task<string> GetNombre(int id)
        {
          return await _dentistaRepository.GetNombreDentistaPorId(id);
        }

        public Task<int> NumeroDePaginas(int dentistaId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PacienteEstaRegistrado(string dniPaciente)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SubirInformeCita(SubirCitaDTO subirCitaDTO)
        {

            var file = subirCitaDTO.file;
            var result = await FileHandler.UploadFile(file);

            if (result.Success)
            {
                var cita = await _citaRepository.GetCitaByIdAsync(subirCitaDTO.citaId);
                cita.SetURLCita(result.FileName);
                await _citaRepository.UpdateCitaAsync(cita);

                /*
                int idDentista = await _context.Citas.Where(c => c.Id == subirCitaDTO.citaId)
                    .Select(c => c.DentistaId)
                    .FirstOrDefaultAsync();
                //await _actionLogger.LogearAccion(idDentista, "Dentista", $"El dentista con id: {idDentista} ha subido el informe :{subirCitaDTO.citaId}");

                await _context.SaveChangesAsync();
                */
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Cita>> VerCitas(int dentistaId, int pagina)
        {
           var citas = await _citaRepository.GetCitasByOdontologoIdAsync(dentistaId, pagina);
           return citas;
        }

        public async Task<List<CitaPacienteDTO>> verCitasPaciente(string dniPaciente)
        {
          
                bool DniValido = StringManager.ValidaDni(dniPaciente);
                if (!DniValido)
                {
                    throw new ArgumentException("DNI no válido", nameof(dniPaciente));
                }

                return (List<CitaPacienteDTO>)await _citaRepository.GetCitasByPacienteDNI(dniPaciente);
        }

        public async Task<List<CitaPacienteDTO>> VerCitasPorFecha(int dentistaId, DateTime fecha)
        {
            var citas = await _citaRepository.GetCitasByFechaAndDentistaId(fecha, dentistaId) ??
                throw new ArgumentException("No hay citas para esa fecha");
            List<CitaPacienteDTO> citasDTO = new List<CitaPacienteDTO>();
            foreach (var cita in citas)
            {
                citasDTO.Add(new CitaPacienteDTO
                {
                    citaId = cita.Id,
                    nombreInteresado = cita.Paciente.Nombre,
                    apellidoInteresado = cita.Paciente.Apellido,
                    fecha = cita.Fecha,
                    hora = cita.Hora,
                    urlCita = cita.URLCita
                });
            }
            return citasDTO;
        }
    }
}
