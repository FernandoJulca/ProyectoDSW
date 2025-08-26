using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Services.Contratos
{
    public interface IVentaService
    {
        Task<VentaViewModel> ConfirmarCompra(VentaViewModel v);
        Task<byte[]> DescargarVentaPdf(int idCliente, int idVenta);
        Task<List<Venta>> ListarVentasRemotas();
        Task<List<Venta>> ListarVentasRemotasPendientes();
        Task<Venta> ObtenerVentaPorUsuario(int idVenta);
        Task ActualizarEstadoVenta(int idVenta);
        Task GenerarEstadoVenta(int idVenta);
        Task CancelarEstadoVenta(int idVenta);
        Task<int> ContarPendientes();
        Task<int> ContarGeneradas();
    }
}
