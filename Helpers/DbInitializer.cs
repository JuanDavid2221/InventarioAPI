using InventarioAPI.Data;
using InventarioAPI.Models.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventarioAPI.Helpers
{
    public static class DbInitializer
    {
        public static void Inicializar(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InventarioContext>();

            // Crear base si no existe
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Rol { Id = 1, Nombre = "Admin" },
                    new Rol { Id = 2, Nombre = "Empleado" }
                );

                context.SaveChanges();
            }
        }
    }
}