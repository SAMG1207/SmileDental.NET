using System.ComponentModel.DataAnnotations;

namespace SmileDental.DTOs.Administrador
{
    public class UsuarioDTO
    {

        public required int Id { get; set; }
      
        public required string Nombre { get; set; }
       
        public required string Apellido { get; set; }
    
        public required string Email { get; set; }
    
        public required DateTime FechaNacimiento { get; set; }


    }
}
