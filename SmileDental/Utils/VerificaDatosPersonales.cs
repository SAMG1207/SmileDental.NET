using SmileDental.DTOs;
using SmileDental.DTOs.Administrador;

namespace SmileDental.Utils
{
    public static class VerificaDatosPersonales
    {
        public static bool ValidarDatosPersonales(DatosPersonalesDTO datosPersonalesDTO)
        {
 

            if (!StringManager.ValidaNombre(datosPersonalesDTO.Nombre) || !StringManager.ValidaNombre(datosPersonalesDTO.Apellido))
            {
                throw new ArgumentException("Nombre o apellido no válido.");
            }
                

            if (!StringManager.ValidaDni(datosPersonalesDTO.Dni))
            { throw new ArgumentException("DNI no válido."); }
                

            if (!StringManager.ValidaTelefono(datosPersonalesDTO.Telefono)) 
            { throw new ArgumentException("Teléfono no válido."); }
                

            if (!StringManager.ValidaPassword(datosPersonalesDTO.Password)) 
            { throw new ArgumentException("Contraseña no válida."); }
                

            if (datosPersonalesDTO.FechaDeNacimiento >= DateTime.Now) 
            { throw new ArgumentException("Fecha de nacimiento no válida."); }
                

            int edad = DateTime.Now.Year - datosPersonalesDTO.FechaDeNacimiento.Year;

            
            if (datosPersonalesDTO.FechaDeNacimiento.Date > DateTime.Now.AddYears(-edad).Date)
            {
                edad--;
            }

            if (edad < 18 || edad > 100)
            {
                throw new ArgumentException("Edad no válida. Debe ser entre 18 y 100 años.");
            }

            return true;
        }

    }
}
