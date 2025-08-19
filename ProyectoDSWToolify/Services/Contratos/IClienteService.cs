using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Services.Contratos
{
    public interface IClienteService
    {
        Task<List<Producto>> ObtenerProductosAsync(List<int> categorias, string orden, int pagina);
        Task<ProductoDetallesViewModel> ObtenerProductoPorIdAsync(int id);
        Task<List<Venta>> ObtenerVentasClienteAsync(int idCliente);
        Task<IndexViewModel> ObtenerResumenAsync();
        Task<List<Categoria>> ObtenerCategorias();
        Task<int> ContarProductosAsync(List<int> categorias);

    }
}
