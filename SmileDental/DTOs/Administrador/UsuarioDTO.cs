namespace SmileDental.DTOs.Administrador
{
    public class UsuarioDTO
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }

        public string email { get; set; }

        public DateTime fechaNacimiento { get; set; }


    }
}
