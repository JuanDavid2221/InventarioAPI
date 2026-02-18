using InventarioAPI.Data;
using InventarioAPI.DTOs.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly InventarioContext _context;

        public ReportesController(InventarioContext context)
        {
            _context = context;
        }

        // 1. REPORTE POR PERIODOS (Día, Mes, Año)
        [HttpGet("ganancias")]
        public async Task<ActionResult<ReporteGananciasDto>> ObtenerGanancias([FromQuery] string periodo = "diario")
        {
            try
            {
                DateTime fechaInicio = periodo.ToLower() switch
                {
                    "diario" => DateTime.Today,
                    "semanal" => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek),
                    "mensual" => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                    "anual" => new DateTime(DateTime.Today.Year, 1, 1),
                    _ => DateTime.Today
                };

                return await CalcularReporte(fechaInicio, DateTime.Now, null, periodo);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // 2. REPORTE PROFESIONAL (Calendarios y Empleados)
        [HttpGet("ganancias-detalladas")]
        public async Task<ActionResult> ObtenerGananciasDetalladas(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin,
            [FromQuery] int? usuarioId)
        {
            try
            {
                var inicio = fechaInicio ?? DateTime.Today;
                var fin = fechaFin ?? DateTime.Now;

                return await CalcularReporte(inicio, fin, usuarioId, "personalizado");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // MÉTODO AUXILIAR PARA NO REPETIR CÓDIGO
        private async Task<ActionResult> CalcularReporte(DateTime inicio, DateTime fin, int? usuarioId, string periodo)
        {
            var query = _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(v => v.Fecha >= inicio && v.Fecha <= fin);

            if (usuarioId.HasValue)
            {
                query = query.Where(v => v.UsuarioId == usuarioId.Value);
            }

            var ventas = await query.ToListAsync();
            decimal totalVentas = ventas.Sum(v => v.Total);
            decimal totalCostos = 0;

            foreach (var v in ventas)
            {
                foreach (var d in v.Detalles)
                {
                    if (d.Producto != null)
                        totalCostos += (d.Cantidad * d.Producto.PrecioCompra);
                }
            }

            return Ok(new
            {
                Periodo = periodo,
                Rango = new { Desde = inicio.ToString("dd/MM/yyyy"), Hasta = fin.ToString("dd/MM/yyyy") },
                UsuarioFiltro = usuarioId.HasValue ? usuarioId.ToString() : "Todos",
                TotalVentas = totalVentas,
                TotalCostos = totalCostos,
                GananciaNeta = totalVentas - totalCostos,
                CantidadVentas = ventas.Count
            });
        }
    }
}