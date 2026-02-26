using InventarioAPI.Models.Seguridad;

namespace InventarioAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerarToken(Usuario usuario);
    }
}
