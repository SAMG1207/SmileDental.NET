using SmileDental.DTOs.Administrador;
using SmileDental.Models;
using System.Globalization;

namespace SmileDental.Services.Interfaces
{
    public interface IAdminInterface
    {
        /*
 * Lista de acciones que debe tener un administrador
 * El administrador se registra directamente al iniciar la aplicación en program.cs
 * Este es un dentista con permiso administrador
 * El administrador puede registrar a un dentista
 * El administrador puede registrar a un paciente, esto lo puede hacer desde el controlador de auth
 * El administrador puede ver las citas de todos los pacientes
 * El administrado puede ver las citas de todos los dentistas
 * El administrador puede agendar citas de especialidad
 * El administrado puede dar de baja a un dentista, aunque no se deberia eliminar de la base de datos.
 * Debería modficarse el modelo de dentista para agregar un campo de activo o inactivo.
 * Los pacientes no pueden darse de baja
 * Seria bueno utilizar los controladores para ahorra codigo, aunque estos utilizan el JWT para obtener el id del usuario
 * 
 */

        Task<bool> RegistrarDentista(RegistrarDentistaDTO registrarDentistaDTO);

        Task<bool> SubirFotoEspecialista(SubirFotoDTO subirFotoDTO);
        Task<List<UsuarioDTO>> VerDentistas();

        Task<List<UsuarioDTO>> VerPacientes();


        Task<List<Cita>> VerCitasPacientes(string dniPaciente);

        Task<List<Cita>> VerCitasDentistas(int idDentista);

        Task<List<Cita>> VerCitaPorFecha(DateTime fechaInicial, DateTime fechaFinal);

        Task<List<Cita>> VerCitaPorFecha(DateTime fechaInicial);
        Task<bool> AgendarCitaEspecialidad(Cita cita);

        Task<bool> DarDeBajaDentista(int idDentista);

    }
}
