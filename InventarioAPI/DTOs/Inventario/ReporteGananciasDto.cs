namespace InventarioAPI.DTOs.Inventario
{
    public class ReporteGananciasDto
    {
        public string PeriodoConsultado { get; set; } = string.Empty;
        public string Desde { get; set; } = string.Empty;
        public decimal TotalVentas { get; set; }
        public decimal TotalCostos { get; set; }
        public decimal GananciaNeta { get; set; }
        public int CantidadVentas { get; set; }
    }
}