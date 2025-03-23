namespace SmileDental.DTOs.Administrador
{
    public class RegistrarDentistaDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Especialidad { get; set; }

        public string Dni { get; set; }

        public string Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public bool Activo { get; set; }

        public bool EsAdmin { get; set; }

        public int HoraEntrada { get; set; }

        public int HoraSalida { get; set; }
    }
}
