using InventarioAPI.DTOs.Auth;

namespace InventarioAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegistrarEmpleado(RegistrarEmpleadoDTO dto, int empresaId);
    }
}