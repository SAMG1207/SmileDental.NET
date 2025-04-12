using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmileDental.Models
{
    public class ActionLogInDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public  int IdUser { get;  set; }

        public  string Role { get;  set; } = string.Empty;

        public string Accion { get;  set; } = string.Empty;

        public DateTime Fecha { get;  set; }

        public int ResponseCode { get;  set; }

        public string ResponseMessage { get;  set; } = string.Empty;

        public ActionLogInDb(int idUser, string role, string accion, int responseCode, string responseMessage)
        {
            IdUser = idUser;
            Role = role;
            Accion = accion;
            Fecha = DateTime.Now;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }
    }
}
