namespace SmileDental.Models
{
    public class Paciente : UsuarioAbstract
    {
        public Paciente(
            string dni,
            string nombre,
            string apellido,
            DateTime fechaNacimiento,
            string telefono,
            string email,
            string password
            ) {

            SetDni(dni);
            SetNombre(nombre);
            SetApellido(apellido);
            SetFechaNacimiento(fechaNacimiento);
            SetTelefono(telefono);
            SetEmail(email);
            SetPassword(password);
        }
    }
}
