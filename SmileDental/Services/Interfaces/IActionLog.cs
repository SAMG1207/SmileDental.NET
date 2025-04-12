using SmileDental.Models;

namespace SmileDental.Services.Interfaces
{
    public interface IActionLog
    {
        Task<bool> LogearAccion(ActionLogInDb actionLog);
    }
}
