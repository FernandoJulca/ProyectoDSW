namespace ProyectoDSWToolify.Models.ViewModels
{
    public class ProductosViewModel
    {
        public List<Categoria> Categorias { get; set; }
        public List<Producto> Productos { get; set; }
        public List<int> IdCategoriasSeleccionadas { get; set; } = new();
        public string OrdenPrecio { get; set; } = "asc"; // "asc" o "desc"

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TamañoPagina { get; set; } = 12;
    }
}
