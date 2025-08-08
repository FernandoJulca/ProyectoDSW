using System.ComponentModel.DataAnnotations;

namespace ProyectoDSWToolify.Models
{
    public class Categoria
    {
        [Display(Name = "Id Categoria")] public int idCategoria { get; set; }
        [Display(Name = "Descripcion")] public string descripcion { get; set; }
    }
}
