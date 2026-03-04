using InventarioAPI.Models.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Data
{
    public static class DbInitializer
    {
        public static void Inicializar(IServiceProvider serviceProvider)
        {
            using var context = new InventarioContext(
                serviceProvider.GetRequiredService<DbContextOptions<InventarioContext>>());

            // Crea la base si no existe
            context.Database.EnsureCreated();

            // Si ya hay roles, no hacemos nada
            if (context.Roles.Any()) return;

            var roles = new Rol[]
            {
                new Rol { Id = 1, Nombre = "Admin" },
                new Rol { Id = 2, Nombre = "Empleado" }
            };

            using var transaction = context.Database.BeginTransaction();
            try
            {
                // Comando para permitir insertar IDs manuales en SQL Server
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Roles ON");
                context.Roles.AddRange(roles);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Roles OFF");
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                // Intento alternativo por si no es SQL Server
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }
        }
    }
}