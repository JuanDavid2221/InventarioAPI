using InventarioAPI.Data;
using InventarioAPI.DTO.Auth;
using InventarioAPI.DTOs.Auth;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly InventarioContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthController(InventarioContext context, IPasswordService passwordService, ITokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        // 1. REGISTRAR ADMIN
        [HttpPost("registrar-admin")]
        public async Task<IActionResult> RegistrarAdmin([FromBody] RegistrarAdminDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest(new { mensaje = "El correo ya existe." });

            var admin = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                RolId = 1,
                FechaRegistro = DateTime.Now,
                Activo = true
            };

            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Admin creado. Ya puede iniciar sesión." });
        }

        // 2. LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var usuario = await _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuario == null || !_passwordService.Verificar(dto.Password, usuario.PasswordHash))
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            var token = _tokenService.GenerarToken(usuario);
            return Ok(new { token, usuario = new { id = usuario.IdUsuario, nombre = usuario.Nombre, rol = usuario.Rol?.Nombre, empresaId = usuario.IdEmpresa } });
        }

        // 3. REGISTRAR EMPLEADO
        [Authorize(Roles = "Admin")]
        [HttpPost("registrar-empleado")]
        public async Task<IActionResult> RegistrarEmpleado([FromBody] RegistrarAdminDTO dto)
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim)) return Unauthorized();

            var adminDb = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == int.Parse(adminIdClaim));

            if (adminDb?.IdEmpresa == null)
                return BadRequest(new { mensaje = "Primero debes crear una empresa para registrar empleados." });

            var empleado = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                RolId = 2,
                IdEmpresa = adminDb.IdEmpresa,
                FechaRegistro = DateTime.Now,
                Activo = true
            };

            _context.Usuarios.Add(empleado);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Empleado registrado y vinculado a tu empresa." });
        }

        // 4. CAMBIO DE CONTRASEÑA (ACTUALIZADO 🔐)
        [Authorize]
        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDTO dto)
        {
            // Validar que las nuevas contraseñas coincidan
            if (dto.PasswordNueva != dto.ConfirmarPasswordNueva)
            {
                return BadRequest(new { mensaje = "La nueva contraseña y su confirmación no coinciden." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var usuario = await _context.Usuarios.FindAsync(int.Parse(userIdClaim));

            // Validar que el usuario exista y que la contraseña actual sea correcta
            if (usuario == null || !_passwordService.Verificar(dto.PasswordActual, usuario.PasswordHash))
                return BadRequest(new { mensaje = "La contraseña actual es incorrecta." });

            // Actualizar el Hash con la nueva clave
            usuario.PasswordHash = _passwordService.Hash(dto.PasswordNueva);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Contraseña actualizada exitosamente." });
        }
    }

    public class CambiarPasswordDTO
    {
        public string PasswordActual { get; set; } = string.Empty;
        public string PasswordNueva { get; set; } = string.Empty;
        public string ConfirmarPasswordNueva { get; set; } = string.Empty;
    }
}