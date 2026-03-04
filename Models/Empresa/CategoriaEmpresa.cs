namespace InventarioAPI.Models.Empresa
{
    public class CategoriaEmpresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Ejemplo: "Tienda", "Supermercado"
        public bool Activa { get; set; } = true;
    }
}