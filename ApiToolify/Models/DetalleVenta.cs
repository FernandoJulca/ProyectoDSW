using System.ComponentModel.DataAnnotations;

namespace ProyectoDSWToolify.Models
{
    public class DetalleVenta
    {
        [Display(Name = "ID Detalle")] public int idDetalleVenta { get; set; }
        [Display(Name = "ID Venta")] public Venta venta { get; set; }
        [Display(Name = "ID Producto")] public Producto producto { get; set; }
        [Display(Name = "Cantidad")] public int cantidad { get; set; }
        [Display(Name = "Sub Total")] public decimal subTotal { get; set; }
    }
}
