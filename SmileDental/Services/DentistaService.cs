using Microsoft.EntityFrameworkCore;
using Serilog;
using SmileDental.DTOs;
using SmileDental.DTOs.Cita;
using SmileDental.DTOs.Dentista;
using SmileDental.Models;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Services
{
    public class DentistaService : IDentistInterface
    {
        private readonly ApiDbContext _context;

        public DentistaService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PacienteEstaRegistrado(string dniPaciente)
        {
            try
            {
                bool dniValido = StringManager.validaDni(dniPaciente);
                if (!dniValido)
                {
                    return false;
                }
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Dni == dniPaciente);
                return paciente != null;
            }
            catch(Exception e)
            {
                return false;
            }
        }


        public async Task<bool> SubirInformeCita(SubirCitaDTO subirCitaDTO)
        {

            var file = subirCitaDTO.file;
            var result = await FileHandler.UploadFile(file);

            if (result.Success)
            {
                // Aquí actualizaríamos la base de datos con result.FileName
                var cita = await _context.Citas.FindAsync(subirCitaDTO.citaId);
                cita.URLCita = result.FileName;

                await _context.SaveChangesAsync();
                return true;
            }

            Log.Error($"Error al subir archivo: {result.Message}");
            return false;
        }

        public async Task<List<Cita>> VerCitas(int dentistaId)
        {
            try
            {
                List<Cita> citas = await _context.Citas.Where(c => c.DentistaId == dentistaId).ToListAsync();
                return citas;
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener las citas del dentista {dentistaId}: {e.Message}");
                return new List<Cita>();
            }
        }

        public async Task<List<CitaPacienteDTO>> verCitasPaciente(string dniPaciente)
        {
            try
            {
                bool DniValido = StringManager.validaDni(dniPaciente);
                if (!DniValido)
                {
                    throw new ArgumentException("DNI no válido", nameof(dniPaciente));
                }

                var citas = await _context.Citas
                    .Where(c => c.Paciente.Dni == dniPaciente).ToListAsync();

                var citasDTO = new List<CitaPacienteDTO>();

                foreach(var cita in citas)
                {
                    citasDTO.Add(new CitaPacienteDTO
                    {
                        citaId = cita.Id,
                        fecha = cita.Fecha,
                        hora = cita.Hora,
                        nombreDentista = await _context.Dentistas.Where(d => d.Id == cita.DentistaId).Select(d => d.Nombre).FirstOrDefaultAsync(),
                        apellidoDentista = await _context.Dentistas.Where(d => d.Id == cita.DentistaId).Select(d => d.Apellido).FirstOrDefaultAsync(),
                        urlCita = cita.URLCita
                    });
                }

                return citasDTO;

            }
            catch(Exception e)
            {
                Log.Error($"Error al obtener las citas del paciente {dniPaciente}: {e.Message}");
                return new List<CitaPacienteDTO>();
            }
        }

    
    }
}
