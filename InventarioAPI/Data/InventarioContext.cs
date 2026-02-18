using Microsoft.EntityFrameworkCore;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Models.Inventario;

namespace InventarioAPI.Data
{
    public class InventarioContext : DbContext
    {
        public InventarioContext(DbContextOptions<InventarioContext> options) : base(options)
        {
        }

        // =========================
        // TABLAS SEGURIDAD
        // =========================
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();

        // =========================
        // TABLAS INVENTARIO
        // =========================
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Producto> Productos => Set<Producto>();

        // 🟢 TABLAS DE VENTAS
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();
        public DbSet<Notificacion> Notificaciones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // CONFIGURACION USUARIO
            // =========================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Correo).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Correo).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.HasOne(u => u.Rol).WithMany(r => r.Usuarios).HasForeignKey(u => u.RolId).OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // CONFIGURACION ROLES
            // =========================
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Nombre).IsRequired().HasMaxLength(50);
            });

            // =========================
            // CONFIGURACION CATEGORIA
            // =========================
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            });

            // =========================
            // CONFIGURACION PRODUCTO (Actualizada)
            // =========================
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
                entity.HasIndex(p => p.CodigoBarras).IsUnique();
                entity.Property(p => p.CodigoBarras).IsRequired().HasMaxLength(50);

                // Nuevos campos para que coincidan con la DB
                entity.Property(p => p.Marca).HasMaxLength(100);
                entity.Property(p => p.UnidadMedida).HasMaxLength(50);
                entity.Property(p => p.FechaVencimiento).IsRequired(false);

                entity.Property(p => p.PrecioCompra).HasColumnType("decimal(10,2)");
                entity.Property(p => p.PrecioVenta).HasColumnType("decimal(10,2)");
                entity.Property(p => p.Stock).IsRequired();
                entity.Property(p => p.StockMinimo).IsRequired();
                entity.Property(p => p.Activo).HasDefaultValue(true);
                entity.Property(p => p.FechaRegistro).HasDefaultValueSql("GETDATE()");

                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.CategoriaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // CONFIGURACION VENTAS
            // =========================
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("Ventas");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Total).HasColumnType("decimal(18,2)");
                entity.HasOne(v => v.Usuario)
                      .WithMany()
                      .HasForeignKey(v => v.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("DetalleVentas");
                entity.HasKey(dv => dv.Id);
                entity.Property(dv => dv.PrecioUnitario).HasColumnType("decimal(18,2)");

                entity.HasOne(dv => dv.Venta)
                      .WithMany(v => v.Detalles)
                      .HasForeignKey(dv => dv.VentaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(dv => dv.Producto)
                      .WithMany()
                      .HasForeignKey(dv => dv.ProductoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // SEED ROLES
            // =========================
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Admin" },
                new Rol { Id = 2, Nombre = "Empleado" }
            );
        }
    }
}