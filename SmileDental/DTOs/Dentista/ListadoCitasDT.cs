using SmileDental.Models;

namespace SmileDental.DTOs.Dentista
{
    public class ListadoCitasDT
    {
        public List<SmileDental.Models.Cita> Citas { get; set; } = new();
        public int NumeroPaginas { get; set; }
    }
}

