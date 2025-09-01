namespace ProyectoDSWToolify.Models.ViewModels
{
    public class VentaViewModel
    {

        public int idVenta { get; set; }
        public int idUsuario { get; set; }
        public DateTime fecha { get; set; }
        public decimal total { get; set; }
        public string? tipoVenta { get; set; }
        public string? estado { get; set; }
        public List<DetalleVentaViewModel> detalles { get; set; } = new();
    }
}
