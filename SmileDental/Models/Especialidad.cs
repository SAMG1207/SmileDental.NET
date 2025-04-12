using SmileDental.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmileDental.Models
{
    public class Especialidad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        public string Nombre { get; private set; }

        public virtual ICollection<Dentista> Dentistas { get; set; }


        public Especialidad()
        {
        }

        public Especialidad(string nombre)
        {
            SetNombre(nombre);
        }

        public void SetNombre(string nombre)
        {
            if(!StringManager.ValidaNombre(nombre))
            {
                throw new ArgumentException("El nombre de la especialidad no es válido.");
            }
            Nombre = nombre;
        }
    }
}
