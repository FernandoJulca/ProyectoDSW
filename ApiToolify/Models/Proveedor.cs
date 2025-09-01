using System.ComponentModel.DataAnnotations;

namespace ProyectoDSWToolify.Models
{
    public class Proveedor
    {
        [Display(Name = "Id Proveedor")] public int idProveedor { get; set; }
        [Display(Name = "RUC")] public string? ruc { get; set; }
        [Display(Name = "Razon Social")] public string? razonSocial { get; set; }
        [Display(Name = "Telefono")] public string? telefono { get; set; }
        [Display(Name = "Direccion")] public string? direccion { get; set; }
        [Display(Name = "Distrito")] public Distrito? distrito { get; set; }
        [Display(Name = "Registro")] public DateTime? fechaRegistro { get; set; }
        [Display(Name = "Estado")] public bool? estado { get; set; }
    }
}
