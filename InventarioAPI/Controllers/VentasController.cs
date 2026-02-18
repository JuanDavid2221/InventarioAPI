using InventarioAPI.Data;
using InventarioAPI.Models.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly InventarioContext _context;

        public VentasController(InventarioContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult> RegistrarVenta(Venta venta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                venta.Fecha = DateTime.Now;
                decimal totalVenta = 0;

                // 🔔 Lista para respuesta inmediata en Postman
                var alertasGeneradas = new List<string>();

                foreach (var detalle in venta.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);

                    if (producto == null || producto.Stock < detalle.Cantidad)
                        return BadRequest($"Stock insuficiente o producto no encontrado para ID: {detalle.ProductoId}");

                    // 📉 Restamos del inventario
                    producto.Stock -= detalle.Cantidad;

                    // 🚩 VALIDACIÓN Y PERSISTENCIA DE NOTIFICACIONES
                    if (producto.Stock <= producto.StockMinimo)
                    {
                        string mensaje = producto.Stock == 0
                            ? $"🚨 PRODUCTO AGOTADO: {producto.Nombre}. ¡Surtir urgente!"
                            : $"⚠️ STOCK BAJO: {producto.Nombre}. Quedan {producto.Stock} unidades.";

                        // Se agrega a la tabla física
                        _context.Notificaciones.Add(new Notificacion
                        {
                            Mensaje = mensaje,
                            Tipo = producto.Stock == 0 ? "Critico" : "Advertencia",
                            Fecha = DateTime.Now,
                            Leida = false
                        });

                        alertasGeneradas.Add(mensaje);
                    }

                    detalle.PrecioUnitario = producto.PrecioVenta;
                    totalVenta += (detalle.Cantidad * producto.PrecioVenta);
                }

                venta.Total = totalVenta;
                _context.Ventas.Add(venta);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Venta realizada y stock actualizado",
                    total = venta.Total,
                    notificaciones = alertasGeneradas // Esto se ve en el JSON de respuesta
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ListarVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            return Ok(ventas);
        }
    }
}