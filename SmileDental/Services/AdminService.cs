using Microsoft.EntityFrameworkCore;
using Serilog;
using SmileDental.DTOs;
using SmileDental.DTOs.Administrador;
using SmileDental.DTOs.Dentista;
using SmileDental.Models;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;
using Sprache;

namespace SmileDental.Services
{
    public class AdminService : IAdminInterface
    {

        private readonly ApiDbContext _context;
        private readonly PasswordManager _passwordManager;

        public AdminService(ApiDbContext context, PasswordManager passwordManager)
        {
            _context = context;
            _passwordManager = passwordManager;
        }

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
                var nuevaCita = new Cita
                {
                    DentistaId = cita.DentistaId,
                    PacienteId = cita.PacienteId,
                    Fecha = cita.Fecha,
                    Hora = cita.Hora,
                };

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

            dentista.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> VerificarRegistroDentista(RegistrarDentistaDTO registrarDentistaDTO)
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

            return true;
        }


        public async Task<bool> RegistrarDentista(RegistrarDentistaDTO registrarDentistaDTO)
        {
            try
            {
                if (!await VerificarRegistroDentista(registrarDentistaDTO))
                {
                    return false;
                }
                int especialidadId = await _context.Especialidades
                .Where(e => e.Nombre == registrarDentistaDTO.Especialidad)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

                if (especialidadId == 0)
                {
                    Log.Error("Especialidad no válida.");
                    return false;
                }
                Dentista dentista = new()
                {
                    Nombre = registrarDentistaDTO.DatosPersonales.Nombre,
                    Apellido = registrarDentistaDTO.DatosPersonales.Apellido,
                    Email = registrarDentistaDTO.DatosPersonales.Email,
                    Password = _passwordManager.HashPassword(registrarDentistaDTO.DatosPersonales.Password),
                    EspecialidadId = especialidadId,
                    Dni = registrarDentistaDTO.DatosPersonales.Dni,
                    FotoUrl = "pending",
                    Telefono = registrarDentistaDTO.DatosPersonales.Nombre,
                    FechaNacimiento = registrarDentistaDTO.DatosPersonales.FechaDeNacimiento,
                    Activo = registrarDentistaDTO.Activo,
                    EsAdmin = registrarDentistaDTO.EsAdmin,
                    HoraEntrada = registrarDentistaDTO.HoraEntrada,
                    HoraSalida = registrarDentistaDTO.HoraSalida
                };

                _context.Dentistas.Add(dentista);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (ArgumentException e)
            {
                // Loguear el error de validación
                Log.Error($"Error de validación al registrar el dentista: {e.Message}");
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Error al registrar el dentista: {e.Message}");
                return false;
            }
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
                    dentista.FotoUrl = result.FileName;
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

        public async Task<List<Cita>> VerCitaPorFecha(DateTime fechaInicial, DateTime? fechaFinal)
        {
            try
            {
                var citas = new List<Cita>();
                if (fechaFinal == null)
                {
                    citas = await _context.Citas.Where(c => c.Fecha >= fechaInicial).ToListAsync();
                    return citas;
                }
                citas = await _context.Citas.Where(c => c.Fecha >= fechaInicial && c.Fecha <= fechaFinal).ToListAsync();
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
                bool dniValido = StringManager.validaDni(dniPaciente);
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
                        id = dentista.Id,
                        nombre = dentista.Nombre,
                        apellido = dentista.Apellido,
                        fechaNacimiento = dentista.FechaNacimiento,
                        email = dentista.Email
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
                foreach(Paciente paciente in pacientes)
                {
                    var UsuarioDTO = new UsuarioDTO
                    {
                        id = paciente.Id,
                        nombre = paciente.Nombre,
                        apellido = paciente.Apellido,
                        email = paciente.Email,
                        fechaNacimiento = paciente.FechaNacimiento

                    };
                    usuarioDTOs.Add(UsuarioDTO);
                }
                return usuarioDTOs;
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener los pacientes: {e.Message}");
                return new List<UsuarioDTO>();
            }
        }

     
    }
}
