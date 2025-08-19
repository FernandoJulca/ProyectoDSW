using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Services.Contratos
{
    public interface IVentaService
    {
        Task<VentaViewModel> ConfirmarCompra(VentaViewModel v);
        Task<byte[]> DescargarVentaPdf(int idCliente, int idVenta);
    }
}
