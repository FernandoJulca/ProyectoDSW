using ApiToolify.Models.DTO;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface IVenta
    {
        Venta generarVentaCliente(VentaDTO v);
        List<Venta> obtenerPorCliente(int id);
        Venta obtenerVentaPorUsuario(int idVenta, int idUsuario);
        List<Venta> obtenerPorVendedor(int id);
        Venta obtenerVentaPorId(int id);

    }
}
