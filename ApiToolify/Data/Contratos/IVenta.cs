using ApiToolify.Models.DTO;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface IVenta
    {
        //CLIENTE
        Venta generarVentaCliente(VentaDTO v);
        List<Venta> obtenerPorCliente(int id);
        Venta obtenerVentaPorUsuario(int idVenta, int idUsuario); //metodo compartido

        //VENDEDOR
        Venta generarVentaVendedor(VentaDTO v);
        List<Venta> obtenerPorVendedor(int id);
        Venta obtenerVentaPorId(int id);

        List<Venta> obtenerLstPedidos();
        Venta editarEstadoVenta(int idVenta, string nuevoEstado);


    }
}
