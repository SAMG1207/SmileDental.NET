using SmileDental.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmileDental.Models
{
    public class Dentista : UsuarioAbstract
    {

        public int EspecialidadId { get; private set; }

        public bool EsAdmin { get; private set; }
        public string FotoUrl { get; private set; }

        public int HoraEntrada { get; private set; }
        public int HoraSalida { get; private set; }

        public bool Activo { get; private set; }

        [ForeignKey("EspecialidadId")]
        public virtual Especialidad Especialidad { get; set; }

        public Dentista(){   }

        public Dentista(
            string dni, 
            string nombre,
            string apellido,
            DateTime fechaNacimiento,
            string telefono,
            string email,
            string password,
            int especialidadId, 
            int horaEntrada,
            int horaSalida
            ) 
        { 
            SetDni(dni);
            SetNombre(nombre);
            SetApellido(apellido);
            SetFechaNacimiento(fechaNacimiento);
            SetTelefono(telefono);
            SetEmail(email);
            SetPassword(password);
            EspecialidadId = especialidadId; // Se maneja en el servicio
            EsAdmin = false;
            SetHoraEntrada(horaEntrada);
            SetHoraSalida(horaSalida);
            Activo = true;
        }

        public void SetActivo(bool activo)
        {
            Activo = activo;
        }

        public void SetHoraEntrada(int horaEntrada)
        {
            if (horaEntrada < 8 || horaEntrada > 16)
            {
                throw new ArgumentException("La hora de entrada no es válida.");
            }
            HoraEntrada = horaEntrada;
        }

        public void SetHoraSalida(int horaSalida)
        {
            if (horaSalida < 12 || horaSalida > 21)
            {
                throw new ArgumentException("La hora de entrada no es válida.");
            }
            HoraSalida = horaSalida;
        }

        public void SetFotoUrl(string fotoUrl)
        {
            if (string.IsNullOrWhiteSpace(fotoUrl))
            {
                throw new ArgumentException("La URL de la foto no puede estar vacía.");
            }
            FotoUrl = fotoUrl;
        }

    }
}
