using System.ComponentModel.DataAnnotations;

namespace SmileDental.DTOs
{
    public class CrearPacienteDTO
    {
        [Required]
        [MinLength(8)]
        public string Dni { get; set; }

        public DateTime FechaNacimiento { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Phone]
        public string Telefono { get; set; }

    }
}
