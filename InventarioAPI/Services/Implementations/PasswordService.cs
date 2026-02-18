using BCrypt.Net;
using InventarioAPI.Services.Interfaces;

namespace InventarioAPI.Services.Implementations
{
    public class PasswordService : IPasswordService
    {
        // Crear hash
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verificar contraseña
        public bool Verificar(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
