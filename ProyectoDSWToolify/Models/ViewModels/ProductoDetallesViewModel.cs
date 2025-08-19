namespace ProyectoDSWToolify.Models.ViewModels
{
    public class ProductoDetallesViewModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string categoria { get; set; }  
        public decimal precio { get; set; }
        public int stock { get; set; }
        public string? imagen { get; set; }
    }
}
