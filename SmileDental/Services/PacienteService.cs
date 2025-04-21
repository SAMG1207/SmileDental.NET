using Microsoft.EntityFrameworkCore;
using SmileDental.Builders;
using SmileDental.DTOs;
using SmileDental.DTOs.Cita;
using SmileDental.Models;
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

        public PacienteService(PacienteRepository pacienteRepository, PasswordManager passwordService)
        {
            _pacienteRepository = pacienteRepository;
            _passwordService = passwordService;
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
                .Where(d => !citasGeneral.Any(c => c.DentistaId == d.Id && hora >= d.HoraEntrada && hora < d.HoraSalida && d.Activo == true && d.EsAdmin == false))
                .ToList();

            // Si no hay dentistas libres, retornar null
            if (dentistasLibres.Count == 0)
            {
                return null;
            }

            // Seleccionar aleatoriamente un dentista disponible
            return dentistasLibres[new Random().Next(dentistasLibres.Count)].Id;
        }

        private async Task<List<int>> DentistasGeneralesId()
        {

                List<int> dentistasGeneralesIds = await _context.Dentistas.
                                                    Where(d => d.EspecialidadId == 6)
                                                   .Select(d => d.Id)
                                                   .ToListAsync();
                return dentistasGeneralesIds;

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
            HashSet<int> horasDisponibles = [];

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
            return [.. horasDisponibles.OrderBy(h => h)];

        }

        private async Task<bool> PacienteEstaRegistrado(int pacienteId)
        {
            var paciente = await _context.Pacientes.FindAsync(pacienteId);
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
            var paciente = await _context.Pacientes.FindAsync(pacienteId) ?? throw new ArgumentException("El paciente no existe.");
            return await _context.Citas
                .AnyAsync(c => c.PacienteId == pacienteId && c.Fecha > DateTime.Now);
        }

        public Task<bool> CambiarEmail(EmailDTO emailDto)
        {
                var paciente = _context.Pacientes.Find(emailDto.PacienteId) ?? throw new ArgumentException("El paciente no existe.");
                paciente.SetEmail(emailDto.Email);
                //paciente.Email = emailDto.Email;
                _context.Pacientes.Update(paciente);
                _context.SaveChanges();
                return Task.FromResult(true);
        }

        public Task<bool> CambiarPassword(PasswordDTO passwordDto)
        {
                var paciente = _context.Pacientes.Find(passwordDto.Id) ?? throw new ArgumentException("El paciente no existe.");
                paciente.SetPassword(_passwordService.HashPassword(passwordDto.Password));
                //paciente.Password = _passwordService.HashPassword(passwordDto.Password);
                _context.Pacientes.Update(paciente);
                _context.SaveChanges();
                return Task.FromResult(true);

        }

        public Task<bool> CambiarTelefono(TelefonoDTO telefonoDto)
        {
                var paciente = _context.Pacientes.Find(telefonoDto.PacienteId) ?? throw new ArgumentException("El paciente no existe.");
                paciente.SetTelefono(telefonoDto.Telefono);
                //paciente.Telefono = telefonoDto.Telefono;
                _context.Pacientes.Update(paciente);
                _context.SaveChanges();
                return Task.FromResult(true);
        }

        public Task<bool> CambiarDni(DniDTO dniDto)
        {
                var paciente = _context.Pacientes.Find(dniDto.Id) ?? throw new ArgumentException("El paciente no existe.");
                paciente.SetDni(dniDto.Dni);
                // paciente.Dni = dniDto.Dni;
                _context.Pacientes.Update(paciente);
                _context.SaveChanges();
                return Task.FromResult(true);
        }

        public Task<string> GetNombre(int id)
        {
                var paciente = _context.Pacientes.Find(id) ?? throw new ArgumentException("El paciente no existe.");
                return Task.FromResult(paciente.Nombre);
        }
    }
}
