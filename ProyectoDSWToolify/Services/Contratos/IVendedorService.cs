using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Services.Contratos
{
    public interface IVendedorService
    {
        Task<List<Categoria>> ListCategoriasAsync();
        Task<List<Producto>> ListProductosVendedorAsync();
        Task<List<Venta>> ObtenerLstVentasAsync(int idUser);
        Task<List<Venta>> ObtenerLstPedidosAsync();
        Task<Venta> ObtenerVentaPorId(int idVenta);
        Task EditarEstadoVenta(int idVenta, string nuevoEstado);
    }
}
