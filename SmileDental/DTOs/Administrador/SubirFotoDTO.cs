namespace SmileDental.DTOs.Administrador
{
    public class SubirFotoDTO
    {
        public int DentistaId { get; set; }
        public IFormFile Foto { get; set; }
    }
}
