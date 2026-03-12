namespace InventarioAPI.Models
{
	public class Producto
	{
		public int Id { get; set; }

		public int NegocioId { get; set; }

		public int CategoriaId { get; set; }

		public string Nombre { get; set; }

		public decimal PrecioVenta { get; set; }

        public bool Activo { get; set; } = true;

        public int Stock { get; set; }

		public Categoria Categoria { get; set; }
	}
}