using InventarioAPI.Data;
using InventarioAPI.DTOs.Auth;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly InventarioContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthController(
            InventarioContext context,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            // Validación básica
            if (dto == null || string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new
                {
                    mensaje = "Debe enviar correo y contraseña"
                });
            }

            // Buscar usuario
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "El usuario no existe"
                });
            }

            // Verificar contraseña
            bool passwordCorrecto = _passwordService.Verificar(dto.Password, usuario.PasswordHash);

            if (!passwordCorrecto)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos"
                });
            }

            // Generar token
            var token = _tokenService.GenerarToken(usuario);

            // 🔐 Enviar token en HEADER (no en el body)
            Response.Headers["Authorization"] = "Bearer " + token;

            // Respuesta limpia (como sistema empresarial)
            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso"

            });
        }
        // =========================
        // REGISTRAR EMPLEADO (SOLO ADMIN)
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPost("registrar-empleado")]
        public async Task<IActionResult> RegistrarEmpleado([FromBody] RegistrarEmpleadoDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Nombre) ||
                string.IsNullOrEmpty(dto.Correo) ||
                string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest(new
                {
                    mensaje = "Todos los campos son obligatorios"
                });
            }

            // Verificar si el correo ya existe
            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);

            if (existe)
            {
                return BadRequest(new
                {
                    mensaje = "El correo ya está registrado"
                });
            }

            // Crear empleado
            var empleado = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                RolId = 2 // 2 = Empleado
            };

            _context.Usuarios.Add(empleado);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Empleado registrado correctamente"
            });
        }

    }

}
