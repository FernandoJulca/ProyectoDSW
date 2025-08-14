namespace ProyectoDSWToolify.Models.ViewModels
{
    public class CarritoViewModel
    {
        public List<Carro> Carrito { get; set; }
        public decimal Total => Carrito?.Sum(i => i.Precio * i.Cantidad) ?? 0;
        public int idUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        public string MetodoPago { get; set; }
        public TarjetaViewModel Tarjeta { get; set; }

        public List<string> Meses { get; set; }
        public List<string> Anios { get; set; }
    }

    public class TarjetaViewModel
    {
        public string Numero { get; set; }
        public string Mes { get; set; }
        public string Anio { get; set; }
        public string CVV { get; set; }
    }
}
