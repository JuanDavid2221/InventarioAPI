using InventarioAPI.Data;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Helpers
{
    public static class DbInitializer
    {
        public static void Inicializar(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InventarioContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

            // Aplica migraciones automáticamente
            context.Database.Migrate();

            // Si NO existe un admin → lo crea
            if (!context.Usuarios.Any(u => u.RolId == 1))
            {
                var admin = new Usuario
                {
                    Nombre = "Administrador",
                    Correo = "admin@inventario.com",
                    PasswordHash = passwordService.Hash("Admin123!"),
                    RolId = 1
                };

                context.Usuarios.Add(admin);
                context.SaveChanges();

                Console.WriteLine("ADMIN CREADO AUTOMATICAMENTE");
            }
        }
    }
}
