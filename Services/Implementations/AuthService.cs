using InventarioAPI.Data;
using InventarioAPI.DTOs.Auth;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly InventarioContext _context;
        private readonly IPasswordService _passwordService;

        public AuthService(InventarioContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<bool> RegistrarEmpleado(RegistrarEmpleadoDTO dto, int empresaId)
        {
            // Verificar si el correo ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return false;

            var nuevoEmpleado = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                RolId = 2, // ID de Rol para Empleado
                IdEmpresa = empresaId,
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(nuevoEmpleado);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}