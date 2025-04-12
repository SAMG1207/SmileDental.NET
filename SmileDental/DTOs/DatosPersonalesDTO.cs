using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace SmileDental.DTOs
{
    public struct DatosPersonalesDTO
    {
       
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Password { get; set; }
        public string Dni {  get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [PhoneAttribute]
        public string Telefono { get; set; }

        public DateTime FechaDeNacimiento  { get; set; }

    }
}
