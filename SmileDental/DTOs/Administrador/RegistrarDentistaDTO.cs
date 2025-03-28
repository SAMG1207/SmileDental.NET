namespace SmileDental.DTOs.Administrador
{
    public class RegistrarDentistaDTO
    {
   
        public string Especialidad { get; set; }
 
        public DatosPersonalesDTO DatosPersonales { get; set; }
        public bool Activo { get; set; }

        public bool EsAdmin { get; set; }

        public int HoraEntrada { get; set; }

        public int HoraSalida { get; set; }
    }
}
