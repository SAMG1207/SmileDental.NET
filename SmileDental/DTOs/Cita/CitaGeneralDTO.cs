using System.Text.Json.Serialization;

namespace SmileDental.DTOs.Cita
{
    public class CitaGeneralDTO
    {
        [JsonIgnore] // Se obtiene directamente del token
        public int PacienteId { get; set; }

        public DateTime Fecha { get; set; }
        public int Hora { get; set; }

        public string? URLCita { get; set; }

    }
}
