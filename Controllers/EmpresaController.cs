using InventarioAPI.Data;
using InventarioAPI.DTOs.Auth;
using InventarioAPI.DTOs.Empresa;
using InventarioAPI.Models.Empresa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventarioAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly InventarioContext _context;

        public EmpresaController(InventarioContext context)
        {
            _context = context;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearEmpresa([FromBody] CrearEmpresaDTO dto)
        {
            // 1. Obtener el ID del usuario desde el token actual
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { mensaje = "Token no válido o sesión expirada." });

            int userId = int.Parse(userIdClaim);

            // 2. Buscar al usuario en la base de datos
            var usuarioDb = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == userId);

            if (usuarioDb == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            if (usuarioDb.IdEmpresa != null)
                return BadRequest(new { mensaje = "Este usuario ya tiene una empresa vinculada." });

            // --- NUEVA VALIDACIÓN DE SEGURIDAD PROFESIONAL ---
            // 2.1. Verificar si el NIT ya existe en el sistema (independientemente del usuario)
            var existeEmpresaConEseNit = await _context.Empresas
                .AnyAsync(e => e.NIT_RUT == dto.NIT_RUT);

            if (existeEmpresaConEseNit)
                return BadRequest(new { mensaje = $"La empresa con NIT {dto.NIT_RUT} ya se encuentra registrada en el sistema." });
            // -------------------------------------------------

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 3. Crear el objeto de la nueva Empresa
                var empresa = new Empresa
                {
                    Nombre = dto.Nombre,
                    NIT_RUT = dto.NIT_RUT ?? "0",
                    Direccion = dto.Direccion,
                    Telefono = dto.Telefono,
                    CorreoContacto = dto.CorreoContacto,
                    TipoNegocio = dto.TipoNegocio,
                    FechaRegistro = DateTime.Now,
                    PropietarioId = userId
                };

                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync();

                // 4. Vincular al usuario con el ID de la empresa recién creada
                usuarioDb.IdEmpresa = empresa.Id;
                _context.Usuarios.Update(usuarioDb);
                await _context.SaveChangesAsync();

                // 5. Confirmar la transacción en la base de datos
                await transaction.CommitAsync();

                // 6. Respuesta limpia
                return Ok(new
                {
                    mensaje = "Empresa creada y vinculada exitosamente",
                    empresaId = empresa.Id,
                    negocio = empresa.TipoNegocio.ToString()
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { mensaje = $"Error en la creación: {ex.InnerException?.Message ?? ex.Message}" });
            }
        }
    }
}