using ColegioSanJose.Models;
using Microsoft.EntityFrameworkCore;

namespace ColegioSanJose.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Expediente> Expedientes { get; set; }
        public DbSet<Carrera> Carreras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Expediente>()
                .HasOne(e => e.Alumno)
                .WithMany(a => a.Expedientes)
                .HasForeignKey(e => e.AlumnoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expediente>()
                .HasOne(e => e.Materia)
                .WithMany(m => m.Expedientes)
                .HasForeignKey(e => e.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expediente>()
                .Property(e => e.NotaFinal)
                .HasColumnType("decimal(5,2)");
        }
    }
}