using ApiToolify.Models.DTO;

namespace ApiToolify.Data.Contratos
{
    public interface IGrafico
    {
        List<CategoriaProductoDTO> CategoriaProducto(string consulta);
        List<ProveedorProductoDTO> ProveedorProducto(string consulta);
        List<VentaPorMesDTO> VentaPorMes(string consulta);
        List<VentaPorMesAndTipoVentaDTO> VentaPorMesAndTipoVenta(string consulta);
        List<VentaPorDistritoDTO> VentaPorDistrito(string consulta);



    }
}
