using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Services.Contratos
{
    public interface IVentaService
    {
        //Cliente
        Task<VentaViewModel> ConfirmarCompra(VentaViewModel v);
        Task<Venta> ObtenerVentaPorUsuario(int idVenta);

        //Vendedor
        Task<VentaViewModel> GenerarVentaVendedor(VentaViewModel v);

        //Compartido
        Task<byte[]> DescargarVentaPdf(int idUsuario, int idVenta);


        //Repartidor
        Task<List<Venta>> ListarVentasRemotas();
        Task<List<Venta>> ListarVentasRemotasPendientes();
        Task ActualizarEstadoVenta(int idVenta);
        Task GenerarEstadoVenta(int idVenta);
        Task CancelarEstadoVenta(int idVenta);
        Task<int> ContarPendientes();
        Task<int> ContarGeneradas();
    }
}
