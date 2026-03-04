using Microsoft.EntityFrameworkCore;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Models.Proveedores;
using InventarioAPI.Models.Empresa;

namespace InventarioAPI.Data
{
    public class InventarioContext : DbContext
    {
        public InventarioContext(DbContextOptions<InventarioContext> options) : base(options) { }

        // Tablas principales
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Empresa> Empresas => Set<Empresa>();

        // --- NUEVA TABLA AGREGADA ---
        public DbSet<Proveedor> Proveedores => Set<Proveedor>();

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
                entity.HasKey(u => u.IdUsuario);

                // Relación: Muchos usuarios pertenecen a una Empresa
                entity.HasOne(u => u.Empresa)
                      .WithMany(e => e.Usuarios)
                      .HasForeignKey(u => u.IdEmpresa)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. Configuración de la Tabla Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.Id);
            });

            // --- 4. CONFIGURACIÓN DE LA TABLA PROVEEDORES ---
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("Proveedores");
                entity.HasKey(p => p.Id);

                // --- MEJORA PROFESIONAL: ÍNDICE ÚNICO ---
                // Esto evita que exista el mismo NIT dos veces para la misma empresa a nivel de SQL
                entity.HasIndex(p => new { p.NIT, p.IdEmpresa }).IsUnique();

                // Relación: Muchos proveedores pertenecen a una Empresa
                entity.HasOne(p => p.Empresa)
                      .WithMany()
                      .HasForeignKey(p => p.IdEmpresa)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Semilla de datos para Roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Admin" },
                new Rol { Id = 2, Nombre = "Empleado" }
            );
        }
    }
}