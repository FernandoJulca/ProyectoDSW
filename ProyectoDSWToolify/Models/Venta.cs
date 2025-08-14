using System.ComponentModel.DataAnnotations;

namespace ProyectoDSWToolify.Models
{
    public class Venta
    {
        [Display(Name = "ID Venta")] public int idVenta { get; set; }
        [Display(Name = "Usuario")] public Usuario usuario { get; set; }
        [Display(Name = "Fecha")] public DateTime fecha { get; set; }
        [Display(Name = "Total")] public decimal  total { get; set; }
        [Display(Name = "Tipo Venta")] public string tipoVenta { get; set; }
        public DateTime Fecha { get; set; }
        public string estado { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();

    }
}
