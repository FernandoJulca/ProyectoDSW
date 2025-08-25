namespace ApiToolify.Data.Contratos
{
    public interface IReporte
    {
        long ContarVentasPorMes(string fechaMes);
        long ContarProductosVendidosPorMes(string fechaMes);
        long ContarClientesAtendidosPorMes(string fechaMes);
        long ObtenerTotalVentas();
        long ObtenerTotalProductosVendidos();
        double ObtenerIngresosTotales();

    }
}
