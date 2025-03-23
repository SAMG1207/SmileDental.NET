namespace SmileDental.DTOs.Dentista
{
    public class MisCitas
    {
        public DateTime Fecha { get; set; }
        public int Hora { get; set; }

        public int idPaciente { get; set; }

        public string NombrePaciente { get; set; }

        public string ApellidosPaciente { get; set; }

        public string? URLCita { get; set; }
    }
}
