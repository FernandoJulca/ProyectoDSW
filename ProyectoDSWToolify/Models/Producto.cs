using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectoDSWToolify.Models
{
    public class Producto
    {
        [Display(Name = "ID Producto")] public int idProducto { get; set; }
        [Display(Name = "Nombre")] public string nombre { get; set; }
        [Display(Name = "Descripcion")] public string descripcion { get; set; }
        [Display(Name = "Proveedor")] public Proveedor proveedor { get; set; }
        [Display(Name = "Categoria" )] public Categoria categoria { get; set; }
        [Display(Name = "Precio")] public decimal precio { get; set; }
        [Display(Name = "Stock")] public int stock { get; set; }
        [Display(Name = "Registro")] public DateTime fechaRegistro { get; set; }
        [Display(Name = "Estado")] public bool estado { get; set; }
        [Display(Name = "Imagen")] public string? imagen { get; set; }

        [NotMapped] 
        public IFormFile? file { get; set; }

    }
}
