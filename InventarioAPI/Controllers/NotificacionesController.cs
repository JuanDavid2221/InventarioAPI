using InventarioAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly InventarioContext _context;
        public NotificacionesController(InventarioContext context) => _context = context;

        // Obtener las últimas 20 notificaciones para la campana
        [HttpGet]
        public async Task<IActionResult> ListarNotificaciones()
        {
            var notis = await _context.Notificaciones
                .OrderByDescending(n => n.Fecha)
                .Take(20)
                .ToListAsync();
            return Ok(notis);
        }

        // Marcar como leída (limpiar la campana)
        [HttpPut("{id}/leer")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            var noti = await _context.Notificaciones.FindAsync(id);
            if (noti == null) return NotFound();

            noti.Leida = true;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Notificación leída" });
        }
    }
}