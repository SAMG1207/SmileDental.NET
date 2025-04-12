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

        public void SetEspecialidadId(int especialidadId)
        {
            if (especialidadId <= 0 || especialidadId > 10)
            {
                throw new ArgumentException("El ID de la especialidad no es válido.");
            }
            EspecialidadId = especialidadId;
        }
        public void SetEsAdmin(bool esAdmin)
        {
            EsAdmin = esAdmin;
        }   

    }
}
