using System.Globalization;

namespace SmileDental.DTOs.Dentista
{
    public class CitaPacienteDTO
    {
        public int citaId { get; set; }
        public string nombreInteresado { get; set; } // puede ser dentista o paciente

        public string apellidoInteresado { get; set; } // puede ser dentista o paciente

        public DateTime fecha { get; set; }

        public int hora { get; set; }

        public string? urlCita { get; set; }
    }
}
