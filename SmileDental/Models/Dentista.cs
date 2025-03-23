using System.ComponentModel.DataAnnotations.Schema;

namespace SmileDental.Models
{
    public class Dentista : UsuarioAbstract
    {

        public int EspecialidadId { get; set; }

        public bool EsAdmin { get; set; }
        public string FotoUrl { get; set; }

        public int HoraEntrada { get; set; }
        public int HoraSalida { get; set; }

        [ForeignKey("EspecialidadId")]
        public virtual Especialidad Especialidad { get; set; }

        public bool Activo { get; set; }


    }
}
