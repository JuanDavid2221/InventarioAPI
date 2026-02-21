using Microsoft.EntityFrameworkCore;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Models.Inventario;

namespace InventarioAPI.Data
{
    public class InventarioContext : DbContext
    {
        public InventarioContext(DbContextOptions<InventarioContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Empresa> Empresas => Set<Empresa>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("Empresas");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Propietario)
                      .WithOne()
                      .HasForeignKey<Empresa>(e => e.PropietarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasOne(u => u.Empresa)
                      .WithMany(e => e.Usuarios)
                      .HasForeignKey(u => u.EmpresaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Admin" },
                new Rol { Id = 2, Nombre = "Empleado" }
            );
        }
    }
}