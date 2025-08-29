namespace ApiToolify.Models.DTO
{
    public class ListadoVentaFechaAndTipoVentaDTO
    {
        public int idVenta { get; set; }
        public string? nombresCompletos { get; set; }
        public string?  direccion { get; set; }
        public DateTime fechaGenerada { get; set; }
        public decimal total { get; set; }
        public string estado { get; set; }
        public string tipoVenta { get; set; }
    }
}
