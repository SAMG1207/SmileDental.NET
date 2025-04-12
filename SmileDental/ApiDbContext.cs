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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la estrategia TPT para la herencia (no se creará una tabla para UsuarioAbstract)
            modelBuilder.Entity<Dentista>().ToTable("Dentistas");  // Asegúrate de que la tabla para Dentista sea explícita
            modelBuilder.Entity<Paciente>().ToTable("Pacientes");  // Lo mismo para Pacientes

            // Configuración de relaciones
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
