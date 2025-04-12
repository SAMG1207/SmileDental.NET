using SmileDental.Models;

namespace SmileDental.Builders
{
    public class PacienteBuilder
    {
        private readonly Paciente _paciente;

        public PacienteBuilder()
        {
            _paciente = new Paciente();
        }

        public PacienteBuilder WithDni(string dni)
        {
            _paciente.SetDni(dni);
            return this;
        }

        public PacienteBuilder WithNombre(string nombre)
        {
            _paciente.SetNombre(nombre);
            return this;
        }

        public PacienteBuilder WithApellido(string apellido)
        {
            _paciente.SetApellido(apellido);
            return this;
        }

        public PacienteBuilder WithFechaNacimiento(DateTime fechaNacimiento)
        {
            _paciente.SetFechaNacimiento(fechaNacimiento);
            return this;
        }

        public PacienteBuilder WithTelefono(string telefono)
        {
            _paciente.SetTelefono(telefono);
            return this;
        }

        public PacienteBuilder WithEmail(string email)
        {
            _paciente.SetEmail(email);
            return this;
        }

        public PacienteBuilder WithPassword(string password)
        {
            _paciente.SetPassword(password);
            return this;
        }

        public Paciente Build()
        {
            return _paciente;
        }
    }
}
