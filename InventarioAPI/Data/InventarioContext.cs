using Microsoft.EntityFrameworkCore;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Models.Auth;

namespace InventarioAPI.Data
{
    public class InventarioContext : DbContext
    {
        public InventarioContext(DbContextOptions<InventarioContext> options) : base(options) { }

        // Tablas principales según tus modelos
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Empresa> Empresas => Set<Empresa>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configuración de la Tabla Empresa
            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("Empresas");
                entity.HasKey(e => e.Id);

                // Relación: Una empresa tiene un Propietario (Admin)
                entity.HasOne(e => e.Propietario)
                      .WithOne()
                      .HasForeignKey<Empresa>(e => e.PropietarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 2. Configuración de la Tabla Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(u => u.IdUsuario); // Clave primaria según tu Lucidspark

                // Relación: Muchos usuarios pertenecen a una Empresa
                entity.HasOne(u => u.Empresa)
                      .WithMany(e => e.Usuarios)
                      .HasForeignKey(u => u.IdEmpresa) // FK según tu Lucidspark
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. Configuración de la Tabla Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.Id);
            });

            // Semilla de datos para Roles (Seed Data)
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Admin" },
                new Rol { Id = 2, Nombre = "Empleado" }
            );
        }
    }
}