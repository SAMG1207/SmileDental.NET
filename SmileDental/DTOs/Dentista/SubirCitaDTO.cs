namespace SmileDental.DTOs.Dentista
{
    public class SubirCitaDTO
    {
        public int citaId { get; set; }
        public required IFormFile file { get; set; }
    }
}
