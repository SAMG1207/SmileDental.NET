using Microsoft.EntityFrameworkCore;
using SmileDental.DTOs.Administrador;
using SmileDental.Models;
using SmileDental.Repositories.Interfaces;

namespace SmileDental.Repositories.Repository
{
    public class DentistaRepository(ApiDbContext context) : IDentistaRepository
    {
        private readonly ApiDbContext _context = context;

        public async Task<bool> ActivarDentista(int id)
        {
                var dentista = await _context.Dentistas.FindAsync(id);
                if (dentista != null)
                {
                    dentista.SetActivo(true);
                    _context.Dentistas.Update(dentista);
                    await _context.SaveChangesAsync();
                    return true;
                }
            return false;
        }


        public async Task AddAsync(Dentista dentista)
        {
            await _context.Dentistas.AddAsync(dentista);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _context.Dentistas
                .Where(d => d.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> DesactivarDentista(int id)
        {
            var dentista = await _context.Dentistas.FindAsync(id);
            if (dentista != null)
            {
                dentista.SetActivo(false);
                _context.Dentistas.Update(dentista);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Dentista> GetAdministradorByEmail(string email)
        {
            return await _context.Dentistas.Where(d => d.Email == email && d.EsAdmin == true)
                .FirstOrDefaultAsync() ?? throw new Exception("Dentista no encontrado");   
        }

        public async Task<IEnumerable<Dentista>> GetAllAsync()
        {
            var dentistas = await _context.Dentistas
                .Include(d => d.Especialidad)
                .ToListAsync();

            return dentistas;
        }

        public async Task<Dentista> GetByIdAsync(int id)
        {
            var dentista = await _context.Dentistas
                .Include(d => d.Especialidad)
                .FirstOrDefaultAsync(d => d.Id == id) ?? throw new Exception("Dentista no encontrado");
            
            return dentista;
        }

        public async Task<Dentista> GetDentistaByEmail(string email)
        {
            return await _context.Dentistas.Where(d => d.Email == email && d.EsAdmin == false)
               .FirstOrDefaultAsync() ?? throw new Exception("Dentista no encontrado");
        }

        public async Task<IEnumerable<int>> GetDentistasEspecialistasIds()
        {
            return await _context.Dentistas
                .Where(d => d.EspecialidadId == 6)
                .Select(d => d.Id)
                .ToListAsync();
        }
        /*
        public async Task<IEnumerable<int>> GetDentistasGeneralesIds()
        {
            return await _context.Dentistas
               .Where(d => d.EspecialidadId != 6)
               .Select(d => d.Id)
               .ToListAsync();
        }
        */
        public async Task<IEnumerable<int>> GetDisponibilidadDeDentistasGeneralesPorFechaYHora(DateTime fecha, int hora)
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

            // Si no hay dentistas libres, retornar una lista vacía  
            if (dentistasLibres.Count == 0)
            {
                return [];
            }

            // Seleccionar los IDs de los dentistas disponibles  
            return dentistasLibres.Select(d => d.Id);
        }

        public async Task<string> GetNombreDentistaPorId(int id)
        {
            string nombre = await _context.Dentistas
                .Where(d => d.Id == id)
                .Select(d => $"{d.Nombre} {d.Apellido}")
                .FirstOrDefaultAsync() ?? throw new Exception("No se ha encontrado al dentista");
            return nombre;
        }

        public async Task<IEnumerable<PresentacionDentistaDTO>> GetPresentacionDentistasNoGenerales()
        {
            var dentistas = await _context.Dentistas
                .Where(d => d.EspecialidadId != 6 && d.Activo == true)
                .Select(d => new PresentacionDentistaDTO
                {
                    Nombre = d.Nombre,
                    Apellido = d.Apellido,
                    Especialidad = d.Especialidad.Nombre,
                    UrlFoto = d.FotoUrl
                })
                .ToListAsync();

            return dentistas;
        }



        /*
public async Task<IEnumerable<int>> GetHorasDisponiblesPorFecha(DateTime fecha)
{
  IEnumerable<int> dentistasGeneralesIds = await GetDentistasGeneralesIds();

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
*/
        public async Task UpdateAsync(Dentista dentista)
        {
            if (await _context.Dentistas.AnyAsync(c => c.Id == dentista.Id))
            {
                _context.Dentistas.Update(dentista);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Dentista no registrada");
            }
        }

    }
}
