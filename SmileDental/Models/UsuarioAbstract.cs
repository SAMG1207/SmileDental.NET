using SmileDental.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmileDental.Models
{
    public abstract class UsuarioAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; protected set; }

        [Required]
        [MinLength(8)]
        public string Dni { get; protected set; }

        public DateTime FechaNacimiento { get; protected set; }

        [Required]
        public string Nombre { get; protected set; }

        [Required]
        [MinLength(8)]
        public string Apellido { get; protected set; }

        [EmailAddress]
        public string Email { get; protected set; }

        [Required]
        [MinLength(8)]
        public string Password { get; protected set; }

        [Phone]
        public string Telefono { get; protected set; }

        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();

        private readonly PasswordManager _passwordManager;

        public void SetDni(string dni)
        {
            if (!StringManager.ValidaDni(dni))
            {
                throw new ArgumentException("El DNI no es válido.");
            }

            Dni = dni;
        }

        public void SetNombre(string nombre)
        {
            if (!StringManager.ValidaNombre(nombre))
            {
                throw new ArgumentException("El nombre no es válido.");
            }
            Nombre = nombre;
        }

        public void SetApellido(string apellido)
        {
            if (!StringManager.ValidaNombre(apellido))
            {
                throw new ArgumentException("El apellido no es válido.");
            }
            Apellido = apellido;
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("El email no puede estar vacío.");
            }
            Email = email;
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void SetFechaNacimiento(DateTime fechaNacimiento)
        {
            if (fechaNacimiento > DateTime.Now)
            {
                throw new ArgumentException("La fecha de nacimiento no puede ser futura.");
            }
            FechaNacimiento = fechaNacimiento;
        }

        public void SetTelefono(string telefono)
        {
            if (!StringManager.ValidaTelefono(telefono))
            {
                throw new ArgumentException("El teléfono no es válido.");
            }
            Telefono = telefono;
        }

    }
}
