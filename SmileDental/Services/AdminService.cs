using Microsoft.EntityFrameworkCore;
using Serilog;
using SmileDental.Builders;
using SmileDental.DTOs;
using SmileDental.DTOs.Administrador;
using SmileDental.DTOs.Dentista;
using SmileDental.Models;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;
using Sprache;

namespace SmileDental.Services
{
    public class AdminService(ApiDbContext context) : IAdminInterface, IGetNombre
    {

        private readonly ApiDbContext _context = context;

        public async Task<bool> AgendarCitaEspecialidad(Cita cita)
        {
            try
            {
                DateTime fechaActual = DateTime.Now;
                bool pacienteTieneCita = await _context.Citas.AnyAsync(c => c.PacienteId == cita.PacienteId && c.Fecha > fechaActual);
                if (pacienteTieneCita)
                {
                    throw new ArgumentException("El paciente ya tiene una cita agendada");
                }
                var nuevaCita = new Cita(cita.PacienteId, cita.DentistaId, cita.Fecha, cita.Hora);


                _context.Citas.Add(nuevaCita);
                await _context.SaveChangesAsync();
                return true;
            }catch(Exception e)
            {
                Log.Error($"Error al agendar la cita: {e.Message}");
                return false;
            }
        }


        public async Task<bool> DarDeBajaDentista(int idDentista)
        {
            var dentista = await _context.Dentistas.FirstOrDefaultAsync(d => d.Id == idDentista && d.Activo == true);
            if (dentista == null)
            {
                throw new Exception("El dentista no existe o ya está dado de baja");
            }

            dentista.SetActivo(false);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task VerificarRegistroDentista(RegistrarDentistaDTO registrarDentistaDTO)
        {
            // Validaciones de datos de entrada
            VerificaDatosPersonales.ValidarDatosPersonales(registrarDentistaDTO.DatosPersonales);

            // Validar horario de trabajo
            if (registrarDentistaDTO.HoraEntrada < 8 || registrarDentistaDTO.HoraSalida > 21 || registrarDentistaDTO.HoraEntrada >= registrarDentistaDTO.HoraSalida)
            {
                throw new ArgumentException("Horario de entrada o salida no válido.");
            }

            // Validar si ya hay un administrador
            if (registrarDentistaDTO.EsAdmin && await _context.Dentistas.AnyAsync(d => d.EsAdmin))
            {
                throw new ArgumentException("Ya hay un administrador registrado.");
            }

            // Validar si la especialidad existe
            bool especialidadExiste = await _context.Especialidades.AnyAsync(e => e.Nombre == registrarDentistaDTO.Especialidad);
            if (!especialidadExiste)
            {
                throw new ArgumentException("Especialidad no válida.");
            }

            if (await _context.Dentistas.AnyAsync(d => d.Dni == registrarDentistaDTO.DatosPersonales.Dni))
            {
                throw new ArgumentException("DNI ya registrado.");
            }

            if (await _context.Dentistas.AnyAsync(d => d.Email == registrarDentistaDTO.DatosPersonales.Email))
            {
                throw new ArgumentException("Email ya registrado.");
            }
        }

        public async Task<bool> RegistrarDentista(RegistrarDentistaDTO registrarDentistaDTO)
        {
            // Verifica si los datos del dentista son válidos
            await VerificarRegistroDentista(registrarDentistaDTO);

            // Obtener el ID de la especialidad
            int especialidadId = await _context.Especialidades
                .Where(e => e.Nombre == registrarDentistaDTO.Especialidad)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            if (especialidadId == 0)
            {
                throw new ArgumentException("Especialidad no válida.");
            }

            // Crear la entidad Dentista
            var dentista = new DentistaBuilder()
                 .WithDni(registrarDentistaDTO.DatosPersonales.Dni)
                 .WithNombre(registrarDentistaDTO.DatosPersonales.Nombre)
                 .WithApellido(registrarDentistaDTO.DatosPersonales.Apellido)
                 .WithFechaNacimiento(registrarDentistaDTO.DatosPersonales.FechaDeNacimiento)
                 .WithTelefono(registrarDentistaDTO.DatosPersonales.Telefono)
                 .WithEmail(registrarDentistaDTO.DatosPersonales.Email)
                 .WithPassword(registrarDentistaDTO.DatosPersonales.Password)
                 .WithEspecialidadId(especialidadId)
                 .WithHoraEntrada(registrarDentistaDTO.HoraEntrada)
                 .WithHoraSalida(registrarDentistaDTO.HoraSalida)
                 .WithActivo(true)
                 .WithFotoUrl("pending")
                 .WithEsAdmin(false)
                 .Build();
            
            _context.Dentistas.Add(dentista);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> SubirFotoEspecialista(SubirFotoDTO subirFotoDTO)
        {
            try
            {
                var file = subirFotoDTO.Foto;
                var result = await FileHandler.UploadFile(file);
                if(result.Success)
                {
                    var dentista = await _context.Dentistas.FindAsync(subirFotoDTO.DentistaId);
                    if ( dentista == null )
                    {
                        throw new Exception("Dentista no encontrado");
                    }
                    dentista.SetFotoUrl(result.FileName);
                    await _context.SaveChangesAsync();
                    return true;
                }
                Log.Error($"Error al subir archivo: {result.Message}");
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Error al subir la foto del dentista: {e.Message}");
                return false;
            }
           
        }

        public async Task<List<Cita>> VerCitaPorFecha(DateTime fechaInicial, DateTime fechaFinal)
        {
            try
            {
                if(fechaInicial > fechaFinal)
                {
                    throw new ArgumentException("La fecha final debe ser mas reciente que la inicial");
                }
                var citas = await _context.Citas.Where(c => c.Fecha >= fechaInicial && c.Fecha <= fechaFinal).ToListAsync();
                return citas;
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener las citas por fecha: {e.Message}");
                return [];
            }
        }

        public async Task<List<Cita>> VerCitasDentistas(int idDentista)
        {
            try
            {
                return await _context.Citas.Where(c => c.DentistaId == idDentista).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener las citas del dentista {idDentista}: {e.Message}");
                return new List<Cita>();
            }
        }

        public async Task<List<Cita>> VerCitasPacientes(string dniPaciente)
        {
            try
            {
                bool dniValido = StringManager.ValidaDni(dniPaciente);
                if (!dniValido)
                {
                    return await Task.FromResult(new List<Cita>());
                }

                return await _context.Citas.Where(c => c.Paciente.Dni == dniPaciente).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener las citas del paciente {dniPaciente}: {e.Message}");
                return new List<Cita>();
            }

        }

        public async Task<List<UsuarioDTO>> VerDentistas()
        {
            try
            {
                var dentistas = await _context.Dentistas.ToListAsync();
                var usuarioDTOs = new List<UsuarioDTO>();
                foreach (Dentista dentista in dentistas)
                {
                    var usuarioDTO = new UsuarioDTO
                    {
                        Id = dentista.Id,
                        Nombre = dentista.Nombre,
                        Apellido = dentista.Apellido,
                        FechaNacimiento = dentista.FechaNacimiento,
                        Email = dentista.Email
                    };

                    usuarioDTOs.Add(usuarioDTO);
                }
                return usuarioDTOs;
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener los dentistas: {e.Message}");
                return new List<UsuarioDTO>();
            }
        }

        public async Task<List<UsuarioDTO>> VerPacientes()
        {
            try
            {
                var pacientes = await _context.Pacientes.ToListAsync();
                var usuarioDTOs = new List<UsuarioDTO>();
                foreach (Paciente paciente in pacientes)
                {
                    var usuarioDTO = new UsuarioDTO
                    {
                        Id = paciente.Id,
                        Nombre = paciente.Nombre,
                        Apellido = paciente.Apellido,
                        Email = paciente.Email,
                        FechaNacimiento = paciente.FechaNacimiento
                    };
                    usuarioDTOs.Add(usuarioDTO);
                }
                return usuarioDTOs;
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener los pacientes: {e.Message}");
                return new List<UsuarioDTO>();
            }
        }

        public async Task<List<Cita>> VerCitaPorFecha(DateTime fechaInicial)
        {
            try
            {
                var citas = await _context.Citas.Where(c => c.Fecha > fechaInicial).ToListAsync();
                return citas;
            }catch (Exception e)
            {
                return new List<Cita>();
            }
        }

        public async Task<string> GetNombre(int id)
        {
            string nombre = await _context.Dentistas
           .Where(d => d.Id == id)
           .Select(d => $"{d.Nombre} {d.Apellido}")
           .FirstOrDefaultAsync() ?? throw new Exception("Dentista no encontrado");

            return nombre;
        }
    }
}
