using InventarioAPI.Data;
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
    public class UsuarioController : ControllerBase
    {
        private readonly InventarioContext _context;

        public UsuarioController(InventarioContext context)
        {
            _context = context;
        }

        // 1. LISTAR EMPLEADOS DE MI EMPRESA
        [HttpGet("mis-empleados")]
        public async Task<IActionResult> Listar()
        {
            var empresaId = int.Parse(User.FindFirst("IdEmpresa")!.Value);
            var empleados = await _context.Usuarios
                .Where(u => u.IdEmpresa == empresaId && u.RolId != 1) // No listar otros admins
                .Select(u => new { u.IdUsuario, u.Nombre, u.Correo, u.Activo, u.FechaRegistro })
                .ToListAsync();

            return Ok(empleados);
        }

        // 2. DESACTIVAR EMPLEADO (Baja Lógica)
        [HttpPatch("desactivar/{id}")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var empresaId = int.Parse(User.FindFirst("IdEmpresa")!.Value);
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.IdEmpresa == empresaId);

            if (usuario == null) return NotFound(new { mensaje = "Empleado no encontrado." });

            usuario.Activo = false;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Empleado desactivado. Ya no podrá iniciar sesión." });
        }

        // 3. REACTIVAR EMPLEADO
        [HttpPatch("reactivar/{id}")]
        public async Task<IActionResult> Reactivar(int id)
        {
            var empresaId = int.Parse(User.FindFirst("IdEmpresa")!.Value);
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.IdEmpresa == empresaId);

            if (usuario == null) return NotFound();

            usuario.Activo = true;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Empleado reactivado correctamente." });
        }

        // 4. ELIMINAR FÍSICAMENTE (Solo si no tiene historial)
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var empresaId = int.Parse(User.FindFirst("IdEmpresa")!.Value);
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.IdEmpresa == empresaId);

            if (usuario == null) return NotFound();

            // Aquí podrías validar si el empleado ha hecho ventas antes de borrarlo
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario eliminado permanentemente de la base de datos." });
        }
    }
}