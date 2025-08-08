using System.ComponentModel.DataAnnotations;

namespace ProyectoDSWToolify.Models
{
    public class Distrito
    {
        [Display(Name = "Id Distrito")] public int idDistrito { get; set; }
        [Display(Name = "Nombre Distrito")] public string nombre { get; set; }
    }
}
