using System.ComponentModel.DataAnnotations;

namespace ProyectoDSWToolify.Models
{
    public class Rol
    {
        [Display(Name = "Id Rol")] public int idRol { get; set; }
        [Display(Name = "Descripcion")] public string? descripcion { get; set; }
    }
}
