using Microsoft.AspNetCore.Http;
using SmileDental.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken != null)
            {
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);

                if (roleClaim == null 
                    || 
                    (roleClaim.Value    !=  "Paciente" 
                    && roleClaim.Value  !=  "Dentista" 
                    && roleClaim.Value  !=  "Administrador"
                    )
                    )  
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("Acceso denegado: No tienes permisos para esta operación.");
                    return;
                }
            }

        }

        await _next(context);
    }
}

