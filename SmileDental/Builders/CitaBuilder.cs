using SmileDental.Models;

namespace SmileDental.Builders
{
    public class CitaBuilder
    {
        private readonly Cita _cita;
        public CitaBuilder()
        {
            _cita = new Cita();
        }
        public CitaBuilder WithPacienteId(int pacienteId)
        {
            _cita.SetPacienteId(pacienteId);
            return this;
        }
        public CitaBuilder WithDentistaId(int dentistaId)
        {
            _cita.SetDentistaId(dentistaId);
            return this;
        }
        public CitaBuilder WithFecha(DateTime fecha)
        {
            _cita.SetFecha(fecha);
            return this;
        }
        public CitaBuilder WithHora(int hora)
        {
            _cita.SetHora(hora);
            return this;
        }
        public Cita Build()
        {
            return _cita;
        }
    }
}
