using InventarioAPI.Data;
using InventarioAPI.DTO.Auth;
using InventarioAPI.Models.Seguridad;
using InventarioAPI.Models.Inventario;
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

        // 1. REGISTRAR ADMINISTRADOR (Dueño)
        [HttpPost("registrar-admin")]
        public async Task<IActionResult> RegistrarAdmin([FromBody] RegistrarAdminDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo ya existe");

            var admin = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                RolId = 1 // Rol Admin
            };

            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();
            return Ok("Admin creado correctamente. Inicie sesión para registrar su empresa.");
        }

        // 2. CREAR EMPRESA (El Admin elige el Tipo)
        [Authorize(Roles = "Admin")]
        [HttpPost("crear-empresa")]
        public async Task<IActionResult> CrearEmpresa([FromBody] CrearEmpresaDTO dto)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null) return Unauthorized("Token inválido");
            int usuarioId = int.Parse(usuarioIdClaim.Value);

            var usuarioDb = await _context.Usuarios.FindAsync(usuarioId);
            if (usuarioDb == null) return BadRequest("Usuario no encontrado.");

            var empresa = new Empresa
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Tipo = dto.Tipo, // Aquí asignamos Tienda, Supermercado o Ventanilla
                PropietarioId = usuarioId
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync();

                usuarioDb.EmpresaId = empresa.Id;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(new { mensaje = "Empresa creada correctamente", empresaId = empresa.Id, tipo = empresa.Tipo.ToString() });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"Error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // 3. REGISTRAR EMPLEADO (El Admin registra personal para SU empresa)
        [Authorize(Roles = "Admin")]
        [HttpPost("registrar-empleado")]
        public async Task<IActionResult> RegistrarEmpleado([FromBody] RegistrarEmpleadoDTO dto)
        {
            // Extraemos el EmpresaId del Admin que está logueado desde su Token
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            if (string.IsNullOrEmpty(empresaIdClaim))
                return BadRequest("El administrador no tiene una empresa asignada.");

            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo del empleado ya existe");

            var empleado = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = _passwordService.Hash(dto.Password),
                RolId = 2, // Rol Empleado
                EmpresaId = int.Parse(empresaIdClaim) // Se vincula automáticamente
            };

            _context.Usuarios.Add(empleado);
            await _context.SaveChangesAsync();
            return Ok("Empleado registrado y vinculado a la empresa con éxito.");
        }

        // 4. LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var usuario = await _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuario == null || !_passwordService.Verificar(dto.Password, usuario.PasswordHash))
                return Unauthorized("Credenciales incorrectas");

            return Ok(new
            {
                token = _tokenService.GenerarToken(usuario),
                usuario = new { usuario.Id, usuario.Nombre, Rol = usuario.Rol?.Nombre, usuario.EmpresaId }
            });
        }
    }
}