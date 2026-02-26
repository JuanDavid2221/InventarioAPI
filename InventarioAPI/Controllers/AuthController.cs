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

        // 1. REGISTRAR ADMINISTRADOR
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
                Rol = "Admin" // 🔥 Aseguramos que se guarde como Admin en la DB
            };

            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();
            return Ok("Administrador creado correctamente. Ahora inicie sesión para registrar su empresa.");
        }

        // 2. CREAR EMPRESA
        [Authorize(Roles = "Admin")]
        [HttpPost("crear-empresa")]
        public async Task<IActionResult> CrearEmpresa([FromBody] CrearEmpresaDTO dto)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null) return Unauthorized("Token no válido.");

            int usuarioId = int.Parse(usuarioIdClaim.Value);
            var usuarioDb = await _context.Usuarios.FindAsync(usuarioId);

            if (usuarioDb == null) return BadRequest("Usuario no encontrado.");
            if (usuarioDb.IdEmpresa != null) return BadRequest("Ya tienes una empresa vinculada.");

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

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync();

                usuarioDb.IdEmpresa = empresa.Id;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { mensaje = "Empresa creada exitosamente", empresaId = empresa.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // 3. REGISTRAR EMPLEADO
        [Authorize(Roles = "Admin")]
        [HttpPost("registrar-empleado")]
        public async Task<IActionResult> RegistrarEmpleado([FromBody] RegistrarEmpleadoDTO dto)
        {
            // Extraemos IdEmpresa del Token generado en el Login
            var empresaIdClaim = User.FindFirst("IdEmpresa")?.Value;

            if (string.IsNullOrEmpty(empresaIdClaim) || empresaIdClaim == "0")
                return BadRequest("Debes crear una empresa antes de registrar empleados.");

            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo ya está registrado.");

            var empleado = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                Rol = "Empleado",
                IdEmpresa = int.Parse(empresaIdClaim)
            };

            _context.Usuarios.Add(empleado);
            await _context.SaveChangesAsync();
            return Ok("Empleado registrado con éxito.");
        }

        // 4. LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuario == null || !_passwordService.Verificar(dto.Password, usuario.PasswordHash))
                return Unauthorized("Credenciales incorrectas.");

            // 🔥 Si borraste datos y el Rol quedó vacío por error manual en SQL, 
            // esto asegura que el token no falle.
            if (string.IsNullOrEmpty(usuario.Rol)) usuario.Rol = "Admin";

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