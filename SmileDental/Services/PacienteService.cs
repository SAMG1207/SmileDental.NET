using Microsoft.EntityFrameworkCore;
using SmileDental.DTOs;
using SmileDental.DTOs.Cita;
using SmileDental.Models;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;

namespace SmileDental.Services
{
    //ESTE CONTROLADOR DEBE ESTAR RESTRINGIDO POR EL TOKEN Y SOLO A LOS PACIENTES
    public class PacienteService : IPacienteInterface
    {
        private readonly ApiDbContext _context;
        
        private readonly PasswordManager _passwordService;

        public PacienteService(ApiDbContext context, PasswordManager passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<List<Paciente>> GetPacientes()
        {
            return await _context.Pacientes.ToListAsync();
        }

        public async Task<int?> GetDentistIdAvailable(DateTime fecha, int hora)
        {
            // Obtener las citas que coinciden con la fecha y la hora
            var citasGeneral = await _context.Citas
                .Where(c => c.Fecha == fecha && c.Hora == hora)
                .ToListAsync();

            // Obtener los dentistas
            var dentistas = await _context.Dentistas
                .ToListAsync(); // Obtener todos los dentistas

            // Filtrar los dentistas que no están ocupados en esa hora
            var dentistasLibres = dentistas
                .Where(d => !citasGeneral.Any(c => c.DentistaId == d.Id && hora >= d.HoraEntrada && hora < d.HoraSalida))
                .ToList();

            // Si no hay dentistas libres, retornar null
            if (!dentistasLibres.Any())
            {
                return null;
            }

            // Seleccionar aleatoriamente un dentista disponible
            return dentistasLibres[new Random().Next(dentistasLibres.Count)].Id;
        }

        private async Task<List<int>> DentistasGeneralesId() 
        {
            try
            {
                List<int> dentistasGeneralesIds = await _context.Dentistas.
                                                    Where(d => d.EspecialidadId == 6)
                                                   .Select(d => d.Id)
                                                   .ToListAsync();
                return dentistasGeneralesIds;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

        }
        public async Task<List<int>> HorasDisponibles(DateTime fecha)
        {
            // Obtener todos los dentistas generales
            List<int> dentistasGeneralesIds = await DentistasGeneralesId();

            // Obtener las horas ocupadas de cada dentista en la fecha indicada
            var horasOcupadas = await _context.Citas
                .Where(c => dentistasGeneralesIds.Contains(c.DentistaId) && c.Fecha == fecha)
                .GroupBy(c => c.DentistaId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(c => c.Hora).ToList());

            // Obtener las horas de trabajo de cada dentista
            var horasDeTrabajo = await _context.Dentistas
                .Where(d => dentistasGeneralesIds.Contains(d.Id))
                .Select(d => new { d.Id, d.HoraEntrada, d.HoraSalida })
                .ToListAsync();

            // Crear un HashSet para almacenar las horas disponibles, evitando duplicados
            HashSet<int> horasDisponibles = new HashSet<int>();

            foreach (var dentista in horasDeTrabajo)
            {
                // Obtener las horas ocupadas por el dentista en la fecha indicada
                var horasOcupadasDentista = horasOcupadas.ContainsKey(dentista.Id) ? horasOcupadas[dentista.Id] : new List<int>();

                // Generar un rango de horas de trabajo para el dentista
                var horasDeTrabajoDentista = Enumerable.Range(dentista.HoraEntrada, dentista.HoraSalida - dentista.HoraEntrada).ToList();

                // Obtener las horas disponibles (horas de trabajo menos las horas ocupadas)
                var horasDisponiblesDentista = horasDeTrabajoDentista.Except(horasOcupadasDentista).ToList();

                // Añadir las horas disponibles al HashSet
                foreach (var hora in horasDisponiblesDentista)
                {
                    horasDisponibles.Add(hora);
                }
            }


            // Convertir el HashSet a una lista ordenada antes de devolver
            return horasDisponibles.OrderBy(h => h).ToList();

        }

        private async Task<bool>PacienteEstaRegistrado(int pacienteId)
        {
            var paciente = await _context.Pacientes.FindAsync(pacienteId);
            return paciente != null;
        }


        public async Task<bool> CrearCita(CitaGeneralDTO citaGeneralDTO)
        {
            int pacienteId = citaGeneralDTO.PacienteId;
            DateTime fecha = citaGeneralDTO.Fecha;
            int hora = citaGeneralDTO.Hora;

            //Añadir la cita a la base de datos

            try
            {
                if(await CitaFutura(pacienteId))
                {
                    throw new ArgumentException("El paciente ya tiene una cita futura.");
                }
                int ? dentistId = await GetDentistIdAvailable( fecha,hora);
                if (dentistId == null)
                {
                    throw new ArgumentException("No hay dentistas disponibles en la fecha y hora seleccionadas.");
                }
                var cita = new Cita
                {
                    PacienteId = pacienteId,
                    DentistaId = dentistId.Value,
                    Fecha = fecha,
                    Hora = hora
                };

                await _context.Citas.AddAsync(cita);
                await _context.SaveChangesAsync();
                return true;
            }catch(Exception e)
            {
                throw new ArgumentException(e.Message);

            }
        }

        public async Task<bool> CancelarCita(int citaId)
        {
            try
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
            catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }


        }

        public async Task<List<Cita>> VerCitas(int pacienteId)
        {
            try
            {
                bool pacienteExiste = await PacienteEstaRegistrado(pacienteId);
                if (!pacienteExiste)
                {
                    throw new ArgumentException("El paciente no existe.");
                }
                var citas = await _context.Citas
                .Where(c => c.PacienteId == pacienteId)
                .ToListAsync();

                if(citas == null)
                {
                    throw new ArgumentException("No hay citas para el paciente.");
                }
                return citas;
            }
            catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }

        }

        public async Task<bool> CitaFutura(int pacienteId)
        {
            var paciente = await _context.Pacientes.FindAsync(pacienteId);
            if(paciente == null)
            {
                throw new ArgumentException("El paciente no existe.");
            }
            return await _context.Citas
                .AnyAsync(c => c.PacienteId == pacienteId && c.Fecha > DateTime.Now);
        }

        public Task<bool> CambiarEmail(EmailDTO emailDto)
        {
            try
            {
                var paciente = _context.Pacientes.Find(emailDto.PacienteId);
                if (paciente == null)
                {
                    throw new ArgumentException("El paciente no existe.");
                }
                paciente.Email = emailDto.Email;
                _context.Pacientes.Update(paciente);
                _context.SaveChanges();
                return Task.FromResult(true);
            }catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }

        }

        public Task<bool> CambiarPassword(PasswordDTO passwordDto)
        {
            try
            {
                var paciente = _context.Pacientes.Find(passwordDto.Id);
                if (paciente == null)
                {
                    throw new ArgumentException("El paciente no existe.");
                }
                paciente.Password = _passwordService.HashPassword(passwordDto.Password);
                _context.Pacientes.Update(paciente);
                _context.SaveChanges();
                return Task.FromResult(true);
            }catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public Task<bool> CambiarTelefono(TelefonoDTO telefonoDto)
        {
            try
            {
                var paciente = _context.Pacientes.Find(telefonoDto.PacienteId);
                if (paciente == null)
                {
                    throw new ArgumentException("El paciente no existe.");
                }
                if(!StringManager.validaTelefono(telefonoDto.Telefono))
                {
                    throw new ArgumentException("El teléfono ingresado no es válido.");
                }
                paciente.Telefono = telefonoDto.Telefono;
                _context.Pacientes.Update(paciente);
                _context.SaveChanges();
                return Task.FromResult(true);
            }catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }

        }

        public Task<bool> CambiarDni(DniDTO dniDto)
        {
            try
            {
                var paciente = _context.Pacientes.Find(dniDto.Id);
                if (paciente == null)
                {
                    throw new ArgumentException("El paciente no existe.");
                }
                if (!StringManager.validaDni(dniDto.Dni))
                {
                    throw new ArgumentException("El DNI ingresado no es válido.");
                }
                paciente.Dni = dniDto.Dni;
                _context.Pacientes.Update(paciente);
                _context.SaveChanges();
                return Task.FromResult(true);
            }catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

   
    }
}
