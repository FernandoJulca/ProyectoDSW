using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface IVenta
    {
        Venta generarVentaCliente(Venta v);
        List<Venta> obtenerPorCliente(int id);
    }
}
