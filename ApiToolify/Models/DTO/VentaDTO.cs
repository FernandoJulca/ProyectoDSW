namespace ApiToolify.Models.DTO
{
    public class VentaDTO
    {
        public int idVenta { get; set; }
        public int idUsuario { get; set; }
        public DateTime fecha { get; set; }
        public decimal total { get; set; }
        public string? tipoVenta { get; set; }
        public string? estado { get; set; }
        public List<DetalleVentaDTO> detalles { get; set; } = new();
    }

    public class DetalleVentaDTO
    {
        public int idProducto { get; set; }
        public int cantidad { get; set; }
        public decimal subTotal { get; set; }
    }

}
