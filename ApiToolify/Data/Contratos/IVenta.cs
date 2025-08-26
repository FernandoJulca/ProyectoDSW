using ApiToolify.Models.DTO;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface IVenta
    {
        Venta generarVentaCliente(VentaDTO v);
        List<Venta> obtenerPorCliente(int id);
        Venta obtenerVentaPorCliente(int idVenta, int idUsuario);
        List<Venta> obtenerVentasRemota();
        void CambiarEstadoVenta(int idVenta, string estado);
        int ContarRemotas(string estado);
        List<Venta> obtenerVentasRemotaPendientes();
    }
}
