using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventarioAPI.Data
{
    public class InventarioContextFactory : IDesignTimeDbContextFactory<InventarioContext>
    {
        public InventarioContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InventarioContext>();

            // 🚩 CORRECCIÓN: Se agrega \\SQLEXPRESS y el TrustServerCertificate
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;");

            return new InventarioContext(optionsBuilder.Options);
        }
    }
}