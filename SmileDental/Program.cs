using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmileDental.Models;
using SmileDental.Services;
using SmileDental.Services.Interfaces;
using SmileDental.Utils;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace SmileDental
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuración de logging con Serilog
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });

            // Middleware de logging
            builder.Logging.ClearProviders(); // Elimina los proveedores de logging por defecto
            builder.Logging.AddConsole(); // Añade un proveedor de logging a la consola
            builder.Logging.AddDebug(); // Añade un proveedor de logging a la consola

            // Configuración de la autenticación (JWT)
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
                        ValidAudience = builder.Configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:SecretKey"])
                        )
                    };
                });

            // Configuración de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .AllowAnyOrigin()   // Permite cualquier origen
                        .AllowAnyMethod()   // Permite cualquier método HTTP (GET, POST, etc.)
                        .AllowAnyHeader()); // Permite cualquier encabezado
            });

            // Configuración de servicios
            builder.Services.AddDbContext<ApiDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            // JWT Handler
            var jwtSettings = builder.Configuration.GetSection("JWTSettings");
            string secretKey = jwtSettings.GetValue<string>("SecretKey");
            string issuer = jwtSettings.GetValue<string>("Issuer");
            string audience = jwtSettings.GetValue<string>("Audience");
            builder.Services.AddSingleton(new JWTHandler(secretKey, issuer, audience));

            // Password services
            builder.Services.AddScoped<PasswordHasher<object>>(); // Usado para el hash de contraseñas
            builder.Services.AddScoped<PasswordManager>(); // Servicio para gestionar el hash de contraseñas

            // Registrar Servicios e Inyección de dependencias
            builder.Services.AddScoped<IInfo, Info>();
            builder.Services.AddScoped<IPacienteInterface, PacienteService>();
            builder.Services.AddScoped<IDentistInterface, DentistaService>();
            builder.Services.AddScoped<IAdminInterface, AdminService>();
            builder.Services.AddScoped<IAuthInterface, AuthServices>();

            // Health Checks
            builder.Services.AddHealthChecks();

            // Rate Limiting
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("fixed", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromSeconds(10)
                        }));
            });

            // Agregar controladores y otras configuraciones
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configuración de la aplicación
            app.UseCors("AllowAll");

            // Configurar la base de datos y migraciones
            using (var scope = app.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

                var especialidades = dataContext.Especialidades.ToList();
                if (especialidades.Count == 0)
                {
                    var leagues = new List<Especialidad>
                    {
                        new() { Nombre = "Ortodoncia" },
                        new() { Nombre = "Periodoncia" },
                        new() { Nombre = "Cirugía" },
                        new() { Nombre = "Endodoncia" },
                        new() { Nombre = "Estética" },
                        new() { Nombre = "General" },
                    };

                    dataContext.Especialidades.AddRange(leagues);
                    dataContext.SaveChanges();
                }

                // Migraciones de base de datos
                dataContext.Database.Migrate();
            }

            // Middleware de manejo de errores global
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (exceptionHandlerPathFeature?.Error is not null)
                    {
                        await context.Response.WriteAsync(new
                        {
                            error = "Ha ocurrido un error inesperado.",
                            details = exceptionHandlerPathFeature.Error.Message
                        }.ToString());
                    }
                });
            });

            // Configuración del pipeline HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middleware de seguridad
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                await next();
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Rate Limiting
            app.UseRateLimiter();

            // Health Checks
            app.MapHealthChecks("/health");

            // Middleware personalizado de JWT (si es necesario)
            // app.UseMiddleware<JwtMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}