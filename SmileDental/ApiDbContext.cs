using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmileDental.Models;

namespace SmileDental
{
    public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
    {
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Dentista> Dentistas { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<ActionLogInDb> ActionLogs { get; set; }

        public async Task<IEnumerable<int>> GetHorasLibresPorFecha(DateTime fecha)
        {
            var fechaParam = new SqlParameter("@dtFechaCita", fecha);
            var horasLibres = await this.Database
                .SqlQueryRaw<int>("EXECUTE GetHorasLibresPorFecha @dtFechaCita", fechaParam)
                .ToListAsync(); 
            return horasLibres;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dentista>().ToTable("Dentistas");
            modelBuilder.Entity<Paciente>().ToTable("Pacientes");

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Dentista)
                .WithMany(d => d.Citas)
                .HasForeignKey(c => c.DentistaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<ActionLogInDb>().ToTable("Logs");

            base.OnModelCreating(modelBuilder);
        }
    }
}
