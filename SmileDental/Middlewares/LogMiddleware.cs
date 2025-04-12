using SmileDental.DTOs;
using SmileDental.Models;
using SmileDental.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace SmileDental.Middlewares
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;

        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var scopedContext = context.RequestServices.GetRequiredService<ApiDbContext>();
            var actionLogger = context.RequestServices.GetRequiredService<ActionLogger>();

            string userId = "0";
            string role = "Desconocido";
            string accion = context.Request.Method + " " + context.Request.Path;
            int responseCode = 200;
            string responseMessage = "Operación exitosa";

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Backup del body original para restaurarlo después
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    if (jwtToken != null)
                    {
                        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                        if (userIdClaim != null)
                            userId = userIdClaim.Value;

                        if (roleClaim != null)
                            role = roleClaim.Value;
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        responseCode = (int)HttpStatusCode.Unauthorized;
                        responseMessage = "Token inválido.";
                    }
                }

                await _next(context);

                // Leemos el body de la respuesta que se ha almacenado en memoria
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();

                // Restauramos la posición y reenviamos la respuesta original al cliente
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);

                responseCode = context.Response.StatusCode;

                if (responseCode >= 400)
                {
                    responseMessage = $"Error en la operación: {responseText}";
                }
                else if
                         (accion.Contains("register", StringComparison.OrdinalIgnoreCase))
                {
                    responseMessage = $"Respuesta: {responseText}";
                }

                else if (accion.Contains("login", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginInfoDTO>(responseText);

                        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.token))
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadToken(loginResponse.token) as JwtSecurityToken;

                            if (jwtToken != null)
                            {
                                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                                if (userIdClaim != null)
                                    userId = userIdClaim.Value;

                                if (roleClaim != null)
                                    role = roleClaim.Value;

                                responseMessage = $"Login exitoso para userId: {userId}, role: {role}";
                            }
                            else
                            {
                                responseMessage = $"Login exitoso, pero el token es inválido: {loginResponse.token}";
                            }
                        }
                        else
                        {
                            responseMessage = $"Login sin token válido. Respuesta: {responseText}";
                        }
                    }
                    catch (Exception ex)
                    {
                        responseMessage = $"Error al procesar respuesta de login: {ex.Message}";
                    }
                }


            }
            catch (Exception ex)
            {
                responseCode = 500;
                responseMessage = $"Error: {ex.Message}";
            }
            finally
            {
                if (!int.TryParse(userId, out var parsedUserId))
                {
                    parsedUserId = 0;
                }

                var actionLog = new ActionLogInDb
                (
                    parsedUserId,
                    role,
                    accion,
                    responseCode,
                    responseMessage
                );

                await actionLogger.LogearAccion(actionLog);
                await scopedContext.SaveChangesAsync();
            }
        }

    }
}
