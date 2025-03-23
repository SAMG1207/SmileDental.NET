using System.Globalization;

namespace SmileDental.DTOs.Dentista
{
    public class CitaPacienteDTO
    {
        public int citaId { get; set; }
        public string nombreDentista { get; set; }

        public string apellidoDentista { get; set; }

        public DateTime fecha { get; set; }

        public int hora { get; set; }

        public string? urlCita { get; set; }
    }
}
