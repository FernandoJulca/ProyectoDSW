using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface IProducto
    {
        List<Producto> listProductosCliente();
        Producto obtenerPorId(int id);

        int contadorProductos();

        //Vendedor
        List<Producto> listProductosVendedor();

    }
}
