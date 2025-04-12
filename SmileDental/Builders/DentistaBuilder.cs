using SmileDental.Models;

namespace SmileDental.Builders
{
    public class DentistaBuilder
    {
        private readonly Dentista _dentista;

        public DentistaBuilder()
        {
            _dentista = new Dentista();
        }

        public DentistaBuilder WithDni(string dni)
        {
            _dentista.SetDni(dni);
            return this;
        }

        public DentistaBuilder WithNombre(string nombre)
        {
            _dentista.SetNombre(nombre);
            return this;
        }

        public DentistaBuilder WithApellido(string apellido)
        {
            _dentista.SetApellido(apellido);
            return this;
        }

        public DentistaBuilder WithFechaNacimiento(DateTime fechaNacimiento)
        {
            _dentista.SetFechaNacimiento(fechaNacimiento);
            return this;
        }

        public DentistaBuilder WithTelefono(string telefono)
        {
            _dentista.SetTelefono(telefono);
            return this;
        }

        public DentistaBuilder WithEmail(string email)
        {
            _dentista.SetEmail(email);
            return this;
        }

        public DentistaBuilder WithPassword(string password)
        {
            _dentista.SetPassword(password);
            return this;
        }

        public DentistaBuilder WithEspecialidadId(int especialidadId)
        {
            _dentista.SetEspecialidadId(especialidadId);
            return this;
        }

        public DentistaBuilder WithHoraEntrada(int horaEntrada)
        {
            _dentista.SetHoraEntrada(horaEntrada);
            return this;
        }

        public DentistaBuilder WithHoraSalida(int horaSalida)
        {
            _dentista.SetHoraSalida(horaSalida);
            return this;
        }

        public DentistaBuilder WithEsAdmin(bool esAdmin)
        {
            _dentista.SetEsAdmin(esAdmin);
            return this;
        }

        public Dentista Build()
        {
            return _dentista;
        }

        public DentistaBuilder WithFotoUrl(string fotoUrl)
        {
            _dentista.SetFotoUrl(fotoUrl);
            return this;
        }
        public DentistaBuilder WithActivo(bool activo)
        {
            _dentista.SetActivo(activo);
            return this;
        }
    }
}
