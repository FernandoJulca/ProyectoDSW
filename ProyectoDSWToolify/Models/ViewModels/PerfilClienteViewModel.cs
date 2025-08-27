namespace ProyectoDSWToolify.Models.ViewModels
{
    public class PerfilClienteViewModel
    {
        public int idCliente { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<Venta> HistorialVentas { get; set; }
    }
}
