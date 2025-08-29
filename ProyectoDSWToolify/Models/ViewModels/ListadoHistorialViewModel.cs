namespace ProyectoDSWToolify.Models.ViewModels
{
    public class ListadoHistorialViewModel
    {
        public int idVendedor { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public List<Venta> HistorialVentas { get; set; }
    }
}
