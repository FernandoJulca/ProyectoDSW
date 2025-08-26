namespace ProyectoDSWToolify.Models.ViewModels
{
    public class RepartidorIndexViewModel
    {
        public int TotalPedidosEntregados { get; set; }
        public int TotalPedidosPendientes { get; set; }
        public List<Venta> PedidosPendientes { get; set; }
        public Venta VentaEnRuta { get; set; }
        public Usuario Usuario { get; set; }
    }
}
