using SmileDental.Models;
using SmileDental.Services.Interfaces;

namespace SmileDental.Services
{
    public class ActionLogger : IActionLog
    {
        private readonly ApiDbContext _context;

        // Inyectamos ApiDbContext
        public ActionLogger(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LogearAccion(ActionLogInDb actionLog)
        {
            try
            {
                ActionLogInDb log = new ActionLogInDb(
                    actionLog.IdUser,
                    actionLog.Role,
                    actionLog.Accion,
                    actionLog.ResponseCode,
                    actionLog.ResponseMessage
                );

                await _context.ActionLogs.AddAsync(log);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al registrar la acción: {ex.Message}");
                return false;
            }
        }
    }
}

