namespace InventarioAPI.Models
{
	public class Categoria
	{
		public int Id { get; set; }

		public int NegocioId { get; set; }

		public string Nombre { get; set; }

		public int? CategoriaPadreId { get; set; }

		public bool Activo { get; set; } = true;

		public ICollection<Producto> Productos { get; set; }
	}



}