namespace ApiToolify.Models.DTO
{
    public class ListarProductosPorCategoriaDTO
    {
        public int idProducto { get; set; }
        public string? nombre { get; set; }
        public string? descripcion { get; set; }
        public string? proveedor { get; set; }
        public string? categoria { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public DateTime fechaRegistro { get; set; }
    }
}
