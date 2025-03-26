using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmileDental.Models
{
    public abstract class UsuarioAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();

    }
}
