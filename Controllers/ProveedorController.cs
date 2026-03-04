using InventarioAPI.Data;
using InventarioAPI.DTOs.Proveedores;
using InventarioAPI.Models.Proveedores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventarioAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedorController : ControllerBase
    {
        private readonly InventarioContext _context;

        public ProveedorController(InventarioContext context)
        {
            _context = context;
        }

        // 1. REGISTRAR PROVEEDOR
        [HttpPost("registrar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Registrar([FromBody] CrearProveedorDTO dto)
        {
            // --- SOLUCIÓN AL PROBLEMA DEL TOKEN ---
            // Intentamos sacar el ID del token primero
            var claimEmpresa = User.FindFirst("IdEmpresa")?.Value;
            int idEmpresa;

            if (string.IsNullOrEmpty(claimEmpresa) || claimEmpresa == "0")
            {
                // Si el token no tiene ID (porque es nuevo), vamos a la DB a buscarlo por seguridad
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
                if (usuario?.IdEmpresa == null)
                    return BadRequest(new { mensaje = "Debes registrar una empresa antes de agregar proveedores." });

                idEmpresa = usuario.IdEmpresa.Value;
            }
            else
            {
                idEmpresa = int.Parse(claimEmpresa);
            }

            // Validar NIT duplicado en la misma empresa
            var existeNit = await _context.Proveedores
                .AnyAsync(p => p.NIT == dto.NIT && p.IdEmpresa == idEmpresa);

            if (existeNit)
                return BadRequest(new { mensaje = $"El NIT {dto.NIT} ya está registrado en su empresa." });

            var proveedor = new Proveedor
            {
                Nombre = dto.Nombre,
                NIT = dto.NIT,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Correo = dto.Correo,
                NombreContacto = dto.NombreContacto,
                IdEmpresa = idEmpresa,
                Activo = true
            };

            try
            {
                _context.Proveedores.Add(proveedor);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Proveedor registrado exitosamente." });
            }
            catch (Exception ex)
            {
                // Esto te ayudará a ver el error real si algo falla en la DB
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("mis-proveedores")]
        public async Task<IActionResult> Listar()
        {
            var idEmpresa = await ObtenerIdEmpresa();
            if (idEmpresa == null) return Unauthorized();

            var proveedores = await _context.Proveedores
                .Where(p => p.IdEmpresa == idEmpresa && p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return Ok(proveedores);
        }

        [HttpPut("actualizar/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProveedorDTO dto)
        {
            var idEmpresa = await ObtenerIdEmpresa();
            if (idEmpresa == null) return Unauthorized();

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(p => p.Id == id && p.IdEmpresa == idEmpresa);

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            proveedor.Nombre = dto.Nombre;
            proveedor.Direccion = dto.Direccion;
            proveedor.Telefono = dto.Telefono;
            proveedor.Correo = dto.Correo;
            proveedor.NombreContacto = dto.NombreContacto;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Proveedor actualizado correctamente." });
        }

        [HttpPatch("desactivar/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var idEmpresa = await ObtenerIdEmpresa();
            if (idEmpresa == null) return Unauthorized();

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(p => p.Id == id && p.IdEmpresa == idEmpresa);

            if (proveedor == null) return NotFound(new { mensaje = "Proveedor no encontrado." });

            proveedor.Activo = false;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Proveedor desactivado correctamente." });
        }

        // Método auxiliar para no repetir código
        private async Task<int?> ObtenerIdEmpresa()
        {
            var claim = User.FindFirst("IdEmpresa")?.Value;
            if (!string.IsNullOrEmpty(claim) && claim != "0") return int.Parse(claim);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return null;

            var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
            return usuario?.IdEmpresa;
        }
    }

    public class CrearProveedorDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? NombreContacto { get; set; }
    }

    public class ActualizarProveedorDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? NombreContacto { get; set; }
    }
}