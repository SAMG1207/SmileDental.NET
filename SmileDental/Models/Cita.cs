using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmileDental.Models
{
    public class Cita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PacienteId { get; set; }

        public int DentistaId { get; set; }

        public DateTime Fecha { get; set; }

        public int Hora { get; set; }


        [JsonIgnore]
        [ForeignKey("PacienteId")]
        public virtual Paciente Paciente { get; set; }


        [JsonIgnore]
        [ForeignKey("DentistaId")]
        public virtual Dentista Dentista { get; set; }

        public string URLCita { get; set; }


    }
}
