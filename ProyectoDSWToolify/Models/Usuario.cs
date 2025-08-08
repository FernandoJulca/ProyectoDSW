using System.ComponentModel.DataAnnotations;

namespace ProyectoDSWToolify.Models
{
    public class Usuario
    {
        [Display(Name = "Id Categoria")] public int idCategoria { get; set; }
        [Display(Name = "Nombres")] public string nombre { get; set; }
        [Display(Name = "Apellido Materno")] public string apeMaterno { get; set; }
        [Display(Name = "Apellido Paterno")] public string apePaterno { get; set; }
        [Display(Name = "Correo")] public string correo { get; set; }
        [Display(Name = "Clave")] public string clave { get; set; }
        [Display(Name = "Documento")] public string nroDoc { get; set; }
        [Display(Name = "Distrito")] public Distrito distrito { get; set; }
        [Display(Name = "Telefono")] public string telefono { get; set; }
        [Display(Name = "Rol")] public Rol rol { get; set; }
        [Display(Name = "Registro")] public DateTime fechaRegistro { get; set; }
        [Display(Name = "Estado")] public bool estado { get; set; }


    }
}
