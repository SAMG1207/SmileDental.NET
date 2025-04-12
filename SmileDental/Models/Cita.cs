using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmileDental.Models
{
    public class Cita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        public int PacienteId { get; private set; }

        public int DentistaId { get; private set; }

        public DateTime Fecha { get; private set; }

        public int Hora { get; private set; }


        [JsonIgnore]
        [ForeignKey("PacienteId")]
        public virtual Paciente Paciente { get; set; }


        [JsonIgnore]
        [ForeignKey("DentistaId")]
        public virtual Dentista Dentista { get; set; }

        public string URLCita { get; private set; }

        public Cita()
        {
           
        }

        public Cita(int pacienteId, int dentistaId, DateTime fecha, int hora)
        {
            SetPacienteId(pacienteId);
            SetDentistaId(dentistaId);
            SetFecha(fecha);
            SetHora(hora);
            URLCita = "pending";
        }

        public void SetURLCita(string url)
        {
            URLCita = url;
        }

        public void SetPacienteId(int pacienteId)
        {
            if (pacienteId < 0)
            {
                throw new ArgumentException("El ID del paciente no puede ser negativo.");
            }
            PacienteId = pacienteId;
        }

        public void SetDentistaId(int dentistaId)
        {
            if (dentistaId < 0)
            {
                throw new ArgumentException("El ID del dentista no puede ser negativo.");
            }
            DentistaId = dentistaId;
        }

        public void SetFecha(DateTime fecha)
        {
            if (fecha < DateTime.Now)
            {
                throw new ArgumentException("La fecha no puede ser en el pasado.");
            }
            Fecha = fecha;
        }

        public void SetHora(int hora)
        {
            if (hora < 8 || hora > 21)
            {
                throw new ArgumentException("La hora debe estar entre 8 y 21.");
            }
            Hora = hora;
        }


    }
}
