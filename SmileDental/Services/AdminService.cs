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


        public async Task<bool> RegistrarDentista(RegistrarDentistaDTO registrarDentistaDTO)
        {
            try
            {
                if (!StringManager.validaNombre(registrarDentistaDTO.Nombre) || !StringManager.validaNombre(registrarDentistaDTO.Apellido))
                {
                    throw new ArgumentException("Nombre o apellido no válido");
                }

                if(!StringManager.validaDni(registrarDentistaDTO.Dni))
                {
                    throw new ArgumentException("Dni no válido");
                }

                if (!StringManager.validaTelefono(registrarDentistaDTO.Telefono))
                {
                    throw new ArgumentException("Teléfono no válido");
                }

                if (!StringManager.validaPassword(registrarDentistaDTO.Password))
                {
                    throw new ArgumentException("Contraseña no válida");
                }

                if (registrarDentistaDTO.HoraEntrada < 8|| registrarDentistaDTO.HoraEntrada > 21 || registrarDentistaDTO.HoraSalida < 8 || registrarDentistaDTO.HoraSalida > 21)
                {
                    throw new ArgumentException("Horario no válido");
                }

                if (registrarDentistaDTO.HoraEntrada >= registrarDentistaDTO.HoraSalida)
                {
                    throw new ArgumentException("Hora de entrada es mas tarde que la de salida");
                }

                if (registrarDentistaDTO.FechaNacimiento >= DateTime.Now)
                {
                    throw new ArgumentException("Fecha de nacimiento no válida");
                }

                if(registrarDentistaDTO.EsAdmin == true)
                {
                    bool yaHayAdmin = await _context.Dentistas.AnyAsync(d => d.EsAdmin == true);
                    if (yaHayAdmin)
                    {
                        throw new ArgumentException("Ya hay un administrador registrado");
                    }
                }

                int especialidadId = await _context.Especialidades.Where(e => e.Nombre == registrarDentistaDTO.Especialidad).Select(e => e.Id).FirstOrDefaultAsync();
                if (especialidadId == 0)
                {
                    throw new ArgumentException("Especialidad no válida");
                }

                Dentista dentista = new Dentista
                {
                    Nombre = registrarDentistaDTO.Nombre,
                    Apellido = registrarDentistaDTO.Apellido,
                    Email = registrarDentistaDTO.Email,
                    Password = _passwordManager.HashPassword(registrarDentistaDTO.Password),
                    EspecialidadId = especialidadId,
                    Dni = registrarDentistaDTO.Dni,
                    FotoUrl = "pending",
                    Telefono = registrarDentistaDTO.Telefono,
                    FechaNacimiento = registrarDentistaDTO.FechaNacimiento,
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
                List<Cita> citas = new List<Cita>();
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
                return new List<Cita>();
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

        public async Task<List<Dentista>> VerDentistas()
        {
            try
            {
                var dentistas = await _context.Dentistas.ToListAsync();
                return dentistas;
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener los dentistas: {e.Message}");
                return new List<Dentista>();
            }
        }

        public async Task<List<Paciente>> VerPacientes()
        {
            try
            {
                var pacientes = await _context.Pacientes.ToListAsync();
                return pacientes;
            }
            catch (Exception e)
            {
                Log.Error($"Error al obtener los pacientes: {e.Message}");
                return new List<Paciente>();
            }
        }

     
    }
}
