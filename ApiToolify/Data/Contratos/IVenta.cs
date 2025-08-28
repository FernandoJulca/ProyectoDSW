using ApiToolify.Models.DTO;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface IVenta
    {
        //CLIENTE
        Venta generarVentaCliente(VentaDTO v);
        List<Venta> obtenerPorCliente(int id);

        //COMPARTIDO
        Venta obtenerVentaPorUsuario(int idVenta, int idUsuario); //metodo compartido pa los detalles de la venta en vista

        //VENDEDOR
        Venta generarVentaVendedor(VentaDTO v);
        List<Venta> obtenerPorVendedor(int id); 
        Venta obtenerVentaPorId(int id);
        List<Venta> obtenerLstPedidos();
        Venta editarEstadoVenta(int idVenta, string nuevoEstado);

        //REPARTIDOR
        List<Venta> obtenerVentasRemota();
        void CambiarEstadoVenta(int idVenta, string estado);
        int ContarRemotas(string estado);
        List<Venta> obtenerVentasRemotaPendientes();

    }
}
