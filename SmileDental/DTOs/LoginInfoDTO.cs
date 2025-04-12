namespace SmileDental.DTOs
{
    public class LoginInfoDTO
    {
        public int userId { get; set; }
        public string token { get; set; } = string.Empty;
        public string role { get; set; } = "Desconocido";
    }
}
