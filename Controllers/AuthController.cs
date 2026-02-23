using InventarioAPI.Data;
using InventarioAPI.DTO.Auth;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Models.Auth;
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

        // 1. REGISTRAR ADMINISTRADOR (Dueño inicial)
        [HttpPost("registrar-admin")]
        public async Task<IActionResult> RegistrarAdmin([FromBody] RegistrarAdminDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo ya existe en el sistema.");

            var admin = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                Rol = "Admin" // Asignamos el string directamente como en tu Lucidspark
            };

            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();
            return Ok("Administrador creado correctamente. Ahora inicie sesión para registrar su empresa.");
        }

        // 2. CREAR EMPRESA (El Admin logueado crea su sede principal)
        [Authorize(Roles = "Admin")]
        [HttpPost("crear-empresa")]
        public async Task<IActionResult> CrearEmpresa([FromBody] CrearEmpresaDTO dto)
        {
            // Obtenemos el ID del usuario desde el Token
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null) return Unauthorized("Token no válido o expirado.");
            int usuarioId = int.Parse(usuarioIdClaim.Value);

            var usuarioDb = await _context.Usuarios.FindAsync(usuarioId);
            if (usuarioDb == null) return BadRequest("Usuario no encontrado.");

            if (usuarioDb.IdEmpresa != null)
                return BadRequest("Este administrador ya tiene una empresa vinculada.");

            var empresa = new Empresa
            {
                Nombre = dto.Nombre,
                NIT_RUT = dto.NIT_RUT,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                CorreoContacto = dto.CorreoContacto,
                TipoNegocio = dto.TipoNegocio,
                PropietarioId = usuarioId,
                FechaRegistro = DateTime.Now
            };

            // Usamos transacción para asegurar que se cree la empresa y se actualice el usuario al tiempo
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync();

                // Vinculamos al admin con su nueva empresa
                usuarioDb.IdEmpresa = empresa.Id;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Empresa creada y vinculada exitosamente",
                    empresaId = empresa.Id,
                    tipo = empresa.TipoNegocio.ToString()
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"Error en el proceso: {ex.Message}");
            }
        }

        // 3. REGISTRAR EMPLEADO (El Admin crea personal para SU empresa)
        [Authorize(Roles = "Admin")]
        [HttpPost("registrar-empleado")]
        public async Task<IActionResult> RegistrarEmpleado([FromBody] RegistrarEmpleadoDTO dto)
        {
            // Extraemos el IdEmpresa que viene en el Token del Admin
            var empresaIdClaim = User.FindFirst("IdEmpresa")?.Value;

            if (string.IsNullOrEmpty(empresaIdClaim) || empresaIdClaim == "0")
                return BadRequest("El administrador primero debe crear una empresa.");

            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo ya está registrado para otro empleado.");

            var empleado = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                Rol = "Empleado",
                IdEmpresa = int.Parse(empresaIdClaim) // Se vincula automáticamente a la misma empresa del Admin
            };

            _context.Usuarios.Add(empleado);
            await _context.SaveChangesAsync();
            return Ok("Empleado registrado y vinculado a su empresa con éxito.");
        }

        // 4. LOGIN (Entrega el Token con IdEmpresa incluido)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuario == null || !_passwordService.Verificar(dto.Password, usuario.PasswordHash))
                return Unauthorized("Correo o contraseña incorrectos.");

            // Generamos el token usando el servicio que configuramos
            var token = _tokenService.GenerarToken(usuario);

            return Ok(new
            {
                token = token,
                usuario = new
                {
                    id = usuario.IdUsuario,
                    nombre = usuario.Nombre,
                    rol = usuario.Rol,
                    empresaId = usuario.IdEmpresa
                }
            });
        }
    }
}