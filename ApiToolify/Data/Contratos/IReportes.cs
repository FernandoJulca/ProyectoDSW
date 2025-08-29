using ApiToolify.Models.DTO;

namespace ApiToolify.Data.Contratos
{
    public interface IReportes
    {
        List<ListadoVentaFechaAndTipoVentaDTO>
            ListadoPorMesAndTipoVenta(DateTime fechaInicio, DateTime fechaFin, string tipoVenta);

        List<ListarProductosPorCategoriaDTO>
            ListarProductosPorCategoria(int idCategoria, string orden);
    }
}
