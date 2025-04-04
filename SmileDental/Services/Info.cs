using Microsoft.EntityFrameworkCore;
using SmileDental.DTOs.Administrador;
using SmileDental.Models;
using SmileDental.Services.Interfaces;

namespace SmileDental.Services
{
    public class Info(ApiDbContext context) : IInfo
    {
        private readonly ApiDbContext _context = context;

        public async Task<List<string>> VerEspecialidades()
        {
            try
            {
                var especialidades = await _context.Especialidades.Select(e => e.Nombre).ToListAsync();
                return especialidades;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<List<PresentacionDentistaDTO>> VerUrlFotosDentistasEspecialidad()
        {
            try
            {
                var datosAMostrar = await _context.Dentistas
                    .Where(d => d.EspecialidadId != 6)
                    .Select(d => new
                    {
                        d.Nombre,
                        d.Apellido,
                        d.FotoUrl,
                        Especialidad = d.Especialidad.Nombre,

                    })
                    .OrderBy(d => d.Nombre)
                    .ToListAsync();


                List<PresentacionDentistaDTO> dentistas = new();

                foreach (var d in datosAMostrar)
                {
                    dentistas.Add(new PresentacionDentistaDTO
                    {
                        nombre = d.Nombre,
                        apellido = d.Apellido,
                        urlFoto = d.FotoUrl,
                        especialidad = d.Especialidad
                    });
                }

                return dentistas;

            }
            catch (Exception)
            {
                return new List<PresentacionDentistaDTO>();
            }
        }
    }
}
