using System.ComponentModel.DataAnnotations;

namespace SmileDental.DTOs.Dentista
{
    public class CrearDentistaDTO
    {

        [Required]
        [MinLength(8)]
        public string Dni { get; set; }

        public DateTime FechaNacimiento { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [MinLength(8)]
        public string Apellido { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Phone]
        public string Telefono { get; set; }

        public int EspecialidadId { get; set; }

        public bool EsAdmin { get; set; }
        public string FotoUrl { get; set; }

        public int HoraEntrada { get; set; }
        public int HoraSalida { get; set; }
    }
}
